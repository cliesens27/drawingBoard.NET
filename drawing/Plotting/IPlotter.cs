using System.Collections.Generic;
using drawingBoard.Drawing.Constants.Drawing;

namespace drawingBoard.Drawing.Plotting {
	internal abstract class IPlotter {
		protected float xMin;
		protected float xMax;
		protected float yMin;
		protected float yMax;
		protected float zeroX;
		protected float zeroY;

		protected void InitPlot(DrawingBoard db, int[] xs, int[] ys) {
		}

		protected void InitPlot(DrawingBoard db, int[] xs, int[] ys, int x, int y, int width, int height) {
		}

		protected void InitPlot(DrawingBoard db, float[] xs, float[] ys) {
		}

		protected void InitPlot(DrawingBoard db, float[] xs, float[] ys, int x, int y, int width, int height) {
		}

		protected void InitPlot(DrawingBoard db, double[] xs, double[] ys) {
		}

		protected void InitPlot(DrawingBoard db, double[] xs, double[] ys, int x, int y, int width, int height) {
		}

		public abstract void Plot(DrawingBoard db, int[] xs, int[] ys);

		public abstract void Plot(DrawingBoard db, int[] xs, int[] ys, int x, int y, int width, int height);

		public abstract void Plot(DrawingBoard db, float[] xs, float[] ys);

		public abstract void Plot(DrawingBoard db, float[] xs, float[] ys, int x, int y, int width, int height);

		public abstract void Plot(DrawingBoard db, double[] xs, double[] ys);

		public abstract void Plot(DrawingBoard db, double[] xs, double[] ys, int x, int y, int width, int height);

		public void Plot(DrawingBoard db, List<int> xs, List<int> ys)
			=> Plot(db, xs.ToArray(), ys.ToArray());

		public void Plot(DrawingBoard db, List<int> xs, List<int> ys, int x, int y, int width, int height)
			=> Plot(db, xs.ToArray(), ys.ToArray(), x, y, width, height);

		public void Plot(DrawingBoard db, List<float> xs, List<float> ys)
			=> Plot(db, xs.ToArray(), ys.ToArray());

		public void Plot(DrawingBoard db, List<float> xs, List<float> ys, int x, int y, int width, int height)
			=> Plot(db, xs.ToArray(), ys.ToArray(), x, y, width, height);

		public void Plot(DrawingBoard db, List<double> xs, List<double> ys)
			=> Plot(db, xs.ToArray(), ys.ToArray());

		public void Plot(DrawingBoard db, List<double> xs, List<double> ys, int x, int y, int width, int height)
			=> Plot(db, xs.ToArray(), ys.ToArray(), x, y, width, height);
	}
}
