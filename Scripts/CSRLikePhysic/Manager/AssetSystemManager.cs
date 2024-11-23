using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using Metrics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class AssetSystemManager
{
	public enum Reason
	{
		StartingUpClient,
		AssetDBChanged,
		OnUserChanged,
		OnAskOverwriteProfile,
		OnClientOutOfDate,
		ChangedServerStack,
		ChangedBranch,
		OnGameCenterChanged,
		OnLocaleChanged,
		OnLoadProgression
	}

	public delegate void JustKickedDelegate(Reason reason);

	private PlayerProfile[] profilesToRestore;

	private Reason reasonCache;

	private bool playerProfileDidOverwritePopup;
	
	private static bool doneForceUpdate = false;

	private string downloadUrl = null;

    public static event JustKickedDelegate JustKicked;

	public static AssetSystemManager Instance
	{
		get;
		private set;
	}

	public int HoursBetweenUpdateNags
	{
		get
		{
			return GameDatabase.Instance.CareerConfiguration.MinimumUpgradeNagSeparationTime;
		}
	}

	public bool IsClientOutOfDate
	{
		get;
		private set;
	}

	public static void Create()
	{
		AssetSystemManager assetSystemManager = new AssetSystemManager();
		assetSystemManager.Initialise();
	}

	private void Initialise()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
        UserManager.LoggedInEvent += this.OnUserLoggedIn;
        UserManager.LoginFailedEvent += this.OnUserLogInFailed;
        UserManager.UserChangedEvent += this.OnUserChanged;
		PlayerProfileWeb.AskOverwriteProfileEvent += this.OnAskOverwriteProfile;
        AssetBackgroundDownloader.MustUpdateClientEvent += this.OnClientOutOfDate;
        AssetDatabaseVersionPoll.PollCompleteEvent += this.OnAssetDatabaseVersionPoll;
		this.IsClientOutOfDate = false;

	}

	private void OnUserLoggedIn()
	{
        AssetDatabaseVersionPoll.Instance.PollNow();
	}

	private void OnUserLogInFailed()
	{
	}

	private void OnUserChanged()
	{
        AssetDatabaseVersionPoll.Instance.PollNow();
	}

	public void BackgroundDownloaderSnapshotReady()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_NEWPATCH_TITLE",
			BodyText = "TEXT_POPUPS_NEWPATCH_BODY",
			IsBig = false,
			ConfirmAction = this.PopupKickBack,
			ConfirmText = "TEXT_BUTTON_OK"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
	}

    private void OnAssetDatabaseVersionPoll(int serverVersion)
    {
        GTDebug.Log(GTLogChannel.AssetSystemManager, "version polled..." + "   " + serverVersion + "    " + AssetDatabaseClient.Instance.Data.GetVersion());
        if (AssetDatabaseClient.Instance.Data.GetVersion() != serverVersion)
        {
            GTDebug.Log(GTLogChannel.AssetSystemManager, "New asset databse version detected.trying to download..." + "   " + serverVersion + "    " + AssetDatabaseClient.Instance.Data.GetVersion());
            AssetBackgroundDownloader.Instance.DownloadAssetDatabase(serverVersion);
        }
    }

	public void KickBackToSafePlaceAndReload(Reason reason)
	{
		if (SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Frontend)
		{
			return;
		}
		this.reasonCache = reason;
        //PopUpDatabase.Common.ShowLoadingSpinnerPopup();
        UserManager.Instance.StartCoroutine(this.WaitForSecondsThenExecute(new Action(this.ActuallyReload), 0.1f));
	}

	private IEnumerator WaitForSecondsThenExecute(Action method, float wait)
	{
	    yield return new WaitForSeconds(wait);
        method.Invoke();
	}

	private void ActuallyReload()
	{
        GTSystemOrder.ShutDownGameSystems(this.reasonCache);
        if (ScreenManager.Instance.CurrentScreen == ScreenID.Splash)
        {
            ScreenManager.Instance.PopScreen();
            ScreenManager.Instance.PushScreen(ScreenID.Splash);
        }
        else if (ScreenManager.Instance.CurrentScreen == ScreenID.VSDummy)
        {
            LoadingScreenManager.Instance.SnapCloseVSScreen();
            ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.Splash, new ScreenID[]
            {
                ScreenID.Dummy
            });
        }
        else
        {
            ScreenManager.Instance.PopToScreen(ScreenID.Splash);
        }
        //GTInitialisers.ResetUICache();
        PopUpManager.Instance.KillPopUp();
        ScreenManager.Instance.UpdateImmediately();
	}

	public void InvokeJustKickedCallback(Reason reason)
	{
		if (JustKicked != null)
		{
			JustKicked(reason);
		}
	}

	private void PopupKickBack()
	{
		this.KickBackToSafePlaceAndReload(Reason.AssetDBChanged);
	}

	private void OnAskOverwriteProfile()
	{
		if (PopUpManager.Instance.CurrentPopUpMatchesID(PopUpID.RestoreProfile))
		{
			return;
		}
		if (!this.LoadRecoveredProfile())
		{
            PlayerProfileManager.Instance.ClearRecoveredProfile();
			return;
		}
		this.GetRecoveredProfileDetails();
		if (this.profilesToRestore == null)
		{
			return;
		}
		this.playerProfileDidOverwritePopup = PopUpManager.Instance.isShowingPopUp || (GarageScreen.Instance!=null && GarageScreen.Instance.ShowingIntro);
        PopUp popup = new PopUp
        {
            Title = "TEXT_POPUPS_RESTORESAVED_TITLE",
            IsProfileRestore = true,
            profiles = this.profilesToRestore,
            ID = PopUpID.RestoreProfile
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
	}

	public void OnProfileSaveNo()
	{
        FileUtils.EraseLocalStorageFile(UserManager.Instance.currentAccount.Username, false);
        PlayerProfileManager.Instance.ClearRecoveredProfile();
		string uUID = BaseIdentity.ActivePlatform.GetUUID();
		string macAddress = BasePlatform.ActivePlatform.GetMacAddress();
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.baid,
                UserManager.Instance.currentAccount.Username
			},
			{
				Parameters.openUDID,
				uUID
			},
			{
				Parameters.mac,
				macAddress
			},
			{
				Parameters.RP,
				PlayerProfileManager.Instance.ActiveProfile.GetPlayerRP().ToString()
			},
			{
				Parameters.BCsh,
				PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash().ToString()
			},
			{
				Parameters.BGld,
				PlayerProfileManager.Instance.ActiveProfile.GetCurrentGold().ToString()
			},
			{
				Parameters.ProfileRecoveryResult,
				"TakenLocal"
			}
		};
		Log.AnEvent(Events.ProfileRecover, data);
	    bool userNotNull =  UserManager.Instance != null && UserManager.Instance.currentAccount != null;
		bool profileNotNull = PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null;
	    if (userNotNull && profileNotNull && UserManager.Instance.currentAccount.Username != PlayerProfileManager.Instance.ActiveProfile.Name)
		{
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            activeProfile.Rename(UserManager.Instance.currentAccount.Username);
			this.KickBackToSafePlaceAndReload(Reason.OnAskOverwriteProfile);
		}
		else if (this.playerProfileDidOverwritePopup || (ScreenManager.Instance.CurrentScreen==ScreenID.Splash))
		{
			this.KickBackToSafePlaceAndReload(Reason.OnAskOverwriteProfile);
		}
	}

	public void OnProfileSaveYes()
	{
		byte[] bytes = null;
        if (FileUtils.ReadLocalStorage(UserManager.Instance.currentAccount.Username, ref bytes, false))
        {
            PlayerProfileFile.WriteBinaryFile(UserManager.Instance.currentAccount.Username, bytes);
            PlayerProfileMigration.MigrateProfile(UserManager.Instance.currentAccount.Username);
            PlayerProfileManager.Instance.LoadActiveProfile();
            //LogUtility.Log(string.Format("Player '{0}' has been recovered : ", PlayerProfileManager.Instance.ActiveProfile.ID));
        }
        FileUtils.EraseLocalStorageFile(UserManager.Instance.currentAccount.Username, false);
        PlayerProfileManager.Instance.ClearRecoveredProfile();
		string uUID = BaseIdentity.ActivePlatform.GetUUID();
		string macAddress = BasePlatform.ActivePlatform.GetMacAddress();
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.baid,
                UserManager.Instance.currentAccount.Username

			},
			{
				Parameters.openUDID,
				uUID
			},
			{
				Parameters.mac,
				macAddress
			},
			{
				Parameters.RP,
				PlayerProfileManager.Instance.ActiveProfile.GetPlayerRP().ToString()
			},
			{
				Parameters.BCsh,
				PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash().ToString()
			},
			{
				Parameters.BGld,
				PlayerProfileManager.Instance.ActiveProfile.GetCurrentGold().ToString()
			},
			{
				Parameters.ProfileRecoveryResult,
				"RecoveredRemote"
			}
		};
		Log.AnEvent(Events.ProfileRecover, data);
		this.KickBackToSafePlaceAndReload(Reason.OnAskOverwriteProfile);
	}

	private bool LoadRecoveredProfile()
	{
        byte[] array = null;
        if (FileUtils.ReadLocalStorage(UserManager.Instance.currentAccount.Username, ref array, false))
        {
            if (PlayerProfileManager.Instance.LoadRecoveredProfile())
            {
                return true;
            }
            FileUtils.EraseLocalStorageFile(UserManager.Instance.currentAccount.Username, false);
        }
        return false;
	}

	private void GetRecoveredProfileDetails()
	{
        this.profilesToRestore = null;
        if (PlayerProfileManager.Instance.RecoveredProfile != null && PlayerProfileManager.Instance.RecoveredProfile.GetPlayerLevel() >= 0)
        {
            this.profilesToRestore = new PlayerProfile[2];
            this.profilesToRestore[0] = PlayerProfileManager.Instance.ActiveProfile;
            this.profilesToRestore[1] = PlayerProfileManager.Instance.RecoveredProfile;
        }
	}

	private void OnClientOutOfDate(bool forceUpdate)
	{
		return;
		this.IsClientOutOfDate = true;
        //if (PlayerProfileManager.Instance.ActiveProfile.LastUpgradeDateTimeNag.AddHours((double)this.HoursBetweenUpdateNags) < GTDateTime.Now)
		//{
            PopUp popup = new PopUp
            {
                Title = "TEXT_POPUPS_NEW_VERSION_TITLE",
                BodyText = LocalizationManager.GetTranslation("TEXT_POPUPS_NEW_VERSION_BODY"),
                BodyAlreadyTranslated = true,
                IsVeryBig = true,
                ConfirmAction = OnYesSirIWillUpdate,
                ConfirmText = "TEXT_BUTTON_UPDATE",
            };

		    if (!forceUpdate)
		    {
		        popup.CancelText = "TEXT_BUTTON_CANCEL";
            }
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
            PlayerProfileManager.Instance.ActiveProfile.LastUpgradeDateTimeNag = GTDateTime.Now;
		//}
	}
	
	private static void OnYesSirIWillUpdate()
	{
		doneForceUpdate = true;
#if UNITY_EDITOR
        Application.OpenURL(GTPlatform.GetPlatformUpdateURL());
        EditorApplication.isPlaying = false;
#else
		Application.OpenURL(GTPlatform.GetPlatformUpdateURL());
        Application.Quit();
#endif
    }
}
