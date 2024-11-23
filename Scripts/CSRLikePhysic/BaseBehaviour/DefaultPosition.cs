using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/DefaultPosition")]
public class DefaultPosition : BaseBehaviour
{
	public override void OnActivate()
	{
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		zResult.position = base.transform.position;
	}
}
