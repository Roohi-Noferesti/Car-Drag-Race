using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColourSwatchListItem : ListItem
{
	public const float _fadeTime = 0.5f;

	public Button Button;

	protected bool Pressed;

	private Color _colour;

	private int _colourIndex;

	//private float _opacityTimer;

	public AudioSfx onClickSound = AudioSfx.PaintChange;

	public Color Colour
	{
		get
		{
			return this._colour;
		}
	}

	public int ColorIndex
	{
		get
		{
			return this._colourIndex;
		}
	}

	public override void GreyOutThisItem(bool zVal)
	{
		if (zVal != base.IsThisGreyedOut())
		{
			//this._opacityTimer = 0.5f;
		}
		base.GreyOutThisItem(zVal);
	}

	public override void AdjustToBe(BaseRuntimeControl.State zState)
	{
		base.AdjustToBe(zState);
		if (zState == BaseRuntimeControl.State.Active)
		{
			this.Pressed = false;
			this.AdjustForNewPressedState();
		}
	}

	protected override void Show()
	{
		this.Pressed = false;
		this._thisIsDisabled = false;
	}

	protected override void Hide()
	{
		this.Pressed = false;
	}

	protected override void Update()
	{
		base.Update();
		//float num = 0.25f;
		//float num2 = 1f;
		//if (this._thisIsDisabled || base.IsThisGreyedOut())
		//{
		//	num2 = num;
		//}
		//if (this._opacityTimer > 0f)
		//{
		//	this._opacityTimer = Mathf.Max(0f, this._opacityTimer - Time.deltaTime);
		//	float num3 = this._opacityTimer / 0.5f;
		//	if (!this._thisIsDisabled && !base.IsThisGreyedOut())
		//	{
		//		num3 = 1f - num3;
		//	}
		//	num2 = num + (1f - num) * num3;
		//}
  //      //Color color = this.Button.color;
        //if (color.a != num2)
        //{
        //    color.a = num2;
        //    this.Button.SetColor(color);
        //}
		if (!this.Pressed)
		{
            //if (this.Button.controlState == UIButton.CONTROL_STATE.ACTIVE)
            //{
            //    if (TouchManager.TryAndGetCarouselPressedLock())
            //    {
            //        this._iHaveThePressedLock = true;
            //    }
            //    if (this._iHaveThePressedLock)
            //    {
            //        this.Pressed = true;
            //        this.AdjustForNewPressedState();
            //    }
            //}
		}
        //else if (this.Pressed && this.Button.controlState != UIButton.CONTROL_STATE.ACTIVE)
        //{
        //    if (this._iHaveThePressedLock)
        //    {
        //        TouchManager.ReleaseCarouselPressedLock();
        //        this._iHaveThePressedLock = false;
        //    }
        //    this.Pressed = false;
        //    this.AdjustForNewPressedState();
        //}
	}

	private void OnClick(IPointerClickHandler zSender)
	{
		base.InvokeTapEvent();
	}

	public void Create(Color zColour, int zColorIndex)
	{
		this._colour = zColour;
		this._colourIndex = zColorIndex;
        //this.Button.SetColor(zColour);
		this.Pressed = false;
        //this.Button.AddValueChangedDelegate(new EZValueChangedDelegate(this.OnClick));
        //this.Button.AddValueChangedDelegate(new EZValueChangedDelegate(this.onPlayButtonSound));
		this.AdjustForNewPressedState();
	}

    public void onPlayButtonSound(IPointerClickHandler obj)
	{
		MenuAudio.Instance.playSound(this.onClickSound);
	}

	private void AdjustForNewPressedState()
	{
	}
}
