using System;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class NarrativeSceneForEventData
	{
		[ProtoMember(1)]
		public string PreRaceSceneID = string.Empty;

		[ProtoMember(2)]
		public string PostRaceWinSceneID = string.Empty;

		[ProtoMember(3)]
		public string PostRaceLoseSceneID = string.Empty;

		[ProtoMember(4)]
		public string IntroSceneID = string.Empty;

		[ProtoMember(5)]
		public ConditionallySelectedString ConditionalPreRaceSceneID = ConditionallySelectedString.CreateEmpty();

		[ProtoMember(6)]
		public ConditionallySelectedString ConditionalPostRaceWinSceneID = ConditionallySelectedString.CreateEmpty();

		[ProtoMember(7)]
		public ConditionallySelectedString ConditionalPostRaceLoseSceneID = ConditionallySelectedString.CreateEmpty();

		[ProtoMember(8)]
		public ConditionallySelectedString ConditionalIntroSceneID = ConditionallySelectedString.CreateEmpty();

		[ProtoMember(9)]
		public EligibilityRequirements PostRaceRequirements = EligibilityRequirements.CreateAlwaysEligible();

		[ProtoMember(10)]
		public EligibilityRequirements PreRaceRequirements = EligibilityRequirements.CreateAlwaysEligible();

		[ProtoMember(11)]
		public EligibilityRequirements IntroRequirements = EligibilityRequirements.CreateAlwaysEligible();

		[ProtoBeforeDeserialization]
		public void Init()
		{
			this.PostRaceRequirements = null;
			this.PreRaceRequirements = null;
			this.IntroRequirements = null;
		}

		public static NarrativeSceneForEventData CreateEmpty()
		{
			return new NarrativeSceneForEventData
			{
				PreRaceRequirements = EligibilityRequirements.CreateNeverEligible(),
				PostRaceRequirements = EligibilityRequirements.CreateNeverEligible(),
				IntroRequirements = EligibilityRequirements.CreateNeverEligible()
			};
		}
	}
}
