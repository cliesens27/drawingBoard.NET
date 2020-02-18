using System;
using System.Collections.Generic;
using System.Drawing;
using drawingBoard.Drawing.Constants;
using drawingBoard.Utils;
using Mathlib.Functions;

namespace drawingBoard.Drawing.Plotting {
	public abstract class IPlotter {
		protected int axesOffset;
		protected Rectangle canvasBounds;
		protected Rectangle axesBounds;
		protected float minX;
		protected float maxX;
		protected float minY;
		protected float maxY;
		protected float zeroX;
		protected float zeroY;

		public abstract void Plot(DrawingBoard db, double[] xs, double[] ys);

		public abstract void Plot(DrawingBoard db, double[] xs, double[] ys, int x, int y, int width, int height);

		protected void InitPlot(DrawingBoard db, double xScale, double yScale,
			double[] xs, double[] ys, int x, int y, int width, int height) {
			if (xs.Length != ys.Length) {
				throw new ArgumentException($"X ({xs.Length}) and Y ({ys.Length}) dimensions do not match");
			}

			axesOffset = (int) (0.05 * 0.5 * (width + height));

			MinMax minMaxX = ArrayUtils.FindMinMax(xs);
			MinMax minMaxY = ArrayUtils.FindMinMax(ys);

			minX = (float) (xScale * minMaxX.min);
			maxX = (float) (xScale * minMaxX.max);
			minY = (float) (yScale * minMaxY.min);
			maxY = (float) (yScale * minMaxY.max);

			canvasBounds = new Rectangle(x, y, width, height);
			axesBounds = new Rectangle(x + axesOffset, y + axesOffset, width - 2 * axesOffset, height - 2 * axesOffset);

			zeroX = (float) Mathlib.SpecialFunctions.Lerp(0, minMaxX.min, minMaxX.max, axesBounds.Left, axesBounds.Right);
			zeroY = (float) Mathlib.SpecialFunctions.Lerp(0, minMaxY.min, minMaxY.max, axesBounds.Bottom, axesBounds.Top);

			DrawBackground(db);
			DrawAxes(db);
			LabelAxes(db);
		}

		protected void InitPlot(DrawingBoard db, double xScale, double yScale,
			double[] xs, double[] ys)
			=> InitPlot(db, xScale, yScale, xs, ys, 0, 0, db.Width, db.Height);

		private void DrawBackground(DrawingBoard db) {
			db.NoStroke();
			db.Fill(245);
			db.Rectangle(canvasBounds);

			db.Fill(255);
			db.Rectangle(axesBounds);
		}

		private void DrawAxes(DrawingBoard db) {
			db.NoFill();
			db.StrokeWidth(2);
			db.Stroke(0);
			db.Rectangle(axesBounds);
		}

		private void LabelAxes(DrawingBoard db) {
			float fontSize = 12;

			db.Font(new Font("cambria", fontSize));
			db.Fill(0);
			db.TextAlign(TextAlignment.RIGHT);

			db.Text($"{minX.ToString("0.00")}".Replace(',', '.'),
				axesBounds.Left + 1.5f * fontSize, axesBounds.Bottom + 2.5f * fontSize);

			db.Text($"{maxX.ToString("0.00")}".Replace(',', '.'),
				axesBounds.Right + 1.5f * fontSize, axesBounds.Bottom + 2.5f * fontSize);

			db.Text($"{minY.ToString("0.00")}".Replace(',', '.'),
				axesBounds.Left - 0.75f * fontSize, axesBounds.Bottom + 0.7f * fontSize);

			db.Text($"{maxY.ToString("0.00")}".Replace(',', '.'),
				axesBounds.Left - 0.75f * fontSize, axesBounds.Top + fontSize);
		}

		#region Plot from Array - Array

		public void Plot(DrawingBoard db, int[] xs, int[] ys)
			=> Plot(db, ArrayUtils.IntToDouble(xs), ArrayUtils.IntToDouble(ys));

		public void Plot(DrawingBoard db, float[] xs, float[] ys)
			=> Plot(db, ArrayUtils.FloatToDouble(xs), ArrayUtils.FloatToDouble(ys));

		public void Plot(DrawingBoard db, int[] xs, int[] ys, int x, int y, int width, int height)
			=> Plot(db, ArrayUtils.IntToDouble(xs), ArrayUtils.IntToDouble(ys), x, y, width, height);

		public void Plot(DrawingBoard db, float[] xs, float[] ys, int x, int y, int width, int height)
			=> Plot(db, ArrayUtils.FloatToDouble(xs), ArrayUtils.FloatToDouble(ys), x, y, width, height);

		#endregion

		#region Plot from List - List

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

		#endregion

		#region Plot from Array - Function

		public void Plot(DrawingBoard db, int[] xs, OneVarFunction f)
			=> Plot(db, xs, f, 0, 0, db.Width, db.Height);

		public void Plot(DrawingBoard db, float[] xs, OneVarFunction f)
			=> Plot(db, xs, f, 0, 0, db.Width, db.Height);

