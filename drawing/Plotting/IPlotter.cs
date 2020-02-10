using System.Collections.Generic;
using System.Drawing;
using drawingBoard.Drawing.Constants.Drawing;
using drawingBoard.Utils;

namespace drawingBoard.Drawing.Plotting {
	internal abstract class IPlotter {
		protected const int AXES_OFFSET = 25;
		protected Rectangle plotBounds;
		protected Rectangle axesBounds;
		protected Rectangle bounds;
		protected float zeroX;
		protected float zeroY;

		public abstract void Plot(DrawingBoard db, double[] xs, double[] ys);

		public abstract void Plot(DrawingBoard db, double[] xs, double[] ys, int x, int y, int width, int height);

		protected void InitPlot(DrawingBoard db, double[] xs, double[] ys, int x, int y, int width, int height) {
			plotBounds = new Rectangle(x, y, width, height);
			axesBounds = new Rectangle(x + AXES_OFFSET, y + AXES_OFFSET, width - AXES_OFFSET, height - AXES_OFFSET);
			bounds = new Rectangle(x, y, width, height);

			MinMax minMaxX = ArrayUtils.FindMinMax(xs);
			MinMax minMaxY = ArrayUtils.FindMinMax(ys);

			zeroX = (float) Mathlib.Misc.Utils.Lerp(0, minMaxX.min, minMaxX.max, axesBounds.Left, axesBounds.Right);
			zeroY = (float) Mathlib.Misc.Utils.Lerp(0, minMaxY.min, minMaxY.max, axesBounds.Bottom, axesBounds.Top);

			db.NoStroke();
			db.Fill(255);
			db.Rectangle(plotBounds);

			db.NoFill();
			db.StrokeWidth(2);
			db.Stroke(0);
			db.Rectangle(axesBounds);

			db.StrokeWidth(1);
			db.Line(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
		}

		protected void InitPlot(DrawingBoard db, double[] xs, double[] ys)
			=> InitPlot(db, xs, ys, 0, 0, db.Width, db.Height);

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
