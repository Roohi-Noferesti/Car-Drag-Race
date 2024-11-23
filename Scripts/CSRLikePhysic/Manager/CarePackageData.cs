using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CarePackageData
{
	private const string CARE_PACKAGE_ID = "cpid";

	private const string CARE_PACKAGE_DISPLAYED = "cpdp";

	private const string LAST_UPDATE_TIME = "cput";

	private const string REWARD_COUNT = "cprc";

	private const string RECEIVED_LIST = "cprl";

	public string CarePackageId;

	public bool CarePackageDisplayed;

	public DateTime LastUpdateTime = GTDateTime.Now;

    [SerializeField]
	private List<CarePackageReceivedLevelData> levelsReceieved = new List<CarePackageReceivedLevelData>();

	public List<CarePackageReceivedLevelData> GetLevelsReceived()
	{
		return this.levelsReceieved;
	}

	public void Set(DateTime time, string id, bool displayed)
	{
		this.LastUpdateTime = time;
		this.CarePackageId = id;
		this.CarePackageDisplayed = displayed;
	}

	public void IncrementReceivedCount(string levelID)
	{
		CarePackageReceivedLevelData carePackageReceivedLevelData = this.getLevel(levelID);
		if (carePackageReceivedLevelData == null)
		{
			carePackageReceivedLevelData = new CarePackageReceivedLevelData(levelID);
			this.levelsReceieved.Add(carePackageReceivedLevelData);
		}
		carePackageReceivedLevelData.IncrementReceivedCount();
	}

	public int TotalReceivedCount()
	{
		int num = 0;
		foreach (CarePackageReceivedLevelData current in this.levelsReceieved)
		{
			num += current.ReceivedCount;
		}
		return num;
	}

	public int TotalReceivedLevelCount(string levelID)
	{
		CarePackageReceivedLevelData level = this.getLevel(levelID);
		if (level == null)
		{
			return 0;
		}
		return level.ReceivedCount;
	}

	public void FilterUnusedReceivedLevels(HashSet<string> levelIDs)
	{
		this.levelsReceieved.RemoveAll((CarePackageReceivedLevelData cp) => !levelIDs.Contains(cp.LevelID));
	}

	public void ToJson(ref JsonDict jsonDict)
	{
		jsonDict.Set("cpid", this.CarePackageId);
		jsonDict.Set("cpdp", this.CarePackageDisplayed);
		jsonDict.Set("cput", this.LastUpdateTime);
		jsonDict.SetObjectList<CarePackageReceivedLevelData>("cprl", this.levelsReceieved, new SetObjectDelegate<CarePackageReceivedLevelData>(this.SetLevelsReceived));
	}

	public void FromJson(ref JsonDict jsonDict)
	{
		jsonDict.TryGetValue("cpid", out this.CarePackageId);
		jsonDict.TryGetValue("cpdp", out this.CarePackageDisplayed, false);
		jsonDict.TryGetValue("cput", out this.LastUpdateTime, GTDateTime.Now);
		if (!jsonDict.TryGetObjectList<CarePackageReceivedLevelData>("cprl", out this.levelsReceieved, new GetObjectDelegate<CarePackageReceivedLevelData>(this.GetLevelsReceived)))
		{
			this.levelsReceieved = new List<CarePackageReceivedLevelData>();
		}
	}

	private void GetLevelsReceived(JsonDict jsonDict, ref CarePackageReceivedLevelData receivedData)
	{
		receivedData.FromJson(jsonDict);
	}

	private void SetLevelsReceived(CarePackageReceivedLevelData receivedData, ref JsonDict jsonDict)
	{
		receivedData.ToJson(jsonDict);
	}

	private CarePackageReceivedLevelData getLevel(string levelID)
	{
		return this.levelsReceieved.Find((CarePackageReceivedLevelData cp) => cp.LevelID == levelID);
	}
}
