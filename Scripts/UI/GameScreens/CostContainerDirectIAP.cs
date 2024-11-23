using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CostContainerDirectIAP : MonoBehaviour
{
	public Text TitleText;

	public Text BodyText;

	public Text CostText;

	public Text OfflineText;

	public Button BuyButton;

	//private BaseRuntimeControl.State ButtonState;

    private UnityAction m_buttonAction;


	private void Awake()
	{
        //this.BuyButton.ForceAwake();
	}

	public void Setup(string title, string bodyText, string buttonText, bool isCurrencyAvailable, MonoBehaviour target, UnityAction action)
	{
		this.TitleText.text = title;
		this.BodyText.text = bodyText;
		if (isCurrencyAvailable)
		{
			this.CostText.text = buttonText;
            //this.BuyButton.Runtime.SetCallback(target, action);
		    if (m_buttonAction != null)
		        BuyButton.onClick.RemoveListener(m_buttonAction);
		    BuyButton.onClick.AddListener(action);
		    m_buttonAction = action;
			//this.ButtonState = BaseRuntimeControl.State.Active;
            //this.BuyButton.Runtime.CurrentState = this.ButtonState;
		}
		else
		{
			this.OfflineText.text = buttonText;
			//this.ButtonState = BaseRuntimeControl.State.Disabled;
            //this.BuyButton.Runtime.CurrentState = this.ButtonState;
		}
	}

	public void LateUpdate()
	{
        ////if (this.BuyButton.Runtime.CurrentState != this.ButtonState)
        ////{
        ////    this.BuyButton.Runtime.CurrentState = this.ButtonState;
        ////}
	}
}
