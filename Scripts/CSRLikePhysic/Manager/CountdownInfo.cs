using System;

public class CountdownInfo
{
	private const int DisableSeasonCode = -1;

	private bool _SeasonStarted;

	private DateTime _timeSeasonFinishes = DateTime.MinValue;

	private TimeSpan _allowDriftOfUpTo = new TimeSpan(0, 5, 0);

	public bool HasSeasonStarted
	{
		get
		{
			return this._SeasonStarted;
		}
	}

	public CountdownInfo(DateTime currentTime, int SecondsFromServer)
	{
		this._SeasonStarted = (SecondsFromServer != -1);
		this._timeSeasonFinishes = GTDateTime.Now.AddSeconds((double)SecondsFromServer);
	}

	public int GetSecondsRemainingForEvent()
	{
		if (this._timeSeasonFinishes == DateTime.MinValue)
		{
			return 0;
		}
        if (this._timeSeasonFinishes <= GTDateTime.Now)
		{
			return 0;
		}
        TimeSpan timeSpan = this._timeSeasonFinishes - GTDateTime.Now;
		return (timeSpan.TotalSeconds <= 0.0) ? 0 : ((int)timeSpan.TotalSeconds);
	}

	public void UpdateRemainingTime(int seconds)
	{
		if (this._SeasonStarted)
		{
            DateTime dateTime = GTDateTime.Now.AddSeconds((double)seconds);
			if (dateTime < this._timeSeasonFinishes - this._allowDriftOfUpTo || dateTime > this._timeSeasonFinishes + this._allowDriftOfUpTo)
			{
				this._timeSeasonFinishes = dateTime;
			}
		}
	}
}
