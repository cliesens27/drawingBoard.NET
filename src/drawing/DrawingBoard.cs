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
		private Pen currentPen;
		private SolidBrush currentBrush;
		private bool fill;
		private float currentRotation;
		private float currentTranslationX;
		private float currentTranslationY;

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

		public double FrameRate => mainForm.FrameCount / mainForm.TotalElapsedTime;
		public double TotalElapsedTime => mainForm.TotalElapsedTime;
		public int FrameCount => mainForm.FrameCount;

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

			Width = width;
			Height = height;
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

		public void Stroke(Color color) => currentPen.Color = color;

		public void Stroke(int grey) => Stroke(grey, grey, grey);

		public void Stroke(int grey, int a) => Stroke(grey, grey, grey, a);

		public void Stroke(int r, int g, int b) => Stroke(Color.FromArgb(r, g, b));

		public void Stroke(int r, int g, int b, int a) => Stroke(Color.FromArgb(a, r, g, b));

		public void StrokeWidth(float w) => currentPen.Width = w;

		public void Fill(Color color) {
			fill = true;
			currentBrush.Color = color;
		}

		public void Fill(int grey) => Fill(grey, grey, grey);

		public void Fill(int grey, int a) => Fill(grey, grey, grey, a);

		public void Fill(int r, int g, int b) => Fill(Color.FromArgb(r, g, b));

		public void Fill(int r, int g, int b, int a) => Fill(Color.FromArgb(a, r, g, b));

		public void NoFill() => fill = false;

		public void Line(Graphics g, float x1, float y1, float x2, float y2) =>
			g.DrawLine(currentPen, x1, y1, x2, y2);

		public void Rectangle(Graphics g, float x, float y, float w, float h) {
			if (fill) {
				g.FillRectangle(currentBrush, x - w, y - h, 2 * w, 2 * h);
			}

			g.DrawRectangle(currentPen, x - w, y - h, 2 * w, 2 * h);
		}

		public void Square(Graphics g, float x, float y, float r) => Rectangle(g, x, y, r, r);

		public void Ellipse(Graphics g, float x, float y, float rx, float ry) {
			if (fill) {
				g.FillEllipse(currentBrush, x - rx, y - ry, 2 * rx, 2 * ry);
			}

			g.DrawEllipse(currentPen, x - rx, y - ry, 2 * rx, 2 * ry);
		}

		public void Circle(Graphics g, float x, float y, float r) => Ellipse(g, x, y, r, r);

		public void Rotate(Graphics g, float degrees) {
			currentRotation += degrees;
			g.RotateTransform(degrees);
		}

		public void Translate(Graphics g, float dx, float dy) {
			currentTranslationX += dx;
			currentTranslationY += dy;
			g.TranslateTransform(dx, dy);
		}

		public void UndoRotation(Graphics g) {
			g.RotateTransform(-currentRotation);
			currentRotation = 0;
		}

		public void UndoTranslation(Graphics g) {
			g.TranslateTransform(-currentTranslationX, -currentTranslationY);
			currentTranslationX = 0;
			currentTranslationY = 0;
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

			currentPen = new Pen(Color.Black, 1);
			currentBrush = new SolidBrush(Color.Black);
			fill = false;
			currentRotation = 0;
			currentTranslationX = 0;
			currentTranslationY = 0;
		}
	}
}
