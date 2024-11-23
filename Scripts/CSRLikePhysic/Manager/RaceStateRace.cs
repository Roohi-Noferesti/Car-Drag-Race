using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Threading;
using Metrics;
using UnityEngine;
using Object = UnityEngine.Object;

public class RaceStateRace : RaceStateBase
{
	private CarPhysics humanPhys;

	private CarPhysics aiPhys;

	private SpeedMileStoneTimer humanTimer;

	private SpeedMileStoneTimer aiTimer;

	private float raceDistance;

	private float humanFinishTime;

	private float aiFinishTime;

	private float aiFinishSpeed;

	private bool humanTimeSet;

	private bool aiTimeSet;

	private bool aiTimeExtrapolated;

	private float finishTimer;

    private ShotSequence camSeqGo;

	private bool raceStarted;

	public bool HasHumanTimeBeenSet
	{
		get
		{
			return humanTimeSet;
		}
	}

	public RaceStateRace(RaceStateMachine inMachine) : base(inMachine, RaceStateEnum.race)
	{
	}

	public override void Enter()
	{
        humanPhys = CompetitorManager.Instance.LocalCompetitor.CarPhysics;
        if (CompetitorManager.Instance.OtherCompetitor != null)
        {
            aiPhys = CompetitorManager.Instance.OtherCompetitor.CarPhysics;
            aiTimer = aiPhys.SpeedMileStoneTimer;
        }
        humanTimer = humanPhys.SpeedMileStoneTimer;
        camSeqGo = SequenceManager.Instance.GetSequence("RaceGo");
        if (SequenceManager.Instance.ActiveSequence != camSeqGo)
        {
            SequenceManager.Instance.PlaySequence(camSeqGo);
        }
        if (RaceEventInfo.Instance.CurrentEvent == null)
        {
            raceDistance = 500;
        }
        else
        {
            raceDistance = ((!RaceEventInfo.Instance.CurrentEvent.IsHalfMile) ? 402.325f : 804.65f);
        }

        //Added By Me
        //this.raceDistance = 500;

        humanTimeSet = false;
        aiTimeSet = false;
        finishTimer = 0f;
        raceStarted = false;
        
        BazaarGameHubManager.Instance.StartTournamentMatch(UserManager.Instance.currentAccount.UserID.ToString() + ":" +
                                                           RaceEventInfo.Instance.CurrentEvent.EventID.ToString() + ":" +
                                                           Mathf.RoundToInt(Time.time).ToString());
	}

