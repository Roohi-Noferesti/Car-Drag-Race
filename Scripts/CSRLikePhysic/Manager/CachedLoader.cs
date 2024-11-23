using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CachedLoader
{
	public enum State
	{
		ERROR,
		WAITING_FOR_LOAD,
		LOADING,
		COMPLETE
	}

	public string assetID;

	public string filePath;

	public CachedLoader.State state;

	private List<FileLoadRequest> requests;

	public float timeSinceFinishedLoading;

	public AssetQuality webQuality;

	public int version
	{
		get;
		private set;
	}

	public CachedLoader(string zAssetID, AssetLoaderQueue zQueue, int zVersion)
	{
		this.assetID = zAssetID;
		this.version = zVersion;
		this.state = CachedLoader.State.WAITING_FOR_LOAD;
		this.requests = new List<FileLoadRequest>();
		zQueue.StartLoading(this);
	}

	public void AddNewRequest(BundleLoadedDelegate zReadyDelegate, IBundleOwner zOwner)
	{
		FileLoadRequest item = new FileLoadRequest(zReadyDelegate, zOwner);
		this.requests.Insert(0, item);
	}

	public FileLoadRequest FindRequest(BundleLoadedDelegate zReadyDelegate, IBundleOwner zOwner)
	{
		return this.requests.Find((FileLoadRequest q) => q.Matches(zReadyDelegate, zOwner));
	}

	public bool ClearRequestsFromOwner(IBundleOwner zOwner)
	{
		int num = this.requests.RemoveAll((FileLoadRequest q) => q.owner == zOwner);
		return num > 0;
	}

	public string GetURL()
	{
        //Debug.Log("Getting asset url for asset '" + assetID + "'");
        AssetDirectoryEntry assetDirectoryEntry = AssetDatabaseClient.Instance.Directory.GetAssetDirectoryEntry(this.assetID);
        if (assetDirectoryEntry != null)
        {
            assetDirectoryEntry = !assetDirectoryEntry.isLocal || assetDirectoryEntry.version != this.version ? null : assetDirectoryEntry;
        }
        string result;
        if (assetDirectoryEntry != null)
        {
#if UNITY_EDITOR_WIN || UNITY_WIN || UNITY_METRO || UNITY_EDITOR_OSX
            result = "file:///" + AssetDatabaseClient.Instance.Directory.GetBundlePath(this.assetID);
#elif UNITY_ANDROID
            result = AssetDatabaseClient.Instance.Directory.GetBundlePath(this.assetID);
#else
            result = "file://" + AssetDatabaseClient.Instance.Directory.GetBundlePath(this.assetID);
#endif
            //Debug.Log("url found : " + result);
        }
        else
        {
            //TODO uncomment to add platform and qulity
            string runtimeName = "ANDROID";//GTPlatform.RuntimeName;
            this.webQuality = AssetQuality.High;//BaseDevice.ActiveDevice.DeviceQuality;
            string qualityString = this.webQuality.ToString();
            string url = string.Concat(new string[]
			{
				runtimeName,
				"/",
				qualityString,
				"/",
				this.assetID,
				".",
				qualityString,
				".",
				this.version.ToString()
			});
            url = url.Replace("\\", "/");
            GTDebug.Log(GTLogChannel.CachedLoader,url);
            result = Endpoint.GetS3URL(url);
            GTDebug.Log(GTLogChannel.CachedLoader,"asset directory is null . getting url from  '" + result + "'");
        }
        return result;
	}

	public void Complete(UnityWebRequest webRequest)
	{
		if (this.requests.Count < 1)
        {
            //Debug.Log("loading completed 0 for asset : " + webRequest.url);
            webRequest.Dispose();
			return;
		}
		if (!string.IsNullOrEmpty(webRequest.error))
		{
            //Debug.Log("loading completed 1 for asset : " + webRequest.url);
            this.state = CachedLoader.State.ERROR;
			this.CompleteFailed();
			return;
		}

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
        if (bundle == null)
		{
            //Debug.Log("loading completed 2 for asset : " + webRequest.url);
            this.state = CachedLoader.State.ERROR;
			this.CompleteFailed();
			return;
		}
		CachedBundle cachedBundle = CachedBundlePool.BundleArrived(this, bundle);
		if (cachedBundle == null || this.assetID != cachedBundle.assetID)
		{
		}
		this.Complete(cachedBundle);
	}

	public void Complete(CachedBundle cachedBundle)
	{
        //Debug.Log("loading completed 3 for asset : " + cachedBundle.assetID);
        if (this.requests.Count < 1)
		{
            //Debug.Log("loading completed 01 for asset : " + cachedBundle.assetID);
            return;
		}
		this.requests.ForEach(delegate(FileLoadRequest request)
		{
			cachedBundle.AddRef();
		});
		AssetBundle assetBundle = cachedBundle.assetBundle;
		this.requests.ForEach(delegate(FileLoadRequest request)
		{
			request.Success(this.assetID, assetBundle);
		});
	}

	public void CompleteFailed()
	{
		AssetDirectoryEntry assetDirectoryEntry = AssetDatabaseClient.Instance.Directory.GetAssetDirectoryEntry(this.assetID);
		string arg = "assetID: " + this.assetID;
		arg = arg + " versionRequested: " + this.version;
		if (assetDirectoryEntry != null)
		{
			arg = arg + " assetDatabaseEntryVersion: " + assetDirectoryEntry.version;
			arg = arg + " assetDatabaseEntryQuality: " + assetDirectoryEntry.quality;
		}
		arg = arg + " assetDatabaseVersion: " + AssetDatabaseClient.Instance.Data.GetVersion();
		this.requests.ForEach(delegate(FileLoadRequest request)
		{
		});
		this.state = CachedLoader.State.ERROR;
	}
}
