using System;

[Serializable]
public class BaseCarTierEvents
{
    //public UIPinPosition UIPosition = new UIPinPosition(0, 0);

	public int UnlockEventID = 2147483647;

	public CrewBattleEvents CrewBattleEvents = new CrewBattleEvents();

	public RestrictionEvents RestrictionEvents = new RestrictionEvents();

	public DailyBattleEvents DailyBattleEvents = new DailyBattleEvents();

	public LadderEvents LadderEvents = new LadderEvents();

	public RegulationRaceEvents RegulationRaceEvents = new RegulationRaceEvents();

	public WorldTourRaceEvents WorldTourRaceEvents = new WorldTourRaceEvents();

	public CarSpecificEvents CarSpecificEvents = new CarSpecificEvents();

	public ManufacturerSpecificEvents ManufacturerSpecificEvents = new ManufacturerSpecificEvents();

	public RaceEventData WinACarEvent = new RaceEventData();

	public void ProcessEvents()
	{
		this.CrewBattleEvents.ProcessEvents(this);
		this.RestrictionEvents.ProcessEvents(this);
		this.DailyBattleEvents.ProcessEvents(this);
		this.LadderEvents.ProcessEvents(this);
		this.RegulationRaceEvents.ProcessEvents(this);
		this.WorldTourRaceEvents.ProcessEvents(this);
		this.CarSpecificEvents.ProcessEvents(this);
		this.ManufacturerSpecificEvents.ProcessEvents(this);
	}

    //public override string ToString()
    //{
    //    string str = string.Empty;
    //    str += this.CrewBattleEvents.ToString();
    //    str += this.RestrictionEvents.ToString();
    //    str += this.DailyBattleEvents.ToString();
    //    str += this.WorldTourRaceEvents.ToString();
    //    str += this.LadderEvents.ToString();
    //    str += this.RegulationRaceEvents.ToString();
    //    str += this.CarSpecificEvents.ToString();
    //    return str + this.ManufacturerSpecificEvents.ToString();
    //}

	public int NumEvents()
	{
		int num = 0;
		num += this.CrewBattleEvents.NumOfEvents();
		num += this.RestrictionEvents.NumOfEvents();
		num += this.LadderEvents.NumOfEvents();
		num += this.CarSpecificEvents.NumOfEvents();
		return num + this.ManufacturerSpecificEvents.NumOfEvents();
	}

	public virtual eCarTier GetCarTier()
	{
		return eCarTier.TIER_1;
	}
}
