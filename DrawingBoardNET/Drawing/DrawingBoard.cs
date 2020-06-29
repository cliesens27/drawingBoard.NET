using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using DrawingBoardNET.Drawing.Constants;
using DrawingBoardNET.Drawing.Window;

namespace DrawingBoardNET.Drawing
{
	public class DrawingBoard
	{
		#region Fields & Properties	

		public InitMethod Init
		{
			get => mainForm.Init;
			set => mainForm.Init = value;
		}

		public DrawMethod Draw
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

		public static int RandomSeed
		{
			get => seed;
			set
			{
				seed = value;
				rng = new Random(seed);
			}
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
		public int MouseX => mainForm.PointToClient(Control.MousePosition).X;
		public int MouseY => mainForm.PointToClient(Control.MousePosition).Y;
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
		private static Random rng;
		private static int seed;
		private Style oldStyle;
		private Font currentFont;
		private MainForm mainForm;
		private Pen currentPen;
		private SolidBrush currentBrush;
		private SolidBrush currentTextBrush;
		private StringFormat currentFormat;
		private RectangleMode rectMode;
		private ImageMode imageMode;
		private LineCap strokeMode;
		private bool fill;
		private float currentRotation;
		private float currentTranslationX;
		private float currentTranslationY;

		#endregion

		#region Constructors

		public DrawingBoard(int width, int height,
			bool isConsoleApplication = true, bool redrawEveryFrame = false)
			: this(width, height, -1, -1, isConsoleApplication, redrawEveryFrame) { }

		public DrawingBoard(int width, int height, int x, int y,
			bool isConsoleApplication = true, bool redrawEveryFrame = false)
		{
			IsConsoleApplication = isConsoleApplication;

			if (screenX != -1 && screenY != -1)
			{
				mainForm = new MainForm(width, height, x, y, redrawEveryFrame);
			}
			else
			{
				mainForm = new MainForm(width, height, redrawEveryFrame);
			}

			Width = width;
			Height = height;
			screenX = x;
			screenY = y;

			SetDefaultSettings();
		}

		private void SetDefaultSettings()
		{
			Application.EnableVisualStyles();

			Init = null;
			Draw = null;
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
			StrokeMode(LineCap.Flat);
			HorizontalTextAlign(HorizontalTextAlignment.LEFT);
			VerticalTextAlign(VerticalTextAlignment.TOP);

			currentRotation = 0;
			currentTranslationX = 0;
			currentTranslationY = 0;

			RandomSeed = (int) DateTime.Now.Ticks;
			rng = new Random(seed);
		}

		#endregion

		public void Start()
		{
			if (Draw == null)
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

		public void SaveStyle()
		{
			oldStyle = new Style(currentFont, currentPen, currentBrush, currentTextBrush,
				currentFormat, rectMode, imageMode, strokeMode, fill);
		}

		public void RestoreStyle()
		{
			Font(oldStyle.Font);

			currentPen = oldStyle.Pen;
			currentBrush = oldStyle.Brush;
			currentTextBrush = oldStyle.TextBrush;
			currentFormat = oldStyle.Format;

			RectMode(oldStyle.RectMode);
			ImgMode(oldStyle.ImageMode);
			StrokeMode(oldStyle.StrokeMode);

			fill = oldStyle.Fill;
		}

		#region Image

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

		public void ImgMode(ImageMode mode) => imageMode = mode;

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

		public void StrokeMode(LineCap mode)
		{
			strokeMode = mode;
			currentPen.StartCap = currentPen.EndCap = strokeMode;
		}

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

		#region Background

		public void Background(Color color)
		{
			NoStroke();
			Fill(color);
			Rectangle(Xmin, Ymin, Width, Height);

			NoFill();
		}

		public void Background(int grey) => Background(grey, grey, grey);

		public void Background(int grey, int a) => Background(grey, grey, grey, a);

		public void Background(int r, int g, int b) => Background(Color.FromArgb(r, g, b));

		public void Background(int r, int g, int b, int a) => Background(Color.FromArgb(a, r, g, b));

		#endregion

		#region Shapes

		public void RectMode(RectangleMode mode) => rectMode = mode;

		public void Point(float x, float y)
		{
			Color oldStroke = currentPen.Color;
			Color oldFill = currentBrush.Color;

			NoStroke();
			Fill(oldStroke);
			Circle(x, y, currentPen.Width);

			if (fill)
			{
				Fill(oldFill);
			}
			else
			{
				NoFill();
			}

			Stroke(oldStroke);
		}

		public void Line(float x1, float y1, float x2, float y2) => Graphics.DrawLine(currentPen, x1, y1, x2, y2);

		public void Arc(float x, float y, float width, float height, float startAngle, float sweepAngle)
			=> Graphics.DrawArc(currentPen, x, y, width, height, startAngle, sweepAngle);

		public void Bezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
			=> Graphics.DrawBezier(currentPen, x1, y1, x2, y2, x3, y3, x4, y4);

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

		public void Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
			=> Polygon(new PointF[] { new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3) });

		public void Polygon(List<PointF> points)
		{
			if (fill)
			{
				Graphics.FillPolygon(currentBrush, points.ToArray());
			}

			Graphics.DrawPolygon(currentPen, points.ToArray());
		}

		public void Polygon(PointF[] points)
		{
			if (fill)
			{
				Graphics.FillPolygon(currentBrush, points);
			}

			Graphics.DrawPolygon(currentPen, points);
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

		public Font CreateFont(string name, float size) => new Font(name, size);

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

		#region Static Utility Methods

		public static double Rand() => rng.NextDouble();

		public static int Rand(int max) => (int) (rng.NextDouble() * max);

		public static float Rand(float max) => (float) (rng.NextDouble() * max);

		public static double Rand(double max) => rng.NextDouble() * max;

		public static int Rand(int min, int max)
		{
			if (min >= max)
			{
				throw new ArgumentException($"Max should be smaller than min" +
					$"\n\tmin = {min}\n\tmax = {max}");
			}

			return (int) (rng.NextDouble() * (max - min) + min);
		}

		public static float Rand(float min, float max)
		{
			if (min >= max)
			{
				throw new ArgumentException($"Max should be smaller than min" +
					$"\n\tmin = {min}\n\tmax = {max}");
			}

			return (float) (rng.NextDouble() * (max - min) + min);
		}

		public static double Rand(double min, double max)
		{
			if (min >= max)
			{
				throw new ArgumentException($"Max should be smaller than min" +
					$"\n\tmin = {min}\n\tmax = {max}");
			}

			return rng.NextDouble() * (max - min) + min;
		}

		public static float Lerp(float val, float x1, float x2, float y1, float y2)
		{
			if (val == x1)
			{
				return y1;
			}

			if (val == x2)
			{
				return y2;
			}

			return (y2 - y1) / (x2 - x1) * (val - x1) + y1;
		}

		public static double Lerp(double val, double x1, double x2, double y1, double y2)
		{
			if (val == x1)
			{
				return y1;
			}

			if (val == x2)
			{
				return y2;
			}

			return (y2 - y1) / (x2 - x1) * (val - x1) + y1;
		}

		#endregion

		private class Style
		{
			internal Font Font { get; private set; }
			internal Pen Pen { get; private set; }
			internal SolidBrush Brush { get; private set; }
			internal SolidBrush TextBrush { get; private set; }
			internal StringFormat Format { get; private set; }
			internal RectangleMode RectMode { get; private set; }
			internal ImageMode ImageMode { get; private set; }
			internal LineCap StrokeMode { get; private set; }
			internal bool Fill { get; private set; }

			internal Style(Font font, Pen pen, SolidBrush brush, SolidBrush textBrush, StringFormat format,
				RectangleMode rectMode, ImageMode imageMode, LineCap strokeMode, bool fill)
			{
				Font = new Font(font.FontFamily, font.Size);
				Pen = new Pen(pen.Color, pen.Width);
				Brush = new SolidBrush(brush.Color);
				TextBrush = new SolidBrush(textBrush.Color);

				Format = new StringFormat();
				Format.LineAlignment = format.LineAlignment;
				Format.Alignment = format.Alignment;

				RectMode = rectMode;
				ImageMode = imageMode;
				StrokeMode = strokeMode;
				Fill = fill;
			}
		}
	}
}
