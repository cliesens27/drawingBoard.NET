using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace drawingBoard.GUI {
	public static class DrawingBoard {
		public static DrawMethod DrawMethod { get; set; } = null;
		public static double TargetFrameRate { get; set; }
		public static int Xmin => 0;
		public static int Ymin => 0;
		public static int Xmax => frameWidth;
		public static int Ymax => frameHeight;

		private static DrawingForm drawingForm;
		private static int frameWidth = -1;
		private static int frameHeight = -1;
		private static int screenX = -1;
		private static int screenY = -1;

		private static void Init() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}

		public static void Init(int width, int height) => Init(width, height, -1, -1);

		public static void Init(int width, int height, int x, int y) {
			Init();

			frameWidth = width;
			frameHeight = height;
			screenX = x;
			screenY = y;
		}

		public static void Draw() {
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

		public static void SaveToPNG(string path) {
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
