using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace drawingBoard.GUI {
	public class DrawingBoard {
		public DrawMethod DrawMethod { get; set; } = null;
		public double TargetFrameRate { get; set; }
		public int Xmin => 0;
		public int Ymin => 0;
		public int Xmax => frameWidth;
		public int Ymax => frameHeight;

		private DrawingForm drawingForm;
		private int frameWidth = -1;
		private int frameHeight = -1;
		private int screenX = -1;
		private int screenY = -1;

		private DrawingBoard() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}

		public DrawingBoard(int width, int height) : this(width, height, -1, -1) { }

		public DrawingBoard(int width, int height, int x, int y) : this() {
			frameWidth = width;
			frameHeight = height;
			screenX = x;
			screenY = y;
		}

		public void Draw() {
			if (DrawMethod == null) {
				throw new Exception("Error, you must set the DrawMethod property before calling Draw()");
			}

			if (screenX != -1 && screenY != -1) {
				drawingForm = new DrawingForm(frameWidth, frameHeight, screenX, screenY, DrawMethod);
			}
			else {
				drawingForm = new DrawingForm(frameWidth, frameHeight, DrawMethod);
			}

			drawingForm.TargetFrameRate = TargetFrameRate;

			Application.Run(drawingForm);
		}

		public void SaveToPNG(string path) {
			Bitmap fullBitmap = new Bitmap(drawingForm.Width, drawingForm.Height);
			drawingForm.DrawToBitmap(fullBitmap, new Rectangle(Point.Empty, drawingForm.Size));

			Point clientOrigin = drawingForm.PointToScreen(Point.Empty);
			Rectangle clientRect = new Rectangle(new Point(clientOrigin.X - drawingForm.Bounds.X, clientOrigin.Y - drawingForm.Bounds.Y), drawingForm.ClientSize);

			Bitmap clientAreaBitmap = fullBitmap.Clone(clientRect, PixelFormat.Format32bppArgb);
			fullBitmap.Dispose();

			clientAreaBitmap.Save(path, ImageFormat.Png);
		}
	}
}
