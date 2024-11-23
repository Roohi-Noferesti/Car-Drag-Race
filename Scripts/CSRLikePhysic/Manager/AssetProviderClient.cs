using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetProviderClient : MonoBehaviour
{
	private AssetLoaderQueue queue;

#if UNITY_EDITOR
    private AssetDatabaseConfiguration _assetdatabaseConfig;
#endif

    public static AssetProviderClient Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
		this.queue = base.gameObject.AddComponent<AssetLoaderQueue>();
		this.queue.SetMaxLoads(8);
	}

	public void ReleaseRequestsForOwner(IBundleOwner zOwner)
	{
		foreach (CachedLoader current in this.queue.Loaders)
		{
			current.ClearRequestsFromOwner(zOwner);
		}
	}

	public void ReleaseAsset(string zAssetID, IBundleOwner zOwner)
	{
		AssetDatabaseData data = AssetDatabaseClient.Instance.Data;
		int assetVersion = data.GetAssetVersion(zAssetID, false);
		CachedBundle cachedBundle = CachedBundlePool.GetCachedBundle(zAssetID, assetVersion);
		if (cachedBundle != null)
		{
			cachedBundle.DeleteRef();
			return;
		}
		this.DeleteAssetRequest(zAssetID, assetVersion, zOwner);
	}

	public bool RequestAsset(string zAssetID, BundleLoadedDelegate zReadyDelegate, IBundleOwner zOwner)
	{
        AssetDatabaseData data = AssetDatabaseClient.Instance.Data;
        int assetVersion = data.GetAssetVersion(zAssetID, false);
		CachedLoader cachedLoader = this.queue.Find(zAssetID, assetVersion);
		if (cachedLoader == null)
		{
			cachedLoader = new CachedLoader(zAssetID, this.queue, assetVersion);
		}
		else
		{
			FileLoadRequest fileLoadRequest = cachedLoader.FindRequest(zReadyDelegate, zOwner);
			if (fileLoadRequest != null)
            {
	            GTDebug.Log(GTLogChannel.AssetProviderClient,"loading failed for  : " + zAssetID + " because fileLoadRequest is already exist");
				return false;
			}
		}
		cachedLoader.AddNewRequest(zReadyDelegate, zOwner);
		return true;
	}

#if UNITY_EDITOR
    public T[] RequestAssets<T>(string zAssetID) where T:Object
    {
        List<T> result = new List<T>();
        AssetDatabaseData data = AssetDatabaseClient.Instance.Data;
        var branchName = data.GetBranch();
        if (_assetdatabaseConfig == null)
            _assetdatabaseConfig =
                AssetDatabase.LoadAssetAtPath<AssetDatabaseConfiguration>(
                    "Assets/Editor Default Resources/AssetDatabaseConfiguration.asset");
        var platform = "Android";
#if UNITY_IOS
        platform = "iOS";
#endif

        var assetGroup =
            _assetdatabaseConfig.AssetGroups.FirstOrDefault(a =>
                String.Equals((a.GroupName + "_" + platform), branchName, StringComparison.CurrentCultureIgnoreCase));
        if (assetGroup != null)
        {
            var assetData = assetGroup.AssetDatabaseAssets.FirstOrDefault(a => a.code == zAssetID);

            //Search in default group
            if (assetData == null)
            {
                var defaultGroup = _assetdatabaseConfig.AssetGroups.FirstOrDefault(a => a.IsDefault);

                if (defaultGroup != null)
                {
                    assetData = defaultGroup.AssetDatabaseAssets.FirstOrDefault(a => a.code == zAssetID);
                }
                else
                {
                    Debug.LogError("Default group of asset database not found");
                    return null;
                }
            }

            foreach (var assetDataObjectsPath in assetData.ObjectsPaths)
            {
                var assetObject = AssetDatabase.LoadAssetAtPath<T>(assetDataObjectsPath);
                if (assetObject != null)
                    result.Add(assetObject);
            }
        }

        return result.ToArray();
    }
#endif


        public void ClearQueue()
	{
		this.queue.Clear();
	}

	private void DeleteAssetRequest(string zAssetID, int zVersion, IBundleOwner zOwner)
	{
		CachedLoader cachedLoader = this.queue.Find(zAssetID, zVersion);
		if (cachedLoader != null)
		{
			cachedLoader.ClearRequestsFromOwner(zOwner);
		}
	}
}
