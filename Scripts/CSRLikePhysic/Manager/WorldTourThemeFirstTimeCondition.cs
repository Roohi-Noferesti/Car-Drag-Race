using System;
using KingKodeStudio;

public class WorldTourThemeFirstTimeCondition : FlowConditionBase
{
	public override bool IsConditionActive()
	{
		return true;
	}
	
	public override void EvaluateHardcodedConditions()
	{
		this.state = ConditionState.NOT_VALID;
		//if (CareerModeMapScreen.mapPaneSelected != 5)
		//{
		//	return;
		//}
		if (TierXManager.Instance.IsOverviewThemeActive())
		{
			return;
		}
		IGameState gameState = new GameStateFacade();
		string currentThemeName = TierXManager.Instance.CurrentThemeName;
		int worldTourThemeSeenCount = gameState.GetWorldTourThemeSeenCount(currentThemeName);
		if (worldTourThemeSeenCount > 1)
		{
			return;
		}
		this.state = ConditionState.VALID;
	}

	public override PopUp GetPopup()
	{
		return new PopUp
		{
			Title = "TEXT_WT_TUTORIAL_POPUPS_FIRST_TIME_IN_THEME_TITLE",
			BodyText = "TEXT_WT_TUTORIAL_POPUPS_FIRST_TIME_IN_THEME_BODY",
			ConfirmAction = new PopUpButtonAction(this.OnPopupShownOK),
			ConfirmText = "TEXT_BUTTON_OK",
			GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
			ImageCaption = "TEXT_NAME_AGENT"
		};
	}

	private void OnPopupShownOK()
	{
		ScreenManager.Instance.PushScreen(ScreenID.WorldTourProgress);
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
