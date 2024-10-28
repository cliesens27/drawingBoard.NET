using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

using DrawingBoardNET.Drawing.Constants;
using DrawingBoardNET.Exceptions;
using DrawingBoardNET.Window;
using DrawingBoardNET.Window.UI;

using Button = DrawingBoardNET.Window.UI.Button;

namespace DrawingBoardNET.Drawing;

public class DrawingBoard
{
    #region Fields & Properties

    public InitMethod Init
    {
        get => form.Init;
        set => form.Init = value;
    }

    public DrawMethod Draw
    {
        get => form.Draw;
        set => form.Draw = value;
    }

    public DrawButtonMethod DrawButton
    {
        get => form.DrawButton;
        set => form.DrawButton = value;
    }

    public DrawSliderMethod DrawSlider
    {
        get => form.DrawSlider;
        set => form.DrawSlider = value;
    }

    public KeyPressedMethod KeyPressed
    {
        get => form.KeyPressed;
        set => form.KeyPressed = value;
    }

    public KeyReleasedMethod KeyReleased
    {
        get => form.KeyReleased;
        set => form.KeyReleased = value;
    }

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

    public string Title
    {
        get => form.Text;
        set => form.Text = value;
    }

    public RectangleMode RectMode
    {
        get => form.RectMode;
        set => form.RectMode = value;
    }

    public ImageMode ImageMode { get; set; }

    public LineCap StrokeMode
    {
        get => currentPen.StartCap;
        set => currentPen.StartCap = currentPen.EndCap = value;
    }

    public DBColorMode ColorMode { get; set; }

    public int Width { get; } = -1;

    public int Height { get; } = -1;

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

    private const double DefaultFramerate = 30;

    private readonly MainForm form;
    private readonly bool isConsoleApplication;
    private Stack<Stack<Transform>> transformStacks;
    private Style oldStyle;
    private Font currentFont;
    private Pen currentPen;
    private SolidBrush currentBrush;
    private SolidBrush currentTextBrush;
    private StringFormat currentFormat;
    private bool fill;
    private bool saveTransforms;
    private bool poppingTransformStack;
    private double currentRotation;
    private double currentTranslationX;
    private double currentTranslationY;

    #endregion

    #region Constructors

    public DrawingBoard(int width, int height, bool isConsoleApp = true)
    {
        form = new MainForm(width, height);
        (isConsoleApplication, Width, Height) = (isConsoleApp, width, height);
        SetDefaultSettings();
    }

    public DrawingBoard(int width, int height, int x, int y, bool isConsoleApp = true)
    {
        form = new MainForm(width, height, x, y);
        (isConsoleApplication, Width, Height) = (isConsoleApp, width, height);
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
        TargetFrameRate = DefaultFramerate;

        currentFont = new Font("cambria", 12);
        currentPen = new Pen(Color.Black, 1);
        currentBrush = new SolidBrush(Color.Black);
        currentTextBrush = new SolidBrush(Color.Black);
        currentFormat = new StringFormat();
        fill = false;
        saveTransforms = false;
        poppingTransformStack = false;

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
    }

    #endregion

    public void Start()
    {
        if (Draw == null)
        {
            throw new Exception("Error, you must set the DrawMethod property before calling Draw()");
        }

        if (isConsoleApplication)
        {
            Application.Run(form);
        }
        else
        {
            Thread t = new((ThreadStart) delegate
            { Application.Run(form); });

            t.Start();
        }
    }

    public void Pause() => form.Pause();

    public void Resume() => form.Resume();

    public void Close() => form.Close();

