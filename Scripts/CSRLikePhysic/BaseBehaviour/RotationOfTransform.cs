using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/Rotation Of Transform")]
public class RotationOfTransform : BaseBehaviour
{
	public Transform Target;

	public override void OnActivate()
	{
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		zResult.rotation = this.Target.rotation;
	}
}
