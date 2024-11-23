using DataSerialization;
using UnityEngine;

public class TutorialConfigorationIsOnCondition : EligibilityConditionBase
{
    protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
    {
        return  (GameDatabase.Instance.TutorialConfiguration.IsOn == details.BoolValue);
    }
}

