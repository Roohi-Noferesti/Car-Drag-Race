using System;
using UnityEngine;

public class CamTransitionToFront : MonoBehaviour
{
	public float transitionTime = 1.5f;

	public AnimationCurve easeCurve;

	private ShowroomCamFront frontCam;

	private float startTime;

	private Vector3 startPos;

	private Quaternion startRot;

	private float startFov;

	private float startDistance;

	private float startHeight;

	private float targetDistance;

	private float targetAngle;

	private float targetHeight;

	private float prevAngle;

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

	private void OnDestroy()
	{
		this.frontCam = null;
		this.pivot = null;
		this.frontPose = null;
		this.backPose = null;
	}

	private void Start()
	{
        //this.frontCam = ShowroomCameraManager.Instance.frontCam;
	}

	private void Update()
	{
		if (Time.time > this.startTime + this.transitionTime)
		{
			ShowroomCameraManager.Instance.EnableFront();
			return;
		}
		base.transform.localPosition = this.startPos;
		base.transform.localRotation = this.startRot;
		base.GetComponent<Camera>().fieldOfView = this.startFov;
		float num = (Time.time - this.startTime) / this.transitionTime;
		float num2 = this.easeCurve.Evaluate(num);
		float num3 = this.targetAngle * num2;
		base.transform.RotateAround(this.pivot.transform.position, Vector3.up, -num3);
		float fieldOfView = Mathf.Lerp(this.startFov, this.frontPose.fieldOfView, num2);
		base.GetComponent<Camera>().fieldOfView = fieldOfView;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = Mathf.Lerp(this.startHeight, this.targetHeight, num2);
		base.transform.localPosition = localPosition;
		base.transform.LookAt(this.pivot.transform);
		Vector3 a = this.pivot.transform.position - base.transform.position;
		a.Normalize();
		float d = Mathf.Lerp(this.startDistance, this.targetDistance, num2);
		base.transform.localPosition = this.pivot.transform.position - d * a;
		if (num > 0.7f)
		{
			float num4 = num3 - this.prevAngle;
			if (num4 <= this.frontCam.incomingRotationSpeed * Time.deltaTime)
			{
				ShowroomCameraManager.Instance.EnableFront();
			}
		}
		this.prevAngle = num3;
	}

	public void StartTransition()
	{
		AudioManager.Instance.PlaySound("ShowroomSwooshOut", null);
		base.enabled = true;
		this.startTime = Time.time;
		this.startPos = base.transform.position;
		this.startRot = base.transform.rotation;
		this.startFov = base.GetComponent<Camera>().fieldOfView;
		this.startDistance = Vector3.Distance(base.transform.position, this.pivot.transform.position);
		this.targetDistance = Vector3.Distance(this.frontPose.transform.position, this.pivot.transform.position);
		this.startHeight = base.transform.position.y;
		this.targetHeight = this.frontPose.transform.position.y;
		Vector3 vector = base.transform.position - this.pivot.transform.position;
		vector.y = 0f;
		vector.Normalize();
		Vector3 vector2 = this.frontPose.transform.position - this.pivot.transform.position;
		vector2.y = 0f;
		vector2.Normalize();
		Vector3 lhs = vector + vector2;
		lhs.Normalize();
		this.targetAngle = Vector3.Angle(vector, vector2);
		if (Vector3.Dot(lhs, Vector3.right) < 0f)
		{
			this.targetAngle = 360f - this.targetAngle;
		}
	}

	public float GetTransitionTime()
	{
		return Time.time - this.startTime;
	}
}
