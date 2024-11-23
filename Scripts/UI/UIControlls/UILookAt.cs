using UnityEngine;

public class UILookAt : MonoBehaviour
{
    [SerializeField] private float m_lerpSpeed = 1;
    [SerializeField] private float m_direction = 1;

    private Quaternion m_targetRotation;

    void Update()
    {
        var dir = (MapCamera.Instance.transform.position - transform.position).normalized * m_direction;
        m_targetRotation = Quaternion.LookRotation(dir);
        var euler = m_targetRotation.eulerAngles;
        euler.y = 0;
        m_targetRotation = Quaternion.Euler(euler);
        transform.rotation = Quaternion.Lerp(transform.rotation, m_targetRotation, Time.deltaTime * m_lerpSpeed);
    }
}
