using System;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public static event ApplicationEvent_Delegate WillResignActiveEvent;

    public static event ApplicationEvent_Delegate DidBecomeActiveEvent;


    public static event Action<bool> ApplicationFocus;

	private void OnApplicationFocus(bool focusStatus)
	{
		this.ApplicationFocusChange(focusStatus);
	}

	protected virtual void ApplicationFocusChange(bool focusStatus)
	{
		if (ApplicationFocus != null)
		{
			ApplicationFocus(focusStatus);
		}
	}

	protected void WillResignActiveEventInternal()
	{
		if (WillResignActiveEvent != null)
		{
			WillResignActiveEvent();
		}
	}

	protected void DidBecomeActiveEventInternal()
	{
		if (DidBecomeActiveEvent != null)
		{
			DidBecomeActiveEvent();
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!GTSystemOrder.SystemsReady)
		{
			return;
		}
		if (paused)
		{
			if (WillResignActiveEvent != null)
			{
				WillResignActiveEvent();
			}
		}
		else if (DidBecomeActiveEvent != null)
		{
			DidBecomeActiveEvent();
		}
		this.ApplicationPaused(paused);
	}

	protected virtual void ApplicationPaused(bool paused)
	{
	}
}
