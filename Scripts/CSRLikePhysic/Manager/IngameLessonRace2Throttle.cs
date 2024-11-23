using Metrics;
using UnityEngine;

public class IngameLessonRace2Throttle : IngameLessonBase
{
	private enum MiniStates
	{
		WaitForGrid,
		WaitForPopUp,
		ShowingPopup
	}

	private const float READY_TO_COUNTDOWN_DELAY = 2f;

	private float readyToCountdownStateTimer;

	public GameObject Backdrop;

	private MiniStates CurrentState = MiniStates.WaitForPopUp;

	private void Awake()
	{
        if (Backdrop != null)
		this.Backdrop.SetActive(false);
	}

	public override void StateOnEnter()
	{
		this.CurrentState = MiniStates.WaitForGrid;
	}

	private void BringUpDialog()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_TUTORIAL_THROTTLE2_TITLE",
			BodyText = "TEXT_POPUPS_TUTORIAL_THROTTLE2_BODY",
			IsBig = false,
            IsCrewLeader = true,
			ConfirmText = string.Empty,
            ItemGraphicPath = PopUpManager.Instance.graphics_greenLightGas,
            TargetGameObjectNames = new[] { "Throttle_Button"}
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, delegate
		{
			PopUpManager.Instance.PopUpScreenInstance.DisableOutsideClickArea();
			PopUpManager.Instance.PopUpScreenInstance.DisableBackground();
		});
		PauseGame.Pause(false);
        if (Backdrop != null)
		this.Backdrop.SetActive(true);
		this.CurrentState = MiniStates.ShowingPopup;
	}

	private void OnDismiss()
	{
		PopUpManager.Instance.KillPopUp();
		PauseGame.UnPause();
		RaceController.Instance.Machine.SetState(RaceStateEnum.countdown);
		Log.AnEvent(Events.FocusGoodStart);
		this.Backdrop.SetActive(false);
	}

	public override bool StateUpdate()
	{
		switch (this.CurrentState)
		{
		case MiniStates.WaitForGrid:
			if (RaceController.Instance.Machine.StateIs(RaceStateEnum.enter))
			{
				this.CurrentState = MiniStates.WaitForPopUp;
			}
			break;
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
		case MiniStates.ShowingPopup:
			this.readyToCountdownStateTimer += Time.deltaTime;
			if (RaceController.Instance.Inputs != null && RaceController.Instance.Inputs.InputState.Throttle && this.IsReadyToCountdownState())
			{
				this.OnDismiss();
				return true;
			}
			break;
		}
		return false;
	}

	public override void StateOnExit()
	{
	}

	private bool IsReadyToCountdownState()
	{
		return this.readyToCountdownStateTimer >= 2f;
	}
}
