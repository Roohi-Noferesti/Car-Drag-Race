using System;
using System.Collections.Generic;

[Serializable]
public class SpotPrizeData
{
	public string PrizeType = string.Empty;

	public float Milestone = 3.40282347E+38f;

	public SpotPrizeDetails Details = new SpotPrizeDetails();

	private static readonly Dictionary<SpotPrizeType, SpotPrize> prizeMapping = new Dictionary<SpotPrizeType, SpotPrize>
	{
		{
			SpotPrizeType.FreeUpgrade,
			new SpotPrizeFreeUpgrade()
		},
		{
			SpotPrizeType.RPBonus,
			new SpotPrizeRPBonus()
		},
		{
			SpotPrizeType.Livery,
			new SpotPrizeLivery()
		},
		{
			SpotPrizeType.Cash,
			new SpotPrizeCash()
		},
		{
			SpotPrizeType.Gold,
			new SpotPrizeGold()
		},
		{
			SpotPrizeType.ConsumableBlogger,
			new SpotPrizeConsumableBlogger()
		},
		{
			SpotPrizeType.ConsumableTires,
			new SpotPrizeConsumableTires()
		},
		{
			SpotPrizeType.ConsumableNitrous,
			new SpotPrizeConsumableNitrous()
		},
		{
			SpotPrizeType.ConsumableEngine,
			new SpotPrizeConsumableEngine()
		}
	};

	public SpotPrizeType PrizeTypeEnum
	{
		get
		{
			return EnumHelper.FromString<SpotPrizeType>(this.PrizeType);
		}
	}

	public bool PrizeAwarded
	{
		get
		{
			return MultiplayerEvent.Saved.GetSpotPrizeAwarded(this.Details.SpotPrizeID);
		}
	}

	private static SpotPrize GetSpotPrizeInstance(SpotPrizeType type)
	{
		if (SpotPrizeData.prizeMapping.ContainsKey(type))
		{
			return SpotPrizeData.prizeMapping[type];
		}
		return null;
	}

	public string GetPinDescription()
	{
		return SpotPrizeData.GetSpotPrizeInstance(this.PrizeTypeEnum).GetPinDescription(this.Details);
	}

	public void AwardPrize()
	{
		SpotPrizeData.GetSpotPrizeInstance(this.PrizeTypeEnum).AwardPrize(this.Details);
		MultiplayerEvent.Saved.SetSpotPrizeAwarded(this.Details.SpotPrizeID);
	}

	public string GetPopupBody()
	{
		return SpotPrizeData.GetSpotPrizeInstance(this.PrizeTypeEnum).GetPopupBody(this.Details);
	}

	public void Initialise(int eventID, int spotPrizeID)
	{
		this.Details.EventID = eventID;
		this.Details.SpotPrizeID = spotPrizeID;
	}
}
