using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/Position At Transform")]
public class PositionAtTransform : BaseBehaviour
{
	public Transform Target;

	public override void OnActivate()
	{
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		zResult.position = this.Target.position;
	}
}
