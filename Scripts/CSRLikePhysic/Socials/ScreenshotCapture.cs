using System;
using System.Collections;
using System.IO;
using Fabric;
using KingKodeStudio;
using UnityEngine;

public class ScreenshotCapture : MonoBehaviour
{
	private const float TimeoutTime = 5f;

	private string _BasePath;

	private string _relativeTemporaryOutputPath;

	private bool _currentlyTakingScreenshot;

	private float _timer;

	private Action _callback;

	private bool _waitingForCameraRollCopyFinish;

	private bool _waitingForTweetComposerFinish;

	public static ScreenshotCapture Instance
	{
		get;
		private set;
	}

	public string CurrentFilename
	{
		get;
		private set;
	}

	private void Awake()
	{
		Instance = this;
		_relativeTemporaryOutputPath = Path.Combine("ScreenshotCapture", "Screenshot.png");
		_BasePath = Path.Combine(Application.persistentDataPath, "ScreenshotCapture");
		if (!Directory.Exists(_BasePath))
		{
			Directory.CreateDirectory(_BasePath);
		}
	}

	private void OnDisable()
	{
		_callback = null;
		_timer = 6f;
	}

	public void CaptureAndTweetIfPossible(string message, Action doneCallback)
	{
		if (_currentlyTakingScreenshot)
		{
			if (doneCallback != null)
			{
				doneCallback();
			}
			return;
		}
		_callback = doneCallback;
		_currentlyTakingScreenshot = true;
		_timer = 0f;
		_waitingForCameraRollCopyFinish = true;
		NativeEvents.ApplicationDidFinishCopyingSnapshotToCameraRollEvent += CameraRollCopyCallback;
		NativeEvents.ApplicationDidFailCopyingSnapshotToCameraRollEvent += CameraRollCopyCallbackFail;
		CurrentFilename = Path.Combine(_BasePath, "Screenshot.png");
		if (File.Exists(CurrentFilename))
		{
			File.Delete(CurrentFilename);
		}
		StartCoroutine(StartScreenshotAndWaitTillFinish(message));
	}

	private IEnumerator WP8ScreenShotHack(string fileName)
	{
	    //return new ScreenshotCapture.<WP8ScreenShotHack>c__Iterator24();
	    return null;
	}

    private IEnumerator StartScreenshotAndWaitTillFinish(string message)
    {
        yield return new WaitForSeconds(0.05f);
        EventManager.Instance.PostEvent("Snapshot", EventAction.PlaySound, null, null);
        var path = _relativeTemporaryOutputPath;
#if UNITY_EDITOR
        path = CurrentFilename;
#elif UNITY_ANDROID
        path = "ScreenshotCapture/Screenshot.png";
#else
#endif
        ScreenCapture.CaptureScreenshot(path);
        BasePlatform.ActivePlatform.AddSkipBackupAttributeToItem(_relativeTemporaryOutputPath);

        while (true)
        {
            if (File.Exists(CurrentFilename))
            {
                ReadyToShareImage(message);
                yield break;
            }
            _timer += 0.1f;
            if (_timer >= 5f)
            {
                ScreenshotFailed();
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ReadyToShareImage(string message)
	{
		_waitingForTweetComposerFinish = true;
		NativeEvents.ApplicationDidFinishWithTweetComposerEvent += TweetCallback;
		ApplicationManager.DidBecomeActiveEvent += TweetCallback;
		SocialController.Instance.ShareSnapshot(message);
		BasePlatform.ActivePlatform.CopyImageToCameraRoll(CurrentFilename);
	}

	private void ScreenshotFailed()
	{
		AllDone();
	}

	private void CameraRollCopyCallback()
	{
		_waitingForCameraRollCopyFinish = false;
		ShowCameraRollPopupIfNeeded();
	}

	private void CameraRollCopyCallbackFail()
	{
		_waitingForCameraRollCopyFinish = false;
		AllDone();
	}

	private void TweetCallback()
	{
		if (_waitingForTweetComposerFinish)
		{
			_waitingForTweetComposerFinish = false;
			NativeEvents.ApplicationDidFinishWithTweetComposerEvent -= TweetCallback;
			ApplicationManager.DidBecomeActiveEvent -= TweetCallback;
            ShowCameraRollPopupIfNeeded();
        }
	}

	private void ShowCameraRollPopupIfNeeded()
	{
		if (_waitingForCameraRollCopyFinish || _waitingForTweetComposerFinish)
		{
			return;
		}
		AllDone();
  //      if (ScreenManager.Instance.CurrentScreen == ScreenID.Workshop || ScreenManager.Instance.CurrentScreen == ScreenID.Customise || ScreenManager.Instance.CurrentScreen == ScreenID.LiveryCustomise)
		//{
		//	string bodyText = "TEXT_POPUP_JUST_TAKEN_SNAPSHOT_BODY_OSX";
		//	PopUp popup = new PopUp
		//	{
		//		Title = "TEXT_POPUP_JUST_TAKEN_SNAPSHOT_TITLE",
		//		BodyText = bodyText,
		//		ConfirmText = "TEXT_BUTTON_OK"
		//	};
		//	if (PopUpManager.Instance.CurrentPopUpMatchesID(PopUpID.TwitterDelay))
		//	{
		//		PopUpManager.Instance.KillPopUp();
		//	}
		//	PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		//}
	}

	private void AllDone()
	{
		_currentlyTakingScreenshot = false;
		NativeEvents.ApplicationDidFinishCopyingSnapshotToCameraRollEvent -= CameraRollCopyCallback;
		NativeEvents.ApplicationDidFailCopyingSnapshotToCameraRollEvent -= CameraRollCopyCallbackFail;
		if (_callback != null)
		{
			_callback();
			_callback = null;
		}
	}
}
