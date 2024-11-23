using System;
using UnityEngine;

public class SMPWinStreakMonitor : MonoBehaviour
{
	public Action OnWinStreakTimedOut;

	public Action OnWinChallengeAvailable;

	private void Update()
	{
		if (PlayerProfileManager.Instance == null || PlayerProfileManager.Instance.ActiveProfile == null || !SMPConfigManager.WinStreak.IsConfigDataValid)
		{
			return;
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (!activeProfile.IsSMPWinChallengeAvailable)
		{
			TimeSpan nextSMPWinChallengeRemainingTime = activeProfile.GetNextSMPWinChallengeRemainingTime();
			if (nextSMPWinChallengeRemainingTime <= TimeSpan.Zero)
			{
				activeProfile.IsSMPWinChallengeAvailable = true;
                activeProfile.SMPWinChallengeActivationTime = ServerSynchronisedTime.Instance.GetDateTime();
				if (this.OnWinChallengeAvailable != null)
				{
					this.OnWinChallengeAvailable();
				}
				PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
			}
		}
	}
}
