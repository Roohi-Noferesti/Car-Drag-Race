using UnityEngine;

public class Aim2Cam : MonoBehaviour {

	[SerializeField] Camera MainCam ;
	[SerializeField] Transform CarBody ;
	[SerializeField] float _size = .5f ;
	[SerializeField] float _Add = 1f ;


	Vector3 InitialScale;


	// Use this for initialization
	void Start () {
		InitialScale = transform.localScale;
        MainCam = Camera.main;
		//Debug.Log (SystemInfo.deviceUniqueIdentifier);
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.LookRotation(MainCam.transform.forward);

		float CarForward = Vector3.Dot (CarBody.transform.forward, -MainCam.transform.forward);
		transform.localScale = InitialScale * ((CarForward+_Add)*_size);
			
	}
}
