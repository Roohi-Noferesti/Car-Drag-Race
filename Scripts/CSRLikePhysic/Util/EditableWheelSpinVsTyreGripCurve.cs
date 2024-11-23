using System;
using UnityEngine;

[Serializable]
public class EditableWheelSpinVsTyreGripCurve : EditableCurve
{
	public override void SetDefault()
	{
		this.CreateDefaultCurve();
	}

	private void CreateDefaultCurve()
	{
		this.animationCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(0.2f, 1f),
			new Keyframe(0.4f, 1f),
			new Keyframe(0.6f, 1f),
			new Keyframe(0.8f, 1f),
			new Keyframe(1f, 1f)
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
				text = text + "          Wheel Spin : " + string.Format("{0:0.000}", keyframe.time);
			}
			else
			{
				text = text + "            " + string.Format("{0:0.000}", keyframe.time);
			}
			if (num == 0)
			{
				text = text + "   Tyre Grip Multiplier : " + keyframe.value;
			}
			else
			{
				text = text + "                          " + keyframe.value;
			}
			text += "\n";
		}
		return text;
	}
}
