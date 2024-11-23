using System;
using System.Collections.Generic;
using UnityEngine;

public class AsyncSwitching : MonoBehaviour
{
	private List<AsyncBundleSlot> bundleSlots;

	public GameObject switchingParent;

	public static AsyncSwitching Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		AsyncSwitching.Instance = this;
		this.switchingParent = base.gameObject;
		this.bundleSlots = new List<AsyncBundleSlot>
		{
			this.MakeSlot(AsyncBundleSlotDescription.HumanCar),
			this.MakeSlot(AsyncBundleSlotDescription.AICar),
			this.MakeSlot(AsyncBundleSlotDescription.HumanCarLivery),
			this.MakeSlot(AsyncBundleSlotDescription.AICarLivery),
			this.MakeSlot(AsyncBundleSlotDescription.ManufacturerLogo)
		};
		this.ClearMemory();
	}

	private void OnDestroy()
	{
		foreach (AsyncBundleSlot current in this.bundleSlots)
		{
			if (current != null && current.gameObject != null)
			{
				UnityEngine.Object.Destroy(current.gameObject);
			}
		}
		this.bundleSlots.Clear();
		this.bundleSlots = null;
	}

	private AsyncBundleSlot MakeSlot(AsyncBundleSlotDescription desc)
	{
		GameObject go = new GameObject();
		go.name = "AsyncSlot_" + desc.ToString();
		go.transform.parent = base.transform;
		//UnityEngine.Object.DontDestroyOnLoad(gameObject);
		go.AddComponent<AsyncBundleSlot>();
		AsyncBundleSlot component = go.GetComponent<AsyncBundleSlot>();
		component.description = desc;
		return component;
	}

	public bool SlotIsLoading(AsyncBundleSlotDescription desc)
	{
		AsyncBundleSlot asyncBundleSlot = this.bundleSlots[(int)desc];
		return asyncBundleSlot.Loading;
	}

	public void RequestAsset(AsyncBundleSlotDescription desc, string name, BundleCallbackDelegate callMe, GameObject owner)
	{
		this.RequestAsset(desc, name, callMe, owner, false, null);
	}

	public void RequestAsset(AsyncBundleSlotDescription desc, string name, BundleCallbackDelegate callMe, GameObject owner, bool useExisting, BundleFailureCallbackDelegate failureCallback = null)
	{
		AsyncBundleSlot asyncBundleSlot = this.bundleSlots[(int)desc];
		asyncBundleSlot.RequestAsset(name, callMe, owner, useExisting, failureCallback);
	}

	public void ClearCallbacks(AsyncBundleSlotDescription slotDesc, GameObject owner)
	{
		if (this.bundleSlots == null)
		{
			return;
		}
		AsyncBundleSlot asyncBundleSlot = this.bundleSlots[(int)slotDesc];
		asyncBundleSlot.ClearRequestCallbacks(owner);
	}

	public void ClearSlot(AsyncBundleSlotDescription slotDesc)
	{
		if (this.bundleSlots == null)
		{
			return;
		}
		AsyncBundleSlot asyncBundleSlot = this.bundleSlots[(int)slotDesc];
		asyncBundleSlot.ClearRequestCallbacks();
		asyncBundleSlot.ClearMemory();
	}

	public void ClearMemory()
	{
		if (this.bundleSlots == null)
		{
			return;
		}
		foreach (AsyncBundleSlot current in this.bundleSlots)
		{
			current.ClearMemory();
		}
	}

	public string RequestedName(AsyncBundleSlotDescription slotDesc)
	{
		if (this.bundleSlots == null)
		{
			return null;
		}
		AsyncBundleSlot asyncBundleSlot = this.bundleSlots[(int)slotDesc];
		return asyncBundleSlot.requestedAssetName;
	}

	public string CurrentName(AsyncBundleSlotDescription slotDesc)
	{
		if (this.bundleSlots == null)
		{
			return null;
		}
		AsyncBundleSlot asyncBundleSlot = this.bundleSlots[(int)slotDesc];
		return asyncBundleSlot.currentAssetName;
	}

	public GameObject CurrentObject(AsyncBundleSlotDescription slotDesc)
	{
		if (this.bundleSlots == null)
		{
			return null;
		}
		AsyncBundleSlot asyncBundleSlot = this.bundleSlots[(int)slotDesc];
		return asyncBundleSlot.theObj;
	}

	public GameObject GetCar(AsyncBundleSlotDescription slotDesc)
	{
		AsyncBundleSlot asyncBundleSlot = this.bundleSlots[(int)slotDesc];
		GameObject theObj = asyncBundleSlot.theObj;
		GameObject gameObject = asyncBundleSlot.MakeObject();
		if (gameObject != theObj)
		{
			CleanDownManager.Instance.OnCarLoadingFinished();
		}
		gameObject.transform.parent = this.switchingParent.transform;
		return gameObject;
	}

	public GameObject GetLivery(AsyncBundleSlotDescription slotDesc)
	{
		AsyncBundleSlot asyncBundleSlot = this.bundleSlots[(int)slotDesc];
		GameObject gameObject = asyncBundleSlot.MakeObject();
		if (gameObject == null)
		{
			return null;
		}
		gameObject.transform.parent = this.switchingParent.transform;
		return gameObject;
	}

	public GameObject GetLogo(AsyncBundleSlotDescription slotDesc)
	{
		AsyncBundleSlot asyncBundleSlot = this.bundleSlots[(int)slotDesc];
		return asyncBundleSlot.MakeObject();
	}

	public static bool IsLiveryName(string inName)
    {
        return false;
		//return !string.IsNullOrEmpty(inName) && inName != "No Livery" && inName.ToLower().Contains("livery");
	}
}
