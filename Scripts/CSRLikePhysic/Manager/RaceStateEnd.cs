//using DataSerialization;

using System.Collections.Generic;
using System.Linq;
using DataSerialization;
using KingKodeStudio;
using Metrics;
using UnityEngine;
using UnityEngine.UI;

public class RaceStateEnd : RaceStateBase
{
	public delegate void RaceFinishEvent();

	private const float BeforeResultPauseTime = 0.1f;

    private Text bestRaceTimeText;

	private float timer;

	private bool showResults;

	private bool triggeredWinLose;

	private bool shownTaunt;

    private AudioSfx MusicToPlay;

    public static event RaceFinishEvent OnRaceFinish;

	public RaceStateEnd(RaceStateMachine inMachine) : base(inMachine, RaceStateEnum.end)
	{
	}

	public override void Enter()
	{
        GameObject gameObject = GameObject.FindWithTag("BestQuarterMileTimeText");
        if (gameObject != null)
        {
            this.bestRaceTimeText = gameObject.GetComponent<Text>();
        }
        this.timer = 0f;
        this.showResults = true;
        this.triggeredWinLose = false;
        this.shownTaunt = false;
        if (NitrousTutorial.Instance.IsActive())
        {
            NitrousTutorial.Instance.EndOfRace();
        }
        RaceCarAudio.FadeDownCarAudio(1f);
        this.MusicToPlay = ((!RaceResultsTracker.You.IsWinner) ? AudioSfx.RaceLost : AudioSfx.RaceWon);
        BubbleManager.Instance.DismissMessages();
	    //TouchManager.Instance.GesturesEnabled = true;
	}

	public override void FixedUpdate()
	{
        this.timer += Time.fixedDeltaTime;
        CompetitorManager.Instance.UpdatePostRaceFixedUpdate();
        if (this.timer < 0.2F)
        {
            return;
        }
        if (!this.triggeredWinLose)
        {
            //PhilsFlag.Instance.FadeIn(0.5f, 0f);
            // ScreenManager.Instance.PushScreen(ScreenID.PinsFlags);
            WinLoseScreen.Instance.Show();
            if (RaceResultsTracker.You.IsWinner)
            {
                AudioManager.Instance.PlaySound("HUD/WinRace", null);
            }
            else
            {
                AudioManager.Instance.PlaySound("HUD/LoseRace", null);
            }
            RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
            //string textID = string.Empty;
            //bool flag;
            if (currentEvent.IsRelay && currentEvent != currentEvent.Group.RaceEvents.Last<RaceEventData>())
            {
                if (currentEvent.IsHalfMile)
                {
                    string humanCar = RaceEventInfo.Instance.CurrentEvent.HumanCar;
                    if (GameDatabase.Instance.RelayConfiguration.HMTimes.ContainsKey(humanCar))
                    {
                        float num = 0f;
                        GameDatabase.Instance.RelayConfiguration.HMTimes.TryGetValue(humanCar, out num);
                        //flag = (RaceResultsTracker.You.RaceTime <= num);
                    }
                    else
                    {
                        //flag = RaceResultsTracker.You.IsWinner;
                    }
                }
                else
                {
                    //int humanCarPPIndex = RaceEventDifficulty.Instance.GetHumanCarPPIndex(currentEvent);
                    //float qMTimeForPPIndex =
                    //    CarPerformanceIndexCalculator.GetQMTimeForPPIndex(humanCarPPIndex -
                    //                                                      GameDatabase.Instance.Relay
                    //                                                          .GetRelayBadRunPPDifference());
                    //float num2 = qMTimeForPPIndex - RaceResultsTracker.You.RaceTime;
                    //flag = (num2 > 0f);
                }
                //textID = ((!flag) ? "TEXT_BAD_RUN" : "TEXT_GOOD_RUN");
            }
            else
            {
                //flag = RaceResultsTracker.You.IsWinner;
                //textID = ((!flag) ? "TEXT_HUD_MESSAGE_YOU_LOSE" : "TEXT_HUD_MESSAGE_YOU_WIN");
            }
            //RaceHUDController.Instance.hudRaceCentreMessage.ShowFinishText(LocalizationManager.GetTranslation(textID), flag);
            MenuAudio.Instance.playSound(this.MusicToPlay);
            this.triggeredWinLose = true;
            
            BazaarGameHubManager.Instance.EndTournamentMatch(RaceResultsTracker.You.IsWinner);
        }
        if (this.timer < 2f)
        {
            return;
        }
	    if (!this.shownTaunt)
	    {
            Chatter.PostRace(new PopUpButtonAction(RaceController.Instance.OnTauntDismiss),
                RaceEventInfo.Instance.CurrentEvent, RaceResultsTracker.You.IsWinner);
            this.shownTaunt = true;
        }
	    if (this.showResults && !PopUpManager.Instance.isShowingPopUp)
        {
            this.FinishRace();
        }
	}

