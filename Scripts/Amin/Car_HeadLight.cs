using UnityEngine;

public class Car_HeadLight : MonoBehaviour
{



    private Light myLight;
    private float newVal;

    [SerializeField] private Transform _camera;
    [SerializeField] private float _intensity = 3f;

    public Transform CameraTransform
    {
        set { _camera = value; }
        get { return _camera; }
    }




	// Use this for initialization
	void Start ()
	{
        //if (!_camera)
        //{
        //    Debug.Log("Car_HeadLight Script needs Camera");
        //    enabled = false;
        //}
            myLight = GetComponent<Light>();
	}
	
	// Update is called once per frame
    private void Update()
    {
        if (_camera != null)
            myLight.intensity = Mathf.Clamp(Vector3.Dot(_camera.forward, -myLight.transform.forward), 0, 10)*_intensity;
    }
}
