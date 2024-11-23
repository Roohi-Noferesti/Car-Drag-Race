using UnityEngine;
using System.Collections;
using DataSerialization;

public class PlayerProfileIntegerGreaterOrEqualCondition : EligibilityConditionBase
{
    protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
    {
        var integerValue =  gameState.GetPlayerProfileInteger(details.StringValue);
        return integerValue >= details.IntValue;
    }
}
