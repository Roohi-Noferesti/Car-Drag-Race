using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class RaceEventTopLevelCategory
{
    //public UIPinPosition UIPosition = new UIPinPosition(0, 0);

	public List<RaceEventGroup> RaceEventGroups = new List<RaceEventGroup>();

    [NonSerialized]
	private BaseCarTierEvents carTier;


	public BaseCarTierEvents GetTierEvents()
	{
	    if (carTier == null)
	    {
	        carTier = new Tier1();
	    }
		return this.carTier;
	}

	public int NumOfEvents()
	{
		int num = 0;
		foreach (RaceEventGroup current in this.RaceEventGroups)
		{
			num += current.NumOfEvents();
		}
		return num;
	}

	public int NumEventsComplete()
	{
		int num = 0;
		foreach (RaceEventGroup current in this.RaceEventGroups)
		{
			num += current.NumEventsComplete();
		}
		return num;
	}

	public void ProcessEvents(BaseCarTierEvents zCarTier)
	{
		this.carTier = zCarTier;
		foreach (RaceEventGroup current in this.RaceEventGroups)
		{
			current.ProcessEvents(this);
		}
	}

    public override string ToString()
    {
        //string text = string.Empty;
        //foreach (RaceEventGroup current in this.RaceEventGroups)
        //{
        //    text += current.ToString();
        //}
        //return text;
        return String.Empty;
    }

	public virtual string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return "Backgrounds/Blue_Star";
	}

	public virtual string GetOverlayTextureName(RaceEventData zEvent)
	{
		return "Restrictions/Pin_battle";
	}

    public virtual Color GetBackgroundColor(RaceEventData zEvent)
    {
        return Color.white;
    }

	public virtual string GetBossTextureName(RaceEventData zEvent)
	{
		return null;
	}

	public abstract string GetProgressBarText(RaceEventData zEvent);

	public abstract Vector2 GetPinPosition();

	public virtual Vector2 GetOverlayOffset()
	{
		return new Vector2(0f, 0f);
	}

	public abstract string GetPinString(RaceEventData zEvent);

	public virtual RaceEventTypeMultipliers GetRaceEventTypeRewardMultipliers(RewardsMultipliers rewardsMultipliers, RaceEventData raceEvent)
	{
		return rewardsMultipliers.RegulationRaceMultipliers;
	}

    public virtual string GetName()
    {
        return String.Empty;
    }
}
