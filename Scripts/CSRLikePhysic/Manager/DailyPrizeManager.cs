using System;
using UnityEngine;

public class DailyPrizeManager : MonoBehaviour
{
    private bool m_dailyPrizeCheck;
    public static DailyPrizeManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnProfileChanged()
    {
        m_dailyPrizeCheck = false;
        enabled = true;
    }
    void Update()
    {
        if (m_dailyPrizeCheck)
        {
            return;
        }
        if (!ServerSynchronisedTime.Instance.ServerTimeValid)
            return;
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        var now = ServerSynchronisedTime.Instance.GetDateTime();
        var lastPrizeTime = activeProfile.DailyPrizeCardLastEventAt;
        var dailyPrizeTimeSpan = now - lastPrizeTime;
        if ((dailyPrizeTimeSpan.Days > 0 || now.Day > lastPrizeTime.Day) && activeProfile.RacesEntered > 0 && activeProfile.RacesWon > 0)
        {
            activeProfile.NumberOfPrizeCardRemaining = GameDatabase.Instance.OnlineConfiguration.PrizeoMatic.NumOfDailyPrize;
            activeProfile.DailyPrizeCardLastEventAt = ServerSynchronisedTime.Instance.GetDateTime();
        }
        else
        {
            activeProfile.NumberOfPrizeCardRemaining = 0;
        }
        m_dailyPrizeCheck = true;
    }

    public void Shutdown()
    {
        HomeScreen.CheckForDailyPrize = false;
        m_dailyPrizeCheck = false;
        enabled = false;
    }
}
