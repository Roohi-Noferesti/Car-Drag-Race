using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CarsConfiguration:ScriptableObject
{
#if UNITY_EDITOR
    public bool GiveAllCars;
#endif

	public CarPerformanceIndexData CarPPData;

	public CarGearChangeLightRangesData CarGearLightData;

	public FrontendStatBarData FrontendCarStatBarData;

    public int Tier1_UnlockLevel;
    public int Tier2_UnlockLevel;
    public int Tier3_UnlockLevel;
    public int Tier4_UnlockLevel;
    public int Tier5_UnlockLevel;

	public List<string> CarsToExcludeFromRegulationRaces;

	public List<string> CarsToExcludeFromMatchRaces;

	public List<string> CarsToExcludeFromHalfMileRegulation;

	public List<string> CarsToExcludeFromRYF;

	public List<string> CarsWithEvoUpgrades;

	public bool showCarsLogo = true;
	public string postfix = "";
}