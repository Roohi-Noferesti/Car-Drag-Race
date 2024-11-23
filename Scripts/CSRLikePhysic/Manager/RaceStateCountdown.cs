using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RaceStateCountdown : RaceStateBase
{
	private bool startRaceFromTrafficLightSignal;

    private BubbleMessage throttleMessage;

	private bool ShownInstruction;

    private float m_startTime;

    private float m_randomTimeToExitInOnlineMode;

    private bool m_countDownStarted;

	public RaceStateCountdown(RaceStateMachine inMachine) : base(inMachine, RaceStateEnum.countdown)
	{
	}

	public override void Enter()
	{
        m_startTime = Time.time;
	    m_randomTimeToExitInOnlineMode = GameDatabase.Instance.Online.GetRandomFakeCountDownDelay();
        this.machine.controller.Events.FireEvent("RaceCountdownStarted");
	    RaceHUDController.Instance.hudRaceCentreMessage.FireStartRaceFromTrafficLightsEvent +=
	        new Action(this.RaceStartFromTrafficLightsEventHandler);

	    if (!RaceEventInfo.Instance.IsSMPEvent)
	    {
	        RaceHUDController.Instance.hudRaceCentreMessage.StartCountdown();
	        m_countDownStarted = true;
	    }
	    if (RaceStartNames.Instance != null)
        {
            RaceStartNames.Instance.FadeOff();
        }
        this.startRaceFromTrafficLightSignal = false;
        this.ShownInstruction = false;

	    //Added by me :mojtaba.
	    //RaceStartFromTrafficLightsEventHandler();

	    CompetitorManager.Instance.EnableVisualAnimation(false);
	}

	public override void FixedUpdate()
	{
		CompetitorManager.Instance.UpdateGridFixedUpdate();
        if (!this.ShownInstruction && !RaceHUDController.Instance.HUDAnimator.IsAnimating())
        {
            //We comment out this code here because it conflict with InGameLessonThrottleTutorial bubble message
            //If you are intersted to show throttle istruction again here , Go ahead and add some code to prevent double showing it
            //Wen InGameLessonThrottleTutorial is Active
            //PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            //if (!activeProfile.HasCompletedFirstCrewRace() && (RaceEventInfo.Instance.IsDailyBattleEvent || activeProfile.DailyBattlesLastEventAt == DateTime.MinValue) && !NitrousTutorial.Instance.IsActive())
            //{
            //    Vector3 centralGearChangeDevicePosition = RaceHUDController.Instance.GetCentralGearChangeDevicePosition();
            //    BubbleMessage.NippleDir nippleDir = BubbleMessage.NippleDir.DOWN;
            //    this.throttleMessage = BubbleManager.Instance.ShowMessage("TEXT_TUTORIAL_MESSAGE_RACE_THROTTLE", false, centralGearChangeDevicePosition, nippleDir, 0.1f, BubbleMessageConfig.DuringRace);
            //}
            this.ShownInstruction = true;
        }

        //Added by me : Mojtaba
        //if (!startRaceFromTrafficLightSignal && Time.time - m_time > 3)
        //{
        //    startRaceFromTrafficLightSignal = true;
        //}

        if (this.startRaceFromTrafficLightSignal)
        {
            this.startRaceFromTrafficLightSignal = false;
            this.machine.SetState(RaceStateEnum.race);
        }

        if (!m_countDownStarted && RaceEventInfo.Instance.IsSMPEvent && Time.time - m_startTime > m_randomTimeToExitInOnlineMode)
        {
            RaceHUDController.Instance.hudRaceCentreMessage.StartCountdown();
            m_countDownStarted = true;
        }

        if (CheatEngine.Instance != null && CheatEngine.Instance.forceEndRaceState != ForceEndRaceState.DONTCARE)
        {
            this.machine.SetState(RaceStateEnum.race);
        }
    }

	public override void Exit()
	{
        RaceHUDController.Instance.hudRaceCentreMessage.FireStartRaceFromTrafficLightsEvent -= new Action(this.RaceStartFromTrafficLightsEventHandler);
        if (this.throttleMessage != null)
        {
            this.throttleMessage.Dismiss();
            this.throttleMessage = null;
        }
	}

	public void RaceStartFromTrafficLightsEventHandler()
	{
        this.startRaceFromTrafficLightSignal = true;
	}
}
