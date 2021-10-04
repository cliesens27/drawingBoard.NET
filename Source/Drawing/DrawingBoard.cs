using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using DrawingBoardNET.Drawing.Constants;
using DrawingBoardNET.DrawingBoardNET.Exceptions;
using DrawingBoardNET.Window;
using DrawingBoardNET.Window.UI;
using Button = DrawingBoardNET.Window.UI.Button;

namespace DrawingBoardNET.Drawing
{
	public partial class DrawingBoard
	{
		#region Fields & Properties	

		public static int RandomSeed
		{
			get => seed;
			set
			{
				seed = value;
				rng = new Random(seed);
			}
		}

		public InitMethod Init { get => form.Init; set => form.Init = value; }

		public DrawMethod Draw { get => form.Draw; set => form.Draw = value; }

		public DrawButtonMethod DrawButton { get => form.DrawButton; set => form.DrawButton = value; }

		public DrawSliderMethod DrawSlider { get => form.DrawSlider; set => form.DrawSlider = value; }

		public KeyPressedMethod KeyPressed { get => form.KeyPressed; set => form.KeyPressed = value; }

		public KeyReleasedMethod KeyReleased { get => form.KeyReleased; set => form.KeyReleased = value; }

		public MousePressedMethod MousePressed
		{
			get => form.MousePressed;
			set => form.MousePressed = value;
		}

		public MouseReleasedMethod MouseReleased
		{
			get => form.MouseReleased;
			set => form.MouseReleased = value;
		}

		public MouseDraggedMethod MouseDragged
		{
			get => form.MouseDragged;
			set => form.MouseDragged = value;
		}

		public MouseWheelUpMethod MouseWheelUp
		{
			get => form.MouseWheelUp;
			set => form.MouseWheelUp = value;
		}

		public MouseWheelDownMethod MouseWheelDown
		{
			get => form.MouseWheelDown;
			set => form.MouseWheelDown = value;
		}

		public double TargetFrameRate
		{
			get => form.TargetFrameRate;
			set => form.TargetFrameRate = value;
		}

		public string Title { get => form.Text; set => form.Text = value; }

		public RectangleMode RectMode { get => form.rectMode; set => form.rectMode = value; }

		public ImageMode ImageMode { get; set; }

		public LineCap StrokeMode
		{
			get => currentPen.StartCap;
			set => currentPen.StartCap = currentPen.EndCap = value;
		}

		public DBColorMode ColorMode { get; set; }

		public int Width { get; private set; } = -1;
		public int Height { get; private set; } = -1;

		public double FrameRate => form.FrameRate;
		public double TotalElapsedTime => form.TotalElapsedTime;
		public int FrameCount => form.TotalFrameCount;
		public int MouseX => form.PointToClient(Control.MousePosition).X;
		public int MouseY => form.PointToClient(Control.MousePosition).Y;
		public int Xmin => 0;
		public int Ymin => 0;
		public int Xcenter => Width / 2;
		public int Ycenter => Height / 2;
		public int Xmax => Width;
		public int Ymax => Height;

		private Graphics Graphics => form.Graphics;

		private const double DEFAULT_FRAMERATE = 30;
		private const float RADIANS_TO_DEGREES = 180.0f / (float) Math.PI;
		private const float DEGREES_TO_RADIANS = (float) Math.PI / 180.0f;
		private static Random rng;
		private static int seed;
		private readonly MainForm form;
		private readonly bool IsConsoleApplication;
		private Stack<Stack<Transform>> transformStacks;
		private Style oldStyle;
		private Font currentFont;
		private Pen currentPen;
		private SolidBrush currentBrush;
		private SolidBrush currentTextBrush;
		private StringFormat currentFormat;
		private bool fill;
		private bool saveTransforms;
		private bool popping;
		private float currentRotation;
		private float currentTranslationX;
		private float currentTranslationY;

		#endregion

		#region Constructors

		public DrawingBoard(int width, int height, bool isConsoleApp = true)
		{
			form = new MainForm(width, height);
			(IsConsoleApplication, Width, Height) = (isConsoleApp, width, height);
			SetDefaultSettings();
		}

