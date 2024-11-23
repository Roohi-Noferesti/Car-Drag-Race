using System;

[Serializable]
public class CarePackageConsumableReward
{
	public string Consumable;

	public int MinutesActive;

	public int RacesActive;

	public void GiveToPlayer()
	{
		if (!string.IsNullOrEmpty(this.Consumable))
		{
			eCarConsumables eCarConsumables = EnumHelper.FromString<eCarConsumables>(this.Consumable);
			this.Consumable = string.Empty;
			ConsumableValueData consumableValueData = new ConsumableValueData();
			consumableValueData.MinutesActive = this.MinutesActive;
			consumableValueData.RacesActive = this.RacesActive;
			if (eCarConsumables != eCarConsumables.WholeTeam)
			{
				ConsumablesManager.SetupRaceTeamConsumable(eCarConsumables, consumableValueData, 0, 0);
			}
			else
			{
				ConsumablesManager.SetupWholeTeamConsumable(consumableValueData);
			}
		}
	}
}
