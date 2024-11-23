using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OfferPackDatabase : ConfigurationAssetLoader
{
	public OfferPackConfiguration Configuration
	{
		get;
		private set;
	}


    public OfferPackDatabase()
        : base(GTAssetTypes.configuration_file, "OfferPackConfiguration")
    {
        this.Configuration = null;
    }

	protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
        this.Configuration = (OfferPackConfiguration) scriptableObject;//JsonConverter.DeserializeObject<OfferPackConfiguration>(assetDataString);
		this.Configuration.Offers.ForEach(delegate(OfferPackData offer)
		{
			offer.Initialise();
		});
	}

	public List<OfferPackData> GetEligibleOfferPacks()
	{
		List<OfferPackData> list = new List<OfferPackData>();
		IGameState gs = new GameStateFacade();
		foreach (OfferPackData current in this.Configuration.Offers)
		{
			if (current.IsEligible(gs))
			{
				list.Add(current);
			}
		}
		return list;
	}

	public OfferPackData GetOfferPackDataForProduct(string productCode)
	{
		return this.Configuration.Offers.FirstOrDefault((OfferPackData q) => productCode.Contains(q.ProductCode.ToLower()));
	}
}
