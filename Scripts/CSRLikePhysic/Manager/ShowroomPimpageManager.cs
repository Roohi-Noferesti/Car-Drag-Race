using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ShowroomPimpageManager : MonoBehaviour
{
	private static ShowroomPimpageManager _instance;

	public GameObject ProjectorMesh;

	public GameObject Lasers;

	public List<Color> ShowroomColours;

	//private Color ColorAccent = new Color(0.121f, 0.447f, 0.639f, 1f);

	public static ShowroomPimpageManager Instance
	{
		get
		{
			return ShowroomPimpageManager._instance;
		}
		set
		{
		}
	}

	private void Start()
	{
		ShowroomPimpageManager._instance = base.gameObject.GetComponent<ShowroomPimpageManager>();
		this.ProjectorMesh = GameObject.Find("ProjectorMesh");
		this.Lasers = new GameObject("Lasers");
		this.Lasers.transform.parent = GameObject.Find("Showroom").transform;
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefab/Laser")) as GameObject;
		gameObject.transform.parent = this.Lasers.transform;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("Prefab/Laser")) as GameObject;
		gameObject2.transform.position = new Vector3(-gameObject2.transform.position.x, gameObject2.transform.position.y, gameObject2.transform.position.z);
		gameObject2.transform.rotation = Quaternion.Euler(gameObject2.transform.localEulerAngles.x, -gameObject2.transform.localEulerAngles.y, gameObject2.transform.localEulerAngles.z);
		gameObject2.transform.parent = this.Lasers.transform;
		this.Lasers.AddComponent<AdvLaserManager>().DetectLasers();
	}

	public static void UpdateColour()
	{
		if (ShowroomPimpageManager._instance == null)
		{
			return;
		}
		if (ShowroomPimpageManager._instance.ShowroomColours == null)
		{
			return;
		}
		int currentColorIndex = SceneManagerShowroom.Instance.currentCarVisuals.GetCurrentColorIndex();
		if (ShowroomPimpageManager._instance.ShowroomColours.Count <= currentColorIndex)
		{
			return;
		}
		ShowroomPimpageManager._instance.StartCoroutine(ShowroomPimpageManager._instance.GoToColor(ShowroomPimpageManager._instance.ShowroomColours[currentColorIndex], 1f));
	}

	public IEnumerator GoToColor(Color goToColor, float time)
	{
	    //ShowroomPimpageManager.<GoToColor>c__Iterator15 <GoToColor>c__Iterator = new ShowroomPimpageManager.<GoToColor>c__Iterator15();
        //<GoToColor>c__Iterator.goToColor = goToColor;
        //<GoToColor>c__Iterator.time = time;
        //<GoToColor>c__Iterator.<$>goToColor = goToColor;
        //<GoToColor>c__Iterator.<$>time = time;
        //<GoToColor>c__Iterator.<>f__this = this;
        //return <GoToColor>c__Iterator;
	    return null;
	}
}
