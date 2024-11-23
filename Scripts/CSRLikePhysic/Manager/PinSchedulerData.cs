using DataSerialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

[Serializable]
public class PinSchedulerData
{
    [Serializable]
    public class PinSchedulerThemeData
	{
		public string ThemeID = string.Empty;

		public int RacesComplete;

		public int SeenCount;

		public List<PinSchedulerData.PinSchedulerSequenceData> SequenceData = new List<PinSchedulerData.PinSchedulerSequenceData>();

		public List<ScheduledPinLifetimeData> LifetimeData = new List<ScheduledPinLifetimeData>();

		public int CompletionLevel;

		public string LastSequenceRaced = string.Empty;

		public ThemeCompletionLevel CompletionLevelEnum
		{
			get
			{
				return (ThemeCompletionLevel)this.CompletionLevel;
			}
		}

		public void IncrementRacesComplete()
		{
			this.RacesComplete++;
		}

		public void IncrementSeenCount()
		{
			this.SeenCount++;
		}

		public void IncrementCompletionLevel()
		{
			if (this.CompletionLevelEnum == ThemeCompletionLevel.LEVEL_MAX)
			{
			}
			this.CompletionLevel++;
		}

		public void SetCompletionLevel(ThemeCompletionLevel level)
		{
			this.CompletionLevel = (int)level;
		}

		public PinSchedulerData.PinSchedulerSequenceData GetSpecificSequenceData(string sequenceID)
		{
			PinSchedulerData.PinSchedulerSequenceData pinSchedulerSequenceData = this.SequenceData.Find((PinSchedulerData.PinSchedulerSequenceData s) => s.ID == sequenceID.GetCrossPlatformHashCode());
			if (pinSchedulerSequenceData == null)
			{
				pinSchedulerSequenceData = new PinSchedulerData.PinSchedulerSequenceData(sequenceID);
				this.SequenceData.Add(pinSchedulerSequenceData);
			}
			return pinSchedulerSequenceData;
		}

		public ScheduledPinLifetimeData GetLifeTimeData(string lifetimeGroup)
		{
			ScheduledPinLifetimeData scheduledPinLifetimeData = this.LifetimeData.Find((ScheduledPinLifetimeData s) => s.LifetimeGroup == lifetimeGroup);
			if (scheduledPinLifetimeData == null)
			{
				scheduledPinLifetimeData = new ScheduledPinLifetimeData();
				scheduledPinLifetimeData.LifetimeGroup = lifetimeGroup;
				this.LifetimeData.Add(scheduledPinLifetimeData);
			}
			return scheduledPinLifetimeData;
		}

		public void UpdateLifetimeFirstShownAt(ScheduledPin pin)
		{
			if (!string.IsNullOrEmpty(pin.LifetimeGroup))
			{
				ScheduledPinLifetimeData lifeTimeData = this.GetLifeTimeData(pin.LifetimeGroup);
				lifeTimeData.UpdateRaceCountFirstShownAt(this.RacesComplete);
			}
		}

		public void UpdateLifetimeLastRacedAt(ScheduledPin pin)
		{
			if (!string.IsNullOrEmpty(pin.LifetimeGroup))
			{
				ScheduledPinLifetimeData lifeTimeData = this.GetLifeTimeData(pin.LifetimeGroup);
				lifeTimeData.UpdateRaceCountLastRacedAt(this.RacesComplete);
			}
		}

		public void ResetLifeCount(List<ScheduledPin> filteredPins)
		{
			foreach (ScheduledPinLifetimeData current in from l in this.LifetimeData
			where !filteredPins.Any((ScheduledPin f) => f.LifetimeGroup == l.LifetimeGroup)
			select l)
			{
				current.ResetRaceCountFirstShownAt();
			}
		}

		public string AsProgressMetricsParameter()
		{
			object[] expr_06 = new object[6];
			expr_06[0] = this.ThemeID;
			expr_06[1] = ":";
			expr_06[2] = this.RacesComplete;
			expr_06[3] = ":[";
			expr_06[4] = string.Join(",", (from x in this.SequenceData
			where x.HasRecordedProgress() && x.ReleventToProgression()
			select x.AsProgressMetricsParameter()).ToArray<string>());
			expr_06[5] = "]";
			return string.Concat(expr_06);
		}
	}

