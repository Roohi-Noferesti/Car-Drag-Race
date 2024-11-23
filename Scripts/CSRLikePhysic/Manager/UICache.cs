using System;
using System.Collections.Generic;
using UnityEngine;

public class UICache : MonoBehaviour
{
	public int numElements = 5;

	public string prefabPathToCache;

	private Stack<GameObject> unused;

	private Stack<GameObject> used_Auto;

	private List<GameObject> used_Manual;

	public void Initialise()
	{
		this.unused = new Stack<GameObject>();
		this.used_Auto = new Stack<GameObject>();
		this.used_Manual = new List<GameObject>();
		GameObject gameObject = Resources.Load(this.prefabPathToCache) as GameObject;
		if (gameObject == null)
		{
			return;
		}
		for (int i = 0; i < this.numElements; i++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
			this.unused.Push(gameObject2);
			gameObject2.transform.parent = base.transform;
			gameObject2.SetActive(false);
		}
	}

	public void Destroy()
	{
		if (this.used_Auto.Count != 0)
		{
			foreach (GameObject current in this.used_Auto)
			{
			}
		}
		if (this.used_Manual.Count != 0)
		{
			foreach (GameObject current2 in this.used_Manual)
			{
			}
		}
		while (this.unused.Count != 0)
		{
			GameObject obj = this.unused.Pop();
			UnityEngine.Object.Destroy(obj);
		}
	}

	public GameObject GetItem()
	{
		if (this.unused.Count == 0)
		{
			return null;
		}
		GameObject gameObject = this.unused.Pop();
		this.used_Auto.Push(gameObject);
		this.ResetItemState(gameObject.gameObject);
		gameObject.gameObject.SetActive(true);
		return gameObject;
	}

	public GameObject GetItemManualRelease()
	{
		if (this.unused.Count == 0)
		{
			return null;
		}
		GameObject gameObject = this.unused.Pop();
		this.used_Manual.Add(gameObject);
		this.ResetItemState(gameObject.gameObject);
		gameObject.gameObject.SetActive(true);
		return gameObject;
	}

	public void ReleaseAutoItems()
	{
		while (this.used_Auto.Count != 0)
		{
			GameObject gameObject = this.used_Auto.Pop();
			if (!(gameObject == null))
			{
				gameObject.transform.parent = base.transform;
				gameObject.SetActive(false);
				this.unused.Push(gameObject);
			}
		}
	}

	public bool ReleaseManualItem(GameObject zObject)
	{
		bool flag = this.used_Manual.Remove(zObject);
		if (flag)
		{
			this.unused.Push(zObject);
			zObject.transform.parent = base.transform;
			zObject.SetActive(false);
		}
		return flag;
	}

	private void ResetItemState(GameObject zObject)
	{
		this.SetActiveRecursivelyLegacy(zObject, true);
	}

	private void SetActiveRecursivelyLegacy(GameObject go, bool active)
	{
		foreach (Transform transform in go.transform)
		{
			this.SetActiveRecursivelyLegacy(transform.gameObject, active);
		}
	}
}
