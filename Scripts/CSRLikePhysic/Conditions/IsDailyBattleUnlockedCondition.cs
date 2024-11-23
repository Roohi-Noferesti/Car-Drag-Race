using UnityEngine;
using System.Collections;
using DataSerialization;

public class IsDailyBattleUnlocked : EligibilityConditionBase 
{
    protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
    {
        return gameState.IsDailyBattleUnlocked();
    }
}
