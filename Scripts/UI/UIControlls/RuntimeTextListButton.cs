using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RuntimeTextListButton : BaseRuntimeControl
{
	public RuntimeButton LeftButton;

	public RuntimeButton RightButton;

	//public PackedSprite CentralBackground;

	public TextMeshProUGUI CurrentText;

	private int _currentIndex = -1;

	private List<string> _values;

	private MonoBehaviour _callbackTarget;

	private string _callbackFunction;

	public int MaxIndex
	{
		get
		{
			return this._values.Count - 1;
		}
	}

	public int IndexOfCurrentValue
	{
		get
		{
			return this._currentIndex;
		}
	}

	public override Vector2 Size
	{
		get
        {
            return new Vector2();//this.CentralBackground.width + this.LeftButton.width + Mathf.Abs(this.RightButton.width), this.CentralBackground.height);
		}
	}

    public void OnLeft()
	{
		int indexOfCurrentValue = this.IndexOfCurrentValue;
		this.SetCurrentIndex(indexOfCurrentValue - 1);
		this._callbackTarget.Invoke(this._callbackFunction, 0f);
	}

    public void OnRight()
	{
		int indexOfCurrentValue = this.IndexOfCurrentValue;
		this.SetCurrentIndex(indexOfCurrentValue + 1);
		this._callbackTarget.Invoke(this._callbackFunction, 0f);
	}

	private void CheckForDisabledSides()
	{
		if (this._currentIndex == 0)
		{
			this.LeftButton.CurrentState = State.Disabled;
		}
		else
        {
            this.LeftButton.CurrentState = State.Active;
            //this.LeftButton.SetControlState(UIButton.CONTROL_STATE.NORMAL);
		}
		if (this._currentIndex == this._values.Count - 1)
		{
            this.RightButton.CurrentState = State.Disabled;
            //this.RightButton.SetControlState(UIButton.CONTROL_STATE.DISABLED);
		}
		else
		{
            this.RightButton.CurrentState = State.Active;
            //this.RightButton.SetControlState(UIButton.CONTROL_STATE.NORMAL);
		}
		if (this._currentIndex == -1)
		{
            this.LeftButton.CurrentState = State.Disabled;
            this.RightButton.CurrentState = State.Disabled;
   //         this.LeftButton.SetControlState(UIButton.CONTROL_STATE.DISABLED);
			//this.RightButton.SetControlState(UIButton.CONTROL_STATE.DISABLED);
		}
	}

	public void SetButtonsEnabled(bool zLeft, bool zRight)
    {
        this.LeftButton.CurrentState = !zLeft ? State.Disabled : State.Active;
        this.RightButton.CurrentState = !zRight ? State.Disabled : State.Active;
  //      this.LeftButton.SetControlState((!zLeft) ? UIButton.CONTROL_STATE.DISABLED : UIButton.CONTROL_STATE.NORMAL);
		//this.RightButton.SetControlState((!zRight) ? UIButton.CONTROL_STATE.DISABLED : UIButton.CONTROL_STATE.NORMAL);
	}

	public void SetCurrentIndex(int zVal)
	{
		if (this._values.Count == 0)
		{
			return;
		}
		this._currentIndex = zVal;
		if (this._currentIndex >= this._values.Count)
		{
			this._currentIndex -= this._values.Count;
		}
		if (this._currentIndex < 0)
		{
			this._currentIndex += this._values.Count;
		}
		if (this._values.Count > this._currentIndex)
		{
			this._callbackTarget.Invoke(this._callbackFunction, 0f);
			this.CurrentText.text = this._values[this._currentIndex];
		}
		this.CheckForDisabledSides();
	}

	public void AddItemsToList()
	{
	}

	public void ResetList(List<string> items)
	{
		if (items.Count == 0)
		{
			return;
		}
		this._values = items;
		this.SetCurrentIndex(0);
	}

	public override void OnActivate()
	{
		this.AdjustToBe(base.CurrentState);
	}

	public override void OnDeactivate()
	{
	}

	protected override void AdjustToBe(BaseRuntimeControl.State zState)
	{
		bool active = zState == BaseRuntimeControl.State.Active || zState == BaseRuntimeControl.State.Disabled || zState == BaseRuntimeControl.State.Highlight;
		this.RightButton.gameObject.SetActive(active);
		this.LeftButton.gameObject.SetActive(active);
		//this.CentralBackground.gameObject.SetActive(active);
		this.CurrentText.gameObject.SetActive(active);
	}

	public void Init(MonoBehaviour zScriptWithMethodToInvoke, string zCallback, float zWidth, List<string> zValues)
	{
		this._callbackTarget = zScriptWithMethodToInvoke;
		this._callbackFunction = zCallback;
		//float num = zWidth - this.LeftButton.width - Mathf.Abs(this.RightButton.width);
		//this.CentralBackground.SetSize(num, this.CentralBackground.height);
		//GameObjectHelper.SetLocalX(this.LeftButton.gameObject, -num / 2f);
		//GameObjectHelper.SetLocalX(this.RightButton.gameObject, num / 2f);
		this._values = zValues;
		if (zValues.Count > 0)
		{
			this.SetCurrentIndex(0);
		}
	}

	protected override void Update()
	{
	}
}
