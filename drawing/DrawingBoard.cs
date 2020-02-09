using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DrawingBoard.Drawing.Constants.Render;

namespace DrawingBoard.Drawing.Constants.Drawing {
	public class DrawingBoard {

		#region Fields & Properties

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

		private Graphics Graphics => mainForm.Graphics;

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

		#endregion

		#region Constructors

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

		#endregion

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
			RectMode(RectangleMode.CENTER);

			currentRotation = 0;
			currentTranslationX = 0;
			currentTranslationY = 0;
		}

		public void Draw() {
			if (DrawMethod == null) {
				throw new Exception("Error, you must set the DrawMethod property before calling Draw()");
			}

			Application.Run(mainForm);
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

		public void RectMode(RectangleMode mode) => rectMode = mode;

		#region Stroke

		public void Stroke(Color color) => currentPen.Color = color;

		public void Stroke(byte grey) => Stroke(grey, grey, grey);

		public void Stroke(byte grey, byte a) => Stroke(grey, grey, grey, a);

		public void Stroke(byte r, byte g, byte b) => Stroke(Color.FromArgb(r, g, b));

		public void Stroke(byte r, byte g, byte b, byte a) => Stroke(Color.FromArgb(a, r, g, b));

		public void StrokeWidth(float w) => currentPen.Width = w;

		public void NoStroke() => currentPen.Color = Color.FromArgb(0, 0, 0, 0);

		#endregion

		#region Fill

		public void Fill(Color color) {
			fill = true;
			currentBrush.Color = color;
		}

		public void Fill(byte grey) => Fill(grey, grey, grey);

		public void Fill(byte grey, byte a) => Fill(grey, grey, grey, a);

		public void Fill(byte r, byte g, byte b) => Fill(Color.FromArgb(r, g, b));

		public void Fill(byte r, byte g, byte b, byte a) => Fill(Color.FromArgb(a, r, g, b));

		public void NoFill() => fill = false;

		#endregion

		#region Shapes

		public void Point(float x, float y) => Circle(x, y, 1);

		public void Line(float x1, float y1, float x2, float y2) =>
			Graphics.DrawLine(currentPen, x1, y1, x2, y2);

		public void Rectangle(Rectangle rect) {
			if (fill) {
				Graphics.FillRectangle(currentBrush, rect);
			}

			Graphics.DrawRectangle(currentPen, rect);
		}

		public void Rectangle(float x, float y, float w, float h) {
			switch (rectMode) {
				case RectangleMode.CORNER:
					if (fill) {
						Graphics.FillRectangle(currentBrush, x, y, w, h);
					}

					Graphics.DrawRectangle(currentPen, x, y, w, h);
					break;
				case RectangleMode.CORNERS:
					if (fill) {
						Graphics.FillRectangle(currentBrush, x, y, w - x, h - y);
					}

					Graphics.DrawRectangle(currentPen, x, y, w - x, h - y);
					break;
				case RectangleMode.CENTER:
					if (fill) {
						Graphics.FillRectangle(currentBrush, x - w, y - h, 2 * w, 2 * h);
					}

					Graphics.DrawRectangle(currentPen, x - w, y - h, 2 * w, 2 * h);
					break;
			}
		}

		public void Square(float x, float y, float r) => Rectangle(x, y, r, r);

		public void Ellipse(float x, float y, float rx, float ry) {
			if (fill) {
				Graphics.FillEllipse(currentBrush, x - rx, y - ry, 2 * rx, 2 * ry);
			}

			Graphics.DrawEllipse(currentPen, x - rx, y - ry, 2 * rx, 2 * ry);
		}

		public void Circle(float x, float y, float r) => Ellipse(x, y, r, r);

		#endregion

		#region Transformations

		public void Rotate(float degrees) {
			currentRotation += degrees;
			Graphics.RotateTransform(degrees);
		}

		public void Translate(float dx, float dy) {
			currentTranslationX += dx;
			currentTranslationY += dy;
			Graphics.TranslateTransform(dx, dy);
		}

		public void UndoRotation() {
			Graphics.RotateTransform(-currentRotation);
			currentRotation = 0;
		}

		public void UndoTranslation() {
			Graphics.TranslateTransform(-currentTranslationX, -currentTranslationY);
			currentTranslationX = 0;
			currentTranslationY = 0;
		}

		#endregion

		#region Text

		public void Font(Font font) => currentFont = font;

		public void DrawString(string str, float x, float y)
			=> Graphics.DrawString(str, currentFont, currentBrush, x, y);

		public void DrawString(string str, float x, float y, bool bold, bool italic) {
			FontStyle style = (bold ? FontStyle.Bold : 0) | (italic ? FontStyle.Italic : 0);
			Font font = new Font(currentFont.FontFamily, currentFont.Size, style);
			Graphics.DrawString(str, font, currentBrush, x, y);
		}

		#endregion
	}
}
