using System;
using Fabric;
using UnityEngine;
using Component = Fabric.Component;
using Random = UnityEngine.Random;

[AddComponentMenu("GT/CameraSequencer/Behaviours/RaceCameraFinishLine")]
public class RaceCameraFinishLine : BaseBehaviour
{
	public AnimationCurve Blend;

	public AnimationCurve TimeBlend;

	public Vector3 QuarterMileFinishPos = new Vector3(12f, 2.6f, 404f);

	public Vector3 HalfMileFinishCamPos = new Vector3(12f, 2.6f, 806f);

	public Vector3 FinishCamAngles = new Vector3(-10.64f, 232f, -8.15f);

	public float fovFinishLine = 37f;

	public float fovFinishLineZoomed = 25f;

	private float originalFixedDeltaTime;

	public static bool closeRace;

	private bool signalledCloseRaceStart;

	private Transform human;

	private CarPhysics humanPhys;

	private Component allAudioForSlow;

	public bool BugOutForEditor()
	{
		return RaceController.Instance == null || CompetitorManager.Instance.LocalCompetitor == null;
	}

	public override void OnActivate()
	{

        Camera.main.GetComponent<AB_ImageBlur_Raw>().enabled = false;



		if (this.BugOutForEditor())
		{
			return;
		}
		this.human = CompetitorManager.Instance.LocalCompetitor.Transform;
		this.humanPhys = CompetitorManager.Instance.LocalCompetitor.CarPhysics;
		this.signalledCloseRaceStart = false;
		if (CompetitorManager.Instance.OtherCompetitor == null || RaceEventInfo.Instance.IsSMPEvent)
		{
			closeRace = false;
		}
		else
		{
			Transform transform = CompetitorManager.Instance.OtherCompetitor.Transform;
			CarPhysics carPhysics = CompetitorManager.Instance.OtherCompetitor.CarPhysics;
			closeRace = (Mathf.Abs(this.human.position.z - transform.position.z) < 3f);
			float f = this.human.position.z - transform.position.z;
			if (Mathf.Abs(f) < 3f)
			{
				closeRace = true;
			}
			else if (Mathf.Abs(f) < 20f)
			{
				bool isHalfMile = false;
				if (RaceEventInfo.Instance.CurrentEvent != null)
				{
					isHalfMile = RaceEventInfo.Instance.CurrentEvent.IsHalfMile;
				}
				float num = this.EstimateFinishTime(carPhysics, isHalfMile, false);
				float num2 = this.EstimateFinishTime(this.humanPhys, isHalfMile, true);
				if (Mathf.Abs(num - num2) < 0.08f)
				{
					closeRace = true;
				}
			}
		}
		this.originalFixedDeltaTime = Time.fixedDeltaTime;
        this.allAudioForSlow = FabricManager.Instance.GetComponentByName("Audio_SFXAudio");
        RaceCarAudio raceCarAudio = CompetitorManager.Instance.LocalCompetitor.RaceCarAudio;
        if (raceCarAudio != null)
        {
            RaceCarAudio.QuietForLine();
            string text = AudioManager.EventEngineNames[(int)raceCarAudio.AudioEngine] + "_v2";
            text = text.Replace("Engines/", AudioEvent.CarEffects_Passbys_Base);
            AudioManager.Instance.PlaySound(text, Camera.main.gameObject);
        }

        //Debug.LogError("here");
    }

	private float EstimateFinishTime(CarPhysics phys, bool isHalfMile, bool isHumanTime = false)
	{
		float num = CarPhysicsCalculations.ExtrapolateApproximateFinishTime(phys, isHalfMile);
		if (RaceEventInfo.Instance.CurrentEvent != null && (RelayManager.IsCurrentEventRelay() || RaceEventInfo.Instance.CurrentEvent.AutoHeadstart))
		{
			float timeDifference = RaceEventInfo.Instance.CurrentEvent.GetTimeDifference();
			if (timeDifference > 0f && !isHumanTime)
			{
				num += timeDifference;
			}
			else if (timeDifference < 0f && isHumanTime)
			{
				num -= timeDifference;
			}
		}
		return num;
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		zResult.position = this.QuarterMileFinishPos;
		if (!this.BugOutForEditor() && Math.Abs(RaceEventInfo.Instance.RaceDistanceMetres - 500) > 100)
		{
			zResult.position = this.HalfMileFinishCamPos;
		}
		zResult.fov = this.fovFinishLine;
		zResult.rotation = Quaternion.Euler(this.FinishCamAngles.x, this.FinishCamAngles.y, this.FinishCamAngles.z);
		Vector3 a = this.human.position;
		a += new Vector3(0f, 1.2f, 1.4f);
		Vector3 vector = a - zResult.position;
		zResult.rotation = Quaternion.LookRotation(vector.normalized);
		Vector3 eulerAngles = zResult.rotation.eulerAngles;
		float num = eulerAngles.y;
		float num2 = 190f;
		float num3 = 225f;
		if (num > 270f)
		{
			num2 = 340f;
			num3 = 310f;
		}
		float num4 = num;
		float num5 = Mathf.Clamp01((num4 - num2) / (270f - num2));
		float num6 = num5;
		num = num3 + num6 * (270f - num3);
		eulerAngles.y = num;
		zResult.rotation = Quaternion.Euler(eulerAngles);
		if (closeRace)
		{
			float num7 = this.TimeBlend.Evaluate(num5);
			Time.timeScale = 1f - num7 * 0.99f;
			if (!this.signalledCloseRaceStart)
			{
				RaceController.Instance.Events.FireEvent("RaceEnteredCloseRaceSlowMo");
				this.signalledCloseRaceStart = true;
			}
			Time.fixedDeltaTime = this.originalFixedDeltaTime * Time.timeScale;
            this.allAudioForSlow.MixerPitch = 1f - num7 * 0.8f;
            this.allAudioForSlow.MixerVolume = 1f - num7 * 0.5f;
			zResult.fov = this.fovFinishLine * (1f - num7) + this.fovFinishLineZoomed * num7;
		}
		if (!this.BugOutForEditor())
		{
		}
	}

	private void DoShake(ref CameraState zResult)
	{
		float speedMS = this.humanPhys.SpeedMS;
		float num = 15f;
		float num2 = 30f;
		float num3 = Mathf.Clamp01((speedMS - num) / num2);
		float num4 = Vector3.Distance(this.human.position, zResult.position);
		num4 = Mathf.Clamp01((20f - num4) / 20f);
		float num5 = 0.1f * num4;
		Vector3 vector = new Vector3(Random.value, Random.value, Random.value);
		vector *= num3 * num5;
		zResult.position += vector;
	}
}
