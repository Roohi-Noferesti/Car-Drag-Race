using System;
using UnityEngine;

public abstract class BaseRuntimeControl : MonoBehaviour
{
	public enum State
	{
		Active,
		Pressed,
		Disabled,
		Hidden,
		Highlight
	}

	private State _currentState;

	public State CurrentState
	{
		get
		{
			return this._currentState;
		}
		set
		{
			this._currentState = value;
			this.AdjustToBe(this._currentState);
		}
	}

	public virtual Vector2 Center
	{
		get { return this.rectTransform().anchoredPosition; }
	}

	public abstract Vector2 Size
	{
		get;
	}

	public virtual Bounds Bounds
	{
		get
		{
			var center = this.Center;
			var size = this.Size;
			return new Bounds(new Vector3(center.x, center.y, 0f), new Vector3(size.x, size.y, 1f));
		}
	}

	public abstract void OnActivate();

	public virtual void OnDeactivate()
	{
	}

	public virtual void OnDestroy()
	{
	}

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
		this.Update();
	}

	protected abstract void Update();

	protected abstract void AdjustToBe(BaseRuntimeControl.State zState);

	public bool Contains(Vector2 screenPos)
	{
	    //Vector3 point = GUICamera.Instance.ScreenToCameraSpace(new Vector3(screenPos.x, screenPos.y, 0f));
        //return this.Bounds.Contains(point);
	    return false;
	}
}
