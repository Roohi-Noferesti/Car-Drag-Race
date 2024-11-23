using System;

namespace Objectives.Impl
{
	public class CompleteAllDailyObjectives : AbstractObjective
	{
		public void AllDailyObjectivesComplete()
		{
			base.ForceComplete();
		}

		public override void UpdateState()
		{
			if (this.IsActive && !this.IsComplete)
			{
				int num = 0;
				foreach (AbstractObjective current in ObjectiveManager.Instance.ActiveObjectives)
				{
					if (current.IsComplete)
					{
						num++;
					}
				}
				if (num >= ObjectiveManager.Instance.MaximumActiveObjectives - 1)
				{
					this.AllDailyObjectivesComplete();
				}
			}
		}
	}
}
