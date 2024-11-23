using System;

[Serializable]
public class RaceEventDatabaseData
{
    public Tier1 Tier1;

    public Tier2 Tier2;

    public Tier3 Tier3;

	public Tier4 Tier4;

	public Tier5 Tier5;

	public TierX TierX;

	public RaceEventData Tutorial;

    public RaceEventData Tutorial2;

    public RaceEventData Tutorial3;

	public RaceTheWorldEvents RaceTheWorldEvents = new RaceTheWorldEvents();

	public RaceTheWorldWorldTourEvents RaceTheWorldWorldTourEvents = new RaceTheWorldWorldTourEvents();

	public OnlineClubRacingEvents OnlineClubRacingEvents = new OnlineClubRacingEvents();

	public FriendRaceEvents FriendRaceEvents = new FriendRaceEvents();

    public SMPRaceEvents SMPRaceEvents = new SMPRaceEvents();

	public void ProcessEvents()
	{
		this.Tier4.ProcessEvents();
		this.Tier3.ProcessEvents();
		this.Tier2.ProcessEvents();
		this.Tier1.ProcessEvents();
        this.Tier5.ProcessEvents();
		this.TierX.ProcessEvents();
		this.RaceTheWorldEvents.ProcessEvents(this.Tier1);
		this.RaceTheWorldWorldTourEvents.ProcessEvents(this.Tier1);
		this.OnlineClubRacingEvents.ProcessEvents(this.Tier1);
		this.FriendRaceEvents.ProcessEvents(this.Tier1);
        this.SMPRaceEvents.ProcessEvents(this.Tier1);
	}

	public bool IsAnUnlockEvent(int zEventID)
	{
		return this.Tier4.UnlockEventID == zEventID || this.Tier3.UnlockEventID == zEventID || this.Tier2.UnlockEventID == zEventID || this.Tier5.UnlockEventID == zEventID || this.TierX.UnlockEventID == zEventID;
	}

	public BaseCarTierEvents GetTierEvents(eCarTier zCarTier)
	{
		switch (zCarTier)
		{
		case eCarTier.TIER_1:
			return this.Tier1;
		case eCarTier.TIER_2:
			return this.Tier2;
		case eCarTier.TIER_3:
			return this.Tier3;
		case eCarTier.TIER_4:
			return this.Tier4;
		case eCarTier.TIER_5:
			return this.Tier5;
		case eCarTier.TIER_X:
			return this.TierX;
		default:
			return null;
		}
	}

	public override string ToString()
	{
		string text = string.Empty;
		if (this.Tier4 == null)
		{
			text += "No Car Tier 4 events found : \n";
		}
		else
		{
			text += this.Tier4.ToString();
		}
		if (this.Tier3 == null)
		{
			text += "No Car Tier 3 events found : \n";
		}
		else
		{
			text += this.Tier3.ToString();
		}
		if (this.Tier2 == null)
		{
			text += "No Car Tier 2 events found : \n";
		}
		else
		{
			text += this.Tier2.ToString();
		}
		if (this.Tier1 == null)
		{
			text += "No Car Tier 1 events found : \n";
		}
		else
		{
			text += this.Tier1.ToString();
		}
		if (this.Tier5 == null)
		{
			text += "No Car Tier 5 events found : \n";
		}
		else
		{
			text += this.Tier5.ToString();
		}
		if (this.TierX == null)
		{
			text += "No Car Tier X events found : \n";
		}
		else
		{
			text += this.TierX.ToString();
		}
		return text;
	}
}
