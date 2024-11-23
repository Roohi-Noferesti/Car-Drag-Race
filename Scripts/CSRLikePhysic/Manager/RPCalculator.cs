using System;
using UnityEngine;

public class RPCalculator
{
	private static RPSettings settings = new RPSettings();

	public static void CalculateRP(float deltaT, int playerPP, int currentRP, bool isEliteRace, bool isEventRace, bool isBlogger, float eventMultiplier, out int raceBonus, out int leadBonus, out int eliteBonus, out int worldTourBonus, out int deltaRP)
	{
		bool flag = deltaT >= 0f;
		if (flag)
		{
			deltaT = Mathf.Clamp01(deltaT) * RPCalculator.settings.K;
			float num = (float)playerPP / RPCalculator.settings.MaxPP;
			float num2 = 1f - (float)currentRP / (RPCalculator.settings.AverageRP * Mathf.Pow((float)playerPP, RPCalculator.settings.N));
			float num3 = num * deltaT * RPBonusManager.GetOverallMultiplier();
			float num4 = num * RPCalculator.settings.RPBase * num2 * RPBonusManager.GetOverallMultiplier();
			float num5 = (!isEliteRace) ? 0f : ((StreakManager.StreakData.EliteClubInfo.RPMultiplier - 1f) * (num3 + num4));
			float num6 = (!isEventRace) ? 0f : ((eventMultiplier - 1f) * (num3 + num4));
			float num7 = num3 + num4 + num5 + num6;
			float num8 = RPCalculator.settings.MinWin;
			if (isEventRace)
			{
				num8 *= eventMultiplier;
			}
			else if (isEliteRace)
			{
				num8 *= 2f;
			}
			num8 *= RPBonusManager.GetOverallMultiplier();
			if (num7 < num8)
			{
				if (isEventRace)
				{
					num6 = RPCalculator.BackCalculateFromDelta(eventMultiplier, num8);
					num5 = 0f;
					num3 = 0.5f * (num8 - num6);
					num4 = 0.5f * (num8 - num6);
				}
				else if (isEliteRace)
				{
					num6 = 0f;
					num5 = RPCalculator.BackCalculateFromDelta(StreakManager.StreakData.EliteClubInfo.RPMultiplier, num8);
					num4 = 0.5f * (num8 - num5);
					num3 = 0.5f * (num8 - num5);
				}
				else
				{
					num4 = num8 / (float)RPCalculator.settings.raceBonusSplit;
					num3 = num8 / (float)RPCalculator.settings.leadBonusSplit;
				}
				num7 = num8;
			}
			raceBonus = Mathf.RoundToInt(num4);
			leadBonus = Mathf.RoundToInt(num3);
			eliteBonus = Mathf.RoundToInt(num5);
			worldTourBonus = Mathf.RoundToInt(num6);
			deltaRP = Mathf.RoundToInt(num7);
		}
		else
		{
			int num9 = 0;
			if (!isBlogger)
			{
				num9 = Mathf.RoundToInt(-((float)playerPP / 800f) * RPCalculator.settings.RPBase * ((float)currentRP / (RPCalculator.settings.AverageRP * Mathf.Pow((float)playerPP, RPCalculator.settings.N))) * 2f);
				if ((float)num9 > RPCalculator.settings.MinLose)
				{
					num9 = (int)RPCalculator.settings.MinLose;
				}
			}
			raceBonus = 0;
			eliteBonus = 0;
			leadBonus = 0;
			worldTourBonus = 0;
			deltaRP = num9;
		}
	}

	private static float BackCalculateFromDelta(float bonusMultiplier, float rpDelta)
	{
		return rpDelta * (1f - 1f / bonusMultiplier);
	}
}
