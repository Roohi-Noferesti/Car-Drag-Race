using System;
using System.Collections.Generic;

public class RaceEventDatabase
{
	public static RaceEventDatabase instance;

	public int CompletedCrewBattleEventsToUnlockAmateurRR = 2;

	public int CompletedCrewBattleEventsToUnlockProRR = 3;

	public HashSet<RaceEventData> RaceEvents = new HashSet<RaceEventData>();

	public RaceEventDatabaseData EventData
	{
		get;
		private set;
	}

    //public RaceEventRewardsMetadata EventRewardsMetadata
    //{
    //    get;
    //    private set;
    //}

    //public DailyBattleEventCollection DailyBattleEvents
    //{
    //    get;
    //    private set;
    //}

    //public SeriesEventCollection InvitationalEvents
    //{
    //    get;
    //    private set;
    //}

    //public GachaEventCollection GachaEvents
    //{
    //    get;
    //    private set;
    //}

    //public ECBEventCollection ECBEvents
    //{
    //    get;
    //    private set;
    //}

    //public ManufacturerLadderEvents ManufacturerLadderEvents
    //{
    //    get;
    //    private set;
    //}

	public RaceEventDatabase()
	{
		this.RaceEvents.Clear();
        //this.EventRewardsMetadata = new RaceEventRewardsMetadata();
	}

	public static void Create()
	{
		if (RaceEventDatabase.instance != null)
		{
			return;
		}
		RaceEventDatabase.instance = new RaceEventDatabase();
	}

	public void Unload()
	{
		this.EventData = null;
        //this.InvitationalEvents = null;
        //this.DailyBattleEvents = null;
        //this.RaceEvents.Clear();
        //this.EventRewardsMetadata.Clear();
	}

    //public void SetAndProcessGachaEventsCalendar(GachaEventCollection event_data)
    //{
    //    long ticks = DateTime.Now.Ticks;
    //    this.GachaEvents = event_data;
    //    long ticks2 = DateTime.Now.Ticks;
    //    TimeSpan timeSpan = new TimeSpan(ticks2 - ticks);
    //}

	public void SetAndProcessCareerRaceEventsData(RaceEventDatabaseData event_data)
	{
		//long ticks = DateTime.Now.Ticks;
		this.EventData = event_data;
        //this.EventData.ProcessEvents();
		//long ticks2 = DateTime.Now.Ticks;
		//TimeSpan timeSpan = new TimeSpan(ticks2 - ticks);
	}

    //public void SetAndProcessManufacturerLadderRaceEventsData(ManufacturerLadderEvents event_data)
    //{
    //    long ticks = DateTime.Now.Ticks;
    //    this.ManufacturerLadderEvents = event_data;
    //    event_data.LadderEvents.ProcessEvents(this.EventData.GetTierEvents(eCarTier.TIER_1));
    //    this.ManufacturerLadderEvents.Manufacturer = event_data.Manufacturer;
    //    long ticks2 = DateTime.Now.Ticks;
    //    TimeSpan timeSpan = new TimeSpan(ticks2 - ticks);
    //}

    //public void SetAndProcessECBRaceEventsData(ECBEventCollection event_data)
    //{
    //    long ticks = DateTime.Now.Ticks;
    //    this.ECBEvents = event_data;
    //    this.ECBEvents.ProcessEvents();
    //    long ticks2 = DateTime.Now.Ticks;
    //    TimeSpan timeSpan = new TimeSpan(ticks2 - ticks);
    //}

    //public void SetAndProcessInvitationalEvents(SeriesEventCollection event_collection)
    //{
    //    long ticks = DateTime.Now.Ticks;
    //    this.InvitationalEvents = event_collection;
    //    this.InvitationalEvents.ProcessEvents();
    //    long ticks2 = DateTime.Now.Ticks;
    //    TimeSpan timeSpan = new TimeSpan(ticks2 - ticks);
    //}

    //public void SetAndProcessCrewInvitationalEvents(SeriesEventCollection event_collection)
    //{
    //    long ticks = DateTime.Now.Ticks;
    //    this.InvitationalEvents.RaceSeries.AddRange(event_collection.RaceSeries);
    //    this.InvitationalEvents.ProcessEvents();
    //    long ticks2 = DateTime.Now.Ticks;
    //    TimeSpan timeSpan = new TimeSpan(ticks2 - ticks);
    //}

    //public void SetAndProcessCupInvitationalEvents(SeriesEventCollection event_collection)
    //{
    //    long ticks = DateTime.Now.Ticks;
    //    this.InvitationalEvents.RaceSeries.AddRange(event_collection.RaceSeries);
    //    this.InvitationalEvents.ProcessEvents();
    //    long ticks2 = DateTime.Now.Ticks;
    //    TimeSpan timeSpan = new TimeSpan(ticks2 - ticks);
    //}

    //public void SetAndProcessRedInvitationalEvents(SeriesEventCollection event_collection)
    //{
    //    long ticks = DateTime.Now.Ticks;
    //    this.InvitationalEvents.RaceSeries.AddRange(event_collection.RaceSeries);
    //    this.InvitationalEvents.ProcessEvents();
    //    long ticks2 = DateTime.Now.Ticks;
    //    TimeSpan timeSpan = new TimeSpan(ticks2 - ticks);
    //}

    //public void SetAndProcessDailyBattleEvents(DailyBattleEventCollection eventCollection)
    //{
    //    this.DailyBattleEvents = eventCollection;
    //    this.DailyBattleEvents.ProcessEvents();
    //}

	public void RegisterEvent(RaceEventData eventData)
	{
		this.RaceEvents.Add(eventData);
	}

	public RaceEventData GetEventByEventIndex(int zEventIndex)
	{
		RaceEventData raceEventData = null;
		foreach (RaceEventData current in this.RaceEvents)
		{
			if (current.EventID == zEventIndex)
			{
				raceEventData = current;
				break;
			}
		}
		if (raceEventData == null)
		{
			return null;
		}
		return raceEventData;
	}

	public void ReadEventRewardsFromMetadata(ConfigDictionary eventRewardsMetadata)
	{
		if (eventRewardsMetadata != null)
		{
            //this.EventRewardsMetadata.ReadFromMetadata(eventRewardsMetadata);
		}
	}
}
