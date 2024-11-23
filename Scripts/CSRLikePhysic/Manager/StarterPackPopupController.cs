using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using Metrics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarterPackPopupController : MonoBehaviour
{
    //public RuntimeTextButton ConfirmButton;

    public RuntimeTextButton CancelButton;

	public Material GoldFontMaterial;

	public TextAsset GoldFontText;

	public Material SilverFontMaterial;

	public TextAsset SilverFontText;

	public Sprite GoldImage;

    public Sprite SilverImage;

	public TextMeshProUGUI StarterPackAmount1;

    public TextMeshProUGUI StarterPackAmount2;

	public Image StarterPackSprite1;

    public Image StarterPackSprite2;

	public string StarterPackItem1;

	public string StarterPackItem2;

	public string StarterPackOfferItem;

	public TimeSpan StarterPackValidityDuration;

    public TextMeshProUGUI OldPrice;

    public TextMeshProUGUI NewPrice;

    public TextMeshProUGUI DiscountPercent;

    public TextMeshProUGUI AvailabilityTimer;

	private bool Available;

	private string lastMessage = string.Empty;

	private string lastCancelText = string.Empty;
	
	public GameObject[] DiscountLabels;

	private void Start()
	{
		AppStoreProduct appStoreProduct1;
		AppStoreProduct appStoreProduct2;
		AppStoreProduct starterPackProduct;

		foreach (GameObject label in DiscountLabels)
		{
			label.SetActive(false);
		}
		
		Available = GetProducts(StarterPackItem1, out appStoreProduct1, StarterPackItem2, out appStoreProduct2, StarterPackOfferItem, out starterPackProduct);
		if (Available)
		{
			SetupItem(appStoreProduct1, ref StarterPackSprite1, ref StarterPackAmount1, 0);
			SetupItem(appStoreProduct2, ref StarterPackSprite2, ref StarterPackAmount2, 1);
			CurrencyStringInfo currencyStringInfo = CurrencyUtils.ParseCurrencyString(appStoreProduct1.LocalisedPrice);
			CurrencyStringInfo currencyStringInfo2 = CurrencyUtils.ParseCurrencyString(appStoreProduct2.LocalisedPrice);
			CurrencyStringInfo currencyStringInfo3 = CurrencyUtils.ParseCurrencyString(starterPackProduct.LocalisedPrice);
			double oldPrice = currencyStringInfo.currencyValue + currencyStringInfo2.currencyValue;
			double currencyValue = currencyStringInfo3.currencyValue;
			double discount = Math.Round((oldPrice - currencyValue) / oldPrice * 100.0);
            DiscountPercent.text = string.Format(LocalizationManager.GetTranslation("TEXT_STICKER_DISCOUNT"), discount);
			BundleOfferController.Instance.CurrentOfferDiscount = (int)discount;
			OldPrice.text = CurrencyUtils.FormatCurrencyValue(oldPrice, currencyStringInfo);
            NewPrice.text = starterPackProduct.LocalisedPrice;
            //float num3 = this.OldPrice.transform.position[0] + this.OldPrice.BottomRight[0];
            //float num4 = this.NewPrice.transform.position[0] + this.NewPrice.TopLeft[0];
            //if (num3 > num4)
            //{
            //    float num5 = (this.OldPrice.GetWidth("2") + num3 - num4) / 2f;
            //    this.OldPrice.SetCharacterSize(this.OldPrice.characterSize * (1f - num5 / this.OldPrice.TotalWidth));
            //    this.NewPrice.SetCharacterSize(this.NewPrice.characterSize * (1f - num5 / this.NewPrice.TotalWidth));
            //}
			Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
			{
				{
					Parameters.SPTitle,
					StarterPackOfferItem
				},
				{
					Parameters.SPTime,
					BundleOfferController.Instance.AvailabilityTimer.ToString()
				},
				{
					Parameters.SPDiscount,
					BundleOfferController.Instance.CurrentOfferDiscount.ToString()
				},
				{
					Parameters.SPPopupIndex,
					BundleOfferController.Instance.ActiveOfferData.ID.ToString()
				}
			};
			if (BundleOfferController.Instance.TimerHasExpired())
			{
				Log.AnEvent(Events.LastChanceOffered, data);
			}
			else
			{
				Log.AnEvent(Events.StarterPackOffered, data);
			}
		}
	}

	public static bool CheckAvailability(string item1, string item2, string offerItem)
	{
		AppStoreProduct appStoreProduct;
		AppStoreProduct appStoreProduct2;
		AppStoreProduct appStoreProduct3;
		return GetProducts(item1, out appStoreProduct, item2, out appStoreProduct2, offerItem, out appStoreProduct3);
	}

	private void SetupItem(AppStoreProduct product, ref Image item, ref TextMeshProUGUI value, int index)
	{
		if (product.Identifier.Contains("gold"))
		{
			value.text = CurrencyUtils.GetGoldStringWithIcon(ProductManager.Instance.GetProductWithID(product.Identifier).Gold + ProductManager.Instance.GetProductWithID(product.Identifier).BonusGold);
            //value.font = this.GoldFontText;
            //value.gameObject.renderer.material = this.GoldFontMaterial;
            item.sprite = this.GoldImage;
            
		}
		else
		{
			value.text = CurrencyUtils.GetCashString(ProductManager.Instance.GetProductWithID(product.Identifier).Cash + ProductManager.Instance.GetProductWithID(product.Identifier).BonusCash);
            //value.font = this.SilverFontText;
            //value.gameObject.renderer.material = this.SilverFontMaterial;
            item.sprite = this.SilverImage;
		}
		
		bool isFirstPack = product.CurrencyCode.ToLower().Contains("pack_1");
		if (ProductManager.Instance.discount != 0 && !isFirstPack) {
			DiscountLabels[index].gameObject.SetActive(true);
			if (ProductManager.Instance.discount == 1) {
				DiscountLabels[index].GetComponentInChildren<TextMeshProUGUI>().text = "%15";
			} else if (ProductManager.Instance.discount == 2) {
				DiscountLabels[index].GetComponentInChildren<TextMeshProUGUI>().text = "%30";
			}
		}
		
		//TODO
		DiscountLabels[index].gameObject.SetActive(false);
	}

	private void Update()
	{
		if (Available)
		{
			string timeRemainingMessage = BundleOfferController.Instance.GetTimeRemainingMessage();
			if (!timeRemainingMessage.Equals(lastMessage))
			{
				AvailabilityTimer.text = timeRemainingMessage;
				lastMessage = timeRemainingMessage;
			}
			string cancelText = BundleOfferController.Instance.GetCancelText();
			if (!cancelText.Equals(lastCancelText))
			{
				CancelButton.SetText(cancelText, true, true);
				lastCancelText = cancelText;
			}
		}
	}

	public static bool GetProducts(string item1, out AppStoreProduct a, string item2, out AppStoreProduct b, string offerItem, out AppStoreProduct c)
	{
		if (AppStore.Instance.IsWaitingForProductData)
		{
			a = null;
			b = null;
			c = null;
		}
		else
		{
			List<AppStoreProduct> products = AppStore.Instance.GetProducts();
			a = products.FirstOrDefault((AppStoreProduct q) => q.Identifier==item1);
			b = products.FirstOrDefault((AppStoreProduct q) => q.Identifier==item2);
			c = products.FirstOrDefault((AppStoreProduct q) => q.Identifier==offerItem);
		}
		return a != null && b != null && c != null;
	}
}
