using DrawingBoardNET.Drawing;

namespace DrawingBoardNET.Window.UI
{
	public class VerticalSlider : Slider
	{
		public int CurrentY { get; private set; }
		public int X { get; private set; }
		public int Y1 { get; }
		public int Y2 { get; }

		public override double Value => MathUtils.Lerp(CurrentY, Y1, Y2, MinValue, MaxValue);

		public VerticalSlider(
				string label, int handleSize, double minVal, double maxVal, int x1, int x2, int y
			) : this(label, handleSize, minVal, maxVal, x1, x2, y, minVal) { }

		public VerticalSlider(
				string label, int handleSize, double minVal, double maxVal, int y1, int y2, int x, double initialVal
			) : base(label, handleSize, minVal, maxVal, initialVal) => (Y1, Y2, X) = (y1, y2, x);

		internal override void Update(int mouseY)
		{
			if (IsLocked)
			{
				CurrentY = mouseY;
				CurrentY = CurrentY < Y1 ? Y1 : CurrentY;
				CurrentY = CurrentY > Y2 ? Y2 : CurrentY;
			}
		}

		internal override bool IsSelected(int mx, int my)
			=> (mx - X) * (mx - X) + (my - CurrentY) * (my - CurrentY) <= (HandleSize * HandleSize);
	}
}