	public override void FixedUpdate()
	{
        if (!raceStarted)
        {
            raceStarted = true;
            machine.controller.Events.FireEvent("RaceStart");
        }
        CompetitorManager.Instance.UpdateRaceFixedUpdate();
        RaceHUDController.Instance.SetRaceTime((!humanTimeSet) ? humanTimer.CurrentTime() : humanFinishTime);
        bool flag = humanPhys.DistanceTravelled > raceDistance;
        if (!humanTimeSet && flag)
        {
            //Debug.Log("Human Time Set at " + Time.realtimeSinceStartup);
            if (PauseGame.isGamePaused)
            {
                PauseGame.UnPause();
                return;
            }
            humanTimeSet = true;
            if (RaceEventInfo.Instance.CurrentEvent == null)
            {
                humanFinishTime = humanTimer.mQuarterMileTime;
            }
            else
            {
                humanFinishTime = ((!RaceEventInfo.Instance.CurrentEvent.IsHalfMile) ? humanTimer.mQuarterMileTime : humanTimer.mHalfMileTime);
            }



            RaceHUDController.Instance.HUDAnimator.DismissHUD();
        }
        if (humanTimeSet)
        {
            if (aiPhys != null)
            {
                bool flag2 = false;
                if (RaceEventInfo.Instance.CurrentEvent != null)
                {
                    flag2 = RaceEventInfo.Instance.CurrentEvent.IsHalfMile;
                }
                finishTimer += Time.fixedDeltaTime;
                if (finishTimer > 0.2f)
                {
                    if (aiPhys.DistanceTravelled < raceDistance)
                    {
                        int ppindex = 0;
                        if (RaceEventInfo.Instance.CurrentEvent != null)
                        {
                            ppindex = RaceEventInfo.Instance.CurrentEvent.GetAIPerformancePotentialIndex();
                        }
                        aiFinishTime = CalculateEstimatedAIFinishTime(aiPhys, flag2, ppindex);
                        aiTimeExtrapolated = true;
                    }
                    else
                    {
                        aiFinishTime = ((!flag2) ? aiTimer.mQuarterMileTime : aiTimer.mHalfMileTime);
                        aiFinishSpeed = ((!flag2) ? aiTimer.SpeedAtQuarter : aiTimer.SpeedAtHalf);
                    }
                    //Debug.Log("Ai Time Set at" + Time.realtimeSinceStartup);
                    aiTimeSet = true;
                }
                else if (aiPhys.DistanceTravelled > raceDistance)
                {
                    //Debug.Log("Ai Time Set at" + Time.realtimeSinceStartup);
                    aiTimeSet = true;
                    aiFinishTime = ((!flag2) ? aiTimer.mQuarterMileTime : aiTimer.mHalfMileTime);
                    aiFinishSpeed = ((!flag2) ? aiTimer.SpeedAtQuarter : aiTimer.SpeedAtHalf);
                }
            }
            else
            {
                aiTimeSet = true;
            }
        }

        if(CheatEngine.Instance != null && CheatEngine.Instance.forceEndRaceState!=ForceEndRaceState.DONTCARE)
        {
            RaceComplete(CheatEngine.Instance.forceEndRaceState);
            CheatEngine.Instance.forceEndRaceState = ForceEndRaceState.DONTCARE;
            machine.SetState(RaceStateEnum.end);
        }

        if (flag && humanTimeSet && aiTimeSet)
        {
            RaceComplete();
            //Debug.Log("Race State End at" + Time.realtimeSinceStartup);
            machine.SetState(RaceStateEnum.end);
        }
	}

	private float CalculateEstimatedAIFinishTime(CarPhysics carPhysics, bool isHalfMile, int ppindex)
	{
        if (RaceEventInfo.Instance.CurrentEvent != null && RaceEventInfo.Instance.CurrentEvent.IsRelay && !isHalfMile && ppindex > 0)
        {
            return CarPerformanceIndexCalculator.GetQMTimeForPPIndex(ppindex - 2);
        }
        return CarPhysicsCalculations.ExtrapolateApproximateFinishTime(aiPhys, isHalfMile, out aiFinishSpeed);
	}