		public DrawingBoard(int width, int height, int x, int y, bool isConsoleApp = true)
		{
			form = new MainForm(width, height, x, y);
			(IsConsoleApplication, Width, Height) = (isConsoleApp, width, height);
			SetDefaultSettings();
		}

		private void SetDefaultSettings()
		{
			Application.EnableVisualStyles();

			Init = null;
			Draw = null;
			DrawButton = null;
			DrawSlider = null;
			KeyPressed = null;
			KeyReleased = null;
			MousePressed = null;
			MouseReleased = null;
			MouseDragged = null;
			MouseWheelUp = null;
			MouseWheelDown = null;

			Title = "Application";
			TargetFrameRate = DEFAULT_FRAMERATE;

			currentFont = new Font("cambria", 12);
			currentPen = new Pen(Color.Black, 1);
			currentBrush = new SolidBrush(Color.Black);
			currentTextBrush = new SolidBrush(Color.Black);
			currentFormat = new StringFormat();
			fill = false;
			saveTransforms = false;
			popping = false;

			ImageMode = ImageMode.Center;
			RectMode = RectangleMode.Center;
			StrokeMode = LineCap.Flat;
			ColorMode = DBColorMode.Rgb;

			HorizontalTextAlign(HorizontalTextAlignment.Left);
			VerticalTextAlign(VerticalTextAlignment.Top);

			transformStacks = new Stack<Stack<Transform>>();

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
				Application.Run(form);
			}
			else
			{
				Thread t = new((ThreadStart) delegate
				{
					Application.Run(form);
				});

				t.Start();
			}
		}

		public void Pause() => form.Pause();

		public void Resume() => form.Resume();

		public void Close() => form.Close();

		public void SaveStyle()
		{
			oldStyle = new Style(currentFont, currentPen, currentBrush, currentTextBrush,
				currentFormat, RectMode, ImageMode, StrokeMode, ColorMode, fill);
		}

		public void RestoreStyle()
		{
			Font(oldStyle.Font);

			currentPen = oldStyle.Pen;
			currentBrush = oldStyle.Brush;
			currentTextBrush = oldStyle.TextBrush;
			currentFormat = oldStyle.Format;

			RectMode = oldStyle.RectMode;
			ImageMode = oldStyle.ImageMode;
			StrokeMode = oldStyle.StrokeMode;
			ColorMode = oldStyle.ColorMode;

			fill = oldStyle.Fill;
		}

		public void AddButton(Button button) => form.AddButton(button);

		public void AddSlider(Slider slider) => form.AddSlider(slider);

		#region Image

		public static Image LoadImage(string path) => Image.FromFile(path);

		public void SaveAsPng(string path) => SaveAs(path, ImageFormat.Png);

		public void SaveAsJpeg(string path) => SaveAs(path, ImageFormat.Jpeg);

		public void SaveAsGif(string path) => SaveAs(path, ImageFormat.Gif);

		public void SaveAs(string path, ImageFormat format)
		{
			using (Bitmap fullBitmap = new(form.Width, form.Height))
			{
				form.DrawToBitmap(fullBitmap, new Rectangle(System.Drawing.Point.Empty, form.Size));

				Point clientOrigin = form.PointToScreen(System.Drawing.Point.Empty);
				Point diff = new(clientOrigin.X - form.Bounds.X, clientOrigin.Y - form.Bounds.Y);
				Rectangle clientRect = new(diff, form.ClientSize);

				using (Bitmap clientAreaBitmap = fullBitmap.Clone(clientRect, PixelFormat.Format32bppArgb))
				{
					clientAreaBitmap.Save(path, format);
				}
			}
		}

		public void DrawImage(Image image) => DrawImage(image, 0, 0);

		public void DrawImage(Image image, float x, float y) => DrawImage(image, x, y, image.Width, image.Height);

