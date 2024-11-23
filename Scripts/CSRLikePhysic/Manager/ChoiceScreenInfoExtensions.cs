using DataSerialization;
using System;
using System.Collections.Generic;

public static class ChoiceScreenInfoExtensions
{
	public static List<ScheduledPin> GetEligiblePins(this ChoiceScreenInfo csi, IGameState gameState, ScheduledPin referrerPin)
	{
		PinSequence sequence = TierXManager.Instance.PinSchedule.GetSequence(csi.SequenceID);
		if (sequence == null)
		{
			return null;
		}
		List<ScheduledPin> allEligiblePins = sequence.GetAllEligiblePins(gameState);
		allEligiblePins.ForEach(delegate(ScheduledPin pin)
		{
			pin.ReferrerPin = referrerPin;
		});
		return allEligiblePins;
	}
}
