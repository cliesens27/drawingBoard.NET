using DrawingBoardNET.Drawing;

namespace DrawingBoardNET.Window.UI
{
	public class Slider : UIElement
	{
		public int Radius { get; private set; }
		public int MinX { get; private set; }
		public int MaxX { get; private set; }
		public float MinValue { get; private set; }
		public float MaxValue { get; private set; }
		public bool IsLocked { get; private set; }

		public float Value => DrawingBoard.Lerp(X, MinX, MaxX, MinValue, MaxValue);

		public Slider(int minX, int maxX, float minVal, float maxVal, int y, int r)
			: this(minX, maxX, minVal, maxVal, y, r, (minX + maxX) / 2) { }

		public Slider(int minX, int maxX, float minVal, float maxVal, int y, int r, int x) : base(x, y)
		{
			Radius = r;
			MinX = minX;
			MaxX = maxX;
			MinValue = minVal;
			MaxValue = maxVal;

			IsLocked = false;
		}

		public void Update(int mouseX)
		{
			if (IsLocked)
			{
				X = mouseX;

				X = X < MinX ? MinX : X;
				X = X > MaxX ? MaxX : X;
			}
		}

		public bool IsSelected(int mouseX, int mouseY)
		{
			int diffX = mouseX - X;
			int diffY = mouseY - Y;

			return (diffX * diffX + diffY * diffY) <= Radius * Radius;
		}

		public void Lock() => IsLocked = true;

		public void Unlock() => IsLocked = false;
	}
}
