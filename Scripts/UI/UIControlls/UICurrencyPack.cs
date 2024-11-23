using System;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Z2HSharedLibrary.DatabaseEntity;

public class UICurrencyPack : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_amountText;
    [SerializeField] private TextMeshProUGUI m_priceText;
    [SerializeField] private GameObject m_discountPanel;
    [SerializeField] private TextMeshProUGUI m_discountText;
    [SerializeField]
    private Button m_buyButton;

    public double Price
    {
        set
        {
            var price_format = LocalizationManager.GetTranslation("TEXT_PRICE");
            var price_amount = String.Format("{0:n0}", value).ToNativeNumber();
            m_priceText.text = String.Format(price_format, price_amount);
        }
    }

    public string PriceString
    {
        set
        {
            //var price_format = LocalizationManager.GetTranslation("TEXT_PRICE");
            //var price_amount = String.Format("{0:n0}", value).ToNativeNumber();
            //m_priceText.text = String.Format(price_format, price_amount);

            m_priceText.text = value.ToCurrency();
            //price = marketPrice.ToCurrencyValue();
            //var discount = (m_originalItemPrice - price) / m_originalItemPrice;
            //if (m_discountObject != null)
            //{
            //    m_discountObject.SetActive(discount > 0 && m_enabled);
            //    m_discountText.text = LocalizationManager.FixRTL_IfNeeded(String.Format("{0:P0}", discount));
            //}
        }
    }

    public int Amount
    {
        set { m_amountText.text = String.Format("x {0:n0}", value).ToNativeNumber(); }
    }

    public void Set(ProductData productData,UnityAction clickAction,bool isGold)
    {
        double price;
        string marketPrice;
        StoreManagerV2.TryGetMarketItemPrice(productData.GtProduct.Code, out price, out marketPrice);
        if (string.IsNullOrEmpty(marketPrice))
            Price = price;
        else
        {
            PriceString = marketPrice;
        }
        m_amountText.text = isGold
            ? CurrencyUtils.GetGoldStringWithIcon(productData.GtProduct.Gold)
            : CurrencyUtils.GetCashString(productData.GtProduct.Cash);
        //var itemID = itemRelation.VirtualItemID;
        m_buyButton.onClick.RemoveAllListeners();
        m_buyButton.onClick.AddListener(clickAction);//
    }
}
