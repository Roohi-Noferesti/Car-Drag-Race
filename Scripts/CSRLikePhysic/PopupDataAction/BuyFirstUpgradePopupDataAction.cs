using DataSerialization;
using KingKodeStudio;
using Metrics;
using Z2HSharedLibrary.DatabaseEntity;

public class BuyFirstUpgradePopupDataAction : PopupDataActionBase
{
	private const int minimumCashRequiredForUpgrade = 1000;

	public override void Execute(EligibilityConditionDetails details)
	{
		Log.AnEvent(Events.AgentIntroToUpgrades);
		int num = minimumCashRequiredForUpgrade - PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash();
		if (num > 0)
		{
			PlayerProfileManager.Instance.ActiveProfile.AddCash(num,"reward","BuyFirstUpgrade");
		}
		TuningScreen.ExternalStartScreenOn = eUpgradeType.NITROUS;
        //ScreenManager.Instance.PushScreen(ScreenID.Tuning);
        GarageScreen.Instance.BringUpgardeBubble();
        //PopUp popup = new PopUp
        //{
        //    Title = "TEXT_POPUPS_FIRSTUPGRADE_RECOMMENDATION_TITLE",
        //    BodyText = "TEXT_POPUPS_FIRSTUPGRADE_RECOMMENDATION_BODY",
        //    IsBig = true,
        //    ConfirmAction = new PopUpButtonAction(this.RecordMetricsEvent),
        //    ConfirmText = "TEXT_BUTTON_OK",
        //    GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
        //    ImageCaption = "TEXT_NAME_AGENT"
        //};
        //PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
    }

	private void RecordMetricsEvent()
	{
		Log.AnEvent(Events.AgentSuggestsUpgrade);
	}
}
