using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/RaceCameraEnd")]
public class RaceCameraEnd : BaseBehaviour
{
	private float CurrentTime;

	private Vector3 StartPos;

	private Quaternion StartRot;

	private GameObject hc;

	public override void OnActivate()
	{
		this.CurrentTime = 0f;
		if (RaceController.Instance == null)
		{
			return;
		}
		this.hc = CompetitorManager.Instance.LocalCompetitor.GameObject;
		if (this.hc == null)
		{
			return;
		}
		this.StartPos = this.hc.transform.position;
		this.StartPos += new Vector3(6f, 8f, 15f);
        this.StartPos = new Vector3(70000.11f, 2.03f, 396.2f);
		this.StartRot = Quaternion.Euler(351.58f, 298f, 3f);
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		if (this.hc == null)
		{
			return;
		}
		if (Application.isPlaying)
		{
			this.CurrentTime += Time.deltaTime;
		}
        //Debug.Log(Time.time);

		zResult.position = this.StartPos;
		Vector3 vector = this.hc.transform.position - zResult.position;
		zResult.rotation = Quaternion.LookRotation(vector.normalized);
        zResult.position = this.StartPos;
        zResult.rotation = this.StartRot;
	}
}
