using System;
using TMPro;
using UnityEngine;

public class WinCountBoxUI : MonoBehaviour
{
	private const string kIncrementTranstionStateKey = "ValueIncrement";

	public Animator WinCountAnimator;

	public TextMeshProUGUI CurrentStreakLabel;

    public TextMeshProUGUI TotalStreakLabel;

    //private CSR2FX_UILabel CurrentStreakFX;

	private void Awake()
    {
    //    if (this.CurrentStreakLabel != null)
    //    {
    //        this.CurrentStreakFX = this.CurrentStreakLabel.GetComponent<CSR2FX_UILabel>();
    //    }
	}

	public void TriggerWinStreakLabelUpdate(bool bIncludeAnimation = true)
	{
		int sMPChallengeWins = PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins;
		this.SetCurrentWinStreakLabel(sMPChallengeWins);
		if (bIncludeAnimation && this.WinCountAnimator != null)
		{
			this.WinCountAnimator.SetTrigger("ValueIncrement");
		}
	}

	public void SetCurrentWinStreakLabel(int winCount)
	{
		if (this.CurrentStreakLabel != null)
		{
			this.CurrentStreakLabel.text = winCount.ToString();
            //if (this.CurrentStreakFX != null)
            //{
            //    if (winCount <= 0)
            //    {
            //        this.CurrentStreakFX.SetPreset("GreyGradientExtraBold");
            //    }
            //    else
            //    {
            //        this.CurrentStreakFX.SetPreset("ChallengeExtraBold");
            //    }
            //}
		}
	}

	public void SetMaxWinStreakLabel()
	{
		int sMPWinStreakMaxValue = SMPConfigManager.WinStreak.SMPWinStreakMaxValue;
		if (this.TotalStreakLabel != null)
		{
			this.TotalStreakLabel.text = "/ " + sMPWinStreakMaxValue.ToString();
		}
	}

	private void OnIncrementAnimationComplete()
	{
	}
}
