using System.Drawing;
using System.Drawing.Drawing2D;
using DrawingBoardNET.Drawing.Constants;

namespace DrawingBoardNET.Drawing;

internal readonly struct Style
{
    internal FontFamily FontFamily { get; }

    internal float FontSize { get; }

    internal Color PenColor { get; }

    internal float PenWidth { get; }

    internal Color BrushColor { get; }

    internal Color TextBrushColor { get; }

    internal StringFormat Format { get; }

    internal RectangleMode RectMode { get; }

    internal ImageMode ImageMode { get; }

    internal LineCap StrokeMode { get; }

    internal DBColorMode ColorMode { get; }

    internal bool Fill { get; }

    internal Style(
        Font font,
        Pen pen,
        SolidBrush brush,
        SolidBrush textBrush,
        StringFormat format,
        RectangleMode rectMode,
        ImageMode imageMode,
        LineCap strokeMode,
        DBColorMode colorMode,
        bool fill
    )
    {
        FontFamily = font.FontFamily;
        FontSize = font.Size;
        PenColor = pen.Color;
        PenWidth = pen.Width;
        BrushColor = brush.Color;
        TextBrushColor = textBrush.Color;

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
