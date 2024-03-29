﻿using System;

using DrawingBoardNET.Drawing.Constants;

using static DrawingBoardNET.Drawing.Constants.RectangleMode;

namespace DrawingBoardNET.Window.UI;

public delegate void ButtonAction();

public class Button
{
	public int X { get; }

	public int Y { get; }

	public int Width { get; }

	public int Height { get; }

	public string Label { get; }

	public bool IsHovered { get; internal set; }

	public bool IsPressed { get; internal set; }

	internal ButtonAction Action { get; }

	public Button(string title, int x, int y, int w, int h, ButtonAction action) =>
		(Action, X, Y, Width, Height, Label) = (action, x, y, w, h, title);

	internal bool IsSelected(RectangleMode rectMode, int mouseX, int mouseY)
	{
		int halfW = Width / 2;
		int halfH = Height / 2;

		bool xSelected;
		bool ySelected;

		switch (rectMode)
		{
			case Center:
				xSelected = mouseX >= X - halfW && mouseX <= X + halfW;
				ySelected = mouseY >= Y - halfH && mouseY <= Y + halfH;

				return xSelected && ySelected;
			case Corner:
				xSelected = mouseX >= X && mouseX <= X + Width;
				ySelected = mouseY >= Y && mouseY <= Y + Height;

				return xSelected && ySelected;
			case Corners:
				xSelected = mouseX >= X && mouseX <= X + Width;
				ySelected = mouseY >= Y && mouseY <= Y + Height;

				return xSelected && ySelected;
			default:
				throw new ArgumentOutOfRangeException(
					$"The provided value of {nameof(rectMode)} " +
					$"is not valid; it must be one of {{{Center}, {Corner}, {Corners}}}"
				);
		}
	}
}
