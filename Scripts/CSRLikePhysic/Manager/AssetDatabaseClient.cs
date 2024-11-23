using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class AssetDatabaseClient : MonoBehaviour
{
	public enum ChangeType
	{
		Safe,
		Unsafe
	}

	public delegate void AssetDatabaseUpdatedDelegate(AssetDatabaseClient.ChangeType type);

	private const string assetDatabaseFileRoot = "asset_database_";

	private string assetDatabaseDefaultFilename = string.Empty;

	private AssetDatabaseData data = new AssetDatabaseData();

	private AssetDirectory directory = new AssetDirectory();

    public static event AssetDatabaseClient.AssetDatabaseUpdatedDelegate AssetDatabaseUpdatedEvent;

	public static AssetDatabaseClient Instance
	{
		get;
		private set;
	}

	public AssetDatabaseData Data
	{
		get
		{
			return this.data;
		}
	}

	public AssetDirectory Directory
	{
		get
		{
			return this.directory;
		}
	}

	public bool IsReadyToUse
	{
		get
		{
			return this.data.IsValid;
		}
	}

	public bool IsCorrupted
	{
		get;
		private set;
	}

	private void Awake()
	{
		this.assetDatabaseDefaultFilename = this.GetAssetDatabaseFilename(BasePlatform.ActivePlatform.GetDefaultBranchName());
		this.IsCorrupted = false;
		if (AssetDatabaseClient.Instance != null)
		{
			return;
		}
		AssetDatabaseClient.Instance = this;
	}

	public void CleanCache()
	{
		FileUtils.EraseLocalStorageFile(this.assetDatabaseDefaultFilename, false);
	}

	public string GetFilename()
	{
		//return "asset_database_facelift_cars_Android";
		if (UserManager.Instance.currentAccount == null)
		{
			GTDebug.Log(GTLogChannel.AssetDatabaseClient,"UserManager.Instance.currentAccount is null");
			return "asset_database_Default_Android";
		}
		return "asset_database_" + UserManager.Instance.currentAccount.AssetDatabaseBranch;
	}

	private bool SerialiseAssetDatabase(Stream zStream)
	{
		string databaseString = string.Empty;
		using (StreamReader streamReader = new StreamReader(zStream))
		{
			databaseString = streamReader.ReadToEnd();
		}
		if (!this.data.ValidateAndLoadAssetDatabase(databaseString))
		{
			this.IsCorrupted = true;
			return false;
		}
		this.IsCorrupted = false;
		this.directory.BuildDirectory(this.data, null);
		string appVersion = this.data.GetAppVersion();
		if (!ApplicationVersion.IsEqualToCurrent(appVersion))
		{
			return false;
		}
		if (!this.directory.AllResourcesLocal())
		{
			List<string> nonLocalResources = this.directory.GetNonLocalResources();
			foreach (string current in nonLocalResources)
			{
				//AssetDirectoryEntry assetDirectoryEntry = this.directory.GetAssetDirectoryEntry(current);
			}
			return false;
		}
		return true;
	}

	public void LoadDatabaseFromLocalStorageAction()
	{
		this.LoadDatabaseFromLocalStorage();
	}

	public bool LoadDatabaseFromLocalStorage()
	{
		var filename = GetFilename();
		GTDebug.Log(GTLogChannel.AssetDatabaseClient,"filename: "+filename);
		Stream stream = FileUtils.OpenFileFromLocalStorage(filename, true, false);
		if (stream == null)
		{
			GTDebug.LogError(GTLogChannel.AssetDatabaseClient,"AssetDatabase cannot be loaded!" );
			return this.FallBackToAppDataRootDatabase();
		}
		return !this.SerialiseAssetDatabase(stream) && this.FallBackToAppDataRootDatabase();
	}

	public void CorruptedDataPopUp()
	{
		Dictionary<GTAppStore, string> dictionary = new Dictionary<GTAppStore, string>
		{
			{
				GTAppStore.Amazon,
				"We have detected that your app is corrupted. Please delete the app and re-install. If you were playing whilst signed into Amazon then your profile will be restored."
			},
			{
				GTAppStore.GooglePlay,
				"We have detected that your app is corrupted. Please delete the app and re-install. If you were playing whilst signed into Google Play then your profile will be restored."
			},
#if UNITY_ANDROID
		    {
		        GTAppStore.Bazaar,
		        "We have detected that your app is corrupted. Please delete the app and re-install. If you were playing whilst signed into Bazaar then your profile will be restored."
		    },
		    {
		        GTAppStore.Iraqapps,
		        "We have detected that your app is corrupted. Please delete the app and re-install. If you were playing whilst signed into Iranapps then your profile will be restored."
		    },
		    {
		        GTAppStore.Myket,
		        "We have detected that your app is corrupted. Please delete the app and re-install. If you were playing whilst signed into Myket then your profile will be restored."
		    },
#endif
            {
				GTAppStore.iOS,
				"We have detected that your app is corrupted. Please delete the app and re-install. If you were playing whilst signed into Game Center then your profile will be restored."
			},
			{
				GTAppStore.OSX,
				"We have detected that your app is corrupted. Please delete the app and re-install. If you were playing whilst signed into Game Center then your profile will be restored."
			},
			{
				GTAppStore.Windows,
				"We have detected that your app is corrupted. Please delete the app and re-install. If you were playing whilst signed into Live then your profile will be restored."
			},
			{
				GTAppStore.Windows_Metro,
				"We have detected that your app is corrupted. Please delete the app and re-install. If you were playing whilst signed into Live then your profile will be restored."
			}
		};
		PopUp popup = new PopUp
		{
			Title = "Corrupted Data",
			BodyText = dictionary[BasePlatform.ActivePlatform.GetTargetAppStore()],
			IsBig = true,
			TitleAlreadyTranslated = true,
			BodyAlreadyTranslated = true,
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.SystemUrgent, null);
	}

	private bool FallBackToAppDataRootDatabase()
	{
		this.CleanCache();
		Stream stream = FileUtils.OpenFileFromLocalStorage(this.assetDatabaseDefaultFilename, false, false);
		if (stream == null)
		{
			return false;
		}
		bool flag = this.SerialiseAssetDatabase(stream);
		if (this.IsCorrupted)
		{
			this.CorruptedDataPopUp();
		}
		if (flag)
		{
		}
		return flag;
	}

	public void SwitchAssetDatabase(string zFilename)
	{
		string content = string.Empty;
		if (!FileUtils.ReadLocalStorage(zFilename, ref content, false, false))
		{
			return;
		}
		FileUtils.WriteLocalStorage(this.GetFilename(), content, false, false);
		FileUtils.EraseLocalStorageFile(zFilename, false);
	}

	public void InvokeAssetDatabaseUpdatedEvent(AssetDatabaseClient.ChangeType updateType)
	{
		if (AssetDatabaseClient.AssetDatabaseUpdatedEvent != null)
		{
			AssetDatabaseClient.AssetDatabaseUpdatedEvent(updateType);
		}
	}

	public string GetAssetDatabaseFilename(string branchName)
	{
		return "asset_database_" + branchName;
	}
}
