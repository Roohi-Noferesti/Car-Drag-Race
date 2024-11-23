using System.Collections.Generic;

public class CarDataDefaults
{
	public const string DefaultCarAssetName = "FordFocusST";

	public static readonly List<string> NonEliteBossCars = new List<string>
	{
		"MiniCooperSBoss",
		"Nissan370ZBoss",
		"BMWM3CoupeBoss",
		"ChevroletCorvetteZR1Boss",
		"AudiR8V10LMSBoss"
	};

	public static bool IsBossCar(string carDBKey)
	{
		return CarDatabase.Instance.GetCar(carDBKey).IsBossCarOverride;
	}

	public static bool CarIsExcludedFromGame(CarInfo car)
	{
		return car.Key == "RedMercedesAMGGT";
	}
}
