using DataSerialization;
using UnityEngine;


public class RemoteConfigCondition : EligibilityConditionBase
{
    protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
    {
        Debug.Log(RemoteConfigManager.Instance.GetValue(details.RemoteConfigKey));
        return RemoteConfigManager.Instance.GetValue(details.RemoteConfigKey) == details.RemoteConfigValue;
    }
}
