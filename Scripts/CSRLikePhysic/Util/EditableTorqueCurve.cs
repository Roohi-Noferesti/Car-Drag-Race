using System;
using UnityEngine;

[Serializable]
public class EditableTorqueCurve : EditableCurve
{
	public override void SetDefault()
	{
		this.CreateApproxBellCurve(ref this.animationCurve, 0f, 7500f, 350f, 25);
	}

	private void CreateApproxBellCurve(ref AnimationCurve curve, float minTime, float maxTime, float maxValue, int numKeys)
	{
		float num = (maxTime - minTime) / (float)(numKeys - 1);
		float num2 = (maxTime - minTime) / 2f;
		Keyframe[] array = new Keyframe[numKeys];
		for (int i = 0; i < numKeys; i++)
		{
			float num3 = (float)i * num + minTime;
			float num4 = (num3 - num2) / num2;
			float value = maxValue - num4 * num4 * maxValue;
			array[i] = new Keyframe(num3, value);
		}
		curve = new AnimationCurve(array);
	}

	public void CopyCurveValues(EditableTorqueCurve torqueCurveIn)
	{
		Keyframe[] array = new Keyframe[torqueCurveIn.animationCurve.keys.Length];
		for (int i = 0; i < torqueCurveIn.animationCurve.keys.Length; i++)
		{
			array[i] = default(Keyframe);
			array[i].time = torqueCurveIn.animationCurve.keys[i].time;
			array[i].value = torqueCurveIn.animationCurve.keys[i].value;
		}
		this.animationCurve = new AnimationCurve(array);
	}

	public EditableTorqueCurve Clone()
	{
		Keyframe[] array = new Keyframe[this.animationCurve.keys.Length];
		for (int i = 0; i < this.animationCurve.keys.Length; i++)
		{
			array[i] = default(Keyframe);
			array[i].time = this.animationCurve.keys[i].time;
			array[i].value = this.animationCurve.keys[i].value;
		}
		EditableTorqueCurve editableTorqueCurve = CreateInstance<EditableTorqueCurve>();
		editableTorqueCurve.animationCurve = new AnimationCurve(array);
		return editableTorqueCurve;
	}

	public void MultiplyCurve(float multiplier)
	{
		Keyframe[] array = new Keyframe[this.animationCurve.keys.Length];
		for (int i = 0; i < this.animationCurve.keys.Length; i++)
		{
			array[i] = default(Keyframe);
			array[i].time = this.animationCurve.keys[i].time;
			array[i].value = this.animationCurve.keys[i].value * multiplier;
		}
		this.animationCurve = new AnimationCurve(array);
	}

	public override string ToString()
	{
		string text = string.Empty;
		text = text + base.ToString() + "\n";
		text += "  Curve data : \n";
		int num = 0;
		Keyframe[] keys = this.animationCurve.keys;
		for (int i = 0; i < keys.Length; i++)
		{
			Keyframe keyframe = keys[i];
			if (num == 0)
			{
				text = text + "      RPM : " + (int)keyframe.time;
			}
			else
			{
				text = text + "            " + (int)keyframe.time;
			}
			if (num == 0)
			{
				text = text + "   Torque : " + keyframe.value;
			}
			else
			{
				text = text + "            " + keyframe.value;
			}
			text += "\n";
		}
		return text;
	}
}
