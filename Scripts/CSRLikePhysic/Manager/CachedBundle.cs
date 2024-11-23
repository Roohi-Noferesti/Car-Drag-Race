using System;
using UnityEngine;

public class CachedBundle
{
	public string assetID
	{
		get;
		private set;
	}

	public int version
	{
		get;
		private set;
	}

	public int refCount
	{
		get;
		private set;
	}

	public AssetBundle assetBundle
	{
		get;
		private set;
	}

	public CachedBundle(string zAssetID, int zVersion, AssetBundle zAssetBundle)
	{
		if (zAssetBundle == null)
		{
		}
		this.assetID = zAssetID;
		this.version = zVersion;
		this.refCount = 0;
		this.assetBundle = zAssetBundle;
	}

	public void AddRef()
	{
		this.refCount++;
	}

	public void DeleteRef()
	{
		this.refCount--;
		if (this.refCount < 0)
		{
			this.refCount = 0;
		}
		if (this.refCount == 0)
		{
			CachedBundlePool.RemoveBundle(this);
		}
	}

	public void Clear()
	{
		this.assetID = null;
		this.version = 0;
		this.assetBundle.Unload(false);
		this.assetBundle = null;
	}
}
