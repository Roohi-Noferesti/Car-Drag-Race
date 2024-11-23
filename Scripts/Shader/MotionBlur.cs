using UnityEngine;
using System.Collections;

public class MotionBlur : MonoBehaviour {


	private AB_ImageBlur_Raw Mult;
	private static float m_maxSpeed = 200;


	// Use this for initialization
	void Start () 
	{   
        Mult = GetComponent<AB_ImageBlur_Raw>();
	    if (EnvQualitySettings.EnvSceneQuality == EnvSceneQuality.Low ||
	        EnvQualitySettings.EnvSceneQuality == EnvSceneQuality.Medium)
	    {
           Mult.enabled = false;
	    }
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (CompetitorManager.Instance.LocalCompetitor != null
			&& CompetitorManager.Instance.LocalCompetitor.CarPhysics!=null)
		{
			var speed = CompetitorManager.Instance.LocalCompetitor.CarPhysics.SpeedKPH;
			Mult.Mult =  speed/m_maxSpeed;
		}
	}
}
