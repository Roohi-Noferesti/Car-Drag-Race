using System;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuyMessageScreen : HUDScreen
{
    [SerializeField] private Text m_messgeText;
    [SerializeField] private Text m_priceText;
    [SerializeField] private Image m_currencyIcon;
    [SerializeField] private Button m_okButton;
    [SerializeField] private Button m_cancelButton;

    public string Message
    {
        set { m_messgeText.text = value; }
    }

    public int Price
    {
        set { m_priceText.text = String.Format("{0}", value); }
    }

    public Sprite CurrencyIcon
    {
        set { m_currencyIcon.sprite = value; }
    }

    public UnityAction OkCommand
    {
        set
        {
            m_okButton.onClick.RemoveAllListeners();
            m_okButton.onClick.AddListener(value);
        }
    }

    public UnityAction CancelCommand
    {
        set
        {
            m_cancelButton.onClick.RemoveAllListeners();
            m_cancelButton.onClick.AddListener(value);
        }
    }
}
