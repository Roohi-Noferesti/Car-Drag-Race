using System;
using System.Collections;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;

public class AssistantScreen : ZHUDScreen
{
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private int m_dialogCount;
    private int m_dialogIndex = -1;
    private string m_currentAnimationName;

    protected override void Awake()
    {
        base.Awake();
        m_text.gameObject.SetActive(false);
        AudioManager.Instance.PlaySound("Ring", Camera.main.gameObject);
    }

    public void Answer()
    {
        m_currentAnimationName = "Assistant@answered";
        m_animator.Play(m_currentAnimationName);
        StartCoroutine(CheckAnimationEnd(m_currentAnimationName, NextDialog));

    }

    public void NextDialog()
    {
        m_dialogIndex++;
        if (m_dialogIndex < m_dialogCount)
        {
            m_text.gameObject.SetActive(true);
            m_text.text = LocalizationManager.GetTranslation("TUTORIAL_DIALOG_" + (m_dialogIndex + 1));
        }
        else
        {
            LoadingScreenManager.Fadein(() =>
            {
                ScreenManager.Instance.PushScreen(ScreenID.CareerModeMap);
                LoadingScreenManager.Fadeout();
            });
            //ScreenManager.Active.pushScreen("MapScreen");

        }
    }

    private IEnumerator CheckAnimationEnd(string animationName,Action onCompleted)
    {
        var endStateReached = false;
        while (!endStateReached)
        {
            if (!Animator.IsInTransition(0))
            {
                var stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
                endStateReached =
                    stateInfo.IsName(animationName)
                    && stateInfo.normalizedTime >= 1;
            }

            yield return new WaitForEndOfFrame();
        }
        if(onCompleted!=null)
            onCompleted.Invoke();
    }
}
