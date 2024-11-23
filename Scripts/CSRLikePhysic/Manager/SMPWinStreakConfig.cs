using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SMPWinStreakConfig
{
	private int[][] m_WinStreakRestoreCosts = new int[][]
	{
		new int[1]
	};

	public TimeSpan WinStreakTimeout
	{
		get;
		private set;
	}

	public TimeSpan WinStreakPNReminder
	{
		get;
		private set;
	}

	public TimeSpan WinChallengeCooldown
	{
		get;
		private set;
	}

	public List<SMPWinStreakReward> SMPWinStreakData
	{
		get;
		private set;
	}

	public int SMPWinStreakMaxValue
	{
		get;
		private set;
	}

	public bool IsConfigDataValid
	{
		get;
		private set;
	}

    public void SetConfig(SMPWinStreakConfiguration configuration)
    {
        if (configuration == null)
        {
            return;
        }
        if (!string.IsNullOrEmpty(configuration.WinStreakTimeout))
        {
            this.WinStreakTimeout = TimeSpan.Parse(configuration.WinStreakTimeout);
            this.WinStreakPNReminder = TimeSpan.Parse(configuration.WinStreakPNReminder);
            this.WinChallengeCooldown = TimeSpan.Parse(configuration.WinChallengeCooldown);
        }
        this.SMPWinStreakMaxValue = -1;
        this.SMPWinStreakData = configuration.SMPWinStreakData;
        if (SMPWinStreakData != null)
        {
            foreach (var sMPWinStreakReward in SMPWinStreakData)
            {
                this.SMPWinStreakMaxValue = Mathf.Max(this.SMPWinStreakMaxValue, sMPWinStreakReward.StreakCount);
            }
        }
        var restoreCosts = configuration.RestoreCosts;
        if (restoreCosts != null)
        {
            int count = restoreCosts.Length;
            this.m_WinStreakRestoreCosts = new int[count][];
            for (int i = 0; i < count; i++)
            {
                this.m_WinStreakRestoreCosts[i] = new int[restoreCosts[i].Costs.Length];
                for (int j = 0; j < m_WinStreakRestoreCosts[i].Length; j++)
                {
                    this.m_WinStreakRestoreCosts[i][j] = restoreCosts[i].Costs[j];
                }
            }
        }
        else
        {
            this.m_WinStreakRestoreCosts = new int[0][];
        }
        this.IsConfigDataValid = true;
    }
}
