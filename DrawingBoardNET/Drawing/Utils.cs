using System;

namespace DrawingBoardNET.Drawing
{
	public static class Utils
	{
		private static Random rng;
		private static int seed;

		public static int Seed
		{
			get => seed;
			set
			{
				seed = value;
				rng = new Random(seed);
			}
		}

		static Utils()
		{
			Seed = (int) DateTime.Now.Ticks;
			rng = new Random(seed);
		}

		public static double Rand() => rng.NextDouble();

		public static int Rand(int max) => (int) (rng.NextDouble() * max);

		public static float Rand(float max) => (float) (rng.NextDouble() * max);

		public static double Rand(double max) => rng.NextDouble() * max;

		public static int Rand(int min, int max)
		{
			if (min >= max)
			{
				throw new ArgumentException($"Max should be smaller than min" +
					$"\n\tmin = {min}\n\tmax = {max}");
			}

			return (int) (rng.NextDouble() * (max - min) + min);
		}

		public static float Rand(float min, float max)
		{
			if (min >= max)
			{
				throw new ArgumentException($"Max should be smaller than min" +
					$"\n\tmin = {min}\n\tmax = {max}");
			}

			return (float) (rng.NextDouble() * (max - min) + min);
		}

		public static double Rand(double min, double max)
		{
			if (min >= max)
			{
				throw new ArgumentException($"Max should be smaller than min" +
					$"\n\tmin = {min}\n\tmax = {max}");
			}

			return rng.NextDouble() * (max - min) + min;
		}
	}
}
