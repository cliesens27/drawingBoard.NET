using System;

namespace DrawingBoardNET.Drawing;

public static class MathUtils
{
	public static int RandomSeed
	{
		get => seed;
		set => (seed, rng) = (value, new Random(value));
	}

	private const double RadiansToDegreesConstant = 180 / Math.PI;
	private const double DegreesToRadiansConstant = Math.PI / 180;
	private static Random rng;
	private static int seed;

	static MathUtils()
	{
		RandomSeed = (int) DateTime.Now.Ticks;
		rng = new Random(seed);
	}

	public static double RadiansToDegrees(double radians) => radians * RadiansToDegreesConstant;

	public static double DegreesToRadians(double degrees) => degrees * DegreesToRadiansConstant;

	public static double Rand() => rng.NextDouble();

	public static int Rand(int max) => (int) (rng.NextDouble() * max);

	public static double Rand(double max) => rng.NextDouble() * max;

	public static int Rand(int min, int max)
	{
		if (min >= max)
		{
			throw new ArgumentException(
				$"Max should be smaller than min" +
				$"\n\tmin = {min}\n\tmax = {max}"
			);
		}

		return (int) (rng.NextDouble() * (max - min) + min);
	}

	public static double Rand(double min, double max)
	{
		if (min >= max)
		{
			throw new ArgumentException(
				$"Max should be smaller than min" +
				$"\n\tmin = {min}\n\tmax = {max}"
			);
		}

		return rng.NextDouble() * (max - min) + min;
	}

	public static double Lerp(double val, double x1, double x2, double y1, double y2)
	{
		if (val == x1)
		{
			return y1;
		}

		if (val == x2)
		{
			return y2;
		}

		return (y2 - y1) / (x2 - x1) * (val - x1) + y1;
	}
}
