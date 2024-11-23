using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/PositionOffset")]
public class PositionOffset : BaseBehaviour
{
	public Vector3 offset;

	public override void OnActivate()
	{
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		zResult.position += zResult.rotation * this.offset;
	}
}
