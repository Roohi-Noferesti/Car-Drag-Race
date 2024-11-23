using System;
using System.Globalization;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileDescription : MonoBehaviour
{
	public GameObject Description;

	public GameObject Confirmation;

	public TextMeshProUGUI ProfileTitle;

    public TextMeshProUGUI ProfileLastPlayed;

    public TextMeshProUGUI ProfileLevel;

    public TextMeshProUGUI ProfileCarsOwned;

    public TextMeshProUGUI ProfileGold;

    public TextMeshProUGUI ProfileCash;

    public TextMeshProUGUI PlayerName;
    public RawImage Avatar;

	public Slider ProgressBar;

    public TextMeshProUGUI ConfirmationText;

	public RuntimeTextButton RestoreButton;

    public RuntimeTextButton CancelRestoreButton;

	private PlayerProfile profile;

	public void Setup(PlayerProfile profile)
	{
        //this.ProgressBarUVs = this.ProgressBar.GetUVs();
		this.profile = profile;
        this.ProfileTitle.text = ((profile != PlayerProfileManager.Instance.RecoveredProfile) ? LocalizationManager.GetTranslation("TEXT_POPUP_ON_DEVICE") : LocalizationManager.GetTranslation("TEXT_POPUP_CLOUD"));
		this.SetupXPBar(profile);
        this.ProfileLevel.text = string.Format(LocalizationManager.GetTranslation("TEXT_UI_XP_LEVEL"),profile.GetPlayerLevel());
        ProgressBar.minValue = GameDatabase.Instance.XPEvents.XPTotalAtEndOfLevel(profile.GetPlayerLevel()-1);
        ProgressBar.maxValue = GameDatabase.Instance.XPEvents.XPTotalAtEndOfLevel(profile.GetPlayerLevel());
        ProgressBar.value = profile.GetPlayerXP();
        PlayerName.text = string.IsNullOrEmpty(profile.DisplayName)
            ? "No_Name"
            : profile.DisplayName;//LocalizationManager.FixRTL_IfNeeded(profile.DisplayName);
        var playerAvatarID = string.IsNullOrEmpty(profile.AvatarID) ? "avatar_1" : profile.AvatarID;
        ResourceManager.LoadAssetAsync<Texture2D>(playerAvatarID, ServerItemBase.AssetType.avatar, true, tex =>
        {
            Avatar.texture = tex;
        });
		if (profile != PlayerProfileManager.Instance.RecoveredProfile && profile.GetPlayerXP() == 0)
		{
            this.ProfileLastPlayed.text = this.GetLastPlayed(profile);
            this.ProfileCarsOwned.text = string.Format(LocalizationManager.GetTranslation("TEXT_RPBOOST_CARSREASON"), profile.CarsOwned.Count.ToString());//LocalizationManager.GetTranslation("TEXT_POPUP_NEW_GAME");
            //this.ProfileCarsOwned.transform.Translate(new Vector2(0f, -0.2f));
            //this.ProfileGold.enabled = false;
            //this.ProfileCash.enabled = false;
            this.ProfileGold.text = CurrencyUtils.GetGoldStringWithIcon(profile.GetCurrentGold());
            this.ProfileCash.text = CurrencyUtils.GetCashString(profile.GetCurrentCash());
		}
		else
		{
            this.ProfileLastPlayed.text = this.GetLastPlayed(profile);
            this.ProfileCarsOwned.text = string.Format(LocalizationManager.GetTranslation("TEXT_RPBOOST_CARSREASON"), profile.CarsOwned.Count.ToString());
			this.ProfileGold.text = CurrencyUtils.GetGoldStringWithIcon(profile.GetCurrentGold());
            this.ProfileCash.text = CurrencyUtils.GetCashString(profile.GetCurrentCash());
		}
        //this.RestoreButton.ForceAwake();
        this.RestoreButton.SetText(LocalizationManager.GetTranslation("TEXT_BUTTON_RESTORE"), true, true);
        //LocalisationManager.AdjustText(this.RestoreButton.RuntimeText.spriteText, 0.76f);
	}

	private string GetLastPlayed(PlayerProfile profile)
	{
		if (profile != PlayerProfileManager.Instance.RecoveredProfile)
		{
			return LocalizationManager.GetTranslation("TEXT_POPUP_CURRENTLY_PLAYING");
		}
        //string text = BasePlatform.ActivePlatform.GetCurrentLocale();
        //text = text.Replace('_', '-');
        //CultureInfo provider = CultureInfo.InvariantCulture;
        //try
        //{
        //    provider = new CultureInfo(text, true);
        //}
        //catch
        //{
        //}
		DateTime date = profile.LastSaveDate.Date;
        bool insideCountry =  BasePlatform.ActivePlatform.InsideCountry;
        string dateTime = insideCountry ? date.ToPersianDate().ToNativeNumber() : date.ToShortDateString();
        return string.Format(LocalizationManager.GetTranslation("TEXT_POPUP_LAST_PLAYED"), dateTime);
	}

	private void SetupXPBar(PlayerProfile profile)
	{
		//float num = GameDatabase.Instance.XPEvents.ProgressThroughLevelForXP(profile.GetPlayerXP(), profile.GetPlayerLevel());
        //Color color = this.ProgressBar.color;
        //float a = (num - this.ProgressBarUVs.xMin) * this.ProgressBarUVs.width;
        //color.a = a;
        //this.ProgressBar.SetColor(color);
	}

	public void ShowConfirmation(bool active)
	{
		this.Description.SetActive(!active);
		this.Confirmation.SetActive(active);
	}

	public void SetConfirmationText(string text)
	{
		this.ConfirmationText.text = text;
	}

	public void RestoreProfile()
	{
		if (this.profile == PlayerProfileManager.Instance.RecoveredProfile)
		{
			AssetSystemManager.Instance.OnProfileSaveYes();
		}
		else
		{
			AssetSystemManager.Instance.OnProfileSaveNo();
		}
	}

	public bool IsNewGame()
	{
		return this.profile != PlayerProfileManager.Instance.RecoveredProfile && this.profile.GetPlayerXP() == 0;
	}
}
