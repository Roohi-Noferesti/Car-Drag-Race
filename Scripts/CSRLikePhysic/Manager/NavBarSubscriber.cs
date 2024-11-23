using System;
using UnityEngine;

public class NavBarSubscriber
{
	public GameObject obj
	{
		get;
		private set;
	}

	public Vector3 origin
	{
		get;
		private set;
	}

	public NavBarSubscriber(GameObject inObj)
	{
		this.obj = inObj;
		this.origin = this.obj.transform.localPosition;
	}
}
