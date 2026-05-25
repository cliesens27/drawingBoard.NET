# drawingBoard.NET

A simple C# 2D graphical library inspired by Processing (https://www.processing.org/).

## Requirements

This library is made with and requires the .NET 8.0 Framework. In order to use it, you must append *-windows* to the value of your project's *TargetFramework* property.

## Code Sample

```C#
using System.Windows.Forms;

using DrawingBoardNET.Drawing;
using DrawingBoardNET.Drawing.Constants;
using DrawingBoardNET.Window.UI;

using Button = DrawingBoardNET.Window.UI.Button;

namespace DrawingBoardTest;

internal class Program
{
    public static void Main()
    {
        var db = new DrawingBoard(600, 400);
        db.Title = "DrawingBoardTest";
        db.TargetFrameRate = 30;

        // This is called once before the first call to Draw()
        db.Init = () =>
        {
            db.Stroke(255);
            db.StrokeWidth(2);

            db.FontSize(14);

            db.RectMode = RectangleMode.Corner;
        };

        var slider = new HorizontalSlider("Slider", handleSize: 20, minVal: 0, maxVal: 255, x1: db.Xmin + 100, x2: db.Xmax - 50, y: db.Ycenter);
        db.AddSlider(slider);

        var button = new Button("Test", x: 50, y: 50, w: 60, h: 30, action: () =>
        {
            Console.WriteLine("You pressed the button!");
        });
        db.AddButton(button);

        // This is called once per frame
        db.Draw = () =>
        {
            db.Background(0, (int) slider.Value, 0);
        };

        // This is called once per frame per slider
        db.DrawSlider = (slider) =>
        {
            var s = (HorizontalSlider) slider;

            db.Fill(0, 255 - (int) s.Value, 0);
            db.Line(s.X1, s.Y, s.X2, s.Y);
            db.Circle(s.CurrentX, s.Y, s.HandleSize);

            db.HorizontalTextAlign(HorizontalTextAlignment.Center);
            db.VerticalTextAlign(VerticalTextAlignment.Center);
            db.TextColor(255);
            db.Text(s.Label, s.X1 - 50, s.Y);
        };

        // This is called once per frame per button
        db.DrawButton = (b) =>
        {
            if (b.IsHovered)
            {
                db.Fill(255, 0, 0);
            }
            else
            {
                db.Fill(255);
            }

            db.Rectangle(b.X, b.Y, b.Width, b.Height);

            db.HorizontalTextAlign(HorizontalTextAlignment.Center);
            db.VerticalTextAlign(VerticalTextAlignment.Center);
            db.TextColor(0);
            db.Text(b.Label, b.X + b.Width / 2, b.Y + b.Height / 2);
        };

        // This is called anytime a key is pressed
        db.KeyPressed = (key) =>
        {
            switch (key)
            {
                case Keys.Space:
                    Console.WriteLine("You pressed space!");
                    break;
            }
        };

        // This is called anytime the mouse is pressed
        db.MousePressed = () =>
        {
            Console.WriteLine($"({db.MouseX}, {db.MouseY})");
        };

        // Run the sketch
        db.Start();
    }
}

```

<p align="center">
  <img src="https://i.imgur.com/xUCOGRO.gif"/>
</p>
