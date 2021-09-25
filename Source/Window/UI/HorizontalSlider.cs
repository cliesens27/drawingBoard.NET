using DrawingBoardNET.Drawing;

namespace DrawingBoardNET.Window.UI
{
	public class HorizontalSlider : Slider
	{
		public int CurrentX { get; private set; }
		public int Y { get; private set; }
		public int X1 { get; }
		public int X2 { get; }

		public override double Value => DrawingBoard.Lerp(CurrentX, X1, X2, MinValue, MaxValue);

		public HorizontalSlider(
				string label, int handleSize, double minVal, double maxVal, int x1, int x2, int y
			) : this(label, handleSize, minVal, maxVal, x1, x2, y, minVal) { }

		public HorizontalSlider(
				string label, int handleSize, double minVal, double maxVal, int x1, int x2, int y, double initialVal
			) : base(label, handleSize, minVal, maxVal, initialVal)
			=> (X1, X2, Y, CurrentX) = (x1, x2, y, (int) DrawingBoard.Lerp(initialVal, minVal, maxVal, x1, x2));

		internal override void Update(int mouseX)
		{
			if (IsLocked)
			{
				CurrentX = mouseX;
				CurrentX = CurrentX < X1 ? X1 : CurrentX;
				CurrentX = CurrentX > X2 ? X2 : CurrentX;
			}
		}

		internal override bool IsSelected(int mx, int my)
			=> (mx - CurrentX) * (mx - CurrentX) + (my - Y) * (my - Y) <= (HandleSize * HandleSize);
	}
}
