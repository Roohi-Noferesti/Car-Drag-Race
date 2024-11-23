using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopUpMultipleChoice : MonoBehaviour
{
	public RuntimeTextButton[] Buttons;

	private List<ButtonDetails> ButtonSpecs;

	private BubbleMessage BubbleMessage;

	private void Awake()
	{
	    if (Buttons.Length > 2)
	    {
	        Buttons[2].CurrentState = BaseRuntimeControl.State.Hidden;
	    }
	}

	private void OnDestroy()
	{
		if (this.BubbleMessage)
		{
			this.BubbleMessage.Dismiss();
			this.BubbleMessage = null;
		}
	}

	public void Setup(List<ButtonDetails> choices)
	{
		this.ButtonSpecs = choices;
		int num = 0;
		foreach (ButtonDetails current in choices)
		{
            //this.Buttons[num].ButtonType = current.Type;
		    this.Buttons[num].AddValueChangedDelegate(Getmethod(num));
            //this.Buttons[num].ForceAwake();
            this.Buttons[num].CurrentState = BaseRuntimeControl.State.Active;
		    this.Buttons[num].SetText(current.Label,true,false);
			if (current.Disabled)
			{
                this.Buttons[num].CurrentState = BaseRuntimeControl.State.Disabled;
			}
            //if (!string.IsNullOrEmpty(current.BubbleMessage))
            //{
            //    BubbleMessage.NippleDir nippleDir = (num != 0) ? BubbleMessage.NippleDir.UP : BubbleMessage.NippleDir.DOWN;
            //    base.StartCoroutine(BubbleManager.Instance.ShowMessageDelayed(0.5f, current.BubbleMessage, true, this.Buttons[num].gameObject.transform, new Vector3(0.7f, -0.05f, -0.05f), nippleDir, 0.25f, BubbleMessageConfig.ThemeStyle.SMALL,false, delegate(BubbleMessage x)
            //    {
            //        this.BubbleMessage = x;
            //    }));
            //}
			num++;
			if (num >= this.Buttons.Length)
			{
				break;
			}
		}
	}

    private UnityAction Getmethod(int x)
    {
        switch (x)
        {
            case 0:
                return IndirectionViaReflection0;
            case 1:
                return IndirectionViaReflection1;
            case 2:
                return IndirectionViaReflection2;
            case 3:
                return IndirectionViaReflection3;
        }
        return null;
    }

	private void ExecuteButtonAction(int ix)
	{
		if (this.ButtonSpecs[ix].Action != null)
		{
			this.ButtonSpecs[ix].Action();
		}
	}

	public void IndirectionViaReflection0()
	{
		this.ExecuteButtonAction(0);
	}

	public void IndirectionViaReflection1()
	{
		this.ExecuteButtonAction(1);
	}

	public void IndirectionViaReflection2()
	{
		this.ExecuteButtonAction(2);
	}

	public void IndirectionViaReflection3()
	{
		this.ExecuteButtonAction(3);
	}
}
