using DataSerialization;
using EventPaneRestriction;
using System;
using System.Collections.Generic;
using System.Linq;

public static class RestrictionHelper
{
	public static IRestriction GetActiveRestriction(RaceEventData raceData)
	{
        if (raceData.IsRelay && !raceData.IsRandomRelay() && !raceData.SwitchBackRace)
        {
            //RelayCarsRequired relayCarsRequired = new RelayCarsRequired(raceData);
            //return (!relayCarsRequired.IsRestrictionActive()) ? null : relayCarsRequired;
        }
        //We comment out this lines of code because it want to show choice screen for world tour races that we do not implemented it in our game
        //if (raceData.IsWorldTourRace())
        //{
        //    PinDetail worldTourPinPinDetail = raceData.GetWorldTourPinPinDetail();
        //    if (worldTourPinPinDetail.WorldTourScheduledPinInfo != null && worldTourPinPinDetail.WorldTourScheduledPinInfo.ChoiceScreen != null)
        //    {
        //        return null;
        //    }
        //}
        List<IRestriction> list = new List<IRestriction>
		{
            new SpecificCarsRequired(raceData),
            new CarTierRequired(raceData),
			new GasRequired(raceData),
			new UpgradeRequired(raceData)
		};
		if (!string.IsNullOrEmpty(raceData.GetHumanCar()))
		{
			if (raceData.SwitchBackRace && raceData.Group.RaceEvents[0] == raceData)
			{
				CarInfo car = CarDatabase.Instance.GetCar(raceData.Group.RaceEvents[1].GetHumanCar());
				if (car.UsesEvoUpgrades())
				{
					list.Add(new EvoFullyFittedRequired(raceData.Group.RaceEvents[1]));
				}
			}
			else
			{
				CarInfo car2 = CarDatabase.Instance.GetCar(raceData.GetHumanCar());
				if (car2.UsesEvoUpgrades())
				{
					list.Add(new EvoFullyFittedRequired(raceData));
				}
			}
		}
		Dictionary<Type, List<IRestriction>> dictionary = new Dictionary<Type, List<IRestriction>>
		{
			{
				typeof(RaceTheWorldWorldTourEvents),
				new List<IRestriction>
				{
                    //new RTWWorldTourSpecificCarModelRequired(raceData),
					new GasRequired(raceData)
				}
			}
		};
		List<IRestriction> source = list;
		if (dictionary.ContainsKey(raceData.Parent.GetType()))
		{
			source = dictionary[raceData.Parent.GetType()];
		}
		return source.FirstOrDefault((IRestriction q) => q.IsRestrictionActive());
	}

    //public static IRestriction GetActiveRestriction(MultiplayerEventData data)
    //{
    //    RaceEventData raceEventData = new RaceEventData();
    //    raceEventData.Restrictions = data.Restrictions;
    //    List<IRestriction> source = new List<IRestriction>
    //    {
    //        new SpecificCarsRequired(raceEventData),
    //        new CarTierRequired(raceEventData),
    //        new CashRequired(raceEventData),
    //        new GasRequired(raceEventData),
    //        new UpgradeRequired(raceEventData)
    //    };
    //    return source.FirstOrDefault((IRestriction q) => q.IsRestrictionActive());
    //}

    public static IRestriction GetActiveRestriction(ModeInfo data)
    {
        RaceEventData raceEventData = new RaceEventData();
        raceEventData.Restrictions = data.Restrictions;
        List<IRestriction> source = new List<IRestriction>
        {
            new SpecificCarsRequired(raceEventData)
        };
        return source.FirstOrDefault((IRestriction q) => q.IsRestrictionActive());
    }
}
