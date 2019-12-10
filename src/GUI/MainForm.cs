using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace drawingBoard.GUI {
	public delegate void DrawMethod(Graphics graphics);
	public delegate void KeyPressedMethod(char key);
	public delegate void KeyReleasedMethod(char key);
	public delegate void MousePressedMethod();
	public delegate void MouseReleasedMethod();
	public delegate void MouseDraggedMethod();

	public partial class MainForm : Form {
		private readonly Stopwatch stopwatch;
		private double lastRedrawTime;

		public DrawMethod Draw { get; set; }
		public KeyPressedMethod KeyPressed { get; set; }
		public KeyReleasedMethod KeyReleased { get; set; }
		public MousePressedMethod MousePressed { get; set; }
		public MouseReleasedMethod MouseReleased { get; set; }
		public MouseDraggedMethod MouseDragged { get; set; }
		public double FrameRate { get; set; }
		public double TargetFrameRate { get; set; }
		public double TotalElapsedTime { get; private set; }
		public int TotalFrameCount { get; private set; }

		private List<char> PressedKeys { get; set; }
		private List<char> ReleasedKeys { get; set; }
		private bool MouseDown { get; set; }
		private double CurrentElapsedTime { get; set; }
		private int CurrentFrameCount { get; set; }

		private MainForm() {
			InitializeComponent();
			StartPosition = FormStartPosition.CenterScreen;
			Application.Idle += Run;

			PressedKeys = new List<char>();
			ReleasedKeys = new List<char>();

			stopwatch = new Stopwatch();
			stopwatch.Start();

			lastRedrawTime = 0;
			TotalFrameCount = 0;
		}

		public MainForm(int width, int height) : this() {
			ClientSize = new Size(width, height);
			mainPictureBox.Size = new Size(width, height);
		}

		public MainForm(int width, int height, int x, int y) : this(width, height) => Location = new Point(x, y);

		private void Run(object sender, EventArgs e) {
			while (IsIdle()) {
				TotalElapsedTime = stopwatch.ElapsedTicks / (double) Stopwatch.Frequency;

				if (TotalElapsedTime - lastRedrawTime > 1.0 / TargetFrameRate) {
					CheckKeyboardInput();
					CheckMouseInput();

					lastRedrawTime = TotalElapsedTime;
					mainPictureBox.Invalidate();
					TotalFrameCount++;

					ComputeFrameRate();
				}
			}
		}

		private void CheckKeyboardInput() {
			if (KeyPressed != null || KeyReleased != null) {
				if (KeyPressed != null) {
					foreach (char key in PressedKeys) {
						KeyPressed(key);
					}
				}

				if (KeyReleased != null) {
					foreach (char key in ReleasedKeys) {
						KeyReleased(key);
					}
				}

				ReleasedKeys.Clear();

				foreach (char key in PressedKeys) {
					ReleasedKeys.Add(key);
				}

				PressedKeys.Clear();
			}
		}

		private void CheckMouseInput() {
			if (MouseDragged != null && MouseDown) {
				MouseDragged();
			}
		}

		private void ComputeFrameRate() {
			const double INTERVAL = 0.25;
			const int MIN_NB_FRAMES = 10;

			if (TotalElapsedTime > 0) {
				CurrentFrameCount++;

				if (TotalElapsedTime - CurrentElapsedTime > INTERVAL && CurrentFrameCount > MIN_NB_FRAMES) {
					FrameRate = CurrentFrameCount / (TotalElapsedTime - CurrentElapsedTime);
					CurrentElapsedTime = TotalElapsedTime;
					CurrentFrameCount = 0;
				}
			}
			else {
				FrameRate = 0;
			}
		}

		private void mainPictureBox_Paint(object sender, PaintEventArgs e) => Draw(e.Graphics);

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e) => PressedKeys.Add(e.KeyChar);

		private void mainPictureBox_MouseDown(object sender, MouseEventArgs e) {
			MouseDown = true;

			if (MousePressed != null) {
				MousePressed();
			}
		}

		private void mainPictureBox_MouseUp(object sender, MouseEventArgs e) {
			MouseDown = false;

			if (MouseReleased != null) {
				MouseReleased();
			}
		}

		private bool IsIdle() => PeekMessage(out Message result, IntPtr.Zero, 0, 0, 0) == 0;

		[StructLayout(LayoutKind.Sequential)]
		public struct Message {
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
