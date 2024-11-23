using DataSerialization;
using System;
using System.Collections.Generic;
using System.Linq;

public static class PinScheduleConfigurationExtensions
{
	public static void Initialise(this PinScheduleConfiguration psc, string themeName)
	{
		psc.themeID = themeName;
		foreach (PinSequence current in psc.Sequences)
		{
            current.Initialise();
		}
	}

	public static ScheduledPin GetFirstAvailablePin(this PinScheduleConfiguration psc, IGameState gameState)
	{
		List<ScheduledPin> pinState = psc.getPinState(gameState, psc.Sequences);
		if (pinState.Count < 1)
		{
			return null;
		}
		return pinState[0];
	}

	public static List<ScheduledPin> GetPins(this PinScheduleConfiguration psc, IGameState gameState)
	{
		List<ScheduledPin> pinState = psc.getPinState(gameState, psc.Sequences);
		for (int i = 0; i < pinState.Count; i++)
		{
		}
        gameState.ResetLifeCount(psc.themeID, pinState);
        return (from p in pinState
                orderby p.GetAutoSelectPriority(gameState) descending
                select p).ToList<ScheduledPin>();
	}

	public static List<ScheduledPin> GetAllPins(this PinScheduleConfiguration psc)
	{
		return psc.Sequences.SelectMany((PinSequence d) => d.Pins).ToList<ScheduledPin>();
	}

	private static List<ScheduledPin> getPinState(this PinScheduleConfiguration psc, IGameState gameState, List<PinSequence> sequences)
	{
	    List<ScheduledPin> result = new List<ScheduledPin>();
        foreach (var pinSequence in sequences)
	    {
	        if (pinSequence.IsEligible(gameState, psc.themeID) && !pinSequence.ID.Contains("Trash_Talk_Race_Sequence"))
	        {
	            var pin = pinSequence.GetCurrentPin(gameState, psc.themeID, null);
                if (pin!=null)
                {
                    result.Add(pin);
                }
            }
	    }

	    return result;
	}

	public static ScheduledPin GetCurrentPinInSequence(this PinScheduleConfiguration psc, IGameState gameState, string sequenceId)
	{
		int num = gameState.LastShownEventSequenceLevel(psc.themeID, sequenceId);
		if (num < 0)
		{
			return null;
		}
		PinSequence sequence = psc.GetSequence(sequenceId);
		if (sequence != null && sequence.Pins != null && sequence.Pins.Count > num)
		{
			return sequence.Pins[num];
		}
		return null;
	}

	public static ScheduledPin GetEligiblePinInSequence(this PinScheduleConfiguration psc, IGameState gameState, string sequenceID, ScheduledPin parentPin = null)
	{
		PinSequence sequence = psc.GetSequence(sequenceID);
		if (sequence != null)
		{
			return sequence.GetCurrentPin(gameState, psc.themeID, parentPin);
		}
		return null;
	}

	public static PinSequence GetSequence(this PinScheduleConfiguration psc, string sequenceID)
	{
		return psc.Sequences.Find((PinSequence s) => s.ID == sequenceID);
	}

	private static int getEventCountInSequence(this PinScheduleConfiguration psc, IGameState gameState, string sequenceID)
	{
		PinSequence pinSequence = psc.Sequences.Find((PinSequence s) => s.ID == sequenceID);
		if (pinSequence == null)
		{
			return 0;
		}
		return pinSequence.Pins.Count;
	}

	public static Fraction GetProgression(this PinScheduleConfiguration psc, IGameState gameState, string sequenceID)
	{
		PinSequence sequence = psc.GetSequence(sequenceID);
		if (sequence != null)
		{
			int eventCountInSequence = psc.getEventCountInSequence(gameState, sequenceID);
			int num = gameState.LastWonEventSequenceLevel(TierXManager.Instance.CurrentThemeName, sequenceID);
			num++;
			return new Fraction
			{
				Numerator = num,
				Denominator = eventCountInSequence
			};
		}
		return default(Fraction);
	}
}
