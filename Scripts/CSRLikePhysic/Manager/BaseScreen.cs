using KingKodeStudio;
using UnityEngine;

public abstract class BaseScreen : MonoBehaviour, IBundleOwner
{
	protected bool firstUpdate = true;

	public GameObject Center;

	public BackgroundManager.BackgroundType ScreenBackground;

	public float OverlayBackgroundHeight;

	protected string _name = "Screen Name";

	public string Name
	{
		get
		{
			return this._name;
		}
	}

	public abstract ScreenID ID
	{
		get;
	}

	protected virtual void OnDestroy()
	{
	}

	public virtual void OnActivate(bool zAlreadyOnStack)
	{
		this._name = this.ID.ToString().ToUpper();
	}

	public virtual void OnDeactivate()
	{
		Destroy(this.Center);
		this.Center = null;
	}

	public virtual void RequestBackup()
	{
        ScreenManager.Instance.PopScreen();
	}

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
		this.Update();
	}

	protected virtual void Update()
	{
		if (this.firstUpdate)
		{
			this.firstUpdate = false;
		}
		if (!PopUpManager.Instance.isShowingPopUp)
		{
			if (InputManager.BackButtonPressed())
			{
                if (!this.IgnoreHardwareBackButton() && ScreenManager.Instance.CurrentScreen != ScreenID.Splash)
                {
                    this.OnHardwareBackButton();
                }
			}
			else if (InputManager.EnterPressed())
			{
				this.OnEnterPressed();
			}
		}
	}

	protected void QuitAppPopup()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_BUTTON_QUIT",
			BodyText = "TEXT_POPUPS_ARE_YOU_SURE",
#if UNITY_ANDROID
			ConfirmAction = new PopUpButtonAction(AndroidSpecific.Quit),
#endif
			ConfirmText = "TEXT_BUTTON_YES",
			CancelText = "TEXT_BUTTON_NO",
			ID = PopUpID.QuitApp
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	protected virtual void OnEnterPressed()
	{
	}

	protected virtual void OnHardwareBackButton()
	{
        if (CommonUI.Instance.IsShowingBackButton || ScreenManager.Instance.CurrentScreen == ScreenID.Home)
        {
            MenuAudio.Instance.playSound(AudioSfx.MenuClickBack);
            if (ScreenManager.Instance.CurrentScreen == ScreenID.Home)
            {
                this.QuitAppPopup();
            }
            else if (CommonUI.Instance.IsShowingBackButton)
            {
                this.RequestBackup();
            }
        }
	}

	protected virtual bool IgnoreHardwareBackButton()
	{
		return false;
	}

	public virtual bool ForceVisualBackButton()
	{
		return false;
	}

	public virtual bool HasBackButton()
	{
		return true;
	}
}
