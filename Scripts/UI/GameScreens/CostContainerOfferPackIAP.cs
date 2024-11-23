using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CostContainerOfferPackIAP : MonoBehaviour
{
	public TMP_Text TitleText;

	public TMP_Text BodyText;

	public TMP_Text CarCountText;

	public TMP_Text CostText;

	public TMP_Text OfflineText;

	public Button BuyButton;

	//private BaseRuntimeControl.State ButtonState;

    private UnityAction m_buttonAction;


	private void Awake()
	{
        //this.BuyButton.ForceAwake();
	}

    public void Setup(string title, string carCount, string bodyText, string buttonText, bool isCurrencyAvailable, UnityAction action)
	{
		this.TitleText.text = title;
		this.BodyText.text = bodyText;
		if (isCurrencyAvailable)
		{
            this.CostText.text = buttonText;
            this.CarCountText.text = carCount;
            //this.BuyButton.Runtime.SetCallback(target, action);
		    if (m_buttonAction != null)
		        this.BuyButton.onClick.RemoveListener(m_buttonAction);
            this.BuyButton.onClick.AddListener(action);
		    m_buttonAction = action;
			//this.ButtonState = BaseRuntimeControl.State.Active;
            ////this.BuyButton.Runtime.CurrentState = this.ButtonState;
		}
		else
		{
			BuyButton.interactable = false;
			BuyButton.SetState(BaseRuntimeControl.State.Disabled);
			//this.OfflineText.text = LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_OFFLINE");
			//this.ButtonState = BaseRuntimeControl.State.Disabled;
            //this.BuyButton.Runtime.CurrentState = this.ButtonState;
		}
	}

	public void LateUpdate()
	{
        //if (this.BuyButton.Runtime.CurrentState != this.ButtonState)
        //{
        //    this.BuyButton.Runtime.CurrentState = this.ButtonState;
        //}
	}
}
