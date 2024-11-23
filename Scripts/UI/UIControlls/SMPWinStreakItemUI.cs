using System;
using TMPro;
using UnityEngine;

public class SMPWinStreakItemUI : MonoBehaviour
{
	public const string kPrizeFadeIn = "PrizeFadeIn";

	public const string kPrizeAvailable = "PrizeAvailable";

	public const string kPrizeWin = "PrizeWin";

	public const string kPrizeActive = "PrizeActive";

	public const string kPrizeCompleted = "PrizeCompleted";

	public const string kPrizeInactive = "PrizeInactive";

	public Animator StreakAnimator;

	public TextMeshProUGUI Amount;

	public string AudioEventName;

    //public UIAnimationEventRedirect AnimRedirect
    //{
    //    get;
    //    private set;
    //}

	public void Awake()
	{
        //this.AnimRedirect = base.GetComponent<UIAnimationEventRedirect>();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Initialise(SMPWinStreakReward reward, int itemStreakCount, int currentWinStreak)
	{
		if (!PlayerProfileManager.Instance.ActiveProfile.IsSMPWinChallengeAvailable)
		{
			this.PlayAnimationState("PrizeInactive");
		}
		else// if (currentWinStreak == SMPWinStreakUI.PreviousWinCount)
		{
			if (itemStreakCount == currentWinStreak+1)
			{
				this.PlayAnimationState("PrizeActive");
			}
            else if (itemStreakCount < currentWinStreak+1)//(itemStreakCount <= SMPWinStreakUI.PreviousWinCount)
			{
				this.PlayAnimationState("PrizeCompleted");
			}
		}
        //else if (currentWinStreak > SMPWinStreakUI.PreviousWinCount)
        //{
        //    if (itemStreakCount == SMPWinStreakUI.PreviousWinCount)
        //    {
        //        this.PlayAnimationState("PrizeActive");
        //    }
        //    else if (itemStreakCount < SMPWinStreakUI.PreviousWinCount)
        //    {
        //        this.PlayAnimationState("PrizeCompleted");
        //    }
        //}
		if (reward != null)
		{
			ERewardType rewardType = reward.WinReward.rewardType;
			switch (rewardType)
			{
			case ERewardType.LockupKey:
			case ERewardType.SilverGachaKey:
			case ERewardType.GoldGachaKey:
				break;
			default:
				if (rewardType != ERewardType.Cash && rewardType != ERewardType.RP)
				{
                    if (this.Amount != null)
					    this.Amount.text = reward.Amount.ToString();
					return;
				}
				break;
			}
            if (this.Amount != null)
		        this.Amount.text = reward.Amount.ToNativeNumber();//LocalisationManager.CultureFormat(reward.Amount, "N0");
		}
	}

	public void PlayAnimationState(string stateName)
	{
		if (this.StreakAnimator != null)
		{
			this.StreakAnimator.Play(stateName);
			if (stateName == "PrizeWin")
			{
                AudioManager.Instance.PlaySound(this.AudioEventName, null);
			}
		}
	}
}
