using System;
using I2.Loc;
using UnityEngine.UI;

public class PopUpScreen : ZHUDScreen
{
	public Text Title;

    public Text Body;

    //public DataDrivenPortrait Character;

    public Text Caption;

	public Button BlackButton;

    public Button BlueButton;

	public static Action BlackButtonAction;

	public static Action BlueButtonAction;

	public static Action BackButtonAction;

	private static string TitleText = string.Empty;

	private static string BodyText = string.Empty;

	private static string GraphicID = string.Empty;

	//private static string CaptionText = string.Empty;

	//private static string BlueButtText = string.Empty;

	//private static string BlackButtText = string.Empty;

	private static bool BackButtonEnabled = true;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.PopUp;
		}
	}

	public override bool HasBackButton()
	{
		return BackButtonEnabled;
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		this.SetScreen();
	}

	public void OnBlueButton()
	{
		BlueButtonAction();
	}

	public void OnBlackButton()
	{
		BlackButtonAction();
	}

    //protected override void OnHardwareBackButton()
    //{
    //    if (BackButtonAction != null)
    //    {
    //        //MenuAudio.Instance.playSound(MenuBleeps.MenuClickForward);
    //        BackButtonAction();
    //    }
    //    else
    //    {
    //        //MenuAudio.Instance.playSound(MenuBleeps.MenuClickBack);
    //        this.RequestBackup();
    //    }
    //}

	public static void InitScreen(string title, string body, string graphic, string caption, Action blueButtAction, string blueButtText, Action blackButtAction = null, string blackButtText = "", bool titleTranslated = false, bool bodyTranslated = false, bool hasBackButton = true)
	{
		TitleText = ((!titleTranslated) ? LocalizationManager.GetTranslation(title).ToUpper() : title);
		BodyText = ((!bodyTranslated) ? LocalizationManager.GetTranslation(body) : body);
		GraphicID = graphic;
		//CaptionText = LocalizationManager.GetTranslation(caption).ToUpper();
		BlueButtonAction = blueButtAction;
		//BlueButtText = blueButtText;
		BlackButtonAction = blackButtAction;
		//BlackButtText = blackButtText;
		BackButtonEnabled = hasBackButton;
		BackButtonAction = null;
	}

	public static void InitScreenAndroidBackButton(string title, string body, string graphic, string caption, Action blueButtAction, string blueButtText, Action blackButtAction = null, string blackButtText = "", Action backButtAction = null, bool titleTranslated = false, bool bodyTranslated = false, bool hasBackButton = true)
	{
		InitScreen(title, body, graphic, caption, blueButtAction, blueButtText, blackButtAction, blackButtText, titleTranslated, bodyTranslated, hasBackButton);
		BackButtonAction = backButtAction;
	}

	private void SetScreen()
	{
		this.Title.text = TitleText;
        //LocalisationManager.AdjustText(this.Title, 2.1f);
        this.Body.text = BodyText;
		if (GraphicID != string.Empty)
		{
            //this.Character.Init(PopUpScreen.GraphicID, PopUpScreen.CaptionText, null);
            //LocalisationManager.AdjustText(this.Caption, 1.3f);
		}
		if (BlackButtonAction != null)
		{
            //this.BlackButton.SetText(PopUpScreen.BlackButtText, false, true);
            //this.BlackButton.Runtime.CurrentState = BaseRuntimeControl.State.Active;
		}
		else
		{
            //this.BlackButton.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
		}
		if (BlueButtonAction != null)
		{
            //this.BlueButton.SetText(PopUpScreen.BlueButtText, false, true);
            //this.BlueButton.Runtime.CurrentState = BaseRuntimeControl.State.Active;
		}
		else
		{
            //this.BlueButton.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
		}
	}
}
