# drawingBoard.NET

A simple 2D graphical C# library inspired by Processing (https://www.processing.org/).

## Code Sample

```C#
using DB = DrawingBoardNET.Drawing.DrawingBoard;

namespace DrawingBoardTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			DB db = new DB(600, 400);
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
				db.Fill(DB.Rand(0, 255), DB.Rand(0, 255), DB.Rand(0, 255));
				db.Square(DB.Rand(db.Xmin, db.Xmax), DB.Rand(db.Ymin, db.Ymax), 30);
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
