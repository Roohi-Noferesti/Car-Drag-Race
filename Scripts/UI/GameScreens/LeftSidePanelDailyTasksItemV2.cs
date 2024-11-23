using System;

public class LeftSidePanelDailyTasksItemV2 : LeftSidePanelDailyTasksItem
{
	public enum DailyObjectiveState
	{
		NOT_SET = -1,
		INCOMPLETE,
		CLAIMABLE,
		CLAIMED
	}

    //private UIWidget m_UIWidgetContainer;

	public override string TimeLimit
	{
		set
		{
		}
	}

	public override float TaskCompletion
	{
		set
		{
		}
	}

	private void Start()
	{
        //this.m_UIWidgetContainer = base.gameObject.GetComponent<UIWidget>();
        //if (this.m_UIWidgetContainer != null)
        //{
        //    UIWidget expr_28 = this.m_UIWidgetContainer;
        //    expr_28.onChange = (UIWidget.OnDimensionsChanged)Delegate.Combine(expr_28.onChange, new UIWidget.OnDimensionsChanged(this.OnDimentionsChanges));
        //}
	}

	private void OnDimentionsChanges()
	{
        //if (LeftSidePanelAchievementsScreen.Instance.scrollTable_DailyTasks != null)
        //{
        //    LeftSidePanelAchievementsScreen.Instance.scrollTable_DailyTasks.repositionNow = true;
        //}
	}

	public override void SetObjectiveID(string ID)
	{
		this.m_ObjectiveID = ID;
	}

	public override string GetObjectiveID()
	{
		return this.m_ObjectiveID;
	}
}
