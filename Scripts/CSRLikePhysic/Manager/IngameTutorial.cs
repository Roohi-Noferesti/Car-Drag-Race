using System.Collections.Generic;
using Metrics;
using UnityEngine;

public class IngameTutorial : MonoBehaviour
{
    public enum TutorialPart
    {
        Tutorial1,
        Tutorial2,
        Tutorial3,
        Tutorial4,
    }
    public IngameLessonBase LastLessonTutorial2;
    public IngameLessonBase LastLessonTutorial3;

    private float BadThrottleTime;

	private CarPhysics humanCar;

	public float LateShiftPopupDelay;

	public Vector3 LateShiftPopupOffset;

	private float LateShiftPopupTimer;

	private BubbleMessage LateShiftPopup;

	private bool DoShowEarlyShiftPopup;

	private int currentLesson = -1;

    private TutorialPart tutorialPart;

    public List<IngameLessonBase> Lessons;
    public List<IngameLessonBase> LessonsTutorial2;
    public List<IngameLessonBase> LessonsTutorial3;
    public List<IngameLessonBase> LessonsTutorialUpgradeNitro;
    
    public List<IngameLessonBase> LessonsGroup
    {
        get
        {
            switch (tutorialPart)
            {
                case TutorialPart.Tutorial1:
                    return Lessons;
                case TutorialPart.Tutorial2:
                    return LessonsTutorial2;
                case TutorialPart.Tutorial3:
                    return LessonsTutorial3;
                case TutorialPart.Tutorial4:
	                return LessonsTutorialUpgradeNitro;
            }
            return null;
        }
    }


	public static IngameTutorial Instance
	{
		get;
		private set;
	}

	public static bool IsInTutorial
	{
		get
		{
			if (RaceEventInfo.Instance.CurrentEvent == null)
			{
				return false;
			}
		    bool flag = RaceEventInfo.Instance.CurrentEvent ==
		                GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial ||
		                RaceEventInfo.Instance.CurrentEvent ==
		                GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial2;
		    return flag && SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Race;
		}
	}

    public static bool IsIn1stTutorialRace
    {
        get
        {
	        return IsInTutorial &&
	               RaceEventInfo.Instance.CurrentEvent ==
	               GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial;
        }
    }
    

    public static bool IsIn2ndTutorialRace
    {
        get
        {
            return IsInTutorial &&
                   RaceEventInfo.Instance.CurrentEvent ==
                   GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial2;
        }
    }

    public static bool IsIn3dTutorialRace
    {
        get
        {
            return IsInTutorial &&
                   RaceEventInfo.Instance.CurrentEvent ==
                   GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial3;
        }
    }

