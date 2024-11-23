using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/DefaultRotation")]
public class DefaultRotation : BaseBehaviour
{
	public override void OnActivate()
	{
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		zResult.rotation = base.transform.rotation;
	}
}
