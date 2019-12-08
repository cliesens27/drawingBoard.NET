using System;
using System.Drawing;
using drawingBoard.drawing;

namespace drawingBoard {
	public static class Program {
		public static void Main() {
			DrawingBoard db = new DrawingBoard(500, 500);

			db.TargetFrameRate = 60;
			db.Title = "My Application";

			db.DrawMethod = (g) => {
				Pen bluePen = new Pen(Color.Blue, 15);

				g.DrawEllipse(bluePen, db.Xcenter - 100, db.Ycenter - 100, 200, 200);
				g.DrawLine(bluePen, db.Xmin, db.Ymin, db.Xmax, db.Ymax);
				g.DrawLine(bluePen, db.Xmin, db.Ymax, db.Xmax, db.Ymin);
			};

			db.KeyPressed = (key) => {
				Console.WriteLine(key);
			};

			db.Draw();
		}
	}
}