	public IngameLessonBase CurrentLesson
	{
		get
		{
			if (this.currentLesson < 0)
			{
				return null;
			}
			if (this.currentLesson >= this.LessonsGroup.Count)
			{
				return null;
			}
			return this.LessonsGroup[this.currentLesson];
		}
	}



	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
	}

	private void OnDestroy()
	{
		if (this.CurrentLesson != null)
		{
			this.CurrentLesson.StateOnExit();
		}
		if (this.humanCar != null)
		{
			this.UnRegisterGearChangeEvents();
		}
		this.humanCar = null;
	}

	public void StartTutorial(TutorialPart part)
	{
	    tutorialPart = part;
		this.SetState(0);
	}

    public void SetState(int zNewLesson)
	{
		if (this.CurrentLesson != null)
		{
			this.CurrentLesson.StateOnExit();
		}
		this.currentLesson = zNewLesson;
		if (this.CurrentLesson != null)
		{
			this.CurrentLesson.StateOnEnter();
		}
	}

	private void RegisterGearChangeEvents()
	{
		this.humanCar.GearChangeLogic.FireGearChangeUpEvent += new GearChangeLogic.ChangeGearUpEventDelegate(this.CheckForBadLaunch);
		this.humanCar.PreChangeGearUpEventDelegateList.Add(new CarPhysics.PreChangeGearUpEventDelegate(this.PreCheckForTooEarlyShift));
	}

	private void UnRegisterGearChangeEvents()
	{
		this.humanCar.GearChangeLogic.FireGearChangeUpEvent -= new GearChangeLogic.ChangeGearUpEventDelegate(this.CheckForBadLaunch);
		this.humanCar.PreChangeGearUpEventDelegateList.Remove(new CarPhysics.PreChangeGearUpEventDelegate(this.PreCheckForTooEarlyShift));
	}

	protected virtual void LateUpdate()
	{
		if (this.CurrentLesson == null)
		{
			return;
		}
		if (this.CurrentLesson.StateUpdate())
		{
			this.SetState(this.currentLesson + 1);
		}

		if (tutorialPart != TutorialPart.Tutorial4)
		{
			if (this.humanCar != CompetitorManager.Instance.LocalCompetitor.CarPhysics)
			{
				if (this.humanCar != null)
				{
					this.UnRegisterGearChangeEvents();
				}

				this.humanCar = CompetitorManager.Instance.LocalCompetitor.CarPhysics;

				if (this.humanCar != null)
				{
					this.RegisterGearChangeEvents();
				}
			}

			if (RaceHUDController.Instance != null &&
			    !RaceController.Instance.Machine.StateBefore(RaceStateEnum.precountdown))
			{
				GearChangeLogic.OutputState state = RaceHUDController.Instance.hudGearLightsDisplay.state;
				if (RaceController.Instance.Inputs)
				{
					if (RaceController.Instance.Inputs.InputState.Throttle || PauseGame.isGamePaused ||
					    RaceController.Instance.Machine.currentId != RaceStateEnum.race)
					{
						this.BadThrottleTime = 0f;
					}
					else if (RaceController.Instance.Inputs.InputState.FakeThrottle &&
					         !RaceController.Instance.HasHumanTimeBeenSet())
					{
						this.BadThrottleTime += Time.deltaTime;
						if (this.BadThrottleTime >
						    GameDatabase.Instance.TutorialConfiguration.BadThrottleMessageThreshold)
						{
							this.BadThrottleTime = 0f;
							this.BadThrottlePopUp();
						}
					}
				}

				//Debug.Log(state.inNeutralGear);
				if (!PauseGame.isGamePaused && !RaceController.RaceIsRunning() &&
				    state.normalisedLightsNumber >= state.lateGearStartNumber && !state.inNeutralGear &&
				    this.LateShiftPopup == null)
				{
					this.LateShiftPopupTimer += Time.deltaTime;
					if (this.LateShiftPopupTimer >= this.LateShiftPopupDelay)
					{
						Vector3 position = RaceHUDController.Instance.HUDAnimator.GetGearUpButtonPosition() +
						                   this.LateShiftPopupOffset;
						this.LateShiftPopup = BubbleManager.Instance.ShowMessage(
							"TEXT_POPUPS_TUTORIAL_GEARREMINDER_BODY",
							false, position, BubbleMessage.NippleDir.DOWN, 0.7f, BubbleMessageConfig.DuringRace);
					}
				}
				else if ((!RaceController.Instance.Machine.StateIs(RaceStateEnum.race) ||
				          state.normalisedLightsNumber < state.lateGearStartNumber) && this.LateShiftPopup != null)
				{
					this.LateShiftPopup.Dismiss();
					this.LateShiftPopup = null;
					this.LateShiftPopupTimer = 0f;
				}

				if (this.DoShowEarlyShiftPopup)
				{
					if (RaceController.Instance.Machine.StateIs(RaceStateEnum.race))
					{
						this.TooEarlyShiftingPopUp();
					}
					else
					{
						this.DoShowEarlyShiftPopup = false;
					}
				}
			}
		}

	}

	private bool PreCheckForTooEarlyShift(GearChangeRating gearRating)
	{
		bool flag = this.humanCar.GearBox.CurrentGear > 0 && gearRating == GearChangeRating.VeryEarly;
		if (flag)
		{
			this.DoShowEarlyShiftPopup = true;
		}
		return !flag;
	}

	private void CheckForBadLaunch(GearChangeRating gearRating)
	{
		if (gearRating == GearChangeRating.WheelspinLaunch)
		{
			if (this.humanCar.Engine.CurrentRPM / this.humanCar.Engine.RedLineRPM > 0.95f)
			{
				this.WheelspinStartPopUp();
			}
		}
		else if (gearRating == GearChangeRating.SlowLaunch && this.humanCar.Engine.CurrentRPM == this.humanCar.Engine.IdleRPM)
		{
			this.IdleStartPopUp();
		}
	}

	public void ShowPoorStartPopUp(string title, string body, Events metric)
	{
		PopUpButtonAction confirmAction;
		string confirmText;
		if (IsIn2ndTutorialRace)
		{
			confirmAction = new PopUpButtonAction(this.OnDismissRestartRace);
			confirmText = "TEXT_BUTTON_RESTART";
		}
		else
		{
			confirmAction = new PopUpButtonAction(this.OnDismissResumeRace);
			confirmText = "TEXT_BUTTON_OK";
		}
		PopUp popup = new PopUp
		{
			Title = title,
			BodyText = body,
			IsBig = false,
            IsCrewLeader = true,
			ConfirmAction = confirmAction,
			ConfirmText = confirmText,
            ItemGraphicPath = PopUpManager.Instance.graphics_greenLightGas,
			ShouldCoverNavBar = true
		};
		Log.AnEvent(metric);
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		PauseGame.Pause(false);
	}

	public void IdleStartPopUp()
	{
		this.ShowPoorStartPopUp("TEXT_POPUPS_TUTORIAL_THROTTLE2_TITLE", "TEXT_POPUPS_TUTORIAL_THROTTLE2_BODY", Events.WarnIdleStart);
	}

	public void WheelspinStartPopUp()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_TUTORIAL_THROTTLE2_TITLE",
			BodyText = "TEXT_POPUPS_TUTORIAL_DONT_HOLD_PEDAL",
			IsBig = false,
            IsCrewLeader = true,
			ConfirmAction = new PopUpButtonAction(this.OnDismissRestartRace),
			ConfirmText = "TEXT_BUTTON_RESTART",
            ItemGraphicPath = PopUpManager.Instance.graphics_greenLightGas,
			ShouldCoverNavBar = true
		};
		Log.AnEvent(Events.WarnWheelspinStart);
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		PauseGame.Pause(false);
	}

	public void TooEarlyShiftingPopUp()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_TUTORIAL_EARLY_SHIFT_TITLE",
			BodyText = "TEXT_TUTORIAL_EARLY_SHIFT_BODY",
			IsBig = false,
            IsCrewLeader = true,
			ConfirmAction = new PopUpButtonAction(this.OnDismissResumeRace),
			ConfirmText = "TEXT_BUTTON_OK",
            ItemGraphicPath = PopUpManager.Instance.graphics_greenLightShift,
			ShouldCoverNavBar = true
		};
		if (PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null))
		{
			Log.AnEvent(Events.WarnEarlyShift);
			PauseGame.Pause(false);
			this.DoShowEarlyShiftPopup = false;
		}
	}

	public void BadThrottlePopUp()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_TUTORIAL_BAD_THROTTLE_TITLE",
			BodyText = "TEXT_TUTORIAL_BAD_THROTTLE_BODY",
			IsBig = true,
			ConfirmAction = new PopUpButtonAction(this.OnDismissResumeRace),
			ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_raceOfficialPrefab,
			ImageCaption = "TEXT_NAME_RACE_OFFICIAL",
			ShouldCoverNavBar = true
		};
		Log.AnEvent(Events.WarnBadThrottle);
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		PauseGame.Pause(false);
	}

	private void OnDismissRestartRace()
	{
		RaceStateEnter.switchToPreCountdownASAP = true;
        RaceHUDController.Instance.hudRaceCentreMessage.Reset();
		RaceController.Instance.ResetRace();
		LoadingScreenManager.Instance.ForceEndLoadingEffects();
	}

	private void OnDismissResumeRace()
	{
		PauseGame.UnPause();
	}
}
