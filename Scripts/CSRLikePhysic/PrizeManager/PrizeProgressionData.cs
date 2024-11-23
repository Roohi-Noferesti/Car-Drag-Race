using System;
using System.Collections.Generic;

[Serializable]
public class PrizeProgressionData
{
	public string Type = string.Empty;

    private static readonly Dictionary<PrizeProgressionType, PrizeProgression> progressionMapping = new Dictionary
        <PrizeProgressionType, PrizeProgression>
    {
        {
            PrizeProgressionType.RacesWon,
            new PrizeProgressionRacesWon()
        },
        {
            PrizeProgressionType.PerfectStarts,
            new PrizeProgressionPerfectStarts()
        },
        {
            PrizeProgressionType.PerfectShifts,
            new PrizeProgressionPerfectShifts()
        },
        {
            PrizeProgressionType.MilesDriven,
            new PrizeProgressionMilesDriven()
        },
        {
            PrizeProgressionType.GachaCards,
            new PrizeProgressionGachaCards()
        },
        {
            PrizeProgressionType.RacesCompleted,
            new PrizeProgressionRacesCompleted()
        },
        {
            PrizeProgressionType.CashWon,
            new PrizeProgressionCashWon()
        },
        {
            PrizeProgressionType.TotalLeadTime,
            new PrizeProgressionTotalLeadTime()
        },
        {
            PrizeProgressionType.StreaksCompleted,
            new PrizeProgressionStreaksCompleted()
        },
        {
            PrizeProgressionType.Perfection,
            new PrizeProgressionPerfection()
        },
        {
            PrizeProgressionType.FuelSpent,
            new PrizeProgressionFuelSpent()
        }
    };

	public PrizeProgressionType TypeEnum
	{
		get
		{
			return EnumHelper.FromString<PrizeProgressionType>(this.Type);
		}
	}

	private static PrizeProgression GetPrizeProgressionInstance(PrizeProgressionType type)
	{
		if (progressionMapping.ContainsKey(type))
		{
			return progressionMapping[type];
		}
		return null;
	}

	public string FormatQuantity(float quantity)
	{
		return GetPrizeProgressionInstance(this.TypeEnum).FormatQuantity(quantity);
	}

	public string GetDescription()
	{
		return GetPrizeProgressionInstance(this.TypeEnum).GetDescription();
	}
}
