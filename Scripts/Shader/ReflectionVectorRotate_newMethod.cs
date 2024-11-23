using UnityEngine;

[ExecuteInEditMode]
public class ReflectionVectorRotate_newMethod : MonoBehaviour
{


    [SerializeField]
    private float _Speed = 200;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        Matrix4x4 m1 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-(_Speed), 0, 0), Vector3.one);

        GetComponent<Renderer>().sharedMaterial.SetMatrix("_myMatrix_up", m1);


    }
}
