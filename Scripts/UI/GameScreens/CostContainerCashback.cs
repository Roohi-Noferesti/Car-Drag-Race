using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CostContainerCashback : MonoBehaviour
{
	public TextMeshProUGUI TitleText;

	public Button BuyButton;

	public TextMeshProUGUI GoldCost;

    public TextMeshProUGUI CentreText;

    public TextMeshProUGUI CashbackAmount;

    private UnityAction m_buttonAction;

	private void Awake()
	{
        //this.BuyButton.ForceAwake();
	}

	public void Setup(int goldCost, int cashback, UnityAction action)
	{
        this.TitleText.text = LocalizationManager.GetTranslation("TEXT_BUTTON_BUY").ToUpper();
        this.GoldCost.text = CurrencyUtils.GetGoldString(goldCost);
        this.CentreText.text = LocalizationManager.GetTranslation("TEXT_AND_GET_CASHBACK");
        this.CashbackAmount.text = CurrencyUtils.GetCashString(cashback);

	    if (m_buttonAction != null)
	        BuyButton.onClick.RemoveListener(m_buttonAction);
	    BuyButton.onClick.AddListener(action);
	    m_buttonAction = action;
	    //this.BuyButton.GetComponentInChildren<Text>().SetCallback(target, action);
	}
}
