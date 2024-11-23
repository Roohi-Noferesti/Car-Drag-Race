using UnityEngine;

public class EditableEngineConsumablePowerCurve : EditableCurve
{
	public override void SetDefault()
	{
		this.CreateDefaultCurve();
	}

	private void CreateDefaultCurve()
	{
		this.animationCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(100f, 50f),
			new Keyframe(150f, 55f),
			new Keyframe(200f, 61f),
			new Keyframe(250f, 68f),
			new Keyframe(300f, 76f),
			new Keyframe(350f, 85f),
			new Keyframe(400f, 95f),
			new Keyframe(450f, 106f),
			new Keyframe(500f, 118f),
			new Keyframe(550f, 131f),
			new Keyframe(600f, 145f),
			new Keyframe(650f, 160f)
		});
	}
}
