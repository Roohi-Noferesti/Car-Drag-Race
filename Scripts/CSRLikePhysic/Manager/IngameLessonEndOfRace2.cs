using Metrics;
using UnityEngine;

public class IngameLessonEndOfRace2 : IngameLessonBase
{
	public GameObject Backdrop;

	private bool isDone;

	public bool Done
	{
		get
		{
			return this.isDone;
		}
	}

	private void Awake()
    {
        if (Backdrop != null)
            this.Backdrop.SetActive(false);
    }

	public override void StateOnEnter()
	{
		this.isDone = false;
		RaceStateEnd.OnRaceFinish += new RaceStateEnd.RaceFinishEvent(this.OnRaceEnd);
	}

	public override bool StateUpdate()
	{
		if (CompetitorManager.Instance.HasCleanedUp())
		{
			isDone = true;
			StateOnExit();
		}
		return this.isDone;
	}

	public override void StateOnExit()
	{
		RaceStateEnd.OnRaceFinish -= new RaceStateEnd.RaceFinishEvent(this.OnRaceEnd);
		if (this.Backdrop != null)
		{
			this.Backdrop.SetActive(false);
		}
	}

	public void OnRaceEnd()
	{
        if (!RaceResultsTracker.You.IsWinner)
        {
            this.BringUpLoseDialog();
            return;
        }
        //else
        //{
        //    this.isDone = true;
        //    PlayerProfileManager.Instance.ActiveProfile.AddEventCompleted(RaceEventInfo.Instance.CurrentEvent.EventID);
        //}

        this.isDone = true;
        PlayerProfileManager.Instance.ActiveProfile.AddEventCompleted(RaceEventInfo.Instance.CurrentEvent.EventID);
        //RaceController.Instance.Machine.SetState(RaceStateEnum.exit);
        //SceneLoadManager.Instance.RequestScene(SceneLoadManager.Scene.Frontend);
	}

	private void BringUpLoseDialog()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_TUTORIAL_TITLE",
			BodyText = "TEXT_POPUPS_TUTORIAL_ENDOFRACE_FAIL_BODY",
			IsBig = true,
			ConfirmAction = new PopUpButtonAction(this.OnRestartPressed),
			ConfirmText = "TEXT_BUTTON_RETRY",
            GraphicPath = PopUpManager.Instance.graphics_greenLightShift,
			ShouldCoverNavBar = true
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
        if (Backdrop != null)
		this.Backdrop.SetActive(true);
	}

	private void OnRestartPressed()
    {
        if (Backdrop != null)
            this.Backdrop.SetActive(false);
		Log.AnEvent(Events.YouCanDoBetter);
		RaceController.Instance.ResetRace();
	}
}
