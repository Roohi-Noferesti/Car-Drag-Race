using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/DollyZoom")]
public class DollyZoom : BaseBehaviour
{
	public AnimationCurve Interpolator;

	public float ZoomAmount = 5f;

	private float CurrentTime;

	public override void OnActivate()
	{
		this.CurrentTime = 0f;
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		if (Application.isPlaying)
		{
			this.CurrentTime += Time.deltaTime;
		}
		float d = this.Interpolator.Evaluate(this.CurrentTime);
		Vector3 a = zResult.rotation * Vector3.forward;
		zResult.position += a * d * this.ZoomAmount;
	}
}
