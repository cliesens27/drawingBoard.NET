namespace DrawingBoardNET.Window.UI;

public abstract class Slider
{
	public int HandleSize { get; }

	public double InitialValue { get; }

	public double MinValue { get; }

	public double MaxValue { get; }

	public string Label { get; }

	public abstract double Value { get; }

	public bool IsLocked { get; private set; }

	public Slider(string label, int handleSize, double minVal, double maxVal)
		: this(label, handleSize, minVal, maxVal, minVal) { }

	public Slider(string label, int handleSize, double minValue, double maxValue, double initialValue) =>
		(Label, HandleSize, MinValue, MaxValue, InitialValue) =
		(label, handleSize, minValue, maxValue, initialValue);

	internal abstract void Update(int mouseLocation);

	internal abstract bool IsSelected(int mx, int my);

	internal void Lock() => IsLocked = true;

	internal void Unlock() => IsLocked = false;
}
