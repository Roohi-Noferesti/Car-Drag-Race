using System;
using UnityEngine;

[Serializable]
public class CarePackageReceivedLevelData
{
	private const string RECEIVED_ID = "cpli";

	private const string RECEIVED_COUNT = "cprc";

    [SerializeField]
	private string levelID;

    [SerializeField]
	private int receivedCount;

	public string LevelID
	{
		get
		{
			return this.levelID;
		}
	}

	public int ReceivedCount
	{
		get
		{
			return this.receivedCount;
		}
	}

	public CarePackageReceivedLevelData(string levelID)
	{
		this.levelID = levelID;
	}

	public CarePackageReceivedLevelData()
	{
	}

	public void FromJson(JsonDict jsonDict)
	{
		jsonDict.TryGetValue("cpli", out this.levelID);
		jsonDict.TryGetValue("cprc", out this.receivedCount);
	}

	public void ToJson(JsonDict jsonDict)
	{
		jsonDict.Set("cpli", this.levelID);
		jsonDict.Set("cprc", this.receivedCount);
	}

	public void IncrementReceivedCount()
	{
		this.receivedCount++;
	}
}
