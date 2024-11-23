using System;

public abstract class FlowConditionBase
{
	public ConditionState state = ConditionState.UNKNOWN;

	public abstract void EvaluateHardcodedConditions();
	public abstract bool IsConditionActive();

	public abstract PopUp GetPopup();

	public abstract bool HasBubbleMessage();

	public abstract string GetBubbleMessage();

	public void Evaluate(IGameState gameState)
	{
		string name = base.GetType().Name;
		if (GameDatabase.Instance.FlowConditionsConfiguration.IsFlowConditionValid(name, gameState))
		{
			this.EvaluateHardcodedConditions();
		}
		else
		{
			this.state = ConditionState.NOT_VALID;
		}
	}

	public static bool IsTooFrequent(DateTime lastOccurrence, int frequencyConstraint)
	{
        return (int)GTDateTime.Now.Subtract(lastOccurrence).TotalMinutes <= frequencyConstraint;
	}
}
