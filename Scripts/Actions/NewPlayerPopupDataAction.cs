using DataSerialization;
using Metrics;

public class NewPlayerPopupDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		Log.AnEvent(Events.HeyYou);
		RaceEventInfo.Instance.PopulateForTutorial(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial, string.Empty);
	    IngameTutorial.Instance.StartTutorial(IngameTutorial.TutorialPart.Tutorial1);
		SceneManagerFrontend.ButtonStart();
	}
}
