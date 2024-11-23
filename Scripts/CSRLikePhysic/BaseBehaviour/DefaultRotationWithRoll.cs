using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/DefaultRotationWithRoll")]
public class DefaultRotationWithRoll : BaseBehaviour
{
	public float RollPerSecond = 10f;

	private float CurrentTime;

	public override void OnActivate()
	{
		this.CurrentTime = 0f;
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		zResult.rotation = base.transform.rotation;
		this.CurrentTime += Time.deltaTime;
		float angle = this.RollPerSecond * this.CurrentTime;
		zResult.rotation *= Quaternion.AngleAxis(angle, base.transform.right);
	}
}
