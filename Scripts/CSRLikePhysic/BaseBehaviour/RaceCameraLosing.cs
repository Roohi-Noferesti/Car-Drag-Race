using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/RaceCameraLosing")]
public class RaceCameraLosing : BaseBehaviour
{
	public Vector3 posLosing = new Vector3(4.2f, 1.55f, -2.6f);

	public Vector3 rotLosing = new Vector3(8.3f, 306f, 0f);

	public float fovLosing = 38f;

	private CameraState losingState = new CameraState();

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
		Vector3 inPosition = this.posLosing + this.human.position;
		this.losingState.SetPRF(inPosition, this.rotLosing, this.fovLosing);
	}

	public CameraState GetState()
	{
		return this.losingState;
	}
}
