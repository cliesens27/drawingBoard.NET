using System;
using DrawingBoardNET.Drawing.Constants;

namespace DrawingBoardNET.Window.UI
{
	public delegate void ButtonAction();

	public class Button : UIElement
	{
		public ButtonAction Trigger { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public string Title { get; private set; }
		public bool IsHovered { get; internal set; }
		public bool IsPressed { get; internal set; }

		public Button(string title, int x, int y, int w, int h, ButtonAction trigger) : base(x, y)
		{
			Title = title;
			Trigger = trigger;
			Width = w;
			Height = h;
			IsHovered = false;
			IsPressed = false;
		}

		public bool IsSelected(RectangleMode rectMode, int mouseX, int mouseY)
		{
			int halfW = Width / 2;
			int halfH = Height / 2;

			bool xSelected;
			bool ySelected;

			switch (rectMode)
			{
				case RectangleMode.Center:
					xSelected = (mouseX >= X - halfW && mouseX <= X + halfW);
					ySelected = (mouseY >= Y - halfH && mouseY <= Y + halfH);
					return xSelected && ySelected;
				case RectangleMode.Corner:
					xSelected = (mouseX >= X && mouseX <= X + Width);
					ySelected = (mouseY >= Y && mouseY <= Y + Height);
					return xSelected && ySelected;
				case RectangleMode.Corners:
					xSelected = (mouseX >= X && mouseX <= X + Width);
					ySelected = (mouseY >= Y && mouseY <= Y + Height);
					return xSelected && ySelected;
				default:
					throw new ArgumentOutOfRangeException("The provided value of rectMode is not valid; " +
						"it must be one of {CENTER, CORNER, CORNERS}");
			}
		}
	}
}
