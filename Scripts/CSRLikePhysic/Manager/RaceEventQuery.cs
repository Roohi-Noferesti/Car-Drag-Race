using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class RaceEventQuery
{
	public static RaceEventQuery Instance
	{
		get;
		private set;
	}

	public static void Create()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = new RaceEventQuery();
	}

	public eCarTier getHighestUnlockedClass()
	{
		if (Instance.IsTierUnlocked(eCarTier.TIER_X))
		{
			return eCarTier.TIER_X;
		}
		if (Instance.IsTierUnlocked(eCarTier.TIER_5))
		{
			return eCarTier.TIER_5;
		}
		if (Instance.IsTierUnlocked(eCarTier.TIER_4))
		{
			return eCarTier.TIER_4;
		}
		if (Instance.IsTierUnlocked(eCarTier.TIER_3))
		{
			return eCarTier.TIER_3;
		}
		if (Instance.IsTierUnlocked(eCarTier.TIER_2))
		{
			return eCarTier.TIER_2;
		}
		return eCarTier.TIER_1;
	}

	public bool IsTierUnlocked(eCarTier zCarTier)
	{
		if (zCarTier == eCarTier.TIER_1)
		{
			return true;
		}
		if (!GameDatabase.Instance.Career.IsValid)
		{
			return false;
		}
		BaseCarTierEvents tierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier);
		List<int> eventsCompleted = PlayerProfileManager.Instance.ActiveProfile.EventsCompleted;
		return eventsCompleted.Contains(tierEvents.UnlockEventID);
	}

	public RaceEventData GetNextEventOnPin(RaceEventData zRaceEventData)
	{
		if (IngameTutorial.IsInTutorial)
		{
			return null;
		}
		RaceEventTopLevelCategory parent = zRaceEventData.Parent;
		if (parent == null)
		{
			return null;
		}
		BaseCarTierEvents tierEvents = zRaceEventData.Parent.GetTierEvents();
		if (tierEvents == null)
		{
			return null;
		}
		if (parent == tierEvents.RegulationRaceEvents || parent == tierEvents.WorldTourRaceEvents)
		{
			return zRaceEventData;
		}
		if (parent == tierEvents.CrewBattleEvents)
		{
			return this.GetCrewBattleEvent(tierEvents, false);
		}
		if (parent == tierEvents.LadderEvents)
		{
			return this.GetLadderEvent(tierEvents, false);
		}
		if (parent == tierEvents.RestrictionEvents)
		{
			return this.GetRestrictionEvent(tierEvents, false);
		}
		if (parent == tierEvents.CarSpecificEvents)
		{
			return this.GetCarSpecificEvent(tierEvents, false);
		}
		if (parent == tierEvents.ManufacturerSpecificEvents)
		{
			return this.GetManufacturerSpecificRaceEvent(tierEvents, false);
		}
		if (parent == tierEvents.DailyBattleEvents)
		{
			return zRaceEventData;
		}
		if (parent is RaceTheWorldEvents)
		{
			return zRaceEventData;
		}
		if (parent is RaceTheWorldWorldTourEvents)
		{
			return zRaceEventData;
		}
		if (parent is OnlineClubRacingEvents)
		{
			return zRaceEventData;
		}
		if (parent is FriendRaceEvents)
		{
			return zRaceEventData;
		}
		return null;
	}

	public RaceEventData GetRandomNextEvent()
	{
		List<RaceEventData> events = this.GetEvents(PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup.BaseCarTier);
		events.RemoveAll(delegate(RaceEventData zData)
		{
			foreach (RaceEventRestriction current in zData.Restrictions)
			{
				if (!current.DoesMeetRestriction(PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup))
				{
					return true;
				}
			}
			return false;
		});
		if (events.Count == 0)
		{
			return null;
		}
		int index = Random.Range(0, events.Count) % events.Count;
		return events[index];
	}

	public List<RaceEventData> GetEvents(eCarTier zClassIndex)
	{
		BaseCarTierEvents baseCarTierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1;
		switch (zClassIndex)
		{
		case eCarTier.TIER_1:
			baseCarTierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1;
			break;
		case eCarTier.TIER_2:
			baseCarTierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier2;
			break;
		case eCarTier.TIER_3:
			baseCarTierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier3;
			break;
		case eCarTier.TIER_4:
			baseCarTierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier4;
			break;
		case eCarTier.TIER_5:
			baseCarTierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier5;
			break;
		case eCarTier.TIER_X:
			baseCarTierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.TierX;
			break;
		}
		List<RaceEventData> list = new List<RaceEventData>();
		list.Add(this.GetCrewBattleEvent(baseCarTierEvents, false));
		list.Add(this.GetRestrictionEvent(baseCarTierEvents, false));
		list.Add(this.GetLadderEvent(baseCarTierEvents, false));
		list.Add(this.GetCarSpecificEvent(baseCarTierEvents, false));
		list.Add(this.GetManufacturerSpecificRaceEvent(baseCarTierEvents, false));
		list.Add(this.GetDailyBattleEvent(baseCarTierEvents, false));
		list.RemoveAll((RaceEventData item) => item == null);
		return list;
	}

	public List<RaceEventGroup> GetEventGroups(eCarTier zClassIndex)
	{
		BaseCarTierEvents tierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zClassIndex);
		List<RaceEventGroup> list = new List<RaceEventGroup>();
		list.Add(this.GetRegulationRaceEvent(tierEvents));
		list.RemoveAll((RaceEventGroup item) => item == null);
		return list;
	}

	public RaceEventGroup GetEventGroupsByGroupID(eCarTier zClassIndex, int zGroupID)
	{
		BaseCarTierEvents tierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zClassIndex);
		List<RaceEventGroup> list = new List<RaceEventGroup>();
		List<RaceEventGroup> worldTourRaceEventGroups = this.GetWorldTourRaceEventGroups(tierEvents);
		list.AddRange(worldTourRaceEventGroups);
		return list.Find((RaceEventGroup item) => item.EventGroupID == zGroupID);
	}

	public RaceEventData GetDailyBattleEvent(BaseCarTierEvents zCarTierEvents, bool zForceFirstRace = false)
	{
		if (zForceFirstRace)
		{
			return zCarTierEvents.DailyBattleEvents.RaceEventGroups[0].RaceEvents[0];
		}
		eCarTier ecarTier = Instance.getHighestUnlockedClass();
		bool flag = false;
		foreach (CarGarageInstance current in PlayerProfileManager.Instance.ActiveProfile.CarsOwned)
		{
			if (current.CurrentTier == ecarTier)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			ecarTier = (eCarTier)Mathf.Max(ecarTier - eCarTier.TIER_2, 0);
		}
		if (ecarTier != zCarTierEvents.GetCarTier())
		{
			return null;
		}
		return zCarTierEvents.DailyBattleEvents.RaceEventGroups[0].RaceEvents[0];
	}

	public RaceEventGroup GetRegulationRaceEvent(BaseCarTierEvents zCarTierEvents)
	{
		return zCarTierEvents.RegulationRaceEvents.RaceEventGroups[0];
	}

	public List<RaceEventGroup> GetRegulationRaceEventGroups(BaseCarTierEvents zCarTierEvents)
	{
		return zCarTierEvents.RegulationRaceEvents.RaceEventGroups;
	}

	public List<RaceEventGroup> GetWorldTourRaceEventGroups(BaseCarTierEvents zCarTierEvents)
	{
		return zCarTierEvents.WorldTourRaceEvents.RaceEventGroups;
	}

	public RaceEventData GetLadderEvent(BaseCarTierEvents zCarTierEvents, bool zForceFirstEvent = false)
	{
		if (zForceFirstEvent)
		{
			return zCarTierEvents.LadderEvents.RaceEventGroups[0].RaceEvents[0];
		}
		RaceEventGroup raceGroup = zCarTierEvents.LadderEvents.RaceEventGroups[0];
		return this.GetNextUnCompletedEventFromGroup(raceGroup);
	}

    public RaceEventData GetWorldTourEvent(RaceEventData eventData, bool zForceFirstEvent = false)
    {
        if (zForceFirstEvent)
        {
            return eventData.Group.RaceEvents[0];
        }
        return this.GetNextUnCompletedEventFromGroup(eventData.Group);
    }

    public RaceEventData GetNextUnCompletedEventFromGroup(RaceEventGroup raceGroup)
	{
        //List<int> eventsCompleted = PlayerProfileManager.Instance.ActiveProfile.EventsCompleted;
	    foreach (RaceEventData current in raceGroup.RaceEvents)
	    {
	        if (!PlayerProfileManager.Instance.ActiveProfile.IsEventCompleted(current.EventID))
	            return current;
	    }
	    return null;
	}

    public short GetEventNumberWithinAllGroup(RaceEventData eventData)
    {
        var parent = eventData.Parent;
        short number = 0;
        if (parent != null)
        {
            foreach (var raceEventGroup in parent.RaceEventGroups)
            {
                foreach (var raceEventData in raceEventGroup.RaceEvents)
                {
                    if (eventData == raceEventData)
                    {
                        return number;
                    }
                    number++;
                }
            }
        }
        return number;
    }

    public RaceEventData GetCarSpecificEvent(BaseCarTierEvents zCarTierEvents, bool zForceFirstRace = false)
    {
        if (zForceFirstRace)
        {
            if (zCarTierEvents.CarSpecificEvents.RaceEventGroups.Count == 0 || zCarTierEvents.CarSpecificEvents.RaceEventGroups[0].RaceEvents.Count == 0)
            {
                return null;
            }
            return zCarTierEvents.CarSpecificEvents.RaceEventGroups[0].RaceEvents[0];
        }
        List<RaceEventGroup> raceEventGroups = zCarTierEvents.CarSpecificEvents.RaceEventGroups;
        var targetCarModel =
            PlayerProfileManager.Instance.ActiveProfile.GetTargetCarModelForCarSpecificEvents(
                zCarTierEvents.GetCarTier());
        if (targetCarModel == "None" || targetCarModel=="")
        {
            RaceEventGroup firstEventGroupForCarWeDontOwn = RaceEventQueryHelpers.GetFirstEventGroupForCarWeDontOwn(raceEventGroups);
            if (firstEventGroupForCarWeDontOwn != null)
            {
                string carForCarSpecificEventGroup = RaceEventQueryHelpers.GetCarForCarSpecificEventGroup(firstEventGroupForCarWeDontOwn);
                PlayerProfileManager.Instance.ActiveProfile.SetTargetCarModelForCarSpecificEvents(zCarTierEvents.GetCarTier(), carForCarSpecificEventGroup);
                return GetNextUnCompletedEventFromGroup(firstEventGroupForCarWeDontOwn);
            }
            foreach (RaceEventGroup item in raceEventGroups)
            {
                int num = (item.NumEventsComplete() >= item.NumOfEvents()) ? (-1) : item.NumEventsComplete();
                if (num >= 0)
                {
                    return GetNextUnCompletedEventFromGroup(item);
                }
            }
            return null;
        }
        foreach (RaceEventGroup item2 in raceEventGroups)
        {
            string carForCarSpecificEventGroup2 = RaceEventQueryHelpers.GetCarForCarSpecificEventGroup(item2);
            if (carForCarSpecificEventGroup2 == PlayerProfileManager.Instance.ActiveProfile.GetTargetCarModelForCarSpecificEvents(zCarTierEvents.GetCarTier()))
            {
                if (item2.NumEventsComplete() < item2.NumOfEvents())
                {
                    return GetNextUnCompletedEventFromGroup(item2);
                }
                PlayerProfileManager.Instance.ActiveProfile.SetTargetCarModelForCarSpecificEvents(zCarTierEvents.GetCarTier(), "None");
                return GetCarSpecificEvent(zCarTierEvents);
            }
        }
        return null;
    }


    public RaceEventData GetOnlineRaceEvent(RaceEventTopLevelCategory zCarTierEvents,int eventIndex, bool zForceFirstRace = false)
    {
        //if (!zForceFirstRace)
        //{
        //    var raceEventGroup = zCarTierEvents.RaceEventGroups[groupIndex];
        //    var streakNumber = PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins;
        //    var raceEvent = raceEventGroup.RaceEvents[streakNumber];
        //    return raceEvent;
        //}
        return zCarTierEvents.RaceEventGroups[0].RaceEvents[eventIndex];
    }

    public RaceEventData GetManufacturerSpecificRaceEvent(BaseCarTierEvents zCarTierEvents, bool zForceFirstRace = false)
	{
        if (zForceFirstRace)
        {
            if (zCarTierEvents.ManufacturerSpecificEvents.RaceEventGroups.Count == 0 || zCarTierEvents.ManufacturerSpecificEvents.RaceEventGroups[0].RaceEvents.Count == 0)
            {
                return null;
            }
            return zCarTierEvents.ManufacturerSpecificEvents.RaceEventGroups[0].RaceEvents[0];
        }
        List<RaceEventGroup> raceEventGroups = zCarTierEvents.ManufacturerSpecificEvents.RaceEventGroups;
        var targetManufacture =
            PlayerProfileManager.Instance.ActiveProfile.GetTargetManufacturerForManufacturerEvents(
                zCarTierEvents.GetCarTier());
        if (targetManufacture == "None" || targetManufacture=="" || targetManufacture == "0")
        {
            RaceEventGroup firstEventGroupForManufacturerWeDontOwn = RaceEventQueryHelpers.GetFirstEventGroupForManufacturerWeDontOwn(raceEventGroups);
            if (firstEventGroupForManufacturerWeDontOwn != null)
            {
                string manufacturerForManufacturerSpecificEvent = RaceEventQueryHelpers.GetManufacturerForManufacturerSpecificEvent(firstEventGroupForManufacturerWeDontOwn);
                PlayerProfileManager.Instance.ActiveProfile.SetTargetManufacturerForManufacturerEvents(zCarTierEvents.GetCarTier(), manufacturerForManufacturerSpecificEvent);
                return GetNextUnCompletedEventFromGroup(firstEventGroupForManufacturerWeDontOwn);
            }
            foreach (RaceEventGroup item in raceEventGroups)
            {
                int num = (item.NumEventsComplete() >= item.NumOfEvents()) ? (-1) : item.NumEventsComplete();
                if (num >= 0)
                {
                    return GetNextUnCompletedEventFromGroup(item);
                }
            }
            return null;
        }
        foreach (RaceEventGroup item2 in raceEventGroups)
        {
            string manufacturerForManufacturerSpecificEvent2 = RaceEventQueryHelpers.GetManufacturerForManufacturerSpecificEvent(item2);
            if (manufacturerForManufacturerSpecificEvent2 == PlayerProfileManager.Instance.ActiveProfile.GetTargetManufacturerForManufacturerEvents(zCarTierEvents.GetCarTier()))
            {
                if (item2.NumEventsComplete() < item2.NumOfEvents())
                {
                    return GetNextUnCompletedEventFromGroup(item2);
                }
                PlayerProfileManager.Instance.ActiveProfile.SetTargetManufacturerForManufacturerEvents(zCarTierEvents.GetCarTier(), "None");
                return GetManufacturerSpecificRaceEvent(zCarTierEvents);
            }
        }
        return null;
    }

	public RaceEventData GetCrewBattleEvent(BaseCarTierEvents zCarTierEvents, bool zForceFirstRace = false)
	{
		if (zForceFirstRace)
		{
			return zCarTierEvents.CrewBattleEvents.RaceEventGroups[0].RaceEvents[0];
		}
		RaceEventGroup raceGroup = zCarTierEvents.CrewBattleEvents.RaceEventGroups[0];
		return this.GetNextUnCompletedEventFromGroup(raceGroup);
	}

	public RaceEventData GetRestrictionEvent(BaseCarTierEvents zCarClassEvents, bool zForceFirstRace = false)
	{
		if (zForceFirstRace)
		{
			return zCarClassEvents.RestrictionEvents.RaceEventGroups[0].RaceEvents[0];
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		List<RaceEventGroup> raceEventGroups = zCarClassEvents.RestrictionEvents.RaceEventGroups;
        int lastEventID = activeProfile.GetLastShownRestrictionEventID(zCarClassEvents.GetCarTier());
		if (lastEventID != 0 && !activeProfile.EventsCompleted.Contains(lastEventID))
		{
			IEnumerable<RaceEventData> source = raceEventGroups.SelectMany((RaceEventGroup q) => q.RaceEvents);
			RaceEventData raceEventData = source.FirstOrDefault((RaceEventData q) => q.EventID == lastEventID);
			if (raceEventData != null)
			{
				return raceEventData;
			}
		}
		List<int> eventsCompleted = PlayerProfileManager.Instance.ActiveProfile.EventsCompleted;
		List<int> numberOfEventsCompletedPerGroup = new List<int>();
		for (int i = 0; i < raceEventGroups.Count; i++)
		{
			RaceEventGroup raceEventGroup = raceEventGroups[i];
			numberOfEventsCompletedPerGroup.Add(0);
			foreach (RaceEventData current in raceEventGroup.RaceEvents)
			{
				if (eventsCompleted.Contains(current.EventID))
				{
					numberOfEventsCompletedPerGroup[i] = numberOfEventsCompletedPerGroup[i] + 1;
				}
			}
		}
		Dictionary<int, int> uncompletedEventIndexPerGroupDict = new Dictionary<int, int>();
		for (int j = 0; j < raceEventGroups.Count; j++)
		{
			bool findUncompletedEvent = false;
			foreach (RaceEventData current2 in raceEventGroups[j].RaceEvents)
			{
				if (!eventsCompleted.Contains(current2.EventID))
				{
					findUncompletedEvent = true;
					break;
				}
			}
			if (findUncompletedEvent)
			{
				uncompletedEventIndexPerGroupDict.Add(j, numberOfEventsCompletedPerGroup[j]);
			}
		}
		if (uncompletedEventIndexPerGroupDict.Count == 0)
		{
			return null;
		}
		int selectedGroup = -1;
		int minEventNumberCompleted = -1;
        foreach (KeyValuePair<int, int> currentGroup in uncompletedEventIndexPerGroupDict)
        {
            if (raceEventGroups[currentGroup.Key].GroupPriority != -1 ||
                raceEventGroups[currentGroup.Key].RaceEvents.Count <= 0 || raceEventGroups[currentGroup.Key]
                    .RaceEvents[0].Restrictions[0].DoesMeetRestriction(activeProfile.PlayerPhysicsSetup))
            {
                if (minEventNumberCompleted < 0 || currentGroup.Value < minEventNumberCompleted || raceEventGroups[currentGroup.Key].GroupPriority < -1)
                {
                    minEventNumberCompleted = numberOfEventsCompletedPerGroup[currentGroup.Key];
                    selectedGroup = currentGroup.Key;
                }
            }
        }

        if (selectedGroup == -1)
		{
            selectedGroup = uncompletedEventIndexPerGroupDict.Keys.First();
		}
		List<RaceEventData> selectedGroupEventList = new List<RaceEventData>(raceEventGroups[selectedGroup].RaceEvents);
		foreach (int completedEventID in eventsCompleted)
		{
			selectedGroupEventList.RemoveAll((RaceEventData x) => x.EventID == completedEventID);
		}
		if (selectedGroupEventList.Count == 0)
		{
			return null;
		}
		int lowestEventOrderIndex = -1;
		foreach (RaceEventData currentEvent in selectedGroupEventList)
		{
			if (currentEvent.Restrictions[0].DoesMeetRestriction(activeProfile.PlayerPhysicsSetup) && (lowestEventOrderIndex == -1 || (int)currentEvent.EventOrder < lowestEventOrderIndex))
			{
				lowestEventOrderIndex = (int)currentEvent.EventOrder;
			}
		}
		RaceEventData selectedEventData = selectedGroupEventList.Find((RaceEventData x) => (int)x.EventOrder == lowestEventOrderIndex);
		if (selectedEventData == null)
		{
			selectedEventData = selectedGroupEventList[0];
		}
		activeProfile.SetLastShownRestrictionEventID(zCarClassEvents.GetCarTier(), selectedEventData.EventID);
		return selectedEventData;
	}

	private void Log(string zLog)
	{
		bool flag = false;
		if (flag)
		{
		}
	}
}
