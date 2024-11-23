using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class HorizontalList : BaseList
{
	private const float MIN_TIME_BETWEEN_DRAG_AND_FLICK = 0.2f;

	private const float DAMPING = 800f;

	private const float MASS = 50f;

	private const float STIFFNESS = 5000f;

	private const float CUTOFF_POINT = 0.51f;

	private const float DRAG_MULTIPLIER = 3f;

	public bool ForceCarouselOff;

	private bool _listenForGestures = true;

	private float _dragFlickTimer;

	private bool _isDragging;

	private int _selectedIndex;

	private float _velocity;

	private float _dragFractionToNextIcon;

	private bool _trackerFlag;

	private float _springyPosition;

	private int _numBufferItems = 1;

    public event BaseList.ListEventDelegate OnSelectionChangeEvent;

    public event BaseList.ListEventDelegate OnItemPressedEvent;

	public int SelectedIndex
	{
		get
		{
			return this._selectedIndex;
		}
		set
		{
			this._selectedIndex = value;
			if (this.ShouldApplyCarouselBehaviour)
			{
				if (this._selectedIndex >= base.NumItems)
				{
					this._selectedIndex = 0;
				}
				else if (this._selectedIndex < 0)
				{
					this._selectedIndex = base.NumItems - 1;
				}
			}
			else if (this._selectedIndex >= base.NumItems)
			{
				this._selectedIndex = base.NumItems - 1;
			}
			else if (this._selectedIndex < 0)
			{
				this._selectedIndex = 0;
			}
			if (!this._isDragging)
			{
				this._dragFractionToNextIcon = 0f;
				this._trackerFlag = false;
				if (this.items.Count > 0)
				{
					this._springyPosition = this.items[this._selectedIndex].transform.localPosition.x;
				}
			}
			else
			{
				this._dragFlickTimer = 0.2f;
			}
			this.TriggerSelectionChangeEvent(this.SelectedItem);
		}
	}

	public bool ListenForGestures
	{
		get
		{
			return this._listenForGestures;
		}
		set
		{
			this._listenForGestures = value;
		}
	}

	public bool IsBeingDragged
	{
		get
		{
			return this._isDragging;
		}
	}

	public int NumIconsOnScreen
	{
		get
		{
			return this.NumIconsOnSideOfSelectedItem * 2 + 1;
		}
	}

	private int NumIconsOnSideOfSelectedItem
	{
		get
		{
		    //float num = (GUICamera.Instance.ScreenWidth - base.ItemWidth) / 2f;
            //float f = num / base.ItemWidth;
            //return Mathf.CeilToInt(f);
		    return 0;
		}
	}

	public int NumBufferItems
	{
		get
		{
			return this._numBufferItems;
		}
		set
		{
			this._numBufferItems = Mathf.Max(0, value);
		}
	}

	public ListItem SelectedItem
	{
		get
		{
			if (this.SelectedIndex >= this.items.Count || this.SelectedIndex < 0)
			{
				return null;
			}
			return this.items[this.SelectedIndex];
		}
	}

	private int IndexOfCentralItem
	{
		get
		{
			return this.NumIconsOnSideOfSelectedItem;
		}
	}

	private float GapOnSideFromCrushingTheWidthOfTheItems
	{
		get
		{
		    //return (GUICamera.Instance.ScreenWidth - base.ItemWidth * (float)(this.NumIconsOnScreen - 1)) / 2f;
		    return 0;
		}
	}

	private bool ShouldApplyCarouselBehaviour
	{
		get
		{
			return !this.ForceCarouselOff && base.NumItems >= this.NumIconsOnScreen;
		}
	}

	public override Vector2 Size
	{
		get
		{
		    //return new Vector2(GUICamera.Instance.ScreenWidth, this.Height);
		    return new Vector2();
		}
	}

	public override Vector2 Center
	{
		get
		{
		    //return GUICamera.Instance.WorldToCameraSpace(base.transform.position);
		    return new Vector2();
		}
	}

	public override void OnActivate()
	{
		base.OnActivate();
		GestureEventSystem.Instance.Drag += new GestureEventSystem.GestureEventHandler(this.OnDragStart);
		GestureEventSystem.Instance.DragUpdate += new GestureEventSystem.GestureEventHandler(this.OnDragUpdate);
		GestureEventSystem.Instance.DragComplete += new GestureEventSystem.GestureEventHandler(this.OnDragEnd);
		GestureEventSystem.Instance.Flick += new GestureEventSystem.GestureEventHandler(this.OnFlick);
		this.Update();
		this.FixedUpdate();
	}

	protected override void Start()
	{
		base.Start();
		this.FixedUpdate();
	}

	protected override void Update()
	{
		if (this._dragFlickTimer > 0f)
		{
			this._dragFlickTimer -= Time.deltaTime;
			if (this._dragFlickTimer < 0f)
			{
				this._dragFlickTimer = 0f;
			}
		}
		this.NotFixedUpdateJustNormalUpdate();
	}

	public override void MarkAllItemsAdded()
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			GenericListItem genericListItem = this.items[i] as GenericListItem;
			if (genericListItem != null)
			{
				genericListItem.ShowRoundedEdges(!this.ShouldApplyCarouselBehaviour);
			}
		}
		this._isDragging = false;
		this._velocity = 0f;
		this._dragFractionToNextIcon = 0f;
		this._trackerFlag = false;
		this._springyPosition = 0f;
		base.MarkAllItemsAdded();
	}

	public override void OnDeactivate()
	{
		GestureEventSystem.Instance.Flick -= new GestureEventSystem.GestureEventHandler(this.OnFlick);
		GestureEventSystem.Instance.Drag -= new GestureEventSystem.GestureEventHandler(this.OnDragStart);
		GestureEventSystem.Instance.DragUpdate -= new GestureEventSystem.GestureEventHandler(this.OnDragUpdate);
		GestureEventSystem.Instance.DragComplete -= new GestureEventSystem.GestureEventHandler(this.OnDragEnd);
	}

	private void NotFixedUpdateJustNormalUpdate()
	{
		if (base.NumItems == 0)
		{
			return;
		}
		this.SpringUpdate();
		Vector3 b = new Vector3(this.ClampDragFraction(), 0f, 0f);
		for (int i = 0; i < this.items.Count; i++)
		{
			ListItem listItem = this.items[i];
			Vector3 localPosition = listItem.transform.localPosition;
			Vector3 vector = new Vector3(this.TargetWorldPositionX(i, this.ShouldApplyCarouselBehaviour), 0f, 0f);
			Vector3 vector2 = vector;
			if (this._isDragging)
			{
				vector2 += b;
			}
			else
			{
				vector2 += new Vector3(this._springyPosition, 0f, 0f);
			}
            //vector2 = GameObjectHelper.MakeLocalPositionPixelPerfect(vector2);
			if (vector2 != localPosition)
			{
				listItem.transform.localPosition = vector2;
				ListItemProxy x = listItem as ListItemProxy;
				if (x != null)
				{
					int num = this.SelectedIndex + (this.NumIconsOnSideOfSelectedItem + this.NumBufferItems);
					int num2 = this.SelectedIndex - (this.NumIconsOnSideOfSelectedItem + this.NumBufferItems);
					bool flag = num2 < i && i < num;
					if (this.ShouldApplyCarouselBehaviour)
					{
						if (num2 < 0)
						{
							int num3 = base.NumItems + num2;
							flag = (flag || num3 < i);
						}
						else if (num > base.NumItems)
						{
							int num4 = num - base.NumItems;
							flag = (flag || i < num4);
						}
					}
					listItem.OnScreen = flag;
				}
			}
		}
	}

	public override void AddItem(ListItem zItem)
	{
		base.AddItem(zItem);
		ListItemProxy listItemProxy = zItem as ListItemProxy;
		if (listItemProxy == null)
		{
			zItem.Tap += new TapEventHandler(this.OnClickItem);
		}
		else
		{
			ListItemProxy expr_32 = listItemProxy;
			expr_32.OnInstanceCreated = (Action<ListItem>)Delegate.Combine(expr_32.OnInstanceCreated, new Action<ListItem>(this.OnProxyClick));
			ListItemProxy expr_54 = listItemProxy;
			expr_54.OnInstanceShutdown = (Action<ListItem>)Delegate.Combine(expr_54.OnInstanceShutdown, new Action<ListItem>(this.OnProxyShutdown));
		}
	}

	public override void RemoveItem(ListItem zItem)
	{
		ListItemProxy listItemProxy = zItem as ListItemProxy;
		if (listItemProxy == null)
		{
			zItem.Tap -= new TapEventHandler(this.OnClickItem);
		}
		else
		{
			if (listItemProxy.Instance != null)
			{
				listItemProxy.Instance.Tap -= new TapEventHandler(this.OnClickItem);
			}
			ListItemProxy expr_53 = listItemProxy;
			expr_53.OnInstanceCreated = (Action<ListItem>)Delegate.Remove(expr_53.OnInstanceCreated, new Action<ListItem>(this.OnProxyClick));
			ListItemProxy expr_75 = listItemProxy;
			expr_75.OnInstanceShutdown = (Action<ListItem>)Delegate.Remove(expr_75.OnInstanceShutdown, new Action<ListItem>(this.OnProxyShutdown));
		}
		base.RemoveItem(zItem);
	}

	public override void Clear()
	{
		base.Clear();
		this._selectedIndex = 0;
	}

	private void TriggerSelectionChangeEvent(ListItem zItem)
	{
		if (this.OnSelectionChangeEvent != null)
		{
			this.OnSelectionChangeEvent(zItem);
		}
	}

	private void TriggerOnItemPressedEvent(ListItem zItem)
	{
		if (this.OnItemPressedEvent != null)
		{
			this.OnItemPressedEvent(zItem);
		}
	}

	private void CheckForSelectionChange(GenericTouch zTouch)
	{
		float num = HorizontalList.TouchUnitsToWorldUnits(zTouch.DeltaPosition.x) / base.ItemWidth;
		float num2 = this._dragFractionToNextIcon + num;
		bool flag = true;
		if (num2 >= 1f && this._trackerFlag)
		{
			this._trackerFlag = false;
			num2 -= 1f;
		}
		else if (num2 <= -1f && this._trackerFlag)
		{
			this._trackerFlag = false;
			num2 += 1f;
		}
		if (num2 > 0.51f && !this._trackerFlag)
		{
			if (!this.ShouldApplyCarouselBehaviour && this.SelectedIndex == 0)
			{
				flag = false;
			}
			else
			{
				this.SelectedIndex--;
				this._trackerFlag = true;
			}
		}
		else if (num2 < -0.51f && !this._trackerFlag)
		{
			if (!this.ShouldApplyCarouselBehaviour && this.SelectedIndex == base.NumItems - 1)
			{
				flag = false;
			}
			else
			{
				this.SelectedIndex++;
				this._trackerFlag = true;
			}
		}
		else if (num2 >= -0.51f && num2 < 0f && this._trackerFlag)
		{
			this.SelectedIndex--;
			this._trackerFlag = false;
		}
		else if (num2 <= 0.51f && num2 > 0f && this._trackerFlag)
		{
			this.SelectedIndex++;
			this._trackerFlag = false;
		}
		if (flag)
		{
			this._dragFractionToNextIcon = num2;
		}
	}

	private void OnFlick(GenericTouch zTouch)
	{
		if (base.CurrentState == BaseRuntimeControl.State.Disabled || base.CurrentState == BaseRuntimeControl.State.Hidden || !this._listenForGestures)
		{
			return;
		}
		if (!base.Contains(zTouch.Position))
		{
			return;
		}
		if (this._dragFlickTimer != 0f)
		{
			return;
		}
		if (zTouch.AverageEndingVelocity.x > 0f)
		{
			this.SelectedIndex--;
		}
		else if (zTouch.AverageEndingVelocity.x < 0f)
		{
			this.SelectedIndex++;
		}
	}

	private void OnDragStart(GenericTouch zTouch)
	{
		if (base.CurrentState == BaseRuntimeControl.State.Disabled || base.CurrentState == BaseRuntimeControl.State.Hidden || !this._listenForGestures)
		{
			return;
		}
		if (!base.Contains(zTouch.Position))
		{
			return;
		}
		this._isDragging = true;
		this._dragFractionToNextIcon = 0f;
		this._trackerFlag = false;
		this.CheckForSelectionChange(zTouch);
	}

	private void OnDragUpdate(GenericTouch zTouch)
	{
		if (!this._isDragging)
		{
			return;
		}
		this.CheckForSelectionChange(zTouch);
	}

	private void OnDragEnd(GenericTouch zTouch)
	{
		if (!this._isDragging)
		{
			return;
		}
		if (this.SelectedIndex >= this.items.Count || this.SelectedIndex < 0)
		{
			return;
		}
		this._springyPosition = this.items[this.SelectedIndex].transform.localPosition.x;
		this._dragFractionToNextIcon = 0f;
		this._trackerFlag = false;
		this._isDragging = false;
		this.CheckForSelectionChange(zTouch);
	}

	private float TargetWorldPositionX(int i, bool zCarousel)
	{
		int num = i - this.SelectedIndex;
		if (this.ShouldApplyCarouselBehaviour)
		{
			if (num < -this.NumIconsOnSideOfSelectedItem)
			{
				num += base.NumItems;
			}
			if (num > base.NumItems - this.NumIconsOnScreen + this.NumIconsOnSideOfSelectedItem)
			{
				num -= base.NumItems;
			}
		}
		return (float)num * base.ItemWidth;
	}

	private void SpringUpdate()
	{
		if (this._isDragging)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		float num = this.TargetWorldPositionX(this.SelectedIndex, this.ShouldApplyCarouselBehaviour);
		if (this._springyPosition != num)
		{
			float num2 = this._springyPosition - num;
			float num3 = -5000f * num2 - 800f * this._velocity;
			float num4 = num3 / 50f;
			this._velocity += num4 * deltaTime;
			this._springyPosition += this._velocity * deltaTime;
			float num5 = Math.Abs(this._springyPosition - num);
			if (num5 < 0.01f)
			{
				this._springyPosition = num;
				this._velocity = 0f;
			}
		}
	}

	private float ClampDragFraction()
	{
		float num = this._dragFractionToNextIcon;
		if (num > 0.51f)
		{
			num -= 1f;
		}
		if (num < -0.51f)
		{
			num += 1f;
		}
		return num * base.ItemWidth;
	}

	private void OnClickItem(ListItem zItem)
	{
		if (base.CurrentState == BaseRuntimeControl.State.Disabled || base.CurrentState == BaseRuntimeControl.State.Hidden)
		{
			return;
		}
		if (this._isDragging)
		{
			return;
		}
		this.TriggerOnItemPressedEvent(zItem);
		if (base.NumItems == 0)
		{
			return;
		}
		int num = this.items.FindIndex((ListItem item) => item == zItem);
		if (num >= 0)
		{
			this.SelectedIndex = num;
		}
	}

	private void OnClickItemProxy(ListItem zItem)
	{
		ListItem zItem2 = this.items.Find(delegate(ListItem item)
		{
			ListItemProxy listItemProxy = item as ListItemProxy;
			return listItemProxy != null && listItemProxy.Instance == zItem;
		});
		this.OnClickItem(zItem2);
	}

	private void OnProxyClick(ListItem zItem)
	{
		zItem.Tap += new TapEventHandler(this.OnClickItemProxy);
	}

	private void OnProxyShutdown(ListItem zItem)
	{
		zItem.Tap -= new TapEventHandler(this.OnClickItemProxy);
	}

	public static float TouchUnitsToWorldUnits(float zValue)
	{
	    //return zValue / (float)BaseDevice.ActiveDevice.GetScreenWidth() * GUICamera.Instance.ScreenWidth;
	    return 0;
	}

    [Conditional("UNITY_EDITOR")]
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.5f, 0f);
		foreach (ListItem current in this.items)
		{
			Gizmos.DrawWireCube(current.gameObject.transform.position, new Vector3(base.ItemWidth, this.Height, 0f));
		}
	}
}
