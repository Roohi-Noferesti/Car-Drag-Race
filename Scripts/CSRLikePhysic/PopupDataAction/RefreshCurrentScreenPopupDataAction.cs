using DataSerialization;
using System;
using KingKodeStudio;

public class RefreshCurrentScreenPopupDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		ScreenManager.Instance.RefreshTopScreen();
	}
}
