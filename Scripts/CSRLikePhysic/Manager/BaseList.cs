using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class BaseList : BaseRuntimeControl
{
	public delegate void ListEventDelegate(ListItem zItem);

	public float DesiredMenuItemWidth = 0.9f;

	public float Height = 0.61f;

	private float _cachedWidth = -1f;

	protected List<ListItem> items = new List<ListItem>();

    public event Action AllItemsAdded;

	public float ItemWidth
	{
		get
		{
			if (this._cachedWidth == -1f)
			{
				float num = this.DesiredMenuItemWidth;
				if (num == -1f)
				{
                    //num = GUICamera.Instance.ScreenWidth / 5f;
				}
				int num2 = (int)Mathf.Round(num * 100f);
				int num3 = num2 % 2;
				num2 -= num3;
				float cachedWidth = (float)num2 / 100f;
				this._cachedWidth = cachedWidth;
			}
			return this._cachedWidth;
		}
	}

	public int MaxIndex
	{
		get
		{
			return this.items.Count - 1;
		}
	}

	public int NumItems
	{
		get
		{
			return this.items.Count;
		}
	}

	protected virtual void FixedUpdate()
	{
	}

	public virtual void MarkAllItemsAdded()
	{
		if (this.AllItemsAdded != null)
		{
			this.AllItemsAdded();
		}
		this.Update();
		this.FixedUpdate();
	}

	public override void OnActivate()
	{
		foreach (ListItem current in this.items)
		{
			current.OnActivate();
		}
		this.AdjustToBe(base.CurrentState);
	}

	protected override void AdjustToBe(BaseRuntimeControl.State zState)
	{
		foreach (ListItem current in this.items)
		{
			current.AdjustToBe(zState);
		}
	}

	public ListItem GetItem(int index)
	{
		return this.items[index];
	}

	public void DisableAllItems(bool zDisable)
	{
		foreach (ListItem current in this.items)
		{
			current.AdjustToBe((!zDisable) ? BaseRuntimeControl.State.Active : BaseRuntimeControl.State.Disabled);
		}
	}

	public virtual void AddItem(ListItem zItem)
	{
		zItem.transform.parent = base.transform;
		this.items.Add(zItem);
		zItem.OnActivate();
	}

	public virtual void RemoveItem(ListItem zItem)
	{
		zItem.Shutdown();
		UICacheManager.Instance.ReleaseItem(zItem.gameObject);
		this.items.Remove(zItem);
	}

	public virtual void Clear()
	{
		while (this.items.Count > 0)
		{
			this.RemoveItem(this.items[0]);
		}
		this.items.Clear();
	}
}
