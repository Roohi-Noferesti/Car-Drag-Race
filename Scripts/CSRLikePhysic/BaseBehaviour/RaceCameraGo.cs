using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/RaceCameraGo")]
public class RaceCameraGo : BaseBehaviour
{
	public enum Condition
	{
		start,
		head_to_head,
		normal
	}

	private const float StartCamIntro = 1f;

	public AnimationCurve Winning;

	public AnimationCurve Midcurve;

	public AnimationCurve Losing;

	public AnimationCurve startBlendCurve;

	public AnimationCurve nitrousCurve;

	public AnimationCurve ShakeScaleCurve;

	private float timeToEnd;

	private float CurrentTime;

	private float RaceTime;

	private float nitrousTime;

	private float nitrousTimer;

	public Vector3 nitrousPush = new Vector3(0f, 0f, -0.3f);

	private float distanceBeforeEndToSwitchCameras;

	private float distanceBeforeEndToSwitchToEnd;

	private float StartCamTime;

	private float StartCamTransition;

	private Vector3 humanStart = Vector3.zero;

	private Condition currentCondition;

	private Transform localCarTransform;

	private Transform otherCarTransform;

	private CarPhysics localPhys;

	private CarPhysics otherCarPhys;

	private int prevGear;

	private float gearPunchTime;

	private float gearPunchTimer;

	private float gearPunchFovChange;

	private float fovRelaxTime;

	public float DriftDamp = 0.97f;

	public float DriftEffect = 0.00015f;

	public float DriftTimer = 1f;

	public float DriftTimerVar = 0.5f;

	public float DriftSpring = 0.001f;

    public float FinishLineDistance = 25;

	private float driftTime;

	private Vector3 driftAccel;

	private Vector3 driftVel;

	private Vector3 driftPos;

	private RaceCameraStart behStart;

	private RaceCameraLosing behLosing;

	private RaceCameraBehind behBehind;

	private RaceCameraInFront behInFront;

	private RaceCameraWinning behWinning;

	private RaceCameraH2H behH2H;

	public static bool BugOutForEditor()
	{
		return RaceController.Instance == null || CompetitorManager.Instance.LocalCompetitor == null;
	}

	public void Reset()
	{
		CurrentTime = 0f;
		RaceTime = 0f;
		currentCondition = Condition.start;
		timeToEnd = 1f;
		distanceBeforeEndToSwitchCameras = 80f;
		distanceBeforeEndToSwitchToEnd = FinishLineDistance;
		driftAccel = Vector3.zero;
		driftVel = Vector3.zero;
		driftPos = Vector3.zero;
		driftTime = 0f;
		prevGear = 0;
		gearPunchTime = 0f;
		gearPunchTimer = 0.6f;
		gearPunchFovChange = 0.9f;
		fovRelaxTime = 0f;
		nitrousTime = 0f;
		nitrousTimer = 0f;
	}

