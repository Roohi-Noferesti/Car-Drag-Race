using System;

namespace Objectives
{
	public class CounterDonatedCrewTokens : AbstractObjectiveCommand
	{
		public CounterDonatedCrewTokens(int donated) : base("CounterDonatedCrewTokens", new object[]
		{
			donated
		})
		{
		}
	}
}
