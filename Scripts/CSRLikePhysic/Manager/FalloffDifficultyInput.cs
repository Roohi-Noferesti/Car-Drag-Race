using System;

public class FalloffDifficultyInput : DifficultyModifier
{
	private DifficultyEventType eventType;

	public FalloffDifficultyInput(DifficultySettings settings, DifficultyEventType type) : base(settings)
	{
		this.eventType = type;
	}

	public override void OnStreakStarted(ref float difficulty)
	{
		if (!ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			return;
		}
		DateTime dateTime = ServerSynchronisedTime.Instance.GetDateTime();
		float num = (float)this.getTimespanForMultiplayerMode(this.eventType, dateTime).TotalHours - base.Settings.FalloffIdleTimeHours;
		if (num > 0f)
		{
			float num2 = base.Settings.FalloffRateDifficultyPerHour * num;
			if (num2 < base.Settings.FalloffMaximum)
			{
				num2 = base.Settings.FalloffMaximum;
			}
			difficulty += num2;
		}
		this.saveLastPlayedToProfile(dateTime);
	}

	private TimeSpan getTimespanForMultiplayerMode(DifficultyEventType type, DateTime serverUTC)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		TimeSpan result = TimeSpan.MinValue;
		switch (this.eventType)
		{
		case DifficultyEventType.RaceTheWorld:
			result = serverUTC - activeProfile.LastPlayedMultiplayer;
			break;
		case DifficultyEventType.EliteClub:
			result = serverUTC - activeProfile.LastPlayedEliteClub;
			break;
		case DifficultyEventType.MultiplayerEvent:
			result = serverUTC - activeProfile.LastPlayedRaceTheWorldWorldTour;
			break;
		}
		return result;
	}

	private void saveLastPlayedToProfile(DateTime serverUTC)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		switch (this.eventType)
		{
		case DifficultyEventType.RaceTheWorld:
			activeProfile.LastPlayedMultiplayer = serverUTC;
			break;
		case DifficultyEventType.EliteClub:
			activeProfile.LastPlayedEliteClub = serverUTC;
			break;
		case DifficultyEventType.MultiplayerEvent:
			activeProfile.LastPlayedRaceTheWorldWorldTour = serverUTC;
			break;
		}
	}
}
