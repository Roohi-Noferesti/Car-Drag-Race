using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIRevBehaviour : IAIBehaviour
{
	private AIPlayer mAIPlayer;

	private bool mRevvingUp;

	private float mEagerNextTime;

	private bool mCountdownStarted;

	private float mGridPreSeekTime;

	private bool mSeekRevRange;

	public AIRevBehaviour(AIPlayer zAIPlayer)
	{
		this.mAIPlayer = zAIPlayer;
		RaceController instance = RaceController.Instance;
		if (instance != null)
		{
			instance.Events.HandleEvent("RaceCountdownStarted", new Action(this.RaceCountdownStarted));
		}
	}

	public void Begin()
	{
		this.mRevvingUp = true;
		this.mCountdownStarted = false;
		this.mSeekRevRange = false;
		this.mEagerNextTime = 0f;
		this.mGridPreSeekTime = 0f;
	}

	public IAIBehaviour Update(out DriverInputs zDriverInputs)
	{
		zDriverInputs.Throttle = 0f;
		zDriverInputs.GearChangeDown = false;
		zDriverInputs.GearChangeUp = false;
		zDriverInputs.Nitrous = false;
		zDriverInputs.Throttle = this.UpdateRevHovering(this.mAIPlayer.SetupData.TargetLaunchRPM);
		return this;
	}

	public float UpdateRevHovering(float zTargetLaunchRPM)
	{
		float result = 0f;
		float num = zTargetLaunchRPM - this.mAIPlayer.AIBehaviourInputData.CurrentEngineRPM;
		if (this.mRevvingUp)
		{
			result = 1f;
		}
		if (this.mAIPlayer.IsTightLoopRun)
		{
			this.mSeekRevRange = true;
		}
		if (this.mCountdownStarted && !this.mSeekRevRange)
		{
			if (this.mGridPreSeekTime > 0f)
			{
				this.mGridPreSeekTime -= Time.deltaTime;
			}
			else
			{
				this.mSeekRevRange = true;
			}
		}
		if (this.mSeekRevRange)
		{
			if (num > this.mAIPlayer.SetupData.LaunchRPMVariation && !this.mRevvingUp)
			{
				result = 1f;
				this.mRevvingUp = true;
			}
			else if (num < -this.mAIPlayer.SetupData.LaunchRPMVariation && this.mRevvingUp)
			{
				result = 0f;
				this.mRevvingUp = false;
			}
		}
		else
		{
			if (this.mEagerNextTime <= 0f)
			{
				if (this.mRevvingUp)
				{
					this.mEagerNextTime = 0.3f + Random.value * 0.8f;
					this.mRevvingUp = false;
				}
				else
				{
					this.mEagerNextTime = 0.1f + Random.value * 0.4f;
					this.mRevvingUp = true;
				}
			}
			else
			{
				this.mEagerNextTime -= Time.deltaTime;
			}
			result = ((!this.mRevvingUp) ? 0f : 1f);
		}
		return result;
	}

	public IAIBehaviour ForceNextState()
	{
		return new AIReduceWheelSpinBehaviour(this.mAIPlayer);
	}

	public void RaceCountdownStarted()
	{
		this.mCountdownStarted = true;
		this.mGridPreSeekTime = 2.7f;
		this.mRevvingUp = false;
		this.mEagerNextTime = 0f;
	}
}
