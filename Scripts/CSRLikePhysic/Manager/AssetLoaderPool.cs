using System;
using System.Collections.Generic;
using System.Linq;

public class AssetLoaderPool
{
	private List<CachedLoader> loaders;

	private static AssetLoaderPool _instance;

	public static AssetLoaderPool Instance
	{
		get
		{
			if (AssetLoaderPool._instance == null)
			{
				AssetLoaderPool._instance = new AssetLoaderPool();
			}
			return AssetLoaderPool._instance;
		}
	}

	public bool isLoading
	{
		get
		{
			return this.loaders.Count > 0;
		}
	}

	public bool IsLoadingOffNet
	{
		get
		{
			return this.loaders.Any((CachedLoader loader) => loader.GetURL().StartsWith("http://"));
		}
	}

	public AssetLoaderPool()
	{
		this.loaders = new List<CachedLoader>();
	}

	public void Add(CachedLoader loader)
	{
		this.loaders.Add(loader);
	}

	public void Remove(CachedLoader loader)
	{
		this.loaders.Remove(loader);
	}

	public void Clear()
	{
		foreach (CachedLoader current in this.loaders)
		{
			current.CompleteFailed();
		}
		this.loaders.Clear();
	}

	public CachedLoader Find(string zAssetID, int zVersion)
	{
		return this.loaders.Find((CachedLoader q) => q.assetID == zAssetID && q.version == zVersion);
	}
}
