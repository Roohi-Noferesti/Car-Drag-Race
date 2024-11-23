using UnityEngine;

public class GarageCamDefaultPos : BaseBehaviour
{
	public float fieldOfView = 35f;

	public float distance = 3f;

	public GameObject carPlacement;

	public GameObject lookAt;

	public Transform defaultPosition;

	public override void OnActivate()
	{
		Camera main = Camera.main;
		if (main == null)
		{
			return;
		}
		Vector3 position = main.transform.position;
		Vector3 a = position - this.carPlacement.transform.position;
		a.Normalize();
		if (GarageCameraManager.Instance.seasonPrizeEndCameraPose == this.defaultPosition)
		{
			this.distance = Vector3.Distance(this.lookAt.transform.position, position);
		}
		base.transform.position = this.carPlacement.transform.position + a * this.distance;
		base.transform.LookAt(this.lookAt.transform);
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		zResult.position = base.transform.position;
		zResult.rotation = base.transform.rotation;
		zResult.fov = this.fieldOfView;
	}
}
