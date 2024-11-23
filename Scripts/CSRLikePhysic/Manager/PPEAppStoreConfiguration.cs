using System;

[Serializable]
public class PPEAppStoreConfiguration
{
	public string Provider;

	public string Engagement;

	public string ID;

	public PayPerEngagementConfiguration.eProvider ProviderEnum
	{
		get
		{
			return EnumHelper.FromString<PayPerEngagementConfiguration.eProvider>(this.Provider);
		}
	}

	public PayPerEngagementConfiguration.eEngagement EngagementEnum
	{
		get
		{
			return EnumHelper.FromString<PayPerEngagementConfiguration.eEngagement>(this.Engagement);
		}
	}
}