		public void Plot(DrawingBoard db, double[] xs, OneVarFunction f)
			=> Plot(db, xs, f, 0, 0, db.Width, db.Height);

		public void Plot(DrawingBoard db, int[] xs, OneVarFunction f, int x, int y, int width, int height) {
			double[] ys = new double[xs.Length];

			for (int i = 0; i < ys.Length; i++) {
				ys[i] = f(xs[i]);
			}

			Plot(db, ArrayUtils.IntToDouble(xs), ys, x, y, width, height);
		}

		public void Plot(DrawingBoard db, float[] xs, OneVarFunction f, int x, int y, int width, int height) {
			double[] ys = new double[xs.Length];

			for (int i = 0; i < ys.Length; i++) {
				ys[i] = f(xs[i]);
			}

			Plot(db, ArrayUtils.FloatToDouble(xs), ys, x, y, width, height);
		}

		public void Plot(DrawingBoard db, double[] xs, OneVarFunction f, int x, int y, int width, int height) {
			double[] ys = new double[xs.Length];

			for (int i = 0; i < ys.Length; i++) {
				ys[i] = f(xs[i]);
			}

			Plot(db, xs, ys, x, y, width, height);
		}

		#endregion

		#region Plot from List - Function

		public void Plot(DrawingBoard db, List<int> xs, OneVarFunction f)
			=> Plot(db, xs, f, 0, 0, db.Width, db.Height);

		public void Plot(DrawingBoard db, List<float> xs, OneVarFunction f)
			=> Plot(db, xs, f, 0, 0, db.Width, db.Height);

		public void Plot(DrawingBoard db, List<double> xs, OneVarFunction f)
			=> Plot(db, xs, f, 0, 0, db.Width, db.Height);

		public void Plot(DrawingBoard db, List<int> xs, OneVarFunction f, int x, int y, int width, int height) {
			double[] ys = new double[xs.Count];

			for (int i = 0; i < ys.Length; i++) {
				ys[i] = f(xs[i]);
			}

			Plot(db, ArrayUtils.IntToDouble(xs.ToArray()), ys, x, y, width, height);
		}

		public void Plot(DrawingBoard db, List<float> xs, OneVarFunction f, int x, int y, int width, int height) {
			double[] ys = new double[xs.Count];

			for (int i = 0; i < ys.Length; i++) {
				ys[i] = f(xs[i]);
			}

			Plot(db, ArrayUtils.FloatToDouble(xs.ToArray()), ys, x, y, width, height);
		}

		public void Plot(DrawingBoard db, List<double> xs, OneVarFunction f, int x, int y, int width, int height) {
			double[] ys = new double[xs.Count];

			for (int i = 0; i < ys.Length; i++) {
				ys[i] = f(xs[i]);
			}

			Plot(db, xs.ToArray(), ys, x, y, width, height);
		}

		#endregion

		#region Plot from Function

		public void Plot(DrawingBoard db, double a, double b, int n, OneVarFunction f) {
			double step = (b - a) / (n - 1);

			double[] xs = new double[n];
			double[] ys = new double[n];

			int i = 0;
			for (double x_ = a; x_ < b; x_ += step) {
				ys[i] = f(xs[i]);
				i++;
			}

			xs[n - 1] = b;
			ys[n - 1] = f(xs[n - 1]);

			Plot(db, xs, ys, 0, 0, db.Width, db.Height);
		}

		public void Plot(DrawingBoard db, double a, double b, int n, OneVarFunction f,
			int x, int y, int width, int height) {
			double step = (b - a) / (n - 1);

			double[] xs = new double[n];
			double[] ys = new double[n];

			int i = 0;
			for (double x_ = a; x_ < b; x_ += step) {
				ys[i] = f(xs[i]);
				i++;
			}

			xs[n - 1] = b;
			ys[n - 1] = f(xs[n - 1]);

			Plot(db, xs, ys, x, y, width, height);
		}

		public void Plot(DrawingBoard db, double a, double b, double step, OneVarFunction f) {
			int n = 1 + (int) ((b - a) / step);

			double[] xs = new double[n];
			double[] ys = new double[n];

			int i = 0;
			for (double x_ = a; x_ < b; x_ += step) {
				ys[i] = f(xs[i]);
				i++;
			}

			xs[n - 1] = b;
			ys[n - 1] = f(xs[n - 1]);

			Plot(db, xs, ys, 0, 0, db.Width, db.Height);
		}

		public void Plot(DrawingBoard db, double a, double b, double step, OneVarFunction f,
			int x, int y, int width, int height) {
			int n = 1 + (int) ((b - a) / step);

			double[] xs = new double[n];
			double[] ys = new double[n];

			int i = 0;
			for (double x_ = a; x_ < b; x_ += step) {
				ys[i] = f(xs[i]);
				i++;
			}

			xs[n - 1] = b;
			ys[n - 1] = f(xs[n - 1]);

			Plot(db, xs, ys, x, y, width, height);
		}

		#endregion
	}
}
