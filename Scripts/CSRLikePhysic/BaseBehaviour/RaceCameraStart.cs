using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/RaceCameraStart")]
public class RaceCameraStart : BaseBehaviour
{
	private Transform human;

	private CameraState startState = new CameraState();

	public Vector3 StartPosition = new Vector3(6f, 2.5f, 0.5f);

	public Vector3 TargetOffset = new Vector3(-2f, -0f, 1f);

	[SerializeField] float CamFov = 55;

	public override void OnActivate()
	{
		if (RaceCameraGo.BugOutForEditor())
		{
			return;
		}
		this.human = CompetitorManager.Instance.LocalCompetitor.Transform;
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		if (RaceCameraGo.BugOutForEditor())
		{
			return;
		}
		this.startState.position = this.StartPosition;
		this.startState.focalPoint = Vector3.zero;
		this.startState.fov = CamFov;
		Vector3 a = this.human.position;
		a += this.TargetOffset;
		a -= this.startState.position;
		this.startState.rotation = Quaternion.LookRotation(a.normalized);
	}

	public CameraState GetState()
	{
		return this.startState;
	}
}
