using DataSerialization;
using System;
using System.Collections.Generic;

public static class StringModificationExtensions
{
	private static readonly Dictionary<string, StringModifier> modifierMapping = new Dictionary<string, StringModifier>
	{
		{
			"EventsCompletedInSequencesCount",
			new EventsCompletedInSequencesCountModifier()
		},
		{
			"AlwaysDefault",
			new AlwaysDefaultStringModifier()
		},
		{
			"SequenceProgression",
			new SequenceProgressionStringModifer()
		},
		{
			"MultipleSequenceProgression",
			new MultipleSequenceProgressionStringModifer()
		},
		{
			"WorldTourChoice",
			new WorldTourChoiceStringModifier()
		},
		{
			"CurrentEventGroupOpponentName",
			new CurrentEventGroupOppenentNameModifier()
		}
	};

	public static string Modify(this StringModification sm, IGameState gameState)
	{
		if (StringModificationExtensions.modifierMapping.ContainsKey(sm.Type))
		{
			StringModifier stringModifier = StringModificationExtensions.modifierMapping[sm.Type];
			if (stringModifier != null)
			{
				return stringModifier.Modify(gameState, sm.ModificationDetails);
			}
		}
		return null;
	}
}
