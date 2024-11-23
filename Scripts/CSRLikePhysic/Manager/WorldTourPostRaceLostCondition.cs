using DataSerialization;
using System;

public class WorldTourPostRaceLostCondition : FlowConditionBase
{
	public override bool IsConditionActive()
	{
		return true;
	}
	
	private PopupData popupData;

	public override void EvaluateHardcodedConditions()
	{
		this.state = ConditionState.NOT_VALID;
	    if (RaceResultsTracker.You==null)
	    {
	        return;
	    }
		if (RaceResultsTracker.You.IsWinner)
		{
			return;
		}
		if (!RaceEventInfo.Instance.IsWorldTourEvent)
		{
			return;
		}
		PinDetail worldTourPinPinDetail = RaceEventInfo.Instance.CurrentEvent.GetWorldTourPinPinDetail();
		if (worldTourPinPinDetail == null)
		{
			return;
		}
		ScheduledPin worldTourScheduledPinInfo = worldTourPinPinDetail.WorldTourScheduledPinInfo;
		if (worldTourScheduledPinInfo == null)
		{
			return;
		}
		IGameState gs = new GameStateFacade();
		PopupData onPostRaceLostPopup = worldTourScheduledPinInfo.GetOnPostRaceLostPopup();
		if (!onPostRaceLostPopup.IsEligible(gs))
		{
			return;
		}
		this.popupData = onPostRaceLostPopup;
		this.state = ConditionState.VALID;
	}

	public override PopUp GetPopup()
	{
		return this.popupData.GetPopup(null, null);
	}

	public override bool HasBubbleMessage()
	{
		return false;
	}

	public override string GetBubbleMessage()
	{
		return string.Empty;
	}
}
