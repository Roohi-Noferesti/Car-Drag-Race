using System;
using DataSerialization;

[Serializable]
public class OfferWallConfiguration
{
	public enum eProvider
	{
		None,
		TapJoy
	}

	public string AppStore;

	public string Provider;

	public EligibilityRequirements Requirements = new EligibilityRequirements();

	public eProvider ProviderEnum
	{
		get
		{
			return EnumHelper.FromString<eProvider>(this.Provider);
		}
	}

	public GTAppStore AppStoreEnum
	{
		get
		{
			return EnumHelper.FromString<GTAppStore>(this.AppStore);
		}
	}

	public void Initialise()
	{
        //this.Requirements.Initialise();
	}
}
