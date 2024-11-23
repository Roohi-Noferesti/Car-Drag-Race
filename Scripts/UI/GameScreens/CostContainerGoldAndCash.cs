using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CostContainerGoldAndCash : MonoBehaviour
{
	public TextMeshProUGUI TitleText;

	public TextMeshProUGUI MiddleText;

    public RuntimeTextButton GoldButton;

    public RuntimeTextButton CashButton;

	private void Awake()
	{
        ////this.GoldButton.ForceAwake();
        ////this.CashButton.ForceAwake();
	}

    public void Setup(string title, int gold, int cash, UnityAction method_cash, UnityAction method_gold)
    {
        if (this.TitleText != null)
            this.TitleText.text = title;
        this.MiddleText.gameObject.SetActive(true);
        this.MiddleText.text = LocalizationManager.GetTranslation("TEXT_COST_BOX_OR");
        this.CashButton.SetText(CurrencyUtils.GetCashNavbarString(cash), true, false);
        this.CashButton.SetCallback(method_cash);
        this.CashButton.CurrentState = BaseRuntimeControl.State.Active;
        this.GoldButton.SetText(CurrencyUtils.GetGoldString(gold), true, false);
        this.GoldButton.SetCallback(method_gold);
        this.GoldButton.CurrentState = BaseRuntimeControl.State.Active;
	}
}
