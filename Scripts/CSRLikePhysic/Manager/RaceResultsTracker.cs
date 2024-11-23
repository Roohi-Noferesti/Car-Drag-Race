public static class RaceResultsTracker
{
	private static RaceResultsData _you;

	private static string _carUsedToSetBest = string.Empty;

	private static bool _wasBestHalfMile;

	private static bool _wasDailyRace;

	public static RaceResultsData Them
	{
		get;
		set;
	}

	public static RaceResultsData Last
	{
		get;
		set;
	}

	public static RaceResultsData Best
	{
		get;
		set;
	}

	public static RaceResultsData You
	{
		get
		{
			return _you;
		}
		set
		{
			if (value == null)
			{
				Best = null;
				Last = null;
				_you = null;
				return;
			}
			NullResultTrackingDataIfNeeded();
			Last = You;
			if (_you != null && Best == null)
			{
				Best = (_you.Clone() as RaceResultsData);
			}
			if (Best != null)
			{
				if (value.Nought60Time < Best.Nought60Time && value.Nought60Time > 0f)
				{
					Best.Nought60Time = value.Nought60Time;
				}
				if (value.Nought100Time < Best.Nought100Time && value.Nought100Time > 0f)
				{
					Best.Nought100Time = value.Nought100Time;
				}
				if (value.RaceTime < Best.RaceTime)
				{
					Best.RaceTime = value.RaceTime;
				}
				if (value.SpeedWhenCrossingFinishLine > Best.SpeedWhenCrossingFinishLine)
				{
					Best.SpeedWhenCrossingFinishLine = value.SpeedWhenCrossingFinishLine;
				}
			}
			_you = value;
		}
	}

	public static RaceResultsTrackerState GetState()
	{
		return new RaceResultsTrackerState
		{
			You = You,
			Them = Them,
			Best = Best,
			Last = Last
		};
	}

	private static void NullResultTrackingDataIfNeeded()
	{
	    if (PlayerProfileManager.Instance.ActiveProfile==null)
	        return;
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		bool flag = false;
		if (currentEvent == null)
		{
			return;
		}
		if (currentEvent.IsHalfMile && !_wasBestHalfMile)
		{
			flag = true;
		}
		if (PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey != _carUsedToSetBest)
		{
			flag = true;
		}
		if (RaceEventInfo.Instance.IsDailyBattleEvent && !_wasDailyRace)
		{
			flag = true;
		}
		if (flag)
		{
			NullTrackingData();
			_carUsedToSetBest = PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey;
			_wasBestHalfMile = currentEvent.IsHalfMile;
			_wasDailyRace = RaceEventInfo.Instance.IsDailyBattleEvent;
		}
	}

	public static void NullTrackingData()
	{
		Best = null;
		Last = null;
		_you = null;
	}
}
