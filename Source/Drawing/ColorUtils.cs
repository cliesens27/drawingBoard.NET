using System;
using System.Drawing;

namespace DrawingBoardNET.Drawing;

public static class ColorUtils
{
	public static Color HSBtoRGB(int hue, int saturation, int brightness)
	{
		double h = 360 * (hue / 255.0);
		double s = saturation / 255.0;
		double b = brightness;

		double f = h / 60 - Math.Floor(h / 60);

		int w = brightness;
		int x = (int) (b * (1 - s));
		int y = (int) (b * (1 - f * s));
		int z = (int) (b * (1 - (1 - f) * s));

		int i = (int) Math.Floor(h / 60) % 6;

		return i switch
		{
			0 => Color.FromArgb(255, w, z, x),
			1 => Color.FromArgb(255, y, w, x),
			2 => Color.FromArgb(255, x, w, z),
			3 => Color.FromArgb(255, x, y, w),
			4 => Color.FromArgb(255, z, x, w),
			_ => Color.FromArgb(255, w, x, y),
		};
	}

	public static Color HSLtoRGB(int hue, int saturation, int lightness)
	{
		double h = 360 * (hue / 255.0);
		double s = saturation / 255.0;
		double l = lightness / 255.0;

		int r, g, b;

		if (saturation == 0)
		{
			r = g = b = lightness;
		}
		else
		{
			double q1, q2;
			double qHue = h / 6;

			if (l < 0.5)
			{
				q2 = l * (1 + s);
			}
			else
			{
				q2 = l + s - l * s;
			}

			q1 = 2d * s - q2;

			double tr, tg, tb;
			tr = qHue + 1.0 / 3.0;
			tg = qHue;
			tb = qHue - 1.0 / 3.0;

			tr = HSLColorComponent(tr, q1, q2);
			tg = HSLColorComponent(tg, q1, q2);
			tb = HSLColorComponent(tb, q1, q2);

			r = (int) Math.Round(tr * 255);
			g = (int) Math.Round(tg * 255);
			b = (int) Math.Round(tb * 255);
		}

		return Color.FromArgb(r, g, b);
	}

	public static double HSLColorComponent(double q1, double q2, double q3)
	{
		if (q1 < 0)
		{
			q1 += 1;
		}

		if (q1 > 1)
		{
			q1 -= 1;
		}

		if (q1 < 1.0 / 6.0)
		{
			return q2 + (q3 - q2) * 6 * q1;
		}

		if (q1 < 0.5)
		{
			return q3;
		}

		if (q1 < 2.0 / 3.0)
		{
			return q2 + (q3 - q2) * (2.0 / 3.0 - q1) * 6;
		}

		return q2;
	}
}
