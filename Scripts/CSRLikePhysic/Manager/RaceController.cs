using System.Collections;
using KingKodeStudio;
using Metrics;
using UnityEngine;

[AddComponentMenu("GT/Logic/RaceController")]
public class RaceController : MonoBehaviour
{
	public static RaceController Instance;

	private static bool raceIsRunning;

    private RaceStateMachine raceStateMachine = new RaceStateMachine();

	private bool resetInputs = true;

	public bool RaceIsRestart
	{
		get;
		set;
	}

    public RaceEvents Events
    {
        get;
        private set;
    }

    public RaceStateMachine Machine
    {
        get
        {
            return raceStateMachine;
        }
    }

    public InputManager Inputs
    {
        get;
        set;
    }

    private void Awake()
	{
        Instance = this;
        //if (SceneLoadManager.Instance == null)
        //{
        //    base.enabled = false;
        //}

        ApplicationManager.WillResignActiveEvent += ApplicationManager_WillResignActiveEvent;

        if (!PolledNetworkState.IsNetworkConnected)
        {
            SMPNetworkManager.Instance.SMPYouLeftRace = true;
            SMPNetworkManager.Instance.SMPRaceInvalidated = true;
        }
	}

    void ApplicationManager_WillResignActiveEvent()
    {
        if (raceIsRunning && RaceEventInfo.Instance != null && RaceEventInfo.Instance.IsSMPEvent && /*!TutorialQuery.IsStoryActive("PlayerOnline") &&*/ 
            SMPNetworkManager.Instance != null /*&& (SMPNetworkManager.Instance.IsConnectedToOpponent() || RaceEventInfo.Instance.IsSMPBotRace)*/)
        {
            if (true)//SMPNetworkManager.Instance.SMPYouTotalWithoutFocusTime > SMPConfigManager.General.TotalLossFocusAllowedTime)
            {
                //SMPNetworkManager.Instance.LeaveGameWithImmediatePriority();
                SMPNetworkManager.Instance.SMPYouLeftRace = true;
                SMPNetworkManager.Instance.SMPRaceInvalidated = true;
                //SMPNetworkManager.Instance.SMPYouTotalWithoutFocusTime = 0f;
            }
            //else
            //{
            //    SMPRaceControllerRPCs raceControllerRPCs = SMPNetworkManager.Instance.GetRaceControllerRPCs();
            //    if (raceControllerRPCs != null)
            //    {
            //        raceControllerRPCs.SendSMPRaceLostFocus();
            //    }
            //}
        }
    }

	IEnumerator Start()
	{
        Events = new RaceEvents();
        Inputs = null;
        raceStateMachine.Initialise(this);
		raceIsRunning = true;
		RaceCompetitor localCompetitor = CompetitorManager.Instance.LocalCompetitor;
		if (localCompetitor != null)
		{
		    while (localCompetitor.CarPhysics == null)
		        yield return 0;

			localCompetitor.SetupRaceEvents();
		}
		RaceCompetitor otherCompetitor = CompetitorManager.Instance.OtherCompetitor;
		if (otherCompetitor != null)
		{
            while (otherCompetitor.CarPhysics == null)
                yield return 0;

			otherCompetitor.SetupRaceEvents();
		}
        //if (PlayerProfileManager.Instance != null)
        //{
        //    PlayerProfileManager.Instance.OnSceneChanged();
        //}

        //Events.HandleEvent("RaceStart", () =>
        //{
        //    float speed;
        //    BaseRunCarPhysicsInTightLoop b = new BaseRunCarPhysicsInTightLoop(localCompetitor.CarPhysics);
        //    b.StartTightLoopRun();
        //    b.Execute(100);
        //    //Debug.Log("end");
        //    //Debug.Log(b.speedMileStoneTimer.mQuarterMileTime);
        //    //var t = CarPhysicsCalculations.ExtrapolateApproximateFinishTime(localCompetitor.CarPhysics, true, out speed);
        //    //Debug.Log(t + "   " + speed);
        //});

	}

	private void OnDestroy()
	{
        if (CompetitorManager.Instance != null)
        {
            CompetitorManager.Instance.LocalCompetitor.RemoveRaceEvents();
        }
        ApplicationManager.WillResignActiveEvent -= ApplicationManager_WillResignActiveEvent;
	}

