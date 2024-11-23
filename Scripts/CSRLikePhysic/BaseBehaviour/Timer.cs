using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/Timer")]
public class Timer : BaseBehaviour
{
	public float Duration = 3f;

	public float CurrentTime
	{
		get;
		private set;
	}

	public override void OnActivate()
	{
		this.CurrentTime = 0f;
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		this.CurrentTime += Time.deltaTime;
	}

	public override float TimeToEnd(CameraState zResult)
	{
		return this.Duration - this.CurrentTime;
	}
}
