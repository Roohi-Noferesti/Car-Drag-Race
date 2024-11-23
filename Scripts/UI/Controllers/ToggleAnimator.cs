using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle),typeof(Animator))]
public class ToggleAnimator : MonoBehaviour
{
    private Toggle m_toggle;
    private Animator m_animator;

    void Awake()
    {
        m_toggle = GetComponent<Toggle>();
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (m_animator.IsInTransition(0))
            return;
        var state = m_toggle.isOn;
        //Debug.Log(state + "    " + m_animator.GetCurrentAnimatorStateInfo(0).IsName("on")
        //    + "   " + m_animator.GetCurrentAnimatorStateInfo(0).IsName("off"));
        if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("on") && !state)
        {
            m_animator.Play("off");
        }
        else if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("off") && state)
        {
            m_animator.Play("on");
        }
    }


}