	public void RaceComplete(ForceEndRaceState forceResult = ForceEndRaceState.DONTCARE)
	{
        machine.controller.Events.FireEvent("RaceEnd");
        if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent() || 
            RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
        {
            NetworkCompetitor networkCompetitor = CompetitorManager.Instance.OtherCompetitor as NetworkCompetitor;
            if (networkCompetitor != null)
            {
                if (networkCompetitor.SlowMoMode)
                {
                    aiFinishTime = networkCompetitor.PlaybackReplayData.ReplayData.finishTime;
                }
            }
        }
        RaceResultsData raceResultsData = new RaceResultsData();
        RaceTimeType raceTimeType = RaceEventInfo.Instance.CurrentEvent.GetRaceTimeType();
	    raceResultsData.IsWinner = aiTimer == null || RaceTimesManager.IsPlayerRaceTimeWinner(humanFinishTime, ref aiFinishTime, raceTimeType, forceResult);
        raceResultsData.RaceTime = humanFinishTime;
        raceResultsData.SpeedWhenCrossingFinishLine = ((!RaceEventInfo.Instance.CurrentEvent.IsHalfMile) ? humanTimer.SpeedAtQuarter : humanTimer.SpeedAtHalf);
        raceResultsData.TopRaceSpeedMPH = humanPhys.MaxSpeedMPH;
        raceResultsData.Nought60Time = humanTimer.TimeToReachMilestone[0];
        raceResultsData.Nought100TimeKPH = humanTimer.TimeToReachMilestone[1];
        raceResultsData.Nought100Time = humanTimer.TimeToReachMilestone[2];
        GearChangeLogic gearChangeLogic = humanPhys.GearChangeLogic;
        RacePlayerInfoComponent component = CompetitorManager.Instance.LocalCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
        raceResultsData.WheelSpinDistance = humanPhys.wheelSpinDistance;
        raceResultsData.NumberOfChanges = gearChangeLogic.NumGearChanges;
        raceResultsData.NumberOfEarlyChanges = gearChangeLogic.NumEarlyChanges;
        raceResultsData.NumberOfGoodChanges = gearChangeLogic.NumGoodChanges;
        raceResultsData.NumberOfOptimalChanges = gearChangeLogic.NumPerfectChanges;
        raceResultsData.NumberOfLateChanges = gearChangeLogic.NumLateChanges;
        raceResultsData.CarTierEnum =  component.CarTier;
	    raceResultsData.PerformancePotential =  component.PPIndex;
        raceResultsData.GreatLaunch = gearChangeLogic.DidGreatLaunch;
        raceResultsData.UsedNitrous = humanPhys.HasUsedNitrous;
        raceResultsData.EventID = ((RaceEventInfo.Instance.CurrentEvent == null) ? 0 : RaceEventInfo.Instance.CurrentEvent.EventID);
        raceResultsData.HadBoostNitrousAvailable = (RaceEventInfo.Instance.CurrentEvent != null && RaceEventInfo.Instance.CurrentEvent.IsHighStakesEvent() && BoostNitrous.HaveBoostNitrous());
        raceResultsData.TimeWasExtrapolated = false;
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (activeProfile != null)
        {
            activeProfile.TotalDistanceTravelled += humanPhys.DistanceTravelled;
            activeProfile.GetCurrentCar().DistanceTravelled += humanPhys.DistanceTravelled;
            activeProfile.TutorialRacesEntered++;
            if (
                IngameTutorial
                    .IsInTutorial /*|| GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.LadderEvents.NumEventsComplete() < 3*/ ||
                activeProfile.RacesWon == 0 || activeProfile.DailyBattlesLastEventAt == DateTime.MinValue ||
                RaceEventInfo.Instance.IsFirstCrewRaceEvent())
            {
                var dictionary = new Dictionary<Parameters, string>
                {
                    {Parameters.IsWinner, raceResultsData.IsWinner.ToString()},
                    {Parameters.LaunchRPM, humanPhys.Engine.LaunchRPM.ToString()},
                    {Parameters.TotalShifts, Mathf.Min(humanPhys.GearShifts.Count, 10).ToString()},
                    {Parameters.Shift0, humanPhys.GetGearShiftTypeForMetricsEvent(0)},
                    {Parameters.Shift1, humanPhys.GetGearShiftTypeForMetricsEvent(1)},
                    {Parameters.Shift2, humanPhys.GetGearShiftTypeForMetricsEvent(2)},
                    {Parameters.Shift3, humanPhys.GetGearShiftTypeForMetricsEvent(3)},
                    {Parameters.Shift4, humanPhys.GetGearShiftTypeForMetricsEvent(4)},
                    {Parameters.Shift5, humanPhys.GetGearShiftTypeForMetricsEvent(5)},
                    {Parameters.Shift6, humanPhys.GetGearShiftTypeForMetricsEvent(6)}
                };
                // new Thread(() =>
                // {
                var logGo = new GameObject("logCoroutineGO");
                var coroutineComponent = logGo.AddComponent<CoroutineComponent>();
                LogCompleteRaceEvents(dictionary,logGo, raceResultsData.EventID);
                
                //     }
                //
                // ).Start();
            }
        }

        RaceResultsData raceResultsData2 = null;
        if (aiTimer != null)
        {
            RacePlayerInfoComponent component2 = CompetitorManager.Instance.OtherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
            raceResultsData2 = new RaceResultsData();
            raceResultsData2.IsWinner = !raceResultsData.IsWinner;
            raceResultsData2.RaceTime = aiFinishTime;
            raceResultsData2.SpeedWhenCrossingFinishLine = aiFinishSpeed;
            raceResultsData2.TopRaceSpeedMPH = aiPhys.MaxSpeedMPH;
            raceResultsData2.Nought60Time = aiTimer.TimeToReachMilestone[0];
            raceResultsData2.Nought100TimeKPH = aiTimer.TimeToReachMilestone[1];
            raceResultsData2.Nought100Time = aiTimer.TimeToReachMilestone[2];
            raceResultsData2.CarTierEnum = component2.CarTier;
            raceResultsData2.UsedNitrous = aiPhys.HasUsedNitrous;
            raceResultsData2.TimeWasExtrapolated = aiTimeExtrapolated;
            raceResultsData2.PerformancePotential = component2.PPIndex;
            if (RaceEventInfo.Instance.IsDailyBattleEvent)
            {
                raceResultsData2.CarTierEnum = raceResultsData.CarTierEnum;
                raceResultsData2.PerformancePotential = raceResultsData.PerformancePotential;
            }
            raceResultsData2.EventID = ((RaceEventInfo.Instance.CurrentEvent == null) ? 0 : RaceEventInfo.Instance.CurrentEvent.EventID);
        }
        RaceResultsTracker.You = raceResultsData;
        RaceResultsTracker.Them = raceResultsData2;
        if (raceResultsData.IsWinner)
        {
            PrizeProgression.AddProgress(PrizeProgressionType.RacesWon, 1f);
        }
        PrizeProgression.AddProgress(PrizeProgressionType.RacesCompleted, 1f);
        PrizeProgression.AddProgress(PrizeProgressionType.PerfectShifts, (float)raceResultsData.NumberOfOptimalChanges);
        PrizeProgression.AddProgress(PrizeProgressionType.Perfection, (float)raceResultsData.NumberOfOptimalChanges);
        PrizeProgression.AddProgress(PrizeProgressionType.MilesDriven, (!RaceEventInfo.Instance.CurrentEvent.IsHalfMile) ? 0.25f : 0.5f);
        //if (MultiplayerUtils.SelectedMultiplayerMode == MultiplayerMode.EVENT)
        //{
        //    MultiplayerEventData data = MultiplayerEvent.Saved.Data;
        //    if (data != null)
        //    {
        //        MultiplayerEvent.Saved.AddRacesCompleted(1);
        //    }
        //}
        if (raceResultsData.GreatLaunch)
        {
            PrizeProgression.AddProgress(PrizeProgressionType.PerfectStarts, 1f);
            PrizeProgression.AddProgress(PrizeProgressionType.Perfection, 1f);
        }
        if (raceResultsData2 != null)
        {
            float num = raceResultsData2.RaceTime - raceResultsData.RaceTime;
            if (num > 0f)
            {
                PrizeProgression.AddProgress(PrizeProgressionType.TotalLeadTime, num);
            }
        }
	}
    
