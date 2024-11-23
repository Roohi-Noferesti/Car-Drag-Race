using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteConfigABTest
{
    public static bool CheckRemoteConfigValue(string key = "CarBonuses", string value ="true")
    {
        if(RemoteConfigManager.Instance != null)
            return RemoteConfigManager.Instance.GetValue(key).ToLower() == value;
        return true;
    }
}
