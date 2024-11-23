using System;
using System.Collections.Generic;

[Serializable]
public class SeasonPrizeMetadata
{
	public enum ePrizeType
	{
		Car,
		LicensePlate,
		Gold,
		Cash
	}

	public int ID = -1;

	public SeasonPrizeMetadata.ePrizeType Type = SeasonPrizeMetadata.ePrizeType.Cash;

	public string Data = string.Empty;

	public bool AwardedCarIsPro;

	public List<string> AssetBundleIds = new List<string>();

	public string PrizeAwardText = string.Empty;

	public string PrizeImageBundle = string.Empty;

	public bool GetPrizeDataAsInt(out int intVal)
	{
		return int.TryParse(this.Data, out intVal);
	}

	public override string ToString()
	{
		string text = "SeasonPrizeMetadata ID=" + this.ID;
		string text2 = text;
		text = string.Concat(new object[]
		{
			text2,
			"\n",
			this.Type,
			" = ",
			this.Data
		});
		if (this.AwardedCarIsPro)
		{
			text += " (pro)";
		}
		return text + "\n" + string.Join(",", this.AssetBundleIds.ToArray());
	}
}
