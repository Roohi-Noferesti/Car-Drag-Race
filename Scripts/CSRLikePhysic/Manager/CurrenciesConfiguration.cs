using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[Serializable]
public class CurrenciesConfiguration:ScriptableObject
{
	public GoldData Gold;

	public FuelData Fuel;

	public ConsumablesData Consumables;

	public DeliveryData Deliveries;

	public RewardsMultipliers RewardsMultipliers;

    public WeeklyRewardData WeeklyRewardData;
  //  public List<WeeklyRewardData> LeagueRewardData;
}




