using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CostContainerFreeUpgrades : MonoBehaviour
{
	public TextMeshProUGUI TitleText;

    public TextMeshProUGUI GoldCostText;

    public TextMeshProUGUI SavingText;

	public Button BuyButton;

    private UnityAction buttonAction;

	private void Awake()
	{
        //this.BuyButton.ForceAwake();
	}

	public void Setup(int upgradelevel, int goldcost, UnityAction action, int cashSaving)
	{
		this.TitleText.text = string.Format(LocalizationManager.GetTranslation("TEXT_AND_GET_FREE_UPGRADES"), upgradelevel);
		this.GoldCostText.text = CurrencyUtils.GetGoldString(goldcost);
		this.SavingText.text = CurrencyUtils.GetCashString(cashSaving);
        //this.BuyButton.Runtime.SetCallback(target, action);
        if(buttonAction!=null)
        this.BuyButton.onClick.RemoveListener(buttonAction);
        this.BuyButton.onClick.AddListener(action);
	    buttonAction = action;
	}
}