    [Serializable]
    public class PinSchedulerSequenceData
	{
		private static readonly List<ISequenceToMetricsIDConverter> SequnceToMetricIDConverter = new List<ISequenceToMetricsIDConverter>
		{
			new LadderSequenceToMetricsIDConverter(),
			new InternationalSequenceToMetricsIDConverter(),
			new FinalsSequenceToMetricsIDConverter()
		};

		public int ID;

		public int LastWonLevel = -1;

		public int LastLostLevel = -1;

		public int LastSeenLevel = -1;

		public int LastRacedLevel = -1;

		public int EventCount = -1;

		public string MetricsID = string.Empty;

		public List<PinSchedulerData.PinSchedulerEventData> SpecificEventData = new List<PinSchedulerData.PinSchedulerEventData>();

		public PinSchedulerSequenceData(string sequenceID)
		{
			this.ID = sequenceID.GetCrossPlatformHashCode();
			string text = (from mp in PinSchedulerData.PinSchedulerSequenceData.SequnceToMetricIDConverter
			select mp.ConvertToMetricsID(sequenceID)).FirstOrDefault((string metricID) => !string.IsNullOrEmpty(metricID));
			this.MetricsID = (text ?? string.Empty);
		}

		public PinSchedulerSequenceData()
		{
		}

		public void SetPinAsRaced(ScheduledPin pin, bool won)
		{
			this.EventCount = pin.ParentSequence.Pins.Count;
			PinSchedulerData.PinSchedulerEventData eventData = this.GetEventData(pin.ID);
			this.LastRacedLevel = pin.Level;
			if (won)
			{
				eventData.WonCount++;
				this.LastWonLevel = pin.Level;
			}
			else
			{
				eventData.LostCount++;
				this.LastLostLevel = pin.Level;
			}
		}

		public void SetChoiceLevel(ScheduledPin pin, int level)
		{
			PinSchedulerData.PinSchedulerEventData eventData = this.GetEventData(pin.ID);
			eventData.ChoiceLevel = level;
		}

		public void SetSeen(ScheduledPin pin)
		{
			this.LastSeenLevel = pin.Level;
			this.EventCount = pin.ParentSequence.Pins.Count;
		}

		public PinSchedulerData.PinSchedulerEventData GetEventData(string ID)
		{
			PinSchedulerData.PinSchedulerEventData pinSchedulerEventData = this.SpecificEventData.Find((PinSchedulerData.PinSchedulerEventData ed) => ed.ID == ID.GetCrossPlatformHashCode());
			if (pinSchedulerEventData == null)
			{
				pinSchedulerEventData = new PinSchedulerData.PinSchedulerEventData();
				pinSchedulerEventData.ID = ID.GetCrossPlatformHashCode();
				this.SpecificEventData.Add(pinSchedulerEventData);
			}
			return pinSchedulerEventData;
		}

		public bool HasRecordedProgress()
		{
			return this.LastSeenLevel > -1;
		}

		public bool ReleventToProgression()
		{
			return !string.IsNullOrEmpty(this.MetricsID);
		}

		public string AsProgressMetricsParameter()
		{
			return this.MetricsID + ":" + (this.LastWonLevel + 1).ToString();
		}
	}

    [Serializable]
    public class PinSchedulerEventData
	{
		public int ID;

		public int LostCount;

		public int WonCount;

		public int ChoiceLevel = -1;

		public bool HasRaced()
		{
			return this.LostCount > 0 || this.WonCount > 0;
		}
	}

	private const string THEME_DATA = "pstd";

	private const string THEME_ID = "id";

	private const string THEME_RACE_COUNT = "rc";

	private const string THEME_SEEN = "ts";

	private const string THEME_FTUE_LEVEL = "fl";

	private const string THEME_LAST_SEQUENCE_RACED = "lsr";

	private const string SEQUENCE_DATA = "sd";

	private const string LIFETIME_DATA = "ld";

