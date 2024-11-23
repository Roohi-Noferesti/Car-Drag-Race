
using System;
using UnityEngine;

[Serializable]
public class Manufacturer
{
    public enum ShowroomType
	{
		Manufacturer,
		Tier,
		WorldTour,
		International
	}

    public string id;

	public string name;

	public int sort;

	public string badge;

	public string icon;

	public string logo;

	public ShowroomType showroomType;

	public bool logoAvailable;

	public bool iconAvailable;

	public bool badgeAvailable;

    [HideInInspector]
    public bool is_tier;

    [HideInInspector]
    public bool is_worldtour;

    [HideInInspector]
    public bool is_international;


    public string translatedName
	{
		get
		{
			if (this.IsTier())
			{
			    return "TEXT_"+ this.id;
			}
			if (this.IsWorldTour())
			{
				return "TEXT_TIER_X";
			}
			if (this.IsInternational())
			{
				return "TEXT_INTERNATIONAL";
			}
			return "TEXT_MANUFACTURE_" + this.id;
		}
	}

    public Manufacturer()
    {

    }

    public Manufacturer(string inId, string inName, int inSorter, string inBadge, string inIcon, string inLogo, bool inIsTier = false, bool inIsWorldTour = false, bool inIsInternational = false)
	{
		this.id = inId;
		this.name = inName;
		this.sort = inSorter;
		this.badge = inBadge;
		this.icon = inIcon;
		this.logo = inLogo;
		if (inIsTier)
		{
			this.showroomType = ShowroomType.Tier;
		}
		else if (inIsWorldTour)
		{
			this.showroomType = ShowroomType.WorldTour;
		}
		else if (inIsInternational)
		{
			this.showroomType = ShowroomType.International;
		}
		else
		{
			this.showroomType = ShowroomType.Manufacturer;
		}
		AssetDatabaseData data = AssetDatabaseClient.Instance.Data;
		this.badgeAvailable = data.AssetExists(this.badge);
		this.logoAvailable = data.AssetExists(this.logo);
		this.iconAvailable = data.AssetExists(this.icon);
	}

	public bool IsTier()
	{
		return this.showroomType == ShowroomType.Tier;
	}

	public bool IsWorldTour()
	{
		return this.showroomType == ShowroomType.WorldTour;
	}

	public bool IsInternational()
	{
		return this.showroomType == ShowroomType.International;
	}

	public bool IsActualManufacturer()
	{
		return !this.IsInternational() && !this.IsTier() && !this.IsWorldTour() && !this.id.Equals("csr_classics");
	}
}
