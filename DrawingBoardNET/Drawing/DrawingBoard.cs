using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using DrawingBoardNET.Drawing.Constants;
using DrawingBoardNET.Drawing.Constants.Window;

namespace DrawingBoardNET.Drawing
{
	public class DrawingBoard
	{
		#region Fields & Properties	

		public InitMethod InitMethod
		{
			get => mainForm.Init;
			set => mainForm.Init = value;
		}

		public DrawMethod DrawMethod
		{
			get => mainForm.Draw;
			set => mainForm.Draw = value;
		}

		public KeyPressedMethod KeyPressed
		{
			get => mainForm.KeyPressed;
			set => mainForm.KeyPressed = value;
		}

		public KeyReleasedMethod KeyReleased
		{
			get => mainForm.KeyReleased;
			set => mainForm.KeyReleased = value;
		}

		public MousePressedMethod MousePressed
		{
			get => mainForm.MousePressed;
			set => mainForm.MousePressed = value;
		}

		public MouseReleasedMethod MouseReleased
		{
			get => mainForm.MouseReleased;
			set => mainForm.MouseReleased = value;
		}

		public MouseDraggedMethod MouseDragged
		{
			get => mainForm.MouseDragged;
			set => mainForm.MouseDragged = value;
		}

		public double TargetFrameRate
		{
			get => mainForm.TargetFrameRate;
			set => mainForm.TargetFrameRate = value;
		}

