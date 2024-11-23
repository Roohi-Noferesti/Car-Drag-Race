using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
    [Serializable]
	public class ThemeProgressionData
	{
		[ProtoMember(1)]
		public NarrativeSceneForEventData IntroNarrative = NarrativeSceneForEventData.CreateEmpty();

		[ProtoMember(2)]
		public EligibilityRequirements IncreaseThemeCompletionLevelRequirements = EligibilityRequirements.CreateNeverEligible();
	}
}
