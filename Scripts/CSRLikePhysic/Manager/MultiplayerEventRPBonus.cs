using System;
using System.Collections.Generic;

public class MultiplayerEventRPBonus : IEquatable<MultiplayerEventRPBonus>
{
	public int EventID = -1;

	public int SpotPrizeID = -1;

	public DateTime StartTime;

	public bool Equals(MultiplayerEventRPBonus other)
	{
		return this.EventID == other.EventID && this.SpotPrizeID == other.SpotPrizeID;
	}

	public override int GetHashCode()
	{
		return this.EventID.GetHashCode() ^ this.SpotPrizeID.GetHashCode();
	}

	public RPBonus GetBonus()
	{
		MultiplayerEventData eventByID = GameDatabase.Instance.MultiplayerEvents.GetEventByID(this.EventID);
		if (eventByID == null)
		{
			return null;
		}
		if (this.SpotPrizeID < 0 || this.SpotPrizeID >= eventByID.SpotPrizes.Count)
		{
			return null;
		}
		SpotPrizeData spotPrizeData = eventByID.SpotPrizes[this.SpotPrizeID];
		if (spotPrizeData.PrizeTypeEnum != SpotPrizeType.RPBonus)
		{
			return null;
		}
	    return null;//new RPBonusMultiplayerEvent(spotPrizeData.Details.FloatQuantity, new RPBonusWindow(this.StartTime, this.StartTime.AddMinutes((double)spotPrizeData.Details.Duration)));
	}

	public bool IsValid(DateTime currentTime)
	{
		List<MultiplayerEventData> allEvents = GameDatabase.Instance.MultiplayerEvents.GetAllEvents();
		MultiplayerEventData multiplayerEventData = allEvents.Find((MultiplayerEventData x) => x.ID == this.EventID);
		if (multiplayerEventData == null)
		{
			return false;
		}
		if (currentTime > DateTime.MinValue && multiplayerEventData.StartTime > currentTime)
		{
			return false;
		}
		if (this.SpotPrizeID < 0 || this.SpotPrizeID >= multiplayerEventData.SpotPrizes.Count)
		{
			return false;
		}
		SpotPrizeData spotPrizeData = multiplayerEventData.SpotPrizes[this.SpotPrizeID];
		DateTime t = this.StartTime.AddMinutes((double)spotPrizeData.Details.Duration);
		return !(t <= currentTime) && spotPrizeData.PrizeAwarded;
	}
}
