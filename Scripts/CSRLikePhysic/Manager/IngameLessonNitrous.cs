using Metrics;
using UnityEngine;

public class IngameLessonNitrous : IngameLessonBase
{
    public GameObject Backdrop;

    //private CarPhysics humanCar;

    private bool hasBroughtUpDialog;

    private bool HasShownPopup;


    private void Awake()
    {
        if (Backdrop != null)
            this.Backdrop.SetActive(false);
    }

    public override void StateOnEnter()
    {
        this.hasBroughtUpDialog = false;
        //this.humanCar = CompetitorManager.Instance.LocalCompetitor.CarPhysics;
    }

    private void BringUpDialog()
    {
        var nitrousButton = RaceHUDController.Instance.HUDAnimator.nitrousButton.transform.rectTransform();
        Vector3 position = RaceHUDController.Instance.HUDAnimator.GetNOSButtonPosition() + new Vector3(0f, 0.36f, 0f);
        BubbleManager.Instance.ShowMessage("TEXT_TRIGGER_NITROUS_DURING_RACE", false, position,
            BubbleMessage.NippleDir.DOWN, .2f, BubbleMessageConfig.DuringRace, true, true, nitrousButton);
        if (Backdrop != null)
            this.Backdrop.SetActive(true);
        PauseGame.Pause(false);
    }

    private void BringUpIntro()
    {
        PopUp popup = new PopUp
        {
            Title = "TEXT_NITROUS_TUTORIAL_UPGRADE_TITLE",
            BodyText = "TEXT_NITROUS_TUTORIAL_UPGRADE_BODY",
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

    private void UnPause()
    {
        Log.AnEvent(Events.ThisIsNitrous);
        PauseGame.UnPause();
    }

    public override bool StateUpdate()
    {
        if (this.hasBroughtUpDialog)
        {
            if (RaceController.Instance.Inputs != null && RaceController.Instance.Inputs.InputState.Nitrous)
            {
                BubbleManager.Instance.DismissMessages();
                return true;
            }
        }
        else if (RaceHUDController.Instance.HUDAnimator.IsAnimating() && !PauseGame.isGamePaused &&
                 !this.HasShownPopup)
        {
            this.HasShownPopup = true;
            //this.BringUpIntro();
        }
        else if (RaceHUDController.Instance.HUDAnimator.GetHUDIsInPositionOnscreen() && !PauseGame.isGamePaused
                 && CompetitorManager.Instance.LocalCompetitor.CarPhysics.SpeedMPH > 40)
        {
            this.hasBroughtUpDialog = true;
            if (!CompetitorManager.Instance.LocalCompetitor.CarPhysics.HasUsedNitrous)
            {
                this.BringUpDialog();
            }
            else
            {
                return true;
            }
            //this.state = eState.Ready;
        }
        else if (RaceController.Instance.HasHumanTimeBeenSet())
        {
            return true;
        }
        return false;
    }

    public override void StateOnExit()
    {
        PauseGame.UnPause();
        if (Backdrop != null)
            this.Backdrop.SetActive(false);
    }
}
