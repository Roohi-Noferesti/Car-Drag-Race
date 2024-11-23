using System;
using System.Collections.Generic;
using KingKodeStudio;
using UnityEngine;

public class AssetBackgroundDownloader : MonoBehaviour
{
	private enum eState
	{
		Idle,
		DownloadingDatabase,
		WaitingToShowDownloadPopUp,
		WaitingToStartDownloadingFiles,
		DownloadingFiles
	}

	private WWW www;

	private AssetDatabaseTest assetDatabaseTest = new AssetDatabaseTest();

	private eState State;

#pragma warning disable 649
	private bool UseDownloadScreen;
#pragma warning restore 649

    public static event Action<bool> MustUpdateClientEvent;

	public static AssetBackgroundDownloader Instance
	{
		get;
		private set;
	}

	public AssetLoaderQueue queue
	{
		get;
		private set;
	}

	public bool IsBusy
	{
		get
		{
			return this.State == eState.DownloadingDatabase || this.State == eState.DownloadingFiles;
		}
	}

	private void Awake()
	{
		Instance = this;
		this.queue = base.gameObject.AddComponent<AssetLoaderQueue>();
		this.queue.SetMaxLoads(1);
		this.State = eState.Idle;
	}

	private void Update()
	{
        if (SceneLoadManager.Instance == null || SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Frontend || this.queue.isBlocking)// || MultiplayerUtils.IsPlayingMultiplayer())
		{
			return;
		}
		switch (this.State)
		{
		case eState.DownloadingDatabase:
			if (this.www.isDone)
			{
                GTDebug.Log(GTLogChannel.AssetBackgroundDownloader, "Download database completed . writing...");
			    if (!this.WriteDatabaseToDisk())
			    {
			        this.State = eState.Idle;
			        this.Unload();
			    }
			    else if (this.UseDownloadScreen)
			    {
			        this.State = eState.WaitingToShowDownloadPopUp;
			    }
			    else
			    {
                    GTDebug.Log(GTLogChannel.AssetBackgroundDownloader, "Downloading missing files...");
			        this.StartDownloading();
			    }
			}
			break;
		case eState.WaitingToShowDownloadPopUp:
			if (this.CanDisplayDatapushPopUp())
			{
				AssetDatabaseTestBase.DatabaseStatus databaseStatus = this.assetDatabaseTest.AsynchronousStatus(false);
				if (databaseStatus != AssetDatabaseTestBase.DatabaseStatus.Pass)
				{
					if (databaseStatus == AssetDatabaseTestBase.DatabaseStatus.Fail)
					{
						this.StopDownloading();
					}
				}
				else
				{
					this.State = eState.WaitingToStartDownloadingFiles;
					this.DisplayAskToDownloadPopUp();
				}
			}
			break;
		case eState.DownloadingFiles:
			if (this.queue.IsEmpty && (this.CanDisplayDatapushPopUp() || this.UseDownloadScreen))
			{
				AssetDirectory assetDirectory;
				if (!this.assetDatabaseTest.SynchronousStatus(out assetDirectory, false))
				{
					this.StopDownloading();
				}
				else if (!assetDirectory.AllResourcesLocal())
				{
					List<string> nonLocalResources = assetDirectory.GetNonLocalResources();
					foreach (string current in nonLocalResources)
					{
					}
					this.StopDownloading();
				}
				else
				{
					if (this.UseDownloadScreen)
					{
						AssetSystemManager.Instance.KickBackToSafePlaceAndReload(AssetSystemManager.Reason.AssetDBChanged);
					}
					else
					{
						this.KickBack();
					}
					this.State = eState.Idle;
				}
			}
			break;
		}
	}

	public bool StartDownloading()
	{
		if (this.State == eState.DownloadingFiles)
		{
			return true;
		}
		this.State = eState.DownloadingFiles;
		if (!this.DownloadMissingFiles(false, true))
		{
			this.State = eState.Idle;
			this.Unload();
			return false;
		}
		return true;
	}

	private void StopDownloading()
	{
		this.State = eState.Idle;
        if (ScreenManager.Instance.CurrentScreen == ScreenID.Downloading)
        {
            ScreenManager.Instance.PopScreen();
        }
	}

