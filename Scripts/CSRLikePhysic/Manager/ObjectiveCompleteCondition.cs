using DataSerialization;

public class ObjectiveCompleteCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
    {
        return true;//LegacyObjectivesManager.IsLegacyObjectiveCompleted(details.Objective);
	}
}
