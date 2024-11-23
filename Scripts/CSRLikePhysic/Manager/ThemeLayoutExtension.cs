using DataSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class ThemeLayoutExtension
{
	[Conditional("CSR_DEBUG_LOGGING")]
	public static void Validate(this ThemeLayout tl)
	{
		tl.PinDetails.ForEach(delegate(PinDetail pin)
		{
		});
	}

	[Conditional("CSR_DEBUG_LOGGING")]
	public static void ValidateRestrictionPins(this ThemeLayout tl)
	{
		if (tl.IsOverviewTheme)
		{
			return;
		}
		Dictionary<string, eRaceEventRestrictionType> textureNameToRestrictionTypeMap = new Dictionary<string, eRaceEventRestrictionType>
		{
			{
				"Star_rest_NOS_strike",
				eRaceEventRestrictionType.CAR_NO_NITROUS_ALLOWED
			},
			{
				"Star_hp",
				eRaceEventRestrictionType.CAR_HORSEPOWER_LESS_THAN
			},
			{
				"Star_weight_up",
				eRaceEventRestrictionType.CAR_WEIGHT_MORE_THAN
			},
			{
				"Star_rest_tyre_Strike",
				eRaceEventRestrictionType.CAR_NO_TYRES_ALLOWED
			},
			{
				"Star_weight_down",
				eRaceEventRestrictionType.CAR_WEIGHT_LESS_THAN
			},
			{
				"Star_no_upgrades",
				eRaceEventRestrictionType.CAR_NO_UPGRADES_ALLOWED
			},
			{
				"Star_rest_NOS",
				eRaceEventRestrictionType.CAR_NITROUS_NEEDED
			},
			{
				"Star_rest_tyre",
				eRaceEventRestrictionType.CAR_TYRES_NEEDED
			}
		};
		foreach (PinDetail current in tl.PinDetails)
		{
			if (current.EventID != 0)
			{
				RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(current.EventID);
				if (eventByEventIndex != null && eventByEventIndex.Restrictions.Count != 0)
				{
					if (!string.IsNullOrEmpty(current.GetOverlaySprite()))
					{
						if (current.GetOverlaySprite() != current.GetEventOverlayTexture())
						{
						}
						string spriteName = current.GetOverlaySprite().Substring(current.GetOverlaySprite().LastIndexOf("/") + 1);
						if (!textureNameToRestrictionTypeMap.ContainsKey(spriteName))
						{
						}
					}
				}
			}
		}
	}

	public static ThemeOptionLayoutDetails GetThemeOptionLayoutDetails(this ThemeLayout tl)
	{
		if (TierXManager.Instance.CurrentThemeOption == null)
		{
			return null;
		}
		ThemeOptionLayoutDetails result = null;
		tl.ThemeOptionLayoutDetails.TryGetValue(TierXManager.Instance.CurrentThemeOption, out result);
		return result;
	}

	public static void Initialise(this ThemeLayout tl)
	{
		tl.PinDetails.ForEach(delegate(PinDetail p)
		{
			p.Initialise();
		});
	}
}
