using System.Collections.Generic;
using UnityEngine;

public class RacePlayerInfoComponent : PlayerInfoComponent
{
	public string _carDBKey;

	public int _ppIndex;

	public Dictionary<eUpgradeType, CarUpgradeStatus> _carUpgradeStatus;

	public eCarTier _carTier;

	public bool _isEliteCar;

	public bool _hasSportsUpgrade;

	public int _colourIndex;

	private string _appliedLivery = string.Empty;

	public Color _colourTint = Color.white;

	public Color CarColour
	{
		get
		{
			return this._colourTint;
		}
	}

	public int AppliedColourIndex
	{
		get
		{
			return this._colourIndex;
		}
	}

	public string AppliedLivery
	{
		get
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(this._appliedLivery))
			{
				CarInfo car = CarDatabase.Instance.GetCar(this.CarDBKey);
				text = car.ModelPrefabString + "_Livery_" + this._appliedLivery;
				if (AssetDatabaseClient.Instance.Directory.GetAssetDirectoryEntry(text) == null)
				{
					text = string.Empty;
				}
			}
			return text;
		}
		set
		{
			this._appliedLivery = value;
			if (!string.IsNullOrEmpty(this._appliedLivery))
			{
				int startIndex = this._appliedLivery.IndexOf("Livery") + 7;
				this._appliedLivery = this._appliedLivery.Substring(startIndex);
			}
		}
	}

	public bool IsEliteLiveryApplied
	{
		get
		{
			return this.AppliedLivery != null && this.AppliedLivery.ToLower().EndsWith("elite");
		}
	}

	public bool IsEliteSportsLiveryApplied
	{
		get
		{
			return this.AppliedLivery != null && this.AppliedLivery.ToLower().EndsWith("elite_sports");
		}
	}

	public string CarDBKey
	{
		get
		{
			return this._carDBKey;
		}
		set
		{
			this._carDBKey = value;
		}
	}

	public int PPIndex
	{
		get
		{
			return this._ppIndex;
		}
		set
		{
			this._ppIndex = value;
		}
	}

	public eCarTier CarTier
	{
		get
		{
			return this._carTier;
		}
		set
		{
			this._carTier = value;
		}
	}

	public Dictionary<eUpgradeType, CarUpgradeStatus> CarUpgradeStatus
	{
		get
		{
			return this._carUpgradeStatus;
		}
		set
		{
			this._carUpgradeStatus = value;
		}
	}

	public bool IsEliteCar
	{
		get
		{
			return this._isEliteCar;
		}
	}

	public bool HasSportsUpgrade
	{
		get
		{
			return this._hasSportsUpgrade;
		}
	}

    public override void SerialiseToJson(JsonDict jsonDict)
    {
        jsonDict.Set("pp", this._ppIndex);
        jsonDict.Set("ci", (int)this._carTier);
        jsonDict.Set("ec", this._isEliteCar);
        jsonDict.Set("su", this._hasSportsUpgrade);
        jsonDict.Set("ac", this._colourIndex);
        jsonDict.Set("al", this._appliedLivery);
        //jsonDict.Set("cl", ColorUtils.HexStringFromUnityColour(this._colourTint));
    }

    public override void SerialiseFromJson(JsonDict jsonDict)
    {
        jsonDict.TryGetValue("pp", out this._ppIndex);
        int carTier;
        jsonDict.TryGetValue("ci", out carTier);
        this._carTier = (eCarTier)carTier;
        jsonDict.TryGetValue("ec", out this._isEliteCar);
        jsonDict.TryGetValue("su", out this._hasSportsUpgrade);
        jsonDict.TryGetValue("ac", out this._colourIndex);
        jsonDict.TryGetValue("al", out this._appliedLivery);
        string colourString;
        jsonDict.TryGetValue("cl", out colourString);
        //this._colourTint = ColorUtils.UnityColourFromString(colourString);
    }

	public void PopulatePostRaceSetup()
	{
        //this._colourTint = CompetitorManager.Instance.LocalCompetitor.CarVisuals.CurrentBaseColor;
	}
}
