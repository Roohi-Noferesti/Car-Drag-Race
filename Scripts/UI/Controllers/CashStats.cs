using KingKodeStudio;
using TMPro;
using UnityEngine;

public class CashStats : MonoBehaviour, IPersistentUI
{
	public TextMeshProUGUI uiTxtCash;

    public TextMeshProUGUI uiTxtGold;

	public Animation CashAnim;

    public TextMeshProUGUI CashAnimText;

	public Animation GoldAnim;

    public TextMeshProUGUI GoldAnimText;

	//private bool glowGold;

	//private bool glowCash;

	private int _trackedCash = -1;

	private int _trackedGold = -1;

	private bool canUpdateCash = true;

	private bool canUpdateGold = true;

	private int LastKnownCash = -1;

	private int LastKnownGold = -1;

	private int CurrentCash = -1;

	private int CurrentGold = -1;

	private int CurrentDisplayCash = -1;

	private int CurrentDisplayGold = -1;

	private bool hasBeenPlaying;

	private bool runCash;

	private float CashTime_Current;

	private float CashTime_Target;

	private int PreserveOldCashVal;

	private bool runGold;

	private float GoldTime_Current;

	private float GoldTime_Target;

	private int PreserveOldGoldVal;

	public void CashLockedState(bool zLocked)
	{
		this.canUpdateCash = !zLocked;
	}

	public void GoldLockedState(bool zLocked)
	{
		this.canUpdateGold = !zLocked;
	}

	public Vector3 getUITextCashCentre()
	{
		return this.uiTxtCash.transform.position;
	}

	public Vector3 getUITextGoldCentre()
	{
		return this.uiTxtGold.transform.position;
	}

    public void OnScreenChanged(ScreenID screen)
    {
    }

    public void Show(bool zShow)
	{
		base.gameObject.SetActive(zShow);
        //this.CashAnim.gameObject.SetActive(false);
        //this.GoldAnim.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		this.uiTxtCash = null;
		this.uiTxtGold = null;
	}

	public bool IsVisible()
	{
		return base.gameObject.activeInHierarchy;
	}

	private void Reset(int zCash, int zGold)
	{
		this.CurrentCash = zCash;
		this.CurrentGold = zGold;
		if (this.LastKnownCash < 0)
		{
			this.CurrentDisplayCash = this.CurrentCash;
			this.CurrentDisplayGold = this.CurrentGold;
			this.LastKnownCash = this.CurrentCash;
			this.LastKnownGold = this.CurrentGold;
            this.uiTxtCash.text = CurrencyUtils.GetCashString(this.CurrentCash);
			this.uiTxtGold.text = CurrencyUtils.GetGoldStringWithIcon(this.CurrentGold);
		}
	}

	private void Awake()
	{
        //NavBarAnimationManager.Instance.Subscribe(base.gameObject);
	}

	private void Start()
	{
        //this.GoldAnim.gameObject.SetActive(false);
        //this.CashAnim.gameObject.SetActive(false);
		this.Update();
	}

