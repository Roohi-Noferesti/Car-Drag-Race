using System;
using System.Collections;
using UnityEngine;

#if UNITY_ANDROID
public class AndroidApplicationManager : ApplicationManager
{
	private static bool isFocused = true;

	private static bool isLocked;

	private static bool isPaused;

    public static event ApplicationEvent_Delegate WillLoseFocusEvent;

	private void Awake()
	{
        AndroidSpecific.Initialise();
        base.StartCoroutine(AndroidDevice.FlickerOnQuitFix());
	}

	private void UpdateAudio()
	{
		if (isLocked || isPaused || !isFocused)
		{
			AudioListener.volume = 0f;
		}
		else if (Math.Abs(AudioListener.volume) < 0.001 && !isLocked)
		{
			base.StartCoroutine(DelayResumeAudio());
		}
	}

	public void ScreenLockMessage(string lockStr)
	{
		if (lockStr == "LOCK")
		{
			isLocked = true;
		}
		else if (lockStr == "UNLOCK")
		{
			isLocked = false;
		}
		this.UpdateAudio();
	}

	private static IEnumerator DelayResumeAudio()
	{
	    yield return new WaitForSeconds(0.5F);
	    if ((!AndroidApplicationManager.isLocked && !AndroidApplicationManager.isPaused) &&
	        AndroidApplicationManager.isFocused)
	    {
	        AudioListener.volume = 1f;
	    }

	}

	private void OnApplicationFocus(bool focus)
	{
		isFocused = focus;
		if (!focus && WillLoseFocusEvent != null)
		{
			WillLoseFocusEvent();
		}
		this.UpdateAudio();
	}

	protected override void ApplicationPaused(bool paused)
	{
		isPaused = paused;
        if (paused)
        {
            CleanDownManager.Instance.OnTaskSwitch();
        }
		this.UpdateAudio();
	}


}
#endif
