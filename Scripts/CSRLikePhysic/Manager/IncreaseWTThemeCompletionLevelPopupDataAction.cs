using DataSerialization;
using System;

public class IncreaseWTThemeCompletionLevelPopupDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		string text = details.ThemeID.ToLower();
		if (string.IsNullOrEmpty(text))
		{
			text = TierXManager.Instance.CurrentThemeName;
		}
		IGameState gameState = new GameStateFacade();
		gameState.IncrementWorldTourThemeCompletionLevel(text);
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}
}
