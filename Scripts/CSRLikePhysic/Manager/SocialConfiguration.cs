using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SocialConfiguration:ScriptableObject
{
	public enum FacebookPermissionList
	{
		HomeScreenLogin,
		RYFModeLogin,
		PostLogin,
		InviteButtonCheck,
		InvitePopupCheck
	}

	public Dictionary<string, List<string>> FaceBookLoginPermissions = new Dictionary<string, List<string>>();

	public string FacebookAppLink = "https://fb.me/837562886363072";

	public int TwitterFollowFuelGain = 1;

	public int FuelsPerSocialPerTime = 3;

	public int FuelGiftTimeRegionHours = 24;

	public int TwitterCashRewardT1 = 10;

	public int TwitterCashRewardT2 = 20;

	public int TwitterCashRewardT3 = 30;

	public int TwitterCashRewardT4 = 40;

	public int TwitterCashRewardT5 = 50;

	public int FacebookSSOReward;

	public int TwitterCashRewardTimeHours = 24;

	public int TwitterCashRewardAllowed = 3;

	public int NumberClockChangesForPunishment = 5;

	public bool BAWebTimeRequestsEnabled = true;

	public int ClockBackCheckTolerance = 60;

	public int ClockOSUTCheckTolerance = 5;

	public int ClockServerCheckTolerance = 10;

	public int ClockChangeResetTime = 168;

	public bool ReplayKitEnabled;

	public List<int> BossEventsIDs = new List<int>();

    public int TelegramCashReward = 3000;

    public int InstagramGoldReward = 20;
    public int[] ReferralGoldRewards;

    public bool UseHelpshift = true;

    public List<string> AllFacebookPermissions
	{
		get
		{
			return this.FaceBookLoginPermissions.SelectMany((KeyValuePair<string, List<string>> keyValuePair) => keyValuePair.Value).Distinct<string>().ToList<string>();
		}
	}


    public List<string> GetFacebookPermissions(SocialConfiguration.FacebookPermissionList loginType)
	{
		List<string> list;
		this.FaceBookLoginPermissions.TryGetValue(loginType.ToString(), out list);
		if (list == null)
		{
			list = new List<string>();
		}
		return list;
	}
	
	
}
