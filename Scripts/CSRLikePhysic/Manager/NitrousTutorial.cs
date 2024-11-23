using Metrics;
using UnityEngine;

public class NitrousTutorial : MonoBehaviour
{
	private enum eState
	{
		Start,
		Waiting,
		WaitingForBubble,
		Ready,
		ThrottlePressed,
		NitrousPressed
	}

	private bool Enabled;

	private eState state;

	private BubbleMessage Message;

	private bool HasShownPopup;

	private bool isEndOfRace;
    private GameObject Backdrop;
    public bool NitrousPressed;

    public static NitrousTutorial Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
		this.Enabled = false;
		this.Reset();
	}

	private void Reset()
	{
		this.state = eState.Start;
		this.isEndOfRace = false;
		if (this.Message != null)
		{
			this.Message.Dismiss();
			this.Message = null;
		}
	}

	public void ShouldActivate()
	{
		if (!PlayerProfileManager.Instance.ActiveProfile.HasSeenNitrousTutorial && RaceEventInfo.Instance
			    .LocalPlayerCarUpgradeSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted > 0)
		{
			MakeActive();
		}
	}

	public void EndOfRace()
	{
		this.isEndOfRace = true;
	}

	public bool IsActive()
	{
		return this.Enabled;
	}

	private void MakeActive()
	{
		this.Reset();
		this.Enabled = true;
	}

	private void BringUpIntro(string title, string body)
	{
		PopUp popup = new PopUp
		{
			Title = title,
			BodyText = body,
			IsBig = true,
			ConfirmAction = new PopUpButtonAction(this.UnPause),
			ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
			ImageCaption = "TEXT_NAME_AGENT",
			ShouldCoverNavBar = true,
			ID = PopUpID.NitrousTutorial
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
        PauseGame.Pause(false);
	}

	private void BringUpDailyBattleIntro()
	{
		this.BringUpIntro("TEXT_NITROUS_TUTORIAL_DAILY_BATTLE_TITLE", "TEXT_NITROUS_TUTORIAL_DAILY_BATTLE_BODY");
		
	}

	private void BringUpUpgradeIntro()
	{
		this.BringUpIntro("TEXT_NITROUS_TUTORIAL_UPGRADE_TITLE", "TEXT_NITROUS_TUTORIAL_UPGRADE_BODY");
	}

	private void UnPause()
	{
		Log.AnEvent(Events.ThisIsNitrous);
		PauseGame.UnPause();
		state = eState.WaitingForBubble;
	}

    private void Update()
    {
        if (!this.Enabled)
        {
            return;
        }
        if (SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Race &&
            this.state != eState.Start)
        {
            this.Reset();
            this.Enabled = false;
            if (PopUpManager.Instance.CurrentPopUpMatchesID(PopUpID.NitrousTutorial))
            {
                PopUpManager.Instance.KillPopUp();
            }
            return;
        }
        switch (this.state)
        {
            case eState.Start:
                if (RaceController.Instance.Machine.StateIs(RaceStateEnum.enter) &&
                    SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Race)
                {
                    this.HasShownPopup = false;
                    this.state = eState.Waiting;
                }
                break;
            case eState.Waiting:
                if (RaceHUDController.Instance.HUDAnimator.IsAnimating() && !PauseGame.isGamePaused &&
                    !this.HasShownPopup)
                {
                    this.HasShownPopup = true;
                    if (false)//RaceEventInfo.Instance.CurrentEvent.IsDailyBattle())
                    {
                        this.BringUpDailyBattleIntro();
                    }
                    else
                    {
                        this.BringUpUpgradeIntro();
                    }
                }
                break;
            case eState.WaitingForBubble:
	            if (RaceHUDController.Instance.HUDAnimator.GetHUDIsInPositionOnscreen() && !PauseGame.isGamePaused
					&& CompetitorManager.Instance.LocalCompetitor.CarPhysics.SpeedMPH > 60)
	            {
                    if (!CompetitorManager.Instance.LocalCompetitor.CarPhysics.HasUsedNitrous)
                    {
                        this.Message = this.BringUpDialog_Intro();
                        this.state = eState.Ready;
                    }
                    else
                    {
                        this.state = eState.NitrousPressed;
                    }
                }

	            break;
            case eState.Ready:
                if (RaceController.Instance.Inputs != null && RaceController.Instance.Inputs.InputState.Nitrous)
                {
                    this.Message.Dismiss();
                    this.state = eState.NitrousPressed;
                }
                //    if (CompetitorManager.Instance.LocalCompetitor.CarPhysics.Engine.Throttle > 0f)
                //{
                //    this.state = eState.ThrottlePressed;
                //}
                break;
            case eState.ThrottlePressed:
                if (CompetitorManager.Instance.LocalCompetitor.CarPhysics.HasUsedNitrous || this.isEndOfRace)
                {
                    this.Message.Dismiss();
                    this.state = eState.NitrousPressed;
                    PauseGame.UnPause();
                    if (Backdrop != null)
                        this.Backdrop.SetActive(false);
                }
                break;
            case eState.NitrousPressed:
                this.Finished();
                break;
        }
    }

    private BubbleMessage BringUpDialog_Intro()
    {
        if (Backdrop != null)
            this.Backdrop.SetActive(true);
        PauseGame.Pause(false);
        Vector3 position = RaceHUDController.Instance.HUDAnimator.GetNOSButtonPosition() + new Vector3(0f, 0.36f, 0f);
        return BubbleManager.Instance.ShowMessage("TEXT_TRIGGER_NITROUS_DURING_RACE", false, position,
            BubbleMessage.NippleDir.DOWN, 0f, BubbleMessageConfig.DuringRace);

    }

    private void Finished()
	{
        PauseGame.UnPause();
        if (Backdrop != null)
            this.Backdrop.SetActive(false);
        NitrousPressed = true;
        RaceController.Instance.Inputs.InputState.Nitrous = true;
        CompetitorManager.Instance.LocalCompetitor.CarPhysics.Engine.NitrousInput = true;
		PlayerProfileManager.Instance.ActiveProfile.HasSeenNitrousTutorial = true;
		this.Enabled = false;
	}
}