		public void DrawImage(Image image, float x, float y, float w, float h)
		{
			switch (ImageMode)
			{
				case ImageMode.Corner:
					Graphics.DrawImage(image, x, y, w, h);
					break;
				case ImageMode.Center:
					Graphics.DrawImage(image, x - 0.5f * w, y - 0.5f * h, w, h);
					break;
			}
		}

		#endregion

		private void CheckColorArguments(int r, int g, int b)
		{
			const int MAX = 255;

			if (ColorMode == DBColorMode.Rgb)
			{
				if (r < 0 || r > MAX)
				{
					throw new ColorModeValueRangeException("R", r, MAX, ColorMode);
				}

				if (g < 0 || g > MAX)
				{
					throw new ColorModeValueRangeException("G", g, MAX, ColorMode);
				}

				if (r < 0 || r > MAX)
				{
					throw new ColorModeValueRangeException("B", b, MAX, ColorMode);
				}
			}
			else if (ColorMode == DBColorMode.Hsb || ColorMode == DBColorMode.Hsl)
			{
				if (r < 0 || r > MAX)
				{
					throw new ColorModeValueRangeException("R (hue, remapped to [0, 360])", r, MAX, ColorMode);
				}

				if (g < 0 || g > MAX)
				{
					throw new ColorModeValueRangeException("G (saturation, remapped to [0, 1])", g, MAX, ColorMode);
				}

				if (r < 0 || r > MAX)
				{
					throw new ColorModeValueRangeException("B (brightness/lightness, remapped to [0, 1])", b, MAX, ColorMode);
				}
			}
		}

		#region Stroke

		public void Stroke(Color color) => currentPen.Color = color;

		public void Stroke(int grey)
		{
			if (ColorMode == DBColorMode.Rgb)
			{
				Stroke(grey, grey, grey);
			}
			else if (ColorMode == DBColorMode.Hsb || ColorMode == DBColorMode.Hsl)
			{
				Stroke(0, 0, grey);
			}
		}

		public void Stroke(int grey, int a)
		{
			if (ColorMode == DBColorMode.Rgb)
			{
				Stroke(grey, grey, grey, a);
			}
			else if (ColorMode == DBColorMode.Hsb || ColorMode == DBColorMode.Hsl)
			{
				Stroke(0, 0, grey, a);
			}
		}

		public void Stroke(int r, int g, int b) => Stroke(r, g, b, 255);

		public void Stroke(int r, int g, int b, int a)
		{
			CheckColorArguments(r, g, b);

			switch (ColorMode)
			{
				case DBColorMode.Rgb:
					currentPen.Color = Color.FromArgb(a, r, g, b);
					break;
				case DBColorMode.Hsb:
					Color fromHSB = ColorUtils.HSBtoRGB(r, g, b);
					currentPen.Color = Color.FromArgb(a, fromHSB.R, fromHSB.G, fromHSB.B);
					break;
				case DBColorMode.Hsl:
					Color fromHSL = ColorUtils.HSLtoRGB(r, g, b);
					currentPen.Color = Color.FromArgb(a, fromHSL.R, fromHSL.G, fromHSL.B);
					break;
			}
		}

		public void StrokeWidth(float w) => currentPen.Width = w;

		public void NoStroke() => currentPen.Color = Color.FromArgb(0, 0, 0, 0);

		#endregion

		#region Fill

		public void Fill(Color color)
		{
			fill = true;
			currentBrush.Color = color;
		}

		public void Fill(int grey)
		{
			if (ColorMode == DBColorMode.Rgb)
			{
				Fill(grey, grey, grey);
			}
			else if (ColorMode == DBColorMode.Hsb || ColorMode == DBColorMode.Hsl)
			{
				Fill(0, 0, grey);
			}
		}

		public void Fill(int grey, int a)
		{
			if (ColorMode == DBColorMode.Rgb)
			{
				Fill(grey, grey, grey, a);
			}
			else if (ColorMode == DBColorMode.Hsb || ColorMode == DBColorMode.Hsl)
			{
				Fill(0, 0, grey, a);
			}
		}

		public void Fill(int r, int g, int b) => Fill(r, g, b, 255);

