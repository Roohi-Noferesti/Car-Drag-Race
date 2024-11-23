using System;
using System.Collections.Generic;
using UnityEngine;

public class NmgBinding : MonoBehaviour
{
	private static string dictionaryToString(Dictionary<string, string> dict)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, string> current in dict)
		{
			list.Add(string.Format("{0}||{1}", current.Key, current.Value));
		}
		return string.Join("|||", list.ToArray());
	}

	public static void logEventWithParameters(string eventName, string parameters)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		string[] array = parameters.Split(new string[]
		{
			"///"
		}, 2147483647, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new string[]
			{
				"|/|"
			}, 2147483647, StringSplitOptions.None);
			for (int j = 0; j < array2.Length; j++)
			{
				string[] array3 = array2[j].Split(new string[]
				{
					"||"
				}, 2147483647, StringSplitOptions.None);
				if (array3[0] != string.Empty)
				{
					list.Add(array3[0]);
					if (array3.Length > 1)
					{
						list2.Add(array3[1]);
					}
					else
					{
						list2.Add(string.Empty);
					}
				}
			}
		}
		string[] paramNames = list.ToArray();
		string[] paramValues = list2.ToArray();
		NmgServices.LogMetricsEvent(eventName, paramNames, paramValues, list.Count);
	}

	public static string NmgServicesUpdate()
	{
		switch (NmgServices.Update())
		{
		case 0:
			return "NMG_METRICS_STATUS_INVALID";
		case 1:
			return "NMG_METRICS_STATUS_NO_NETWORK";
		case 2:
			return "NMG_METRICS_OK";
		default:
			return "NMG_METRICS_?";
		}
	}

	public static bool NmgServicesConnect(string url, string preSharedKey, string productKey, string productDesc)
	{
		return NmgServices.Initialise(url, preSharedKey, productKey, productDesc);
	}

	public static void SetGameCenterID(string gameCenterId)
	{
		NmgServices.SetGameCenterID(gameCenterId);
	}

	public static void SetPushNotificationData(string token, bool badges, bool alerts, bool sounds)
	{
		NmgServices.SetPushNotificationData(Convert.FromBase64String(token), badges, alerts, sounds);
	}

	public static void AddSocialNetworkData(string network, string userId)
	{
		NmgServices.AddSocialNotificationData(network, userId);
	}

	public static void AddSocialNetworkDatas(string network, string userIds)
	{
		NmgServices.AddSocialNotificationDatas(network, userIds);
	}

	public static string GetCoreID()
	{
		return NmgServices.GetCoreID();
	}
}
