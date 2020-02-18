using Mathlib;

namespace drawingBoard.Drawing.Plotting {
	public class LinePlotter : IPlotter {
		private const double X_SCALE = 1;
		private const double Y_SCALE = 1.1;

		public override void Plot(DrawingBoard db, double[] xs, double[] ys)
			=> Plot(db, xs, ys, 0, 0, db.Width, db.Height);

		public override void Plot(DrawingBoard db, double[] xs, double[] ys,
			int x, int y, int width, int height) {
			InitPlot(db, X_SCALE, Y_SCALE, xs, ys, x, y, width, height);

			db.Stroke(0);
			db.StrokeWidth(1.5f);

			float oldX = (float) SpecialFunctions.Lerp(xs[0], minX, maxY, axesBounds.Left, axesBounds.Right);
			float oldY = (float) SpecialFunctions.Lerp(ys[0], minY, maxY, axesBounds.Bottom, axesBounds.Top);

			for (int i = 1; i < xs.Length; i++) {
				float screenX = (float) SpecialFunctions.Lerp(xs[i], minX, maxY, axesBounds.Left, axesBounds.Right);
				float screenY = (float) SpecialFunctions.Lerp(ys[i], minY, maxY, axesBounds.Bottom, axesBounds.Top);

				db.Line(oldX, oldY, screenX, screenY);

				oldX = screenX;
				oldY = screenY;
			}
		}
	}
}
