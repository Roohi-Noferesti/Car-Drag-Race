using System;
using System.Collections;
using System.Collections.Generic;

public class OfferWallData
{
	public class SpecificOfferWallData
	{
		public string provider = string.Empty;

		public int totalCashAwarded;

		public int totalGoldAwarded;

		public int totalAwardEvents;
	}

	public const string OBJECT_NAME = "owda";

	private const string SEEN_OFFER_WALL_BUTTON = "owsb";

	private const string PROVIDER_NAME = "owpv";

	private const string TOTAL_CASH_AWARDED = "toca";

	private const string TOTAL_GOLD_AWARDED = "toga";

	private const string TOTAL_AWARD_EVENTS = "toae";

	public bool SeenOfferWallButton;

	private List<OfferWallData.SpecificOfferWallData> offerWallData = new List<OfferWallData.SpecificOfferWallData>();

	public OfferWallData()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(OfferWallConfiguration.eProvider)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				OfferWallConfiguration.eProvider eProvider = (OfferWallConfiguration.eProvider)((int)enumerator.Current);
				if (eProvider != OfferWallConfiguration.eProvider.None)
				{
					OfferWallData.SpecificOfferWallData specificOfferWallData = new OfferWallData.SpecificOfferWallData();
					specificOfferWallData.provider = eProvider.ToString();
					this.offerWallData.Add(specificOfferWallData);
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}

	private void SetSpecificOfferWallData(OfferWallData.SpecificOfferWallData specificData, ref JsonDict jsonDict)
	{
		if (specificData.provider != OfferWallConfiguration.eProvider.None.ToString())
		{
			jsonDict.Set("owpv", specificData.provider);
			jsonDict.Set("toca", specificData.totalCashAwarded);
			jsonDict.Set("toga", specificData.totalGoldAwarded);
			jsonDict.Set("toae", specificData.totalAwardEvents);
		}
	}

	public void ToJson(ref JsonDict jsonDict)
	{
		jsonDict.Set("owsb", this.SeenOfferWallButton);
		jsonDict.SetObjectList<OfferWallData.SpecificOfferWallData>("owda", this.offerWallData, new SetObjectDelegate<OfferWallData.SpecificOfferWallData>(this.SetSpecificOfferWallData));
	}

	public void FromJson(ref JsonDict jsonDict)
	{
		jsonDict.TryGetValue("owsb", out this.SeenOfferWallButton);
		List<OfferWallData.SpecificOfferWallData> list = new List<OfferWallData.SpecificOfferWallData>();
		if (!jsonDict.TryGetObjectList<OfferWallData.SpecificOfferWallData>("owda", out list, new GetObjectDelegate<OfferWallData.SpecificOfferWallData>(this.GetSpecificOfferWallData)))
		{
			list = new List<OfferWallData.SpecificOfferWallData>();
		}
		List<string> list2 = new List<string>(Enum.GetNames(typeof(OfferWallConfiguration.eProvider)));
		foreach (string provider in list2)
		{
			if (provider != OfferWallConfiguration.eProvider.None.ToString() && list.Find((OfferWallData.SpecificOfferWallData p) => p.provider == provider) == null)
			{
				OfferWallData.SpecificOfferWallData specificOfferWallData = new OfferWallData.SpecificOfferWallData();
				specificOfferWallData.provider = provider;
				list.Add(specificOfferWallData);
			}
		}
		this.offerWallData = list;
	}

	private void GetSpecificOfferWallData(JsonDict jsonDict, ref OfferWallData.SpecificOfferWallData specificData)
	{
		jsonDict.TryGetValue("owpv", out specificData.provider);
		jsonDict.TryGetValue("toca", out specificData.totalCashAwarded);
		jsonDict.TryGetValue("toga", out specificData.totalGoldAwarded);
		jsonDict.TryGetValue("toae", out specificData.totalAwardEvents);
	}

	private OfferWallData.SpecificOfferWallData GetProvider(OfferWallConfiguration.eProvider provider)
	{
		return this.offerWallData.Find((OfferWallData.SpecificOfferWallData p) => p.provider == provider.ToString());
	}

	public int GetGold(OfferWallConfiguration.eProvider provider)
	{
		OfferWallData.SpecificOfferWallData provider2 = this.GetProvider(provider);
		if (this.offerWallData == null)
		{
			return 0;
		}
		return provider2.totalGoldAwarded;
	}

	public int GetAwardEvents(OfferWallConfiguration.eProvider provider)
	{
		OfferWallData.SpecificOfferWallData provider2 = this.GetProvider(provider);
		if (this.offerWallData == null)
		{
			return 0;
		}
		return provider2.totalAwardEvents;
	}

	public void AddAwardEvents(OfferWallConfiguration.eProvider provider, int amount)
	{
		OfferWallData.SpecificOfferWallData provider2 = this.GetProvider(provider);
		if (provider2 != null)
		{
			provider2.totalAwardEvents += amount;
		}
	}

	public void AddGold(OfferWallConfiguration.eProvider provider, int amount)
	{
		OfferWallData.SpecificOfferWallData provider2 = this.GetProvider(provider);
		if (provider2 != null)
		{
			provider2.totalGoldAwarded += amount;
		}
	}
}
