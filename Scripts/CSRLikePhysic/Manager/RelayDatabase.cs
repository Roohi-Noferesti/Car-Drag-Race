using UnityEngine;

public class RelayDatabase : ConfigurationAssetLoader
{
	private const int DEFAULT_GOLD_TO_FILL_TANK = 5;

	public RelayConfiguration Configuration
	{
		get;
		private set;
	}

	public RelayDatabase() : base(GTAssetTypes.configuration_file, "RelayConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (RelayConfiguration) scriptableObject;//JsonConverter.DeserializeObject<RelayConfiguration>(assetDataString);
        //this.Configuration.ChallengingGrindRequirements.Initialise();
        //this.Configuration.HardGrindRequirements.Initialise();
	}

	public int GetRelayBadRunPPDifference()
	{
		return this.Configuration.RelayLegBadRunPPDifference;
	}

	public int GetMechanicPPBoost()
	{
		return this.Configuration.MechanicPPBoost;
	}

	public int GetGrindReward()
	{
		int raceCount = RelayManager.GetRaceCount();
		if (raceCount == 3)
		{
			return this.Configuration.EasyGrindReward;
		}
		if (raceCount != 4)
		{
			return this.Configuration.HardGrindReward;
		}
		return this.Configuration.ChallengingGrindReward;
	}

	public float GetTimeDifference(int numberOfEvents)
	{
		float num;
		if (numberOfEvents != 3)
		{
			if (numberOfEvents != 4)
			{
				num = this.Configuration.HardTimeDifference;
			}
			else
			{
				num = this.Configuration.ChallengingTimeDifference;
			}
		}
		else
		{
			num = this.Configuration.EasyTimeDifference;
		}
		return num * (float)numberOfEvents;
	}

	public bool IsRookieAvailable()
	{
	    //return this.Configuration.ChallengingGrindRequirements.IsEligible(new GameStateFacade());
	    return false;
	}

    public bool IsProAvailable()
    {
        //return this.Configuration.HardGrindRequirements.IsEligible(new GameStateFacade());
        return false;
    }
}
