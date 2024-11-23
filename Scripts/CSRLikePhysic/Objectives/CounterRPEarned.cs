using System;

namespace Objectives
{
	public class CounterRPEarned : AbstractObjectiveCommand
	{
		public CounterRPEarned(int rpEarned) : base("CounterRPEarned", new object[]
		{
			rpEarned
		})
		{
		}
	}
}
