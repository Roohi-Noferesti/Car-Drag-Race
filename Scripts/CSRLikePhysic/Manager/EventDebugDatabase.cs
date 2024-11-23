using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventDebugDatabase : ConfigurationAssetLoader
{
    public EventDebugConfiguration Configuration;
    public EventDebugDatabase() : base(GTAssetTypes.configuration_file, "EventDebugConfiguration")
    {

    }

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        Configuration = (EventDebugConfiguration) scriptableObject;
    }

    public bool ContainsEventID(int eventID)
    {
        return Events.Contains(eventID);
    }

    public List<int> Events
    {
        get
        {
            var raceEvents = GameDatabase.Instance.Career.Configuration.CareerRaceEvents;
            var events = new List<int>();
            if (Configuration.TutorialEventIndex > 0)
                events.Add(raceEvents.Tutorial.EventID);
            if (Configuration.TutorialEventIndex > 1)
                events.Add(raceEvents.Tutorial2.EventID);
            if (Configuration.TutorialEventIndex > 2)
                events.Add(raceEvents.Tutorial3.EventID);
            AddEventsFromTier(ref events, Configuration.Tier1Events, raceEvents.Tier1);
            AddEventsFromTier(ref events, Configuration.Tier2Events, raceEvents.Tier2);
            AddEventsFromTier(ref events, Configuration.Tier3Events, raceEvents.Tier3);
            AddEventsFromTier(ref events, Configuration.Tier4Events, raceEvents.Tier4);
            AddEventsFromTier(ref events, Configuration.Tier5Events, raceEvents.Tier5);

            return events;
        }
    }

    private void AddEventsFromTier(ref List<int> events, BaseTierEventDebug baseTierEventDebug, BaseCarTierEvents baseTierEvents)
    {
        if (!baseTierEventDebug.Active)
            return;

        AddEventIDsFromGroupEvent(ref events, baseTierEvents.CrewBattleEvents.RaceEventGroups[0],
            baseTierEventDebug.CrewMemberEventIndex);
        AddEventIDsFromGroupEvent(ref events, baseTierEvents.LadderEvents.RaceEventGroups[0],
            baseTierEventDebug.LadderEventIndex);
        if (baseTierEvents.CarSpecificEvents.RaceEventGroups.Count > 0)
            AddEventIDsFromGroupEvent(ref events, baseTierEvents.CarSpecificEvents.RaceEventGroups[0],
                baseTierEventDebug.SpecificEventIndex);
    }

    private void AddEventIDsFromGroupEvent(ref List<int> events,RaceEventGroup eventGroup,int eventIndex)
    {
        for (int i = 0; i < eventGroup.RaceEvents.Count && i<eventIndex; i++)
        {
            events.Add(eventGroup.RaceEvents[i].EventID);
        }
    }
}
