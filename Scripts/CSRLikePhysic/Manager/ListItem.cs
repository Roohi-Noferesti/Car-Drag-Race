using UnityEngine;

public abstract class ListItem : MonoBehaviour
{
	protected bool _thisIsGreyedOut;

	protected bool _thisIsDisabled;

	protected bool _iHaveThePressedLock;

	protected bool _ignoreThePressLock;

	protected bool _useOwnTouchEvent;

	protected bool _allowTapsOnGreyedOut;

	public float ListItemHeight = 0.2f;

	private bool _onScreen;

    public event TapEventHandler Tap;

    public event TapEventHandler GreyedOutTap;

	public virtual bool Ready
	{
		get
		{
			return true;
		}
	}

	public bool OnScreen
	{
		get
		{
			return this._onScreen;
		}
		set
		{
			if (this._onScreen != value)
			{
				this._onScreen = value;
				if (this._onScreen)
				{
					this.OnMoveOnScreen();
				}
				else
				{
					this.OnMoveOffScreen();
				}
			}
		}
	}

	public virtual void GreyOutThisItem(bool disable)
	{
		this._thisIsGreyedOut = disable;
	}

	protected bool IsThisGreyedOut()
	{
		return this._thisIsGreyedOut;
	}

	protected void OnDeactivate()
	{
		if (this._iHaveThePressedLock)
		{
			TouchManager.ReleaseCarouselPressedLock();
			this._iHaveThePressedLock = false;
		}
	}

	protected void InvokeTapEvent()
	{
		if (this._iHaveThePressedLock || this._ignoreThePressLock)
		{
			TouchManager.ReleaseCarouselPressedLock();
			this._iHaveThePressedLock = false;
			if (!this._thisIsGreyedOut && !this._thisIsDisabled && (this._useOwnTouchEvent || TouchManager.AttemptToUseButton(base.gameObject.name)) && this.Tap != null)
			{
				this.Tap(this);
			}
			if (this._thisIsGreyedOut && this.GreyedOutTap != null)
			{
				this.GreyedOutTap(this);
			}
		}
	}

	protected virtual void Awake()
	{
		this._thisIsGreyedOut = false;
		this._thisIsDisabled = false;
	}

	protected virtual void Start()
	{
		this.Update();
	}

	protected virtual void Update()
	{
	}

	public virtual void AdjustToBe(BaseRuntimeControl.State zState)
	{
		switch (zState)
		{
		case BaseRuntimeControl.State.Active:
			this._thisIsDisabled = false;
			this.Show();
			break;
		case BaseRuntimeControl.State.Pressed:
			this._thisIsDisabled = false;
			break;
		case BaseRuntimeControl.State.Disabled:
			this._thisIsDisabled = true;
			break;
		case BaseRuntimeControl.State.Hidden:
			this.Hide();
			break;
		}
	}

	protected virtual void Show()
	{
		if (this.OnScreen)
		{
			base.gameObject.SetActive(true);
		}
		this._thisIsDisabled = false;
		this._thisIsGreyedOut = false;
	}

	protected virtual void OnDisable()
	{
		if (this._iHaveThePressedLock)
		{
			TouchManager.ReleaseCarouselPressedLock();
			this._iHaveThePressedLock = false;
		}
	}

	protected virtual void OnEnable()
	{
		this._thisIsGreyedOut = false;
		this._thisIsDisabled = false;
		if (this._iHaveThePressedLock)
		{
			TouchManager.ReleaseCarouselPressedLock();
			this._iHaveThePressedLock = false;
		}
	}

	protected virtual void OnDestroy()
	{
		this.Tap = null;
		if (this._iHaveThePressedLock)
		{
			TouchManager.ReleaseCarouselPressedLock();
			this._iHaveThePressedLock = false;
		}
	}

	public virtual void OnMoveOffScreen()
	{
	}

	public virtual void OnMoveOnScreen()
	{
	}

	public virtual void OnShowFlowConditionBubbleMessage(FlowConditionBase condition)
	{
	}

	protected virtual void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public virtual void Shutdown()
	{
	}

	public virtual void OnActivate()
	{
		this.Update();
	}
}
