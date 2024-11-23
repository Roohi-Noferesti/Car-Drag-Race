using KingKodeStudio;
using TMPro;
using UnityEngine;

public class BalanceRecoveryScreen : ZHUDScreen
{
	public TextMeshProUGUI AdjustmentText;

    public TextMeshProUGUI LinkText;

    public TextMeshProUGUI MainText;

	private static string defaultAdjustmentText = string.Empty;

	private static string link;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.BalanceRecovery;
		}
	}

	public static void SetAdjustmentText(string zText)
	{
		defaultAdjustmentText = zText;
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		link = GTPlatform.GetPlatformSupportURL();
		base.OnActivate(zAlreadyOnStack);
		if (PopUpManager.Instance.isShowingPopUp)
		{
			PopUpManager.Instance.KillPopUp();
		}
		LinkText.text = link;
        AdjustmentText.text = defaultAdjustmentText;
		defaultAdjustmentText = string.Empty;
	}

    public void OnSupportPress()
    {
        ScreenManager.Instance.PushScreen(ScreenID.Contact);
        //Application.OpenURL(GTPlatform.GetPlatformSupportURL());
    }
}
