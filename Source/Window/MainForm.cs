using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DrawingBoardNET.Drawing.Constants;
using DrawingBoardNET.Window.UI;
using Button = DrawingBoardNET.Window.UI.Button;

namespace DrawingBoardNET.Window
{
	public delegate void InitMethod();
	public delegate void DrawMethod();
	public delegate void DrawButtonMethod(Button button);
	public delegate void DrawSliderMethod(Slider slider);
	public delegate void KeyPressedMethod(char key);
	public delegate void KeyReleasedMethod(char key);
	public delegate void MousePressedMethod();
	public delegate void MouseReleasedMethod();
	public delegate void MouseDraggedMethod();
	public delegate void MouseWheelUpMethod();
	public delegate void MouseWheelDownMethod();

	internal partial class MainForm : Form
	{
		#region Fields & Properties

		internal InitMethod Init { get; set; }
		internal DrawMethod Draw { get; set; }
		internal DrawButtonMethod DrawButton { get; set; }
		internal DrawSliderMethod DrawSlider { get; set; }
		internal KeyPressedMethod KeyPressed { get; set; }
		internal KeyReleasedMethod KeyReleased { get; set; }
		internal MousePressedMethod MousePressed { get; set; }
		internal MouseReleasedMethod MouseReleased { get; set; }
		internal MouseDraggedMethod MouseDragged { get; set; }
		internal MouseWheelUpMethod MouseWheelUp { get; set; }
		internal MouseWheelDownMethod MouseWheelDown { get; set; }
		internal double TargetFrameRate { get; set; }

		internal Graphics Graphics { get; private set; }
		internal double FrameRate { get; private set; }
		internal double TotalElapsedTime { get; private set; }
		internal int TotalFrameCount { get; private set; }

		internal RectangleMode rectMode;

		private readonly List<Button> buttons;
		private readonly List<Slider> sliders;
		private readonly Stopwatch stopwatch;
		private readonly List<char> pressedKeys;
		private readonly List<char> releasedKeys;
		private readonly Queue<double> frameRateSamples;
		private bool isFirstFrame;
		private bool isMouseDragged;
		private bool isMousePressed;
		private bool isMouseReleased;
		private bool isPaused;
		private double currentElapsedTime;
		private double lastRedrawTime;
		private double sampleAccumulator;
		private int currentFrameCount;

		#endregion

		#region Constructors

		private MainForm()
		{
			InitializeComponent();
			StartPosition = FormStartPosition.CenterScreen;
			Application.Idle += Run;

			buttons = new List<Button>();
			sliders = new List<Slider>();
			pressedKeys = new List<char>();
			releasedKeys = new List<char>();
			frameRateSamples = new Queue<double>();

			stopwatch = new Stopwatch();
			stopwatch.Start();

			lastRedrawTime = 0;
			TotalFrameCount = 0;

			isFirstFrame = true;
			isMouseDragged = false;
			isMousePressed = false;
			isMouseReleased = false;
			isPaused = false;
		}

		internal MainForm(int width, int height) : this()
		{
			ClientSize = new Size(width, height);
			mainPictureBox.Size = new Size(width, height);
		}

		internal MainForm(int width, int height, int x, int y)
			: this(width, height) => Location = new Point(x, y);

		#endregion

		internal void Pause() => isPaused = true;

		internal void Resume() => isPaused = false;

		internal void AddButton(Button button) => buttons.Add(button);

		internal void AddSlider(Slider slider) => sliders.Add(slider);

		private void Run(object sender, EventArgs e)
		{
			while (IsIdle())
			{
				TotalElapsedTime = stopwatch.ElapsedTicks / (double) Stopwatch.Frequency;

				if (TotalElapsedTime - lastRedrawTime > 1.0 / TargetFrameRate)
				{
					mainPictureBox.Invalidate();
					lastRedrawTime = TotalElapsedTime;
					TotalFrameCount++;

					ComputeFrameRate();
				}
			}
		}

		private void CheckKeyboardInput()
		{
			if (KeyPressed != null || KeyReleased != null)
			{
				if (KeyPressed != null)
				{
					foreach (char key in pressedKeys)
					{
						KeyPressed(key);
					}
				}

				if (KeyReleased != null)
				{
					foreach (char key in releasedKeys)
					{
						KeyReleased(key);
					}
				}

				releasedKeys.Clear();

				foreach (char key in pressedKeys)
				{
					releasedKeys.Add(key);
				}

				pressedKeys.Clear();
			}
		}

