using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameLessonBackButton : IngameLessonBase, ITutorial
{
    private bool _hasDone;
    public GameObject BackDrop;
    
    private void Awake()
    {
        if (BackDrop != null)
            BackDrop.SetActive(false);
    }
    public override void StateOnEnter()
    {
        _hasDone = false;
    }

    public override bool StateUpdate()
    {
        if (_hasDone)
        {
            return true;
        }
        if (BubbleManager.Instance.isShowingBubble)
        {
            return false;
        }

        if (IsOn())
        {
            CommonUI.Instance.OnBack();
            _hasDone = true;
        }
        return false;
    }

    public override void StateOnExit()
    {
        if (BackDrop != null)
            BackDrop.SetActive(false);
    }

    public void ShowInTutorialDialog()
    {

    }

    public void HideTutorialDialog()
    {

    }

    public void LockScreen()
    {

    }

    public bool ShouldLockScreen()
    {
        return true;
    }

    public bool IsOn()
    {
        return GameDatabase.Instance.TutorialConfiguration.IsOn;
    }
}
