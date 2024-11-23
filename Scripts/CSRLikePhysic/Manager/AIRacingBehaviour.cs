using UnityEngine;

public class AIRacingBehaviour : IAIBehaviour
{
	private AIPlayer mAIPlayer;

	public AIRacingBehaviour(AIPlayer zAIPlayer)
	{
		this.mAIPlayer = zAIPlayer;
	}

	public void Begin()
	{
	}

	public IAIBehaviour Update(out DriverInputs zDriverInputs)
	{
		zDriverInputs.Throttle = 0f;
		zDriverInputs.GearChangeDown = false;
		zDriverInputs.GearChangeUp = false;
		zDriverInputs.Nitrous = false;
		bool flag = this.ShallWeChangeGear();
		if (flag)
		{
			zDriverInputs.GearChangeUp = true;
		}
		if (this.mAIPlayer.AIBehaviourInputData.CurrentGear > 0)
		{
			zDriverInputs.Throttle = 1f;
		}
		if (this.mAIPlayer.AIBehaviourInputData.CurrentSpeedMPH >= this.mAIPlayer.FireMyNitrousAtThisMPH && this.mAIPlayer.AIBehaviourInputData.HasNitrousAvailable)
		{
			zDriverInputs.Nitrous = true;
		}
		return this;
	}

	private bool ShallWeChangeGear()
	{
		if (this.mAIPlayer.CarPhysics.GearBox.IsInTopGear)
		{
			return false;
		}
		float num = this.mAIPlayer.AIBehaviourInputData.PeakPowerRPM - this.mAIPlayer.SetupData.RPMFromPeakPowerAtGearChange;
		num = Mathf.Clamp(num, 0f, this.mAIPlayer.AIBehaviourInputData.EngineRedLineRPM - 30f);
		var flag =  this.mAIPlayer.AIBehaviourInputData.CurrentEngineRPM > num;

        //Debug.Log(mAIPlayer.AIBehaviourInputData.PeakPowerRPM + "   " + mAIPlayer.SetupData.RPMFromPeakPowerAtGearChange
        //    + "   " + num + "   " + mAIPlayer.AIBehaviourInputData.CurrentEngineRPM
        //    +"     "+flag);
	    return flag;
	}

	public IAIBehaviour ForceNextState()
	{
		return new AIStopBehaviour(this.mAIPlayer);
	}
}
