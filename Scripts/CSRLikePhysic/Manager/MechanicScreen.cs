using Metrics;
using System;
using System.Collections.Generic;
using GameAnalyticsSDK;
using GameAnalyticsSDK.Events;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;

public class MechanicScreen : ZHUDScreen
{
	public RuntimeTextButton Button1;

	public RuntimeTextButton Button2;

	public RuntimeTextButton Button3;

	public TextMeshProUGUI ButtonTitleTxt1;

	public TextMeshProUGUI ButtonTitleTxt2;

	public TextMeshProUGUI ButtonTitleTxt3;

	public TextMeshProUGUI bodyTitle;

	public TextMeshProUGUI bodyText;

	//public DataDrivenPortrait Mechanic;

	private int chosenNumberOfRaces;

	private int chosenCost;

	private List<MechanicData> mechanicCostData;

	//public TextAsset _goldSpriteFont;

	//public TextAsset _cashSpriteFont;

	//public Material _goldMaterial;

	//public Material _cashMaterial;

    public GameObject ButtonPanel;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.Mechanic;
		}
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		base.OnActivate(zAlreadyOnStack);
		//TextAsset newFont = null;
		//Material fontMaterial = null;
		//if (!DynamicFontManager.Instance.GetFontTextAssetForCurrentLanguage(this._goldSpriteFont.name, ref this._goldMaterial, out newFont, out fontMaterial))
		//{
		//}
        //Button1.spriteText.SetFont(newFont, fontMaterial);
        //Button1.spriteText.transform.localPosition += new Vector3(0f, 0.03f, 0f);
        //Button2.spriteText.SetFont(newFont, fontMaterial);
        //Button2.spriteText.transform.localPosition += new Vector3(0f, 0.03f, 0f);
        //Button3.spriteText.SetFont(newFont, fontMaterial);
        //Button3.spriteText.transform.localPosition += new Vector3(0f, 0.03f, 0f);
		this.mechanicCostData = GameDatabase.Instance.Currencies.getCurrentMechanicScreenData(PlayerProfileManager.Instance.ActiveProfile.GetHighestCarTierOwned());
		if (this.mechanicCostData.Count < 3)
		{
		}
		this.SetButtonsAndText();
		//this.Mechanic.Init(PopUpManager.Instance.graphics_mechanicPrefab, LocalizationManager.GetTranslation("TEXT_NAME_MECHANIC"),null);
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (!activeProfile.HasVisitedMechanicScreen)
		{
			activeProfile.HasVisitedMechanicScreen = true;
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}
	}

	public void SetButtonsAndText()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile != null)
		{
			if (activeProfile.MechanicTuningRacesRemaining != 0)
			{
				int mechanicTuningRacesRemaining = activeProfile.MechanicTuningRacesRemaining;
				this.Button1.CurrentState = BaseRuntimeControl.State.Hidden;
				this.Button2.CurrentState = BaseRuntimeControl.State.Hidden;
				this.Button3.CurrentState = BaseRuntimeControl.State.Hidden;
				this.ButtonTitleTxt1.text = string.Empty;
				this.ButtonTitleTxt2.text = string.Empty;
				this.ButtonTitleTxt3.text = string.Empty;
				this.bodyTitle.text = LocalizationManager.GetTranslation("TEXT_MECHANIC_OWNED_TITLE").ToUpper();
				this.bodyText.text = string.Format(LocalizationManager.GetTranslation("TEXT_MECHANIC_DONT_NEED_TUNING"), mechanicTuningRacesRemaining);
                ButtonPanel.SetActive(false);

            }
			else
			{
                ButtonPanel.SetActive(true);
                this.Button1.CurrentState = BaseRuntimeControl.State.Active;
				this.Button2.CurrentState = BaseRuntimeControl.State.Active;
				this.Button3.CurrentState = BaseRuntimeControl.State.Active;
				this.bodyTitle.text = LocalizationManager.GetTranslation("TEXT_MECHANIC_TITLE").ToUpper();
				this.bodyText.text = LocalizationManager.GetTranslation("TEXT_MECHANIC_INFO");
				this.SetButtonText(this.Button1, this.ButtonTitleTxt1, this.mechanicCostData[0].AmountOfRaces, this.mechanicCostData[0].Cost);
				this.SetButtonText(this.Button2, this.ButtonTitleTxt2, this.mechanicCostData[1].AmountOfRaces, this.mechanicCostData[1].Cost);
				this.SetButtonText(this.Button3, this.ButtonTitleTxt3, this.mechanicCostData[2].AmountOfRaces, this.mechanicCostData[2].Cost);
			}
		}
	}

	private void SetButtonText(RuntimeTextButton btn, TextMeshProUGUI title, int numRaces, int cost)
	{
		string text;
		if (numRaces == 1)
		{
			text = LocalizationManager.GetTranslation("TEXT_MECHANIC_ONE_RACE").ToUpper();
		}
		else
		{
			string format = LocalizationManager.GetTranslation("TEXT_MECHANIC_X_RACES").ToUpper();
			text = string.Format(format, numRaces);
		}

        string goldCost = CurrencyUtils.GetGoldStringWithIcon(cost);
		//string text2 = string.Format("{0} {1}", '{', cost);
		btn.SetText(goldCost, true, true);
		title.text = text;
	}

	public void OnButton1()
	{
		this.AttemptToPurchaseMechanic(this.mechanicCostData[0].AmountOfRaces, this.mechanicCostData[0].Cost);
	}

	public void OnButton2()
	{
		this.AttemptToPurchaseMechanic(this.mechanicCostData[1].AmountOfRaces, this.mechanicCostData[1].Cost);
	}

	public void OnButton3()
	{
		this.AttemptToPurchaseMechanic(this.mechanicCostData[2].AmountOfRaces, this.mechanicCostData[2].Cost);
	}

	private void AttemptToPurchaseMechanic(int zNumberOfRaces, int zCost)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return;
		}
		if (activeProfile.GetCurrentGold() < zCost)
		{
			MiniStoreController.Instance.ShowMiniStoreNotEnoughGold(new ItemTypeId("mch", zNumberOfRaces.ToString()), new ItemCost
			{
				GoldCost = zCost
			}, "TEXT_POPUPS_INSUFFICIENT_GOLD_BODY_MECHANIC", null, null, null);
			return;
		}
		PopUpManager.Instance.TryShowPopUp(this.getPopUp_ConfirmPurchase(zNumberOfRaces, zCost), PopUpManager.ePriority.Default, null);
	}

	private PopUp getPopUp_ConfirmPurchase(int zNumberOfRaces, int zCost)
	{
		this.chosenNumberOfRaces = zNumberOfRaces;
		this.chosenCost = zCost;
		string text = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_GET_MECHANIC_CONFIRMATION"), CurrencyUtils.GetColouredCostStringBrief(0, this.chosenCost,0), this.chosenNumberOfRaces);
		return new PopUp
		{
			Title = "TEXT_POPUPS_ARE_YOU_SURE",
			BodyText = text,
			BodyAlreadyTranslated = true,
			IsBig = false,
			ConfirmAction = new PopUpButtonAction(this.ConfirmBuyMechanicsServices),
			CancelText = "TEXT_BUTTON_CANCEL",
			ConfirmText = "TEXT_BUTTON_BUY"
		};
	}

	public void ConfirmBuyMechanicsServices()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile != null)
		{
			Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
			{
				{
					Parameters.DGld,
					(-this.chosenCost).ToString()
				},
				{
					Parameters.ItmClss,
					"mch"
				},
				{
					Parameters.Itm,
					this.chosenNumberOfRaces.ToString()
				}
			};
			Log.AnEvent(Events.PurchaseItem, data);
			AchievementChecks.ReportUseMechanicAchievement();
			
			activeProfile.SpendGold(this.chosenCost,"mechanic","service");
			
			activeProfile.MechanicTuningRacesRemaining = this.chosenNumberOfRaces;
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
			if (ScreenManager.Instance.IsScreenOnStack(ScreenID.VSDummy))
			{
				LocalPlayerInfo localPlayerInfo = CompetitorManager.Instance.LocalCompetitor.PlayerInfo as LocalPlayerInfo;
				localPlayerInfo.PopulatePhysicsCarSetup(true);
			}
			MenuAudio.Instance.playSound(AudioSfx.Purchase);
		}
        ScreenManager.Instance.PopScreen();
		this.SetButtonsAndText();
	}
}