		private void CheckMouseInput()
		{
			int mx = PointToClient(MousePosition).X;
			int my = PointToClient(MousePosition).Y;

			HoverButtons(mx, my);

			if (isMouseDragged)
			{
				MouseDragged?.Invoke();

				UpdateSliders(mx);
			}

			if (isMousePressed)
			{
				MousePressed?.Invoke();
				isMousePressed = false;

				LockSliders(mx, my);
				PressButtons(mx, my);
			}

			if (isMouseReleased)
			{
				MouseReleased?.Invoke();
				isMouseReleased = false;

				UnlockSliders(mx, my);
				TriggerButtons(mx, my);
			}
		}

		private void HoverButtons(int mx, int my)
		{
			foreach (Button b in buttons)
			{
				b.IsHovered = b.IsSelected(rectMode, mx, my);
			}
		}

		private void PressButtons(int mx, int my)
		{
			foreach (Button b in buttons)
			{
				if (!b.IsPressed)
				{
					b.IsPressed = b.IsSelected(rectMode, mx, my);
				}
			}
		}

		private void TriggerButtons(int mx, int my)
		{
			foreach (Button b in buttons)
			{
				if (b.IsPressed && b.IsSelected(rectMode, mx, my))
				{
					b.Action();
					b.IsPressed = false;
				}
			}
		}

		private void UpdateSliders(int mx)
		{
			foreach (Slider s in sliders)
			{
				s.Update(mx);
			}
		}

		private void LockSliders(int mx, int my)
		{
			foreach (Slider s in sliders)
			{
				if (s.IsSelected(mx, my))
				{
					s.Lock();
				}
			}
		}

		private void UnlockSliders(int mx, int my)
		{
			foreach (Slider s in sliders)
			{
				if (s.IsSelected(mx, my))
				{
					s.Unlock();
				}
			}
		}

		private void ComputeFrameRate()
		{
			const double SMOOTHING = 0.9;
			const int MIN_NB_FRAMES = 5;
			const int WINDOW_SIZE = 5;

			if (TotalElapsedTime > 0)
			{
				double timeSinceLast = TotalElapsedTime - currentElapsedTime;
				currentFrameCount++;

				if (currentFrameCount >= MIN_NB_FRAMES)
				{
					double newSample = currentFrameCount / timeSinceLast;
					currentElapsedTime = TotalElapsedTime;
					currentFrameCount = 0;

					sampleAccumulator += newSample;
					frameRateSamples.Enqueue(newSample);

					if (frameRateSamples.Count > WINDOW_SIZE)
					{
						sampleAccumulator -= frameRateSamples.Dequeue();
					}

					double newFrameRate = sampleAccumulator / frameRateSamples.Count;
					FrameRate = (newFrameRate * SMOOTHING) + (FrameRate * (1 - SMOOTHING));
				}
			}
			else
			{
				FrameRate = TargetFrameRate;
			}
		}

		private void mainPictureBox_Paint(object sender, PaintEventArgs e)
		{
			Graphics = e.Graphics;

			if (isFirstFrame)
			{
				Init?.Invoke();
				isFirstFrame = false;
			}

			if (!isPaused)
			{
				Draw();
				CheckKeyboardInput();
				CheckMouseInput();

				foreach (Slider s in sliders)
				{
					DrawSlider?.Invoke(s);
				}

				foreach (Button b in buttons)
				{
					DrawButton?.Invoke(b);
				}
			}
		}

		private void mainPictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			isMousePressed = true;
			isMouseDragged = true;
		}

		private void mainPictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			isMouseReleased = true;
			isMouseDragged = false;
		}

		private void mainPictureBox_MouseWheel(object sender, MouseEventArgs e)
		{
			int delta = e.Delta;

			if (delta > 0)
			{
				MouseWheelUp?.Invoke();
			}
			else if (delta < 0)
			{
				MouseWheelDown?.Invoke();
			}
		}

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e) => pressedKeys.Add(e.KeyChar);

		private bool IsIdle() => PeekMessage(out Message result, IntPtr.Zero, 0, 0, 0) == 0;

		[StructLayout(LayoutKind.Sequential)]
		internal struct Message
		{
			public IntPtr handle;
			public int message;
			public IntPtr wParameter;
			public IntPtr lParameter;
			public int time;
			public Point location;
		}

		[DllImport("user32.dll")]
		internal static extern int PeekMessage(out Message msg, IntPtr window, int filterMin, int filterMax, int removeMsg);
	}
}
