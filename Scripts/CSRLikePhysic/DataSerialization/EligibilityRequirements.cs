using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
    [ProtoContract]
	[Serializable]
	public class EligibilityRequirements
	{
        [ProtoContract]
        [Serializable]
		public class PossibleGameState
		{
            [ProtoMember(1)]
			public List<EligibilityCondition> Conditions = new List<EligibilityCondition>();
		}

        [ProtoMember(1)]
		public List<PossibleGameState> PossibleGameStates = new List<PossibleGameState>();

		public static EligibilityRequirements CreateAlwaysEligible()
		{
			return new EligibilityRequirements
			{
				PossibleGameStates = new List<PossibleGameState>
				{
					new PossibleGameState()
				}
			};
		}

		public static EligibilityRequirements CreateNeverEligible()
		{
			return new EligibilityRequirements();
		}
	}
}
