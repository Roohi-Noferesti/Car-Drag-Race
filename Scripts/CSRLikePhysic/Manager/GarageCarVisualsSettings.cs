using System;
using UnityEngine;

public class GarageCarVisualsSettings : MonoBehaviour
{
	public Light directionalLight;

	public Light ambientLight;

	public GameObject carPlacementNode;

	public static GarageCarVisualsSettings Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		GarageCarVisualsSettings.Instance = this;
	}

	private void OnDestroy()
	{
		GarageCarVisualsSettings.Instance = null;
	}
}
