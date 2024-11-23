using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/Look At Transform")]
public class LookAtTransform : BaseBehaviour
{
	public Transform Target;

	public override void OnActivate()
	{
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		Vector3 vector = this.Target.transform.position - zResult.position;
		zResult.rotation = Quaternion.LookRotation(vector.normalized);
	}
}
