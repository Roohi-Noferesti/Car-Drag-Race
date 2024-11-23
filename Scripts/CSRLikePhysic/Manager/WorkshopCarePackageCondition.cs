using System;

public class WorkshopCarePackageCondition : FlowConditionBase
{
	public override bool IsConditionActive()
	{
		return true;
	}
	
	public override void EvaluateHardcodedConditions()
	{
		if (GameDatabase.Instance.CarePackages.GetPopUp() != null)
		{
			this.state = ConditionState.VALID;
		}
		else
		{
			this.state = ConditionState.NOT_VALID;
		}
	}

	public override PopUp GetPopup()
	{
		return GameDatabase.Instance.CarePackages.GetPopUp();
	}

	public override bool HasBubbleMessage()
	{
		return false;
	}

	public override string GetBubbleMessage()
	{
		return string.Empty;
	}
}
