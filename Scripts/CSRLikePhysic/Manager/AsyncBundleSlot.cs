using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AsyncBundleSlot : MonoBehaviour, IBundleOwner
{
	public AsyncBundleSlotDescription description;

	private bool reusingAsset;

	private List<AsyncBundleCallback> requestCallbacks = new List<AsyncBundleCallback>();

	private bool bundleLoadedOk;

	private AssetBundle bundle;

	private string bundleAssetID;

	public string requestedAssetName
	{
		get;
		private set;
	}

	public string currentAssetName
	{
		get;
		private set;
	}

	public GameObject theObj
	{
		get;
		private set;
	}

	public string BundleAssetID
	{
		get
		{
			return this.bundleAssetID;
		}
	}

	public bool Loading
	{
		get;
		private set;
	}

	public void ReleaseAssetBundle(string zAssetID)
	{
		AssetProviderClient.Instance.ReleaseAsset(zAssetID, this);
	}

	public void ClearBundle()
	{
		this.bundle = null;
		if (this.bundleAssetID != null)
		{
			this.ReleaseAssetBundle(this.bundleAssetID);
			this.bundleAssetID = null;
		}
	}

	public void FullyDestroyObject()
	{
		if (this.theObj == null)
		{
			return;
		}
		this.currentAssetName = null;
		UnityEngine.Object.DestroyImmediate(this.theObj);
		this.theObj = null;
	}

	public void RequestAsset(string assetName, BundleCallbackDelegate callback, GameObject owner)
	{
		this.RequestAsset(assetName, callback, owner, false, null);
	}

	public void RequestAsset(string assetName, BundleCallbackDelegate callback, GameObject owner, bool reuseAsset, BundleFailureCallbackDelegate failureCallback = null)
    {
        assetName = assetName.ToLower();
		this.reusingAsset = false;
		AsyncBundleCallback item = new AsyncBundleCallback(assetName, callback, owner, failureCallback);
		this.ClearRequestCallbacks();
		this.requestCallbacks.Add(item);
		if (reuseAsset && assetName == this.currentAssetName && this.theObj != null)
		{
			this.reusingAsset = true;
			if (!this.theObj.activeInHierarchy)
			{
				this.theObj.SetActive(true);
			}
			this.CallCallbacks(this.currentAssetName);
			return;
		}
		this.Loading = true;
		AssetProviderClient.Instance.RequestAsset(assetName, new BundleLoadedDelegate(this.BundleReady), this);
		this.requestedAssetName = assetName;
	}

	public void BundleReady(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
	{
		this.Loading = false;
		this.bundleLoadedOk = true;
		if (this.requestedAssetName != zAssetID)
		{
			this.ReleaseAssetBundle(zAssetID);
			this.ClearRequestCallbacks(zAssetID);
			return;
		}
		this.bundleAssetID = zAssetID;
		this.bundle = zAssetBundle;
		this.CallCallbacks(this.bundleAssetID);
	}

	public void CallCallbacks(string assetID)
	{
		List<AsyncBundleCallback> list = this.requestCallbacks.FindAll((AsyncBundleCallback q) => q.name == assetID);
		foreach (AsyncBundleCallback current in list)
		{
			current.callback(this.bundleLoadedOk, assetID);
			this.requestCallbacks.Remove(current);
		}
	}

	public GameObject MakeObject()
	{
		if (this.reusingAsset)
		{
		}
		if (this.theObj != null && this.reusingAsset)
		{
			this.reusingAsset = false;
			return this.theObj;
		}
		if (this.theObj != null)
		{
			this.FullyDestroyObject();
		}
		if (this.bundle == null)
		{
			return null;
		}

		GameObject gameObject = this.bundle.LoadAsset(this.bundle.mainAsset(), typeof(GameObject)) as GameObject;
		if (gameObject == null)
		{
			return null;
		}
		this.theObj = (UnityEngine.Object.Instantiate(gameObject) as GameObject);
		UnityEngine.Object.DontDestroyOnLoad(this.theObj);
		this.currentAssetName = this.bundleAssetID;

#if UNITY_EDITOR
        //Fixing shader bugs // it may occure just in editor so make sure to appy it in UNITY_EDITOR only
        var rends = theObj.GetComponentsInChildren<Renderer>();
        var projects = theObj.GetComponentsInChildren<Projector>();
        foreach (var rend in rends)
        {
            rend.material.shader = Shader.Find(rend.material.shader.name);
        }
        foreach (var proj in projects)
        {
            if (proj.material != null)
                proj.material.shader = Shader.Find(proj.material.shader.name);
            else
            {
                //Debug.Log(proj.name + "  material is null");
            }
        }
#endif


		return this.theObj;
	}

	public void ClearMemory()
	{
		this.FullyDestroyObject();
	}

	private void CallFailureCallbacks(List<AsyncBundleCallback> failedRequestCallbacks)
	{
		foreach (AsyncBundleCallback current in failedRequestCallbacks)
		{
			if (current.failureCallback != null)
			{
				current.failureCallback();
			}
		}
	}

	public void ClearRequestCallbacks()
	{
		this.CallFailureCallbacks(this.requestCallbacks);
		this.requestCallbacks.Clear();
	}

	public void ClearRequestCallbacks(GameObject owner)
	{
		this.CallFailureCallbacks(this.requestCallbacks.FindAll((AsyncBundleCallback q) => q.owner == owner));
		this.requestCallbacks = this.requestCallbacks.FindAll((AsyncBundleCallback q) => q.owner != owner);
	}

	public void ClearRequestCallbacks(string assetName)
	{
		this.CallFailureCallbacks(this.requestCallbacks.FindAll((AsyncBundleCallback q) => q.name == assetName));
		this.requestCallbacks = this.requestCallbacks.FindAll((AsyncBundleCallback q) => q.name != assetName);
	}

	[Conditional("CSR_DEBUG_LOGGING")]
	public void SlotDebug(string output)
	{
	}
}