	private bool CanDisplayDatapushPopUp()
	{
	    return ScreenManager.Instance.CurrentScreen != ScreenID.Shop &&
                ScreenManager.Instance.CurrentScreen != ScreenID.PrizeOMatic &&
	           !PopUpManager.Instance.isShowingPopUp &&
	           (GarageScreen.Instance == null || !GarageScreen.Instance.ShowingIntro)
               && PlayerProfileManager.Instance != null
	           && PlayerProfileManager.Instance.ActiveProfile != null
	           && PlayerProfileManager.Instance.ActiveProfile.HasCompletedFirstThreeTutorialRaces();
	}

	private void DisplayAskToDownloadPopUp()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_NEWPATCH_TITLE",
			BodyText = "TEXT_POPUPS_STARTDOWNLOAD_BODY",
			IsBig = true,
			ConfirmAction = new PopUpButtonAction(this.PopUpStartDownload),
			ConfirmText = "TEXT_BUTTON_OK",
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
	}

	private void PopUpStartDownload()
	{
	    ScreenManager.Instance.PushScreen(ScreenID.Downloading);
	}

	public void DownloadAssetDatabase(int serverVersion)
	{
		if (this.State == eState.DownloadingDatabase)
		{
			return;
		}
		if (!this.queue.IsEmpty)
		{
			this.queue.Clear();
		}
		string zFileName = "asset_database." + serverVersion;
		string s3URL = Endpoint.GetS3URL(zFileName);
		this.www = new WWW(s3URL);
		this.State = eState.DownloadingDatabase;
	}

	private bool WriteDatabaseToDisk()
	{
		if (this.www.error != null)
		{
			return false;
		}
		AssetDatabaseData assetDatabaseData = new AssetDatabaseData();
		string text = this.www.text;
		if (!assetDatabaseData.ValidateAndLoadAssetDatabase(text))
		{
            GTDebug.Log(GTLogChannel.AssetBackgroundDownloader, "Databse is not valid , return false...");
            return false;
		}

        string appVersion = assetDatabaseData.GetAppVersion();
        string minimum = assetDatabaseData.GetMinimumVersion();
		if (!ApplicationVersion.IsEqualToCurrent(appVersion))
		{
			if (ApplicationVersion.IsGreaterThanCurrent(appVersion))
			{
			    bool forceUpdate = ApplicationVersion.IsGreaterThanCurrent(minimum);
                this.InvokeMustUpdateClientEvent(forceUpdate);
			}
            GTDebug.Log(GTLogChannel.AssetBackgroundDownloader, "Databse version not valid return false... appVersion : " + appVersion);
            return false;
		}
		FileUtils.WriteLocalStorage(this.GetDownloadingFilename(), text, false, false);
		this.www.Dispose();
		this.www = null;
		return true;
	}

	public bool DownloadMissingFiles(bool fromStartup = false, bool testAssetDatabase = true)
	{
		AssetDirectory assetDirectory;
		if (!this.assetDatabaseTest.SynchronousStatus(out assetDirectory, fromStartup))
		{
			return false;
		}
		List<string> nonLocalResources = assetDirectory.GetNonLocalResources();
		foreach (string current in nonLocalResources)
		{
			AssetDirectoryEntry assetDirectoryEntry = assetDirectory.GetAssetDirectoryEntry(current);
			int version = assetDirectoryEntry.version;
			if (this.queue.Find(current, version) == null)
			{
				/*CachedLoader cachedLoader = */new CachedLoader(current, this.queue, version);
			}
		}
		return true;
	}

	private void KickBack()
	{
		AssetSystemManager.Instance.BackgroundDownloaderSnapshotReady();
	}

	public void OnStartup()
	{
		AssetDirectory assetDirectory;
		if (this.assetDatabaseTest.SynchronousStatus(out assetDirectory, false) && assetDirectory.AllResourcesLocal())
		{
			AssetDatabaseClient.Instance.SwitchAssetDatabase(this.GetDownloadingFilename());
		}
	}

	public void Unload()
	{
		if (this.www != null)
		{
			this.www.Dispose();
			this.www = null;
		}
		if (!this.queue.IsEmpty)
		{
			this.queue.Clear();
		}
		this.State = eState.Idle;
	}

	private string GetAssetDBFilename()
	{
        return "asset_database_" + UserManager.Instance.currentAccount.AssetDatabaseBranch;
	}

	private string GetDownloadingFilename()
	{
        return "asset_database_" + UserManager.Instance.currentAccount.AssetDatabaseBranch + "_downloading";
	}

	public void InvokeMustUpdateClientEvent(bool forceUpdate)
	{
		if (MustUpdateClientEvent != null)
		{
			MustUpdateClientEvent(forceUpdate);
		}
	}
}
