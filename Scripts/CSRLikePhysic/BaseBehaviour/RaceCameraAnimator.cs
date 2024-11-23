using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RaceCameraAnimator : BaseBehaviour
{
    [SerializeField] private string m_animationName;
    [SerializeField] private Transform m_targetObject;
    //[SerializeField] private float m_fov = 30;
    private float m_delayTime;
    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private Animator m_animator;
    public override void OnActivate()
    {
        m_delayTime = 0;
        m_animator.Play(m_animationName);
    }

    public override void OnUpdate(ref CameraState zResult)
    {
        zResult.position = m_targetObject.position;
        zResult.rotation = m_targetObject.rotation;
        //zResult.fov = m_fov;
        zResult.fov = m_targetObject.GetComponent<Camera>().fieldOfView;
    }

    public override float TimeToEnd(CameraState zResult)
    {
        m_delayTime += Time.deltaTime;
        var endStateReached = false;
        if (!m_animator.IsInTransition(0))
        {
            var stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
            endStateReached =
                stateInfo.IsName(m_animationName)
                && stateInfo.normalizedTime >= 1 && m_delayTime > 0.3F;
        }
        else
        {
            return 0;
        }
        return endStateReached ? 0 : 1;
    }
}
