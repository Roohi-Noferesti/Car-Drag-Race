using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeasonPrizeDatabase : ConfigurationAssetLoader
{
	private SeasonPrizesMetadata _prizes;

	private Dictionary<int, int> _lookupDictionary;

	public SeasonPrizeDatabase() : base(GTAssetTypes.configuration_file, "Prizes")
	{
	}

	public SeasonPrizeMetadata GetPrizeMetadata(SeasonPrizeIdentifier prize)
	{
		return this.GetPrize(prize.PrizeID);
	}

	public bool ContainsPrize(int prizeID)
	{
		SeasonPrizeMetadata prize = this.GetPrize(prizeID);
		if (prize == null)
		{
			return false;
		}
		List<string> list = prize.AssetBundleIds.FindAll((string bid) => !AssetDatabaseClient.Instance.Data.AssetExists(bid)).ToList<string>();
		return list.Count == 0;
	}

	public SeasonPrizeMetadata GetPrize(int prizeID)
	{
		if (this._lookupDictionary.ContainsKey(prizeID))
		{
			return this._prizes.Prizes[this._lookupDictionary[prizeID]];
		}
		return null;
	}

    //protected override void ProcessEventAssetDataString(string assetDataString)
    //{
    //    this._prizes = JsonConverter.DeserializeObject<SeasonPrizesMetadata>(assetDataString);
    //    this._lookupDictionary = new Dictionary<int, int>();
    //    for (int i = 0; i < this._prizes.Prizes.Count; i++)
    //    {
    //        this._lookupDictionary.Add(this._prizes.Prizes[i].ID, i);
    //    }
    //}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this._prizes = (SeasonPrizesMetadata)scriptableObject;
        this._lookupDictionary = new Dictionary<int, int>();
        for (int i = 0; i < this._prizes.Prizes.Count; i++)
        {
            this._lookupDictionary.Add(this._prizes.Prizes[i].ID, i);
        }
    }

	public SeasonPrizesMetadata GetAllPrizes()
	{
		return this._prizes;
	}


}
