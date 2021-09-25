using DrawingBoardNET.Drawing;
using DrawingBoardNET.Window.UI;

namespace DrawingBoardTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			DrawingBoard db = new(600, 400);
			db.Title = "DrawingBoardTest";
			db.TargetFrameRate = 30;

			HorizontalSlider slider = new("", 20, 0, 255, 150, 450, db.Ycenter);
			db.AddSlider(slider);

			// This is executed once before the first call to Draw()
			db.Init = () =>
			{
				db.Stroke(255);
				db.StrokeWidth(2);
			};

			// This is executed once per frame
			db.Draw = () =>
			{
				db.Background(0, (int) slider.Value, 0);
			};

			// This is executed once per frame per slider
			db.DrawSlider = (s) =>
			{
				if (s is HorizontalSlider slider)
				{
					db.Fill(0, 255 - (int) slider.Value, 0);
					db.Line(slider.X1, slider.Y, slider.X2, slider.Y);
					db.Circle(slider.CurrentX, slider.Y, slider.HandleSize);
				}
			};

			// Run the sketch
			db.Start();
		}
	}
}