	private const string LIFETIME_GROUP = "g";

	private const string LIFETIME_SHOWN_COUNT = "sc";

	private const string LIFETIME_RACED_COUNT = "rc";

	private const string SEQUENCE_ID = "id";

	private const string SEQUENCE_METRICS_ID = "mi";

	private const string SEQUENCE_LAST_WON = "lw";

	private const string SEQUENCE_LAST_LOST = "ll";

	private const string SEQUENCE_LAST_RACED = "lr";

	private const string SEQUENCE_LAST_SEEN = "ls";

	private const string SEQUENCE_EVENT_COUNT = "ec";

	private const string SPECIFIC_EVENT_DATA = "ed";

	private const string SPECIFIC_EVENT_ID = "id";

	private const string SPECIFIC_EVENT_WON_COUNT = "wc";

	private const string SPECIFIC_EVENT_LOST_COUNT = "lc";

	private const string SPECIFIC_EVENT_CHOICE_LEVEL = "cl";

    [SerializeField]
	private List<PinSchedulerData.PinSchedulerThemeData> themes = new List<PinSchedulerData.PinSchedulerThemeData>();

	public void Reset()
	{
		this.themes.Clear();
	}

	public void DuplicateWorldTourThemeDataForDebugging(string themeIDToCopy, string newThemeID)
	{
		PinSchedulerData.PinSchedulerThemeData pinSchedulerThemeData = this.themes.Find((PinSchedulerData.PinSchedulerThemeData t) => t.ThemeID == themeIDToCopy);
		if (pinSchedulerThemeData != null)
		{
			JsonDict jsonDict = new JsonDict();
			this.setSpecificThemeData(pinSchedulerThemeData, ref jsonDict);
			PinSchedulerData.PinSchedulerThemeData pinSchedulerThemeData2 = new PinSchedulerData.PinSchedulerThemeData();
			this.getSpecificThemeData(jsonDict, ref pinSchedulerThemeData2);
			pinSchedulerThemeData2.ThemeID = newThemeID;
			this.themes.Add(pinSchedulerThemeData2);
		}
	}

	public void ToJson(ref JsonDict jsonDict)
	{
		jsonDict.SetObjectList<PinSchedulerData.PinSchedulerThemeData>("pstd", (from x in this.themes
		where !string.IsNullOrEmpty(x.ThemeID)
		select x).ToList<PinSchedulerData.PinSchedulerThemeData>(), new SetObjectDelegate<PinSchedulerData.PinSchedulerThemeData>(this.setSpecificThemeData));
	}

	private void setSpecificThemeData(PinSchedulerData.PinSchedulerThemeData themeData, ref JsonDict jsonDict)
	{
		jsonDict.Set("id", themeData.ThemeID);
		jsonDict.Set("rc", themeData.RacesComplete);
		jsonDict.Set("ts", themeData.SeenCount);
		jsonDict.Set("fl", themeData.CompletionLevel);
		jsonDict.Set("lsr", themeData.LastSequenceRaced);
		if (themeData.SequenceData.Count > 0)
		{
			jsonDict.SetObjectList<PinSchedulerData.PinSchedulerSequenceData>("sd", (from x in themeData.SequenceData
			where x.HasRecordedProgress()
			select x).ToList<PinSchedulerData.PinSchedulerSequenceData>(), new SetObjectDelegate<PinSchedulerData.PinSchedulerSequenceData>(this.setSpecificSequenceData));
		}
		if (themeData.LifetimeData.Count > 0)
		{
			jsonDict.SetObjectList<ScheduledPinLifetimeData>("ld", (from x in themeData.LifetimeData
			where x.HasRecordedProgress()
			select x).ToList<ScheduledPinLifetimeData>(), new SetObjectDelegate<ScheduledPinLifetimeData>(this.setLifetimeData));
		}
	}

	private void setLifetimeData(ScheduledPinLifetimeData lifetimeData, ref JsonDict jsonDict)
	{
		jsonDict.Set("g", lifetimeData.LifetimeGroup);
		jsonDict.Set("sc", lifetimeData.RaceCountFirstShownAt);
		jsonDict.Set("rc", lifetimeData.RaceCountLastRacedAt);
	}

