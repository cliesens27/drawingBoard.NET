using System;
using System.Linq;
using drawingBoard.Drawing.Constants;
using drawingBoard.Utils;
using Mathlib;

namespace drawingBoard.Drawing.Plotting
{
	public class HistogramPlotter : IPlotter
	{
		private const double X_SCALE = 1;
		private const double Y_SCALE = 1.2;
		private const int MIN_NB_BINS = 5;
		private const int MAX_NB_BINS = 20;

		public override void Plot(DrawingBoard db, double[] xs, double[] ys)
			=> throw new NotImplementedException();

		public override void Plot(DrawingBoard db, double[] xs, double[] ys,
			int x, int y, int width, int height)
			=> throw new NotImplementedException();

		public void Plot(DrawingBoard db, double[] xs)
			=> Plot(db, xs, 0, 0, db.Width, db.Height);

		public void Plot(DrawingBoard db, double[] data, int x, int y, int width, int height)
		{
			int nbBins = ComputeNbBins(data);

			Plot(db, data, x, y, width, height, nbBins);
		}

		public void Plot(DrawingBoard db, double[] data, int nbBins)
			=> Plot(db, data, 0, 0, db.Width, db.Height, nbBins);

		public void Plot(DrawingBoard db, double[] data, int x, int y,
			int width, int height, int nbBins)
		{
			if (data.Length < 2 * MIN_NB_BINS)
			{
				throw new ArgumentException($"X has too few entries ({data.Length}) to plot a histogram\n\t" +
					$"X should have at least {2 * MIN_NB_BINS} entries");
			}

			MinMax minMaxY = Utils.ArrayUtils.FindMinMax(data);

			minY = (float) minMaxY.min;
			maxY = (float) minMaxY.max;

			int[] counts = ComputeCounts(data, nbBins);
			double incr = (maxY - minY) / nbBins;

			double[] xs = new double[nbBins + 1];
			double[] ys = new double[nbBins + 1];

			for (int i = 0; i < nbBins; i++)
			{
				xs[i] = minY + i * incr;
				ys[i] = counts[i] / (double) data.Length;
			}

			xs[nbBins] = maxY;

			InitPlot(db, X_SCALE, Y_SCALE, xs, ys, x, y, width, height);

			PlotHistogram(db, data, counts, ys, nbBins);
		}

		private void PlotHistogram(DrawingBoard db, double[] data, int[] counts, double[] ys, int nbBins)
		{
			db.RectMode(RectangleMode.CORNER);
			db.Stroke(0);
			db.Fill(230);
			db.StrokeWidth(1);

			float colWidth = (axesBounds.Right - axesBounds.Left) / nbBins;
			float maxY = (float) (Y_SCALE * counts.Max() / data.Length);

			for (int i = 0; i < nbBins; i++)
			{
				float screenX = SpecialFunctions.Lerp(i, 0, nbBins, axesBounds.Left, axesBounds.Right);
				float screenY = (float) SpecialFunctions.Lerp(ys[i], 0, maxY, 0, axesBounds.Height);

				db.Rectangle(screenX, axesBounds.Bottom - screenY, colWidth, screenY);
			}
		}

		private int[] ComputeCounts(double[] data, int nbBins)
		{
			double incr = (maxY - minY) / nbBins;
			int[] counts = new int[nbBins];

			for (int i = 0; i < data.Length; i++)
			{
				double val = data[i];

				for (int j = 0; j < nbBins; j++)
				{
					double from = minY + j * incr;
					double to = minY + (j + 1) * incr;

					if (val >= from && val < to)
					{
						counts[j]++;
						break;
					}
				}
			}

			return counts;
		}

		private int ComputeNbBins(double[] data)
		{
			//int n = (int) Math.Sqrt(xs.Length);
			//int n = 1 + (int) Math.Log(xs.Length, 2);
			int n = (int) (2 * Math.Pow(data.Length, 1.0 / 3.0));

			n = n > MAX_NB_BINS ? MAX_NB_BINS : n;
			n = n < MIN_NB_BINS ? MIN_NB_BINS : n;

			return n;
		}
	}
}
