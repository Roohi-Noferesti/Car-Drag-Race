using System;

public abstract class FlowConditionalBase
{
	private class ConditionContainer : IComparable
	{
		public FlowConditionBase Condition;

		public int Priority;

		public ConditionShowMode ShowMode;

		public int CompareTo(object obj)
		{
			ConditionContainer conditionContainer = (ConditionContainer)obj;
			return this.Priority.CompareTo(conditionContainer.Priority);
		}
	}

	private PriorityQueue<ConditionContainer> conditions;

	private int popupsDisplayed;

	public FlowConditionalBase()
	{
		this.conditions = new PriorityQueue<ConditionContainer>();
		this.popupsDisplayed = 0;
	}

	protected void AddCondition(FlowConditionBase condition, int priority, ConditionShowMode showMode)
	{
		this.conditions.Push(new ConditionContainer
		{
			Condition = condition,
			Priority = priority,
			ShowMode = showMode
		});
	}

	public virtual void EvaluateAll()
	{
		if (!this.IsConditionalActive())
		{
			return;
		}
		IGameState gameState = new GameStateFacade();
		foreach (ConditionContainer current in this.conditions.GetList())
		{
			current.Condition.Evaluate(gameState);
		}
	}

	public virtual FlowConditionBase GetNextValidCondition()
	{
		if (!this.IsConditionalActive())
		{
			return null;
		}
		while (!this.conditions.IsEmpty())
		{
			ConditionContainer conditionContainer = this.conditions.Pop();
			if (conditionContainer.ShowMode != ConditionShowMode.ShowIfFirstCondition || this.popupsDisplayed <= 0)
			{
				if (conditionContainer.Condition.state == ConditionState.VALID)
				{
					this.popupsDisplayed++;
					return conditionContainer.Condition;
				}
			}
		}
		return null;
	}

	protected abstract bool IsConditionalActive();
}
