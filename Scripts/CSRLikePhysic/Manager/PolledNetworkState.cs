using System;
using System.Diagnostics;

public static class PolledNetworkState
{
	private const int MS_BETWEEN_CALLS = 2000;

	private static Stopwatch _timer;

	private static BasePlatform.eReachability _cachedValue;

    //private static bool _forcedOffline;

    public static event Action JustCameOnline;

    public static event Action JustWentOffline;

	public static bool IsNetworkConnected
	{
		get
		{
			return CachedValue != BasePlatform.eReachability.OFFLINE;
		}
	}

	public static BasePlatform.eReachability CachedValue
	{
		get
		{
			if (_timer.Elapsed.TotalMilliseconds >= 2000.0)
			{
				UpdateCachedValue();
				_timer.Reset();
				_timer.Start();
			}
			return _cachedValue;
		}
	}

	static PolledNetworkState()
	{
		_timer = new Stopwatch();
		_cachedValue = BasePlatform.eReachability.OFFLINE;
        //_forcedOffline = false;
		UpdateCachedValue();
		_timer.Start();
	}

	private static void InvokeJustCameOnlineEvent()
	{
		if (JustCameOnline != null)
		{
			JustCameOnline();
		}
	}

	private static void InvokeJustWentOfflineEvent()
	{
		if (JustWentOffline != null)
		{
			JustWentOffline();
		}
	}

	public static void ForceOffline()
	{
        //_forcedOffline = true;
		BasePlatform.eReachability cachedValue = _cachedValue;
		_cachedValue = BasePlatform.eReachability.OFFLINE;
		if (cachedValue != BasePlatform.eReachability.OFFLINE)
		{
			InvokeJustWentOfflineEvent();
		}
	}

	private static void UpdateCachedValue()
	{
        //if (_forcedOffline)
        //{
        //    return;
        //}
		BasePlatform.eReachability cachedValue = _cachedValue;
		_cachedValue = BasePlatform.ActivePlatform.GetReachability();
		BasePlatform.eReachability cachedValue2 = _cachedValue;
		if (cachedValue == BasePlatform.eReachability.OFFLINE && cachedValue2 != BasePlatform.eReachability.OFFLINE)
		{
			InvokeJustCameOnlineEvent();
		}
		if (cachedValue != BasePlatform.eReachability.OFFLINE && cachedValue2 == BasePlatform.eReachability.OFFLINE)
		{
			InvokeJustWentOfflineEvent();
		}
	}
}
