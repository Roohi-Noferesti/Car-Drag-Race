using UnityEngine;

public class GUICameraShake : MonoBehaviour
{
	public float TimeBetweenPushes = 0.1f;

	public float PushForce = 0.4f;

	public float Drag = 0.97f;

	public float HookeConstant = -1000f;

	private Vector3 position;

	//private Vector3 oldPosition;

	private Vector3 velocity;

	//private float timeSinceLastPush = 1f;

	private AnimationCurve curveControlingShake;

	private CameraClearFlags previousGUICameraClearFlags;

	private Color previousGUICameraClearColor;

	private static GUICameraShake ActiveShake;

	public float ShakingDuration
	{
		get;
		set;
	}

	public float ShakeTime
	{
		get;
		set;
	}

	private void Start()
	{
		if (ActiveShake != null)
		{
			DestroyImmediate(ActiveShake);
		}
        //this.oldPosition = GUICamera.Instance.gameObject.transform.position;
        //this.previousGUICameraClearFlags = GUICamera.Instance.camera.clearFlags;
        //this.previousGUICameraClearColor = GUICamera.Instance.camera.backgroundColor;
		ActiveShake = this;
	}

	private void Update()
	{
		if (this.IsShakeOver())
		{
			this.ShakeOver();
		}
		else
		{
            //GUICamera.Instance.camera.clearFlags = CameraClearFlags.Color;
            //GUICamera.Instance.camera.backgroundColor = Color.black;
            //GameObjectHelper.MakeLocalPositionPixelPerfect(GUICamera.Instance.gameObject.transform);
		}
	}

	private void FixedUpdate()
	{
		if (!this.IsShakeOver())
		{
			this.UpdatePhysics();
		}
	}

	private bool IsShakeOver()
	{
		float time = Time.time;
		float num = time - this.ShakeTime;
        //GUICamera.Instance.camera.clearFlags = this.previousGUICameraClearFlags;
        //GUICamera.Instance.camera.backgroundColor = this.previousGUICameraClearColor;
		return num >= this.ShakingDuration;
	}

	public void OnDestroy()
	{
        //GUICamera.Instance.gameObject.transform.position = this.oldPosition;
        //GUICamera.Instance.camera.clearFlags = this.previousGUICameraClearFlags;
        //GUICamera.Instance.camera.backgroundColor = this.previousGUICameraClearColor;
	}

	public void ShakeOver()
	{
		DestroyImmediate(this);
	}

	public void SetCurve(AnimationCurve aCurve)
	{
		this.curveControlingShake = aCurve;
		this.ShakingDuration = this.curveControlingShake[this.curveControlingShake.length - 1].time;
	}

	public void UpdatePhysics()
	{
	 //   GameObject gameObject = null;//GUICamera.Instance.gameObject;
		//this.timeSinceLastPush += Time.fixedDeltaTime;
		//if (this.timeSinceLastPush > this.TimeBetweenPushes)
		//{
		//	this.timeSinceLastPush -= this.TimeBetweenPushes;
		//	this.Push();
		//}
		//this.velocity += this.HookeConstant * this.position * Time.fixedDeltaTime;
		//this.position += this.velocity * Time.fixedDeltaTime;
		//float time = Time.time;
		//float time2 = time - this.ShakeTime;
		//float d = this.curveControlingShake.Evaluate(time2);
		//Vector3 a = gameObject.transform.rotation * Vector3.right * this.position.x;
		//a += gameObject.transform.rotation * Vector3.up * this.position.y;
		//gameObject.transform.position = this.oldPosition + a * d;
		//this.velocity *= this.Drag;
	}

	private void Push()
	{
		Vector3 a = new Vector3(Random.value - 0.5f, Random.value - 0.5f, 0f);
		a.Normalize();
		this.velocity += a * this.PushForce;
	}
}