	private void setSpecificSequenceData(PinSchedulerData.PinSchedulerSequenceData sequenceData, ref JsonDict jsonDict)
	{
		jsonDict.Set("id", sequenceData.ID.ToString("X"));
		jsonDict.Set("ls", sequenceData.LastSeenLevel);
		jsonDict.Set("ec", sequenceData.EventCount);
		if (!string.IsNullOrEmpty(sequenceData.MetricsID))
		{
			jsonDict.Set("mi", sequenceData.MetricsID);
		}
		if (sequenceData.LastWonLevel > -1)
		{
			jsonDict.Set("lw", sequenceData.LastWonLevel);
		}
		if (sequenceData.LastLostLevel > -1)
		{
			jsonDict.Set("ll", sequenceData.LastLostLevel);
		}
		if (sequenceData.LastRacedLevel != Mathf.Max(sequenceData.LastWonLevel, sequenceData.LastLostLevel))
		{
			jsonDict.Set("lr", sequenceData.LastRacedLevel);
		}
		if (sequenceData.SpecificEventData.Count > 0)
		{
			jsonDict.SetObjectList<PinSchedulerData.PinSchedulerEventData>("ed", (from x in sequenceData.SpecificEventData
			where x.HasRaced()
			select x).ToList<PinSchedulerData.PinSchedulerEventData>(), new SetObjectDelegate<PinSchedulerData.PinSchedulerEventData>(this.setSpecificEventsData));
		}
	}

	private void setSpecificEventsData(PinSchedulerData.PinSchedulerEventData eventData, ref JsonDict jsonDict)
	{
		jsonDict.Set("id", eventData.ID.ToString("X"));
		if (eventData.WonCount > 0)
		{
			jsonDict.Set("wc", eventData.WonCount);
		}
		if (eventData.LostCount > 0)
		{
			jsonDict.Set("lc", eventData.LostCount);
		}
		if (eventData.ChoiceLevel > -1)
		{
			jsonDict.Set("cl", eventData.ChoiceLevel);
		}
	}

	public void FromJson(ref JsonDict jsonDict)
	{
		if (!jsonDict.TryGetObjectList<PinSchedulerData.PinSchedulerThemeData>("pstd", out this.themes, new GetObjectDelegate<PinSchedulerData.PinSchedulerThemeData>(this.getSpecificThemeData)))
		{
			this.themes = new List<PinSchedulerData.PinSchedulerThemeData>();
		}
	}

	private void getSpecificThemeData(JsonDict jsonDict, ref PinSchedulerData.PinSchedulerThemeData themeData)
	{
		jsonDict.TryGetValue("id", out themeData.ThemeID);
		jsonDict.TryGetValue("rc", out themeData.RacesComplete);
		jsonDict.TryGetValue("ts", out themeData.SeenCount);
		jsonDict.TryGetValue("fl", out themeData.CompletionLevel);
		jsonDict.TryGetValue("lsr", out themeData.LastSequenceRaced);
		if (!jsonDict.TryGetObjectList<PinSchedulerData.PinSchedulerSequenceData>("sd", out themeData.SequenceData, new GetObjectDelegate<PinSchedulerData.PinSchedulerSequenceData>(this.getSpecificSequenceData)))
		{
			themeData.SequenceData = new List<PinSchedulerData.PinSchedulerSequenceData>();
		}
		if (!jsonDict.TryGetObjectList<ScheduledPinLifetimeData>("ld", out themeData.LifetimeData, new GetObjectDelegate<ScheduledPinLifetimeData>(this.getLifetimeData)))
		{
			themeData.LifetimeData = new List<ScheduledPinLifetimeData>();
		}
	}

	private void getLifetimeData(JsonDict jsonDict, ref ScheduledPinLifetimeData lifetimeData)
	{
		jsonDict.TryGetValue("g", out lifetimeData.LifetimeGroup);
		jsonDict.TryGetValue("sc", out lifetimeData.RaceCountFirstShownAt, -1);
		jsonDict.TryGetValue("rc", out lifetimeData.RaceCountLastRacedAt, -1);
	}

