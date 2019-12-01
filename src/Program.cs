using System.Drawing;
using drawingBoard.GUI;

namespace drawingBoard {
	public static class Program {
		public static void Main() {
			DrawingBoard db = new DrawingBoard(500, 500);

			db.TargetFrameRate = 60;
			db.Title = "My Application";
			db.DrawMethod = (g) => {
				g.DrawLine(new Pen(Color.Red, 5), db.Xmin, db.Ymin, db.Xmax, db.Ymax);
			};

			db.Draw();
		}
	}
}
