using System;
using System.Collections;
using UnityEngine;

public class BlackScreen : MonoBehaviour
{
    [SerializeField] private Animator m_blackAnimator;
    private bool m_checkAnimation;
    private string m_animationName;

    public void OpenBlackScreen(Action oncompleted=null)
    {
        SetActive(true);
        m_blackAnimator.transform.SetAsFirstSibling();
        m_blackAnimator.StopPlayback();
        m_animationName = "back@open";
        m_checkAnimation = true;

        if (oncompleted != null)
        {
            StartCoroutine(_whenAnimationCompleted(oncompleted));
        }
    }

    public void CloseBlackScreen(Action oncompleted = null)
    {
        SetActive(true);
        m_blackAnimator.transform.SetAsFirstSibling();
        m_blackAnimator.StopPlayback();
        m_animationName = "back@close";
        m_checkAnimation = true;

        if (oncompleted != null)
        {
            StartCoroutine(_whenAnimationCompleted(oncompleted));
        }
    }

    public void SetActive(bool value)
    {
        if (m_blackAnimator.gameObject.activeInHierarchy != value)
            m_blackAnimator.gameObject.SetActive(value);
    }

    private IEnumerator _whenAnimationCompleted(Action oncompleted)
    {
        var endStateReached = false;
        while (!endStateReached)
        {
            if (!m_blackAnimator.IsInTransition(0))
            {
                var stateInfo = m_blackAnimator.GetCurrentAnimatorStateInfo(0);
                endStateReached =
                    stateInfo.IsName(m_animationName)
                    && stateInfo.normalizedTime >= 1;
            }

            yield return new WaitForEndOfFrame();
        }
        if (oncompleted != null)
            oncompleted.Invoke();
    }

    void Update()
    {
        if (m_checkAnimation)
        {
            if (!m_blackAnimator.GetCurrentAnimatorStateInfo(0).IsName(m_animationName))
            {
                m_checkAnimation = false;
                m_blackAnimator.Play(m_animationName);
            }
        }
    }
}
