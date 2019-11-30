using System.Drawing;
using drawingBoard.GUI;

namespace drawingBoard {
	public static class Program {
		public static void Main() {
			DrawingBoardRunner.Init(500, 500);

			DrawingBoardRunner.TargetFrameRate = 60;

			DrawingBoardRunner.DrawMethod = (g) => {
				g.DrawLine(new Pen(Color.Red, 5), 0, 0, 500, 500);

				DrawingBoardRunner.SaveToPNG(@"C:\Users\Cesarnijunana27\Downloads\test.png");
			};

			DrawingBoardRunner.Draw();
		}
	}
}