	public override void OnActivate()
	{
		Reset();
		if (BugOutForEditor())
		{
			return;
		}
		localCarTransform = CompetitorManager.Instance.LocalCompetitor.Transform;
		otherCarTransform = ((CompetitorManager.Instance.OtherCompetitor == null) ? null : CompetitorManager.Instance.OtherCompetitor.Transform);
		localPhys = CompetitorManager.Instance.LocalCompetitor.CarPhysics;
		otherCarPhys = ((CompetitorManager.Instance.OtherCompetitor == null) ? null : CompetitorManager.Instance.OtherCompetitor.CarPhysics);
		behStart = GetComponent<RaceCameraStart>();
		behLosing = GetComponent<RaceCameraLosing>();
		behBehind = GetComponent<RaceCameraBehind>();
		behInFront = GetComponent<RaceCameraInFront>();
		behWinning = GetComponent<RaceCameraWinning>();
		behH2H = GetComponent<RaceCameraH2H>();
		switch (CompetitorManager.Instance.LocalCompetitor.CarPhysics.CarTier)
		{
		case eCarTier.TIER_1:
			StartCamTime = 2.5f;
			break;
		case eCarTier.TIER_2:
			StartCamTime = 2.3f;
			break;
		case eCarTier.TIER_3:
			StartCamTime = 2.1f;
			break;
		case eCarTier.TIER_4:
			StartCamTime = 1.9f;
			break;
		case eCarTier.TIER_5:
			StartCamTime = 1.7f;
			break;
		default:
			StartCamTime = 2.5f;
			break;
		}
		StartCamTransition = StartCamTime - 1f;
	    humanStart = RaceEnvironmentSettings.Instance.HumanStartPosition.position;
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		if (PauseGame.isGamePaused)
		{
			return;
		}
		if (BugOutForEditor())
		{
			return;
		}
		CurrentTime += Time.deltaTime;
		if (RaceController.Instance.Machine.StateIs(RaceStateEnum.race))
		{
			RaceTime += Time.deltaTime;
		}
		DecideCondition();
        //Debug.Log(currentCondition);
	    switch (currentCondition)
	    {
	        case Condition.start:
	            DoStartCam(ref zResult);
	            break;
	        case Condition.head_to_head:
	            DoH2HCam(ref zResult);
                break;
	        case Condition.normal:
	            DoNormalCam(ref zResult);
                break;
	    }
        DoNitrous(ref zResult);
        DoShake(ref zResult);
        DoDrift(ref zResult);
        DoGears(ref zResult);
	}

	private void DecideCondition()
	{
		if (RaceTime < StartCamTime)
		{
			currentCondition = Condition.start;
			return;
		}
		float num = localCarTransform.position.z - humanStart.z;
		if (num > RaceEventInfo.Instance.RaceDistanceMetres - distanceBeforeEndToSwitchToEnd)
		{
			timeToEnd = 0f;
			return;
		}
		if (otherCarTransform == null)
		{
			return;
		}
		if (currentCondition == Condition.head_to_head)
		{
			return;
		}
		if (num > RaceEventInfo.Instance.RaceDistanceMetres - distanceBeforeEndToSwitchCameras)
		{
			float num2 = localCarTransform.position.z - otherCarTransform.position.z;
			float num3 = Mathf.Abs(num2);
			float num4 = localPhys.SpeedMS - otherCarPhys.SpeedMS;
			float num5 = Mathf.Abs(num4);
			float num6 = num2 * num4;
			if (num3 < 3f && num5 < 2f && num6 < 0f)
			{
				currentCondition = Condition.head_to_head;
				return;
			}
		}
		currentCondition = Condition.normal;
	}

	private void DoStartCam(ref CameraState zResult)
	{
		zResult = behStart.GetState();
        bool flag = RaceTime > 1f;
        if (flag)
        {
            CameraState dest = new CameraState();
            DoNormalCam(ref dest);
            float num = RaceTime - 1f;
            num /= StartCamTransition;
            num = startBlendCurve.Evaluate(num);
            CameraState cameraState = CameraState.Lerp(zResult, dest, num);
            zResult = cameraState;
        }
	}

	private void DoH2HCam(ref CameraState zResult)
	{
		zResult = behH2H.GetState();
	}

	private void DoShake(ref CameraState zResult)
	{
		float speedMS = localPhys.SpeedMS;
		float num = 15f;
		float num2 = 30f;
		float num3 = Mathf.Clamp01((speedMS - num) / num2);
		float num4 = 0.015f;
		if (currentCondition == Condition.head_to_head)
		{
			num4 = 0.03f;
		}
		if (ShakeScaleCurve != null)
		{
			float z = localCarTransform.position.z;
			float time = z / 100f;
			num4 *= 1f + ShakeScaleCurve.Evaluate(time);
		}
		Vector3 vector = new Vector3(Random.value, Random.value, Random.value);
		vector *= num3 * num4;
		zResult.position += vector;
	}

