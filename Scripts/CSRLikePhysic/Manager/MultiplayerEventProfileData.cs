using System;
using System.Collections.Generic;

public class MultiplayerEventProfileData
{
	public int ID = -1;

	public bool Entered;

	public float LastSeenPrizeProgression;

	public float CurrentPrizeProgression;

	public List<int> SpotPrizesAwarded = new List<int>();

	public int RacesCompleted;
}
