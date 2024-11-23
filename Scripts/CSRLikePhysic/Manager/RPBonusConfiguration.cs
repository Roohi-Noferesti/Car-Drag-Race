using System;
using System.Collections.Generic;

[Serializable]
public class RPBonusConfiguration:UnityEngine.ScriptableObject
{
	public static string DateFormat = "MM/dd/yyyy HH:mm:ss";

	public List<RPMultiplierBonus> RPBonuses;

	public List<RPReward> RPRewards;

	public StreakChainRP StreakChainRP;

	public float BonusLimit;

	public float VideoAdMultiplier;

	public TimeSpan VideoAdMultiplierDuration = TimeSpan.Zero;

	public float GetStreakChainRPMultiplier(int chainLength)
	{
		float num = 0f;
		if (this.StreakChainRP != null)
		{
			num = this.StreakChainRP.GetMultiplier(chainLength);
		}
		if (num > this.BonusLimit)
		{
			num = this.BonusLimit;
		}
		return num;
	}

	public TimeSpan GetStreakChainRPDuration()
	{
		if (this.StreakChainRP != null)
		{
			return this.StreakChainRP.MultiplierDuration;
		}
		return TimeSpan.Zero;
	}

	public bool HasStreakChainData()
	{
		return this.StreakChainRP != null && this.StreakChainRP.ChainMultiplers != null && this.StreakChainRP.ChainMultiplers.Count >= 1;
	}
}
