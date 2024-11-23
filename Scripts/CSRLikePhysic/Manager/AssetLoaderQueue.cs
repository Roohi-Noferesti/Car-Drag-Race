using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class AssetLoaderQueue : MonoBehaviour
{
	public static bool slowDownloads;

	private List<CachedLoader> loaders;

	private List<CachedLoader> activeLoaders;

	private int maxConcurrentLoads = 1;

	public bool isBlocking;

	public List<CachedLoader> Loaders
	{
		get
		{
			return this.loaders;
		}
	}

	public bool IsEmpty
	{
		get
		{
			return this.loaders.Count == 0;
		}
	}

	private void Awake()
	{
		this.loaders = new List<CachedLoader>();
		this.activeLoaders = new List<CachedLoader>();
	}

	public int ActiveSlots()
	{
		return this.activeLoaders.Count;
	}

	public int AvailableSlots()
	{
		return this.maxConcurrentLoads - this.activeLoaders.Count;
	}

	public void SetMaxLoads(int zMaxConcurrentLoads)
	{
		this.maxConcurrentLoads = zMaxConcurrentLoads;
	}

	public CachedLoader Find(string zAssetID, int zVersion)
	{
		return AssetLoaderPool.Instance.Find(zAssetID, zVersion);
	}

	public void StartLoading(CachedLoader loader)
	{
		base.StartCoroutine(this.StartLoadingCoroutine(loader));
	}

    private IEnumerator StartLoadingCoroutine(CachedLoader loader)
    {
        //Debug.Log("start loading www : " + loader.GetURL());
        CachedBundle cachedBundle = null;
        loaders.Add(loader);
        AssetLoaderPool.Instance.Add(loader);
        cachedBundle = CachedBundlePool.GetCachedBundle(loader.assetID, loader.version);
        if (cachedBundle != null)
        {
            AssetLoaderPool.Instance.Remove(loader);
            loaders.Remove(loader);
            yield return new WaitForFixedUpdate();
            loader.Complete(cachedBundle);
            yield break;
        }

        while (activeLoaders.Count > maxConcurrentLoads)
        {
            yield return new WaitForFixedUpdate();
        }
        var cached = CachedBundlePool.GetCachedBundleAnyVersion(loader.assetID);
        if (cached != null)
        {
            CachedBundlePool.RemoveBundle(cached);
        }
        var url = loader.GetURL();
        activeLoaders.Add(loader);
        //var www = WWW.LoadFromCacheOrDownload(url, loader.version);
        var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(new Uri(url));//, (uint)loader.version);
        //Debug.Log("www created by url : " + url);
        yield return webRequest.SendWebRequest();
        //yield return www;
        if (!string.IsNullOrEmpty(webRequest.error))
        {
            //Debug.Log("error on loading www : " + url + "   ,    error : " + webRequest.error);
        }
        else
        {
            //Debug.Log("www loaded url : " + url);
        }
        if (AssetLoaderQueue.slowDownloads)
        {
            yield return new WaitForSeconds(10);
        }
        AssetLoaderPool.Instance.Remove(loader);
        loaders.Remove(loader);
        loader.Complete(webRequest);
        activeLoaders.Remove(loader);
    }

    public void Clear()
	{
		foreach (CachedLoader current in this.loaders)
		{
			current.CompleteFailed();
			AssetLoaderPool.Instance.Remove(current);
		}
		this.loaders.Clear();
	}
}
