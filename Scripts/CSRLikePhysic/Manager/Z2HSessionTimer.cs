using System.Collections;
using System.Collections.Generic;
using KingKodeStudio;
using UnityEngine;

public class Z2HSessionTimer : MonoBehaviour
{
	public void StartSessionTimer()
	{
		base.StartCoroutine(this.SessionTimerCoroutine(60));
	}

    public IEnumerator SessionTimerCoroutine(int interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            if (activeProfile != null)
            {
                activeProfile.TotalPlayTime++;
                if (ScreenManager.Instance.CurrentScreen == ScreenID.Workshop)
                {
                    activeProfile.TotalGarageTime++;
                }
            }
        }
    }
}
