using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Shots/BaseShot")]
public class BaseShot : MonoBehaviour
{
	public List<BaseBehaviour> Behaviours = new List<BaseBehaviour>();

	private CameraState cameraState = new CameraState();

	private void Awake()
	{
	}

	public void Activate()
	{
		this.cameraState.position = Vector3.zero;
		this.cameraState.rotation = Quaternion.identity;
		this.cameraState.fov = 60f;
		foreach (BaseBehaviour current in this.Behaviours)
		{
			current.OnActivate();
		}
	}

	public float TimeToEnd()
	{
		float num = float.PositiveInfinity;
		int count = this.Behaviours.Count;
		for (int i = 0; i < count; i++)
		{
			num = Mathf.Min(num, this.Behaviours[i].TimeToEnd(this.cameraState));
		}
		return num;
	}

	public bool OnUpdate(out CameraState zResultCameraState)
	{
		int count = this.Behaviours.Count;
		for (int i = 0; i < count; i++)
		{
			this.Behaviours[i].OnUpdate(ref this.cameraState);
		}
		zResultCameraState = this.cameraState;
		return this.TimeToEnd() > 0f;
	}
}
