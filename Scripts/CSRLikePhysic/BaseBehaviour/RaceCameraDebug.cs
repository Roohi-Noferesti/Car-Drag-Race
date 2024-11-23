using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/RaceCameraDebug")]
public class RaceCameraDebug : BaseBehaviour
{
	private GameObject humanCar;

	public Vector3 cameraOffset;

	public Vector3 cameraLookatOffset;

	public override void OnActivate()
	{
		if (RaceController.Instance == null)
		{
			return;
		}
		this.humanCar = CompetitorManager.Instance.LocalCompetitor.GameObject;
	    GetComponent<Camera>().enabled = true;
	    m_time = Time.time;
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		if (this.humanCar == null)
		{
			return;
		}
		zResult.position = this.humanCar.transform.position + this.cameraOffset;
		Vector3 vector = this.humanCar.transform.position + this.cameraLookatOffset - zResult.position;
		zResult.rotation = Quaternion.LookRotation(vector.normalized);
	}

    private float m_time;
    public override float TimeToEnd(CameraState zResult)
    {
        return (3-(Time.time - m_time));
    }
}