	private void FinishRace()
	{
        VideoCapture.ClearRecording();
        VideoCapture.StopRecording();
        Time.fixedDeltaTime = Z2HInitialisation.fixedDeltaTime;
        Time.timeScale = Z2HInitialisation.timeScale;
        RaceHUDController.Instance.hudRaceCentreMessage.HideWinLoseText();
        if (OnRaceFinish != null)
        {
            OnRaceFinish();
        }
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (activeProfile != null)
        {
           PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
        }
        this.showResults = false;
        SequenceManager.Instance.PlaySequence("RaceEnd");


        if (this.bestRaceTimeText != null)
        {
            bestRaceTimeText.text = string.Format("Best 1/4 Mile Time : {0:0.##}s", PlayerProfileManager.Instance.ActiveProfile.BestOverallQuarterMileTime);
        }
        if (RaceEventInfo.Instance.CurrentEvent.IsTutorial())
        {
            if (IngameTutorial.IsInTutorial && (IngameTutorial.Instance.CurrentLesson == null ||
                (IngameTutorial.IsIn2ndTutorialRace && IngameTutorial.Instance.CurrentLesson == IngameTutorial.Instance.LastLessonTutorial2 && 
                ((IngameLessonEndOfRace2)IngameTutorial.Instance.CurrentLesson).Done) ||
                (IngameTutorial.IsIn3dTutorialRace && IngameTutorial.Instance.CurrentLesson == IngameTutorial.Instance.LastLessonTutorial3 &&
                ((IngameLessonEndOfRace3)IngameTutorial.Instance.CurrentLesson).Done)))
            {
                this.AddPostTutorialCash();

                //This is because we may inject highest nitrous level for player to help him win , so we take back our gift here
                if (RaceEventInfo.Instance.CurrentEvent.IsTutorial3thRace())
                {
                    var currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
                    currentCar.UpgradeStatus[eUpgradeType.NITROUS].levelFitted = 1;
                    currentCar.UpgradeStatus[eUpgradeType.NITROUS].levelOwned = 1;
                }
                RaceController.Instance.BackToFrontend();
            }
        }
        else if (RaceEventInfo.Instance.CurrentEvent.IsRelay)
        {
            RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
            PinDetail worldTourPinPinDetail = currentEvent.GetWorldTourPinPinDetail();
            ScheduledPin worldTourScheduledPinInfo = worldTourPinPinDetail.WorldTourScheduledPinInfo;
            if (worldTourScheduledPinInfo != null && (!currentEvent.IsRelay || RelayManager.GetNextRelayEvent() == null))
            {
                IGameState gameState = new GameStateFacade();
                string currentThemeName = TierXManager.Instance.CurrentThemeName;
                gameState.SetPinToRaced(currentThemeName, worldTourScheduledPinInfo, RaceResultsTracker.You.IsWinner);
            }
            PlayerProfileManager.Instance.ActiveProfile.RaceCompleted(RaceResultsTracker.You, RaceResultsTracker.Them);
            Log.AnEvent(Events.CompleteRace, new Dictionary<Parameters, string>
            {
                {
                    Parameters.CmlRly,
                    PlayerProfileManager.Instance.ActiveProfile.NumberOfRelaysCompetetedIn.ToString()
                }
            });
            RaceReCommon.GoBackToFrontEnd();
        }
        else
        {
            this.LaunchRaceResultsScreen();
        }
        if (RaceCrowdsManager.Instance != null)
        {
            RaceCrowdsManager.Instance.StopFlashes();
        }
	}

	private void AddPostTutorialCash()
	{
        //int num = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash();
        //int num2 = 25000 - num;
        //if (num2 > 0)
        //{
        //PlayerProfileManager.Instance.ActiveProfile.AddCash(num2);
        //}
	}

	private void LaunchRaceResultsScreen()
	{
        if (RaceRewardScreen.Instance == null)
        {
            ScreenManager.Instance.PushScreen(ScreenID.RaceRewards);
        }
        else
        {
            ScreenManager.Instance.PushScreenFromGameObject(RaceRewardScreen.Instance, ScreenID.RaceRewards);
        }

	    //ScreenManager.Instance.PushScreen(ScreenID.RaceRewards);

	}
}
