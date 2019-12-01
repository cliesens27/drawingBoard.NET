using System.Drawing;
using drawingBoard.GUI;

namespace drawingBoard {
	public static class Program {
		public static void Main() {
			DrawingBoard.Init(500, 500);

			DrawingBoard.TargetFrameRate = 60;

			DrawingBoard.DrawMethod = (g) => {
				g.DrawLine(new Pen(Color.Red, 5), 0, 0, 500, 500);

				DrawingBoard.SaveToPNG(@"C:\Users\Cesarnijunana27\Downloads\test.png");
			};

			DrawingBoard.Draw();
		}
	}
}
