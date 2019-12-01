using System.Drawing;
using drawingBoard.drawing;

namespace drawingBoard {
	public static class Program {
		public static void Main() {
			DrawingBoard db = new DrawingBoard(500, 500);

			db.TargetFrameRate = 60;
			db.Title = "My Application";
			db.DrawMethod = (g) => {
				g.DrawLine(new Pen(Color.Blue, 15), db.Xmin, db.Ymin, db.Xmax, db.Ymax);
				g.DrawLine(new Pen(Color.Blue, 15), db.Xmin, db.Ymax, db.Xmax, db.Ymin);
			};

			db.Draw();
		}
	}
}
