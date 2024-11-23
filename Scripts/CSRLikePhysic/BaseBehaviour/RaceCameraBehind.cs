using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/RaceCameraBehind")]
public class RaceCameraBehind : BaseBehaviour
{
	public Vector3 posBehind = new Vector3(4.5f, 1.6f, -1f);

	public Vector3 rotBehind = new Vector3(10f, 300f, 2f);

	public float fovBehind = 45f;

	private CameraState behindState = new CameraState();

	private Transform human;

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
		Vector3 inPosition = this.posBehind + this.human.position;
		this.behindState.SetPRF(inPosition, this.rotBehind, this.fovBehind);
	}

	public CameraState GetState()
	{
		return this.behindState;
	}
}
