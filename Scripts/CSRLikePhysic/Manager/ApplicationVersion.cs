using System.Collections.Generic;
using UnityEngine;

public static class ApplicationVersion
{
	public static string Current
	{
		get
		{
			return BasePlatform.ActivePlatform.GetApplicationVersion();
		}
	}

	public static bool IsEqualToCurrent(string testVersion)
	{
		return IsEqual(testVersion, Current);
	}

	private static bool IsEqual(string testVersion, string againstVersion)
	{
		return Compare(testVersion, againstVersion) == 0;
	}

	public static bool IsGreaterThanCurrent(string inVersion)
	{
		return IsGreater(inVersion, Current);
	}

    public static bool IsGreater(string testVersion, string againstVersion)
	{
		return Compare(testVersion, againstVersion) > 0;
	}

	public static bool ValidVersion(string version)
	{
		if (string.IsNullOrEmpty(version))
		{
			return false;
		}
		List<string> list = new List<string>(version.Split(new char[]
		{
			'.'
		}));
		if (list.Count < 1)
		{
			return false;
		}
		int num = 0;
		foreach (string current in list)
		{
			if (!int.TryParse(current, out num))
			{
				return false;
			}
		}
		return true;
	}

	public static int Compare(string inVersion, string againstVersion)
	{
		if (!ValidVersion(inVersion))
		{
			return -1;
		}
		if (!ValidVersion(againstVersion))
		{
			return 1;
		}
		List<string> inVersionListString = new List<string>(inVersion.Split(new char[]
		{
			'.'
		}));
		List<string> againtsVersionlistString = new List<string>(againstVersion.Split(new char[]
		{
			'.'
		}));
		List<int> inVersionList = new List<int>();
		foreach (string current in inVersionListString)
		{
			inVersionList.Add(int.Parse(current));
		}
		List<int> againtVersionList = new List<int>();
		foreach (string current2 in againtsVersionlistString)
		{
			againtVersionList.Add(int.Parse(current2));
		}
		int maxVersionLength = Mathf.Max(inVersionList.Count, againtVersionList.Count);
		int num2 = maxVersionLength - inVersionList.Count;
		int num3 = maxVersionLength - againtVersionList.Count;
		for (int i = num2; i > 0; i--)
		{
			inVersionList.Add(0);
		}
		for (int j = num3; j > 0; j--)
		{
			againtVersionList.Add(0);
		}
		for (int k = 0; k < maxVersionLength; k++)
		{
			int inNum = inVersionList[k];
			int againtsNum = againtVersionList[k];
			if (inNum > againtsNum)
			{
				return 1;
			}
			if (inNum < againtsNum)
			{
				return -1;
			}
		}
		return 0;
	}
}
