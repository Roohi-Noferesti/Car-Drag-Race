using System.Collections;
using System.Collections.Generic;
using KingKodeStudio;
using UnityEngine;

public class RaceStateEnter : RaceStateBase
{
	private RaceCompetitor local;

	private RaceCompetitor other;

	private bool doneChatter;

	private bool switchToPreCountdown;

    private float m_startTime;

    private float m_randomTimeToExitInOnlineMode;

	public static bool switchToPreCountdownASAP;

	public RaceStateEnter(RaceStateMachine inMachine) : base(inMachine, RaceStateEnum.enter)
	{
	}

	public override void Enter()
	{
        if (CheatEngine.Instance != null) {
            CheatEngine.Instance.forceEndRaceState = ForceEndRaceState.DONTCARE;
        }

        m_startTime = Time.time;
	    m_randomTimeToExitInOnlineMode = Random.Range(1, 7);
        this.local = CompetitorManager.Instance.LocalCompetitor;
        this.other = CompetitorManager.Instance.OtherCompetitor;
        Time.timeScale = Z2HInitialisation.timeScale;
        Time.fixedDeltaTime = Z2HInitialisation.fixedDeltaTime;
        this.PositionCars();
        this.ResetAudio();
        if (RaceLightsManager.instance != null)
        {
            RaceLightsManager.instance.ResetScene();
        }
        if (RaceCrowdsManager.Instance != null)
        {
            RaceCrowdsManager.Instance.StartFlashes();
        }


        if (!switchToPreCountdownASAP)
        {
            this.switchToPreCountdown = false;
            this.doneChatter = false;
            string zName = (this.other == null) ? "PreLights1Car" : "PreLights";
            SequenceManager.Instance.PlaySequence(zName);
            SequenceManager.Instance.OnSequenceEnd += this.PreLightsSequenceEnd;

            if (this.other == null)
            {
                CompetitorManager.Instance.LocalCompetitor.CarVisuals.EnableAnimation(true);
            }
        }
        else
        {
            this.switchToPreCountdown = true;
            this.doneChatter = true;
            switchToPreCountdownASAP = false;
        }

        if (RaceEventInfo.Instance != null && RaceEventInfo.Instance.IsSMPEvent)
        {
            if (PlayerProfileManager.Instance.ActiveProfile != null)
            {
                PlayerProfileManager.Instance.ActiveProfile.SMPTotalRaces++;
                PlayerProfileManager.Instance.ActiveProfile.SMPTotalRacesLastSession++;
                if (PlayerProfileManager.Instance.ActiveProfile.SMPTotalRaces % 5 == 0 && PlayerProfileManager.Instance.ActiveProfile.SMPTotalRaces <= 100)
                {
                    //Apsalar.SendEvent("SMP Race " + PlayerProfileManager.Instance.ActiveProfile.SMPTotalRaces);
                    //AdjustMetricsHelper.LogSMPRace(PlayerProfileManager.Instance.ActiveProfile.SMPTotalRaces);
                }
            }
            //ZTrackMetricsHelper.LogSMPStartRace();
        }

        SceneLoadManager.Instance.CompleteLoadingScene();
	    CoroutineManager.Instance.StartCoroutine(delayedPushDummyScreen());
        //PhilsFlag.Instance.Hide();

        if (WinLoseScreen.Instance != null)
        {
            WinLoseScreen.Instance.gameObject.SetActive(false);
        }
        
        BazaarGameHubManager.Instance.Initialise(false);

        RaceRewardScreen.alreadyAwardedWinStreakThisRace = false;
    }

    private IEnumerator delayedPushDummyScreen()
    {
        yield return new WaitForSeconds(1);
        if (ScreenManager.Instance.CurrentScreen != ScreenID.Dummy)
        {
            ScreenManager.Instance.PushScreen(ScreenID.Dummy);
            ScreenManager.Instance.UpdateImmediately();
        }
    } 

	public override void FixedUpdate()
	{
        //if (LoadingScreenManager.Instance.IsFading)
        //{
        //    return;
        //}
        CompetitorManager.Instance.UpdateEnterStateFixedUpdate();
        if (!this.doneChatter)
        {
            this.doneChatter = true;
            //Chatter.PreRace(new PopUpButtonAction(RaceController.Instance.OnTauntDismiss), RaceEventInfo.Instance.CurrentEvent);
        }
        else if (!PopUpManager.Instance.isShowingPopUp && RaceController.Instance.Inputs.InputState.CatchAll)
        {
            this.switchToPreCountdown = true;
        }
        if (this.switchToPreCountdown)
        {
            CompetitorManager.Instance.SwitchToRaceWheels();
            this.machine.SetState(RaceStateEnum.precountdown);
        }

	    if (RaceEventInfo.Instance.IsSMPEvent && Time.time - m_startTime > m_randomTimeToExitInOnlineMode)
	    {
            this.switchToPreCountdown = true;
	    }
	}

	public override void Exit()
	{
        CompetitorManager.Instance.LocalCompetitor.CarVisuals.EnableAnimation(false);
        SequenceManager.Instance.OnSequenceEnd -= this.PreLightsSequenceEnd;
	}

	private void PreLightsSequenceEnd(string zSequence)
	{
		this.switchToPreCountdown = true;
	}

	private void PositionCars()
	{
        CarPhysics humanCarPhysic = this.local.CarPhysics;
        GameObject humanGameobject = this.local.GameObject;
        Transform humanStartPosition = RaceEnvironmentSettings.Instance.HumanStartPosition;
        humanGameobject.transform.position = humanStartPosition.position;
        //gameObject.transform.rotation = humanStartPosition.rotation;
        //CarVisuals component = humanGameobject.GetComponent<CarVisuals>();
        //float z = component.BodyBounds.size.z;
        humanCarPhysic.Position = humanCarPhysic.gameObject.transform.position;
        humanCarPhysic.ResetPhysics();
        if (this.other == null)
        {
            return;
        }
        CarPhysics aiCarPhysic = this.other.CarPhysics;
        GameObject aiGameobject = this.other.GameObject;
        Transform aiStartPosition = RaceEnvironmentSettings.Instance.AiStartPosition;
        aiGameobject.transform.position = aiStartPosition.position;
        //gameObject2.transform.rotation = aiStartPosition.rotation;
        //CarVisuals component2 = aiGameobject.GetComponent<CarVisuals>();
        //float z2 = component2.BodyBounds.size.z;
        //float num = z - z2;
        Vector3 position = aiGameobject.transform.position;
        //position.z += num / 2f;
        aiCarPhysic.Position = position;
        aiCarPhysic.ResetPhysics();
        aiGameobject.GetComponent<AIPlayer>().Reset();
	}

	private void ResetAudio()
	{
        this.local.GameObject.GetComponent<RaceCar>().carAudio.Reset();
        if (this.other != null)
        {
            this.other.GameObject.GetComponent<RaceCar>().carAudio.Reset();
        }
        RaceCarAudio.FadeUpCarAudio(1f);
        MenuAudio.Instance.setMusicPlaying(false);
	}
}
