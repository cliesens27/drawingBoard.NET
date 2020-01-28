using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using drawingBoard.GUI;
using drawingBoard.src.drawing;

namespace drawingBoard.drawing {
	public class DrawingBoard {
		private readonly int screenX = -1;
		private readonly int screenY = -1;
		private RectangleMode rectMode;
		private Font currentFont;
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

		public KeyPressedMethod KeyPressed {
			get => mainForm.KeyPressed;
			set => mainForm.KeyPressed = value;
		}

		public KeyReleasedMethod KeyReleased {
			get => mainForm.KeyReleased;
			set => mainForm.KeyReleased = value;
		}

		public MousePressedMethod MousePressed {
			get => mainForm.MousePressed;
			set => mainForm.MousePressed = value;
		}

		public MouseReleasedMethod MouseReleased {
			get => mainForm.MouseReleased;
			set => mainForm.MouseReleased = value;
		}

		public MouseDraggedMethod MouseDragged {
			get => mainForm.MouseDragged;
			set => mainForm.MouseDragged = value;
		}

		public double TargetFrameRate {
			get => mainForm.TargetFrameRate;
			set => mainForm.TargetFrameRate = value;
		}

		public string Title {
			get => mainForm.Text;
			set => mainForm.Text = value;
		}

		public double FrameRate => mainForm.FrameRate;
		public double TotalElapsedTime => mainForm.TotalElapsedTime;
		public int FrameCount => mainForm.TotalFrameCount;
		public int MouseX => Control.MousePosition.X - (mainForm.Location.X + 5);
		public int MouseY => Control.MousePosition.Y - (mainForm.Location.Y + 25);
		public int Xmin => 0;
		public int Ymin => 0;
		public int Xcenter => Width / 2;
		public int Ycenter => Height / 2;
		public int Xmax => Width;
		public int Ymax => Height;

		private DrawingBoard() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			RectMode(RectangleMode.CENTER);
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

		public void RectMode(RectangleMode mode) => rectMode = mode;

		public void Draw() {
			if (DrawMethod == null) {
				throw new Exception("Error, you must set the DrawMethod property before calling Draw()");
			}

			Application.Run(mainForm);
		}

		public void Stroke(Color color) => currentPen.Color = color;

		public void Stroke(byte grey) => Stroke(grey, grey, grey);

		public void Stroke(byte grey, byte a) => Stroke(grey, grey, grey, a);

		public void Stroke(byte r, byte g, byte b) => Stroke(Color.FromArgb(r, g, b));

		public void Stroke(byte r, byte g, byte b, byte a) => Stroke(Color.FromArgb(a, r, g, b));

		public void StrokeWidth(float w) => currentPen.Width = w;

		public void NoStroke() => currentPen.Color = Color.FromArgb(0, 0, 0, 0);

		public void Fill(Color color) {
			fill = true;
			currentBrush.Color = color;
		}

		public void Fill(byte grey) => Fill(grey, grey, grey);

		public void Fill(byte grey, byte a) => Fill(grey, grey, grey, a);

		public void Fill(byte r, byte g, byte b) => Fill(Color.FromArgb(r, g, b));

		public void Fill(byte r, byte g, byte b, byte a) => Fill(Color.FromArgb(a, r, g, b));

		public void NoFill() => fill = false;

		public void Point(Graphics g, float x, float y) => Circle(g, x, y, 1);

		public void Line(Graphics g, float x1, float y1, float x2, float y2) =>
			g.DrawLine(currentPen, x1, y1, x2, y2);

		public void Rectangle(Graphics g, Rectangle rect) {
			if (fill) {
				g.FillRectangle(currentBrush, rect);
			}

			g.DrawRectangle(currentPen, rect);
		}

		public void Rectangle(Graphics g, float x, float y, float w, float h) {
			switch (rectMode) {
				case RectangleMode.CORNER:
					if (fill) {
						g.FillRectangle(currentBrush, x, y, w, h);
					}

					g.DrawRectangle(currentPen, x, y, w, h);
					break;
				case RectangleMode.CORNERS:
					if (fill) {
						g.FillRectangle(currentBrush, x, y, w - x, h - y);
					}

					g.DrawRectangle(currentPen, x, y, w - x, h - y);
					break;
				case RectangleMode.CENTER:
					if (fill) {
						g.FillRectangle(currentBrush, x - w, y - h, 2 * w, 2 * h);
					}

					g.DrawRectangle(currentPen, x - w, y - h, 2 * w, 2 * h);
					break;
			}
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

		public void Font(Font font) => currentFont = font;

		public void DrawString(Graphics g, string str, float x, float y)
			=> g.DrawString(str, currentFont, currentBrush, x, y);

		public void DrawString(Graphics g, string str, float x, float y, bool bold, bool italic) {
			FontStyle style = (bold ? FontStyle.Bold : 0) | (italic ? FontStyle.Italic : 0);
			Font font = new Font(currentFont.FontFamily, currentFont.Size, style);
			g.DrawString(str, font, currentBrush, x, y);
		}

		public void SaveToPNG(string path) {
			Bitmap fullBitmap = new Bitmap(mainForm.Width, mainForm.Height);
			mainForm.DrawToBitmap(fullBitmap, new Rectangle(System.Drawing.Point.Empty, mainForm.Size));

			Point clientOrigin = mainForm.PointToScreen(System.Drawing.Point.Empty);
			Rectangle clientRect = new Rectangle(new Point(clientOrigin.X - mainForm.Bounds.X, clientOrigin.Y - mainForm.Bounds.Y), mainForm.ClientSize);

			Bitmap clientAreaBitmap = fullBitmap.Clone(clientRect, PixelFormat.Format32bppArgb);
			fullBitmap.Dispose();

			clientAreaBitmap.Save(path, ImageFormat.Png);
			clientAreaBitmap.Dispose();
		}

		private void SetDefaultSettings() {
			DrawMethod = null;
			KeyPressed = null;
			KeyReleased = null;
			MousePressed = null;
			MouseReleased = null;
			MouseDragged = null;

			TargetFrameRate = 30;
			Title = "Application";

			currentFont = new Font("cambria", 12);
			currentPen = new Pen(Color.Black, 1);
			currentBrush = new SolidBrush(Color.Black);
			fill = false;
			currentRotation = 0;
			currentTranslationX = 0;
			currentTranslationY = 0;
		}
	}
}
