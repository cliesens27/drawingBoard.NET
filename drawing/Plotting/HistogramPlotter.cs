namespace drawingBoard.Drawing.Plotting {
	public class HistogramPlotter : IPlotter {
		private const int MIN_NB_BINS = 5;
		private const int MAX_NB_BINS = 20;

		public override void Plot(DrawingBoard db, double[] xs, double[] ys)
			=> Plot(db, xs, ys, 0, 0, db.Width, db.Height);

		public override void Plot(DrawingBoard db, double[] xs, double[] ys,
			int x, int y, int width, int height) {
			int nbBins = 0;	// TODO

			Plot(db, xs, ys, x, y, width, height, nbBins);
		}

		public void Plot(DrawingBoard db, double[] xs, double[] ys, int nbBins)
			=> Plot(db, xs, ys, 0, 0, db.Width, db.Height);

		public void Plot(DrawingBoard db, double[] xs, double[] ys,
			int x, int y, int width, int height, int nbBins) {
			InitPlot(db, xs, ys, x, y, width, height);

			db.Stroke(0);
			db.Fill(255);
			db.StrokeWidth(1);

			// TODO
		}
	}
}
