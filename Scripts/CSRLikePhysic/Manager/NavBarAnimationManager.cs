using System;
using System.Collections.Generic;
using UnityEngine;

public class NavBarAnimationManager : MonoBehaviour
{
	private enum AnimState
	{
		SHOWING,
		SHOWN,
		HIDDEN,
		HIDING
	}

	private const float AnimDuration = 0.3f;

	public static NavBarAnimationManager Instance;

	private NavBarAnimationManager.AnimState CurrentAnimState = NavBarAnimationManager.AnimState.HIDDEN;

	private float AnimTimePos;

	private Vector3 HiddenOffset = new Vector3(0f, 0.7f, 0f);

	private List<NavBarSubscriber> subscribers = new List<NavBarSubscriber>();

	private float lastInterp = -1f;

	private void Awake()
	{
		if (NavBarAnimationManager.Instance != null)
		{
			return;
		}
		NavBarAnimationManager.Instance = this;
	}

	public void Subscribe(GameObject obj)
	{
		this.subscribers.Add(new NavBarSubscriber(obj));
		this.lastInterp = -1f;
	}

	private void OnDestroy()
	{
		this.subscribers = null;
	}

	private void UpdatePositions(float interp, bool force = false)
	{
		if (this.lastInterp == interp && !force)
		{
			return;
		}
		Vector3 move = interp * this.HiddenOffset;
		this.subscribers.ForEach(delegate(NavBarSubscriber q)
		{
			q.obj.transform.localPosition = q.origin + move;
		});
		this.lastInterp = interp;
	}

	private void Update()
	{
		switch (this.CurrentAnimState)
		{
		case NavBarAnimationManager.AnimState.SHOWING:
			this.AnimTimePos -= Time.deltaTime;
			if (this.AnimTimePos <= 0f)
			{
				this.AnimTimePos = 0f;
				this.CurrentAnimState = NavBarAnimationManager.AnimState.SHOWN;
                //CommonUI.Instance.NavBar.CheckBlocker();
			}
			this.UpdatePositions(this.AnimTimePos / 0.3f, false);
			break;
		case NavBarAnimationManager.AnimState.SHOWN:
			this.UpdatePositions(0f, false);
			break;
		case NavBarAnimationManager.AnimState.HIDDEN:
			this.UpdatePositions(1f, false);
			break;
		case NavBarAnimationManager.AnimState.HIDING:
			this.AnimTimePos -= Time.deltaTime;
			if (this.AnimTimePos <= 0f)
			{
				this.AnimTimePos = 0f;
				this.CurrentAnimState = NavBarAnimationManager.AnimState.HIDDEN;
			}
			this.UpdatePositions(1f - this.AnimTimePos / 0.3f, false);
			break;
		}
	}

	public void ShowNow()
	{
		this.CurrentAnimState = NavBarAnimationManager.AnimState.SHOWN;
		this.UpdatePositions(0f, true);
	}

	public void HideNow()
	{
		this.CurrentAnimState = NavBarAnimationManager.AnimState.HIDDEN;
		this.UpdatePositions(1f, true);
	}

	public void ShowAnimate()
	{
		this.AnimTimePos = 0.3f;
		this.CurrentAnimState = NavBarAnimationManager.AnimState.SHOWING;
		this.Update();
	}

	public void HideAnimate()
	{
		this.AnimTimePos = 0.3f;
		this.CurrentAnimState = NavBarAnimationManager.AnimState.HIDING;
	}

	public bool IsStill()
	{
		return this.CurrentAnimState == NavBarAnimationManager.AnimState.HIDDEN || this.CurrentAnimState == NavBarAnimationManager.AnimState.SHOWN;
	}

	public bool IsShowing()
	{
		return this.CurrentAnimState == NavBarAnimationManager.AnimState.SHOWN;
	}

	public float AmountShowing()
	{
		NavBarAnimationManager.AnimState currentAnimState = this.CurrentAnimState;
		if (currentAnimState != NavBarAnimationManager.AnimState.SHOWN)
		{
			return 0f;
		}
		return 1f;
	}
}
