using System;
using System.Linq;
using UnityEngine;

public class CarePackagesDatabase : ConfigurationAssetLoader
{
	private CarePackageLevel carePackageLevelToDisplay;

	public CarePackagesConfiguration Configuration
	{
		get;
		private set;
	}

	public CarePackagesDatabase() : base(GTAssetTypes.configuration_file, "CarePackagesConfiguration")
	{
		this.Configuration = null;
	}

	protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (CarePackagesConfiguration) scriptableObject;//JsonConverter.DeserializeObject<CarePackagesConfiguration>(assetDataString);
		this.Configuration.Initialise();
	}

	private CarePackageLevel getSuitableLevelFromProfile()
	{
		string carePackageId = PlayerProfileManager.Instance.ActiveProfile.CarePackageId;
		CarePackage carePackage = this.Configuration.GetCarePackage(carePackageId);
		if (carePackage != null)
		{
			return carePackage.GetActiveLevel();
		}
		return null;
	}

	public PopUp GetPopUp()
	{

#if UNITY_EDITOR
	    var eventDebug = GameDatabase.Instance.EventDebugConfiguration;
	    if (eventDebug.UseDebugCarePackage)
	    {
	        var id = eventDebug.CarePackageID;
	        var level = eventDebug.CarePackageLevel;
	        var carepackage = this.Configuration.CarePackages.FirstOrDefault(c => c.ID == id);
	        if (carepackage != null)
	        {
                return carepackage.CarePackageLevels[level].GetPopUp();
	        }
            return null;
	    }
	    else
	    {
            if (!this.IsInProgress())
            {
                return null;
            }
            return this.carePackageLevelToDisplay.GetPopUp();
        }
#else
        if (!this.IsInProgress())
        {
            return null;
        }
        return this.carePackageLevelToDisplay.GetPopUp();
#endif
    }

	private void setUpCarePackagePackageToDisplay()
	{
		if (!this.IsInProgress())
		{
			CarePackageLevel suitableLevelFromProfile = this.getSuitableLevelFromProfile();
			if (suitableLevelFromProfile != null)
			{
				this.carePackageLevelToDisplay = suitableLevelFromProfile;
				this.carePackageLevelToDisplay.Activate();
			}
		}
	}

	public void ScheduleSuitableCarePackage()
	{
		if (this.Configuration == null)
		{
			return;
		}
		if (!GameDatabase.Instance.Career.IsValid)
		{
			return;
		}
		this.setUpCarePackagePackageToDisplay();
		if (!this.IsInProgress())
		{
			CarePackage firstValidCarePackage = this.Configuration.GetFirstValidCarePackage();
			if (firstValidCarePackage != null)
			{
				firstValidCarePackage.Schedule();
			}
		}
	}

	public int ReceivedRewardCount(string carePackageID)
	{
		int num = 0;
		CarePackage carePackage = this.Configuration.GetCarePackage(carePackageID);
		foreach (CarePackageLevel current in carePackage.CarePackageLevels)
		{
			if (current.Rewards.Count > 0)
			{
				num += PlayerProfileManager.Instance.ActiveProfile.CarePackageTotalReceivedLevelCount(current.ID);
			}
		}
		return num;
	}

	public int TotalReceivedCount(string carePackageID)
	{
		CarePackage carePackage = this.Configuration.GetCarePackage(carePackageID);
		int num = 0;
		foreach (CarePackageLevel current in carePackage.CarePackageLevels)
		{
			num += PlayerProfileManager.Instance.ActiveProfile.CarePackageTotalReceivedLevelCount(current.ID);
		}
		return num;
	}

	private bool IsInProgress()
	{
		return this.carePackageLevelToDisplay != null && this.carePackageLevelToDisplay.IsInProgress;
	}

	public static DateTime Time()
	{
		return GTDateTime.Now;
	}
}
