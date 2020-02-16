using System;

namespace drawingBoard.Drawing.Plotting {
	public class HistogramPlotter : IPlotter {
		private const int MIN_NB_BINS = 5;
		private const int MAX_NB_BINS = 20;

		public override void Plot(DrawingBoard db, double[] xs, double[] ys)
			=> Plot(db, xs, ys, 0, 0, db.Width, db.Height);

		public override void Plot(DrawingBoard db, double[] xs, double[] ys,
			int x, int y, int width, int height) {
			int nbBins = ComputeNbBins(xs);

			Plot(db, xs, ys, x, y, width, height, nbBins);
		}

		public void Plot(DrawingBoard db, double[] xs, double[] ys, int nbBins)
			=> Plot(db, xs, ys, 0, 0, db.Width, db.Height);

		public void Plot(DrawingBoard db, double[] xs, double[] ys,
			int x, int y, int width, int height, int nbBins) {
			if (xs.Length < 2 * MIN_NB_BINS) {
				throw new ArgumentException($"X has too few entries ({xs.Length}) to plot a histogram\n\t" +
					$"X.Length should be at least {2 * MIN_NB_BINS}");
			}

			InitPlot(db, xs, ys, x, y, width, height);

			db.Stroke(0);
			db.Fill(255);
			db.StrokeWidth(1);

			// TODO
		}

		private int ComputeNbBins(double[] xs) {
			//int n = (int) Math.Sqrt(xs.Length);
			//int n = 1 + (int) Math.Log(xs.Length, 2);
			int n = (int) (2 * Math.Pow(xs.Length, 1.0 / 3.0));

			n = n > MAX_NB_BINS ? MAX_NB_BINS : n;
			n = n < MIN_NB_BINS ? MIN_NB_BINS : n;

			return n;
		}
	}
}
