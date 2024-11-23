using System;

namespace Objectives
{
	public class CounterFreshnessRPEarned : AbstractObjectiveCommand
	{
		public CounterFreshnessRPEarned(int rpEarnedViaFreshness) : base("CounterFreshnessRPEarned", new object[]
		{
			rpEarnedViaFreshness
		})
		{
		}
	}
}
