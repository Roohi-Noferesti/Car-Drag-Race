using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkCompetitor : RaceCompetitor
{
	private float speedPerSecond;

	private float mUpdateThrottleStateTimer;

	private bool mPreCountdownThrottleOn;

	private bool hasWarnedOnNoData;

	public bool SlowMoMode
	{
		get;
		set;
	}

    public NetworkReplay PlaybackReplayData
    {
        get;
        set;
    }

	public NetworkCompetitor()
	{
		base.CompetitorType = eRaceCompetitorType.NETWORK_COMPETITOR;
	}

	public override void SetupPlayerInfo()
	{
	}

	public void EnterCloseRaceMode()
	{
		this.SlowMoMode = true;
	}

    public PlayerReplay LoadReplayFromLocalStorage()
    {
        this.PlaybackReplayData = new NetworkReplay();
        PlayerReplay playerReplay = NetworkReplayManager.Instance.LoadReplay();
        this.PlaybackReplayData.SetReplayData(playerReplay.replayData);
        this.PlaybackReplayData.SetPhase(eReplayPhase.GRID_PHASE);
        this.PlaybackReplayData.ReplayMode = eReplayMode.PLAY_MODE;
        base.PlayerInfo = playerReplay.playerInfo;
        return playerReplay;
    }

    public void LoadReplay(PlayerReplay zPlayerReplay)
    {
        this.PlaybackReplayData = new NetworkReplay();
        this.PlaybackReplayData.SetReplayData(zPlayerReplay.replayData);
        this.PlaybackReplayData.SetPhase(eReplayPhase.GRID_PHASE);
        this.PlaybackReplayData.ReplayMode = eReplayMode.PLAY_MODE;
        base.PlayerInfo = zPlayerReplay.playerInfo;
    }

	public override void SetupRaceEvents()
	{
		RaceController instance = RaceController.Instance;
		instance.Events.HandleEvent("RaceReset", new Action(this.RaceReset));
		instance.Events.HandleEvent("RaceCountdownStarted", new Action(this.RaceCountdownStarted));
		instance.Events.HandleEvent("RaceStart", new Action(this.RaceStarted));
		instance.Events.HandleEvent("RaceEnteredCloseRaceSlowMo", new Action(this.EnterCloseRaceMode));
	}

	public override void RemoveRaceEvents()
	{
		RaceController instance = RaceController.Instance;
		instance.Events.StopHandlingEvent("RaceReset", new Action(this.RaceReset));
		instance.Events.StopHandlingEvent("RaceCountdownStarted", new Action(this.RaceCountdownStarted));
		instance.Events.StopHandlingEvent("RaceStart", new Action(this.RaceStarted));
		instance.Events.StopHandlingEvent("RaceEnteredCloseRaceSlowMo", new Action(this.EnterCloseRaceMode));
	}

	public override void SetUpObjectReferences(GameObject gameObject,CarPhysics carPhysics)
	{
        base.SetUpObjectReferences(gameObject, carPhysics);
		base.CarPhysics.IsNetworkPlayer = true;
	}

	public void RaceCountdownStarted()
	{
        this.PlaybackReplayData.Start();
		this.mPreCountdownThrottleOn = false;
		this.mUpdateThrottleStateTimer = 0f;
		this.SlowMoMode = false;
	}

	public void RaceReset()
	{
        this.PlaybackReplayData.Reset();
		this.mPreCountdownThrottleOn = false;
		this.mUpdateThrottleStateTimer = 0f;
		this.SlowMoMode = false;
	}

	public void RaceStarted()
	{
        this.PlaybackReplayData.SetPhase(eReplayPhase.RACE_PHASE);
        CarPhysics.RaceStartedEventHandler();
	}

	public override void UpdateEnterStateFixedUpdate()
	{
		this.UpdatePreCountDownRevving();
	}

	public override void UpdatePreCountDownFixedUpdate()
	{
		this.UpdatePreCountDownRevving();
	}

	private void UpdatePreCountDownRevving()
	{
		DriverInputs driverInputs;
		driverInputs.GearChangeUp = false;
		driverInputs.GearChangeDown = false;
		if (this.mUpdateThrottleStateTimer <= 0f)
		{
			if (!this.mPreCountdownThrottleOn)
			{
				this.mUpdateThrottleStateTimer = 0.4f + Random.value * 0.5f;
				this.mPreCountdownThrottleOn = true;
			}
			else
			{
				this.mUpdateThrottleStateTimer = 0.1f + Random.value * 0.4f;
				this.mPreCountdownThrottleOn = false;
			}
		}
		else
		{
			this.mUpdateThrottleStateTimer -= Time.fixedDeltaTime;
		}
		driverInputs.Throttle = ((!this.mPreCountdownThrottleOn) ? 0f : 1f);
		driverInputs.Nitrous = false;
		base.CarPhysics.DriverInputs = driverInputs;
		base.CarPhysics.RunPreCountdownCarPhysics();
	}

	public override void UpdateGridFixedUpdate()
	{
        if (this.PlaybackReplayData == null)
        {
            if (!this.hasWarnedOnNoData)
            {
                this.hasWarnedOnNoData = true;
            }
            return;
        }
        DriverInputs driverInputs;
        this.PlaybackReplayData.FillCarInputsGrid(out driverInputs);
        base.CarPhysics.DriverInputs = driverInputs;
        base.CarPhysics.RunCarPhysics();
        this.PlaybackReplayData.FixedUpdate();
	}

	public override void UpdateRaceFixedUpdate()
	{
        if (this.SlowMoMode)
        {
            float num = (!RaceEventInfo.Instance.CurrentEvent.IsHalfMile) ? 402.325f : 804.65f;
            float num2 = this.PlaybackReplayData.ReplayData.finishTime - base.CarPhysics.SpeedMileStoneTimer.CurrentTime();
            this.speedPerSecond = (num - base.CarPhysics.DistanceTravelled) / num2;
            base.CarPhysics.InterpolatePosition(this.speedPerSecond);
            return;
        }
        DriverInputs driverInputs;
        this.PlaybackReplayData.FillCarInputsRace(out driverInputs);
        driverInputs.Throttle = 1f;
        base.CarPhysics.DriverInputs = driverInputs;
        base.CarPhysics.RunCarPhysics();
        this.PlaybackReplayData.FixedUpdate();
	}

	public override void UpdatePostRaceFixedUpdate()
	{
		base.CarPhysics.RunCarPhysics();
	}
}
