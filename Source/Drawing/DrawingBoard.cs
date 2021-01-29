﻿using System;
using System.Collections.Generic;
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
	public class DrawingBoard
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

		public RectangleMode RectMode
		{
			get => form.rectMode;
			private set => form.rectMode = value;
		}

		public ImageMode ImageMode { get; private set; }

		public LineCap StrokeMode
		{
			get => currentPen.StartCap;
			private set => currentPen.StartCap = currentPen.EndCap = value;
		}

		public DBColorMode ColorMode { get; private set; }

		public int Width { get; private set; } = -1;
		public int Height { get; private set; } = -1;

		private Graphics Graphics => form.Graphics;

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

		private const double DEFAULT_FRAMERATE = 30;
		private static Random rng;
		private static int seed;
		private readonly MainForm form;
		private readonly bool IsConsoleApplication;
		private Style oldStyle;
		private Font currentFont;
		private Pen currentPen;
		private SolidBrush currentBrush;
		private SolidBrush currentTextBrush;
		private StringFormat currentFormat;
		private bool fill;
		private float currentRotation;
		private float currentTranslationX;
		private float currentTranslationY;

		#endregion

		#region Constructors

		public DrawingBoard(int width, int height, bool isConsoleApplication = true)
		{
			IsConsoleApplication = isConsoleApplication;
			form = new MainForm(width, height);
			(Width, Height) = (width, height);

			SetDefaultSettings();
		}

		public DrawingBoard(int width, int height, int x, int y, bool isConsoleApplication = true)
		{
			IsConsoleApplication = isConsoleApplication;
			form = new MainForm(width, height, x, y);
			(Width, Height) = (width, height);

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

			TargetFrameRate = DEFAULT_FRAMERATE;
			Title = "Application";

			currentFont = new Font("cambria", 12);
			currentPen = new Pen(Color.Black, 1);
			currentBrush = new SolidBrush(Color.Black);
			currentTextBrush = new SolidBrush(Color.Black);
			currentFormat = new StringFormat();
			fill = false;

			ImageMode = ImageMode.Center;
			RectMode = RectangleMode.Center;
			StrokeMode = LineCap.Flat;
			ColorMode = DBColorMode.Rgb;

			HorizontalTextAlign(HorizontalTextAlignment.Left);
			VerticalTextAlign(VerticalTextAlignment.Top);

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
				Thread t = new Thread((ThreadStart) delegate
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
			Bitmap fullBitmap = new Bitmap(form.Width, form.Height);
			form.DrawToBitmap(fullBitmap, new Rectangle(System.Drawing.Point.Empty, form.Size));

			Point clientOrigin = form.PointToScreen(System.Drawing.Point.Empty);

			Rectangle clientRect = new Rectangle(
				new Point(clientOrigin.X - form.Bounds.X, clientOrigin.Y - form.Bounds.Y),
				form.ClientSize);

			Bitmap clientAreaBitmap = fullBitmap.Clone(clientRect, PixelFormat.Format32bppArgb);
			clientAreaBitmap.Save(path, format);

			fullBitmap.Dispose();
			clientAreaBitmap.Dispose();
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
			const int max = 255;

			if (ColorMode == DBColorMode.Rgb)
			{
				if (r < 0 || r > max)
				{
					throw new ColorModeValueRangeException("R", r, max, ColorMode);
				}

				if (g < 0 || g > max)
				{
					throw new ColorModeValueRangeException("G", g, max, ColorMode);
				}

				if (r < 0 || r > max)
				{
					throw new ColorModeValueRangeException("B", b, max, ColorMode);
				}
			}
			else if (ColorMode == DBColorMode.Hsb || ColorMode == DBColorMode.Hsl)
			{
				if (r < 0 || r > max)
				{
					throw new ColorModeValueRangeException("R (hue, remapped to [0, 360])", r, max, ColorMode);
				}

				if (g < 0 || g > max)
				{
					throw new ColorModeValueRangeException("G (saturation, remapped to [0, 1])", g, max, ColorMode);
				}

				if (r < 0 || r > max)
				{
					throw new ColorModeValueRangeException("B (brightness/lightness, remapped to [0, 1])", b, max, ColorMode);
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
					Color fromHSB = HSBtoRGB(r, g, b);
					currentPen.Color = Color.FromArgb(a, fromHSB.R, fromHSB.G, fromHSB.B);
					break;
				case DBColorMode.Hsl:
					Color fromHSL = HSLtoRGB(r, g, b);
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
					Color fromHSB = HSBtoRGB(r, g, b);
					currentBrush.Color = Color.FromArgb(a, fromHSB.R, fromHSB.G, fromHSB.B);
					break;
				case DBColorMode.Hsl:
					Color fromHSL = HSLtoRGB(r, g, b);
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
					Color fromHSB = HSBtoRGB(r, g, b);
					currentTextBrush.Color = Color.FromArgb(a, fromHSB.R, fromHSB.G, fromHSB.B);
					break;
				case DBColorMode.Hsl:
					Color fromHSL = HSLtoRGB(r, g, b);
					currentTextBrush.Color = Color.FromArgb(a, fromHSL.R, fromHSL.G, fromHSL.B);
					break;
			}
		}

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

		private static Color HSBtoRGB(int hue, int saturation, int brightness)
		{
			double h = 360 * (hue / 255.0);
			double s = saturation / 255.0;
			double b = brightness;

			double f = h / 60 - Math.Floor(h / 60);

			int w = brightness;
			int x = (int) (b * (1 - s));
			int y = (int) (b * (1 - f * s));
			int z = (int) (b * (1 - (1 - f) * s));

			int i = ((int) Math.Floor(h / 60)) % 6;

			return i switch
			{
				0 => Color.FromArgb(255, w, z, x),
				1 => Color.FromArgb(255, y, w, x),
				2 => Color.FromArgb(255, x, w, z),
				3 => Color.FromArgb(255, x, y, w),
				4 => Color.FromArgb(255, z, x, w),
				_ => Color.FromArgb(255, w, x, y),
			};
		}

		private static Color HSLtoRGB(int hue, int saturation, int lightness)
		{
			double h = 360 * (hue / 255.0);
			double s = saturation / 255.0;
			double l = lightness / 255.0;

			int r, g, b;

			if (saturation == 0)
			{
				r = g = b = lightness;
			}
			else
			{
				double q1, q2;
				double qHue = h / 6;

				if (l < 0.5)
				{
					q2 = l * (1 + s);
				}
				else
				{
					q2 = (l + s) - (l * s);
				}

				q1 = 2d * s - q2;

				double tr, tg, tb;
				tr = qHue + (1.0 / 3.0);
				tg = qHue;
				tb = qHue - (1.0 / 3.0);

				tr = HSLColorComponent(tr, q1, q2);
				tg = HSLColorComponent(tg, q1, q2);
				tb = HSLColorComponent(tb, q1, q2);

				r = (int) Math.Round(tr * 255);
				g = (int) Math.Round(tg * 255);
				b = (int) Math.Round(tb * 255);
			}

			return Color.FromArgb(r, g, b);
		}

		private static double HSLColorComponent(double q1, double q2, double q3)
		{
			if (q1 < 0)
			{
				q1 += 1;
			}
			if (q1 > 1)
			{
				q1 -= 1;
			}
			if (q1 < 1.0 / 6.0)
			{
				return q2 + (q3 - q2) * 6 * q1;
			}
			if (q1 < 0.5)
			{
				return q3;
			}
			if (q1 < 2.0 / 3.0)
			{
				return q2 + (q3 - q2) * (2.0 / 3.0 - q1) * 6;
			}

			return q2;
		}

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
			internal DBColorMode ColorMode { get; private set; }
			internal bool Fill { get; private set; }

			internal Style(Font font, Pen pen, SolidBrush brush, SolidBrush textBrush, StringFormat format,
				RectangleMode rectMode, ImageMode imageMode, LineCap strokeMode, DBColorMode colorMode, bool fill)
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
				ColorMode = colorMode;
				Fill = fill;
			}
		}
	}
}
