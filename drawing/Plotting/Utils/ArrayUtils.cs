namespace drawingBoard.Plotting.Utils
{
	internal static class ArrayUtils
	{
		internal static double[] IntToDouble(int[] numbers)
		{
			double[] res = new double[numbers.Length];

			for (int i = 0; i < res.Length; i++)
			{
				res[i] = numbers[i];
			}

			return res;
		}

		internal static double[] FloatToDouble(float[] numbers)
		{
			double[] res = new double[numbers.Length];

			for (int i = 0; i < res.Length; i++)
			{
				res[i] = numbers[i];
			}

			return res;
		}

		internal static MinMax FindMinMax(double[] numbers)
		{
			double min = double.MaxValue;
			double max = double.MinValue;

			for (int i = 0; i < numbers.Length; i++)
			{
				double val = numbers[i];

				min = val < min ? val : min;
				max = val > max ? val : max;
			}

			return new MinMax(min, max);
		}
	}
}
