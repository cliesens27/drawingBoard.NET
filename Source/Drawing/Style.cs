using System.Drawing;
using System.Drawing.Drawing2D;

using DrawingBoardNET.Drawing.Constants;

namespace DrawingBoardNET.Drawing;

internal readonly struct Style
{
    internal Font Font { get; }

    internal Pen Pen { get; }

    internal SolidBrush Brush { get; }

    internal SolidBrush TextBrush { get; }

    internal StringFormat Format { get; }

    internal RectangleMode RectMode { get; }

    internal ImageMode ImageMode { get; }

    internal LineCap StrokeMode { get; }

    internal DBColorMode ColorMode { get; }

    internal bool Fill { get; }

    internal Style(
        Font font, Pen pen, SolidBrush brush, SolidBrush textBrush, StringFormat format,
        RectangleMode rectMode, ImageMode imageMode, LineCap strokeMode, DBColorMode colorMode, bool fill
    )
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
