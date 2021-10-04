using DrawingBoardNET.Drawing.Constants;

namespace DrawingBoardNET.Drawing
{
	internal struct Transform
	{
		internal TransformType Type { get; }
		internal float Value { get; }

		public Transform(TransformType type, float value) => (Type, Value) = (type, value);
	}
}
