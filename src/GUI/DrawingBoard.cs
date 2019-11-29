using System.Drawing;
using System.Windows.Forms;

namespace drawingBoard.GUI {
	public partial class DrawingBoard : Form {
		private DrawingBoard() {
			InitializeComponent();

			ClientSize = new Size(1, 1);
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Program";
		}

		public DrawingBoard(int width, int height) : this() {
			ClientSize = new Size(width, height);
		}

		public DrawingBoard(int width, int height, int x, int y) : this(width, height) {
			Location = new Point(x, y);
		}
	}
}
