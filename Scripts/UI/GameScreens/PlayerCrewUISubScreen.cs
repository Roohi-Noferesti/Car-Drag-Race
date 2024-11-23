using System;
using UnityEngine;

public class PlayerCrewUISubScreen : MonoBehaviour
{
	public virtual void OnActivate()
	{
	}

	public virtual void OnDeactivate()
	{
	}

	public virtual bool RequestBackup()
	{
		return false;
	}

	public virtual void OnFacebookStateChanged()
	{
	}

	public virtual void GCPlayerStatusChanged()
	{
	}

	public virtual void OnApplicationDidBackground()
	{
	}

	public virtual void OnApplicationWillForeground()
	{
	}

	public virtual void LogMetricEvent(string eventType)
	{
	}
}
