namespace drawingBoard.Utils {
	public static class ArrayUtils {
		public static double[] IntToDouble(int[] numbers) {
			double[] res = new double[numbers.Length];

			for (int i = 0; i < res.Length; i++) {
				res[i] = numbers[i];
			}

			return res;
		}

		public static double[] FloatToDouble(float[] numbers) {
			double[] res = new double[numbers.Length];

			for (int i = 0; i < res.Length; i++) {
				res[i] = numbers[i];
			}

			return res;
		}
	}
}
