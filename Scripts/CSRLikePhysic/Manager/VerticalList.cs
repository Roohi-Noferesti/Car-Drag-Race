using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public sealed class VerticalList : BaseList
{
	[Serializable]
	public class PositionManager
	{
		public const float Damping = 1000f;

		public const float Mass = 50f;

		public const float Stiffness = 9000f;

		public const float ListEndDragRatio = 0.4f;

		private VerticalList _parent;

		[SerializeField]
		private float _velocity;

		[SerializeField]
		private float _targetOffset;

		[SerializeField]
		private float _offset;

		[SerializeField]
		private float _topOffsetLimit;

		[SerializeField]
		private float _bottomOffsetLimit;

		[SerializeField]
		private float _topTargetLimit;

		[SerializeField]
		private float _bottomTargetLimit;

		public float Offset
		{
			get
			{
				return this._offset;
			}
			private set
			{
				this._offset = Mathf.Clamp(value, this._topOffsetLimit, this._bottomOffsetLimit);
			}
		}

		public float Velocity
		{
			get
			{
				return this._velocity;
			}
			private set
			{
				this._velocity = value;
			}
		}

		public float TargetOffset
		{
			get
			{
				return this._targetOffset;
			}
			set
			{
				float y = this._parent.Size.y;
				float min = this._topOffsetLimit + y * 0.4f;
				float max = this._bottomOffsetLimit - y * 0.4f;
				this._targetOffset = Mathf.Clamp(value, min, max);
			}
		}

		public PositionManager(VerticalList parent)
		{
			this._parent = parent;
			if (this._parent != null)
			{
				this._parent.AllItemsAdded += new Action(this.SetLimits);
			}
		}

		public void SetLimits()
		{
			float y = this._parent.Size.y;
			float totalHeightOfAllItems = this._parent.TotalHeightOfAllItems;
			this._topTargetLimit = 0f;
			if (totalHeightOfAllItems > y)
			{
				this._bottomTargetLimit = totalHeightOfAllItems - y;
			}
			else
			{
				this._bottomTargetLimit = 0f;
			}
			float num = y * 0.4f;
			this._topOffsetLimit = this._topTargetLimit - num;
			this._bottomOffsetLimit = this._bottomTargetLimit + num;
		}

		public void Reset()
		{
			this._offset = 0f;
			this._targetOffset = 0f;
			this._velocity = 0f;
		}

		public void ForceOffset(float offset)
		{
			this.Velocity = 0f;
			this.Offset = offset;
			this.TargetOffset = this.Offset;
		}

		public void UpdateOffset()
		{
			if (this.Offset != this.TargetOffset)
			{
				float num = this.Offset - this.TargetOffset;
				float num2 = -(9000f * num) - 1000f * this.Velocity;
				float num3 = num2 / 50f;
				this.Velocity += num3 * Time.fixedDeltaTime;
				this.Offset += this.Velocity * Time.fixedDeltaTime;
				float num4 = Math.Abs(this.Offset - this.TargetOffset);
				if (num4 < 0.01f)
				{
					this.Offset = this.TargetOffset;
				}
			}
		}
	}

	private const float ScrollbarWidth = 0.03f;

	private const float ScrollbarWaitBeforeFadeTime = 0.6f;

	private const float ScrollbarFadeTime = 1.2f;

	private const float ScrollbarIndentX = 0.05f;

	private const float ScrollbarFadeOutTime = 1.5f;

	public Color ScrollbarColour = Color.gray;

	public Image ScrollTop;

	public Image ScrollMid;

	public Image ScrollBot;

	private float ScrollBarHeight;

	public Image BottomFade;

	public Image TopFade;

	public Texture2D DividerTexture;

	public float TopY = 2f;

	public float BottomY = -2f;

	public float DividerHeight = 0.03f;

	private float _scrollbarWaitTimer;

	private float _scrollbarFadeTimer;

	private bool _scrollbarWaiting;

	private bool _scrollbarFading;

	public bool AlwayShowScrollBar = true;

	public bool AllowScrollingOfShortLists = true;

	public bool AlignToListTop;

	public Material GUIMaterial;

	//private float LastBottomFadeValue = -1f;

	//private float LastTopFadeValue = -1f;

    private List<Image> _sprDividers = new List<Image>();

	private bool _dragging;

	private bool _listenForGestures = true;

	private VerticalList.PositionManager _positionManager;

	private float RightX = 1f;

	private float LeftX = -1f;

    public event BaseList.ListEventDelegate OnItemPressedEvent;

	public float Offset
	{
		set
		{
			this._positionManager.ForceOffset(value);
		}
	}

	public float TargetOffset
	{
		set
		{
			this._positionManager.TargetOffset = value;
		}
	}

	public override Vector2 Size
	{
		get
		{
			return new Vector2(this.RightX - this.LeftX, this.TopY - this.BottomY);
		}
	}

	public float TotalHeightOfAllItems
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < this.items.Count; i++)
			{
				num += this.getHeightOfItemAtIndex(i);
			}
			return num + (float)(base.NumItems - 1) * this.DividerHeight;
		}
	}

	public void SetXLimits(float leftX, float rightX)
	{
		this.LeftX = leftX;
		this.RightX = rightX;
	}

	public override void MarkAllItemsAdded()
	{
		base.MarkAllItemsAdded();
		this._scrollbarWaiting = false;
		this._scrollbarFading = false;
	}

	protected override void Update()
	{
		this.UpdateScrollbar();
		this.UpdateBottomFade();
		this.UpdateTopFade();
		float num = 0f;
		for (int i = 0; i < this.items.Count; i++)
		{
			float heightOfItemAtIndex = this.getHeightOfItemAtIndex(i);
			float num2 = this.Size.y / 2f;
			num2 -= heightOfItemAtIndex / 2f;
			if (this.AlignToListTop)
			{
				num2 = this.Size.y / 2f;
			}
			num2 -= (float)i * this.DividerHeight + num;
			num2 += this._positionManager.Offset;
			float x = this.items[i].transform.localPosition.x;
			this.items[i].transform.localPosition = new Vector3(x, num2, 0f);
			float num3 = (num2 - heightOfItemAtIndex) * base.transform.localScale.y;
			float num4 = num2 * base.transform.localScale.y;
			float num5 = heightOfItemAtIndex * 1.5f;
			this.items[i].OnScreen = (num3 + num5 > -this.Size.y / 2f && num4 - num5 < this.Size.y / 2f);
			num += heightOfItemAtIndex;
		}
		num = 0f;
		for (int j = 0; j < this._sprDividers.Count; j++)
		{
			float heightOfItemAtIndex2 = this.getHeightOfItemAtIndex(j);
			float num6 = this.Size.y / 2f;
			num6 -= heightOfItemAtIndex2 + this.DividerHeight / 2f;
			num6 -= (float)j * this.DividerHeight + num;
			num6 += this._positionManager.Offset;
			this._sprDividers[j].transform.localPosition = new Vector3(0f, num6, 0f);
			num += heightOfItemAtIndex2;
		}
	}

	protected override void FixedUpdate()
	{
		if (!this._dragging)
		{
			this._positionManager.UpdateOffset();
		}
	}

	public override void OnActivate()
	{
        //this.LeftX = -GUICamera.Instance.ScreenWidth / 2f;
        //this.RightX = GUICamera.Instance.ScreenWidth / 2f;
		base.OnActivate();
		this._positionManager = new VerticalList.PositionManager(this);
		this._dragging = false;
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		GestureEventSystem.Instance.Flick += new GestureEventSystem.GestureEventHandler(this.OnFlick);
		GestureEventSystem.Instance.Drag += new GestureEventSystem.GestureEventHandler(this.OnDragStart);
		GestureEventSystem.Instance.DragUpdate += new GestureEventSystem.GestureEventHandler(this.OnDragUpdate);
		GestureEventSystem.Instance.DragComplete += new GestureEventSystem.GestureEventHandler(this.OnDragEnd);
	}

	public override void OnDeactivate()
	{
		this.Clear();
		GestureEventSystem.Instance.Drag -= new GestureEventSystem.GestureEventHandler(this.OnDragStart);
		GestureEventSystem.Instance.DragUpdate -= new GestureEventSystem.GestureEventHandler(this.OnDragUpdate);
		GestureEventSystem.Instance.DragComplete -= new GestureEventSystem.GestureEventHandler(this.OnDragEnd);
		GestureEventSystem.Instance.Flick -= new GestureEventSystem.GestureEventHandler(this.OnFlick);
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
		if (!this.AllowScrollingOfShortLists && this.TotalHeightOfAllItems < this.Size.y)
		{
			return;
		}
		this._dragging = true;
	}

	public bool IsScrollingEnabled()
	{
		return this.AllowScrollingOfShortLists || this.TotalHeightOfAllItems >= this.Size.y;
	}

	private void OnDragUpdate(GenericTouch zTouch)
	{
		if (!this._dragging)
		{
			return;
		}
		//float num = 1f;
		if (this._positionManager.Offset < 0f || this._positionManager.Offset > this.TotalHeightOfAllItems + this.Size.y)
		{
			//num = 0.4f;
		}
	    float num2 = 0;//zTouch.DeltaPosition.y / (float)BaseDevice.ActiveDevice.GetScreenHeight() * GUICamera.Instance.ScreenHeight * num;
		float offset = this._positionManager.Offset + num2;
		this._positionManager.ForceOffset(offset);
	}

	private float getHeightOfItemAtIndex(int index)
	{
		return this.items[index].ListItemHeight;
	}

	public float GetDistanceToItemAtIndex(int index)
	{
		float num = 0f;
		for (int i = 0; i < Math.Min(this.items.Count, index); i++)
		{
			num += this.items[index].ListItemHeight;
		}
		return num;
	}

	private void OnDragEnd(GenericTouch zTouch)
	{
		if (!this._dragging)
		{
			return;
		}
		this._dragging = false;
		float num = zTouch.AverageEndingVelocity.y * Time.fixedDeltaTime * 0.04f;
		this._positionManager.TargetOffset = this._positionManager.TargetOffset + num;
	}

	private void OnFlick(GenericTouch zTouch)
	{
		if (base.CurrentState == BaseRuntimeControl.State.Disabled || base.CurrentState == BaseRuntimeControl.State.Hidden || !this._listenForGestures)
		{
			return;
		}
		if (!this._dragging)
		{
			return;
		}
		if (!base.Contains(zTouch.Position))
		{
			return;
		}
		float num = zTouch.AverageEndingVelocity.y * Time.fixedDeltaTime * 0.12f;
		this._positionManager.TargetOffset = this._positionManager.TargetOffset + num;
	}

	public override void AddItem(ListItem zItem)
	{
		base.AddItem(zItem);
		zItem.Tap += new TapEventHandler(this.OnClickItem);
		zItem.gameObject.transform.position = base.transform.position;
		float height;
		if (this.TotalHeightOfAllItems < this.Size.y)
		{
			height = this.Size.y;
		}
		else
		{
			height = this.Size.y / this.TotalHeightOfAllItems * this.Size.y;
		}
		this.ScrollBarSetHeight(height);
		if (base.NumItems <= 1 || this.DividerHeight == 0f || this.DividerTexture == null)
		{
			return;
		}
        Image sprite = new GameObject("Divider " + base.NumItems.ToString()).AddComponent<Image>();
		sprite.gameObject.layer = LayerMask.NameToLayer("GUI");
		sprite.gameObject.transform.parent = base.transform;
		sprite.gameObject.transform.position = base.transform.position + new Vector3(0f, 0f, -0.2f);
        //sprite.renderer.material = this.GUIMaterial;
        //sprite.renderer.material.SetTexture("_MainTex", this.DividerTexture);
        //sprite.Setup(this.Size.x, this.DividerHeight, new Vector2(0f, (float)(this.DividerTexture.height - 1)), new Vector2((float)this.DividerTexture.width, (float)this.DividerTexture.height));
		sprite.gameObject.transform.position = base.transform.position;
		this._sprDividers.Add(sprite);
	}

	private void OnClickItem(ListItem zItem)
	{
		if (base.CurrentState == BaseRuntimeControl.State.Disabled || base.CurrentState == BaseRuntimeControl.State.Hidden)
		{
			return;
		}
		if (this._dragging)
		{
			return;
		}
		if (this.OnItemPressedEvent != null)
		{
			this.OnItemPressedEvent(zItem);
		}
	}

	public override void RemoveItem(ListItem zItem)
	{
		if (this._sprDividers.Count > 0)
		{
			UnityEngine.Object.Destroy(this._sprDividers[0].gameObject);
			this._sprDividers.RemoveAt(0);
		}
		zItem.Tap -= new TapEventHandler(this.OnClickItem);
		base.RemoveItem(zItem);
	}

	private void UpdateScrollbar()
	{
		if (this.ScrollBot == null || this.ScrollMid == null || this.ScrollTop == null)
		{
			return;
		}
		if (!this.AllowScrollingOfShortLists && this.TotalHeightOfAllItems < this.Size.y)
		{
			this.ScrollBarSetAlpha(0f);
			return;
		}
		if (!this.AlwayShowScrollBar)
		{
			if (this._dragging)
			{
				this._scrollbarWaitTimer = 0.6f;
				this._scrollbarWaiting = true;
				this._scrollbarFading = false;
			}
			float alpha = 0f;
			if (this._scrollbarWaiting)
			{
				alpha = 1f;
				this._scrollbarWaitTimer -= Time.deltaTime;
				if (this._scrollbarWaitTimer <= 0f)
				{
					this._scrollbarWaiting = false;
					this._scrollbarFadeTimer = 1.5f;
					this._scrollbarFading = true;
				}
			}
			if (this._scrollbarFading)
			{
				this._scrollbarFadeTimer -= Time.deltaTime;
				if (this._scrollbarFadeTimer <= 0f)
				{
					this._scrollbarFading = false;
				}
				alpha = this._scrollbarFadeTimer / 1.5f;
			}
			this.ScrollBarSetAlpha(alpha);
		}
		float num = this._positionManager.Offset / (this.TotalHeightOfAllItems - this.Size.y);
		num = Mathf.Clamp01(num);
		float xPos = this.RightX - 0.05f;
	    float yPos = 0;//-(this.Size.y / 2f - this.ScrollBarHeight - this.ScrollTop.height) + (1f - num) * (this.Size.y - this.ScrollBarHeight);
		this.ScrollBarSetPosition(xPos, yPos);
	}

	public override void Clear()
	{
		base.Clear();
		this._dragging = false;
		this._positionManager.Reset();
	}

	private void ScrollBarSetHeight(float height)
	{
        //float num = Mathf.Max(0f, height - this.ScrollTop.height + this.ScrollBot.height);
        //this.ScrollMid.SetSize(this.ScrollMid.width, num);
        //this.ScrollMid.transform.localPosition = new Vector3(0f, -this.ScrollTop.height, 0f);
        //this.ScrollBot.transform.localPosition = new Vector3(0f, -this.ScrollTop.height - num, 0f);
        //this.ScrollBarHeight = height;
	}

	private void ScrollBarSetAlpha(float alpha)
	{
		this.ScrollTop.gameObject.GetComponent<Renderer>().material.SetColor("_Tint", new Color(1f, 1f, 1f, alpha));
		this.ScrollBot.gameObject.GetComponent<Renderer>().material.SetColor("_Tint", new Color(1f, 1f, 1f, alpha));
		this.ScrollMid.gameObject.GetComponent<Renderer>().material.SetColor("_Tint", new Color(1f, 1f, 1f, alpha));
	}

	private void ScrollBarSetPosition(float xPos, float yPos)
	{
        //this.ScrollTop.transform.localPosition = new Vector3(xPos, yPos, -0.3f);
        //this.ScrollMid.transform.localPosition = new Vector3(xPos, yPos - this.ScrollTop.width, -0.3f);
        //this.ScrollBot.transform.localPosition = new Vector3(xPos, yPos - this.ScrollTop.width - this.ScrollMid.height, -0.3f);
	}

	private void UpdateBottomFade()
	{
        //if (this.BottomFade == null)
        //{
        //    return;
        //}
        //float num = this.TotalHeightOfAllItems - this._positionManager.Offset - this.Size.y;
        //float num2 = Mathf.Clamp01(num / this.BottomFade.height);
        //if (num2 == this.LastBottomFadeValue)
        //{
        //    return;
        //}
        //this.LastBottomFadeValue = num2;
        //this.BottomFade.gameObject.GetComponent<Renderer>().material.SetColor("_Tint", new Color(1f, 1f, 1f, num2));
	}

	private void UpdateTopFade()
	{
        //if (this.TopFade == null)
        //{
        //    return;
        //}
        //float offset = this._positionManager.Offset;
        //float num = Mathf.Clamp01(offset / Mathf.Abs(this.TopFade.height));
        //if (num == this.LastTopFadeValue)
        //{
        //    return;
        //}
        //this.LastTopFadeValue = num;
        //this.TopFade.gameObject.GetComponent<Renderer>().material.SetColor("_Tint", new Color(1f, 1f, 1f, num));
	}

	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmos()
	{
        //Gizmos.color = Color.red;
        //Vector3 position = base.gameObject.transform.position;
        //if (GUICamera.Instance != null)
        //{
        //    Gizmos.DrawWireCube(position, new Vector3(this.Size.x, this.Size.y, 0f));
        //}
	}
}
