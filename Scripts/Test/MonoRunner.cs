using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoRunner : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        var monotest = gameObject.AddComponent<MonoTest>();
        monotest.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
