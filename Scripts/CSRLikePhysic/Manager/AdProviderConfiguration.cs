using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class AdProviderConfiguration
{
	[Serializable]
	public class PerPlatformConfiguration
	{
        [Serializable]
        public class AdSpaceDIs
        {
            public string AdSpace;
            public string ID;
        }
		public int Priority;
		public int PriorityInIslamicCountries;

        public List<AdSpaceDIs> AdSpaces = new List<AdSpaceDIs>();
	}

	public PerPlatformConfiguration Android = new PerPlatformConfiguration();

	public PerPlatformConfiguration Apple = new PerPlatformConfiguration();

	public PerPlatformConfiguration GetActivePlatformConfiguration()
	{
#if UNITY_ANDROID
        return this.Android;
#elif UNITY_IOS
        return this.Apple;
#endif
    }

    public int GetPriority()
	{
		PerPlatformConfiguration activePlatformConfiguration = this.GetActivePlatformConfiguration();
		return (BasePlatform.ActivePlatform.ShouldOverwriteAdPriority?activePlatformConfiguration.PriorityInIslamicCountries:activePlatformConfiguration.Priority);
	}

	public string GetAdUnitIDForAdSpace(string space)
	{
		PerPlatformConfiguration activePlatformConfiguration = this.GetActivePlatformConfiguration();
	    var adSpaceID = activePlatformConfiguration.AdSpaces.FirstOrDefault(a => a.AdSpace == space);
	    return (adSpaceID == null ? null : adSpaceID.ID);
	}
}
