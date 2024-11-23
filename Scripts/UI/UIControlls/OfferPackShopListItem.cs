using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;

public class OfferPackShopListItem : ShopListItem
{
	public GameObject BonusCashLabel;

	public TextMeshProUGUI CashText;

	public GameObject BonusGoldLabel;

    public TextMeshProUGUI GoldText;

	private OfferPackData OfferData;

	private List<string> carsInOffer = new List<string>();

	public GoRaceButton RaceButton;

	public void Create(ProductData product, GameObject uiPrefab, Sprite sprite)
	{
		this._product = product;
	    this.OfferData = GameDatabase.Instance.OfferPacks.GetOfferPackDataForProduct(product.AppStoreProduct.Identifier.ToLower());
		this.SetUpInfoText(this.OfferData, product);
		base.SetupProductSprite(ShopScreen.ItemType.OfferPack, product.AnimFrameIndex, uiPrefab,sprite);
		this.TitleText.text = product.AppStoreProduct.Title;
		this.SubTitleText.text = product.AppStoreProduct.LocalisedPrice;
		base.CurrentState = BaseRuntimeControl.State.Active;
		this.ActivateButton(this.Button, base.GetButtonText(product, ShopScreen.ItemType.OfferPack));
		this.carsInOffer = this.OfferData.CarsInPack;
		this.SetupStickers(product);
		//this.RaceButton.gameObject.SetActive(true);
		this.RaceButton.RaceText.text = LocalizationManager.GetTranslation("TEXT_VIEW_CARS").ToUpper();
		//this.RaceButton.GetComponent<GoRaceButton>().button.width = 0.8f;
	}

	private void ActivateButton(RuntimeButton buttonToActivate, string buttonText)
	{
        //buttonToActivate.ForceAwake();
        //buttonToActivate.SetText(buttonText, true, true);
        //buttonToActivate.Runtime.UIButton.SetSize(this.BGCenter.width + this.BGRight.width + this.BGLeft.width, 0.5f);
        //buttonToActivate.Runtime.UIButton.transform.position = buttonToActivate.gameObject.transform.position;
        //buttonToActivate.Runtime.EnableFeatureCreepFridayHack();
	}

	private void SetUpInfoText(OfferPackData offerData, ProductData product)
	{
        this.ThemeText.text = LocalizationManager.GetTranslation(offerData.InfoText);
		string text = string.Format(LocalizationManager.GetTranslation("TEXT_OFFERPACK_CARS_REMAINING"), offerData.GetCarsRemainingInPack());
        this.NumberText.text = text;
	}

	[ContextMenu("OnViewCars")]
	public void OnViewCars()
	{
		List<CarInfo> list = (from c in this.carsInOffer
		select CarDatabase.Instance.GetCar(c)).ToList<CarInfo>();
		IGameState gs = new GameStateFacade();
		list = list.FindAll((CarInfo q) => !gs.IsCarOwned(q.Key));
		ShowroomScreen.SetUpScreenForOfferPack(list, this._product);
		ScreenManager.Instance.PushScreen(ScreenID.Showroom);
	}

	private void SetupStickers(ProductData product)
    {
        string bonusText = null;
        if (this.DiscountLabel != null) {
	        this.DiscountLabel.gameObject.SetActive(false);
        }
        if (product.GtProduct.BonusCash > 0)
		{
			if (this.RedLabel != null)
			{
				this.RedLabel.gameObject.SetActive(true);
			}
            bonusText = CurrencyUtils.GetCashString(product.GtProduct.BonusCash);
		}
        if (product.GtProduct.BonusGold > 0)
		{
			if (this.RedLabel != null)
			{
				this.RedLabel.gameObject.SetActive(true);
			}
            string goldString = CurrencyUtils.GetGoldStringWithIcon(product.GtProduct.BonusGold);

            if (!string.IsNullOrEmpty(bonusText))
            {
                bonusText += Environment.NewLine;
            }

            bonusText += goldString;
        }

        this.BonusText.text = bonusText;
    }
}
