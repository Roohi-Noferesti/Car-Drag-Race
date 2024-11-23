using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class StaticList : BaseList
{
    public event BaseList.ListEventDelegate OnItemPressedEvent;

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
		for (int i = 0; i < this.items.Count; i++)
		{
			GenericListItem genericListItem = this.items[i] as GenericListItem;
			if (genericListItem != null)
			{
				genericListItem.ShowRoundedEdges(true);
			}
		}
	}

	public override void OnDeactivate()
	{
	}

	protected override void Update()
	{
	}

	public override void MarkAllItemsAdded()
	{
		float num = (float)(-(float)base.NumItems) * base.ItemWidth / 2f;
		num += base.ItemWidth / 2f;
		for (int i = 0; i < this.items.Count; i++)
		{
			this.items[i].transform.localPosition = new Vector3(num + (float)i * base.ItemWidth, 0f, 0f);
		}
		base.MarkAllItemsAdded();
	}

	public override void AddItem(ListItem zItem)
	{
		base.AddItem(zItem);
		zItem.Tap += new TapEventHandler(this.OnClickItem);
	}

	public override void RemoveItem(ListItem zItem)
	{
		zItem.Tap -= new TapEventHandler(this.OnClickItem);
		base.RemoveItem(zItem);
	}

	private void OnClickItem(ListItem zItem)
	{
		if (base.CurrentState == BaseRuntimeControl.State.Disabled || base.CurrentState == BaseRuntimeControl.State.Hidden)
		{
			return;
		}
		this.TriggerOnItemPressedEvent(zItem);
	}

	private void TriggerOnItemPressedEvent(ListItem zItem)
	{
		if (this.OnItemPressedEvent != null)
		{
			this.OnItemPressedEvent(zItem);
		}
	}
}
