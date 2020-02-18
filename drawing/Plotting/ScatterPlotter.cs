using Mathlib;

namespace drawingBoard.Drawing.Plotting {
	public class ScatterPlotter : IPlotter {
		private const double X_SCALE = 1.01;
		private const double Y_SCALE = 1.01;

		public override void Plot(DrawingBoard db, double[] xs, double[] ys)
			=> Plot(db, xs, ys, 0, 0, db.Width, db.Height);

		public override void Plot(DrawingBoard db, double[] xs, double[] ys,
			int x, int y, int width, int height) {
			InitPlot(db, X_SCALE, Y_SCALE, xs, ys, x, y, width, height);

			db.NoStroke();
			db.Fill(0);

			for (int i = 0; i < xs.Length; i++) {
				float screenX = (float) SpecialFunctions.Lerp(xs[i], minX, maxX, axesBounds.Left, axesBounds.Right);
				float screenY = (float) SpecialFunctions.Lerp(ys[i], minY, maxY, axesBounds.Bottom, axesBounds.Top);

				db.Circle(screenX, screenY, 3);
			}
		}
	}
}
