using DataSerialization;
using KingKodeStudio;
using Metrics;

public class ReRateAppPopupDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		if (!BuildType.CanShowRate())
			return;
		
		Log.AnEvent(Events.ConfirmedRateApp);
        //RateTheAppNagger.TriggerRateAppPage();
        ScreenManager.Instance.PushScreen(ScreenID.UserRatingGame);
    }
}
