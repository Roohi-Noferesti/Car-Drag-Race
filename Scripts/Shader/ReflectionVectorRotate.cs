using UnityEngine;

public class ReflectionVectorRotate : MonoBehaviour
{


    [SerializeField] private float _Speed = 200;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        
        Matrix4x4 m1 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-(Time.time * _Speed), 0, 0), Vector3.one);
        Matrix4x4 m2 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, Time.time * _Speed, 0), Vector3.one);

        GetComponent<Renderer>().material.SetMatrix("_myMatrix_up" , m1);
        GetComponent<Renderer>().material.SetMatrix("_myMatrix_side", m2);


    }
}
