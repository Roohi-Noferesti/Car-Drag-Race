using System;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;

public class ServerAccountConnectActions : MonoBehaviour
{
	public enum eAction
	{
		None,
		AllowOfflineProgress,
		BlockAllProgress,
		BlockAllProgressPromptAppStore,
		BlockAllProgressTillLatestAssetDBDownloaded
	}

	private enum eActionPrompt
	{
		OkPrompt,
		AppStorePrompt,
		NoPrompt
	}

	private bool _shouldCheckWhenCan;

	private bool _displayingPopup;

	public static ServerAccountConnectActions Instance
	{
		get;
		private set;
	}

	public void OnUserLoggedIn()
	{
		this._shouldCheckWhenCan = true;
		this._displayingPopup = false;
	}

	private bool AreWeSafeToDoStuff()
	{
		return SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend && ScreenManager.Instance.CurrentScreen != ScreenID.Intro && ScreenManager.Instance.CurrentScreen != ScreenID.Splash && ScreenManager.Instance.CurrentScreen != ScreenID.BalanceRecovery;
	}

	private void Awake()
	{
		if (ServerAccountConnectActions.Instance != null)
		{
		}
		ServerAccountConnectActions.Instance = this;
		UserManager.LoggedInEvent += new UserLoggedInDelegate(this.OnUserLoggedIn);
	}

	private void Update()
	{
		if (!this._shouldCheckWhenCan)
		{
			return;
		}
		if (!this.AreWeSafeToDoStuff())
		{
			return;
		}
		if (this._displayingPopup)
		{
			return;
		}
		if (ApplicationVersion.IsGreaterThanCurrent(UserManager.Instance.currentAccount.ServerLastSavedProfileProductVersion))
		{
			this.OnDeviceOlderThanProfileVersion();
		}
		else
		{
			switch (UserManager.Instance.currentAccount.ClientAction)
			{
			case 0:
				if (!string.IsNullOrEmpty(UserManager.Instance.currentAccount.ClientActionTextBody))
				{
					this.DisplayClientActionPrompt(ServerAccountConnectActions.eActionPrompt.OkPrompt);
				}
				this._shouldCheckWhenCan = false;
				break;
			case 1:
				this.DisableOnlineConnection();
				this.DisplayClientActionPrompt(ServerAccountConnectActions.eActionPrompt.OkPrompt);
				this._shouldCheckWhenCan = false;
				break;
			case 2:
				this.DisableOnlineConnection();
				this.DisplayClientActionPrompt(ServerAccountConnectActions.eActionPrompt.NoPrompt);
				break;
			case 3:
				this.DisableOnlineConnection();
				this.DisplayClientActionPrompt(ServerAccountConnectActions.eActionPrompt.AppStorePrompt);
				break;
			case 4:
				this.DisableOnlineConnection();
				this.DisplayClientActionPrompt(ServerAccountConnectActions.eActionPrompt.NoPrompt);
				break;
			default:
				this._shouldCheckWhenCan = false;
				break;
			}
		}
	}

	private void DisplayClientActionPrompt(ServerAccountConnectActions.eActionPrompt promptType)
	{
		PopUpButtonAction confirmAction = null;
		string confirmText = string.Empty;
		switch (promptType)
		{
		case ServerAccountConnectActions.eActionPrompt.AppStorePrompt:
			confirmAction = new PopUpButtonAction(this.OnYesSirIWillUpdate);
			confirmText = "TEXT_BUTTON_GOTO_APP_STORE";
			goto IL_5B;
		case ServerAccountConnectActions.eActionPrompt.NoPrompt:
			goto IL_5B;
		}
		confirmAction = new PopUpButtonAction(this.OkActionResponse);
		confirmText = "TEXT_BUTTON_OK";
		IL_5B:
		PopUp popup = new PopUp
		{
			Title = UserManager.Instance.currentAccount.ClientActionTextTitle,
			BodyText = UserManager.Instance.currentAccount.ClientActionTextBody,
			TitleAlreadyTranslated = true,
			BodyAlreadyTranslated = true,
			IsBig = true,
			ConfirmAction = confirmAction,
			ConfirmText = confirmText,
			ID = PopUpID.DeviceOlderThanProfileVersion,
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
		this._displayingPopup = PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.SystemUrgent, null);
	}

	private void OnDeviceOlderThanProfileVersion()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_NEEDUPDATES_TITLE",
			BodyText = LocalizationManager.GetTranslation("TEXT_POPUPS_NEEDUPDATES_PROFILE_FORCE_BODY"),
            BodyAlreadyTranslated = true,
			IsBig = true,
			ConfirmAction = new PopUpButtonAction(this.OnYesSirIWillUpdate),
            ConfirmText = "TEXT_BUTTON_GOTO_APP_STORE",
			ID = PopUpID.DeviceOlderThanProfileVersion,
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
		this._displayingPopup = PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.SystemUrgent, null);
	}

	private void DisableOnlineConnection()
	{
		PolledNetworkState.ForceOffline();
		WebRequestQueue.Instance.ResetQueue();
		WebRequestQueueRTW.Instance.ResetQueue();
	}

	private void OnYesSirIWillUpdate()
	{
		Application.OpenURL(GTPlatform.GetPlatformUpdateURL());
		AssetSystemManager.Instance.KickBackToSafePlaceAndReload(AssetSystemManager.Reason.OnClientOutOfDate);
		this._displayingPopup = false;
	}

	private void OkActionResponse()
	{
		this._displayingPopup = false;
		this._shouldCheckWhenCan = false;
	}
}
