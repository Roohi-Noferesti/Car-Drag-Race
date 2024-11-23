using System;

[Serializable]
public class ConsumableValueData
{
	public int GoldCost;

	public int CashCost;

	public int MinutesActive;

	public int RacesActive;

	public ConsumableValueData Clone()
	{
		return base.MemberwiseClone() as ConsumableValueData;
	}
}
