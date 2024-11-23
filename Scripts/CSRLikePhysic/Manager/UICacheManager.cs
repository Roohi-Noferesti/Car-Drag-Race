using System;
using System.Collections.Generic;
using UnityEngine;

public class UICacheManager : MonoBehaviour
{
	private Dictionary<string, UICache> cachesWithPath;

	public bool CachingEnabled = true;

	public static UICacheManager Instance
	{
		get;
		private set;
	}

	public static void Create()
	{
		GameObject original = Resources.Load("Prefabs/UICacheManager") as GameObject;
		UnityEngine.Object.Instantiate(original);
	}

	private void Awake()
	{
		UICacheManager.Instance = this;
	}

	public void CreateInstances()
	{
		if (!this.CachingEnabled)
		{
			return;
		}
		UICache[] componentsInChildren = base.GetComponentsInChildren<UICache>();
		this.cachesWithPath = new Dictionary<string, UICache>();
		UICache[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			UICache uICache = array[i];
			uICache.Initialise();
			this.cachesWithPath.Add(uICache.prefabPathToCache, uICache);
		}
	}

	public void DestroyInstances()
	{
		if (this.cachesWithPath != null)
		{
			foreach (KeyValuePair<string, UICache> current in this.cachesWithPath)
			{
				current.Value.Destroy();
			}
		}
		this.cachesWithPath = null;
	}

	public GameObject GetItem(string zResourcePath)
	{
		return this.GetItem(zResourcePath, true);
	}

	public GameObject GetItem(string zResourcePath, bool zAutoRelease)
	{
		if (this.CachingEnabled && this.cachesWithPath != null)
		{
			UICache uICache = null;
			if (this.cachesWithPath.TryGetValue(zResourcePath, out uICache))
			{
				GameObject gameObject;
				if (zAutoRelease)
				{
					gameObject = uICache.GetItem();
				}
				else
				{
					gameObject = uICache.GetItemManualRelease();
				}
				if (gameObject != null)
				{
					return gameObject;
				}
			}
		}
		UnityEngine.Object x = Resources.Load(zResourcePath);
		if (x == null)
		{
			return null;
		}
		return UnityEngine.Object.Instantiate(Resources.Load(zResourcePath)) as GameObject;
	}

	public void ReleaseAutoItems()
	{
		if (this.cachesWithPath != null)
		{
			foreach (KeyValuePair<string, UICache> current in this.cachesWithPath)
			{
				current.Value.ReleaseAutoItems();
			}
		}
	}

	public void ReleaseItem(GameObject zItem)
	{
		if (this.cachesWithPath != null)
		{
			foreach (KeyValuePair<string, UICache> current in this.cachesWithPath)
			{
				if (current.Value.ReleaseManualItem(zItem))
				{
					return;
				}
			}
		}
		UnityEngine.Object.Destroy(zItem);
	}
}
