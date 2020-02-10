using drawingBoard.Drawing.Constants.Drawing;

namespace drawingBoard.Drawing.Plotting {
	internal class LinePlotter : IPlotter {
		public override void Plot(DrawingBoard db, double[] xs, double[] ys)
			=> Plot(db, xs, ys, 0, 0, db.Width, db.Height);

		public override void Plot(DrawingBoard db, double[] xs, double[] ys, int x, int y, int width, int height) {
			InitPlot(db, xs, ys, x, y, width, height);


		}
	}
}
