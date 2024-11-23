using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

public class RemoteConfigManager : MonoBehaviour
{
    public static RemoteConfigManager Instance;

    
    public string ABTestingId;
    public string ABTestingVariantId;
    public bool IsInitialized = false;

    private Dictionary<string, string> keyValues = new Dictionary<string, string>();
    
    // remote configs
    public string ABTestBranch
    {
        get
        {
            return keyValues["ABTestBranch"];
        }
        set
        {
            string key = "ABTestBranch";
            if(keyValues.ContainsKey(key))
                keyValues[key] = value;
            else
                keyValues.Add(key, value);
        }
    }

    public bool IsOnlineRaceActive
    {
        get
        {
            return keyValues["Online Races"].ToLower()=="true";
        }
        set
        {
            string key = "Online Races";
            if(keyValues.ContainsKey(key))
                keyValues[key] = value.ToString();
            else
                keyValues.Add(key, value.ToString());
        }
    }

    public bool IsCarBonusActive
    {
        get
        {
            return keyValues["CarBonuses"].ToLower() == "true";
        }
        set
        {
            string key = "CarBonuses";
            if (keyValues.ContainsKey(key))
                keyValues[key] = value.ToString();
            else
                keyValues.Add(key, value.ToString());
        }
        
    }

    public float DifficultyLevelMultiplier
    {
        get
        {
            return float.Parse(keyValues["DLMultiplier"]);
        }
        set
        {
            string key = "DLMultiplier";
            if (keyValues.ContainsKey(key))
                keyValues[key] = value.ToString();
            else
                keyValues.Add(key, value.ToString());
        }
    }
    private const int TIME_OUT = 2;

    

    void Awake()
    {
        IsInitialized = false;
        Instance = this;
        Init();
    }

    private void Init()
    {
        if (!BuildType.CanCollectData())
        {
            SetDefaultValues();
            return;
        }


        StartCoroutine(GetRemoteConfigs());
    }
    
    IEnumerator GetRemoteConfigs()
    {
        float timer = 0;
        while (!GameAnalytics.IsRemoteConfigsReady())
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            if (timer > TIME_OUT)
                break;
        }
        
        if(GameAnalytics.IsRemoteConfigsReady ()) {
            ABTestBranch = GameAnalytics.GetRemoteConfigsValueAsString ("ABTestBranch", "Default_Android");
            
            // A/B testing id
            ABTestingId = GameAnalytics.GetABTestingId();

            // A/B testing variant id
            ABTestingVariantId = GameAnalytics.GetABTestingVariantId();
            
            //KEY-VALUES
            IsOnlineRaceActive = GameAnalytics.GetRemoteConfigsValueAsString ("Online Races", "True").ToLower() == "true";
            
            // A/B testing Car Bonuses
            IsCarBonusActive = GameAnalytics.GetRemoteConfigsValueAsString("CarBonuses", "True").ToLower() == "true";
            
            // A/B testing difficulty Level 
            DifficultyLevelMultiplier = float.Parse(GameAnalytics.GetRemoteConfigsValueAsString("DLMultiplier", "1"));
            
            IsInitialized = true;
        } else {
            SetDefaultValues();
            Debug.LogError("RemoteConfigManager is not ready yet.");
        }
    }
    

    public void ReportToFirebase()
    {
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("ABTestBranch", ABTestBranch);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("ABTestingId", ABTestingId);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("ABTestingVariantId", ABTestingVariantId);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("Online Races", IsOnlineRaceActive.ToString());
    }

    public string GetValue(string key)
    {
        return keyValues[key];
    }

    public void Print()
    {
        Debug.Log($"[RemoteConfig] isInitialized is {IsInitialized}");

        Debug.Log($"[RemoteConfig] AB testing id is {ABTestingId}");

        Debug.Log($"[RemoteConfig] AB testing variant id is {ABTestingVariantId}");
        
        foreach (var key in keyValues.Keys)
        {
            Debug.Log($"[RemoteConfig] {key}={keyValues[key]}");
        }
    }

    public void SetDefaultValues()
    {
        ABTestBranch = "Default_Android";
        ABTestingId = "";
        ABTestingVariantId = "";
        IsOnlineRaceActive = true;
        IsCarBonusActive = true;
        DifficultyLevelMultiplier = 1f;
    }
}
