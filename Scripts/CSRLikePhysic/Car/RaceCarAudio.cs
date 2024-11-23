using System.Collections.Generic;
using UnityEngine;

public class RaceCarAudio : MonoBehaviour
{
	private const string _parameterLoad = "load";

	private const string _parameterRPM = "rpm";

	public eAudioEngineType AudioEngine;

	public AnimationCurve RevsFrig;

	public AnimationCurve TransmissionCurve;

	public AnimationCurve TransmissionVolumeCurve;

	public AnimationCurve TransmissionWobbleCurve;

	public float TransmissionWobbleMultiplier = 1f;

	public AnimationCurve[] EngineWobbleCurves;

	public float EngineWobbleMultiplier = 1f;

	public float[] EngineWobbleDurationGearMultiplier = {
		1f,
		1.1f,
		1.2f,
		1.3f,
		1.4f,
		1.5f,
		1.6f,
		1.7f,
		1.8f,
		1.9f
	};

	public int RevsLimitFade = 6250;

	public int RevsLimitAbsolute = 6500;

	public AnimationCurve RevsLimitCurve;

	public AnimationCurve RevsRedLineCurve;

	public int RevsBoostIdleGear0;

	public int RevsBoostIdleGear1;

	public int RevsBoostIdleGear2;

	public int RevsBoostIdleGear3;

	public int RevsBoostIdleGear4;

	public int RevsBoostIdleGear5;

	public int RevsBoostIdleGear6;

	public bool EnableEngine = true;

	public bool EnableGears = true;

	public bool EnableLaunch = true;

	public bool EnableNitrous = true;

	public bool EnableTransmission = true;

	public bool EnableTurbo = true;

	public bool EnableTyres = true;

	private float redLineRPMTime;

	private float redLineRPMDelta;

	private float maxSpeed;

	private int wheelSpinLockOut;

	private float wheelSpinTarget;

	private float wheelSpin;

	private bool wheelSpinStartPlayed;

	private static Dictionary<eAudioEngineType, RaceCarAudio> engineControls_Human;

	private static Dictionary<eAudioEngineType, RaceCarAudio> engineControls_AI;

	private static List<RaceCarAudio> instances = new List<RaceCarAudio>();

	private string engineAudioEvent;

	private string engineElectricAudioEvent;

	private string surfaceAudioEvent;

	private CarUpgradeSetup _carUpgradeSetup;

	private bool muted;

	private int prevGear;

	private float prevClutch = 1f;

	private float timeSinceGearChange;

	private bool prevNitrous;

	private bool carIsHuman = true;

	private GameObject theCar;

	private CarPhysics physics;

	private AudioManager audioManager;

	private bool readyToRace;

	//private bool launchPlayed;

	private int engineWobbleCurveIndex;

	private float _rpmPrevious;

	private float _mphPrevious;

	public float DebugRPMOverride = -1f;

	public bool AdoptTheBodge;

	private bool isDSGGearBox;

	public static bool DebugMute = false;

	private static void FindEngineControls()
	{
		engineControls_Human = new Dictionary<eAudioEngineType, RaceCarAudio>();
        Transform enginesRootTransform_Human = AudioManager.Instance.EnginesRootTransform_Human;
		foreach (Transform transform in enginesRootTransform_Human)
		{
			RaceCarAudio component = transform.GetComponent<RaceCarAudio>();
			if (component != null)
			{
				engineControls_Human[component.AudioEngine] = component;
			}
		}
		engineControls_AI = new Dictionary<eAudioEngineType, RaceCarAudio>();
        Transform enginesRootTransform_AI = AudioManager.Instance.EnginesRootTransform_AI;
		foreach (Transform transform2 in enginesRootTransform_AI)
		{
			RaceCarAudio component2 = transform2.GetComponent<RaceCarAudio>();
			if (component2 != null)
			{
				engineControls_AI[component2.AudioEngine] = component2;
			}
		}
	}

