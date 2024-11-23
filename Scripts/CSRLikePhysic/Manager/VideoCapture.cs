public class VideoCapture
{
	public static bool IsSupportedAndEnabled
	{
		get
		{
		    return false;//GameDatabase.Instance.SocialConfiguration.ReplayKitEnabled && UnityReplayKit.IsSupported() && BaseDevice.ActiveDevice.SupportsVideoCapture();
		}
	}

	public static void StartRecording()
	{
		if (!IsSupportedAndEnabled)// || !UnityReplayKit.TryStartRecording(true))
		{
		}
	}

	public static bool CanStartRecording()
	{
		return IsSupportedAndEnabled;// && UnityReplayKit.IsAvailable();
	}

	public static void StopRecording()
	{
		if (!IsSupportedAndEnabled)// || !UnityReplayKit.IsRecording() || !UnityReplayKit.TryStopRecording())
		{
		}
	}

	public static bool Share()
	{
		return IsSupportedAndEnabled;// && UnityReplayKit.IsPreviewAvailable() && UnityReplayKit.TryDisplayPreviewView();
	}

	public static void ClearRecording()
	{
		if (!IsSupportedAndEnabled)// || UnityReplayKit.IsRecording() || !UnityReplayKit.TryDiscardRecording())
		{
		}
	}
}
