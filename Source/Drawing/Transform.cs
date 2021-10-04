using DrawingBoardNET.Drawing.Constants;

namespace DrawingBoardNET.Drawing
{
	internal struct Transform
	{
		internal TransformType Type { get; }
		internal double Value { get; }

		public Transform(TransformType type, double value) => (Type, Value) = (type, value);
	}
}
