using System;
using System.Collections.Generic;
using I2.Loc;

public static class SeasonCountdownManager
{
	private static Dictionary<int, CountdownInfo> listOfEventTimes = new Dictionary<int, CountdownInfo>();

	public static void ConnectToSeasonServerDatabase()
	{
		SeasonServerDatabase.Instance.ServerLeaderboardStatusesUpdated += new SeasonServerDatabase.SeasonServerDataUpdated(SeasonCountdownManager.newServerDataArrived);
	}

	public static void DisconnectFromSeasonServerDatabase()
	{
		SeasonServerDatabase.Instance.ServerLeaderboardStatusesUpdated -= new SeasonServerDatabase.SeasonServerDataUpdated(SeasonCountdownManager.newServerDataArrived);
	}

	public static void UpdateEventCountdown(int eventID, int timeRemaining)
	{
		if (SeasonCountdownManager.listOfEventTimes.ContainsKey(eventID))
		{
			SeasonCountdownManager.listOfEventTimes[eventID].UpdateRemainingTime(timeRemaining);
		}
		else
		{
			SeasonCountdownManager.listOfEventTimes.Add(eventID, new CountdownInfo(GTDateTime.Now, timeRemaining));
		}
	}

	public static void newServerDataArrived(SeasonServerDatabase updatedDatabase)
	{
		List<RtwLeaderboardStatusItem> allLeaderbordsWithCountdowns = updatedDatabase.GetAllLeaderbordsWithCountdowns();
		allLeaderbordsWithCountdowns.ForEach(delegate(RtwLeaderboardStatusItem p)
		{
			SeasonCountdownManager.UpdateEventCountdown(p.event_id, p.finishing_in);
		});
	}

	public static bool HasEventStarted(int eventID)
	{
		return SeasonCountdownManager.listOfEventTimes.ContainsKey(eventID) && SeasonCountdownManager.listOfEventTimes[eventID].HasSeasonStarted;
	}

	public static bool HasEventEnded(int eventID)
	{
		if (SeasonCountdownManager.listOfEventTimes.ContainsKey(eventID))
		{
			CountdownInfo countdownInfo = SeasonCountdownManager.listOfEventTimes[eventID];
			if (countdownInfo.HasSeasonStarted && countdownInfo.GetSecondsRemainingForEvent() <= 0)
			{
				return true;
			}
		}
		return false;
	}

	public static string GetRemainingTimeString(int eventID, bool addRemaining = false)
	{
		if (!SeasonCountdownManager.listOfEventTimes.ContainsKey(eventID))
		{
			return string.Empty;
		}
		CountdownInfo countdownInfo = SeasonCountdownManager.listOfEventTimes[eventID];
		if (!countdownInfo.HasSeasonStarted)
		{
			return string.Empty;
		}
		int secondsRemainingForEvent = countdownInfo.GetSecondsRemainingForEvent();
		if (secondsRemainingForEvent <= 0)
		{
			return LocalizationManager.GetTranslation((!addRemaining) ? "TEXT_SEASON_ENDED_WT_THEME" : "TEXT_SEASON_ENDED");
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)secondsRemainingForEvent);
		int num = timeSpan.Hours;
		if (timeSpan.Days > 1)
		{
            string format = LocalizationManager.GetTranslation((!addRemaining) ? "TEXT_SEASON_DAYS_REMAINING_WT_THEME" : "TEXT_SEASON_DAYS_REMAINING");
			return string.Format(format, timeSpan.Days);
		}
		if (timeSpan.Days > 0)
		{
			num += 24;
		}
		string text = string.Format("{0:D2}:{1:D2}:{2:D2}", num, timeSpan.Minutes, timeSpan.Seconds);
		string text2 = text.TrimStart(new char[]
		{
			'0'
		});
		return text2.TrimStart(new char[]
		{
			':'
		});
	}
}
