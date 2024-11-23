using System;
using UnityEngine;

public class ShowroomCamFront : MonoBehaviour
{
	public float incomingRotationSpeed = 10f;

	public float minimumRotationSpeed = 0.8f;

	public AnimationCurve dampCurve;

	public float dampDurationCar;

	public float dampDurationCamera;

	public float speedLimit = 500f;

	public float cameraSpeedMultiplier = 0.1f;

	public float fingerSensitivity = 0.5f;

	private GameObject theCar;

	private Material turntableMaterial;

	private Vector3 prevMousePosition;

	private float dampStartTimeCar;

	private float dampStartCarSpeed;

	private float currentCarSpeed;

	private float dampStartTimeCamera;

	private Vector2 dampStartCamSpeed;

	private Vector2 currentCamSpeed;

	public GameObject pivot
	{
		get;
		set;
	}

	public Camera frontPose
	{
		get;
		set;
	}

	public Camera backPose
	{
		get;
		set;
	}

	private void Start()
	{
		this.turntableMaterial = GameObject.Find("Turntable/InnerCircle").GetComponent<Renderer>().material;
	}

	private void OnEnable()
	{
		this.theCar = AsyncSwitching.Instance.CurrentObject(AsyncBundleSlotDescription.AICar);
		this.dampStartTimeCar = Time.time;
		this.dampStartCarSpeed = 0f;
		this.currentCarSpeed = 0f;
		this.dampStartTimeCamera = Time.time;
		this.currentCamSpeed.x = this.incomingRotationSpeed;
		this.currentCamSpeed.y = 0f;
		this.dampStartCamSpeed = this.currentCamSpeed;
	}

	private void Update()
	{
		float time = (Time.time - this.dampStartTimeCar) / this.dampDurationCar;
		float num = this.dampCurve.Evaluate(time);
		float time2 = (Time.time - this.dampStartTimeCamera) / this.dampDurationCamera;
		float d = this.dampCurve.Evaluate(time2);
		Vector3 lhs = base.transform.position - this.pivot.transform.position;
		lhs.y = 0f;
		lhs.Normalize();
		float num2 = Vector3.Dot(lhs, Vector3.right);
		Vector3 lhs2 = base.transform.position - this.pivot.transform.position;
		lhs2.x = 0f;
		lhs2.Normalize();
		float num3 = Vector3.Dot(lhs2, Vector3.up);
		Vector2 vector = Vector2.zero;
		if (!PopUpManager.Instance.isShowingPopUp)
		{
			if (Input.touches.Length == 1)
			{
				Touch touch = Input.touches[0];
				Vector2 point = new Vector2(touch.position.x / (float)BaseDevice.ActiveDevice.GetScreenWidth(), touch.position.y / (float)BaseDevice.ActiveDevice.GetScreenHeight());
				if (Camera.main.rect.Contains(point))
				{
					vector += touch.deltaPosition * this.fingerSensitivity;
				}
			}
			vector.y *= -1f;
		}
		if (vector.magnitude != 0f)
		{
			this.dampStartTimeCar = Time.time;
			this.dampStartCarSpeed = this.currentCarSpeed + vector.x;
			this.dampStartTimeCamera = Time.time;
			this.dampStartCamSpeed = this.currentCamSpeed + vector;
		}
		this.currentCarSpeed = this.dampStartCarSpeed * num;
		if (Mathf.Abs(this.currentCarSpeed) > this.speedLimit)
		{
			this.currentCarSpeed = this.speedLimit * Mathf.Sign(this.currentCarSpeed);
		}
		if (Mathf.Abs(this.currentCarSpeed) < 0.01f)
		{
			this.currentCarSpeed = 0f;
		}
		else
		{
			this.theCar.transform.Rotate(Vector3.up, -this.currentCarSpeed * Time.deltaTime);
			this.turntableMaterial.SetFloat("_Rotation", this.theCar.transform.localEulerAngles.y * 0.0174532924f);
		}
		this.currentCamSpeed = this.dampStartCamSpeed * d;
		if (this.currentCamSpeed.x > 0f && num2 > 0.3f)
		{
			this.dampStartCamSpeed.x = this.dampStartCamSpeed.x * 0.7f;
		}
		else if (this.currentCamSpeed.x < 0f && num2 < -0.3f)
		{
			this.dampStartCamSpeed.x = this.dampStartCamSpeed.x * 0.7f;
		}
		if (this.currentCamSpeed.x > 0f && num2 > 0.4f)
		{
			this.dampStartCamSpeed.x = 0f;
			this.currentCamSpeed.x = 0f;
		}
		else if (this.currentCamSpeed.x < 0f && num2 < -0.4f)
		{
			this.dampStartCamSpeed.x = 0f;
			this.currentCamSpeed.x = 0f;
		}
		if (this.currentCamSpeed.y > 0f && num3 > 0.33f)
		{
			this.dampStartCamSpeed.y = 0f;
			this.currentCamSpeed.y = 0f;
		}
		else if (this.currentCamSpeed.y < 0f && num3 < -0.05f)
		{
			this.dampStartCamSpeed.y = 0f;
			this.currentCamSpeed.y = 0f;
		}
		base.transform.RotateAround(this.pivot.transform.position, Vector3.up, this.currentCamSpeed.x * this.cameraSpeedMultiplier * Time.deltaTime);
		base.transform.RotateAround(this.pivot.transform.position, base.transform.right, this.currentCamSpeed.y * this.cameraSpeedMultiplier * Time.deltaTime);
	}
}
