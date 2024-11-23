using DataSerialization;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CarePackage
{
	public string ID;

    public bool Enabled;

	public List<EligibilityCondition> EligibilityConditions = new List<EligibilityCondition>();

	public List<CarePackageLevel> CarePackageLevels = new List<CarePackageLevel>();

	public HashSet<string> Initialise()
	{
		this.EligibilityConditions.ForEach(delegate(EligibilityCondition item)
		{
			item.Initialise();
		});
		HashSet<string> hashSet = new HashSet<string>();
		foreach (CarePackageLevel current in this.CarePackageLevels)
		{
			current.Initialise();
			hashSet.Add(current.ID);
		}
		this.sortLevelsByElapsedTime();
		return hashSet;
	}

	private void sortLevelsByElapsedTime()
	{
		this.CarePackageLevels.Sort((CarePackageLevel level1, CarePackageLevel level2) => Convert.ToInt32(level2.InactiveTimeSpan.TotalSeconds - level1.InactiveTimeSpan.TotalSeconds));
	}

	public bool IsEligible()
	{
	    if (!Enabled)
	        return false;
		IGameState gameState = new GameStateFacade();
		foreach (EligibilityCondition current in this.EligibilityConditions.Where(c=>c.IsActive))
		{
			if (!current.IsValid(gameState))
			{
				return false;
			}
		}
		return true;
	}

	public CarePackageLevel GetActiveLevel()
	{
		DateTime carePackageUpdateTime = PlayerProfileManager.Instance.ActiveProfile.CarePackageUpdateTime;
		DateTime t = CarePackagesDatabase.Time();
		foreach (CarePackageLevel current in this.CarePackageLevels)
		{
			if (carePackageUpdateTime.Add(current.InactiveTimeSpan) < t)
			{
				return current;
			}
		}
		return null;
	}

	public CarePackageLevel GetLevel(string levelID)
	{
		return this.CarePackageLevels.Find((CarePackageLevel level) => level.ID == levelID);
	}

	public void Schedule()
	{
		PlayerProfileManager.Instance.ActiveProfile.SetCarePackageInfo(CarePackagesDatabase.Time(), this.ID, false);
		NotificationManager.Active.ClearCarePackageNotifications();
		this.CarePackageLevels.ForEach(delegate(CarePackageLevel item)
		{
			item.ScheduleNotification();
		});
	}
}
