using System.Collections.Generic;
using UnityEngine;

public class AIPlayersDatabase : ConfigurationAssetLoader
{
	public AIDriverData DefaultAISetupData
	{
		get;
		set;
	}

	public AIPlayersConfiguration Configuration
	{
		get;
		private set;
	}

	public AIPlayersDatabase() : base(GTAssetTypes.configuration_file, "AIPlayersConfiguration")
	{
		this.Configuration = null;
		this.DefaultAISetupData = new AIDriverData();
		this.DefaultAISetupData.AIDriverDBKey = "DefaultDriver";
		this.DefaultAISetupData.LaunchRPMVariation = 500f;
		this.DefaultAISetupData.TargetLaunchRPM = 5000f;
		this.DefaultAISetupData.RPMFromPeakPowerAtGearChange = 300f;
		this.DefaultAISetupData.ReactionTime = 0.2f;
		this.DefaultAISetupData.FirstGearLimitChangeUpPercent = 80f;
		this.DefaultAISetupData.Name = "Driver";
		this.DefaultAISetupData.NumberPlateString = "X15 PAM";
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (AIPlayersConfiguration) scriptableObject;//JsonConverter.DeserializeObject<AIPlayersConfiguration>(assetDataString);
	}

	public AIDriverData GetAIDriverData(string driverDBKey)
	{
		AIDriverData aIDriverData = this.Configuration.AIDrivers.Find((AIDriverData x) => x.AIDriverDBKey == driverDBKey);
		if (aIDriverData == null)
		{
			return this.DefaultAISetupData;
		}
		return aIDriverData;
	}

	public AIDriverData GetSpecificCarDriver(string zCarForDriver)
	{
		AIDriverData aIDriverData = this.Configuration.AIDrivers.Find((AIDriverData x) => x.AIDriverDBKey == zCarForDriver);
		if (aIDriverData == null)
		{
			return this.DefaultAISetupData;
		}
		return aIDriverData;
	}

	public List<AIDriverData> GetAllDrivers()
	{
		return this.Configuration.AIDrivers;
	}

	public string GetDriverNumberPlateString(AIDriverData aiDriver)
	{
		if (aiDriver.NumberPlateString != null && aiDriver.NumberPlateString.Length > 0)
		{
			return aiDriver.NumberPlateString;
		}
		if (this.Configuration.AINumberPlates != null && this.Configuration.AINumberPlates.Count > 0)
		{
			return this.Configuration.AINumberPlates[Random.Range(0, this.Configuration.AINumberPlates.Count)];
		}
		return "ENEMY";
	}
}
