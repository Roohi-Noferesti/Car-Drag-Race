using DataSerialization;
using System;

public class RefreshWorldTourThemePopupdataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		TierXManager.Instance.RefreshThemeMap();
	}
}
