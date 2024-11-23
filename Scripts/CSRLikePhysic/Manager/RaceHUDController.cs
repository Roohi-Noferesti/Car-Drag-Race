using UnityEngine;

[AddComponentMenu("GT/NewRaceHUD/RaceHUDController")]
public class RaceHUDController : MonoBehaviour
{
	public GameObject pauseButton;
	
	public RaceProgress hudRaceProgress;

	public RaceTime hudRaceTime;

	public Tacho hudTacho;

	public Speedo hudSpeedo;

	public GearDisplay hudGearDisplay;

	public WheelspinDisplay hudWheelspinDisplay;

	public NitrousDisplay hudNitrousDisplay;

	public NitrousButtonDisplay hudNitrousButtonDisplay;

	public RecordButtonDisplay hudRecordButtonDisplay;

	public GearLightsDisplay hudGearLightsDisplay;

	public RaceGearMessaging hudRaceGearMessage;

	public LowerRaceGearMessaging hudRaceGearMessageLower;

	public RaceCentreMessaging hudRaceCentreMessage;

	public InputManager hudInputManager;

	public RaceHUDAnimator HUDAnimator;

	public static RaceHUDController Instance;

	private CarPhysics humanCarPhysics;

	private CarPhysics opponentCarPhysics;

	private int currentGear;

	private int lastGear = -1;

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;

