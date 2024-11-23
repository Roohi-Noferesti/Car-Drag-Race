using UnityEngine;

public class InGameCurve
{
	protected float[] values;

	protected int numValues;

	public float minXAxis
	{
		get;
		private set;
	}

	public float maxXAxis
	{
		get;
		private set;
	}

	public InGameCurve(int numDataPoints)
	{
		this.numValues = numDataPoints;
	}

	public float GetValueAtIndex(int index)
	{
		return this.values[index];
	}

	public void SetFromAnimationCurve(AnimationCurve zCurve, float zExtraMultiplier = 1f)
	{
		this.values = new float[this.numValues];
		this.minXAxis = zCurve.keys[0].time;
		this.maxXAxis = zCurve.keys[zCurve.length - 1].time;
		float num = (this.maxXAxis - this.minXAxis) / (float)(this.numValues - 1);
		for (int i = 0; i < this.numValues; i++)
		{
			float time = num * (float)i + this.minXAxis;
			this.values[i] = zCurve.Evaluate(time) * zExtraMultiplier;
		}
	}

	public void AddTorqueCurve(InGameTorqueCurve zCurve)
	{
		for (int i = 0; i < this.numValues; i++)
		{
			this.values[i] += zCurve.GetValueAtIndex(i);
		}
	}

	public void TakeAwayTorqueCurve(InGameTorqueCurve zCurve)
	{
		for (int i = 0; i < this.numValues; i++)
		{
			this.values[i] -= zCurve.GetValueAtIndex(i);
		}
	}

	public float EvaluateYValueAtNormalisedXValue(float zXValue)
	{
		float num = 1f / (float)(this.numValues - 1);
		float f = zXValue / num;
		int num2 = (int)Mathf.Floor(f);
		num2 = Mathf.Clamp(num2, 0, this.numValues - 1);
		int num3 = num2 + 1;
		num3 = Mathf.Clamp(num3, 0, this.numValues - 1);
		float num4 = (zXValue - num * (float)num2) / num;
		float num5 = 1f - num4;
		return this.values[num2] * num5 + this.values[num3] * num4;
	}
}
