using System;

namespace Objectives
{
	public class SpendSC : AbstractObjectiveCommand
	{
		public SpendSC(int SCSpent) : base("SpendSC", new object[]
		{
			SCSpent
		})
		{
		}
	}
}
