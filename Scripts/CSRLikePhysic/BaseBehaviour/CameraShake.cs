using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/CameraShake")]
public class CameraShake : BaseBehaviour
{
	public float TimeBetweenPushes = 0.4f;

	public float PushForce = 0.2f;

	public float Drag = 0.97f;

	public float HookeConstant = -1f;

	private Vector3 position;

	private Vector3 velocity;

	private float timeSinceLastPush;

	public override void OnActivate()
	{
		this.position = Vector3.zero;
		this.velocity = Vector3.zero;
		this.timeSinceLastPush = 0f;
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		if (Application.isPlaying)
		{
			this.timeSinceLastPush += Time.deltaTime;
			if (this.timeSinceLastPush > this.TimeBetweenPushes)
			{
				this.timeSinceLastPush -= this.TimeBetweenPushes;
				this.Push();
			}
		}
		this.velocity += this.HookeConstant * this.position * Time.deltaTime;
		this.position += this.velocity * Time.deltaTime;
		Vector3 vector = zResult.rotation * Vector3.right * this.position.x;
		vector += zResult.rotation * Vector3.up * this.position.y;
		zResult.position += vector;
		this.velocity *= this.Drag;
	}

	private void Push()
	{
		Vector3 a = new Vector3(Random.value - 0.5f, Random.value - 0.5f, 0f);
		a.Normalize();
		this.velocity += a * this.PushForce;
	}
}
