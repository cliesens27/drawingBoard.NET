using System;
using System.Drawing;
using drawingBoard.drawing;

namespace drawingBoard {
	public static class Program {
		public static void Main() {
			DrawingBoard db = new DrawingBoard(500, 500);

			db.TargetFrameRate = 60;
			db.Title = "My Application";

			Color col = Color.FromArgb(255);

			db.DrawMethod = () => {
				db.Fill(255);
				db.StrokeWidth(0);
				db.Rectangle(db.Xmin, db.Ymin, db.Xmax, db.Ymax);
				db.Stroke(0, 0, 255);
				db.StrokeWidth(10);

				db.Ellipse(db.Xcenter, db.Ycenter, 100, 100);
				db.Line(db.Xmin, db.Ymin, db.Xmax, db.Ymax);
				db.Line(db.Xmin, db.Ymax, db.Xmax, db.Ymin);

				db.Fill(col);
				db.Ellipse(db.MouseX, db.MouseY, 50, 50);
				Console.WriteLine(db.FrameRate);
			};

			db.KeyPressed = (key) => {
				Console.WriteLine($"PRESSED  {key}");
			};

			db.KeyReleased = (key) => {
				Console.WriteLine($"RELEASED {key}");
			};

			db.MousePressed = () => {
				col = Color.FromArgb(0, 255, 0);
			};

			db.MouseReleased = () => {
				col = Color.FromArgb(255);
			};

			db.Draw();
		}
	}
}
