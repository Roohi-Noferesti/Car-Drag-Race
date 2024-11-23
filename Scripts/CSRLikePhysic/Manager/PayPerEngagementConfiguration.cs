using System;
using System.Collections.Generic;

[Serializable]
public class PayPerEngagementConfiguration
{
	public enum eProvider
	{
		None,
		TapJoy
	}

	public enum eEngagement
	{
		PlayDailyBattle,
		Win5Races,
		UnlockTier2,
		UnlockTier3
	}

	public const int Win5RacesCount = 5;

	public string AppStore;

	public List<PPEAppStoreConfiguration> Engagements = new List<PPEAppStoreConfiguration>();

	public GTAppStore AppStoreEnum
	{
		get
		{
			return EnumHelper.FromString<GTAppStore>(this.AppStore);
		}
	}
}
