using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CostContainerGoldOrCash : MonoBehaviour
{
	public GameObject DiscountParent;

	public TextMeshProUGUI TitleText;

	public TextMeshProUGUI MiddelText;

	public TextMeshProUGUI DiscountText;

	public RuntimeTextButton CashButton;

    public RuntimeTextButton GoldButton;

	private void Awake()
	{
        //this.BuyButton.ForceAwake();
	}

	public void Setup(string title, int discount = 0)
	{
        if (this.TitleText!=null)
            this.TitleText.text = LocalizationManager.GetTranslation("TEXT_BUTTON_BUY").ToUpper();

	    if (this.DiscountParent != null)
	    {
	        this.DiscountParent.gameObject.SetActive(discount > 0);
	        if (discount > 0)
	        {
	            this.DiscountText.text = string.Format(LocalizationManager.GetTranslation("TEXT_STICKER_DISCOUNT"), discount);
	        }
	    }
	}

    public void SetupButtonForGold(int goldCost, UnityAction action)
    {
        if (MiddelText != null)
            MiddelText.gameObject.SetActive(false);
        this.GoldButton.CurrentState = BaseRuntimeControl.State.Active;
        this.CashButton.CurrentState = BaseRuntimeControl.State.Hidden;
        this.GoldButton.SetText(CurrencyUtils.GetGoldString(goldCost), true, false);
        this.GoldButton.SetCallback(action);
    }

    public void SetupButtonForCash(int cashCost, UnityAction action)
    {
        if (MiddelText != null)
            MiddelText.gameObject.SetActive(false);
        this.CashButton.SetText(CurrencyUtils.GetCashNavbarString(cashCost), true, false);
        this.CashButton.SetCallback(action);
        this.CashButton.CurrentState = BaseRuntimeControl.State.Active;
        this.GoldButton.CurrentState = BaseRuntimeControl.State.Hidden;
    }


    public void SetupButtonForKey(int keyCost, UnityAction action)
    {
        if (MiddelText != null)
            MiddelText.gameObject.SetActive(false);
        this.GoldButton.CurrentState = BaseRuntimeControl.State.Hidden;
        this.CashButton.CurrentState = BaseRuntimeControl.State.Hidden;
    }
}