	public static RaceCarAudio Find(string carName, eAudioEngineType audioEngine, bool isHuman)
	{
		if (engineControls_Human == null)
		{
			FindEngineControls();
		}
		if (isHuman)
		{
			if (engineControls_Human.ContainsKey(audioEngine))
			{
				return engineControls_Human[audioEngine];
			}
		}
		else if (engineControls_AI.ContainsKey(audioEngine))
		{
			return engineControls_AI[audioEngine];
		}
        eAudioEngineType audioEngine2 = AudioManager.Instance.DefaultEngine.AudioEngine;
		if (audioEngine == audioEngine2)
		{
			return null;
		}
		return Find(carName, audioEngine2, isHuman);
	}

	public static void StopAllSounds()
	{
		foreach (RaceCarAudio current in instances)
		{
			current.Stop();
		}
	}

	public void Initialise(bool isHuman,CarPhysics physics, CarUpgradeSetup carUpgradeSetup)
	{
        audioManager = AudioManager.Instance;
		carIsHuman = isHuman;
		theCar = transform.parent.gameObject;
	    this.physics = physics;//theCar.GetComponent<CarPhysics>();
		if (physics == null)
		{
			gameObject.SetActive(false);
		}
		_carUpgradeSetup = carUpgradeSetup;
		int qualityLevel = GetQualityLevel(_carUpgradeSetup);
		engineAudioEvent = AudioManager.GetEventNameEngine(AudioEngine, qualityLevel, isHuman);
		if (_carUpgradeSetup.CarDBKey == "McLaren_P1_2014")
		{
			engineElectricAudioEvent = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_ElectricWhine, isHuman);
		}
		//CarInfo car = CarDatabase.Instance.GetCar(_carUpgradeSetup.CarDBKey);
	    isDSGGearBox = false;//(car != null && car.DSGGearBox);
		if (isHuman && RaceEnvironmentSettings.Instance != null && !string.IsNullOrEmpty(RaceEnvironmentSettings.Instance.SurfaceAudioEventName))
		{
			surfaceAudioEvent = RaceEnvironmentSettings.Instance.SurfaceAudioEventName + ((!isHuman) ? AudioManager.EventNameSuffixAI : AudioManager.EventNameSuffixHuman);
		}
		instances.Add(this);
		float zCombinedGearRatio = physics.GearBox.GearRatioFinalDrive * physics.GearBox.GearRatio(physics.GearBox.NumGears);
		maxSpeed = Mathf.CeilToInt(CarPhysicsCalculations.CalculateTheoreticalTopSpeedForThisGear(physics.Engine.RedLineRPM, zCombinedGearRatio, physics.TireData.WheelRadius) * 2.236f);
	}

	private int GetQualityLevel(CarUpgradeSetup carUpgradeSetup)
	{
		int num = 0;
		int num2 = 0;
		foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
		{
			CarUpgradeStatus carUpgradeStatus = carUpgradeSetup.UpgradeStatus[current];
			num += carUpgradeStatus.levelFitted;
			num2 += 6;
		}
		return Mathf.FloorToInt(num / (float)num2 * 3f) + 1;
	}

	private void OnDestroy()
	{
		instances.Remove(this);
	}

	public static void PreRaceQuiet()
	{
		foreach (RaceCarAudio current in instances)
		{
			string eventNameUpgrade = AudioManager.GetEventNameUpgrade(eUpgradeType.NITROUS, current._carUpgradeSetup, current.carIsHuman);
			AudioManager.Instance.SetVolume(eventNameUpgrade, 0f, current.gameObject);
			string eventNameUpgrade2 = AudioManager.GetEventNameUpgrade(eUpgradeType.TRANSMISSION, current._carUpgradeSetup, current.carIsHuman);
			AudioManager.Instance.SetVolume(eventNameUpgrade2, 0f, current.gameObject);
			string eventNameUpgrade3 = AudioManager.GetEventNameUpgrade(eUpgradeType.TURBO, current._carUpgradeSetup, current.carIsHuman);
			if (eventNameUpgrade3 != string.Empty)
			{
				AudioManager.Instance.SetVolume(eventNameUpgrade3, 0f, current.gameObject);
			}
		}
	}

	public void Reset()
	{
		if (carIsHuman)
		{
			audioManager.PlaySound(engineAudioEvent, audioManager.HumanEngineVolume, gameObject);
			if (!string.IsNullOrEmpty(engineElectricAudioEvent))
			{
				audioManager.PlaySound(engineElectricAudioEvent, audioManager.HumanEngineVolume, gameObject);
				audioManager.SetParameter(engineElectricAudioEvent, "rpm", 0f, gameObject);
				audioManager.SetParameter(engineElectricAudioEvent, "load", 0f, gameObject);
			}
			string eventNameUpgrade = AudioManager.GetEventNameUpgrade(eUpgradeType.TRANSMISSION, _carUpgradeSetup, carIsHuman);
			audioManager.PlaySoundSilentLoop(eventNameUpgrade, gameObject);
			string eventNameUpgrade2 = AudioManager.GetEventNameUpgrade(eUpgradeType.TURBO, _carUpgradeSetup, carIsHuman);
			if (eventNameUpgrade2 != string.Empty)
			{
				audioManager.PlaySoundSilentLoop(eventNameUpgrade2, gameObject);
			}
		}
		else
		{
			audioManager.PlaySound(engineAudioEvent, audioManager.AIEngineVolume, gameObject);
			if (!string.IsNullOrEmpty(engineElectricAudioEvent))
			{
				audioManager.PlaySound(engineElectricAudioEvent, audioManager.AIEngineVolume, gameObject);
				audioManager.SetParameter(engineElectricAudioEvent, "rpm", 0f, gameObject);
				audioManager.SetParameter(engineElectricAudioEvent, "load", 0f, gameObject);
			}
		}
		string eventNameUpgrade3 = AudioManager.GetEventNameUpgrade(eUpgradeType.NITROUS, _carUpgradeSetup, carIsHuman);
		audioManager.PlaySoundSilentLoop(eventNameUpgrade3, gameObject);
		string eventNameWithHumanAISuffix = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_TyresLoop, carIsHuman);
		audioManager.PlaySoundSilentLoop(eventNameWithHumanAISuffix, gameObject);
		if (!string.IsNullOrEmpty(surfaceAudioEvent))
		{
            audioManager.PlaySound(surfaceAudioEvent, gameObject);
		}
		redLineRPMTime = 0f;
		redLineRPMDelta = 0f;
		wheelSpinLockOut = 0;
		wheelSpinTarget = 0f;
		wheelSpin = 0f;
		wheelSpinStartPlayed = false;
		readyToRace = true;
		//launchPlayed = false;
	}

	public void Stop()
	{
		readyToRace = false;
		audioManager.SetParameter(engineAudioEvent, "rpm", 0f, gameObject);
		audioManager.SetParameter(engineAudioEvent, "load", 0f, gameObject);
		audioManager.StopSound(engineAudioEvent, gameObject);
        //Debug.Log("stop here");
		if (!string.IsNullOrEmpty(engineElectricAudioEvent))
		{
			audioManager.SetParameter(engineElectricAudioEvent, "rpm", 0f, gameObject);
			audioManager.SetParameter(engineElectricAudioEvent, "load", 0f, gameObject);
			audioManager.StopSound(engineElectricAudioEvent, gameObject);
		}
		if (carIsHuman)
		{
			string eventNameUpgrade = AudioManager.GetEventNameUpgrade(eUpgradeType.TRANSMISSION, _carUpgradeSetup, carIsHuman);
			audioManager.StopSound(eventNameUpgrade, gameObject);
			string eventNameUpgrade2 = AudioManager.GetEventNameUpgrade(eUpgradeType.TURBO, _carUpgradeSetup, carIsHuman);
			if (eventNameUpgrade2 != string.Empty)
			{
				audioManager.StopSound(eventNameUpgrade2, gameObject);
			}
		}
		string eventNameWithHumanAISuffix = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_GearChangeEngage, carIsHuman);
		audioManager.StopSound(eventNameWithHumanAISuffix, gameObject);
		string eventNameWithHumanAISuffix2 = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_GearChangeRelease, carIsHuman);
		audioManager.StopSound(eventNameWithHumanAISuffix2, gameObject);
		string eventNameUpgrade3 = AudioManager.GetEventNameUpgrade(eUpgradeType.NITROUS, _carUpgradeSetup, carIsHuman);
		audioManager.StopSound(eventNameUpgrade3, gameObject);
		string eventNameWithHumanAISuffix3 = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_TyresLoop, carIsHuman);
		audioManager.StopSound(eventNameWithHumanAISuffix3, gameObject);
		if (!string.IsNullOrEmpty(surfaceAudioEvent))
		{
			audioManager.StopSound(surfaceAudioEvent, gameObject);
		}
	}

	private bool UpdateMute()
	{
		bool flag = (!carIsHuman) ? audioManager.AIEngineMute : audioManager.HumanEngineMute;
		if (flag)
		{
			if (!muted)
			{
				audioManager.SetVolume(engineAudioEvent, 0f, gameObject);
                Debug.Log("mute here");

                if (!string.IsNullOrEmpty(engineElectricAudioEvent))
				{
					audioManager.SetVolume(engineElectricAudioEvent, 0f, gameObject);
				}
				muted = true;
			}
		}
		else if (muted)
		{
			float volume = (!carIsHuman) ? audioManager.AIEngineVolume : audioManager.HumanEngineVolume;
			audioManager.SetVolume(engineAudioEvent, volume, gameObject);
			if (!string.IsNullOrEmpty(engineElectricAudioEvent))
			{
				audioManager.SetVolume(engineElectricAudioEvent, volume, gameObject);
			}
			muted = false;
		}
		return muted;
	}

	private void Update()
	{
	    if (Input.GetKeyDown(KeyCode.G))
	    {
            AudioManager.Instance.PlaySound(engineAudioEvent, 1, gameObject);
	    }
		if (!readyToRace || UpdateMute() || DebugMute)
		{
			return;
		}
		float carSpeedMPH = GetCarSpeedMPH();
		float carSpeedRatio = carSpeedMPH / maxSpeed;
		if (EnableLaunch)
		{
			UpdateLaunch();
		}
		if (EnableEngine)
		{
			UpdateEngine();
		}
		if (EnableTransmission)
		{
			UpdateTransmission(carSpeedRatio);
		}
		if (EnableTurbo || EnableGears)
		{
			UpdateTurbo(carSpeedRatio);
		}
		if (EnableGears)
		{
			UpdateGearChanges();
		}
		if (EnableTyres)
		{
			UpdateTyres();
		}
		if (EnableNitrous)
		{
			UpdateNitrous();
		}
		if (!string.IsNullOrEmpty(surfaceAudioEvent))
		{
            audioManager.SetParameter(surfaceAudioEvent, "Speed", carSpeedMPH, gameObject);
		}
	}

	public float GetCarSpeedMPH()
	{
        //if (RaceController.Instance != null && RaceController.Instance.Machine.StateIs(RaceStateEnum.enter))
        //{
        //    RaceCarVisuals component = theCar.GetComponent<RaceCarVisuals>();
        //    if (component != null)
        //    {
        //        return component.GetMotionTrackerMPH();
        //    }
        //}
		return physics.SpeedMPH;
	}

	private void UpdateLaunch()
	{
        //if (!launchPlayed)
        //{
        //    if (physics.Engine.HasEngagedClutchBeforeLaunch && physics.Engine.TimeEngagedClutchBeforeLaunch > 0f)
        //    {
        //        AudioManager.Instance.Instance.PlaySound(AudioEvent.HUD_LaunchBad, null);
        //        launchPlayed = true;
        //    }
        //    else if (physics.HasDisengagedClutch && physics.HasEngagedClutch)
        //    {
        //string eventNameWithHumanAISuffix = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_LaunchEngage, carIsHuman);
        //        AudioManager.Instance.Instance.PlaySound(eventNameWithHumanAISuffix, gameObject);
        //        if (carIsHuman && physics.IsClutchEnabled && physics.SpeedMilestoneTimer.CurrentTime() > 0f)
        //        {
        //            if (physics.SpeedMilestoneTimer.CurrentTime() < MiscRaceData.secondsAfterStartForPerfectLaunch)
        //            {
        //                AudioManager.Instance.Instance.PlaySound(AudioEvent.HUD_LaunchPerfect, null);
        //            }
        //            else if (physics.SpeedMilestoneTimer.CurrentTime() < MiscRaceData.secondsAfterStartForGoodLaunch)
        //            {
        //                AudioManager.Instance.Instance.PlaySound(AudioEvent.HUD_LaunchGood, null);
        //            }
        //            else
        //            {
        //                AudioManager.Instance.Instance.PlaySound(AudioEvent.HUD_LaunchBad, null);
        //            }
        //        }
        //        launchPlayed = true;
        //    }
        //}
    }

	private void UpdateGearChanges()
	{
		if (physics.GearBox.CurrentGear == prevGear)
		{
			if (prevClutch < 1f)
			{
				if (physics.GearBox.Clutch == 1f && carIsHuman)
				{
					string eventNameWithHumanAISuffix = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_GearChangeRelease, carIsHuman);
					audioManager.PlaySound(eventNameWithHumanAISuffix, gameObject);
					engineWobbleCurveIndex = Random.Range(0, EngineWobbleCurves.Length);
				}
				prevClutch = physics.GearBox.Clutch;
			}
			if (prevClutch >= 1f)
			{
				timeSinceGearChange += Time.deltaTime;
			}
		}
		else
		{
			if (carIsHuman)
			{
				string eventNameWithHumanAISuffix2 = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_GearChangeEngage, carIsHuman);
				audioManager.PlaySound(eventNameWithHumanAISuffix2, gameObject);
			}
			prevGear = physics.GearBox.CurrentGear;
			prevClutch = 0f;
			timeSinceGearChange = 0f;
		}
	}

	private void UpdateEngine()
	{
		float rPMParameter = GetRPMParameter();
		audioManager.SetParameter(engineAudioEvent, "rpm", rPMParameter, gameObject);
		float loadParameter = GetLoadParameter();
        audioManager.SetParameter(engineAudioEvent, "load", loadParameter, gameObject);
        if (!string.IsNullOrEmpty(engineElectricAudioEvent) && !physics.GearBox.IsInNeutral)
        {
            audioManager.SetParameter(engineElectricAudioEvent, "rpm", rPMParameter, gameObject);
            audioManager.SetParameter(engineElectricAudioEvent, "load", loadParameter, gameObject);
        }
        if (!isDSGGearBox)
        {
            float duration = (!physics.GearBox.IsInNeutral) ? EngineWobbleDurationGearMultiplier[physics.GearBox.CurrentGear] : 1f;
            float pitchCurveValue = EngineWobbleCurves[engineWobbleCurveIndex].Evaluate(timeSinceGearChange / duration);
            float pitch = pitchCurveValue * EngineWobbleMultiplier + physics.LongitudinalRollAmount * 0.15f;
            audioManager.SetPitch(engineAudioEvent, pitch, gameObject);
        }
	}

	private void UpdateTurbo(float carSpeedRatio)
	{
		if (carIsHuman)
		{
			float clutch = physics.GearBox.Clutch;
			float volume = carSpeedRatio * clutch * audioManager.MaxTurboVolume;
			string eventNameUpgrade = AudioManager.GetEventNameUpgrade(eUpgradeType.TURBO, _carUpgradeSetup, carIsHuman);
			if (eventNameUpgrade != string.Empty)
			{
				audioManager.SetVolume(eventNameUpgrade, volume, gameObject);
			}
			if (physics.GearBox.CurrentGear != prevGear && physics.Engine.CurrentRPM > 5000f && eventNameUpgrade != string.Empty)
			{
				if (prevGear == 0)
				{
					string eventNameWithHumanAISuffix = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_TurboBlowOffValve_Gear1, carIsHuman);
					audioManager.PlaySound(eventNameWithHumanAISuffix, gameObject);
				}
				else if (prevGear == 1)
				{
					string eventNameWithHumanAISuffix2 = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_TurboBlowOffValve_Gear2, carIsHuman);
					audioManager.PlaySound(eventNameWithHumanAISuffix2, gameObject);
				}
				else
				{
					string eventNameWithHumanAISuffix3 = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_TurboBlowOffValve_Gear3, carIsHuman);
					audioManager.PlaySound(eventNameWithHumanAISuffix3, gameObject);
				}
			}
		}
	}

	private float GetRPMParameter()
	{
		if (DebugRPMOverride >= 0f)
		{
			return DebugRPMOverride;
		}
		if (RaceController.Instance != null && RaceController.Instance.Machine.StateIs(RaceStateEnum.enter))
		{
			RaceCarVisuals component = theCar.GetComponent<RaceCarVisuals>();
			if (component != null)
			{
                //float motionTrackerMPH = component.GetMotionTrackerMPH();
                //if (motionTrackerMPH > 0f && motionTrackerMPH >= _mphPrevious)
                //{
                //    return physics.Engine.CurrentRPM;
                //}
				return physics.Engine.CurrentRPM;
			}
		}
		float num = physics.Engine.CurrentRPM;
		if (AdoptTheBodge)
		{
			int currentGear = physics.GearBox.CurrentGear;
			if (currentGear == 0)
			{
				num += Mathf.Lerp(RevsBoostIdleGear0, 0f, num / 10000f);
			}
			else if (currentGear == 1)
			{
				num += Mathf.Lerp(RevsBoostIdleGear1, 0f, num / 10000f);
			}
			else if (currentGear == 2)
			{
				num += Mathf.Lerp(RevsBoostIdleGear2, 0f, num / 10000f);
			}
		}
        if (num > RevsLimitFade)
        {
            float aboveFade = num - RevsLimitFade;
            float redlineFade = physics.Engine.RedLineRPM + RevsBoostIdleGear2 - RevsLimitFade;
            float num4 = Mathf.Clamp01(aboveFade / redlineFade);
            float num5 = RevsLimitAbsolute - 1 - RevsLimitFade;
            float num6 = num4 * num5;
            num = RevsLimitFade + num6;
            if (physics.Engine.CurrentRPM == physics.Engine.RedLineRPM)
            {
                if (num < RevsLimitAbsolute - 1)
                {
                    redLineRPMTime += Time.deltaTime;
                    redLineRPMDelta += RevsRedLineCurve.Evaluate(redLineRPMTime);
                    num = Mathf.Clamp(num + redLineRPMDelta, 0f, RevsLimitAbsolute - 1);
                }
            }
            else if (physics.Engine.TargetRPM != physics.Engine.RedLineRPM)
            {
                redLineRPMDelta = 0f;
            }
        }
		return num;
	}

	private float GetLoadParameter()
	{
		float currentRPM = physics.Engine.CurrentRPM;
		float num = currentRPM - _rpmPrevious;
		float result = 0f;
		if (num > 0f || physics.Engine.BustingTheEngine)
		{
			result = 1f;
		}
		_rpmPrevious = currentRPM;
		if (isDSGGearBox && RaceController.Instance != null && RaceController.Instance.Machine.StateIs(RaceStateEnum.race))
		{
			result = 1f;
		}
		return result;
	}

	private void UpdateTransmission(float carSpeedRatio)
	{
		if (carIsHuman)
		{
			float clutchVal = physics.GearBox.Clutch * 0.8f + 0.2f;
			if (physics.GearBox.IsInNeutral)
			{
				clutchVal = 0f;
			}
			float transVolum = clutchVal * audioManager.MaxTransmissionVolume;
			float volumeValue = TransmissionVolumeCurve.Evaluate(carSpeedRatio);
			transVolum *= volumeValue;
			float transPitch = TransmissionCurve.Evaluate(carSpeedRatio);
			if (!isDSGGearBox)
			{
				float num5 = TransmissionWobbleCurve.Evaluate(timeSinceGearChange);
				float num6 = num5 * TransmissionWobbleMultiplier;
				transPitch += 1f / num6;
			}
			string eventNameUpgrade = AudioManager.GetEventNameUpgrade(eUpgradeType.TRANSMISSION, _carUpgradeSetup, carIsHuman);
		    if (transPitch > 100)
		        transPitch = 1;
            audioManager.SetPitch(eventNameUpgrade, transPitch, gameObject);
			audioManager.SetVolume(eventNameUpgrade, transVolum, gameObject);
		}
	}

	private void UpdateTyres()
	{
		if (carIsHuman)
		{
			if (wheelSpinLockOut < 2)
			{
				wheelSpinTarget = physics.Wheels.GetNormalisedWheelSpin();
				if (wheelSpinLockOut == 0)
				{
					if (wheelSpinTarget > 0.1f)
					{
						wheelSpin = wheelSpinTarget;
						wheelSpinLockOut = 1;
					}
				}
				else if (wheelSpinLockOut == 1)
				{
					if (wheelSpinTarget < 0.1f)
					{
						wheelSpinTarget = 0f;
					}
					if (wheelSpinTarget > wheelSpin)
					{
						wheelSpin = wheelSpinTarget;
					}
					else
					{
						wheelSpin = Mathf.Lerp(wheelSpin, wheelSpinTarget, Time.deltaTime * 4f);
					}
					if (wheelSpinTarget < 0.1f && wheelSpin < 0.1f)
					{
						wheelSpin = 0f;
						wheelSpinLockOut = 2;
					}
				}
				string eventNameWithHumanAISuffix = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_TyresLoop, carIsHuman);
				audioManager.SetVolume(eventNameWithHumanAISuffix, wheelSpin, gameObject);
			}
			if (!wheelSpinStartPlayed && physics.GearBox.CurrentGear == 1)
			{
				if (!physics.HasHadWheelSpinStart)
				{
					string eventNameWithHumanAISuffix2 = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_TyresLaunch, carIsHuman);
					audioManager.PlaySound(eventNameWithHumanAISuffix2, gameObject);
				}
				wheelSpinStartPlayed = true;
			}
		}
	}

	private void UpdateNitrous()
	{
		if (physics.IsUsingNitrous)
		{
			if (carIsHuman && !prevNitrous)
			{
				string eventNameWithHumanAISuffix = AudioManager.GetEventNameWithHumanAISuffix(AudioEvent.CarEffects_NitrousEngage, carIsHuman);
				audioManager.PlaySound(eventNameWithHumanAISuffix, Camera.main.gameObject);
				prevNitrous = physics.IsUsingNitrous;
			}

            float num = Mathf.Min(physics.Engine.NitrousTimeLeft / 0.5f, 1f);
			string eventNameUpgrade = AudioManager.GetEventNameUpgrade(eUpgradeType.NITROUS, _carUpgradeSetup, carIsHuman);
			audioManager.SetVolume(eventNameUpgrade, num * audioManager.MaxNitrousVolume, gameObject);
		}
		else if (prevNitrous)
		{
			string eventNameUpgrade2 = AudioManager.GetEventNameUpgrade(eUpgradeType.NITROUS, _carUpgradeSetup, carIsHuman);
			audioManager.SetVolume(eventNameUpgrade2, 0f, gameObject);
			prevNitrous = physics.IsUsingNitrous;
		}
	}

	public static void FadeUpCarAudio(float fadeTime)
	{
		AudioManager.Instance.FadeInComponent("Audio_SFXAudio_Ambience", fadeTime, 0.5f);
		AudioManager.Instance.FadeInComponent("Audio_SFXAudio_Atmosphere", fadeTime, 0.5f);
		AudioManager.Instance.FadeInComponent("Audio_SFXAudio_CarEffects", fadeTime, 0.5f);
		AudioManager.Instance.FadeInComponent("Audio_SFXAudio_Engines", fadeTime, 0.5f);
		AudioManager.Instance.FadeInComponent("Audio_SFXAudio_Cars_Human", fadeTime, 0.5f);
		AudioManager.Instance.FadeInComponent("Audio_SFXAudio_Cars_AI", fadeTime, 0.5f);
	}

	public static void FadeDownCarAudio(float fadeTime)
	{
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_Ambience", fadeTime, 0.5f);
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_Atmosphere", fadeTime, 0.5f);
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_CarEffects", fadeTime, 0.5f);
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_Engines", fadeTime, 0.5f);
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_Cars_Human", fadeTime, 0.5f);
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_Cars_AI", fadeTime, 0.5f);
	}

	public static void QuietForLine()
	{
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_Ambience", 0.25f, 0.5f);
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_Atmosphere", 0.25f, 0.5f);
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_CarEffects", 0.25f, 0.5f);
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_Engines", 0.25f, 0.5f);
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_Cars_Human", 0.25f, 0.5f);
		AudioManager.Instance.FadeOutComponent("Audio_SFXAudio_Cars_AI", 0.25f, 0.5f);
	}
}
