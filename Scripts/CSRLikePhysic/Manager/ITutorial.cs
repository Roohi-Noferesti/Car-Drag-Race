using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorial
{
    void ShowInTutorialDialog();

    void HideTutorialDialog();

    void LockScreen();

    bool ShouldLockScreen();

    bool IsOn();

}
