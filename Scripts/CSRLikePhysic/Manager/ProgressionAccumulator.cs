using DataSerialization;
using System;
using System.Collections.Generic;
using System.Linq;

public class ProgressionAccumulator
{
	private delegate Fraction AccumulateDelegate(ProgressionVisualisation details, string currentSequence, string currentTheme, PinScheduleConfiguration pinSchedule);

	private static Dictionary<string, ProgressionAccumulator.AccumulateDelegate> Accumulators = new Dictionary<string, ProgressionAccumulator.AccumulateDelegate>
	{
		{
			"ProgressionThroughCurrentSequence",
			new ProgressionAccumulator.AccumulateDelegate(ProgressionAccumulator.ProgressionThroughCurrentSequenceAccumulator)
		},
		{
			"ProgressionThroughOtherSequence",
			new ProgressionAccumulator.AccumulateDelegate(ProgressionAccumulator.ProgressionThroughOtherSequenceAccumulator)
		},
		{
			"SumOfSequencesCompleted",
			new ProgressionAccumulator.AccumulateDelegate(ProgressionAccumulator.SumOfSequencesCompletedAccumulator)
		},
		{
			"Zero",
			new ProgressionAccumulator.AccumulateDelegate(ProgressionAccumulator.ZeroAccumulator)
		}
	};

	public static Fraction Accumulate(ProgressionVisualisation details, string currentSequence, string currentTheme, PinScheduleConfiguration pinSchedule = null)
	{
		if (!ProgressionAccumulator.Accumulators.ContainsKey(details.Accumulator))
		{
			return default(Fraction);
		}
		return ProgressionAccumulator.Accumulators[details.Accumulator](details, currentSequence, currentTheme, pinSchedule);
	}

	private static Fraction ZeroAccumulator(ProgressionVisualisation details, string currentSequence, string currentTheme, PinScheduleConfiguration pinSchedule)
	{
		return default(Fraction);
	}

	private static Fraction ProgressionThroughCurrentSequenceAccumulator(ProgressionVisualisation details, string currentSequence, string currentTheme, PinScheduleConfiguration pinSchedule)
	{
		GameStateFacade gameState = new GameStateFacade();
		return pinSchedule.GetProgression(gameState, currentSequence);
	}

	private static Fraction ProgressionThroughOtherSequenceAccumulator(ProgressionVisualisation details, string currentSequence, string currentTheme, PinScheduleConfiguration pinSchedule)
	{
		GameStateFacade gameState = new GameStateFacade();
		if (details.SequenceIDs.Count < 1)
		{
			return default(Fraction);
		}
		return pinSchedule.GetProgression(gameState, details.SequenceIDs[0]);
	}

	private static Fraction SumOfSequencesCompletedAccumulator(ProgressionVisualisation details, string currentSequence, string currentTheme, PinScheduleConfiguration pinSchedule)
	{
		GameStateFacade gameState = new GameStateFacade();
		List<string> arg_2C_0;
		if ((arg_2C_0 = details.SequenceIDs) == null)
		{
			arg_2C_0 = new List<string>
			{
				currentSequence
			};
		}
		List<string> list = arg_2C_0;
		string themeID = details.ThemeID ?? currentTheme;
		return new Fraction
		{
			Numerator = list.Count((string seq) => gameState.IsSequenceComplete(themeID, seq)),
			Denominator = list.Count
		};
	}
}
