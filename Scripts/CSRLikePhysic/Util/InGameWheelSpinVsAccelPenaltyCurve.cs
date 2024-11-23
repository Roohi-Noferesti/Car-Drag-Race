public class InGameWheelSpinVsAccelPenaltyCurve : InGameCurve
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

	public InGameWheelSpinVsAccelPenaltyCurve() : base(100)
	{
	}

	public float EvaluateAccelPenaltyAtWheelSpin(float zNormalisedWheelSpin)
	{
		return base.EvaluateYValueAtNormalisedXValue(zNormalisedWheelSpin);
	}
}
