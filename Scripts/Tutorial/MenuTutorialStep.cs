using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuTutorialStep
{
    public string AnimationName;
    public RectTransform TargetTransform;
    public string TargetScreenName;
    public Action OnCompletedAction;
    public bool DisableDefaultInteraction;

    public Button Button
    {
        get
        {
            if (TargetTransform != null)
                return TargetTransform.GetComponent<Button>();
            return null;
        }
    }
}
