using UnityEngine;
using System.Collections;
using DataSerialization;

public class PlayerProfileIntegerCondition : EligibilityConditionBase
{
    protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
    {
        return gameState.GetPlayerProfileInteger(details.StringValue) == details.IntValue;
    }
}
