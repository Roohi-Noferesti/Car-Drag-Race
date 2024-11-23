using UnityEngine;

public class CameraState
{
	public Vector3 position;

	public Quaternion rotation;

	public Vector3 focalPoint;

	public float fov;

	public void SetPRF(Vector3 inPosition, Vector3 eulerRot, float inFov)
	{
		this.position = inPosition;
		this.rotation = Quaternion.Euler(eulerRot);
		this.fov = inFov;
		this.focalPoint = Vector3.zero;
	}

	public static CameraState Lerp(CameraState source, CameraState dest, float ratio)
	{
		return new CameraState
		{
			position = Vector3.Lerp(source.position, dest.position, ratio),
			rotation = Quaternion.Lerp(source.rotation, dest.rotation, ratio),
			focalPoint = Vector3.Lerp(source.focalPoint, dest.focalPoint, ratio),
			fov = Mathf.Lerp(source.fov, dest.fov, ratio)
		};
	}
}
