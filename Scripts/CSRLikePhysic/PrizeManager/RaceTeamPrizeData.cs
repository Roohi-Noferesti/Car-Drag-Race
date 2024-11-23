using System;

[Serializable]
public class RaceTeamPrizeData
{
	public int ProTunerDuration;

	public int N20ManiacDuration;

	public int TireCrewDuration;

	public int GetConsumableExtensionDuration(eCarConsumables type)
	{
		switch (type)
		{
		case eCarConsumables.EngineTune:
			return this.ProTunerDuration;
		case eCarConsumables.Nitrous:
			return this.N20ManiacDuration;
		case eCarConsumables.Tyre:
			return this.TireCrewDuration;
		default:
			return 0;
		}
	}
}
