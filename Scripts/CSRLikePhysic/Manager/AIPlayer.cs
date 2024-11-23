using System;
using UnityEngine;

[AddComponentMenu("GT/AI/AIPlayer")]
public class AIPlayer : MonoBehaviour
{
	private float startTimer;

	private bool signalRaceStart;

	private DriverInputs mDriverInputs;

	private bool isTightLoopRun;

	private CarPhysics mCarPhysics;

	private AIBehaviourInputData mAIBehaviourInputData;

	private IAIBehaviour mBehaviour;

	//private bool started;

	//private float timeDifferenceCached = 3.40282347E+38f;

	public float FireMyNitrousAtThisMPH
	{
		get;
		set;
	}

	public DriverInputs DriverInputs
	{
		get
		{
			return this.mDriverInputs;
		}
	}

	public AIDriverData SetupData
	{
		get;
		set;
	}

	public CarPhysics CarPhysics
	{
		get
		{
			return this.mCarPhysics;
		}
		set
		{
			this.mCarPhysics = value;
		}
	}

	public bool IsTightLoopRun
	{
		get
		{
			return this.isTightLoopRun;
		}
		set
		{
			this.isTightLoopRun = value;
		}
	}

	public AIBehaviourInputData AIBehaviourInputData
	{
		get
		{
			return this.mAIBehaviourInputData;
		}
	}

	private void Start()
	{
		RaceController raceController = FindObjectOfType(typeof(RaceController)) as RaceController;
		if (raceController != null)
		{
			raceController.Events.HandleEvent("RaceStart", new Action(this.RaceStartedEventHandler));
			raceController.Events.HandleEvent("RaceEnd", new Action(this.RaceEndedEventHandler));
			raceController.Events.HandleEvent("RaceReset", new Action(this.RaceResetEventHandler));
		}
	    this.mCarPhysics = CompetitorManager.Instance.OtherCompetitor.CarPhysics;// base.gameObject.GetComponent<CarPhysics>();
		this.FireMyNitrousAtThisMPH = 60f;
		this.mAIBehaviourInputData.EngineRPMForNoWheelSpin = this.mCarPhysics.Engine.MaxEngineRPMWithNoWheelSpin(0.01f);
		this.Reset();
	}

	public void Reset()
	{
		this.signalRaceStart = false;
		//this.started = false;
		this.startTimer = 0f;
		this.FireMyNitrousAtThisMPH = 60f;
		this.mBehaviour = new AIRevBehaviour(this);
		this.mBehaviour.Begin();
	}

	public void DoUpdate()
	{
		if (this.mCarPhysics == null)
		{
			return;
		}
		this.mDriverInputs.Reset();
		this.mAIBehaviourInputData.CurrentEngineRPM = this.mCarPhysics.Engine.CurrentRPM;
		this.mAIBehaviourInputData.CurrentWheelSpin = this.mCarPhysics.Wheels.WheelSpin;
		this.mAIBehaviourInputData.CurrentGear = this.mCarPhysics.GearBox.CurrentGear;
		this.mAIBehaviourInputData.LaunchWheelSpin = this.mCarPhysics.Engine.LaunchWheelSpin;
		this.mAIBehaviourInputData.CurrentSpeedMPH = this.mCarPhysics.SpeedMPH;
		this.mAIBehaviourInputData.EngineRedLineRPM = this.mCarPhysics.RedLineRPM;
		this.mAIBehaviourInputData.HasNitrousAvailable = !this.mCarPhysics.HasUsedNitrous;
		this.mAIBehaviourInputData.FirstGearTheoreticalMaxSpeed = this.mCarPhysics.TheoreticalMaxSpeedForThisGear * 2.236f;
		bool useTrueRedLineRPMForCalculation = false;
		this.mAIBehaviourInputData.PeakPowerRPM = this.mCarPhysics.Engine.PeakPowerRPM(0.01f, useTrueRedLineRPMForCalculation);
		if (!this.mCarPhysics.GearBox.IsInNeutral)
		{
			this.mAIBehaviourInputData.OptimalGearChangeMPH = this.mCarPhysics.GearBox.CalculatedOptimalGearChangeMPHArray()[this.mCarPhysics.GearBox.CurrentGear - 1];
		}
		IAIBehaviour iAIBehaviour = this.mBehaviour.Update(out this.mDriverInputs);
		if (iAIBehaviour != this.mBehaviour)
		{
			this.mBehaviour = null;
			iAIBehaviour.Begin();
			this.mBehaviour = iAIBehaviour;
		}
		if (this.signalRaceStart && this.CanStartEvent())
		{
			this.startTimer += Time.fixedDeltaTime;
			this.mDriverInputs.GearChangeUp = true;
			this.signalRaceStart = false;
			this.mDriverInputs.Throttle = 1f;
			this.mBehaviour = new AIReduceWheelSpinBehaviour(this);
		}
	}

	public bool CanStartEvent()
	{
	    return true;
	    //if (this.started)
	    //{
	    //    return true;
	    //}
	    //if (RaceEventInfo.Instance.CurrentEvent == null)
	    //{
	    //    return true;
	    //}
	    //if (!RelayManager.IsCurrentEventRelay() && !RaceEventInfo.Instance.CurrentEvent.AutoHeadstart)
	    //{
	    //    this.started = true;
	    //    return this.started;
	    //}
	    //float num = CompetitorManager.Instance.LocalCompetitor.CarPhysics.SpeedMileStoneTimer.CurrentTime();
	    //if (this.timeDifferenceCached == 3.40282347E+38f)
	    //{
	    //    this.timeDifferenceCached = RaceEventInfo.Instance.CurrentEvent.GetTimeDifference();
	    //}
	    //float num2 = this.timeDifferenceCached;
	    //if (num >= num2)
	    //{
	    //    this.started = true;
	    //}
	    //return this.started;
	}

	public void RaceStartedEventHandler()
	{
		this.signalRaceStart = true;
	}

	private void RaceEndedEventHandler()
	{
		IAIBehaviour iAIBehaviour = this.mBehaviour.ForceNextState();
		this.mBehaviour = iAIBehaviour;
		this.mBehaviour.Begin();
	}

	private void RaceResetEventHandler()
	{
		this.Reset();
	}
}
