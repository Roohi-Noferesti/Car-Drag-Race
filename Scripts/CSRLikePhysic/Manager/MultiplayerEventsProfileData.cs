using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class MultiplayerEventsProfileData
{
	private const string MULTIPLAYER_EVENTS_CURRENT = "cmpe";

	private const string MULTIPLAYER_EVENTS_ID = "mpei";

	private const string MULTIPLAYER_EVENTS_ENTERED = "mpee";

	private const string MULTIPLAYER_EVENTS_LAST_SEEN_PRIZE_PROGRESSION = "mpel";

	private const string MULTIPLAYER_EVENTS_CURRENT_PRIZE_PROGRESSION = "mpec";

	private const string MULTIPLAYER_EVENTS_PRIZES_AWARDED = "mpea";

	private const string MULTIPLAYER_EVENTS_RACES_COMPLETED = "mper";

	private const string MULTIPLAYER_EVENTS_RP_BONUSES = "merp";

	private const string MULTIPLAYER_EVENTS_RP_BONUS_EVENT_ID = "eid";

	private const string MULTIPLAYER_EVENTS_RP_BONUS_SPOT_PRIZE_ID = "sid";

	private const string MULTIPLAYER_EVENTS_RP_BONUS_START_TIME = "rpb";

	private MultiplayerEventProfileData CurrentEventData = new MultiplayerEventProfileData();

	private List<MultiplayerEventRPBonus> RPBonuses = new List<MultiplayerEventRPBonus>();

	private string DateTimeToHex(DateTime dt)
	{
		return dt.ToBinary().ToString("X");
	}

	private void FilterRPBonuses()
	{
		DateTime currentTime = DateTime.MinValue;
		if (ServerSynchronisedTime.Instance != null && ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			currentTime = ServerSynchronisedTime.Instance.GetDateTime();
		}
		this.RPBonuses = (from b in this.RPBonuses
		where b.IsValid(currentTime)
		select b).Distinct<MultiplayerEventRPBonus>().ToList<MultiplayerEventRPBonus>();
	}

	private bool TryHexToDateTime(string hexValue, out DateTime dt)
	{
		long dateData;
		if (long.TryParse(hexValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out dateData))
		{
			dt = DateTime.FromBinary(dateData);
			return true;
		}
		dt = DateTime.MinValue;
		return false;
	}

	private void SetBonusData(MultiplayerEventRPBonus bonusData, ref JsonDict jsonDict)
	{
		jsonDict.Set("eid", bonusData.EventID);
		jsonDict.Set("sid", bonusData.SpotPrizeID);
		jsonDict.Set("rpb", this.DateTimeToHex(bonusData.StartTime));
	}

	private void GetBonusData(JsonDict jsonDict, ref MultiplayerEventRPBonus bonusData)
	{
		string hexValue;
		jsonDict.TryGetValue("rpb", out hexValue);
		if (this.TryHexToDateTime(hexValue, out bonusData.StartTime))
		{
			jsonDict.TryGetValue("eid", out bonusData.EventID);
			jsonDict.TryGetValue("sid", out bonusData.SpotPrizeID);
		}
	}

	private void SetMultiplayerEventData(MultiplayerEventProfileData profileData, ref JsonDict jsonDict)
	{
		jsonDict.Set("mpei", profileData.ID);
		jsonDict.Set("mpee", profileData.Entered);
		jsonDict.Set("mpel", profileData.LastSeenPrizeProgression);
		jsonDict.Set("mpec", profileData.CurrentPrizeProgression);
		jsonDict.Set("mpea", profileData.SpotPrizesAwarded);
		jsonDict.Set("mper", profileData.RacesCompleted);
	}

	private void GetMultiplayerEventData(JsonDict jsonDict, ref MultiplayerEventProfileData profileData)
	{
		jsonDict.TryGetValue("mpei", out profileData.ID);
		jsonDict.TryGetValue("mpee", out profileData.Entered);
		jsonDict.TryGetValue("mpel", out profileData.LastSeenPrizeProgression);
		jsonDict.TryGetValue("mpec", out profileData.CurrentPrizeProgression);
		jsonDict.TryGetValue("mpea", out profileData.SpotPrizesAwarded);
		jsonDict.TryGetValue("mper", out profileData.RacesCompleted);
	}

	public void ToJson(ref JsonDict jsonDict)
	{
		jsonDict.SetObject<MultiplayerEventProfileData>("cmpe", this.CurrentEventData, new SetObjectDelegate<MultiplayerEventProfileData>(this.SetMultiplayerEventData));
		jsonDict.SetObjectList<MultiplayerEventRPBonus>("merp", this.RPBonuses, new SetObjectDelegate<MultiplayerEventRPBonus>(this.SetBonusData));
		this.FilterRPBonuses();
	}

	public void FromJson(ref JsonDict jsonDict)
	{
		if (!jsonDict.TryGetObject<MultiplayerEventProfileData>("cmpe", out this.CurrentEventData, new GetObjectDelegate<MultiplayerEventProfileData>(this.GetMultiplayerEventData)))
		{
			this.CurrentEventData = new MultiplayerEventProfileData();
		}
		if (!jsonDict.TryGetObjectList<MultiplayerEventRPBonus>("merp", out this.RPBonuses, new GetObjectDelegate<MultiplayerEventRPBonus>(this.GetBonusData)))
		{
			this.RPBonuses = new List<MultiplayerEventRPBonus>();
		}
	}

	public MultiplayerEventProfileData GetMultiplayerEventProfileData()
	{
		if (this.CurrentEventData == null)
		{
			this.CurrentEventData = new MultiplayerEventProfileData();
		}
		return this.CurrentEventData;
	}

	public void UpdateMultiplayerEventProfileData(int ID)
	{
		this.CurrentEventData = new MultiplayerEventProfileData();
		this.CurrentEventData.ID = ID;
	}

	public void SetEntered()
	{
		this.CurrentEventData.Entered = true;
	}

	public bool HasBeenEntered()
	{
		return this.CurrentEventData.Entered;
	}

	public void AddProgression(float quantity)
	{
		this.CurrentEventData.CurrentPrizeProgression += quantity;
	}

	public void SetProgression(float quantity)
	{
		this.CurrentEventData.CurrentPrizeProgression = quantity;
	}

	public float GetProgression()
	{
		return this.CurrentEventData.CurrentPrizeProgression;
	}

	public float GetLastSeenProgression()
	{
		return this.CurrentEventData.LastSeenPrizeProgression;
	}

	public void SetSeenProgression()
	{
		this.CurrentEventData.LastSeenPrizeProgression = this.CurrentEventData.CurrentPrizeProgression;
	}

	public void AddRPBonus(int eventID, int spotPrizeID, DateTime startTime)
	{
		this.RPBonuses.Add(new MultiplayerEventRPBonus
		{
			EventID = eventID,
			SpotPrizeID = spotPrizeID,
			StartTime = startTime
		});
		this.FilterRPBonuses();
	}

	public IEnumerable<MultiplayerEventRPBonus> GetRPBonuses()
	{
		this.FilterRPBonuses();
		return this.RPBonuses;
	}

	public void SetSpotPrizeAwarded(int spotPrizeID)
	{
		MultiplayerEventProfileData currentEventData = this.CurrentEventData;
		if (!currentEventData.SpotPrizesAwarded.Contains(spotPrizeID))
		{
			currentEventData.SpotPrizesAwarded.Add(spotPrizeID);
		}
	}

	public bool GetSpotPrizeAwarded(int spotPrizeID)
	{
		return this.CurrentEventData.SpotPrizesAwarded.Contains(spotPrizeID);
	}

	public void AddRacesCompleted(int races)
	{
		this.CurrentEventData.RacesCompleted += races;
	}

	public int GetRacesCompleted()
	{
		return this.CurrentEventData.RacesCompleted;
	}
}
