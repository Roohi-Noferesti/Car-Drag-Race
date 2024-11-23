using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using I2.Loc;

public static class MultiplayerEvent
{
	public static class Saved
	{
		public static MultiplayerEventData Data
		{
			get
			{
				return GameDatabase.Instance.MultiplayerEvents.GetEventByID(MultiplayerEvent.Saved.ProfileData.GetMultiplayerEventID());
			}
		}

		private static PlayerProfile ProfileData
		{
			get
			{
				return PlayerProfileManager.Instance.ActiveProfile;
			}
		}

		public static void SetSpotPrizeAwarded(int spotPrizeID)
		{
			MultiplayerEvent.Saved.ProfileData.SetMultiplayerEventSpotPrizeAwarded(spotPrizeID);
		}

		public static bool GetSpotPrizeAwarded(int spotPrizeID)
		{
			return MultiplayerEvent.Saved.ProfileData.GetMultiplayerEventSpotPrizeAwarded(spotPrizeID);
		}

		public static float GetLastSeenProgression()
		{
			return MultiplayerEvent.Saved.ProfileData.GetLastSeenMultiplayerEventProgression();
		}

		public static void SetEntered()
		{
			MultiplayerEvent.Saved.ProfileData.SetMultiplayerEventEntered();
		}

		public static bool HasBeenEntered()
		{
			return MultiplayerEvent.Saved.ProfileData.HasMultiplayerEventBeenEntered();
		}

		public static void AddProgression(float quantity)
		{
			MultiplayerEvent.Saved.ProfileData.AddMultiplayerEventProgression(quantity);
		}

		public static void SetProgression(float quantity)
		{
			MultiplayerEvent.Saved.ProfileData.SetMultiplayerEventProgression(quantity);
		}

		public static float GetProgression()
		{
			return MultiplayerEvent.Saved.ProfileData.GetMultiplayerEventProgression();
		}

		public static void SetSeenProgression()
		{
			MultiplayerEvent.Saved.ProfileData.SetSeenMultiplayerEventProgression();
		}

		public static void AddRacesCompleted(int races)
		{
			MultiplayerEvent.Saved.ProfileData.AddMultiplayerEventRacesCompleted(races);
		}

		public static int GetRacesCompleted()
		{
			return MultiplayerEvent.Saved.ProfileData.GetMultiplayerEventRacesCompleted();
		}

		private static bool HasPrizesToAward()
		{
			return MultiplayerEvent.Saved.GetMissingSpotPrizes(new Func<float>(MultiplayerEvent.Saved.GetProgression)).Count<SpotPrizeData>() > 0;
		}

		[DebuggerHidden]
		public static IEnumerable<SpotPrizeData> GetMissingSpotPrizes(Func<float> progressionMethod)
		{
		    //MultiplayerEvent.Saved.<GetMissingSpotPrizes>c__Iterator12 <GetMissingSpotPrizes>c__Iterator = new MultiplayerEvent.Saved.<GetMissingSpotPrizes>c__Iterator12();
            //<GetMissingSpotPrizes>c__Iterator.progressionMethod = progressionMethod;
            //<GetMissingSpotPrizes>c__Iterator.<$>progressionMethod = progressionMethod;
            //MultiplayerEvent.Saved.<GetMissingSpotPrizes>c__Iterator12 expr_15 = <GetMissingSpotPrizes>c__Iterator;
            //expr_15.$PC = -2;
            //return expr_15;
		    return null;
		}

	    public static void AwardMissingPrizesIfEnded()
		{
			if (MultiplayerEvent.Saved.Data != null && !MultiplayerEvent.Saved.Data.IsActive() && MultiplayerEvent.Saved.HasPrizesToAward())
			{
				PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_EVENT_ENDED_TITLE", string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_MULTIPLAYER_EVENT_AWARDS_BODY"), LocalizationManager.GetTranslation(MultiplayerEvent.Saved.Data.GetTitle())), delegate
				{
					Queue<SpotPrizeData> awardQueue = new Queue<SpotPrizeData>();
					MultiplayerEvent.Saved.GetMissingSpotPrizes(new Func<float>(MultiplayerEvent.Saved.GetProgression)).ForEachWithIndex(delegate(SpotPrizeData e, int idx)
					{
						awardQueue.Enqueue(e);
					});
					MultiplayerEvent.Saved.Data.AwardNextSpotPrize(awardQueue);
				}, true);
			}
		}
	}

	public static class Next
	{
		public static MultiplayerEventData Data
		{
			get
			{
				return GameDatabase.Instance.MultiplayerEvents.GetNextEvent();
			}
		}
	}

	public static class Actual
	{
		public static MultiplayerEventData Data
		{
			get
			{
				return GameDatabase.Instance.MultiplayerEvents.GetActiveEvent();
			}
		}

		public static void SaveToProfile()
		{
			if (MultiplayerEvent.Actual.Data == null)
			{
				return;
			}
			if (MultiplayerEvent.Saved.Data != null)
			{
				if (MultiplayerEvent.Actual.Data.ID == MultiplayerEvent.Saved.Data.ID)
				{
					return;
				}
				IEnumerable<SpotPrizeData> missingSpotPrizes = MultiplayerEvent.Saved.GetMissingSpotPrizes(new Func<float>(MultiplayerEvent.Saved.GetProgression));
				if (missingSpotPrizes.Any<SpotPrizeData>())
				{
					return;
				}
			}
			PlayerProfileManager.Instance.ActiveProfile.UpdateMultiplayerEventProfileData(MultiplayerEvent.Actual.Data.ID);
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}
	}

	public static bool ShouldShowEventBreadcrumbs()
	{
		return MultiplayerEvent.Actual.Data != null && (MultiplayerEvent.Saved.Data == null || MultiplayerEvent.Actual.Data.ID != MultiplayerEvent.Saved.Data.ID);
	}
}
