using DataSerialization;

public class DebugWelcomeDismissAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		PlayerProfileManager.Instance.ActiveProfile.DebugWelcomeDismissed = true;
	}
}
