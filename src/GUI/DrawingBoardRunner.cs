using System.Windows.Forms;

namespace drawingBoard.GUI {
	public static class DrawingBoardRunner {
		private static void Init() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}

		public static DrawingBoard Init(int width, int height, DrawMethod drawMethod) {
			Init();
			DrawingBoard db = new DrawingBoard(width, height, drawMethod);
			Application.Run(db);
			return db;
		}

		public static DrawingBoard Init(int width, int height, int x, int y, DrawMethod drawMethod) {
			Init();
			DrawingBoard db = new DrawingBoard(width, height, x, y, drawMethod);
			Application.Run(db);
			return db;
		}
	}
}
