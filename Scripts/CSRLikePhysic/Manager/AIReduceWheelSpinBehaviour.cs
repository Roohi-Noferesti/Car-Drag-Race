public class AIReduceWheelSpinBehaviour : IAIBehaviour
{
	private AIPlayer mAIPlayer;

	public AIReduceWheelSpinBehaviour(AIPlayer zAIPlayer)
	{
		this.mAIPlayer = zAIPlayer;
	}

	public void Begin()
	{
	}

	public IAIBehaviour Update(out DriverInputs zDriverInputs)
	{
		zDriverInputs.Throttle = 1f;
		zDriverInputs.GearChangeDown = false;
		zDriverInputs.GearChangeUp = false;
		zDriverInputs.Nitrous = false;
		if (this.mAIPlayer.CarPhysics.SpeedMileStoneTimer.CurrentTime() > 0.2f)
		{
			if (this.mAIPlayer.AIBehaviourInputData.LaunchWheelSpin == 0f)
			{
				return new AIRacingBehaviour(this.mAIPlayer);
			}
			float num = this.mAIPlayer.SetupData.FirstGearLimitChangeUpPercent / 100f;
			if (this.mAIPlayer.AIBehaviourInputData.CurrentSpeedMPH > this.mAIPlayer.AIBehaviourInputData.FirstGearTheoreticalMaxSpeed * num)
			{
				return new AIRacingBehaviour(this.mAIPlayer);
			}
		}
		return this;
	}

	public IAIBehaviour ForceNextState()
	{
		return new AIRacingBehaviour(this.mAIPlayer);
	}
}
