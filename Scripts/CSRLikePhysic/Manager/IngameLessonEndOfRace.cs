using Metrics;
using UnityEngine;

public class IngameLessonEndOfRace : IngameLessonBase
{
	public GameObject Backdrop;

	private bool isDone;

	private bool waitingForRaceExit;

	private void Awake()
	{
	    if (Backdrop != null)
	        this.Backdrop.SetActive(false);
	}

	public override void StateOnEnter()
	{
		if (!IngameTutorial.IsInTutorial)
		{
		}
		RaceStateEnd.OnRaceFinish += new RaceStateEnd.RaceFinishEvent(this.OnRaceEnd);
		this.isDone = false;
		this.waitingForRaceExit = false;
	}

	public override bool StateUpdate()
	{
		if (this.waitingForRaceExit && CompetitorManager.Instance.HasCleanedUp())
		{
			this.isDone = true;
			//We call OnExit here because by calling lines bellow , StateOnExit will not called
			StateOnExit();
            RaceEventInfo.Instance.PopulateForTutorial(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial2, string.Empty);
            IngameTutorial.Instance.StartTutorial(IngameTutorial.TutorialPart.Tutorial2);
        }
		return this.isDone;
	}

	public override void StateOnExit()
	{
		RaceStateEnd.OnRaceFinish -= new RaceStateEnd.RaceFinishEvent(this.OnRaceEnd);
        if (Backdrop != null)
		this.Backdrop.SetActive(false);
	}

	public void OnRaceEnd()
	{
		this.BringUpDialog();
	}

	private void BringUpDialog()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_TUTORIAL_TITLE",
			BodyText = "TEXT_POPUPS_TUTORIAL_ENDOFRACE_BODY_NEW",
			IsBig = true,
			ConfirmAction = new PopUpButtonAction(this.OnNextPressed),
			ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
			ImageCaption = "TEXT_NAME_AGENT",
			ShouldCoverNavBar = true
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
        if (Backdrop != null)
		this.Backdrop.SetActive(true);
	}

	private void OnNextPressed()
    {
        #region CSR-Tut
        PlayerProfileManager.Instance.ActiveProfile.AddEventCompleted(RaceEventInfo.Instance.CurrentEvent.EventID);
        Log.AnEvent(Events.NowDo2ndRace);
        RaceController.Instance.Machine.SetState(RaceStateEnum.exit);
        SceneLoadManager.Instance.RequestScene(SceneLoadManager.Scene.Race);
        this.waitingForRaceExit = true;
        //PhilsFlag.Instance.FadeOut(0.6f, 0.25f);
        #endregion

        #region GT-Tut
        //this.waitingForRaceExit = true;
        //PlayerProfileManager.Instance.ActiveProfile.AddEventCompleted(RaceEventInfo.Instance.CurrentEvent.EventID);
        //RaceController.Instance.Machine.SetState(RaceStateEnum.exit);
        //SceneLoadManager.Instance.RequestScene(SceneLoadManager.Scene.Frontend);
        #endregion
    }
}
