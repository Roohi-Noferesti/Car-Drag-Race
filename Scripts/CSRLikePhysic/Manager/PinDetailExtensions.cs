using DataSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public static class PinDetailExtensions
{
	public static string GetCarRender(this PinDetail pd)
	{
		return pd.GetTextureName(PinDetail.TextureKeys.CarRender);
	}

	public static string GetEventOverlayTexture(this PinDetail pd)
	{
		return pd.GetTextureName(PinDetail.TextureKeys.EventPaneOverlay);
	}

	public static string GetEventBackgroundTexture(this PinDetail pd)
	{
		return pd.GetTextureName(PinDetail.TextureKeys.EventPaneBackground);
	}

	public static string GetEventPaneBossSprite(this PinDetail pd)
	{
		return pd.GetTextureName(PinDetail.TextureKeys.EventPaneBoss);
	}

	public static string GetOverlaySprite(this PinDetail pd)
	{
		return pd.GetTextureName(PinDetail.TextureKeys.PinOverlay);
	}

	public static UnityEngine.Vector2 GetOverlayOffset(this PinDetail pd)
	{
		return pd.GetTextureOffset(PinDetail.TextureKeys.PinOverlay);
	}

	public static string GetBossSprite(this PinDetail pd)
	{
		return pd.GetTextureName(PinDetail.TextureKeys.PinBoss);
	}

	public static UnityEngine.Vector2 GetBossOffset(this PinDetail pd)
	{
		return pd.GetTextureOffset(PinDetail.TextureKeys.PinBoss);
	}

	public static UnityEngine.Vector2 GetBossScale(this PinDetail pd)
	{
		return pd.GetTextureScale(PinDetail.TextureKeys.PinBoss);
	}

	public static string GetBackground(this PinDetail pd)
	{
		return pd.GetTextureName(PinDetail.TextureKeys.PinBackground);
	}

	public static UnityEngine.Vector2 GetBackgroundOffset(this PinDetail pd)
	{
		return pd.GetTextureOffset(PinDetail.TextureKeys.PinBackground);
	}

	public static UnityEngine.Vector2 GetPinTextureScale(this PinDetail pd)
	{
		return pd.GetTextureScale(PinDetail.TextureKeys.PinBackground);
	}

	public static UnityEngine.Vector2 GetCrossOffset(this PinDetail pd)
	{
		return pd.GetTextureOffset(PinDetail.TextureKeys.Cross);
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void Validate(this PinDetail pd)
	{
        List<string> possibleTextureKeys = Enum.GetNames(typeof(PinDetail.TextureKeys)).ToList<string>();
        IEnumerable<string> enumerable = from t in pd.Textures
                                         where !possibleTextureKeys.Contains(t.Key)
                                         select t.Key;
	}

	public static TextureDetail GetTextureDetails(this PinDetail pd, PinDetail.TextureKeys key)
	{
		TextureDetail result;
		pd.Textures.TryGetValue(key.ToString(), out result);
		return result;
	}

	private static string GetTextureName(this PinDetail pd, PinDetail.TextureKeys key)
	{
        TextureDetail textureDetails = pd.GetTextureDetails(key);
        return (textureDetails == null) ? string.Empty : textureDetails.GetName();
	}

    private static UnityEngine.Vector2 GetTextureOffset(this PinDetail pd, PinDetail.TextureKeys key)
    {
        TextureDetail textureDetails = pd.GetTextureDetails(key);
        return (textureDetails == null) ? UnityEngine.Vector2.zero : textureDetails.Offset.AsUnityVector2();
    }

    private static UnityEngine.Vector2 GetTextureScale(this PinDetail pd, PinDetail.TextureKeys key)
    {
        TextureDetail textureDetails = pd.GetTextureDetails(key);
        return (textureDetails == null) ? UnityEngine.Vector2.one : textureDetails.Scale.AsUnityVector2();
    }

    public static bool IsAutoStartPin(this PinDetail pd)
	{
		return pd.WorldTourScheduledPinInfo != null && pd.WorldTourScheduledPinInfo.AutoStart;
	}

	public static ScreenID GetLoadingScreen(this PinDetail pd)
	{
		VSDummy.eVSMode vSModeFromString = pd.GetVSModeFromString(pd.LoadingScreen);
		if (vSModeFromString == VSDummy.eVSMode.International || vSModeFromString == VSDummy.eVSMode.Multiplayer)
		{
			return ScreenID.VSDummy;
		}
		return ScreenID.Dummy;
	}

	public static bool ActivateVSLoadingScreen(this PinDetail pd)
	{
		ScreenID loadingScreen = pd.GetLoadingScreen();
		if (loadingScreen == ScreenID.VSDummy)
		{
			VSDummy.eVSMode vSModeFromString = pd.GetVSModeFromString(pd.LoadingScreen);
			VSDummy.BeginRace(null, vSModeFromString);
			return true;
		}
		return false;
	}

	private static VSDummy.eVSMode GetVSModeFromString(this PinDetail pd, string loadingScreen)
	{
		VSDummy.eVSMode result;
		if (EnumHelper.TryParse<VSDummy.eVSMode>(pd.LoadingScreen, out result))
		{
			return result;
		}
		return VSDummy.eVSMode.None;
	}

	public static PinDetail.PinType GetPinType(this PinDetail pd)
	{
		PinDetail.PinType result;
		if (!EnumHelper.TryParse<PinDetail.PinType>(pd.PinID, out result))
		{
			return PinDetail.PinType.NORMAL;
		}
		return result;
	}

	public static string GetConditionalString(this PinDetail pd, string key, IGameState gameState)
	{
		if (pd.ConditionallySelectedStrings.ContainsKey(key))
		{
			return pd.ConditionallySelectedStrings[key].GetText(gameState);
		}
		return null;
	}

	public static bool IsPartOfTimeline(this PinDetail pd)
	{
		return pd.CurrentTimelineDirection != PinDetail.TimelineDirection.None;
	}

	public static bool IsPreviousRaceInTimeline(this PinDetail pd)
	{
		return pd.CurrentTimelineDirection == PinDetail.TimelineDirection.Previous;
	}

	public static bool IsNextRaceInTimeline(this PinDetail pd)
	{
		return pd.CurrentTimelineDirection == PinDetail.TimelineDirection.Next;
	}

	public static void ApplyTemplate(this PinDetail pd, PinTemplate template)
	{
		pd.Position = template.Position;
		pd.PositionOffset = template.PositionOffset;
		pd.LoadingScreen = template.LoadingScreen;
		pd.ProgressIndicator = template.ProgressIndicator;
		pd.ShowSelectionArrow = template.ShowSelectionArrow;
		foreach (KeyValuePair<string, TextureDetail> current in template.Textures)
		{
			if (!pd.Textures.ContainsKey(current.Key))
			{
				pd.Textures[current.Key] = current.Value;
			}
		}
	}

	public static bool IsLocked(this PinDetail pd)
	{
		return pd.Lock.IsLocked(pd);
	}

	public static void Initialise(this PinDetail pd)
	{
		if (pd.ClickAction != null)
		{
			pd.ClickAction.Initialise();
		}
		pd.ConditionallySelectedStrings.Values.ForEachWithIndex(delegate(ConditionallySelectedString item, int index)
		{
			item.Initialise();
		});
	}
}
