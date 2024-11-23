using Metrics;
using UnityEngine;

public class IngameLessonThrottle : IngameLessonBase
{
	private enum MiniStates
	{
		WaitForHUD,
        WaitForPopUp,
		ShowingPopup,
		ShowingBubble,
		WaitingForEnd
	}

	private const float READY_TO_COUNTDOWN_DELAY = 2f;

	private float readyToCountdownStateTimer;

	private MiniStates state;

	public GameObject Backdrop;

	private void Awake()
	{
        if (Backdrop != null)
		this.Backdrop.SetActive(false);
	}

	public override void StateOnEnter()
	{
		this.state = MiniStates.WaitForHUD;
	}

	private void BringUpBubble()
	{
	    Vector3 position = RaceHUDController.Instance.HUDAnimator.GetThrottleButtonPosition() + new Vector3(-0.007f, 0, 0);
	    BubbleManager.Instance.ShowMessage("TEXT_POPUPS_TUTORIAL_THROTTLE_BODY", false, position,
	        BubbleMessage.NippleDir.DOWN, 1f, BubbleMessageConfig.DuringRace
            , true,true, RaceHUDController.Instance.HUDAnimator.throttleButton.transform.rectTransform());
        if (Backdrop != null)		
        this.Backdrop.SetActive(true);
		PauseGame.disablePause = true;
		this.state = MiniStates.ShowingBubble;
	}

	public override bool StateUpdate()
	{
		MiniStates miniStates = this.state;

        switch (miniStates)
	    {
            case MiniStates.WaitForPopUp:
	            if (!RaceController.Instance.Machine.StateBefore(RaceStateEnum.precountdown))
	            {
	                if (RaceHUDController.Instance.HUDAnimator.GetHUDIsInPositionOnscreen())
	                {
	                    if (!PauseGame.isGamePaused)
	                    {
	                        this.BringUpDialog();
	                    }
	                }
	            }
	            break;
	        case MiniStates.ShowingBubble:
	            this.readyToCountdownStateTimer += Time.deltaTime;
	            bool flag = RaceController.Instance.Inputs != null &&
	                        RaceController.Instance.Inputs.InputState.Throttle;
	            bool flag2 = RaceController.Instance.Machine.StateIs(RaceStateEnum.countdown);
	            if ((flag || flag2) && this.IsReadyToCountdownState())
	            {
	                Log.AnEvent(Events.PressGas);
	                BubbleManager.Instance.DismissMessages();
	                RaceController.Instance.Machine.SetState(RaceStateEnum.countdown);
	                return true;
	            }
	            break;
	        case MiniStates.WaitForHUD:
	            if (RaceController.Instance.Machine.StateIs(RaceStateEnum.enter))
	            {
                    this.state = MiniStates.WaitForPopUp;
	            }
	            else if (!RaceController.Instance.Machine.StateBefore(RaceStateEnum.precountdown))
	            {
	                if (RaceHUDController.Instance.HUDAnimator.GetHUDIsInPositionOnscreen())
	                {
	                    this.BringUpBubble();
	                }
	            }
	            break;
	    }
	    return false;
	}


    private void BringUpDialog()
    {
        PopUp popup = new PopUp
        {
            Title = "TEXT_POPUPS_TUTORIAL_THROTTLE1_TITLE",
            BodyText = "TEXT_POPUPS_TUTORIAL_THROTTLE1_BODY",
            IsBig = true,
            ConfirmText = "TEXT_BUTTON_OK",
            ImageCaption = "TEXT_NAME_AGENT",
            ConfirmAction = () =>
            {
                this.BringUpBubble();
                this.state = MiniStates.ShowingBubble;
                PauseGame.UnPause();
            },
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective,null);
        PauseGame.Pause(false);
        if (Backdrop != null)
            this.Backdrop.SetActive(true);
        this.state = MiniStates.ShowingPopup;
    }

    public override void StateOnExit()
	{
		PauseGame.disablePause = false;
        if (Backdrop != null)
		this.Backdrop.SetActive(false);
	}

	private bool IsReadyToCountdownState()
	{
		return this.readyToCountdownStateTimer >= 2f;
	}
}
