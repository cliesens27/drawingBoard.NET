using System.Windows.Forms;
using drawingBoard.GUI;

namespace drawingBoard {
	public static class DrawingBoardRunner {
		private static void Start() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}

		public static void Start(int width, int height) {
			Start();
			Application.Run(new DrawingBoard(width, height));
		}

		public static void Start(int width, int height, int x, int y) {
			Start();
			Application.Run(new DrawingBoard(width, height, x, y));
		}
	}
}
