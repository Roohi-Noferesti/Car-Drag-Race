using System;
using System.Collections.Generic;

public class AwardConsumablePrize : AwardPrizeBase
{
	private eCarConsumables consumableType;

	private Dictionary<eCarConsumables, string> consumableMetricTypes = new Dictionary<eCarConsumables, string>
	{
		{
			eCarConsumables.EngineTune,
			"Race Team Pro Tuner"
		},
		{
			eCarConsumables.Nitrous,
			"Race Team Tire Crew"
		},
		{
			eCarConsumables.Tyre,
			"Race Team N20 Maniac"
		}
	};

	private Dictionary<eCarConsumables, Action> profileRemoval;

	public AwardConsumablePrize(eCarConsumables consumable)
	{
		Dictionary<eCarConsumables, Action> dictionary = new Dictionary<eCarConsumables, Action>();
		dictionary.Add(eCarConsumables.EngineTune, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfProTunerRewardsRemaining--;
		});
		dictionary.Add(eCarConsumables.Nitrous, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfN20ManiacRewardsRemaining--;
		});
		dictionary.Add(eCarConsumables.Tyre, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfTireCrewRewardsRemaining--;
		});
		this.profileRemoval = dictionary;
        //base..ctor();
		this.consumableType = consumable;
	}

	public override void AwardPrize()
	{
		//int consumableExtensionDuration = GameDatabase.Instance.OnlineConfiguration.RaceTeamPrizeData.GetConsumableExtensionDuration(this.consumableType);
        //ConsumablesManager.SetupRaceTeamConsumablePrize(this.consumableType, consumableExtensionDuration);
	}

	public override string GetMetricsTypeString()
	{
		return this.consumableMetricTypes[this.consumableType];
	}

    public override string GetPrizeString()
    {
        throw new NotImplementedException();
    }

    public override void TakePrizeAwayFromProfile()
	{
		this.profileRemoval[this.consumableType]();
	}
}
