using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class GenericListItem : ListItem
{
	public Button Button;

	protected bool Pressed;

	public AudioSfx onClickSound = AudioSfx.MenuClickForward;

	public Image CentralSprite;

    public Image CentralSpriteHit;

    public Image LeftEdge;

    public Image RightEdge;

    public Image LeftSpacer;

    public Image RightSpacer;

    public Image LeftSpacerHit;

    public Image RightSpacerHit;

	//private float _width;

	private float _height;

	private bool _roundLeft;

	private bool _roundRight;

	private bool _parentWantsRoundedEdges;

	public void ShowRoundedEdges(bool zShow)
	{
		this._parentWantsRoundedEdges = zShow;
		if (zShow)
		{
			this.LeftEdge.gameObject.SetActive(this._roundLeft);
			this.RightEdge.gameObject.SetActive(this._roundRight);
		}
		else
		{
			this.LeftEdge.gameObject.SetActive(false);
			this.RightEdge.gameObject.SetActive(false);
		}
	}

	protected override void Show()
	{
		this.Pressed = false;
		if (this.Button != null)
		{
			this.Button.gameObject.SetActive(true);
			BoxCollider component = this.Button.gameObject.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.enabled = true;
			}
			this.Button.enabled = true;
            //this.Button.controlIsEnabled = true;
		}
		this.CentralSprite.gameObject.SetActive(true);
		this.CentralSpriteHit.gameObject.SetActive(false);
		this.ShowRoundedEdges(this._parentWantsRoundedEdges);
		this.LeftSpacer.gameObject.SetActive(true);
		this.RightSpacer.gameObject.SetActive(true);
		this.LeftSpacerHit.gameObject.SetActive(false);
		this.RightSpacerHit.gameObject.SetActive(false);
		this._thisIsDisabled = false;
	}

	protected override void Hide()
	{
		this.Pressed = false;
		this.Button.gameObject.SetActive(false);
		this.CentralSprite.gameObject.SetActive(false);
		this.CentralSpriteHit.gameObject.SetActive(false);
		this.LeftEdge.gameObject.SetActive(false);
		this.RightEdge.gameObject.SetActive(false);
		this.LeftSpacer.gameObject.SetActive(false);
		this.RightSpacer.gameObject.SetActive(false);
		this.LeftSpacerHit.gameObject.SetActive(false);
		this.RightSpacerHit.gameObject.SetActive(false);
	}

	private void OnClick(IPointerClickHandler zSender)
	{
		if (!base.IsThisGreyedOut())
		{
			MenuAudio.Instance.playSound(this.onClickSound);
		}
		base.InvokeTapEvent();
	}

	protected virtual void AdjustForNewPressedState()
	{
		if (base.IsThisGreyedOut() || this._thisIsDisabled)
		{
			return;
		}
		this.CentralSprite.gameObject.SetActive(!this.Pressed);
		this.CentralSpriteHit.gameObject.SetActive(this.Pressed);
		this.LeftSpacer.gameObject.SetActive(!this.Pressed);
		this.RightSpacer.gameObject.SetActive(!this.Pressed);
		this.LeftSpacerHit.gameObject.SetActive(this.Pressed);
		this.RightSpacerHit.gameObject.SetActive(this.Pressed);
	}

	protected override void Update()
	{
		base.Update();
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

	protected void BaseCreate(float zWidth, bool zFirstElement, bool zLastElement)
	{
		this.Pressed = false;
		this.Show();
		//this._width = zWidth;
        //this._height = this.CentralSprite.height;
		this._roundLeft = zFirstElement;
		this._roundRight = zLastElement;
        //if (this.Button != null)
        //{
        //    this.Button.SetSize(this._width, this._height);
        //    this.Button.AddValueChangedDelegate(new EZValueChangedDelegate(this.OnClick));
        //    this.Button.gameObject.name = "Button: " + base.gameObject.name;
        //}
        //float w = this._width - this.LeftSpacer.width - this.RightSpacer.width;
        //this.CentralSprite.SetSize(w, this._height);
        //float w2 = this._width - this.LeftSpacerHit.width - this.RightSpacerHit.width;
        //this.CentralSpriteHit.SetSize(w2, this._height);
        //GameObjectHelper.SetLocalX(this.LeftSpacer.gameObject, -this.CentralSprite.width / 2f);
        //GameObjectHelper.SetLocalX(this.LeftSpacerHit.gameObject, -this.CentralSpriteHit.width / 2f);
        //GameObjectHelper.SetLocalX(this.LeftEdge.gameObject, -(this.CentralSprite.width / 2f) - this.LeftSpacer.width);
        //GameObjectHelper.SetLocalX(this.RightSpacer.gameObject, this.CentralSprite.width / 2f);
        //GameObjectHelper.SetLocalX(this.RightSpacerHit.gameObject, this.CentralSpriteHit.width / 2f);
        //GameObjectHelper.SetLocalX(this.RightEdge.gameObject, this.CentralSprite.width / 2f + this.RightSpacer.width);
		this.LeftEdge.gameObject.SetActive(zFirstElement);
		this.RightEdge.gameObject.SetActive(zLastElement);
		this.AdjustForNewPressedState();
		this.GreyOutThisItem(false);
		this._thisIsDisabled = false;
	}
}