		public string Title
		{
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

		public int Width { get; private set; } = -1;
		public int Height { get; private set; } = -1;

		private Graphics Graphics => mainForm.Graphics;

		private readonly bool IsConsoleApplication;
		private readonly int screenX = -1;
		private readonly int screenY = -1;
		private Font currentFont;
		private MainForm mainForm;
		private Pen currentPen;
		private SolidBrush currentBrush;
		private SolidBrush currentTextBrush;
		private StringFormat currentFormat;
		private RectangleMode rectMode;
		private ImageMode imageMode;
		private bool fill;
		private float currentRotation;
		private float currentTranslationX;
		private float currentTranslationY;

		#endregion

		#region Constructors

		private DrawingBoard()
		{
			Application.EnableVisualStyles();
		}

		public DrawingBoard(bool isConsoleApplication, int width, int height)
			: this(isConsoleApplication, width, height, -1, -1) { }

		public DrawingBoard(bool isConsoleApplication, int width, int height, int x, int y) : this()
		{
			IsConsoleApplication = isConsoleApplication;

			if (screenX != -1 && screenY != -1)
			{
				mainForm = new MainForm(width, height, x, y);
			}
			else
			{
				mainForm = new MainForm(width, height);
			}

			Width = width;
			Height = height;
			screenX = x;
			screenY = y;

			SetDefaultSettings();
		}

		private void SetDefaultSettings()
		{
			InitMethod = null;
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
			currentTextBrush = new SolidBrush(Color.Black);
			currentFormat = new StringFormat();
			fill = false;

			ImgMode(ImageMode.CENTER);
			RectMode(RectangleMode.CENTER);
			HorizontalTextAlign(HorizontalTextAlignment.LEFT);
			VerticalTextAlign(VerticalTextAlignment.TOP);

			currentRotation = 0;
			currentTranslationX = 0;
			currentTranslationY = 0;
		}

		#endregion

		#region Misc

		public void Draw()
		{
			if (DrawMethod == null)
			{
				throw new Exception("Error, you must set the DrawMethod property before calling Draw()");
			}

			if (IsConsoleApplication)
			{
				Application.Run(mainForm);
			}
			else
			{
				Thread t = new Thread((ThreadStart) delegate
				{
					Application.Run(mainForm);
				});

				t.Start();
			}
		}

		public void Pause() => mainForm.Pause();

		public void Resume() => mainForm.Resume();

		public void Close() => mainForm.Close();

		public void SaveAsPng(string path) => SaveAs(path, ImageFormat.Png);

		public void SaveAsJpeg(string path) => SaveAs(path, ImageFormat.Jpeg);

		public void SaveAsGif(string path) => SaveAs(path, ImageFormat.Gif);

		public void SaveAs(string path, ImageFormat format)
		{
			Bitmap fullBitmap = new Bitmap(mainForm.Width, mainForm.Height);
			mainForm.DrawToBitmap(fullBitmap, new Rectangle(System.Drawing.Point.Empty, mainForm.Size));

			Point clientOrigin = mainForm.PointToScreen(System.Drawing.Point.Empty);

			Rectangle clientRect = new Rectangle(
				new Point(clientOrigin.X - mainForm.Bounds.X, clientOrigin.Y - mainForm.Bounds.Y),
				mainForm.ClientSize);

			Bitmap clientAreaBitmap = fullBitmap.Clone(clientRect, PixelFormat.Format32bppArgb);
			clientAreaBitmap.Save(path, format);
			fullBitmap.Dispose();
			clientAreaBitmap.Dispose();
		}

		public void RectMode(RectangleMode mode) => rectMode = mode;

		public void ImgMode(ImageMode mode) => imageMode = mode;

		public void HorizontalTextAlign(HorizontalTextAlignment mode)
		{
			switch (mode)
			{
				case HorizontalTextAlignment.LEFT:
					currentFormat.Alignment = StringAlignment.Near;
					break;
				case HorizontalTextAlignment.RIGHT:
					currentFormat.Alignment = StringAlignment.Far;
					break;
				case HorizontalTextAlignment.CENTER:
					currentFormat.Alignment = StringAlignment.Center;
					break;
			}
		}

		public void VerticalTextAlign(VerticalTextAlignment mode)
		{
			switch (mode)
			{
				case VerticalTextAlignment.TOP:
					currentFormat.LineAlignment = StringAlignment.Near;
					break;
				case VerticalTextAlignment.BOTTOM:
					currentFormat.LineAlignment = StringAlignment.Far;
					break;
				case VerticalTextAlignment.CENTER:
					currentFormat.LineAlignment = StringAlignment.Center;
					break;
			}
		}

		public Image LoadImage(string path) => Image.FromFile(path);

		public void DrawImage(Image image) => DrawImage(image, 0, 0);

		public void DrawImage(Image image, float x, float y) => DrawImage(image, x, y, image.Width, image.Height);

		public void DrawImage(Image image, float x, float y, float w, float h)
		{
			switch (imageMode)
			{
				case ImageMode.CORNER:
					Console.WriteLine("CORNER");
					Graphics.DrawImage(image, x, y, w, h);
					break;
				case ImageMode.CENTER:
					Console.WriteLine("CENTER");
					Graphics.DrawImage(image, x - 0.5f * w, y - 0.5f * h, w, h);
					break;
			}
		}

		#endregion

		#region Stroke

		public void Stroke(Color color) => currentPen.Color = color;

		public void Stroke(int grey) => Stroke(grey, grey, grey);

		public void Stroke(int grey, int a) => Stroke(grey, grey, grey, a);

		public void Stroke(int r, int g, int b) => Stroke(Color.FromArgb(r, g, b));

		public void Stroke(int r, int g, int b, int a) => Stroke(Color.FromArgb(a, r, g, b));

		public void StrokeWidth(float w) => currentPen.Width = w;

		public void NoStroke() => currentPen.Color = Color.FromArgb(0, 0, 0, 0);

		#endregion

		#region Fill

		public void Fill(Color color)
		{
			fill = true;
			currentBrush.Color = color;
		}

		public void Fill(int grey) => Fill(grey, grey, grey);

		public void Fill(int grey, int a) => Fill(grey, grey, grey, a);

		public void Fill(int r, int g, int b) => Fill(Color.FromArgb(r, g, b));

		public void Fill(int r, int g, int b, int a) => Fill(Color.FromArgb(a, r, g, b));

		public void NoFill() => fill = false;

		#endregion

		#region Shapes

		public void Point(float x, float y) => Circle(x, y, 1);

		public void Line(float x1, float y1, float x2, float y2) => Graphics.DrawLine(currentPen, x1, y1, x2, y2);

		public void Rectangle(Rectangle rect)
		{
			if (fill)
			{
				Graphics.FillRectangle(currentBrush, rect);
			}

			Graphics.DrawRectangle(currentPen, rect);
		}

		public void Rectangle(float x, float y, float w, float h)
		{
			switch (rectMode)
			{
				case RectangleMode.CORNER:
					if (fill)
					{
						Graphics.FillRectangle(currentBrush, x, y, w, h);
					}

					Graphics.DrawRectangle(currentPen, x, y, w, h);
					break;
				case RectangleMode.CORNERS:
					if (fill)
					{
						Graphics.FillRectangle(currentBrush, x, y, w - x, h - y);
					}

					Graphics.DrawRectangle(currentPen, x, y, w - x, h - y);
					break;
				case RectangleMode.CENTER:
					if (fill)
					{
						Graphics.FillRectangle(currentBrush, x - w, y - h, 2 * w, 2 * h);
					}

					Graphics.DrawRectangle(currentPen, x - w, y - h, 2 * w, 2 * h);
					break;
			}
		}

		public void Square(float x, float y, float r) => Rectangle(x, y, r, r);

		public void Ellipse(float x, float y, float rx, float ry)
		{
			if (fill)
			{
				Graphics.FillEllipse(currentBrush, x - rx, y - ry, 2 * rx, 2 * ry);
			}

			Graphics.DrawEllipse(currentPen, x - rx, y - ry, 2 * rx, 2 * ry);
		}

		public void Circle(float x, float y, float r) => Ellipse(x, y, r, r);

		#endregion

		#region Transformations

		public void RotateDegrees(float angle)
		{
			currentRotation += angle;
			Graphics.RotateTransform(angle);
		}

		public void RotateRadians(float angle) => RotateDegrees((float) (180.0 * angle / Math.PI));

		public void Translate(float x, float y)
		{
			currentTranslationX += x;
			currentTranslationY += y;
			Graphics.TranslateTransform(x, y);
		}

		public void UndoRotations()
		{
			Graphics.RotateTransform(-currentRotation);
			currentRotation = 0;
		}

		public void UndoTranslations()
		{
			Graphics.TranslateTransform(-currentTranslationX, -currentTranslationY);
			currentTranslationX = 0;
			currentTranslationY = 0;
		}

		#endregion

		#region Text

		public void Font(Font font) => currentFont = font;

		public void Font(string name, float size) => currentFont = new Font(name, size);

		public void Font(string name) => currentFont = new Font(name, currentFont.Size);

		public void FontSize(float size) => currentFont = new Font(currentFont.FontFamily, size);

		public void FontColor(Color color) => currentTextBrush.Color = color;

		public void FontColor(int grey) => FontColor(grey, grey, grey);

		public void FontColor(int grey, int a) => FontColor(grey, grey, grey, a);

		public void FontColor(int r, int g, int b) => FontColor(Color.FromArgb(r, g, b));

		public void FontColor(int r, int g, int b, int a) => FontColor(Color.FromArgb(a, r, g, b));

		public void Text(string str, float x, float y)
			=> Graphics.DrawString(str, currentFont, currentTextBrush, x, y, currentFormat);

		public void Text(string str, float x, float y, bool bold, bool italic)
		{
			FontStyle style = (bold ? FontStyle.Bold : 0) | (italic ? FontStyle.Italic : 0);
			Font font = new Font(currentFont.FontFamily, currentFont.Size, style);

			Graphics.DrawString(str, font, currentTextBrush, x, y, currentFormat);
		}

		#endregion
	}
}