		public void Fill(int r, int g, int b, int a)
		{
			CheckColorArguments(r, g, b);
			fill = true;

			switch (ColorMode)
			{
				case DBColorMode.Rgb:
					currentBrush.Color = Color.FromArgb(a, r, g, b);
					break;
				case DBColorMode.Hsb:
					Color fromHSB = ColorUtils.HSBtoRGB(r, g, b);
					currentBrush.Color = Color.FromArgb(a, fromHSB.R, fromHSB.G, fromHSB.B);
					break;
				case DBColorMode.Hsl:
					Color fromHSL = ColorUtils.HSLtoRGB(r, g, b);
					currentBrush.Color = Color.FromArgb(a, fromHSL.R, fromHSL.G, fromHSL.B);
					break;
			}
		}

		public void NoFill() => fill = false;

		#endregion

		#region Background

		public void Background(Color color)
		{
			SaveStyle();

			NoStroke();
			Fill(color);
			Rectangle(Xmin, Ymin, Width, Height);

			RestoreStyle();
		}

		public void Background(int grey)
		{
			if (ColorMode == DBColorMode.Rgb)
			{
				Background(grey, grey, grey);
			}
			else if (ColorMode == DBColorMode.Hsb || ColorMode == DBColorMode.Hsl)
			{
				Background(0, 0, grey);
			}
		}

		public void Background(int grey, int a)
		{
			if (ColorMode == DBColorMode.Rgb)
			{
				Background(grey, grey, grey, a);
			}
			else if (ColorMode == DBColorMode.Hsb || ColorMode == DBColorMode.Hsl)
			{
				Background(0, 0, grey, a);
			}
		}

		public void Background(int r, int g, int b) => Background(r, g, b, 255);

		public void Background(int r, int g, int b, int a)
		{
			CheckColorArguments(r, g, b);

			SaveStyle();

			NoStroke();
			Fill(r, g, b, a);
			Rectangle(Xmin, Ymin, Width, Height);

			RestoreStyle();
		}

		#endregion

		#region Shapes

