using System;
using KingKodeStudio;

public class MultiplayerUnlockScreen : ZHUDScreen
{
    private const string screenBundleName = "ui_multiplayer_splash_screen";
	public DataDrivenObject DataDrivenNode;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.MultiplayerUnlock;
		}
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		base.OnActivate(zAlreadyOnStack);
		if (zAlreadyOnStack)
		{
		}
        this.DataDrivenNode.CreateDataDrivenObjectFromAssetID(screenBundleName, null);
	}

	public override bool HasBackButton()
	{
		return false;
	}

	public void OnContinue()
	{
		PlayerProfileManager.Instance.ActiveProfile.HasSeenMultiplayerIntroScreen = true;
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		ScreenManager.Instance.PopScreen();
	}
}
