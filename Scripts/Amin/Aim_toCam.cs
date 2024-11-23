using UnityEngine;

public class Aim_toCam : MonoBehaviour
{
    private Transform m_mainCamera;
	// Use this for initialization
	void Start ()
	{
	    m_mainCamera = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {

        transform.rotation = Quaternion.LookRotation(-m_mainCamera.forward);
	
	}
}
