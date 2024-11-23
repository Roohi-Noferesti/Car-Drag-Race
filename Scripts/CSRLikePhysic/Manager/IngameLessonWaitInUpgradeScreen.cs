using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameLessonWaitInUpgradeScreen : IngameLessonBase, ITutorial
{
    private bool _hasFinished;
    public override void StateOnEnter()
    {
        _hasFinished = false;
    }

    public override bool StateUpdate()
    {
        StartCoroutine(WaitSomeSeconds((() =>
        {
            _hasFinished = true;
        })));
        return _hasFinished;
    }

    public override void StateOnExit()
    {
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

    IEnumerator WaitSomeSeconds(Action isFinished = null)
    {
        yield return new WaitForSeconds(1f);
        if (isFinished != null)
        {
            isFinished.Invoke();
        }
    }
}
