using DataSerialization;

public class AreAnimationsCompletedCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
        string iD = TierXManager.Instance.ThemeDescriptor.ID;
        int num = 0;
        foreach (string current in details.StringValues)
        {
            if (PlayerProfileManager.Instance.ActiveProfile.IsAnimationCompletedForWorldTourEventID(iD, current))
            {
                num++;
            }
        }
        return base.IsInRange(num, details);
    }
}
