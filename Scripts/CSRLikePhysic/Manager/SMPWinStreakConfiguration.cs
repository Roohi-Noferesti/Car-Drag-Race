using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SMPWinStreakConfiguration
{
    [Serializable]
    public class RestoreCost
    {
        public int[] Costs;
    }

    public string WinStreakTimeout;
    public string WinStreakPNReminder;
    public string WinChallengeCooldown;
    public List<SMPWinStreakReward> SMPWinStreakData;

    public RestoreCost[] RestoreCosts;
}
