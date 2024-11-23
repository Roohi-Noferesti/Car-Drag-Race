using Metrics;
using UnityEngine;

public class IngameLessonGearUp : IngameLessonBase, ITutorial
{
	public GameObject Backdrop;

	private CarPhysics humanCar;

	private bool hasBroughtUpDialog;

    private int m_gear;

    private float m_lastDialogAt;

    private void Awake()
	{
	    if (Backdrop != null)
	        this.Backdrop.SetActive(false);
	}

	public override void StateOnEnter()
	{
	    m_gear = 1;
		this.hasBroughtUpDialog = false;
		this.humanCar = CompetitorManager.Instance.LocalCompetitor.CarPhysics;
	}

    //public override bool StateUpdate()
    //{
    //    if (Time.time - m_lastDialogAt < 1)
    //        return false;
    //    if (this.hasBroughtUpDialog)
    //    {
    //        if (RaceController.Instance.Inputs != null && RaceController.Instance.Inputs.InputState.GearChangeUp)
    //        {
    //            Log.AnEvent(Events.TeachShiftUp);
    //            BubbleManager.Instance.DismissMessages();
    //            RaceHUDController.Instance.HUDAnimator.SetGearUpHightlight(false);
    //            if (this.humanCar.GearBox.CurrentGear < 2)
    //            {
    //                m_gear++;
    //                PauseGame.UnPause();
    //                if (Backdrop != null)
    //                    this.Backdrop.SetActive(false);
    //                hasBroughtUpDialog = false;
    //                RaceHUDController.Instance.HUDAnimator.ShiftPaddleAnims.ReFade();
    //                return false;
    //            }
    //            m_lastDialogAt = Time.time;
    //            return true;
    //        }
    //    }
    //    else if (RaceHUDController.Instance.hudGearLightsDisplay.GreenLightOn() && !this.humanCar.GearBox.IsInNeutral)
    //    {
    //        this.BringUpDialog();
    //        this.hasBroughtUpDialog = true;
    //        RaceHUDController.Instance.HUDAnimator.ShiftPaddleAnims.ActivateGearUp();
    //    }
    //    else if (RaceController.Instance.HasHumanTimeBeenSet())
    //    {
    //        return true;
    //    }
    //    return false;
    //}


    public override bool StateUpdate()
    {
        if (this.hasBroughtUpDialog)
        {
            if (RaceController.Instance.Inputs != null && RaceController.Instance.Inputs.InputState.GearChangeUp)
            {
                Log.AnEvent(Events.TeachShiftUp);
                BubbleManager.Instance.DismissMessages();
                RaceHUDController.Instance.HUDAnimator.SetGearUpHightlight(false);
                return true;
            }
        }
        else if (RaceHUDController.Instance.hudGearLightsDisplay.GreenLightOn() && !this.humanCar.GearBox.IsInNeutral && humanCar.DistanceTravelled < GetTotalRaceDistance())
        {
            this.BringUpDialog();
            this.hasBroughtUpDialog = true;
            RaceHUDController.Instance.HUDAnimator.ShiftPaddleAnims.ActivateGearUp();
        }
        else if (RaceController.Instance.HasHumanTimeBeenSet())
        {
            return true;
        }
        return false;
    }

    private void BringUpDialog()
    {
        ShowInTutorialDialog();
        RaceHUDController.Instance.HUDAnimator.SetGearUpHightlight(true);
        LockScreen();
    }

	public override void StateOnExit()
	{
		HideTutorialDialog();
	}

	public void ShowInTutorialDialog()
	{
		
		var gearUpButton = RaceHUDController.Instance.HUDAnimator.gearUpButton.transform.rectTransform();
		var gauge = RaceHUDController.Instance.HUDAnimator.GaugeRoot.transform.rectTransform();
		Vector3 position = RaceHUDController.Instance.HUDAnimator.GetGearUpButtonPosition() + new Vector3(0.158f, 0, -1.1f);
		BubbleManager.Instance.ShowMessage("TEXT_POPUPS_TUTORIAL_GEARUP_" + m_gear + "_BODY", false, position,
			BubbleMessage.NippleDir.DOWN, 0.9f, BubbleMessageConfig.DuringRace
			, true, true, gearUpButton, gauge);
		
		
	}

	public void HideTutorialDialog()
	{
		PauseGame.UnPause();
		if (Backdrop != null)
			this.Backdrop.SetActive(false);
		RaceHUDController.Instance.HUDAnimator.ShiftPaddleAnims.ActivateGearDown();
	}

	public void LockScreen()
	{
		if (Backdrop != null)
			this.Backdrop.SetActive(true);
		PauseGame.Pause(false);
		
	}

	public bool ShouldLockScreen()
	{
		return false;
	}

	public bool IsOn()
	{
		return false;
	}
}
