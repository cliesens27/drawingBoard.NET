using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DrawingBoardNET.Drawing.Constants.Window
{
	public delegate void InitMethod();
	public delegate void DrawMethod();
	public delegate void KeyPressedMethod(char key);
	public delegate void KeyReleasedMethod(char key);
	public delegate void MousePressedMethod();
	public delegate void MouseReleasedMethod();
	public delegate void MouseDraggedMethod();

	public partial class MainForm : Form
	{
		#region Fields & Properties

		internal InitMethod Init { get; set; }
		internal DrawMethod Draw { get; set; }
		internal KeyPressedMethod KeyPressed { get; set; }
		internal KeyReleasedMethod KeyReleased { get; set; }
		internal MousePressedMethod MousePressed { get; set; }
		internal MouseReleasedMethod MouseReleased { get; set; }
		internal MouseDraggedMethod MouseDragged { get; set; }
		internal double TargetFrameRate { get; set; }

		internal Graphics Graphics { get; private set; }
		internal double FrameRate { get; private set; }
		internal double TotalElapsedTime { get; private set; }
		internal int TotalFrameCount { get; private set; }

		private readonly Stopwatch stopwatch;
		private readonly bool redrawEveryFrame;
		private List<char> pressedKeys;
		private List<char> releasedKeys;
		private Bitmap previousFrameBuffer;
		private bool isFirstFrame;
		private bool isMouseDragged;
		private bool isMousePressed;
		private bool isMouseReleased;
		private bool isPaused;
		private double currentElapsedTime;
		private double lastRedrawTime;
		private int currentFrameCount;

		#endregion

		#region Constructors

		private MainForm()
		{
			InitializeComponent();
			StartPosition = FormStartPosition.CenterScreen;
			Application.Idle += Run;

			previousFrameBuffer = null;
			pressedKeys = new List<char>();
			releasedKeys = new List<char>();

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

		public MainForm(int width, int height, bool redrawEveryFrame) : this()
		{
			this.redrawEveryFrame = redrawEveryFrame;
			ClientSize = new Size(width, height);
			mainPictureBox.Size = new Size(width, height);
		}

		public MainForm(int width, int height, int x, int y, bool redrawEveryFrame)
			: this(width, height, redrawEveryFrame) => Location = new Point(x, y);

		#endregion

		public void Pause() => isPaused = true;

		public void Resume() => isPaused = false;

		private void Run(object sender, EventArgs e)
		{
			while (IsIdle())
			{
				TotalElapsedTime = stopwatch.ElapsedTicks / (double) Stopwatch.Frequency;

				if (TotalElapsedTime - lastRedrawTime > 1.0 / TargetFrameRate)
				{
					if (previousFrameBuffer == null)
					{
						previousFrameBuffer = new Bitmap(Width, Height);
					}

					if (redrawEveryFrame)
					{
						DrawToBitmap(previousFrameBuffer, new Rectangle(Point.Empty, Size));
					}

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
			if (MouseDragged != null && isMouseDragged)
			{
				MouseDragged();
			}

			if (MousePressed != null && isMousePressed)
			{
				MousePressed();
				isMousePressed = false;
			}

			if (MouseReleased != null && isMouseReleased)
			{
				MouseReleased();
				isMouseReleased = false;
			}
		}

		private void ComputeFrameRate()
		{
			const double MIN_INTERVAL = 0.25;
			const int MIN_NB_FRAMES = 10;

			if (TotalElapsedTime > 0)
			{
				currentFrameCount++;

				if (TotalElapsedTime - currentElapsedTime > MIN_INTERVAL && currentFrameCount > MIN_NB_FRAMES)
				{
					FrameRate = currentFrameCount / (TotalElapsedTime - currentElapsedTime);
					currentElapsedTime = TotalElapsedTime;
					currentFrameCount = 0;
				}
			}
			else
			{
				FrameRate = 0;
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
				if (previousFrameBuffer != null && redrawEveryFrame)
				{
					Graphics.DrawImage(previousFrameBuffer, PointToScreen(Point.Empty));
					Graphics.DrawImage(previousFrameBuffer, -8, -31);
				}

				Draw();
				CheckKeyboardInput();
				CheckMouseInput();
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

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e) => pressedKeys.Add(e.KeyChar);

		private bool IsIdle() => PeekMessage(out Message result, IntPtr.Zero, 0, 0, 0) == 0;

		[StructLayout(LayoutKind.Sequential)]
		public struct Message
		{
			public IntPtr handle;
			public int message;
			public IntPtr wParameter;
			public IntPtr lParameter;
			public int time;
			public Point location;
		}

		[DllImport("user32.dll")]
		public static extern int PeekMessage(out Message msg, IntPtr window, int filterMin, int filterMax, int removeMsg);
	}
}
