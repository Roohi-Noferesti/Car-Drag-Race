using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CarePackagesConfiguration:ScriptableObject
{
	public List<CarePackage> CarePackages = new List<CarePackage>();

	public void Initialise()
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (CarePackage current in this.CarePackages)
		{
			HashSet<string> other = current.Initialise();
			hashSet.UnionWith(other);
		}
		if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
		{
			PlayerProfileManager.Instance.ActiveProfile.FilterUnusedReceivedCarePackageLevels(hashSet);
		}
	}

	public CarePackage GetCarePackage(string id)
	{
		return this.CarePackages.Find((CarePackage package) => package.ID == id);
	}

	public CarePackage GetFirstValidCarePackage()
	{
        foreach (CarePackage current in this.CarePackages)
        {
            if (current.IsEligible())
            {
                return current;
            }
        }
        return null;
    }
}
