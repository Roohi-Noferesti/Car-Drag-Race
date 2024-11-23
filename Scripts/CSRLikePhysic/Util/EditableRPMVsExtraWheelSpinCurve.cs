using System;
using UnityEngine;

[Serializable]
public class EditableRPMVsExtraWheelSpinCurve : EditableCurve
{
	public override void SetDefault()
	{
		this.CreateDefaultCurve();
	}

	private void CreateDefaultCurve()
	{
		this.animationCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.1f, 0f),
			new Keyframe(0.2767f, 0.027f),
			new Keyframe(0.4555f, 0.0388f),
			new Keyframe(0.7316f, 0.061f),
			new Keyframe(0.9196f, 0.0813f),
			new Keyframe(1f, 0.1161f)
		});
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
				text = text + "          RPM : " + (int)keyframe.time;
			}
			else
			{
				text = text + "            " + (int)keyframe.time;
			}
			if (num == 0)
			{
				text = text + "   Extra Spin : " + keyframe.value;
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
