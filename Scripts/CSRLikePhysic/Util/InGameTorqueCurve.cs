public class InGameTorqueCurve : InGameCurve
{
	public float MinRPMValue
	{
		get
		{
			return base.minXAxis;
		}
	}

	public float MaxRPMValue
	{
		get
		{
			return base.maxXAxis;
		}
	}

	public InGameTorqueCurve() : base(200)
	{
	}

	public float EvaluateTorqueAtNormalisedRPM(float zNormalisedRPM)
	{
		return base.EvaluateYValueAtNormalisedXValue(zNormalisedRPM);
	}
}
