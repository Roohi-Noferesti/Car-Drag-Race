using System;
using System.Collections;
using System.Collections.Generic;
using KingKodeStudio;
using Metrics;
using UnityEngine;

public class IngameLessonUpgradeNitro : IngameLessonBase, ITutorial
{
    public GameObject BackDrop;

    private bool _hasBroughtUpDialog;

    private bool _hasShownPopup;

    private void Awake()
    {
        if (BackDrop != null)
            BackDrop.SetActive(false);
    }
    
    public override void StateOnEnter()
    {
        _hasBroughtUpDialog = false;
        
        if (IsOn() && PlayerProfileManager.Instance.ActiveProfile.FreeUpgradesLeft <= 0)
        {
            AwardPrizeBase awardPrizeBase = PrizeomaticAwarding.CreatePrize(Reward.FreeUpgrade);
            awardPrizeBase.AwardPrize();
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        }
    }

    public override bool StateUpdate()
    {
        if (BubbleManager.Instance.isShowingBubble)
        {
            return false;
        }
        if (_hasBroughtUpDialog)
        {
            return true;
        }
        ShowInTutorialDialog();
        _hasBroughtUpDialog = true;
        LockScreen();
        return false;
    }

    public override void StateOnExit()
    {
        HideTutorialDialog();
    }

    public void ShowInTutorialDialog()
    {
        if (IsOn())
        {
            GameObject upgradeButton = null;
            if (ScreenManager.Instance.CurrentScreen == ScreenID.Tuning)
            {
                upgradeButton = GameObject.Find("CostContainer");
            }
            var pos = new Vector3(1.5f, 0.7f, 0);
            BubbleManager.Instance.ShowMessage("TEXT_TAP_TO_UPGRADE", false, pos,
             BubbleMessage.NippleDir.DOWN, 0.2F, BubbleMessageConfig.Frontend, true,true,
             upgradeButton.transform.rectTransform());

        }
    }
    

    public void HideTutorialDialog()
    {
        if (BackDrop != null)
            BackDrop.SetActive(false);
    }

    public void LockScreen()
    {
        PauseGame.Pause(false);
    }

    public bool ShouldLockScreen()
    {
        return GameDatabase.Instance.TutorialConfiguration.ShouldLockScreen;

    }

    public bool IsOn()
    {
        return GameDatabase.Instance.TutorialConfiguration.IsOn;

    }
}
