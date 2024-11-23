using System;

namespace Objectives
{
	public class CrewRPEarned : AbstractObjectiveCommand
	{
		public CrewRPEarned(int rpCrewEarned) : base("CrewRPEarned", new object[]
		{
			rpCrewEarned
		})
		{
		}
	}
}
