using System;

[Serializable]
public class SMPWinStreakReward
{
	public int StreakCount;

	public CSR2Reward WinReward;

	public int Amount;

	public int AchievementIndex = -1;

	public bool IsAchievementReward
	{
		get
		{
			return this.AchievementIndex != -1;
		}
	}
}
