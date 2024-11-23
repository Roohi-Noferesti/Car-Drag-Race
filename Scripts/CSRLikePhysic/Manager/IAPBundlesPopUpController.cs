using Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;

public class IAPBundlesPopUpController : MonoBehaviour
{
    public RuntimeTextButton CancelButton;

	public TextMeshProUGUI OldPrice;

	public TextMeshProUGUI NewPrice;

	public GameObject TimerParent;

	public TextMeshProUGUI DiscountPrice;

	public TextMeshProUGUI AvailabilityTimer;

	public GameObject WidgetParent;

	private BundleWidgetCreator widgetCreator = new BundleWidgetCreator();

	private bool Available;

	private bool HasTimer = true;

	private string lastMessage = string.Empty;

	private string lastCancelText = string.Empty;

	private void Start()
	{
		List<AppStoreProduct> products = AppStore.Instance.GetProducts();
		BundleOfferPopupData offerPopupData;
		if (CheatEngine.isFromCheatEngine)
		{
			offerPopupData = CheatEngine.PopupData;
		}
		else
		{
			offerPopupData = BundleOfferController.Instance.ActiveOfferData.PopupData;
		}
		List<IBundleOfferWidgetInfo> widgetInfo = offerPopupData.WidgetInfo;
        AppStoreProduct appStoreProduct = products != null
            ? products.FirstOrDefault((AppStoreProduct q) => q.Identifier == offerPopupData.BundleOfferItem)
            : null;
		this.Available = (appStoreProduct != null);
		if (this.Available)
		{
			CurrencyStringInfo currencyStringInfo = CurrencyUtils.ParseCurrencyString(appStoreProduct.LocalisedPrice);
			double num = this.CalculateOldCost(products, widgetInfo);
			this.OldPrice.text = CurrencyUtils.FormatCurrencyValue(num, currencyStringInfo);
			double currencyValue = currencyStringInfo.currencyValue;
            this.NewPrice.text = CurrencyUtils.FormatCurrencyValue(currencyValue, currencyStringInfo);
			double num2 = Math.Round((num - currencyValue) / num * 100.0);
            this.DiscountPrice.text = string.Format(LocalizationManager.GetTranslation("TEXT_STICKER_DISCOUNT"), num2);
			BundleOfferController.Instance.CurrentOfferDiscount = (int)num2;
            //float num3 = this.OldPrice.transform.position[0] + this.OldPrice.BottomRight[0];
            //float num4 = this.NewPrice.transform.position[0] + this.NewPrice.TopLeft[0];
            //if (num3 > num4)
            //{
            //    float num5 = (this.OldPrice.GetWidth("2") + num3 - num4) / 2f;
            //    this.OldPrice.SetCharacterSize(this.OldPrice.characterSize * (1f - num5 / this.OldPrice.TotalWidth));
            //    this.NewPrice.SetCharacterSize(this.NewPrice.characterSize * (1f - num5 / this.NewPrice.TotalWidth));
            //}
			this.HasTimer = offerPopupData.TimerActive;
			this.TimerParent.SetActive(this.HasTimer);
			if(!CheatEngine.isFromCheatEngine)
				this.UpdateTimer();
			this.SetupUIWidgets(widgetInfo);
			CheatEngine.isFromCheatEngine = false;
			Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
			{
				{
					Parameters.SPTitle,
					BundleOfferController.Instance.OfferItem
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
				Log.AnEvent(Events.BundlePackOffered, data);
			}
		}
	}

	private void SetupUIWidgets(List<IBundleOfferWidgetInfo> widgetsToCreate)
	{
	    for (int i = 0; i < widgetsToCreate.Count; i++)
	    {
	        IBundleOfferWidgetInfo current = widgetsToCreate[i];
            this.CreateWidget(current, i < widgetsToCreate.Count-1);
	    }
	}

    private void CreateWidget(IBundleOfferWidgetInfo widgetInfo,bool placePlusSign)
	{
		GameObject widgetIns = this.widgetCreator.LoadPrefab(widgetInfo.OfferType);
		widgetIns.transform.SetParent(this.WidgetParent.transform,false);
		BundleOfferWidget component = widgetIns.GetComponent<BundleOfferWidget>();
		component.Setup(widgetInfo);

        if (placePlusSign)
        {
            //Place plus sign
            string path = "BundleOfferWidgets/Plus";
            UnityEngine.Object original = Resources.Load(path);
            widgetIns = UnityEngine.Object.Instantiate(original) as GameObject;
            widgetIns.transform.SetParent(this.WidgetParent.transform, false);
        }
	}

	private double CalculateOldCost(List<AppStoreProduct> appStoreProducts, List<IBundleOfferWidgetInfo> widgetInfo)
	{
		double num = 0.0;
		foreach (IBundleOfferWidgetInfo widget in widgetInfo)
		{
			if (!string.IsNullOrEmpty(widget.ShopItem))
			{
				AppStoreProduct appStoreProduct = appStoreProducts.FirstOrDefault((AppStoreProduct q) => q.Identifier==widget.ShopItem);
				if (appStoreProduct != null)
				{
					CurrencyStringInfo currencyStringInfo = CurrencyUtils.ParseCurrencyString(appStoreProduct.LocalisedPrice);
					num += currencyStringInfo.currencyValue;
				}
			}
		}
		return num;
	}

	private void UpdateTimer()
	{
		if (this.Available && this.HasTimer)
		{
			string timeRemainingMessage = BundleOfferController.Instance.GetTimeRemainingMessage();
			if (!timeRemainingMessage.Equals(this.lastMessage))
			{
				this.AvailabilityTimer.text = timeRemainingMessage;
				this.lastMessage = timeRemainingMessage;
			}
			string cancelText = BundleOfferController.Instance.GetCancelText();
			if (!cancelText.Equals(this.lastCancelText))
			{
				this.CancelButton.SetText(cancelText, true, true);
				this.lastCancelText = cancelText;
			}
		}
	}

	private void Update()
	{
		if (PlayerPrefs.GetString("mode") != "cheat")
			this.UpdateTimer();
	}
}
