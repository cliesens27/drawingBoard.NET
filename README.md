# drawingBoard.NET

A simple C# 2D graphical library inspired by Processing (https://www.processing.org/).

## Requirements

This library is made with and requires the .NET 5.0 Framework. In order to use it, you must append *-windows* to the value of your project's *TargetFramework* property.

## Code Sample

```C#
using DrawingBoardNET.Drawing;
using DrawingBoardNET.Window.UI;

namespace DrawingBoardTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			DrawingBoard db = new DrawingBoard(600, 400);
			db.Title = "DrawingBoardTest";
			db.TargetFrameRate = 30;

			Slider slider = new Slider(150, 450, 0, 255, db.Ycenter, 20);
			db.AddSlider(slider);

			// This is executed once before the first call to Draw()
			db.Init = () =>
			{
				db.Stroke(255);
				db.StrokeWidth(2);
			};

			// This is executed once per frame
			db.Draw = () =>
			{
				db.Background(0, (int) slider.Value, 0);
			};

			// This is executed once per frame per slider
			db.DrawSlider = (slider) =>
			{
				db.Fill(0, 255 - (int) slider.Value, 0);
				db.Line(slider.MinX, slider.Y, slider.MaxX, slider.Y);
				db.Circle(slider.X, slider.Y, slider.Radius);
			};

			// Run the sketch
			db.Start();
		}
	}
}
```

<p align="center">
  <img src="https://i.imgur.com/Zbc3F3Y.gif"/>
</p>
