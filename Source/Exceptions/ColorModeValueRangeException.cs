using System;

using DrawingBoardNET.Drawing.Constants;

namespace DrawingBoardNET.Exceptions;

public class ColorModeValueRangeException : ArgumentOutOfRangeException
{
	public ColorModeValueRangeException(string component, int value, int maxValue, DBColorMode colorMode)
		: base(WriteErrorMessage(component, value, maxValue, colorMode)) { }

	private static string WriteErrorMessage(string component, int value, int maxValue, DBColorMode colorMode) =>
		$"\nError, the value of {component} should be between 0 and {maxValue} " +
		$"in {colorMode} color mode\n" +
		$"\t{component} = {value}\n";
}
