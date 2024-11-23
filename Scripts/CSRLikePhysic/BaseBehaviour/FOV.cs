using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/Animated FOV")]
public class FOV : BaseBehaviour
{
	public AnimationCurve Interpolator;

	public float InitialFieldOfView = 60f;

	private float CurrentTime;

	public override void OnActivate()
	{
		this.CurrentTime = 0f;
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		this.CurrentTime += Time.deltaTime;
		float num = this.Interpolator.Evaluate(this.CurrentTime);
		float fov = this.InitialFieldOfView + num;
		zResult.fov = fov;
	}
}
