using System;

public class WorkshopUpgradeCondition : FlowConditionBase
{
	public override bool IsConditionActive()
	{
		return true;
	}
	
	public override void EvaluateHardcodedConditions()
	{
		this.state = ConditionState.NOT_VALID;
        if (GarageCameraManager.Instance.IsInCarPornMode)
        {
            return;
        }
		if (CarDatabase.Instance.GetCar(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey).UsesEvoUpgrades())
		{
			return;
		}
		if (AgentUpgradeNags.IsUpgradeAvailable())
		{
			this.state = ConditionState.VALID;
		}
	}

	public override PopUp GetPopup()
	{
		return AgentUpgradeNags.GetUpgradePopup();
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
