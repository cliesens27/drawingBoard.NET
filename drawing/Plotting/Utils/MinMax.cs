namespace drawingBoard.Plotting.Utils
{
	internal struct MinMax
	{
		internal double Min { get; private set; }
		internal double Max { get; private set; }

		internal MinMax(double min, double max)
		{
			Min = min;
			Max = max;
		}
	}
}
