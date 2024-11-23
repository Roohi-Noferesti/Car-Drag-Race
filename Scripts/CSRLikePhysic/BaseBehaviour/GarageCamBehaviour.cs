using UnityEngine;

public class GarageCamBehaviour : BaseBehaviour
{
	public float maxFieldOfView = 55f;

	public GameObject carPlacement;

	public GameObject lookAt;

	public AnimationCurve dampCurve;

	public AnimationCurve limitDampCurve;

	public float dampDuration;

	public float fingerSensitivity = 180f;

	public bool IsSeasonPrizeCamera;

	private float dampStartTime;

	private Vector2 dampStartCamSpeed;

	private Vector2 currentCamSpeed;

	public float speedLimit = 380f;

	private Vector3 prevMousePosition;

	public override void OnActivate()
	{
		if (this.IsSeasonPrizeCamera)
		{
			this.dampStartCamSpeed = new Vector2(0f, 0f);
		}
		else
		{
			this.dampStartCamSpeed = new Vector2(-12f, 0f);
		}
		this.dampStartTime = Time.time;
	}

	public static Vector2 FixTouchDelta(Touch aT)
	{
		float num = Time.deltaTime / aT.deltaTime;
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			num = 1f;
		}
		return aT.deltaPosition * num;
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		float time = (Time.time - this.dampStartTime) / this.dampDuration;
		float d = this.dampCurve.Evaluate(time);
		Vector2 vector = Vector2.zero;
	    if (true)//!PopUpManager.Instance.isShowingPopUp)
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch aT = touches[i];
				Vector2 point = new Vector2(aT.position.x / (float)BaseDevice.ActiveDevice.GetScreenWidth(), aT.position.y / (float)BaseDevice.ActiveDevice.GetScreenHeight());
				if (Camera.main.rect.Contains(point))
				{
					if (aT.deltaTime <= 0f)
					{
						vector.Set(0f, 0f);
					}
					else
					{
						Vector2 vector2 = FixTouchDelta(aT);
						Vector2 a = new Vector2(vector2.x / (float)BaseDevice.ActiveDevice.GetScreenWidth(), vector2.y / (float)BaseDevice.ActiveDevice.GetScreenHeight());
						vector += a * this.fingerSensitivity;
					}
				}
			}

			vector.y *= -1f;
		}
		if (vector.magnitude != 0f)
		{
			this.dampStartTime = Time.time;
			this.dampStartCamSpeed = this.currentCamSpeed + vector;
		}
		this.currentCamSpeed = this.dampStartCamSpeed * d;
		bool flag = this.currentCamSpeed.y > 0f;
		if (flag)
		{
			float time2 = (base.transform.position.y - 2.8f) / 0.4000001f;
			float num = this.limitDampCurve.Evaluate(time2);
			this.currentCamSpeed.y = this.currentCamSpeed.y * num;
		}
		else
		{
			float time3 = (base.transform.position.y - 1f) / -0.8f;
			float num2 = this.limitDampCurve.Evaluate(time3);
			this.currentCamSpeed.y = this.currentCamSpeed.y * num2;
		}
		if (Mathf.Abs(this.currentCamSpeed.x) > this.speedLimit)
		{
			this.currentCamSpeed.x = this.speedLimit * Mathf.Sign(this.currentCamSpeed.x);
		}
		if (Mathf.Abs(this.currentCamSpeed.y) > this.speedLimit)
		{
			this.currentCamSpeed.y = this.speedLimit * Mathf.Sign(this.currentCamSpeed.y);
		}
		Vector3 position = this.lookAt.transform.position;
		base.transform.RotateAround(position, Vector3.up, this.currentCamSpeed.x * Time.deltaTime);
		base.transform.RotateAround(position, base.transform.right, this.currentCamSpeed.y * Time.deltaTime);
		Vector3 vector3 = base.transform.position - position;
		vector3.Normalize();
		if (base.transform.position.y > 3.2f)
		{
			Vector3 position2 = base.transform.position;
			position2.y = 3.2f;
			Vector3 to = position2 - position;
			to.Normalize();
			float num3 = Vector3.Angle(vector3, to);
			base.transform.RotateAround(position, base.transform.right, -num3);
		}
		else if (base.transform.position.y < 0.2f)
		{
			Vector3 position3 = base.transform.position;
			position3.y = 0.2f;
			Vector3 to2 = position3 - position;
			to2.Normalize();
			float angle = Vector3.Angle(vector3, to2);
			base.transform.RotateAround(position, base.transform.right, angle);
		}
		if (!this.IsSeasonPrizeCamera)
		{
			vector3 = base.transform.position - position;
			vector3.Normalize();
			float d2 = Vector3.Dot(Vector3.forward, vector3);
			Vector3 b = Vector3.forward * d2 * 1.2f;
			zResult.position = base.transform.position + b;
			zResult.rotation = base.transform.rotation;
			float value = (base.transform.position.y - 0.2f) / 3f;
			zResult.fov = Mathf.Lerp(zResult.fov, this.maxFieldOfView, Mathf.Clamp01(value));
		}
		else
		{
			zResult.position = base.transform.position;
			zResult.rotation = base.transform.rotation;
		}
	}
}
