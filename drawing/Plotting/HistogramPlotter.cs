using System;
using System.Linq;
using drawingBoard.Drawing.Constants;
using Mathlib;

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
			db.Fill(235);
			db.StrokeWidth(1);
			db.RectMode(RectangleMode.CORNER);

			int[] counts = ComputeCounts(ys, nbBins);
			float colWidth = (axesBounds.Right - axesBounds.Left) / nbBins;
			float maxCount = counts.Max() / (float) xs.Length;

			for (int i = 0; i < nbBins; i++) {
				float val = counts[i] / (float) xs.Length;
				float screenX = SpecialFunctions.Lerp(i, 0, nbBins, axesBounds.Left, axesBounds.Right);
				float screenY = SpecialFunctions.Lerp(val, 0, maxCount, axesBounds.Bottom, axesBounds.Top);

				db.Rectangle(screenX, axesBounds.Y + axesBounds.Height - screenY, colWidth, screenY);
			}
		}

		private int[] ComputeCounts(double[] ys, int nbBins) {
			double incr = (minMaxY.max - minMaxY.min) / nbBins;
			int[] counts = new int[nbBins];

			for (int i = 0; i < ys.Length; i++) {
				double val = ys[i];

				for (int j = 0; j < nbBins; j++) {
					double from = minMaxY.min + j * incr;
					double to = minMaxY.min + (j + 1) * incr;

					if (val >= from && val < to) {
						counts[j]++;
						break;
					}
				}
			}

			return counts;
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
