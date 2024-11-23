using DataSerialization;
using System;
using UnityEngine;

public static class ScheduledPinExtensions
{
	public static PopupData GetOnPostRaceLostPopup(this ScheduledPin sp)
	{
		if (string.IsNullOrEmpty(sp.OnPostRaceLostPopupID))
		{
			return PopupData.CreateDontShowPopupData();
		}
		return TierXManager.Instance.GetPopupDataWithID(sp.OnPostRaceLostPopupID);
	}

	public static PopupData GetOnWorkshopPopup(this ScheduledPin sp)
	{
		if (string.IsNullOrEmpty(sp.OnWorkshopPopupID))
		{
			return PopupData.CreateDontShowPopupData();
		}
		return TierXManager.Instance.GetPopupDataWithID(sp.OnWorkshopPopupID);
	}

	public static PopupData GetOnMapPinTapPopup(this ScheduledPin sp)
	{
		if (string.IsNullOrEmpty(sp.OnMapPinTapPopupID))
		{
			return PopupData.CreateDontShowPopupData();
		}
		return TierXManager.Instance.GetPopupDataWithID(sp.OnMapPinTapPopupID);
	}

    public static SchedulePinAnimationSelectionType GetAppearAnimationSelectionTypeEnum(this ScheduledPin sp)
    {
        return EnumHelper.FromString<SchedulePinAnimationSelectionType>(sp.AppearAnimationSelectionTypeString);
    }

	public static int GetAutoSelectPriority(this ScheduledPin sp, IGameState gameState)
	{
		int result = sp.DefaultAutoSelectPriority;
		ScheduledPin currentEventScheduledPin = gameState.GetCurrentEventScheduledPin();
		if (currentEventScheduledPin != null)
		{
			PinSequence rootParentSequence = currentEventScheduledPin.GetRootParentSequence();
			PinSequence rootParentSequence2 = sp.GetRootParentSequence();
			bool flag = gameState.IsPinWon(gameState.CurrentWorldTourThemeID, sp) && !rootParentSequence.Repeatable;
			if (flag)
			{
				result = -100;
			}
			else if (rootParentSequence.ID == rootParentSequence2.ID)
			{
				result = sp.LastSequenceAutoSelectPriority;
			}
		}
		return result;
	}

	public static int GetPinIndex(this ScheduledPin sp)
	{
		return sp.ParentSequence.Pins.FindIndex((ScheduledPin p) => p.ID == sp.ID);
	}

	public static PinSequence GetRootParentSequence(this ScheduledPin sp)
	{
		if (sp.ReferrerPin != null)
		{
			return sp.ReferrerPin.GetRootParentSequence();
		}
		return sp.ParentSequence;
	}

	public static bool IsEligible(this ScheduledPin sp, IGameState gameState)
	{
		return sp.Requirements.IsEligible(gameState);
	}

	public static void Initialise(this ScheduledPin sp)
	{
		sp.Requirements.Initialise();
		sp.Narrative.Initialise();
	}

	public static string SelectAnimationIn(this ScheduledPin sp)
	{
		int count = sp.AppearAnimations.Count;
		if (count == 0)
		{
			return string.Empty;
		}
		if (count == 1 || sp.GetAppearAnimationSelectionTypeEnum() == SchedulePinAnimationSelectionType.SELECT_FIRST)
		{
			return sp.AppearAnimations[0];
		}
		return sp.AppearAnimations[UnityEngine.Random.Range(0, count - 1)];
	}

    public static ScreenID GetScreenToPushAfterResult(this ScheduledPin sp)
    {
        return EnumHelper.FromString<ScreenID>(sp.NextScreen);
    }

    public static void SetScreenToPushAfterResult(this ScheduledPin sp, ScreenID screenID)
    {
        sp.NextScreen = screenID.ToString();
    }
}