    private static void LogCompleteRaceEvents(Dictionary<Parameters, string> dictionary, GameObject go, int eventID)
    {
        if(!dictionary.ContainsKey(Parameters.IsWinner) || dictionary[Parameters.IsWinner] != "True")
            return;

        //yield return null;
        //Log.AnEvent(Events.CompleteRace, false);
        //yield return null;
        if (IngameTutorial.IsIn1stTutorialRace)
        {
            Log.AnEvent(Events.Complete1stTutRace, dictionary, false);
        }
        else if (IngameTutorial.IsIn2ndTutorialRace)
        {
            Log.AnEvent(Events.Complete2ndTutRace, dictionary, false);
        }
        else if (IngameTutorial.IsIn3dTutorialRace)
        {
            Log.AnEvent(Events.Complete3thTutRace, dictionary, false);
        }
        else if (eventID==602 && PlayerProfileManager.Instance.ActiveProfile.RacesWon == 0)
        {
            Log.AnEvent(Events.CompleteRegTutRace, dictionary, false);
        }
        else if (eventID==4101 && PlayerProfileManager.Instance.ActiveProfile.RacesWon > 0)
        {
            Log.AnEvent(Events.CompleteCrewTutRace, dictionary, false);
        }
        //return;

        Object.Destroy(go);
        
    }
    
}
