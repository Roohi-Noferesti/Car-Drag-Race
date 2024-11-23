using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/RaceCameraH2H")]
public class RaceCameraH2H : BaseBehaviour
{
	private CameraState h2hState = new CameraState();

	private Transform human;

	public Vector3 Position = new Vector3(5.83f, 2.75f, 3.3f);

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
		this.h2hState.position = this.human.position;
		this.h2hState.position += this.Position;
		this.h2hState.rotation = Quaternion.Euler(16f, 260.86f, 3f);
		this.h2hState.fov = 24f;
	}

	public CameraState GetState()
	{
		return this.h2hState;
	}
}
