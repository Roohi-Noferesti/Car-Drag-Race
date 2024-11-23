using System;

namespace Objectives
{
	[Serializable]
	public abstract class AbstractObjectiveReward
	{
		public int Amount;

		public abstract void Apply();
	}
}
