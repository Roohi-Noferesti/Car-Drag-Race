using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AssetDirectory
{
	public static AssetUpdatedDelegate AssetUpdatedEvent;

	public static Array qualities = Enum.GetValues(typeof(AssetQuality));

	private static List<GTAssetTypes> ignoreTypes = new List<GTAssetTypes>
	{
		GTAssetTypes.invalid,
		GTAssetTypes.vehicle_audio,
		GTAssetTypes.engine_audio,
		GTAssetTypes.upgrade,
		GTAssetTypes.music,
		GTAssetTypes.device_quality
	};

	private static List<GTAssetTypes> assetTypes = Enum.GetValues(typeof(GTAssetTypes)).Cast<GTAssetTypes>().Except(AssetDirectory.ignoreTypes).ToList<GTAssetTypes>();

	private Dictionary<string, AssetDirectoryEntry> directoryEntries = new Dictionary<string, AssetDirectoryEntry>();

	public Dictionary<string, AssetDirectoryEntry> DirectoryEntries
	{
		get
		{
			return this.directoryEntries;
		}
	}

	public string GetBundlePath(string zAssetID)
	{
		AssetDirectoryEntry assetDirectoryEntry = null;
		if (!this.directoryEntries.TryGetValue(zAssetID, out assetDirectoryEntry))
		{
			return null;
		}
		return AssetDirectory.GetBundlePath(zAssetID, assetDirectoryEntry.quality, assetDirectoryEntry.version, assetDirectoryEntry.isCached);
	}

	public AssetDirectoryEntry GetAssetDirectoryEntry(string zAssetID)
	{
		AssetDirectoryEntry result = null;
		if (this.directoryEntries.TryGetValue(zAssetID, out result))
		{
			return result;
		}
		return null;
	}

	public void CleanCache()
	{
		CachedBundlePool.DeleteAllBundlesFromMemory();
	}

	public void BuildDirectory(AssetDatabaseData zAssetDatabaseData, HashSet<string> existingFiles = null)
	{
		this.directoryEntries.Clear();
		this.BuildDirectoryByAssetDatabase(zAssetDatabaseData, existingFiles);
	}

	private void BuildDirectoryByAssetList(List<AssetDatabaseAsset> zAssets, HashSet<string> existingFiles = null)
	{
		List<AssetQuality> list = new List<AssetQuality>();
		IEnumerator enumerator = AssetDirectory.qualities.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				AssetQuality assetQuality = (AssetQuality)((int)enumerator.Current);
				if (assetQuality <= BaseDevice.ActiveDevice.DeviceQuality)
				{
					list.Add(assetQuality);
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		list.Remove(BaseDevice.ActiveDevice.DeviceQuality);
		list.Insert(0, BaseDevice.ActiveDevice.DeviceQuality);
        //foreach (AssetQuality current in list)
        //{
            AssetQuality current = AssetQuality.High;
			string defaultFilePath = FileUtils.GetDefaultFilePath(AssetDirectory.GetAssetBundleSubDir(current));
			foreach (AssetDatabaseAsset current2 in zAssets)
			{
				int version = current2.version;
				string code = current2.code;
				string bundleFilename = AssetDirectory.GetBundleFilename(code, current, version);
				this.AddToDirectory(code, current, bundleFilename, version, defaultFilePath, existingFiles);
			}
        //}
	}

	private void BuildDirectoryByAssetDatabase(AssetDatabaseData zAssetDatabase, HashSet<string> existingFiles = null)
	{
		foreach (GTAssetTypes current in AssetDirectory.assetTypes)
		{
			this.BuildDirectoryByAssetList(zAssetDatabase.GetAssetsOfType(current), existingFiles);
		}
	}

	public bool AllResourcesLocal()
	{
		return this.DirectoryEntries.All((KeyValuePair<string, AssetDirectoryEntry> entry) => entry.Value.isLocal);
	}

	public void LogOutput()
	{
		foreach (KeyValuePair<string, AssetDirectoryEntry> current in this.directoryEntries)
		{
		}
	}

	public List<string> GetNonLocalResources()
	{
		return (from entry in this.DirectoryEntries
		where !entry.Value.isLocal
		select entry.Key).ToList<string>();
	}

	private void AddToDirectory(string assetId, AssetQuality quality, string filename, int zVersion, string assetBundleSubDir, HashSet<string> existingFiles = null)
	{
		string text = assetBundleSubDir + filename;
		bool flag = (existingFiles == null) ? File.Exists(text) : existingFiles.Contains(text);
        //Debug.Log(text);
#if !UNITY_EDITOR && UNITY_ANDROID
		if (!flag)
		{
			string assetBundleSubDir2WithoutLastSlash = AssetDirectory.GetAssetBundleSubDirRemoveLastSlash(quality);
			string assetBundleSubDir2 = AssetDirectory.GetAssetBundleSubDir(quality);
			string bundleFilename = AssetDirectory.GetBundleFilename(assetId, quality, zVersion);
			flag = AndroidSpecific.APKFileExists("AppDataRoot/" + assetBundleSubDir2WithoutLastSlash, bundleFilename) ||
			       (AndroidSpecific.OBBFileExists("/assets/AppDataRoot/" + assetBundleSubDir2 + bundleFilename, false));
			//  || AndroidSpecific.OBBFileExists("/assets/AppDataRoot/" + assetBundleSubDir2 + bundleFilename, true));
		}
#endif
        bool inCached = !flag && Caching.IsVersionCached(filename, zVersion);
		AssetDirectoryEntry assetDirectoryEntry = new AssetDirectoryEntry(zVersion, quality, flag, inCached);
		if (this.directoryEntries.ContainsKey(assetId))
		{
			AssetDirectoryEntry assetDirectoryEntry2 = this.directoryEntries[assetId];
			if (assetDirectoryEntry2.quality >= quality)
			{
				if (assetDirectoryEntry2.isLocal)
				{
					return;
				}
				if (!assetDirectoryEntry.isLocal)
				{
					return;
				}
			}
			this.directoryEntries.Remove(assetId);
		}
		this.directoryEntries[assetId] = assetDirectoryEntry;
	}

	public static string GetBundleFilename(string zAssetID, AssetQuality quality, int version)
	{
		return string.Concat(new string[]
		{
			zAssetID,
			".",
			quality.ToString().ToLower(),
			".",
			version.ToString()
		});
	}

	private static string GetAssetBundleSubDir(AssetQuality zQuality)
    {
        return "AssetBundles/" + zQuality.ToString().ToLower()+"/";
	}

	//This is for checking existance of android asste file so we remove last slash
	private static string GetAssetBundleSubDirRemoveLastSlash(AssetQuality zQuality)
	{
		return "AssetBundles/" + zQuality.ToString().ToLower();
	}
	
    public static string GetBundlePath(string zAssetID, AssetQuality quality, int version, bool isLoadingFromCache)
	{
		string assetBundleSubDir = AssetDirectory.GetAssetBundleSubDir(quality);
		string bundleFilename = AssetDirectory.GetBundleFilename(zAssetID, quality, version);
		string text = FileUtils.GetDefaultFilePath(assetBundleSubDir) + bundleFilename;
		if (isLoadingFromCache)
		{
			return bundleFilename;
		}
#if UNITY_ANDROID
		if (!File.Exists(text))
		{
			text = AndroidSpecific.GetObbFilePathInPatchOrMain(assetBundleSubDir + bundleFilename);
        }
#endif
		return text;
	}
}
