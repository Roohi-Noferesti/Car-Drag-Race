using System;
using System.Collections.Generic;
using UnityEngine;

public class ShowroomCarVisualsSettings : MonoBehaviour
{
	public Light directionalLight;

	public Light ambientLight;

	private bool initialized;

	public static ShowroomCarVisualsSettings Instance
	{
		get;
		private set;
	}

	public List<Collider> flareColliders
	{
		get;
		private set;
	}

	private void Awake()
	{
		ShowroomCarVisualsSettings.Instance = this;
		this.flareColliders = new List<Collider>();
	}

	private void OnEnable()
	{
		if (this.initialized)
		{
			return;
		}
        GameObject[] array = GameObject.FindGameObjectsWithTag("TestForCarFlareRaycast");
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].GetComponent<Collider>() != null)
            {
                this.flareColliders.Add(array[i].GetComponent<Collider>());
            }
        }
		this.initialized = true;
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
