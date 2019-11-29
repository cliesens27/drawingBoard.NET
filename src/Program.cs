using System.Drawing;
using drawingBoard.GUI;

namespace drawingBoard {
	public static class Program {
		public static void Main() {
			DrawingBoard db = DrawingBoardRunner.Init(500, 500, (g) => {
				g.DrawLine(new Pen(Color.Red, 5), 0, 0, 500, 500);
			});
		}
	}
}
