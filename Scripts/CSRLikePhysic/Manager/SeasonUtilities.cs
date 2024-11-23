using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;

public static class SeasonUtilities
{
	public const int UnsetSeasonValue = -1;

	public static bool FakingDataOutOfDate;

	public static bool IsFakingDataOutOfDate()
	{
		return false;
	}

	public static bool IsDataOutOfDate()
	{
		if (SeasonUtilities.IsFakingDataOutOfDate())
		{
			return true;
		}
		List<int> allEventIDs = SeasonServerDatabase.Instance.GetAllEventIDs();
		if (!allEventIDs.TrueForAll(new Predicate<int>(GameDatabase.Instance.SeasonEvents.ContainsEvent)))
		{
			return true;
		}
		List<int> allFinishedEventPrizeIDs = SeasonServerDatabase.Instance.GetAllFinishedEventPrizeIDs();
		return !allFinishedEventPrizeIDs.TrueForAll(new Predicate<int>(GameDatabase.Instance.SeasonPrizes.ContainsPrize)) && false;
	}

	public static bool IsCarPreviousSeasonPrize(CarInfo car)
	{
		if (!GameDatabase.Instance.SeasonEvents.IsSeasonFutureEvent(car.CarUnlocksInSeason))
		{
			return false;
		}
		List<int> allFinishedEventPrizeIDs = SeasonServerDatabase.Instance.GetAllFinishedEventPrizeIDs();
		List<SeasonPrizeMetadata> list = (from id in allFinishedEventPrizeIDs
		select GameDatabase.Instance.SeasonPrizes.GetPrize(id)).ToList<SeasonPrizeMetadata>();
		for (int i = 0; i < list.Count; i++)
		{
			SeasonPrizeMetadata seasonPrizeMetadata = list[i];
			if (seasonPrizeMetadata != null && seasonPrizeMetadata.Type == SeasonPrizeMetadata.ePrizeType.Car && seasonPrizeMetadata.Data.Equals(car.Key))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CanCarBeUnlockedBySeason(CarInfo car)
	{
		return SeasonUtilities.HasCarBeenAvailableInShowroomForNSeasons(car, 0);
	}

	public static bool ValidForRTWAndDoWeHaveStatusAndStandings()
	{
		return RTWStatusManager.NetworkStateValidToEnterRTW() && SeasonServerDatabase.Instance.DoWeHaveStatusAndStandings();
	}

	public static bool HasCarBeenAvailableInShowroomForNSeasons(CarInfo car, int seasons)
	{
		int carUnlocksInSeason = car.CarUnlocksInSeason;
		if (carUnlocksInSeason > 0 && GameDatabase.Instance.SeasonEvents.IsSeasonFutureEvent(carUnlocksInSeason + seasons))
		{
			return false;
		}
		if (car.UnlockDependencies.Count > 0)
		{
			foreach (string current in car.UnlockDependencies)
			{
				if (!current.Equals(car.Key))
				{
					CarInfo car2 = CarDatabase.Instance.GetCar(current);
					if (car2 != null)
					{
						if (!SeasonUtilities.HasCarBeenAvailableInShowroomForNSeasons(car2, seasons))
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		return true;
	}

	public static bool IsPinUnlockedByCurrentSeason(int themeSeasonToUnlock)
	{
		int mostRecentActiveSeasonEventID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonEventID();
		if (mostRecentActiveSeasonEventID == -1)
		{
			return false;
		}
		SeasonEventMetadata @event = GameDatabase.Instance.SeasonEvents.GetEvent(mostRecentActiveSeasonEventID);
		return !GameDatabase.Instance.SeasonEvents.IsSeasonFutureEvent(themeSeasonToUnlock);
	}

	public static void ShowSeasonEndedPopUp(PopUpButtonAction confirmAction)
	{
		int mostRecentFinishedSeasonStatusLeaderboardID = SeasonServerDatabase.Instance.getMostRecentFinishedSeasonStatusLeaderboardID();
		SeasonEventMetadata eventForLeaderboard = SeasonServerDatabase.Instance.GetEventForLeaderboard(mostRecentFinishedSeasonStatusLeaderboardID);
		RtwLeaderboardStatusItem leaderboardStatusForUpcommingSeason = SeasonServerDatabase.Instance.GetLeaderboardStatusForUpcommingSeason();
		SeasonEventMetadata seasonEventMetadata = (leaderboardStatusForUpcommingSeason != null) ? SeasonServerDatabase.Instance.GetEventForLeaderboard(leaderboardStatusForUpcommingSeason.leaderboard_id) : null;
		PopUp popup;
		if (eventForLeaderboard != null && seasonEventMetadata != null)
		{
			popup = new PopUp
			{
				Title = "TEXT_SEASON_ENDED_NO_DATA",
				BodyText = string.Format(LocalizationManager.GetTranslation("TEXT_SEASON_END_WAITING_RESULTS"), LocalizationManager.GetTranslation(eventForLeaderboard.EventTitle), LocalizationManager.GetTranslation(seasonEventMetadata.EventTitle)),
				BodyAlreadyTranslated = true,
				ConfirmText = "TEXT_BUTTON_OK",
				ConfirmAction = confirmAction,
				IsBig = true,
				GraphicPath = PopUpManager.Instance.graphics_raceOfficialPrefab,
				ImageCaption = "TEXT_NAME_RACE_OFFICIAL"
			};
		}
		else
		{
			popup = new PopUp
			{
				Title = "TEXT_SEASON_ENDED_NO_DATA",
				BodyText = LocalizationManager.GetTranslation("TEXT_SEASON_END_WAITING_RESULTS_NO_DATA"),
				BodyAlreadyTranslated = true,
				ConfirmText = "TEXT_BUTTON_OK",
				ConfirmAction = confirmAction,
				IsBig = true,
				GraphicPath = PopUpManager.Instance.graphics_raceOfficialPrefab,
				ImageCaption = "TEXT_NAME_RACE_OFFICIAL"
			};
		}
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public static void ShowDataOutOfDatePopUp(PopUpButtonAction confirmAction)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUP_DATA_OUTOFDATE",
			BodyText = "TEXT_POPUP_DATA_OUTOFDATE_BODY",
			ConfirmText = "TEXT_BUTTON_OK",
			ConfirmAction = confirmAction,
			ShouldCoverNavBar = true
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
	}
}