	private void getSpecificEventsData(JsonDict jsonDict, ref PinSchedulerData.PinSchedulerEventData eventData)
	{
		string s;
		jsonDict.TryGetValue("id", out s);
		if (int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out eventData.ID))
		{
			jsonDict.TryGetValue("wc", out eventData.WonCount);
			jsonDict.TryGetValue("lc", out eventData.LostCount);
			jsonDict.TryGetValue("cl", out eventData.ChoiceLevel, -1);
		}
	}

	private void getSpecificSequenceData(JsonDict jsonDict, ref PinSchedulerData.PinSchedulerSequenceData sequenceData)
	{
		string s;
		jsonDict.TryGetValue("id", out s);
		if (int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out sequenceData.ID))
		{
			jsonDict.TryGetValue("mi", out sequenceData.MetricsID);
			jsonDict.TryGetValue("ls", out sequenceData.LastSeenLevel, -1);
			jsonDict.TryGetValue("lw", out sequenceData.LastWonLevel, -1);
			jsonDict.TryGetValue("ll", out sequenceData.LastLostLevel, -1);
			jsonDict.TryGetValue("lr", out sequenceData.LastRacedLevel, Mathf.Max(sequenceData.LastWonLevel, sequenceData.LastLostLevel));
			jsonDict.TryGetValue("ec", out sequenceData.EventCount, -1);
			if (!jsonDict.TryGetObjectList<PinSchedulerData.PinSchedulerEventData>("ed", out sequenceData.SpecificEventData, new GetObjectDelegate<PinSchedulerData.PinSchedulerEventData>(this.getSpecificEventsData)))
			{
				sequenceData.SpecificEventData = new List<PinSchedulerData.PinSchedulerEventData>();
			}
		}
	}

	public List<PinSchedulerData.PinSchedulerThemeData> GetAllThemes()
	{
		return this.themes;
	}

	public int GetEventCountInSequence(string themeID, string sequenceID)
	{
		PinSchedulerData.PinSchedulerSequenceData specificSequenceData = this.getSpecificSequenceData(themeID, sequenceID);
		int num = specificSequenceData.EventCount;
		if (num < 0)
		{
			num = 2147483647;
		}
		return num;
	}

	public int GetRacesWonSinceStateChange(string themeID)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		return specificThemeData.RacesComplete;
	}

	public string GetLastSequenceRaced(string themeID)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		return specificThemeData.LastSequenceRaced;
	}

	public bool HasRacedSpecificPinSchedulerPin(string themeID, string sequenceID, string scheduledPinID)
	{
		PinSchedulerData.PinSchedulerEventData specificEventData = this.getSpecificEventData(themeID, sequenceID, scheduledPinID);
		return specificEventData != null && specificEventData.HasRaced();
	}

	public int GetLastRacedLevelInPinScheduleSequence(string themeID, string sequenceID)
	{
		PinSchedulerData.PinSchedulerSequenceData specificSequenceData = this.getSpecificSequenceData(themeID, sequenceID);
		return specificSequenceData.LastRacedLevel;
	}

	public void IncrementRacesComplete(string themeID)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		specificThemeData.IncrementRacesComplete();
	}

	public int GetThemeSeenCount(string themeID)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		return specificThemeData.SeenCount;
	}

	public void IncrementThemeSeenCount(string themeID)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		specificThemeData.IncrementSeenCount();
	}

	public ThemeCompletionLevel GetThemeCompletionLevel(string themeID)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		return specificThemeData.CompletionLevelEnum;
	}

	public void IncrementThemeCompletionLevel(string themeID)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		specificThemeData.IncrementCompletionLevel();
	}

	public void SetThemeCompletionLevel(string themeID, ThemeCompletionLevel level)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		specificThemeData.SetCompletionLevel(level);
	}

	public int GetLastSeenLevelInSequence(string themeID, string sequenceID)
	{
		PinSchedulerData.PinSchedulerSequenceData specificSequenceData = this.getSpecificSequenceData(themeID, sequenceID);
		return specificSequenceData.LastSeenLevel;
	}

	public int GetLastWonLevelInSequence(string themeID, string sequenceID)
	{
		PinSchedulerData.PinSchedulerSequenceData specificSequenceData = this.getSpecificSequenceData(themeID, sequenceID);
		return specificSequenceData.LastWonLevel;
	}

	public int GetChoiceSelection(string themeID, string sequenceID, string pinID)
	{
		PinSchedulerData.PinSchedulerEventData specificEventData = this.getSpecificEventData(themeID, sequenceID, pinID);
		return specificEventData.ChoiceLevel;
	}

	public void SetLastSeenLevelInSequence(string themeID, ScheduledPin pin)
	{
		PinSchedulerData.PinSchedulerSequenceData specificSequenceData = this.getSpecificSequenceData(themeID, pin.ParentSequence.ID);
		specificSequenceData.SetSeen(pin);
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		specificThemeData.UpdateLifetimeFirstShownAt(pin);
	}

	public ScheduledPinLifetimeData GetPinLifetimeData(string themeID, string lifetimeGroup)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		return specificThemeData.GetLifeTimeData(lifetimeGroup);
	}

	public void SetPinAsRaced(string themeID, ScheduledPin pin, bool won)
	{
		PinSchedulerData.PinSchedulerSequenceData specificSequenceData = this.getSpecificSequenceData(themeID, pin.ParentSequence.ID);
		specificSequenceData.SetPinAsRaced(pin, won);
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		specificThemeData.UpdateLifetimeLastRacedAt(pin);
		specificThemeData.LastSequenceRaced = pin.ParentSequence.ID;
		if (pin.ParentSequence.TypeEnum == PinSequence.eSequenceType.Choice && won)
		{
			PinSchedulerData.PinSchedulerSequenceData specificSequenceData2 = this.getSpecificSequenceData(themeID, pin.ReferrerPin.ParentSequence.ID);
			specificSequenceData2.SetChoiceLevel(pin.ReferrerPin, pin.Level);
		}
	}

	public int GetWorldTourRaceResultCount(string themeID, string sequenceID, string pinID, bool didWin)
	{
		PinSchedulerData.PinSchedulerEventData specificEventData = this.getSpecificEventData(themeID, sequenceID, pinID);
		if (didWin)
		{
			return specificEventData.WonCount;
		}
		return specificEventData.LostCount;
	}

	public void ResetLifeCount(string themeID, List<ScheduledPin> filteredPins)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		specificThemeData.ResetLifeCount(filteredPins);
	}

	private PinSchedulerData.PinSchedulerSequenceData getSpecificSequenceData(string themeID, string sequenceID)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		return specificThemeData.GetSpecificSequenceData(sequenceID);
	}

	private PinSchedulerData.PinSchedulerEventData getSpecificEventData(string themeID, string sequenceID, string pinID)
	{
		PinSchedulerData.PinSchedulerThemeData specificThemeData = this.getSpecificThemeData(themeID);
		PinSchedulerData.PinSchedulerSequenceData specificSequenceData = specificThemeData.GetSpecificSequenceData(sequenceID);
		return specificSequenceData.GetEventData(pinID);
	}

	private PinSchedulerData.PinSchedulerThemeData getSpecificThemeData(string themeID)
	{
		PinSchedulerData.PinSchedulerThemeData pinSchedulerThemeData = this.themes.Find((PinSchedulerData.PinSchedulerThemeData themeData) => themeData.ThemeID == themeID);
		if (pinSchedulerThemeData == null)
		{
			pinSchedulerThemeData = new PinSchedulerData.PinSchedulerThemeData();
			pinSchedulerThemeData.ThemeID = themeID;
			if (!string.IsNullOrEmpty(pinSchedulerThemeData.ThemeID))
			{
				this.themes.Add(pinSchedulerThemeData);
			}
		}
		return pinSchedulerThemeData;
	}

	public string AsProgressMetricsParameter()
	{
		return string.Join(";", (from x in this.themes
		select x.AsProgressMetricsParameter()).ToArray<string>());
	}
}
