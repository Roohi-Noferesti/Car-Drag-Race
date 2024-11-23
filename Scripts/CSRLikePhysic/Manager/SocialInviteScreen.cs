using System;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using UnityEngine;

public class SocialInviteScreen : HUDScreen
{
	private enum eSocialEventType
	{
		email,
		none,
		twitter,
		twitterFollow,
		videoAd
	}

	private const int MaxTwitterRewardsPerDay = 3;

    //public IconTextButton TwitterButton;

    //public IconTextButton VideoAdButton;

    //public IconTextButton TwitterFollowButton;

	public Transform ButtonNode;

    //public DataDrivenPortrait Mechanic;

    //public PackedSprite ForegroundObject;

	private eSocialEventType socialEventType = eSocialEventType.none;

	private int RewardAmount;

    //public override ScreenID ID
    //{
    //    get
    //    {
    //        return ScreenID.SocialInvite;
    //    }
    //}

	public override void OnCreated(bool zAlreadyOnStack)
	{
        base.OnCreated(zAlreadyOnStack);
        //this.TwitterButton.gameObject.SetActive(false);
        //this.TwitterFollowButton.gameObject.SetActive(false);
        //VideoForRewardConfiguration configuration = GameDatabase.Instance.Ad.GetConfiguration(VideoForRewardConfiguration.eRewardID.PressForFuel);
        //if (configuration != null && configuration.Enabled)
        //{
        //    this.VideoAdButton.Button.Runtime.CurrentState = BaseRuntimeControl.State.Active;
        //    this.VideoAdButton.Button.SetText(string.Format(LocalizationManager.GetTranslation("TEXT_BUTTON_VIDEO_AD_FOR_FUEL"), VideoForRewardsManager.GetRewardAmount(configuration)), true, true);
        //}
        //else
        //{
        //    this.VideoAdButton.Button.Runtime.CurrentState = BaseRuntimeControl.State.Disabled;
        //    this.VideoAdButton.Button.SetText(string.Format(LocalizationManager.GetTranslation("TEXT_BUTTON_VIDEO_AD_FOR_FUEL"), 0), true, true);
        //}
        //this.Mechanic.Init(PopUpManager.Instance.graphics_mechanicPrefab, LocalizationManager.GetTranslation("TEXT_NAME_MECHANIC").ToUpper(), null);
        //SocialController.Instance.DoesTwitterUserFollowUsEvent += new SocialController_BoolDelegate(this.TwitterUserFollowedUsCallback);
		ApplicationManager.DidBecomeActiveEvent += new ApplicationEvent_Delegate(this.ApplicationWillEnterForeground);
	}

	public void TwitterUserFollowedUsCallback(bool didFollow)
	{
		if (didFollow)
		{
            //if (!PlayerProfileManager.Instance.ActiveProfile.HasFollowedUsOnTwitter)
            //{
            //    SocialUtils.ShowTwitterFollowFuelAwardPopUp();
            //}
            //this.TwitterFollowButton.Button.Runtime.CurrentState = BaseRuntimeControl.State.Disabled;
		}
	}

	public override void OnDeactivate()
	{
        //SocialController.Instance.DoesTwitterUserFollowUsEvent -= new SocialController_BoolDelegate(this.TwitterUserFollowedUsCallback);
        ApplicationManager.DidBecomeActiveEvent -= new ApplicationEvent_Delegate(this.ApplicationWillEnterForeground);
        base.OnDeactivate();
    }

