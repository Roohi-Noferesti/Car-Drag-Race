using System;

namespace Objectives
{
	public static class ObjectiveCommand
	{
		public static void Execute(AbstractObjectiveCommand objectiveCommand, bool includeInactive = true)
		{
			//return;
			if (includeInactive)
			{
				foreach (AbstractObjective current in ObjectiveManager.Instance.AllObjectives)
				{
					if ((!current.IsComplete && current.CanUpdateWhenInactive) || current.IsActive)
					{
						objectiveCommand.ExecuteOn(current);
					}
				}
			}
			else
			{
				foreach (AbstractObjective current2 in ObjectiveManager.Instance.AllObjectives)
				{
					if (current2.IsActive)
					{
						objectiveCommand.ExecuteOn(current2);
					}
				}
			}
		}
	}
}