	private void DoNitrous(ref CameraState zResult)
	{
		if (localPhys.IsUsingNitrous && localPhys.Engine.NitrousTimeLeft > nitrousTimer)
		{
			nitrousTime = localPhys.Engine.NitrousTimeLeft;
			nitrousTimer = nitrousTime;
		}
		if (nitrousTime > 0f)
		{
			nitrousTime -= Time.deltaTime;
			float num = (nitrousTimer - nitrousTime) / nitrousTimer;
			num = nitrousCurve.Evaluate(num);
			Vector3 b = nitrousPush * num;
			zResult.position += b;
			float z = Random.Range(-0.5f, 0.5f);
			Quaternion rhs = Quaternion.Euler(0f, 0f, z);
			zResult.rotation *= rhs;
		}
	}

	private void DoGears(ref CameraState zResult)
	{
		if (localPhys.GearBox.CurrentGear != prevGear)
		{
			prevGear = localPhys.GearBox.CurrentGear;
			if (localPhys.SpeedMS > 2f)
			{
				gearPunchTime = gearPunchTimer;
			}
		}
		if (gearPunchTime > 0f)
		{
			gearPunchTime -= Time.deltaTime;
			float num = (gearPunchTimer - gearPunchTime) / gearPunchTimer;
			num = nitrousCurve.Evaluate(num);
			float num2 = zResult.fov;
			num2 = Mathf.Lerp(num2, num2 * gearPunchFovChange, num);
			zResult.fov = num2;
		}
	}

	private void DoNormalCam(ref CameraState zResult)
	{
		float num;
		if (otherCarTransform == null)
		{
			num = 5f;
		}
		else
		{
			num = localCarTransform.position.z - otherCarTransform.position.z;
		}
		CameraState source = new CameraState();
		CameraState dest = new CameraState();
		float num2;

		if (num > 3f)
		{
			source = behInFront.GetState();
			dest = behWinning.GetState();
			num2 = Mathf.Clamp01((num - 3f) / 10f);
			num2 = Winning.Evaluate(num2);
			if (num2 > 0.99f)
			{
				fovRelaxTime += Time.deltaTime;
			}
			else
			{
				fovRelaxTime -= Time.deltaTime * 5f;
				fovRelaxTime = Mathf.Max(0f, fovRelaxTime);
			}
		}
		else if (num > -5f)
		{
			source = behBehind.GetState();
		    dest = behInFront.GetState();
            num2 = Mathf.Clamp01((num + 5f) / 8f);
            num2 = Midcurve.Evaluate(num2);
			fovRelaxTime = 0f;
		}
		else
		{
			source = behLosing.GetState();
			dest = behBehind.GetState();
			num2 = Mathf.Clamp01((num + 35f) / 30f);
			num2 = Losing.Evaluate(num2);
			if (num2 < 0.01f)
			{
				fovRelaxTime += Time.deltaTime;
			}
			else
			{
				fovRelaxTime -= Time.deltaTime * 5f;
				fovRelaxTime = Mathf.Max(0f, fovRelaxTime);
			}
		}
		zResult = CameraState.Lerp(source, dest, num2);
		if (fovRelaxTime > 1f)
		{
			num2 = Mathf.Clamp01((fovRelaxTime - 1f) / 5f);
			num2 = Losing.Evaluate(num2);
			zResult.fov = Mathf.Lerp(zResult.fov, zResult.fov * 1.4f, num2);
		}
	}

	private void DoDrift(ref CameraState zResult)
	{
		driftTime -= Time.deltaTime;
		float num = DriftEffect;
		if (currentCondition == Condition.start)
		{
			num *= 0.35f;
		}
		if (driftTime < 0f)
		{
			driftTime = Random.Range(DriftTimer - DriftTimerVar, DriftTimer + DriftTimerVar);
			float x = Random.Range(-num, num);
			float y = Random.Range(-num, num);
			float z = Random.Range(-num, num);
			driftAccel = new Vector3(x, y, z);
		}
		driftVel += driftAccel;
		Vector3 b = driftPos * -DriftSpring;
		driftVel += b;
		driftVel *= DriftDamp;
		driftPos += driftVel;
		zResult.position += driftPos;
	}

	public override float TimeToEnd(CameraState zResult)
	{
		return timeToEnd;
	}
}
