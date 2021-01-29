using DrawingBoardNET.Drawing;

namespace DrawingBoardNET.Window.UI
{
	public class Slider : UIElement
	{
		public int Radius { get; }
		public int MinX { get; }
		public int MaxX { get; }
		public float MinValue { get; }
		public float MaxValue { get; }

		public bool IsLocked { get; private set; } = false;

		public float Value => DrawingBoard.Lerp(X, MinX, MaxX, MinValue, MaxValue);

		public Slider(int minX, int maxX, float minVal, float maxVal, int y, int r)
			: this(minX, maxX, minVal, maxVal, y, r, (minX + maxX) / 2) { }

		public Slider(int minX, int maxX, float minVal, float maxVal, int y, int r, int x) : base(x, y)
			=> (Radius, MinX, MaxX, MinValue, MaxValue) = (r, minX, maxX, minVal, maxVal);

		internal void Update(int mouseX)
		{
			if (IsLocked)
			{
				X = mouseX;
				X = X < MinX ? MinX : X;
				X = X > MaxX ? MaxX : X;
			}
		}

		internal bool IsSelected(int mx, int my)
			=> ((mx - X) * (mx - X) + (my - Y) * (my - Y)) <= Radius * Radius;

		internal void Lock() => IsLocked = true;

		internal void Unlock() => IsLocked = false;
	}
}
