public static class RaceTimesManager
{
	private const string DEFAULT_TIME_FORMAT_STRING = "TEXT_UNITS_TIME_SECONDS_WITH_MS";

	private const string EXTRA_PRECISION_TIME_FORMAT_STRING = "TEXT_UNITS_TIME_SECONDS_WITH_5_DECIMAL_DIGITS";

	public static bool IsPlayerRaceTimeWinner(float playerTime, ref float opponentTime, RaceTimeType timeType, ForceEndRaceState forceResult = ForceEndRaceState.DONTCARE)
	{
		if (forceResult == ForceEndRaceState.FORCEWIN)
			return true;
		else if (forceResult == ForceEndRaceState.FORCELOSE)
			return false;

			if (RaceEventInfo.Instance.CurrentEvent != null && (RelayManager.IsCurrentEventRelay() || RaceEventInfo.Instance.CurrentEvent.AutoHeadstart))
        {
            float num = RaceEventInfo.Instance.CurrentEvent.GetTimeDifference();
            num += opponentTime - playerTime;
            return num >= 0f;
        }
        bool result = playerTime <= opponentTime;
        if (timeType == RaceTimeType.RYF)
        {
            return result;
        }
        float num2 = playerTime - opponentTime;
        if (num2 >= 0f && num2 < 0.001f)
        {
            opponentTime = playerTime;
            return true;
        }
        return result;
	}

	public static string GetFriendRaceTimeStringWithOpponent(float time, float opponentTime, string carDBKey, bool checkForExtraPrecision = false, bool skipGoldStarExtraPrecision = false)
	{
	    return null;
	    //string friendRaceTimeFormatString = RaceTimesManager.GetFriendRaceTimeFormatString(time, carDBKey, skipGoldStarExtraPrecision);
	    //string text = string.Format(LocalizationManager.GetTranslation(friendRaceTimeFormatString), time);
	    //if (friendRaceTimeFormatString == "TEXT_UNITS_TIME_SECONDS_WITH_MS" && checkForExtraPrecision)
	    //{
	    //    string b = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), opponentTime);
	    //    if (text == b)
	    //    {
	    //        text = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_5_DECIMAL_DIGITS"), time);
	    //    }
	    //}
	    //return text;
	}

	public static string GetRaceTimeStringForMultiplayerRaces(float time)
	{
	    return time.ToString();//string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), time);
	}

	public static string GetRaceTimeStringForSingleplayerRaces(float time)
	{
	    return time.ToString();//string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), time);
	}

	public static string GetFriendRaceTimeFormatString(float time, string carDBKey, bool skipGoldStarExtraPrecision = false)
	{
        //if (!skipGoldStarExtraPrecision && StarsManager.GetStarForTime(carDBKey, time) == StarType.GOLD)
        //{
        //    return "TEXT_UNITS_TIME_SECONDS_WITH_5_DECIMAL_DIGITS";
        //}
		return "TEXT_UNITS_TIME_SECONDS_WITH_MS";
	}
}
