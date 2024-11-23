using System.Collections;
using System.Collections.Generic;
using Metrics;
using UnityEngine;

public class IngameLessonGearUpAfterFirst : IngameLessonBase, ITutorial
{
    public GameObject BackDrop;
    
    private CarPhysics _humanCar;

    private bool _hasBroughtUpDialog;
    
    private void Awake()
    {
        if (BackDrop != null)
            BackDrop.SetActive(false);
    }
    
    public void ShowInTutorialDialog()
    {
        if (IsOn())
        {
            var gearUpButton = RaceHUDController.Instance.HUDAnimator.gearUpButton.transform.rectTransform();
            var gauge = RaceHUDController.Instance.HUDAnimator.GaugeRoot.transform.rectTransform();
            Vector3 position = RaceHUDController.Instance.HUDAnimator.GetGearUpButtonPosition() + new Vector3(0.158f, 0, -1.1f);
            BubbleManager.Instance.ShowMessage("TEXT_POPUPS_TUTORIAL_GEARUP_" + "1" + "_BODY", false, position,
                BubbleMessage.NippleDir.DOWN, 0.9f, BubbleMessageConfig.DuringRace
                , true, true, gearUpButton, gauge);
        }
        
    }

    public void HideTutorialDialog()
    {
        PauseGame.UnPause();
        if (BackDrop != null)
            BackDrop.SetActive(false);
        RaceHUDController.Instance.HUDAnimator.ShiftPaddleAnims.ActivateGearDown();
    }

    public void LockScreen()
    {
        if (ShouldLockScreen() && IsOn())
        {
            if (BackDrop != null)
                BackDrop.SetActive(true);
            PauseGame.Pause(false);
        }
    }

    public bool ShouldLockScreen()
    {
        return GameDatabase.Instance.TutorialConfiguration.ShouldLockScreen;
    }

    public bool IsOn()
    {
        return GameDatabase.Instance.TutorialConfiguration.IsOn;
    }

    public override void StateOnEnter()
    {
        
        _hasBroughtUpDialog = false;
        _humanCar = CompetitorManager.Instance.LocalCompetitor.CarPhysics;
    }

    public override bool StateUpdate()
    {
        if (_hasBroughtUpDialog && IsOn())
        {
            if (RaceController.Instance.Inputs != null && RaceController.Instance.Inputs.InputState.GearChangeUp)
            {
                Log.AnEvent(Events.TeachShiftUp);
                BubbleManager.Instance.DismissMessages();
                RaceHUDController.Instance.HUDAnimator.SetGearUpHightlight(false);
                return true;
            }
        }
        else if (RaceHUDController.Instance.hudGearLightsDisplay.GreenLightOn() && !this._humanCar.GearBox.IsInNeutral && _humanCar.DistanceTravelled < GetTotalRaceDistance())
        {
            BringUpDialog();
            _hasBroughtUpDialog = true;
            RaceHUDController.Instance.HUDAnimator.ShiftPaddleAnims.ActivateGearUp();
        }
        else if (RaceController.Instance.HasHumanTimeBeenSet())
        {
            return true;
        }
        return false;
    }

    public override void StateOnExit()
    {
        HideTutorialDialog();
    }
    
    private void BringUpDialog()
    {
        ShowInTutorialDialog();
        RaceHUDController.Instance.HUDAnimator.SetGearUpHightlight(true);
        LockScreen();
    }
}
