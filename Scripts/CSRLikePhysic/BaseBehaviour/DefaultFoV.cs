using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/DefaultFoV")]
public class DefaultFoV : BaseBehaviour
{
	public float FieldOfView = 60f;

	public override void OnActivate()
	{
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		zResult.fov = this.FieldOfView;
	}
}