		public void Point(float x, float y)
		{
			SaveStyle();

			NoStroke();
			Fill(currentPen.Color);
			Circle(x, y, currentPen.Width);

			RestoreStyle();
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
			switch (RectMode)
			{
				case RectangleMode.Corner:
					if (fill)
					{
						Graphics.FillRectangle(currentBrush, x, y, w, h);
					}

					Graphics.DrawRectangle(currentPen, x, y, w, h);
					break;
				case RectangleMode.Corners:
					if (fill)
					{
						Graphics.FillRectangle(currentBrush, x, y, w - x, h - y);
					}

					Graphics.DrawRectangle(currentPen, x, y, w - x, h - y);
					break;
				case RectangleMode.Center:
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

		public void RotateDegrees(float degrees)
		{
			if (saveTransforms && !popping && transformStacks.Count > 0)
			{
				transformStacks.Peek().Push(new Transform(TransformType.Rotation, degrees));
			}
			else if (!saveTransforms && !popping)
			{
				currentRotation += degrees;
			}

			Graphics.RotateTransform(degrees);
		}

		public void RotateRadians(float radians) => RotateDegrees(RadiansToDegrees(radians));

		public void Translate(float x, float y)
		{
			if (saveTransforms && !popping && transformStacks.Count > 0)
			{
				transformStacks.Peek().Push(new Transform(TransformType.TranslationX, x));
			}
			else if (!saveTransforms && !popping)
			{
				currentTranslationX += x;
			}

			if (saveTransforms && !popping && transformStacks.Count > 0)
			{
				transformStacks.Peek().Push(new Transform(TransformType.TranslationY, y));
			}
			else if (!saveTransforms && !popping)
			{
				currentTranslationY += y;
			}

			Graphics.TranslateTransform(x, y);
		}

		public void PushMatrix()
		{
			saveTransforms = true;
			transformStacks.Push(new Stack<Transform>());
		}

		public void PopMatrix()
		{
			popping = true;
			var states = transformStacks.Pop();

			while (states.Count > 0)
			{
				var transform = states.Pop();

				switch (transform.Type)
				{
					case TransformType.TranslationX:
						Translate(-transform.Value, 0);
						break;
					case TransformType.TranslationY:
						Translate(0, -transform.Value);
						break;
					case TransformType.Rotation:
						RotateDegrees(-transform.Value);
						break;
				}
			}

			if (transformStacks.Count == 0)
			{
				saveTransforms = false;
			}

			popping = false;
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

		public static Font CreateFont(string name, float size) => new Font(name, size);

		public void HorizontalTextAlign(HorizontalTextAlignment mode)
		{
			switch (mode)
			{
				case HorizontalTextAlignment.Left:
					currentFormat.Alignment = StringAlignment.Near;
					break;
				case HorizontalTextAlignment.Right:
					currentFormat.Alignment = StringAlignment.Far;
					break;
				case HorizontalTextAlignment.Center:
					currentFormat.Alignment = StringAlignment.Center;
					break;
			}
		}

		public void VerticalTextAlign(VerticalTextAlignment mode)
		{
			switch (mode)
			{
				case VerticalTextAlignment.Top:
					currentFormat.LineAlignment = StringAlignment.Near;
					break;
				case VerticalTextAlignment.Bottom:
					currentFormat.LineAlignment = StringAlignment.Far;
					break;
				case VerticalTextAlignment.Center:
					currentFormat.LineAlignment = StringAlignment.Center;
					break;
			}
		}

		public void Font(Font font) => currentFont = font;

		public void Font(string name, float size) => currentFont = new Font(name, size);

		public void Font(string name) => currentFont = new Font(name, currentFont.Size);

		public void FontSize(float size) => currentFont = new Font(currentFont.FontFamily, size);

		public void TextColor(Color color) => currentTextBrush.Color = color;

		public void TextColor(int grey)
		{
			if (ColorMode == DBColorMode.Rgb)
			{
				TextColor(grey, grey, grey);
			}
			else if (ColorMode == DBColorMode.Hsb || ColorMode == DBColorMode.Hsl)
			{
				TextColor(0, 0, grey);
			}
		}

		public void TextColor(int grey, int a)
		{
			if (ColorMode == DBColorMode.Rgb)
			{
				TextColor(grey, grey, grey, a);
			}
			else if (ColorMode == DBColorMode.Hsb || ColorMode == DBColorMode.Hsl)
			{
				TextColor(0, 0, grey, a);
			}
		}

		public void TextColor(int r, int g, int b) => TextColor(r, g, b, 255);

		public void TextColor(int r, int g, int b, int a)
		{
			CheckColorArguments(r, g, b);

			switch (ColorMode)
			{
				case DBColorMode.Rgb:
					currentTextBrush.Color = Color.FromArgb(a, r, g, b);
					break;
				case DBColorMode.Hsb:
					Color fromHSB = ColorUtils.HSBtoRGB(r, g, b);
					currentTextBrush.Color = Color.FromArgb(a, fromHSB.R, fromHSB.G, fromHSB.B);
					break;
				case DBColorMode.Hsl:
					Color fromHSL = ColorUtils.HSLtoRGB(r, g, b);
					currentTextBrush.Color = Color.FromArgb(a, fromHSL.R, fromHSL.G, fromHSL.B);
					break;
			}
		}

		public void Text(string str, float x, float y)
			=> Graphics.DrawString(str, currentFont, currentTextBrush, x, y, currentFormat);

		public void Text(string str, float x, float y, bool bold, bool italic)
		{
			FontStyle style = (bold ? FontStyle.Bold : 0) | (italic ? FontStyle.Italic : 0);
			Font font = new(currentFont.FontFamily, currentFont.Size, style);

			Graphics.DrawString(str, font, currentTextBrush, x, y, currentFormat);
		}

		#endregion

		#region Static Utility Methods

		public static float RadiansToDegrees(float radians) => radians * RADIANS_TO_DEGREES;

		public static float DegreesToRadians(float degrees) => degrees * DEGREES_TO_RADIANS;

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
	}
}
