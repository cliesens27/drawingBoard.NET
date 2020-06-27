# drawingBoard.NET

A simple 2D graphical C# library inspired by Processing (https://www.processing.org/).

## Code Sample

```C#
using DrawingBoardNET.Drawing;

namespace DrawingBoardTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			DrawingBoard db = new DrawingBoard(600, 400);
			db.Title = "DrawingBoardTest";
			db.TargetFrameRate = 30;

			// This is executed once before the first call to Draw()
			db.InitMethod = () =>
			{
				db.Stroke(0, 0, 0);
				db.StrokeWidth(2.5f);
			};

			// This is executed at most 30 times per second
			db.DrawMethod = () =>
			{
				db.Fill(Utils.Rand(0, 255), Utils.Rand(0, 255), Utils.Rand(0, 255));
				db.Square(Utils.Rand(db.Xmin, db.Xmax), Utils.Rand(db.Ymin, db.Ymax), 30);
			};

			// Draw a white circle when the mouse is clicked
			db.MousePressed = () =>
			{
				db.Fill(255);
				db.Circle(db.MouseX, db.MouseY, 100);
			};

			// Run the sketch!
			db.Draw();
		}
	}
}
```

![](https://i.imgur.com/Bu79XLU.png)
