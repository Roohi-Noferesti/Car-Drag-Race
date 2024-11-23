using UnityEngine;

public class FinishCamera : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float m_zOffset = 3;
    [SerializeField] private float m_maxAngle = 270;
    void OnEnable()
    {
        transform.LookAt(target);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    transform.LookAt(target.position + Vector3.forward*m_zOffset);
        if (transform.eulerAngles.y > m_maxAngle)
	        enabled = false;
	}
}
