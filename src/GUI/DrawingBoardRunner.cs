using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace drawingBoard.GUI {
	public static class DrawingBoardRunner {
		public static DrawMethod DrawMethod { get; set; } = null;
		public static int Width { get; private set; } = -1;
		public static int Height { get; private set; } = -1;
		public static int X { get; private set; } = -1;
		public static int Y { get; private set; } = -1;
		private static DrawingBoard db;

		public static double TargetFrameRate {
			get => db.TargetFrameRate;
			set => db.TargetFrameRate = value;
		}

		private static void Init() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}

		public static void Init(int width, int height) => Init(width, height, -1, -1);

		public static void Init(int width, int height, int x, int y) {
			Init();

			Width = width;
			Height = height;
			X = x;
			Y = y;
		}

		public static void Draw() {
			if (DrawMethod == null) {
				throw new Exception("Error, you must set the DrawMethod property before calling Draw()");
			}

			if (X != -1 && Y != -1) {
				db = new DrawingBoard(Width, Height, X, Y, DrawMethod);
			}
			else {
				db = new DrawingBoard(Width, Height, DrawMethod);
			}

			Application.Run(db);
		}

		public static void SaveToPNG(string path) {
			Bitmap bitmap = new Bitmap(db.Width, db.Height);
			db.DrawToBitmap(bitmap, new Rectangle(Point.Empty, db.Size));

			Point p = db.PointToScreen(Point.Empty);
			Rectangle clientRect = new Rectangle(new Point(p.X - db.Bounds.X, p.Y - db.Bounds.Y), db.ClientSize);
			Bitmap clientAreBitmap = bitmap.Clone(clientRect, PixelFormat.Format32bppArgb);
			bitmap.Dispose();
			clientAreBitmap.Save(path, ImageFormat.Png);
		}
	}
}