        if (UICamera.Instance != null)
        {
            GetComponentInChildren<Canvas>().worldCamera = UICamera.Instance.Camera;
        }
	}

	public void SetHumanPhysics(CarPhysics physicsObj)
	{
		this.humanCarPhysics = physicsObj;
		this.hudRaceProgress.DisplayOpponentProgess(true);
	}

	public void Reset(CarPhysics HumanPhysics, float RaceDistance)
	{
		this.humanCarPhysics = HumanPhysics;
		this.opponentCarPhysics = null;
		if (this.humanCarPhysics != null)
		{
            this.UpdateTacho();
			this.hudTacho.maximumRevs = this.humanCarPhysics.Engine.RedLineRPM;
			this.hudRaceProgress.raceDistance = RaceDistance;
		}
        //this.HUDAnimator = base.GetComponent<RaceHUDAnimator>();
		this.HUDAnimator.Reset();
		this.hudTacho.Reset();
		this.hudSpeedo.Reset();
		this.hudGearDisplay.Reset();
		this.hudGearLightsDisplay.Reset(true);
		this.hudNitrousDisplay.Reset();
		this.hudNitrousButtonDisplay.Reset();
		this.hudWheelspinDisplay.Reset();
		this.hudRaceProgress.Reset();
		this.hudRaceTime.Reset();
		this.hudRaceCentreMessage.Reset();
		this.hudRaceGearMessage.Reset();
		this.hudRaceGearMessageLower.Reset();
		this.lastGear = -1;
		this.currentGear = 0;
		SetPauseButton();
	}

	public void SetOpponentPhysics(CarPhysics AIPhysics)
	{
		this.opponentCarPhysics = AIPhysics;
		this.hudRaceProgress.DisplayOpponentProgess(true);
	}

	public void SetRaceTime(float RaceTime)
	{
		this.hudRaceTime.currentTime = RaceTime;
	}

	public void SetPauseButton()
	{
		pauseButton.SetActive(!RaceEventInfo.Instance.IsNonPausable());
	}

	public InputManager GetInputManager()
	{
		return this.hudInputManager;
	}

	private void Update()
	{
		if (!PauseGame.isGamePaused && this.humanCarPhysics != null)
		{
			this.currentGear = this.humanCarPhysics.GearBox.CurrentGear;
            if (PlayerProfileManager.Instance.ActiveProfile.UseMileAsUnit)
                this.hudSpeedo.currentSpeed = (int) this.humanCarPhysics.SpeedMPH;
            else
            {
                this.hudSpeedo.currentSpeed = (int) this.humanCarPhysics.SpeedKPH;
            }

            this.hudGearDisplay.currentGear = this.currentGear;
			this.hudWheelspinDisplay.isWheelSpinning = (this.humanCarPhysics.Wheels.WheelSpin > 0f);
			this.hudTacho.currentRevs = this.humanCarPhysics.Engine.GaugeVisibleRPM;
			float currentNitrous = this.humanCarPhysics.Engine.NitrousTimeLeft / this.humanCarPhysics.Engine.NitrousTimeLeftAtStart;
			bool anyNitrousAvailable = this.humanCarPhysics.NitrousAvailable || this.humanCarPhysics.Engine.SuperNitrousAvailable;
			this.hudNitrousDisplay.anyNitrousAvailable = anyNitrousAvailable;
			this.hudNitrousDisplay.currentNitrous = currentNitrous;
			this.hudNitrousButtonDisplay.anyNitrousAvailable = anyNitrousAvailable;
			this.hudNitrousButtonDisplay.currentNitrous = currentNitrous;
            //this.hudRecordButtonDisplay.Hide |= !this.hudRaceCentreMessage.MainRaceCountdown.IsWaiting;
            this.hudGearLightsDisplay.currentRevs = this.humanCarPhysics.Engine.CurrentRPM;
		    this.hudGearLightsDisplay.redlineRpm = this.humanCarPhysics.Engine.RedLineRPM;
			this.hudGearLightsDisplay.state = this.humanCarPhysics.GearChangeState;
            //Debug.Log(hudGearLightsDisplay.state.inNeutralGear + "   " + humanCarPhysics.GearBox.CurrentGear);
			this.hudRaceProgress.humanCarDistance = this.humanCarPhysics.DistanceTravelled;
			if (this.opponentCarPhysics != null)
			{
				this.hudRaceProgress.opponentCarDistance = this.opponentCarPhysics.DistanceTravelled;
			}
			if (this.currentGear != this.lastGear)
			{
				this.lastGear = this.currentGear;
                this.UpdateTacho();
				Instance.HUDAnimator.EnableGearDownPaddle(this.currentGear > 1);
				if (this.humanCarPhysics.GearBox.IsInTopGear)
				{
					this.hudRaceGearMessageLower.ResetText();
				}
			}
            this.UpdateTacho();
		}
	}

    private void UpdateTacho()
    {
        this.hudTacho.maximumRevs = this.humanCarPhysics.Engine.RedLineRPM;
        float num = this.humanCarPhysics.RedLineRPM - this.humanCarPhysics.IdleRPM;
        this.hudTacho.minimumGoodRevs = this.humanCarPhysics.GearChangeLogic.State.goodGearStartNumber * num + this.humanCarPhysics.IdleRPM;
        this.hudTacho.minimumPerfectRevs = this.humanCarPhysics.GearChangeLogic.State.perfectGearStartNumber * num + this.humanCarPhysics.IdleRPM;
        this.hudTacho.maximumPerfectRevs = this.humanCarPhysics.GearChangeLogic.State.perfectGearEndNumber * num + this.humanCarPhysics.IdleRPM;
        if (this.humanCarPhysics.GearBox.IsInTopGear)
        {
            this.hudTacho.showGreenLine = false;
        }
        else
        {
            this.hudTacho.showGreenLine = true;
            this.hudTacho.maximumGoodRevs = this.humanCarPhysics.GearChangeLogic.State.lateGearStartNumber * num + this.humanCarPhysics.IdleRPM;
        }
        this.hudTacho.minimumBadRevs = this.hudTacho.minimumGoodRevs - 500f;
        this.hudTacho.maximumBadRevs = this.hudTacho.maximumGoodRevs + 500f;
        //this.hudTacho.ResetNotchesAndNumbersAndLines();
        //if (RaceHUDManager.HUD_V2)
        //{
        //    this.HUDShiftZones.SetShiftZones();
        //}
    }

    public Vector3 GetCentralGearChangeDevicePosition()
    {
        //return this.hudGearLightsDisplay.lights[this.hudGearLightsDisplay.lights.Count - 1].transform.position + new Vector3(0f, 0.09f, -0.3f);
        return new Vector3();
    }

    public void Reload()
    {
        RaceController.Instance.ResetRace();
    }
}