	private void Update()
	{
        if (Inputs != null)
        {
            if (resetInputs)
            {
                Inputs.Reset();
                resetInputs = false;
            }
            Inputs.PollInputs();
        }
	}

	private void FixedUpdate()
	{
        //if (!Z2HSystemOrder.SystemsReady) return;
        if (!PauseGame.isGamePaused)
        {
            raceStateMachine.FixedUpdate();
        }
		resetInputs = true;
	    if (Inputs != null)
	    {
	        Inputs.InputState.GearChangeUp = false;
	        Inputs.InputState.GearChangeDown = false;
	    }
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void RebakeCarVisuals()
	{
        //RaceStateSetup state = this.raceStateMachine.GetState<RaceStateSetup>(RaceStateEnum.setup);
        //state.RebuildHumanCarVisuals();
        //state.RebuildAICarVisuals();
	}

	public void ResetRace()
	{
        //VideoCapture.StopRecording();
        //ScreenManager.Active.HideAll();
        ScreenManager.Instance.PopToScreen(ScreenID.Dummy);
        RaceHUDManager.Instance.ResetHUD();
        if (raceStateMachine.currentId == RaceStateEnum.race)
        {
            RaceCarAudio.FadeDownCarAudio(0f);
            if (IngameTutorial.IsInTutorial)
            {
                CompetitorManager.Instance.LocalCompetitor.RaceCarAudio.Reset();
            }
        }
        else if (raceStateMachine.currentId == RaceStateEnum.end)
        {
            if (IngameTutorial.IsInTutorial)
            {
                CompetitorManager.Instance.LocalCompetitor.RaceCarAudio.Reset();
                if (CompetitorManager.Instance.OtherCompetitor != null)
                {
                    CompetitorManager.Instance.OtherCompetitor.RaceCarAudio.Reset();
                }
            }
            RaceCarAudio.FadeUpCarAudio(1f);
            MenuAudio.Instance.setMusicPlaying(false);
        }
        RaceIsRestart = true;
        Events.FireEvent("RaceReset");
        CompetitorManager.Instance.ResetCarVisuals();
        CompetitorManager.Instance.LocalCompetitor.CarPhysics.ResetPhysics();
        raceStateMachine.SetState(RaceStateEnum.enter);
        RaceHUDController.Instance.HUDAnimator.DismissHUD();
        PauseGame.UnPause();
	}

	public void EndRace()
	{
        RaceHUDController.Instance.HUDAnimator.DismissHUD();
        raceStateMachine.SetState(RaceStateEnum.end);
        Instance.SetRaceRunning(false);
        Events.FireEvent("RaceEnd");
        raceIsRunning = false;
	}

	public void BackToFrontend()
	{
        raceStateMachine.SetState(RaceStateEnum.exit);
        SceneLoadManager.Instance.RequestScene(SceneLoadManager.Scene.Frontend);
        MenuAudio.Instance.setMusicPlaying(true);
        Time.fixedDeltaTime = Z2HInitialisation.fixedDeltaTime;
        Time.timeScale = Z2HInitialisation.timeScale;
        RaceHUDManager.Instance.DestroyHUD();
        //MetricsCalculate.SetTimeToNow();
	}

	public static bool RaceIsRunning()
	{
	    return raceIsRunning;
	}



	public void SetRaceRunning(bool state)
	{
		raceIsRunning = state;
	}

    public void OnTauntDismiss()
    {
        RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
        if (currentEvent != null && currentEvent.IsCrewRace() && currentEvent.Parent != null &&
            currentEvent.Parent.GetTierEvents().GetCarTier() == eCarTier.TIER_1 &&
            PlayerProfileManager.Instance.ActiveProfile.RacesEntered == 1 &&
            PlayerProfileManager.Instance.ActiveProfile.RacesWon == 1)
        {
            Log.AnEvent(Metrics.Events.PapaGrief);
        }
        PauseGame.UnPause();
    }

    public void OnTauntDismissRace()
	{
        SequenceManager.Instance.TriggerSequenceEndCall();
        PauseGame.UnPause();
	}

	public bool HasHumanTimeBeenSet()
	{
        RaceStateBase current = raceStateMachine.current;
        if (current.id == RaceStateEnum.race)
        {
            RaceStateRace raceStateRace = current as RaceStateRace;
            if (raceStateRace.HasHumanTimeBeenSet)
            {
                return true;
            }
        }
        return current.id == RaceStateEnum.end || current.id == RaceStateEnum.exit;
	}
}
