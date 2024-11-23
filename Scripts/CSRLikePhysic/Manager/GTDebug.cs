using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static GTLogConfiguration;
using Debug = UnityEngine.Debug;

public static class GTDebug
{
	public enum LogType
	{
		Debug,
		Warning,
		Error
	}

	private static HashSet<GTLogChannel> activeChannels = new HashSet<GTLogChannel>
	{
		GTLogChannel.SMARTBuild,
		GTLogChannel.ReplayKit
	};

	private static HashSet<GTLogChannel> activeLogToFileChannels = new HashSet<GTLogChannel>();

    private static uint logCounter;

    private static float? stopWatchTime;

	public static HashSet<GTLogChannel> ActiveChannels
	{
		get
		{
			return activeChannels;
		}
	}

	public static HashSet<GTLogChannel> ActiveLogToFileChannels
	{
		get
		{
			return activeLogToFileChannels;
		}
	}

	private static IEnumerable<string> ChunksOf(string str, int maxChunkSize)
	{
	    int i = 0;
	    while (i<str.Length)
	    {
	        yield return str.Substring(i, Mathf.Min(maxChunkSize, str.Length - i));
	        i += maxChunkSize;
	    }
	}

    [Conditional("GT_DEBUG_LOGGING")]
	private static void Log(GTLogChannel channel, LogType type, string logString)
	{
        if (type == LogType.Debug && !ChannelIsActive(channel))
        {
            return;
        }
        string str = string.Format("f{0}   {1}Log: {2}", logCounter++, channel, logString);
        foreach (string current in ChunksOf(str, 8192))
        {
            switch (type)
            {
                case LogType.Debug:
                    Debug.Log(current);
                    continue;
                case LogType.Warning:
                    Debug.LogWarning(current);
                    continue;
                case LogType.Error:
                    Debug.LogError(current);
                    continue;
            }
        }
        if (!ChannelIsLoggingToFile(channel))
        {
        }
	}




	[Conditional("GT_DEBUG_LOGGING")]
	public static void Log(GTLogChannel channel, string logstring)
	{
        Log(channel, LogType.Debug, logstring);
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void LogIf(bool condition, GTLogChannel channel, string logstring)
	{
		if (condition)
		{
            Log(channel, LogType.Debug, logstring);
		}
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void LogWarning(GTLogChannel channel, string logstring)
	{
        Log(channel, LogType.Warning, logstring);
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void LogWarnIf(bool condition, GTLogChannel channel, string logstring)
	{
		if (condition)
		{
            Log(channel, LogType.Warning, logstring);
		}
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void LogError(GTLogChannel channel, string logstring)
	{
        Log(channel, LogType.Error, logstring);
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void LogErrorIf(bool condition, GTLogChannel channel, string logstring)
	{
		if (condition)
		{
            Log(channel, LogType.Error, logstring);
		}
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void Break()
	{
		Debug.Break();
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void ActivateChannel(GTLogChannel channel)
	{
		activeChannels.Add(channel);
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void RemoveChannel(GTLogChannel channel)
	{
		activeChannels.Remove(channel);
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void ActivateChannelLogToFile(GTLogChannel channel)
	{
		activeLogToFileChannels.Add(channel);
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void RemoveChannelLogToFile(GTLogChannel channel)
	{
		activeLogToFileChannels.Remove(channel);
	}

	public static bool ChannelIsActive(GTLogChannel channel)
	{
		return activeChannels.Contains(channel);
	}

	private static bool ChannelIsLoggingToFile(GTLogChannel channel)
	{
		return activeLogToFileChannels.Contains(channel);
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void BeginStopwatch(GTLogChannel channel)
	{
        stopWatchTime = Time.time;
	}

	[Conditional("GT_DEBUG_LOGGING")]
	public static void LogStopwatch(GTLogChannel channel, string logstring)
	{
	    var time = Time.time - stopWatchTime;
	    Log(channel, LogType.Error, logstring + ", in seconds : " + time);
	}

	static GTLogConfiguration config;
	public static void Initialise(GTLogConfiguration configs)
    {
        foreach (var logStatuse in configs.Logs.Where(l => l.Active))
        {
            ActiveChannels.Add(logStatuse.Channel);
        }

		config = configs;

	}
}
