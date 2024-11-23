using System;
using UnityEngine;

public class LocalCompetitor : RaceCompetitor
{
	public const float MAX_FINISH_TIME_DELTA = 0.01f;

	private bool waitingToTriggerPostRaceFixedUpdate = true;

	public static bool useSimulationTime = true;

    private bool started;

    private float timeDifferenceCached = 3.40282347E+38f;

    private bool hasDumpedReplay;

	public LocalCompetitor()
	{
		base.CompetitorType = eRaceCompetitorType.LOCAL_COMPETITOR;
        this.RecordableReplayData = new NetworkReplay();
        this.RecordableReplayData.ReplayMode = eReplayMode.RECORD_MODE;
	}

	public override void SetupPlayerInfo()
	{
        base.PlayerInfo = new LocalPlayerInfo();
	}

	public override void SetupRaceEvents()
	{
		RaceController instance = RaceController.Instance;
        instance.Events.HandleEvent("RaceReset", new Action(this.RaceReset));
        instance.Events.HandleEvent("RaceCountdownStarted", new Action(this.RaceCountdownStarted));
        instance.Events.HandleEvent("RaceStart", new Action(this.RaceStarted));
	}

	public override void RemoveRaceEvents()
	{
		RaceController instance = RaceController.Instance;
        instance.Events.StopHandlingEvent("RaceReset", new Action(this.RaceReset));
        instance.Events.StopHandlingEvent("RaceCountdownStarted", new Action(this.RaceCountdownStarted));
        instance.Events.StopHandlingEvent("RaceStart", new Action(this.RaceStarted));
	}

	public void RaceCountdownStarted()
	{
        this.RecordableReplayData.Start();
	}

	public void RaceStarted()
	{
        this.RecordableReplayData.SetPhase(eReplayPhase.RACE_PHASE);
        this.RecordableReplayData.Start();
        CarPhysics.RaceStartedEventHandler();
	}

	public override void UpdateGridFixedUpdate()
	{
        InputManager inputs = RaceController.Instance.Inputs;
        ShiftPaddleAnimator shiftPaddleAnims = RaceHUDController.Instance.HUDAnimator.ShiftPaddleAnims;
		DriverInputs driverInputs;
	    driverInputs.GearChangeUp = (inputs.InputState.GearChangeUp) && shiftPaddleAnims.IsGearUpActive;
	    driverInputs.GearChangeDown = (inputs.InputState.GearChangeDown && shiftPaddleAnims.IsGearDownActive);
		driverInputs.Throttle = (!inputs.InputState.Throttle) ? 0f : 1f;
		driverInputs.Nitrous = false;
		base.CarPhysics.DriverInputs = driverInputs;
		base.CarPhysics.RunCarPhysics();
        this.RecordableReplayData.AddGridFrameData(base.CarPhysics.DriverInputs);
        this.RecordableReplayData.FixedUpdate();
	}

	private bool CanStartEvent()
	{
        if (this.started)
        {
            return true;
        }
        if (!RelayManager.IsCurrentEventRelay() && !RaceEventInfo.Instance.CurrentEvent.AutoHeadstart)
        {
            this.started = true;
            return this.started;
        }
        float num = CompetitorManager.Instance.OtherCompetitor.CarPhysics.SpeedMileStoneTimer.CurrentTime();
        if (this.timeDifferenceCached == 3.40282347E+38f)
        {
            this.timeDifferenceCached = RaceEventInfo.Instance.CurrentEvent.GetTimeDifference();
        }
        float num2 = this.timeDifferenceCached;
        if (num >= -num2)
        {
            this.started = true;
        }
        return this.started;
	}

