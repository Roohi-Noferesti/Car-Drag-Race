using DataSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public static class PinSequenceExtensions
{
	private abstract class SequenceOrderBase
	{
		public abstract ScheduledPin GetCurrentPin(IGameState gameState, string themeID, PinSequence sequence);

		public virtual bool IsSequenceConditionValid(IGameState gameState, ScheduledPin pin)
		{
			return true;
		}

		public virtual int GetLastLevelActionPerformed(IGameState gameState, string themeID, PinSequence sequence)
		{
			return gameState.LastWonEventSequenceLevel(themeID, sequence.ID);
		}

		public virtual bool PinFinished(IGameState gameState, string themeID, PinSequence sequence)
		{
			int num = gameState.LastShownEventSequenceLevel(themeID, sequence.ID);
			int lastLevelActionPerformed = this.GetLastLevelActionPerformed(gameState, themeID, sequence);
			return num == lastLevelActionPerformed;
		}

		public bool ChildPinFinished(IGameState gameState, string themeID, ScheduledPin pin)
		{
			bool flag = true;
			if (!string.IsNullOrEmpty(pin.SequenceReference))
			{
				ScheduledPin eligiblePinInSequence = TierXManager.Instance.PinSchedule.GetEligiblePinInSequence(gameState, pin.SequenceReference, null);
				flag = eligiblePinInSequence.ParentSequence.PinFinished(gameState, themeID);
			}
			return flag && this.PinFinished(gameState, themeID, pin.ParentSequence);
		}
	}

	private class LadderSequence : PinSequenceExtensions.SequenceOrderBase
	{
		public override ScheduledPin GetCurrentPin(IGameState gameState, string themeID, PinSequence sequence)
		{
			int num = this.GetLastLevelActionPerformed(gameState, themeID, sequence) + 1;
			if (num >= sequence.Pins.Count)
			{
				num = sequence.Pins.Count - 1;
			}
			return sequence.Pins[num];
		}
	}

	private class NextRandomValidSequence : PinSequenceExtensions.SequenceOrderBase
	{
		public override ScheduledPin GetCurrentPin(IGameState gameState, string themeID, PinSequence sequence)
		{
			int lastLevelShown = gameState.LastShownEventSequenceLevel(themeID, sequence.ID);
			ScheduledPin scheduledPin = null;
			if (lastLevelShown >= 0)
			{
				scheduledPin = sequence.Pins[lastLevelShown];
			}
			if (scheduledPin == null || !scheduledPin.IsEligible(gameState) || base.ChildPinFinished(gameState, themeID, scheduledPin))
			{
				int count = sequence.Pins.Count;
				List<int> list = Enumerable.Range(0, count).ToList<int>();
				list.RemoveAll((int i) => i == lastLevelShown || !sequence.Pins[i].IsEligible(gameState) || !this.IsSequenceConditionValid(gameState, sequence.Pins[i]));
				if (list.Count > 0)
				{
					int index = list[UnityEngine.Random.Range(0, list.Count)];
					return sequence.Pins[index];
				}
			}
			return scheduledPin;
		}
	}

	private class NextRandomValidAfterWinOrLoseSequence : PinSequenceExtensions.NextRandomValidSequence
	{
		public override int GetLastLevelActionPerformed(IGameState gameState, string themeID, PinSequence sequence)
		{
			return gameState.LastRacedEventSequenceLevel(themeID, sequence.ID);
		}
	}

	private class CycleSequence : PinSequenceExtensions.SequenceOrderBase
	{
		public override ScheduledPin GetCurrentPin(IGameState gameState, string themeID, PinSequence sequence)
		{
			int num = this.GetLastLevelActionPerformed(gameState, themeID, sequence) + 1;
			if (num >= sequence.Pins.Count)
			{
				num = 0;
			}
			return sequence.Pins[num];
		}
	}

	private class CycleNextValidSequence : PinSequenceExtensions.SequenceOrderBase
	{
		public override ScheduledPin GetCurrentPin(IGameState gameState, string themeID, PinSequence sequence)
		{
			int num = gameState.LastShownEventSequenceLevel(themeID, sequence.ID);
			int lastLevelActionPerformed = this.GetLastLevelActionPerformed(gameState, themeID, sequence);
			int num2 = 0;
			if (num >= sequence.Pins.Count)
			{
				num = sequence.Pins.Count - 1;
			}
			if (num >= 0)
			{
				ScheduledPin pin = sequence.Pins[num];
				if (base.ChildPinFinished(gameState, themeID, pin))
				{
					num2 = lastLevelActionPerformed + 1;
				}
				else
				{
					num2 = num;
				}
			}
			for (int i = 0; i < sequence.Pins.Count; i++)
			{
				if (num2 >= sequence.Pins.Count)
				{
					num2 = 0;
				}
				ScheduledPin scheduledPin = sequence.Pins[num2];
				if (scheduledPin != null && scheduledPin.IsEligible(gameState) && this.IsSequenceConditionValid(gameState, scheduledPin))
				{
					return scheduledPin;
				}
				num2++;
			}
			return null;
		}
	}

	private class CycleNextValidAfterWinOrLoseSequence : PinSequenceExtensions.CycleNextValidSequence
	{
		public override int GetLastLevelActionPerformed(IGameState gameState, string themeID, PinSequence sequence)
		{
			return gameState.LastRacedEventSequenceLevel(themeID, sequence.ID);
		}
	}

	private class CycleNextValidNotRaced : PinSequenceExtensions.CycleNextValidSequence
	{
		public override bool IsSequenceConditionValid(IGameState gameState, ScheduledPin pin)
		{
			bool flag = gameState.HasRacedSpecificPinSchedulerPin(gameState.CurrentWorldTourThemeID, pin.ParentSequence.ID, pin.ID);
			return !flag;
		}
	}

	private class ChoiceOptions : PinSequenceExtensions.SequenceOrderBase
	{
		public override ScheduledPin GetCurrentPin(IGameState gameState, string themeID, PinSequence sequence)
		{
			return null;
		}
	}

	private static IDictionary<PinSequence.eSequenceType, PinSequenceExtensions.SequenceOrderBase> conditionMapping = new Dictionary<PinSequence.eSequenceType, PinSequenceExtensions.SequenceOrderBase>
	{
		{
			PinSequence.eSequenceType.Ladder,
			new PinSequenceExtensions.LadderSequence()
		},
		{
			PinSequence.eSequenceType.Cycle,
			new PinSequenceExtensions.CycleSequence()
		},
		{
			PinSequence.eSequenceType.CycleNextValidAfterWin,
			new PinSequenceExtensions.CycleNextValidSequence()
		},
		{
			PinSequence.eSequenceType.CycleNextValidAfterWinOrLose,
			new PinSequenceExtensions.CycleNextValidAfterWinOrLoseSequence()
		},
		{
			PinSequence.eSequenceType.CycleNextValidNotRaced,
			new PinSequenceExtensions.CycleNextValidNotRaced()
		},
		{
			PinSequence.eSequenceType.NextRandomValidAfterWin,
			new PinSequenceExtensions.NextRandomValidSequence()
		},
		{
			PinSequence.eSequenceType.NextRandomValidAfterWinOrLose,
			new PinSequenceExtensions.NextRandomValidAfterWinOrLoseSequence()
		},
		{
			PinSequence.eSequenceType.Choice,
			new PinSequenceExtensions.ChoiceOptions()
		}
	};

	public static void Initialise(this PinSequence ps)
	{
		if (ps.Requirements != null)
		{
			ps.Requirements.Initialise();
		}
		ps.TypeEnum = EnumHelper.FromString<PinSequence.eSequenceType>(ps.Type);
		for (int i = 0; i < ps.Pins.Count; i++)
		{
			ps.Pins[i].Initialise();
			ps.Pins[i].Level = i;
			ps.Pins[i].ParentSequence = ps;
		}
	}

	[Conditional("CSR_DEBUG_LOGGING")]
	public static void CheckPinIdUniqueness(this PinSequence ps)
	{
		Dictionary<int, string> pinIDHashes = new Dictionary<int, string>();
		for (int i = 0; i < ps.Pins.Count; i++)
		{
			ScheduledPin scheduledPin = ps.Pins[i];
            //int crossPlatformHashCode = scheduledPin.ID.GetCrossPlatformHashCode();
            //pinIDHashes[crossPlatformHashCode] = scheduledPin.ID;
			for (int j = i + 1; j < ps.Pins.Count; j++)
			{
			}
		}
		ps.Pins.ForEach(delegate(ScheduledPin pin)
		{
			int hashCode = pin.ID.GetHashCode();
			if (!pinIDHashes.ContainsKey(hashCode) || pinIDHashes[hashCode] != pin.ID)
			{
			}
		});
	}

	public static bool IsEligible(this PinSequence ps, IGameState gameState, string themeID)
	{
		if (ps.Requirements.IsEligible(gameState))
		{
			ScheduledPin currentPin = ps.GetCurrentPin(gameState, themeID, null);
			return currentPin != null && currentPin.IsEligible(gameState);
		}
		return false;
	}

	public static ScheduledPin GetCurrentPin(this PinSequence ps, IGameState gameState, string themeID, ScheduledPin parentPin = null)
	{
		ScheduledPin currentPin = PinSequenceExtensions.conditionMapping[ps.TypeEnum].GetCurrentPin(gameState, themeID, ps);
		if (currentPin != null && !string.IsNullOrEmpty(currentPin.SequenceReference))
		{
			currentPin.ReferrerPin = parentPin;
			return TierXManager.Instance.PinSchedule.GetEligiblePinInSequence(gameState, currentPin.SequenceReference, currentPin);
		}
		if (currentPin != null)
		{
			currentPin.ReferrerPin = parentPin;
		}
		return currentPin;
	}

	public static bool PinFinished(this PinSequence ps, IGameState gameState, string themeID)
	{
		return PinSequenceExtensions.conditionMapping[ps.TypeEnum].PinFinished(gameState, themeID, ps);
	}

	public static List<ScheduledPin> GetAllEligiblePins(this PinSequence ps, IGameState gameState)
	{
	    //return ps.Pins.FindAll((ScheduledPin pin) => pin.IsEligible(gameState));
	    return null;
	}

    [DebuggerHidden]
	public static IEnumerable<ScheduledPin> GetNextTimelinePins(this PinSequence ps, int afterIndex, int count)
	{
	    //PinSequenceExtensions.<GetNextTimelinePins>c__Iterator19 <GetNextTimelinePins>c__Iterator = new PinSequenceExtensions.<GetNextTimelinePins>c__Iterator19();
        //<GetNextTimelinePins>c__Iterator.count = count;
        //<GetNextTimelinePins>c__Iterator.ps = ps;
        //<GetNextTimelinePins>c__Iterator.afterIndex = afterIndex;
        //<GetNextTimelinePins>c__Iterator.<$>count = count;
        //<GetNextTimelinePins>c__Iterator.<$>ps = ps;
        //<GetNextTimelinePins>c__Iterator.<$>afterIndex = afterIndex;
        //PinSequenceExtensions.<GetNextTimelinePins>c__Iterator19 expr_31 = <GetNextTimelinePins>c__Iterator;
        //expr_31.$PC = -2;
        //return expr_31;
	    return null;
	}

    [DebuggerHidden]
	public static IEnumerable<ScheduledPin> GetPreviousTimelinePins(this PinSequence ps, int beforeIndex, int count)
    {
        //PinSequenceExtensions.<GetPreviousTimelinePins>c__Iterator1A <GetPreviousTimelinePins>c__Iterator1A = new PinSequenceExtensions.<GetPreviousTimelinePins>c__Iterator1A();
        //<GetPreviousTimelinePins>c__Iterator1A.count = count;
        //<GetPreviousTimelinePins>c__Iterator1A.ps = ps;
        //<GetPreviousTimelinePins>c__Iterator1A.beforeIndex = beforeIndex;
        //<GetPreviousTimelinePins>c__Iterator1A.<$>count = count;
        //<GetPreviousTimelinePins>c__Iterator1A.<$>ps = ps;
        //<GetPreviousTimelinePins>c__Iterator1A.<$>beforeIndex = beforeIndex;
        //PinSequenceExtensions.<GetPreviousTimelinePins>c__Iterator1A expr_31 = <GetPreviousTimelinePins>c__Iterator1A;
        //expr_31.$PC = -2;
        //return expr_31;
        return null;
    }
}