	private void Update()
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return;
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return;
		}
		int num = this._trackedCash;
		if (this.canUpdateCash)
		{
			num = activeProfile.GetCurrentCash();
		}
		int num2 = this._trackedGold;
		if (this.canUpdateGold)
		{
			num2 = activeProfile.GetCurrentGold();
		}
		if (num != this._trackedCash || num2 != this._trackedGold)
		{
			this._trackedCash = num;
			this._trackedGold = num2;
			this.Reset(num, num2);
		}
		this.HandleExplosions();
		this.HandleCashGoldAnim();
		this.HandleCashGoldGlow();
	}

	private void HandleExplosions()
	{
        //if (this.CashAnim.isPlaying || this.GoldAnim.isPlaying)
        //{
        //    this.hasBeenPlaying = true;
        //    //this.CashAnimText.renderer.material.SetColor("_Tint", this.CashAnimText.color);
        //    //this.GoldAnimText.renderer.material.SetColor("_Tint", this.GoldAnimText.color);
        //}
        //else if (this.hasBeenPlaying)
        //{
        //    this.hasBeenPlaying = false;
        //    this.CashAnim.gameObject.SetActive(false);
        //    this.GoldAnim.gameObject.SetActive(false);
        //}
	}

	private void HandleCashGoldAnim()
	{
		if (this.runCash)
		{
			this.CashTime_Current += Time.deltaTime;
			if (this.CashTime_Current >= this.CashTime_Target)
			{
				this.CurrentDisplayCash = this.CurrentCash;
				this.runCash = false;
				//this.glowCash = false;
			}
			else
			{
				float num = this.CashTime_Current / this.CashTime_Target;
				this.CurrentDisplayCash = (int)((float)this.CurrentCash * num + (float)this.PreserveOldCashVal * (1f - num));
			}
            this.uiTxtCash.text = CurrencyUtils.GetCashString(this.CurrentDisplayCash);
		}
		if (this.runGold)
		{
			this.GoldTime_Current += Time.deltaTime;
			if (this.GoldTime_Current >= this.GoldTime_Target)
			{
				this.CurrentDisplayGold = this.CurrentGold;
				this.runGold = false;
				//this.glowGold = false;
			}
			else
			{
				float num2 = this.GoldTime_Current / this.GoldTime_Target;
				this.CurrentDisplayGold = (int)((float)this.CurrentGold * num2 + (float)this.PreserveOldGoldVal * (1f - num2));
			}
			this.uiTxtGold.text = CurrencyUtils.GetGoldStringWithIcon(this.CurrentDisplayGold);
		}
		if (this.CurrentCash != this.LastKnownCash)
		{
			this.runCash = true;
			this.CashTime_Current = 0f;
			this.CashTime_Target = (Mathf.Log10((float)Mathf.Abs(this.CurrentDisplayCash - this.CurrentCash)) + 1f) * 0.3f;
			this.PreserveOldCashVal = this.CurrentDisplayCash;
			if (this.CurrentCash < this.LastKnownCash)
			{
                //this.CashAnim.gameObject.SetActive(true);
                //this.CashAnimText.text = CurrencyUtils.GetCashString(this.LastKnownCash - this.CurrentCash);
                ////this.CashAnimText.SetColor(Color.white);
                //this.CashAnim.Rewind();
                //this.CashAnim.Play("CashStats_CashSpend", PlayMode.StopAll);
			}
			else
			{
				//this.glowCash = true;
                //Vector3 localPosition = this.CashGlow.transform.localPosition;
                //localPosition.x = this.uiTxtCash.transform.localPosition.x - this.uiTxtCash.TotalWidth / 2f;
                //this.CashGlow.transform.localPosition = localPosition;
			}
			this.LastKnownCash = this.CurrentCash;
		}
		if (this.CurrentGold != this.LastKnownGold)
		{
			this.runGold = true;
			this.GoldTime_Current = 0f;
			this.GoldTime_Target = (Mathf.Log10((float)Mathf.Abs(this.CurrentDisplayGold - this.CurrentGold)) + 1f) * 0.3f;
			this.PreserveOldGoldVal = this.CurrentDisplayGold;
			if (this.CurrentGold < this.LastKnownGold)
			{
                //this.GoldAnim.gameObject.SetActive(true);
                //this.GoldAnimText.text = CurrencyUtils.GetGoldString(this.LastKnownGold - this.CurrentGold);
                ////this.GoldAnimText.SetColor(Color.white);
                //this.GoldAnim.Rewind();
                //this.GoldAnim.Play("CashStats_GoldSpend", PlayMode.StopAll);
			}
			else
			{
				//this.glowGold = true;
                //Vector3 localPosition2 = this.GoldGlow.transform.localPosition;
                //localPosition2.x = this.uiTxtGold.transform.localPosition.x - this.uiTxtGold.TotalWidth / 2f;
                //this.GoldGlow.transform.localPosition = localPosition2;
			}
			this.LastKnownGold = this.CurrentGold;
		}
	}

    public void OnbankButtonPressed()
    {
        ScreenManager.Instance.PushScreen(ScreenID.Shop);
    }

    public void OnTap()
    {
        if (!ScreenManager.Instance.ActiveScreen.HasBackButton())
        {
            return;
        }
        MenuAudio.Instance.playSound(AudioSfx.MenuClickBack);
        ScreenManager.Instance.ActiveScreen.RequestBackup();
    }

    public void OnShop()
    {
        //if (!this.ShopButtonIsEnabled)
        //{
        //    return;
        //}
        ScreenID iD = ScreenManager.Instance.CurrentScreen;
        if (iD == ScreenID.Shop || iD == ScreenID.ShopOverview || iD == ScreenID.Dummy)
        {
            return;
        }
        if (!AppStore.ShouldPoll())
        {
            SceneManagerFrontend.SetScreenToPushWhenInFrontend(ScreenID.ShopOverview);
            RaceController.Instance.BackToFrontend();
            return;
        }
        ScreenManager.Instance.PushSingletonScreen(ScreenID.ShopOverview);
        //if (this.PopupCallback != null)
        //{
        //    this.PopupCallback();
        //}
        if (PopUpManager.Instance.isShowingPopUp)
        {
            PopUpManager.Instance.KillPopUp();
        }
    }

    private void HandleCashGoldGlow()
	{
        //this.HandleGlow(this.CashGlow, this.glowCash);
        //this.HandleGlow(this.GoldGlow, this.glowGold);
	}

    //private void HandleGlow(global::Sprite glower, bool doGlow)
    //{
    //    float deltaTime = Time.deltaTime;
    //    Color color = glower.Color;
    //    if (doGlow)
    //    {
    //        color.a = Mathf.Min(color.a + deltaTime, 1f);
    //    }
    //    else
    //    {
    //        color.a = Mathf.Max(color.a - deltaTime, 0f);
    //    }
    //    if (color.a > 0f)
    //    {
    //        if (!glower.gameObject.activeSelf)
    //        {
    //            glower.gameObject.SetActive(true);
    //        }
    //        if (glower.color.a != color.a)
    //        {
    //            glower.SetColor(color);
    //        }
    //    }
    //    else if (glower.gameObject.activeSelf)
    //    {
    //        glower.gameObject.SetActive(false);
    //    }
    //}
}
