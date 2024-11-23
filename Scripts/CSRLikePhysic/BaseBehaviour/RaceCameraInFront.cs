using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/RaceCameraInFront")]
public class RaceCameraInFront : BaseBehaviour
{
	public Vector3 posInFront = new Vector3(4.5f, 2.72f, 2.5f);

	public Vector3 rotInFront = new Vector3(18.8f, 254.3f, 0f);

	public float fovInFront = 43f;

	private CameraState inFrontState = new CameraState();

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
		Vector3 inPosition = this.posInFront + this.human.position;
		this.inFrontState.SetPRF(inPosition, this.rotInFront, this.fovInFront);
	}

	public CameraState GetState()
	{
		return this.inFrontState;
	}
}