    public void SaveStyle()
    {
        oldStyle = new Style(
            currentFont,
            currentPen,
            currentBrush,
            currentTextBrush,
            currentFormat,
            RectMode,
            ImageMode,
            StrokeMode,
            ColorMode,
            fill
        );
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

    public void DrawImage(Image image, double x, double y) => DrawImage(image, x, y, image.Width, image.Height);

    public void DrawImage(Image image, double x, double y, double w, double h)
    {
        switch (ImageMode)
        {
            case ImageMode.Corner:
                Graphics.DrawImage(image, (float) x, (float) y, (float) w, (float) h);

                break;
            case ImageMode.Center:
                Graphics.DrawImage(image, (float) (x - 0.5f * w), (float) (y - 0.5f * h), (float) w, (float) h);

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
                throw new ColorModeValueRangeException(
                    "B (brightness/lightness, remapped to [0, 1])",
                    b,
                    max,
                    ColorMode
                );
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

    public void StrokeWidth(double w) => currentPen.Width = (float) w;

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

    public void Point(double x, double y)
    {
        SaveStyle();

        NoStroke();
        Fill(currentPen.Color);
        Circle(x, y, currentPen.Width);

        RestoreStyle();
    }

    public void Line(double x1, double y1, double x2, double y2) => Graphics.DrawLine(
        currentPen,
        (float) x1,
        (float) y1,
        (float) x2,
        (float) y2
    );

    public void Arc(double x, double y, double width, double height, double startAngle, double sweepAngle) =>
        Graphics.DrawArc(
            currentPen,
            (float) x,
            (float) y,
            (float) width,
            (float) height,
            (float) startAngle,
            (float) sweepAngle
        );

    public void Bezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4) =>
        Graphics.DrawBezier(
            currentPen,
            (float) x1,
            (float) y1,
            (float) x2,
            (float) y2,
            (float) x3,
            (float) y3,
            (float) x4,
            (float) y4
        );

    public void Rectangle(Rectangle rect)
    {
        if (fill)
        {
            Graphics.FillRectangle(currentBrush, rect);
        }

        Graphics.DrawRectangle(currentPen, rect);
    }

    public void Rectangle(double x, double y, double w, double h)
    {
        switch (RectMode)
        {
            case RectangleMode.Corner:
                if (fill)
                {
                    Graphics.FillRectangle(currentBrush, (float) x, (float) y, (float) w, (float) h);
                }

                Graphics.DrawRectangle(currentPen, (float) x, (float) y, (float) w, (float) h);

                break;
            case RectangleMode.Corners:
                if (fill)
                {
                    Graphics.FillRectangle(currentBrush, (float) x, (float) y, (float) (w - x), (float) (h - y));
                }

                Graphics.DrawRectangle(currentPen, (float) x, (float) y, (float) (w - x), (float) (h - y));

                break;
            case RectangleMode.Center:
                if (fill)
                {
                    Graphics.FillRectangle(
                        currentBrush,
                        (float) (x - w),
                        (float) (y - h),
                        (float) (2 * w),
                        (float) (2 * h)
                    );
                }

                Graphics.DrawRectangle(
                    currentPen,
                    (float) (x - w),
                    (float) (y - h),
                    (float) (2 * w),
                    (float) (2 * h)
                );

                break;
        }
    }

    public void Triangle(double x1, double y1, double x2, double y2, double x3, double y3) => Polygon(
        new PointF[]
        {
            new((float) x1, (float) y1),
            new((float) x2, (float) y2),
            new((float) x3, (float) y3)
        }
    );

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

    public void Square(double x, double y, double r) => Rectangle(x, y, r, r);

    public void Ellipse(double x, double y, double rx, double ry)
    {
        if (fill)
        {
            Graphics.FillEllipse(
                currentBrush,
                (float) (x - rx),
                (float) (y - ry),
                (float) (2 * rx),
                (float) (2 * ry)
            );
        }

        Graphics.DrawEllipse(
            currentPen,
            (float) (x - rx),
            (float) (y - ry),
            (float) (2 * rx),
            (float) (2 * ry)
        );
    }

    public void Circle(double x, double y, double r) => Ellipse(x, y, r, r);

    #endregion

    #region Transformations

    public void RotateDegrees(double degrees)
    {
        if (saveTransforms && !poppingTransformStack && transformStacks.Count > 0)
        {
            transformStacks.Peek().Push(new Transform(TransformType.Rotation, degrees));
        }
        else if (!saveTransforms && !poppingTransformStack)
        {
            currentRotation += degrees;
        }

        Graphics.RotateTransform((float) degrees);
    }

    public void RotateRadians(double radians) => RotateDegrees(MathUtils.RadiansToDegrees(radians));

    public void Translate(double x, double y)
    {
        if (saveTransforms && !poppingTransformStack && transformStacks.Count > 0)
        {
            transformStacks.Peek().Push(new Transform(TransformType.TranslationX, x));
        }
        else if (!saveTransforms && !poppingTransformStack)
        {
            currentTranslationX += x;
        }

        if (saveTransforms && !poppingTransformStack && transformStacks.Count > 0)
        {
            transformStacks.Peek().Push(new Transform(TransformType.TranslationY, y));
        }
        else if (!saveTransforms && !poppingTransformStack)
        {
            currentTranslationY += y;
        }

        Graphics.TranslateTransform((float) x, (float) y);
    }

    public void PushMatrix()
    {
        saveTransforms = true;
        transformStacks.Push(new Stack<Transform>());
    }

    public void PopMatrix()
    {
        poppingTransformStack = true;
        Stack<Transform> states = transformStacks.Pop();

        while (states.Count > 0)
        {
            Transform transform = states.Pop();

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

        poppingTransformStack = false;
    }

    public void UndoRotations()
    {
        Graphics.RotateTransform((float) -currentRotation);
        currentRotation = 0;
    }

    public void UndoTranslations()
    {
        Graphics.TranslateTransform((float) -currentTranslationX, (float) -currentTranslationY);
        currentTranslationX = 0;
        currentTranslationY = 0;
    }

    #endregion

    #region Text

    public static Font CreateFont(string name, double size) => new(name, (float) size);

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

    public void Font(string name, double size) => currentFont = new Font(name, (float) size);

    public void Font(string name) => currentFont = new Font(name, currentFont.Size);

    public void FontSize(double size) => currentFont = new Font(currentFont.FontFamily, (float) size);

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

    public void Text(string str, double x, double y) => Graphics.DrawString(
        str,
        currentFont,
        currentTextBrush,
        (float) x,
        (float) y,
        currentFormat
    );

    public void Text(string str, double x, double y, bool bold, bool italic)
    {
        FontStyle style = (bold ? FontStyle.Bold : 0) | (italic ? FontStyle.Italic : 0);
        Font font = new(currentFont.FontFamily, currentFont.Size, style);

        Graphics.DrawString(str, font, currentTextBrush, (float) x, (float) y, currentFormat);
    }

    #endregion
}