	public void OnTwitterSendInviteCompleted(bool success)
	{
		if (!success)
		{
			return;
		}
		int rew = 1;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (GameDatabase.Instance.CurrenciesConfiguration.Fuel != null)
		{
			rew = GameDatabase.Instance.CurrenciesConfiguration.Fuel.TwitterInviteFuelGain;
		}
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.Pltfrm,
				"Twitter"
			},
			{
				Parameters.Intfrm,
				"Invite"
			},
			{
				Parameters.Dfuel,
				rew.ToString()
			}
		};
		Log.AnEvent(Events.SocialInteraction, data);
		activeProfile.CumulativeTweets++;
		if (this.CanGiveTwitterReward())
		{
			this.socialEventType = eSocialEventType.twitter;
			this.FuelRewardPopUp(rew);
		}
		else
		{
			this.NoRewardThankTwitter();
		}
	}

	public void OnTwitterButton()
	{
        //SocialController.Instance.SendInviteTweetAction(new TwitterSentCallBack(this.OnTwitterSendInviteCompleted));
	}

	public void OnVideoAdForFuelButton()
	{
		TriggerVideoAdForFuelFlow();
	}

	public void OnTwitterFollowPressed()
	{
		if (PlayerProfileManager.Instance.ActiveProfile.HasFollowedUsOnTwitter)
		{
		}
        //SocialController.Instance.PerformFollowUsOnTwitter(new TwitterSentCallBack(this.TwitterUserFollowedUsCallback));
	}

	public static void TriggerVideoAdForFuelFlow()
	{
        if (BasePlatform.ActivePlatform.GetReachability() == BasePlatform.eReachability.OFFLINE)
        {
		
            PopUpDatabase.Common.ShowNoInternetConnectionPopup();
            return;
        }

		VideoForRewardsManager.Instance.StartFlow(VideoForRewardConfiguration.eRewardID.PressForFuel);
	}

    ////protected void Update()
    ////{
    ////    //base.Update();
    ////    //if (this.ForegroundObject != null && PopUpManager.Instance.isShowingPopUp != this.ForegroundObject.gameObject.activeInHierarchy)
    ////    //{
    ////    //    this.ForegroundObject.gameObject.SetActive(PopUpManager.Instance.isShowingPopUp);
    ////    //}
    ////}

	private void FuelRewardPopUp(int rew)
	{
		this.RewardAmount = rew;
		string bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_FUEL_INVITE_FRIEND"), this.RewardAmount);
		PopUp popup = new PopUp
		{
			Title = "TEXT_FUEL_INVITE_FRIEND_TITLE",
			BodyText = bodyText,
			BodyAlreadyTranslated = true,
			IsBig = true,
			ConfirmAction = new PopUpButtonAction(this.GiveReward),
			ConfirmText = "TEXT_BUTTON_OK"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private void NoRewardThankTwitter()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        TimeSpan timeSpan = ServerSynchronisedTime.Instance.GetDateTime() - activeProfile.TwitterInviteFuelRewardsTime;
		string bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_FUEL_INVITE_FRIEND_NOREWARD_TWITTER"), Mathf.Max(1, 24 - Mathf.FloorToInt((float)timeSpan.TotalHours)));
		PopUp popup = new PopUp
		{
			Title = "TEXT_FUEL_INVITE_FRIEND_TITLE",
			BodyText = bodyText,
			BodyAlreadyTranslated = true,
			IsBig = true,
			ConfirmText = "TEXT_BUTTON_OK"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private void ApplicationWillEnterForeground()
	{
        //if (!PlayerProfileManager.Instance.ActiveProfile.HasLikedOnFacebook && SocialController.Instance.isLoggedIntoFacebook)
        //{
        //    BasePlatform.ActivePlatform.GetDoesLikeUsOnFacebook();
        //}
	}

	private void GiveReward()
	{
		FuelManager.Instance.AddFuel(this.RewardAmount, FuelReplenishTimeUpdateAction.KEEP, FuelAnimationLockAction.OBEY);
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.Dfuel,
				this.RewardAmount.ToString()
			},
			{
				Parameters.InvtTyp,
				this.socialEventType.ToString()
			}
		};
		Log.AnEvent(Events.InviteFriend, data);
	}

	private bool CanGiveTwitterReward()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (activeProfile.TwitterInviteFuelRewardsTime.AddHours(24.0) < ServerSynchronisedTime.Instance.GetDateTime())
		{
            activeProfile.TwitterInviteFuelRewardsTime = ServerSynchronisedTime.Instance.GetDateTime();
			activeProfile.TwitterInviteFuelRewardsCount = 0;
		}
		if (activeProfile.TwitterInviteFuelRewardsCount < 3)
		{
            activeProfile.TwitterInviteFuelRewardsTime = ServerSynchronisedTime.Instance.GetDateTime();
			activeProfile.TwitterInviteFuelRewardsCount++;
			return true;
		}
		return false;
	}
}
