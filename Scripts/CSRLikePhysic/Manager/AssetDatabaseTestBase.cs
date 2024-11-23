using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AssetDatabaseTestBase
{
	public enum DatabaseStatus
	{
		Working,
		Pass,
		Fail
	}

	public abstract AssetDatabaseTestBase.DatabaseStatus AsynchronousStatus(bool fromStartup = false);

	public bool SynchronousStatus(out AssetDirectory zDirectory, HashSet<string> existingFiles = null, bool fromStartup = false)
	{
		zDirectory = new AssetDirectory();
		string empty = string.Empty;
		string zLocalPath = string.Empty;
		if (fromStartup)
		{
			zLocalPath = this.GetAssetDBFilename();
		}
		else
		{
			zLocalPath = this.GetDownloadingFilename();
		}
		if (!FileUtils.ReadLocalStorage(zLocalPath, ref empty, false, false))
		{
			return false;
		}
		AssetDatabaseData assetDatabaseData = new AssetDatabaseData();
		if (!assetDatabaseData.ValidateAndLoadAssetDatabase(empty))
		{
			return false;
		}
		string appVersion = assetDatabaseData.GetAppVersion();
		if (!ApplicationVersion.IsEqualToCurrent(appVersion))
		{
			return false;
		}
		zDirectory.BuildDirectory(assetDatabaseData, existingFiles);
		zDirectory.LogOutput();
		return true;
	}

	private string GetAssetDBFilename()
	{
		if (UserManager.Instance.currentAccount == null)
		{
			Debug.Log("UserManager.Instance.currentAccount is null");
			return "asset_database_Default";
		}
		return "asset_database_" + UserManager.Instance.currentAccount.AssetDatabaseBranch;
	}

	private string GetDownloadingFilename()
	{
		if (UserManager.Instance.currentAccount == null)
		{
			Debug.Log("UserManager.Instance.currentAccount is null");
			return "asset_database_Default_downloading";
		}
		return "asset_database_" + UserManager.Instance.currentAccount.AssetDatabaseBranch + "_downloading";
	}
}
