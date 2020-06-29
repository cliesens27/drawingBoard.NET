# drawingBoard.NET

A simple 2D graphical C# library inspired by Processing (https://www.processing.org/).

## Code Sample

```C#
using DrawingBoardNET.DrawingBoardNET.Drawing;
using DB = DrawingBoardNET.Drawing.DrawingBoard;

namespace DrawingBoardTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			DB db = new DB(600, 400, true, false);
			db.Title = "DrawingBoardTest";
			db.TargetFrameRate = 30;

			Slider slider = new Slider(150, 450, 0, 255, db.Ycenter, 25);
			db.AddSlider(slider);

			// This is executed once before the first call to Draw()
			db.Init = () =>
			{
				db.Stroke(255);
				db.StrokeWidth(3);
				db.Fill(DB.Rand(0, 255), DB.Rand(0, 255), DB.Rand(0, 255));
			};

			// This is executed at most 30 times per second, i.e. every frame
			db.Draw = () =>
			{
				db.Background(0, (int) slider.Value, 0);
			};

			// This is executed once per slider per frame
			db.DrawSlider = (s) =>
			{
				db.Fill(0, 255 - (int) slider.Value, 0);
				db.Line(s.MinX, s.Y, s.MaxX, s.Y);
				db.Circle(s.X, s.Y, s.Radius);
			};

			// Run the sketch
			db.Start();
		}
	}
}
```

![](https://i.imgur.com/Zbc3F3Y.gif)