	public override void UpdateRaceFixedUpdate()
	{
		InputManager inputs = RaceController.Instance.Inputs;
        ShiftPaddleAnimator shiftPaddleAnims = RaceHUDController.Instance.HUDAnimator.ShiftPaddleAnims;
		DriverInputs driverInputs;
		driverInputs.GearChangeUp = (inputs.InputState.GearChangeUp && shiftPaddleAnims.IsGearUpActive && this.CanStartEvent());
		if (base.CarPhysics.GearBox.CurrentGear == 0 && this.CanStartEvent())
		{
			driverInputs.GearChangeUp = true;
		}
		if (driverInputs.GearChangeUp)
		{
		}
		driverInputs.GearChangeDown = (inputs.InputState.GearChangeDown);// && shiftPaddleAnims.IsGearDownActive);
		driverInputs.Throttle = 0f;
		driverInputs.Nitrous = inputs.InputState.Nitrous;
		driverInputs.Nitrous &= this.CanStartEvent();
		if (base.CarPhysics.GearBox.CurrentGear > 0)
		{
			driverInputs.Throttle = 1f;
		}
		else
		{
			driverInputs.Throttle = ((!inputs.InputState.Throttle) ? 0f : 1f);
		}
        //LogUtility.Log(driverInputs.Throttle);
		base.CarPhysics.DriverInputs = driverInputs;
		base.CarPhysics.RunCarPhysics();
        this.RecordableReplayData.AddRaceFrameData(base.CarPhysics.DriverInputs);
        this.RecordableReplayData.FixedUpdate();
	}

	public void RaceReset()
	{
		this.waitingToTriggerPostRaceFixedUpdate = true;
        this.hasDumpedReplay = false;
        this.started = false;
        this.RecordableReplayData.Reset();
        this.RecordableReplayData.ClearReplayData();
	}

	public override void UpdatePostRaceFixedUpdate()
	{
		base.CarPhysics.RunCarPhysics();
		if (this.waitingToTriggerPostRaceFixedUpdate)
		{
			this.OnEnteredPostRace();
			this.waitingToTriggerPostRaceFixedUpdate = false;
		}
		if (Input.GetKey(KeyCode.S))
		{
			this.DumpReplayRecord();
		}
	}

	public void DumpReplayRecord()
	{
        if (!this.hasDumpedReplay && !MultiplayerUtils.DisableMultiplayer)
        {
            this.RecordReplayFinishTime();
            this.hasDumpedReplay = true;
            PlayerReplay playerReplay = new PlayerReplay(base.PlayerInfo, this.RecordableReplayData.ReplayData);
            string fileName = string.Concat(new object[]
            {
                "Replay-",
                base.PlayerInfo.GetComponent<RacePlayerInfoComponent>().CarDBKey,
                "-",
                this.RecordableReplayData.ReplayData.finishTime,
                ".txt"
            });
            NetworkReplayManager.Instance.SaveReplay(playerReplay, fileName);
        }
	}

	protected void RecordReplayFinishTime()
	{
        if (RaceEventInfo.Instance.CurrentEvent.IsHalfMile)
        {
            this.RecordableReplayData.ReplayData.finishTime = base.CarPhysics.SpeedMileStoneTimer.mHalfMileTime;
            this.RecordableReplayData.ReplayData.finishSpeed = base.CarPhysics.SpeedMileStoneTimer.SpeedAtHalf * 0.44722718f;
        }
        else
        {
            this.RecordableReplayData.ReplayData.finishTime = base.CarPhysics.SpeedMileStoneTimer.mQuarterMileTime;
            this.RecordableReplayData.ReplayData.finishSpeed = base.CarPhysics.SpeedMileStoneTimer.SpeedAtQuarter * 0.44722718f;
        }
	}

	protected virtual void OnEnteredPostRace()
	{
		try
		{
			if (RaceEventInfo.Instance.CurrentEvent.IsTutorial() || RaceEventInfo.Instance.CurrentEvent.IsTestDrive())
				return;
			
			this.RecordReplayFinishTime();
			//NetworkCompetitor networkCompetitor = CompetitorManager.Instance.OtherCompetitor as NetworkCompetitor;
			//NetworkReplay playbackReplayData = networkCompetitor.PlaybackReplayData;
			PlayerReplay zOpponentPlayerReplay = null;// new PlayerReplay(networkCompetitor.PlayerInfo, playbackReplayData.ReplayData);
			PlayerReplay playerReplay = new PlayerReplay(base.PlayerInfo, this.RecordableReplayData.ReplayData);
			playerReplay.replayData.replayVersion = GameDatabase.Instance.OnlineConfiguration.NetworkReplayVersion;
			NetworkReplayManager.Instance.AddReplayToUploadQueue(playerReplay, zOpponentPlayerReplay, RaceEventInfo.Instance.CurrentEvent, ReplayType.RaceTheWorld);
			//DifficultyManager.OnMultiplayerFinishedRace(RaceResultsTracker.You.RaceTime, RaceResultsTracker.You.IsWinner);
		} catch {
			Debug.Log("Failed to Record Replay Data.");
		}
		
	}
}
