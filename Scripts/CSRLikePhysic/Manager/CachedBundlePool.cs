using System;
using System.Collections.Generic;
using UnityEngine;

public static class CachedBundlePool
{
	public static List<CachedBundle> cache
	{
		get;
		private set;
	}

	static CachedBundlePool()
	{
		CachedBundlePool.cache = new List<CachedBundle>();
	}

	public static CachedBundle BundleArrived(CachedLoader loader, AssetBundle assetBundle)
	{
		CachedBundle cachedBundle = CachedBundlePool.TouchCachedBundle(loader.assetID, loader.version);
		if (cachedBundle == null)
		{
			cachedBundle = new CachedBundle(loader.assetID, loader.version, assetBundle);
			CachedBundlePool.cache.Add(cachedBundle);
		}
		return cachedBundle;
	}

	public static void ClearCache()
	{
		List<CachedBundle> list = CachedBundlePool.cache.FindAll((CachedBundle q) => q.refCount == 0);
		foreach (CachedBundle current in list)
		{
			CachedBundlePool.RemoveBundle(current);
		}
	}

	public static void RemoveBundle(CachedBundle bundle)
	{
		CachedBundlePool.cache.Remove(bundle);
		bundle.Clear();
	}

	public static void DeleteAllBundlesFromMemory()
	{
		CachedBundlePool.ClearCache();
		if (CachedBundlePool.cache.Count > 0)
		{
			for (int i = CachedBundlePool.cache.Count - 1; i >= 0; i--)
			{
				CachedBundle bundle = CachedBundlePool.cache[i];
				CachedBundlePool.RemoveBundle(bundle);
			}
		}
	}

	public static CachedBundle GetCachedBundle(string assetID, int version)
	{
		return CachedBundlePool.cache.Find((CachedBundle q) => q.assetID == assetID && q.version == version);
	}

	public static CachedBundle GetCachedBundleAnyVersion(string assetID)
	{
		CachedBundle cachedBundle = CachedBundlePool.cache.Find((CachedBundle q) => q.assetID == assetID);
		if (cachedBundle != null)
		{
		}
		return cachedBundle;
	}

	private static CachedBundle TouchCachedBundle(string assetID, int version)
	{
		CachedBundle cachedBundle = CachedBundlePool.cache.Find((CachedBundle q) => q.assetID == assetID && q.version == version);
		if (cachedBundle != null)
		{
			CachedBundlePool.cache.Remove(cachedBundle);
			CachedBundlePool.cache.Add(cachedBundle);
		}
		return cachedBundle;
	}
}
