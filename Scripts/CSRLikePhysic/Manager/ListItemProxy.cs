using System;
using UnityEngine;

public class ListItemProxy : ListItem
{
	private Func<ListItem> _createFn;

	private ListItem _instance;

	public Action<ListItem> OnInstanceCreated;

	public Action<ListItem> OnInstanceShutdown;

	public ListItem Instance
	{
		get
		{
			return this._instance;
		}
	}

	public static ListItemProxy Create(Func<ListItem> createFn, float height)
	{
		GameObject gameObject = new GameObject();
		ListItemProxy listItemProxy = gameObject.AddComponent(typeof(ListItemProxy)) as ListItemProxy;
		listItemProxy._createFn = createFn;
		listItemProxy.ListItemHeight = height;
		return listItemProxy;
	}

	public override void GreyOutThisItem(bool disable)
	{
		this._thisIsGreyedOut = disable;
		if (this._instance)
		{
			this._instance.GreyOutThisItem(disable);
		}
	}

	protected override void Update()
	{
		if (this._instance)
		{
			this._instance.transform.position = base.transform.position;
		}
	}

	public override void AdjustToBe(BaseRuntimeControl.State zState)
	{
		if (this._instance)
		{
			this._instance.AdjustToBe(zState);
		}
		base.AdjustToBe(zState);
	}

	protected override void OnDestroy()
	{
		this._instance = null;
		base.OnDestroy();
	}

	public override void OnMoveOffScreen()
	{
		if (this.OnInstanceShutdown != null)
		{
			this.OnInstanceShutdown(this._instance);
		}
		this.Shutdown();
		if (this._instance != null)
		{
			this._instance.OnMoveOffScreen();
		}
	}

	public override void OnMoveOnScreen()
	{
		if (this._instance == null && this._createFn != null)
		{
			this._instance = this._createFn();
			this._instance.transform.parent = base.transform.parent;
			this._instance.GreyOutThisItem(this._thisIsGreyedOut);
			if (this.OnInstanceCreated != null)
			{
				this.OnInstanceCreated(this._instance);
			}
		}
		this._instance.OnMoveOnScreen();
	}

	public override void OnShowFlowConditionBubbleMessage(FlowConditionBase condition)
	{
		if (this._instance != null)
		{
			this._instance.OnShowFlowConditionBubbleMessage(condition);
		}
	}

	protected override void Hide()
	{
		base.gameObject.SetActive(false);
		if (this._instance != null)
		{
			this._instance.gameObject.SetActive(false);
		}
	}

	public override void Shutdown()
	{
		if (this._instance != null)
		{
			this._instance.Shutdown();
			UICacheManager.Instance.ReleaseItem(this._instance.gameObject);
			this._instance = null;
		}
	}

	public override void OnActivate()
	{
		if (this._instance != null)
		{
			this._instance.OnActivate();
		}
		this.Update();
	}
}
