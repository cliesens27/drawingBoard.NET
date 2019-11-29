using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace drawingBoard.GUI {
	public delegate void DrawMethod(Graphics graphics);

	public partial class DrawingBoard : Form {
		public DrawMethod Draw { get; set; } = null;
		public readonly double TARGET_FRAME_RATE;
		private readonly Stopwatch stopwatch;
		private readonly double startTime;
		private double lastTime;

		private DrawingBoard() : this(null) { }

		private DrawingBoard(DrawMethod drawMethod) {
			InitializeComponent();

			ClientSize = new Size(1, 1);
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Program";
			Draw = drawMethod;

			Application.Idle += Run;

			stopwatch = new Stopwatch();
			stopwatch.Start();

			startTime = lastTime = 0;
		}

		public DrawingBoard(int width, int height, DrawMethod drawMethod) : this(drawMethod) {
			ClientSize = new Size(width, height);
			SetComponents();
		}

		public DrawingBoard(int width, int height, int x, int y, DrawMethod drawMethod) : this(width, height, drawMethod) {
			Location = new Point(x, y);
		}

		private void Run(object sender, EventArgs e) {
			while (IsApplicationIdle()) {
				double elapsedTime = stopwatch.ElapsedTicks / (double) Stopwatch.Frequency;

				if (elapsedTime - lastTime > 1.0 / TARGET_FRAME_RATE) {
					lastTime = elapsedTime;
					mainPictureBox.Invalidate();
					Console.WriteLine("DRAW");
				}
			}
		}

		private bool IsApplicationIdle() {
			NativeMessage result;
			return PeekMessage(out result, IntPtr.Zero, (uint) 0, (uint) 0, (uint) 0) == 0;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NativeMessage {
			public IntPtr Handle;
			public uint Message;
			public IntPtr WParameter;
			public IntPtr LParameter;
			public uint Time;
			public Point Location;
		}

		[DllImport("user32.dll")]
		public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);

		private void SetComponents() {
			mainPictureBox.Size = new Size(Size.Width, Size.Height);
		}

		private void mainPictureBox_Paint(object sender, PaintEventArgs e) {
			if (Draw != null) {
				Draw(e.Graphics);
			}
		}
	}
}
