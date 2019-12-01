using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace drawingBoard.GUI {
	public delegate void DrawMethod(Graphics graphics);

	public partial class MainForm : Form {
		private readonly Stopwatch stopwatch;
		private readonly double startTime;
		private double lastTime;

		public DrawMethod Draw { get; set; }
		public double TargetFrameRate { get; set; }

		private MainForm() {
			InitializeComponent();

			ClientSize = new Size(1, 1);
			StartPosition = FormStartPosition.CenterScreen;

			Application.Idle += Run;

			stopwatch = new Stopwatch();
			stopwatch.Start();

			startTime = lastTime = 0;
		}

		public MainForm(int width, int height) : this() {
			ClientSize = new Size(width, height);
			mainPictureBox.Size = new Size(width, height);
		}

		public MainForm(int width, int height, int x, int y) : this(width, height) => Location = new Point(x, y);

		private void Run(object sender, EventArgs e) {
			while (IsIdle()) {
				double elapsedTime = stopwatch.ElapsedTicks / (double) Stopwatch.Frequency;

				if (elapsedTime - lastTime > 1.0 / TargetFrameRate) {
					lastTime = elapsedTime;
					mainPictureBox.Invalidate();
				}
			}
		}

		private bool IsIdle() => PeekMessage(out Message result, IntPtr.Zero, 0, 0, 0) == 0;

		private void mainPictureBox_Paint(object sender, PaintEventArgs e) => Draw(e.Graphics);

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
