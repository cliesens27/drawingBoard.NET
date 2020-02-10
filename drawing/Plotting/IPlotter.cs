using System.Collections.Generic;
using System.Drawing;
using drawingBoard.Drawing.Constants.Drawing;
using drawingBoard.Utils;

namespace drawingBoard.Drawing.Plotting {
	internal abstract class IPlotter {
		protected const int AXES_OFFSET = 35;
		protected Rectangle plotBounds;
		protected Rectangle axesBounds;
		protected MinMax minMaxX;
		protected MinMax minMaxY;
		protected float zeroX;
		protected float zeroY;

		public abstract void Plot(DrawingBoard db, double[] xs, double[] ys);

		public abstract void Plot(DrawingBoard db, double[] xs, double[] ys, int x, int y, int width, int height);

		protected void InitPlot(DrawingBoard db, double[] xs, double[] ys, int x, int y, int width, int height) {
			minMaxX = ArrayUtils.FindMinMax(xs);
			minMaxY = ArrayUtils.FindMinMax(ys);

			plotBounds = new Rectangle(x, y, width, height);
			axesBounds = new Rectangle(x + AXES_OFFSET, y + AXES_OFFSET, width - 2 * AXES_OFFSET, height - 2 * AXES_OFFSET);

			zeroX = (float) Mathlib.Misc.Utils.Lerp(0, minMaxX.min, minMaxX.max, axesBounds.Left, axesBounds.Right);
			zeroY = (float) Mathlib.Misc.Utils.Lerp(0, minMaxY.min, minMaxY.max, axesBounds.Bottom, axesBounds.Top);

			DrawBackground(db);
			DrawAxes(db);
			LabelAxes(db);
		}

		protected void InitPlot(DrawingBoard db, double[] xs, double[] ys)
			=> InitPlot(db, xs, ys, 0, 0, db.Width, db.Height);

		private void DrawBackground(DrawingBoard db) {
			db.NoStroke();
			db.Fill(255);
			db.Rectangle(plotBounds);
		}

		private void DrawAxes(DrawingBoard db) {
			db.NoFill();
			db.StrokeWidth(2);
			db.Stroke(0);
			db.Rectangle(axesBounds);
		}

		private void LabelAxes(DrawingBoard db) {
			int fontSize = 14;

			db.Font(new Font("consolas", fontSize));
			db.Fill(0);

			db.DrawString($"{minMaxX.min}", axesBounds.Left - fontSize, axesBounds.Bottom);
			db.DrawString($"{minMaxX.max}", axesBounds.Right - fontSize, axesBounds.Bottom);

			if (minMaxY.min == minMaxY.max) {

			}
			else {

			}
		}

		public void Plot(DrawingBoard db, int[] xs, int[] ys)
			=> Plot(db, ArrayUtils.IntToDouble(xs), ArrayUtils.IntToDouble(ys));

		public void Plot(DrawingBoard db, float[] xs, float[] ys)
			=> Plot(db, ArrayUtils.FloatToDouble(xs), ArrayUtils.FloatToDouble(ys));

		public void Plot(DrawingBoard db, int[] xs, int[] ys, int x, int y, int width, int height)
			=> Plot(db, ArrayUtils.IntToDouble(xs), ArrayUtils.IntToDouble(ys), x, y, width, height);

		public void Plot(DrawingBoard db, float[] xs, float[] ys, int x, int y, int width, int height)
			=> Plot(db, ArrayUtils.FloatToDouble(xs), ArrayUtils.FloatToDouble(ys), x, y, width, height);

		public void Plot(DrawingBoard db, List<int> xs, List<int> ys)
			=> Plot(db, xs.ToArray(), ys.ToArray());

		public void Plot(DrawingBoard db, List<float> xs, List<float> ys)
			=> Plot(db, xs.ToArray(), ys.ToArray());

		public void Plot(DrawingBoard db, List<double> xs, List<double> ys)
			=> Plot(db, xs.ToArray(), ys.ToArray());

		public void Plot(DrawingBoard db, List<int> xs, List<int> ys, int x, int y, int width, int height)
			=> Plot(db, xs.ToArray(), ys.ToArray(), x, y, width, height);

		public void Plot(DrawingBoard db, List<float> xs, List<float> ys, int x, int y, int width, int height)
			=> Plot(db, xs.ToArray(), ys.ToArray(), x, y, width, height);

		public void Plot(DrawingBoard db, List<double> xs, List<double> ys, int x, int y, int width, int height)
			=> Plot(db, xs.ToArray(), ys.ToArray(), x, y, width, height);
	}
}
