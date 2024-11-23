using DataSerialization;

public class NotShowingEventTypeInSequenceCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    //ScheduledPin currentPinInSequence = TierXManager.Instance.PinSchedule.GetCurrentPinInSequence(gameState, details.SequenceID);
        //return currentPinInSequence == null || !(currentPinInSequence.EventType == details.EventType);
	    return false;
	}
}
