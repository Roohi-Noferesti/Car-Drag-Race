public class InGameWheelSpinVsTyreGripCurve : InGameCurve
{
	public float MinWheelSpinValue
	{
		get
		{
			return base.minXAxis;
		}
	}

	public float MaxWheelSpinValue
	{
		get
		{
			return base.maxXAxis;
		}
	}

	public InGameWheelSpinVsTyreGripCurve() : base(100)
	{
	}

	public float EvaluateTyreGripAtWheelSpin(float zNormalisedWheelSpin)
	{
		return base.EvaluateYValueAtNormalisedXValue(zNormalisedWheelSpin);
	}
}
