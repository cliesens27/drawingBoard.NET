using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using drawingBoard.GUI;

namespace drawingBoard.drawing {
	public class DrawingBoard {
		private readonly int screenX = -1;
		private readonly int screenY = -1;
		private MainForm mainForm;

		public int Width { get; private set; } = -1;
		public int Height { get; private set; } = -1;

		public DrawMethod DrawMethod {
			get => mainForm.Draw;
			set => mainForm.Draw = value;
		}

		public double TargetFrameRate {
			get => mainForm.TargetFrameRate;
			set => mainForm.TargetFrameRate = value;
		}

		public string Title {
			get => mainForm.Text;
			set => mainForm.Text = value;
		}

		public int Xmin => 0;
		public int Ymin => 0;
		public int Xcenter => Width / 2;
		public int Ycenter => Height / 2;
		public int Xmax => Width;
		public int Ymax => Height;

		private DrawingBoard() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}

		public DrawingBoard(int width, int height) : this(width, height, -1, -1) { }

		public DrawingBoard(int width, int height, int x, int y) : this() {
			if (screenX != -1 && screenY != -1) {
				mainForm = new MainForm(width, height, x, y);
			}
			else {
				mainForm = new MainForm(width, height);
			}

			this.Width = width;
			this.Height = height;
			screenX = x;
			screenY = y;

			SetDefaultSettings();
		}

		public void Draw() {
			if (DrawMethod == null) {
				throw new Exception("Error, you must set the DrawMethod property before calling Draw()");
			}

			Application.Run(mainForm);
		}

		public void SaveToPNG(string path) {
			Bitmap fullBitmap = new Bitmap(mainForm.Width, mainForm.Height);
			mainForm.DrawToBitmap(fullBitmap, new Rectangle(Point.Empty, mainForm.Size));

			Point clientOrigin = mainForm.PointToScreen(Point.Empty);
			Rectangle clientRect = new Rectangle(new Point(clientOrigin.X - mainForm.Bounds.X, clientOrigin.Y - mainForm.Bounds.Y), mainForm.ClientSize);

			Bitmap clientAreaBitmap = fullBitmap.Clone(clientRect, PixelFormat.Format32bppArgb);
			fullBitmap.Dispose();

			clientAreaBitmap.Save(path, ImageFormat.Png);
			clientAreaBitmap.Dispose();
		}

		private void SetDefaultSettings() {
			DrawMethod = null;
			TargetFrameRate = 30;
			Title = "Application";
		}
	}
}
