using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : class
{
	private static T _instance;

	public static T Instance
	{
		get
		{
			if (Singleton<T>._instance == null)
			{
			}
			return Singleton<T>._instance;
		}
		protected set
		{
			if (Singleton<T>._instance == null)
			{
				if (value == null)
				{
				}
			}
			else if (value != null)
			{
			}
			Singleton<T>._instance = value;
		}
	}

	public static bool Instantiated
	{
		get
		{
			return _instance != null;
		}
	}

	public virtual void Awake()
	{
		Singleton<T>._instance = (this as T);
	}

	public virtual void OnDestroy()
	{
		Singleton<T>._instance = (T)((object)null);
	}
}
