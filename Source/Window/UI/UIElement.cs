namespace DrawingBoardNET.Window.UI
{
	public abstract class UIElement
	{
		public int X { get; protected set; }
		public int Y { get; protected set; }

		public UIElement(int x, int y) => (X, Y) = (x, y);
	}
}
