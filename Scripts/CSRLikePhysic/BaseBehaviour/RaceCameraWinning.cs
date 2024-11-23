using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/RaceCameraWinning")]
public class RaceCameraWinning : BaseBehaviour
{
	public Vector3 posWinning = new Vector3(4.2f, 1.55f, 4.9f);

	public Vector3 rotWinning = new Vector3(8.3f, 225f, 0f);

	public float fovWinning = 41f;

	private CameraState winningState = new CameraState();

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
		Vector3 inPosition = this.posWinning + this.human.position;
		this.winningState.SetPRF(inPosition, this.rotWinning, this.fovWinning);
	}

	public CameraState GetState()
	{
		return this.winningState;
	}
}
