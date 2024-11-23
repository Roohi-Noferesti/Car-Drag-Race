public class AIStopBehaviour : IAIBehaviour
{
	public AIStopBehaviour(AIPlayer zAIPlayer)
	{
	}

	public void Begin()
	{
	}

	public IAIBehaviour Update(out DriverInputs zDriverInputs)
	{
		zDriverInputs.Throttle = 0f;
		zDriverInputs.GearChangeDown = true;
		zDriverInputs.GearChangeUp = false;
		zDriverInputs.Nitrous = false;
		return this;
	}

	public IAIBehaviour ForceNextState()
	{
		return this;
	}
}
