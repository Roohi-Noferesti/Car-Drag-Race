using System;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;

public abstract class PostRaceScreen : ZHUDScreen
{
	protected enum MenuItems
	{
		Restart,
		Prizes,
		Share,
		Analysis,
		Mechanic,
		Tuning,
		Next
	}

    public RuntimeTextButton TuningButton;
    public RuntimeTextButton NextButton;
    public RuntimeTextButton MechanicButton;
    public RuntimeTextButton ShareButton;
    public RuntimeTextButton ProfileButton;
    public RuntimeTextButton RetryButton;
    public RuntimeTextButton ExtraRewardButton;

	public Texture2D[] MenuItemTexturesHit;

	public RaceReCommon CommonReUIElements;

	public bool UseFakeRaceData;

	//private bool isShowingPreviewView;

	protected static RaceResultsTrackerState ShownResults = default(RaceResultsTrackerState);

	public override void OnCreated(bool zAlreadyOnStack)
	{
		ShareButton.gameObject.SetActive(BuildType.CanShowShareButton());
		
		RetryButton.gameObject.SetActive(ShouldShowRetryButton());
		
		if (ShouldShowExtraRewardButton()) {
			ExtraRewardButton.gameObject.SetActive(true);
			ExtraRewardButton.CurrentState = BaseRuntimeControl.State.Active;
		} else {
			ExtraRewardButton.gameObject.SetActive(false);
		}
		

		if (!zAlreadyOnStack)
		{
			PostRaceScreen.ShownResults = RaceResultsTracker.GetState();
		}
		if (RaceEventInfo.Instance.CurrentEvent == null)
		{
			this.UseFakeRaceData = true;
		}
		if (!this.UseFakeRaceData)
		{
			this.CommonReUIElements.SetButtonFunctionality();
            //if (SocialController.TwitterIsDisabled && SocialController.FacebookIsDisabled)
            //{
            //    base.CarouselList.GetItem(1).GreyOutThisItem(true);
            //}
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;

			if (currentEvent.IsRelay)
			{
                //bool disable = RelayManager.HumanWon() || RaceEventInfo.Instance.IsNonRestartable();
                //base.CarouselList.GetItem(0).GreyOutThisItem(disable);
			}
			else if (PostRaceScreen.ShownResults.You.IsWinner || RaceEventInfo.Instance.IsNonRestartable() || (activeProfile.RacesEntered > 0 && activeProfile.RacesWon == 1) || currentEvent.IsRaceTheWorldOrClubRaceEvent())
			{
				bool flag = true;
				if (currentEvent.IsFriendRaceEvent())
				{
					CompetitorManager instance = CompetitorManager.Instance;
					if (instance.LocalCompetitor.PlayerInfo.CsrUserID == instance.OtherCompetitor.PlayerInfo.CsrUserID)
					{
						flag = false;
					}
				}
				if (flag)
				{
                    //base.CarouselList.GetItem(0).GreyOutThisItem(true);
				}
			}
			if ((this.ShouldShowMechanic() && !RaceReCommon.IsAfterRaceMechanicAvailable()) || currentEvent.IsRaceTheWorldOrClubRaceEvent() || currentEvent.IsLocalCarLoaned())
			{
				if(MechanicButton!=null)
					MechanicButton.CurrentState = BaseRuntimeControl.State.Disabled;
                //base.CarouselList.GetItem(3).GreyOutThisItem(true);
			}
		}
		//this.isShowingPreviewView = false;
	}

	private bool ShouldShowRetryButton()
	{
		return !ShownResults.You.IsWinner && !RaceEventInfo.Instance.IsNonRestartable();
	}
	
	private bool ShouldShowExtraRewardButton()
	{
		return GTAdManager.Instance.CanShowAdForExtraReward() &&
		       ShownResults.You.IsWinner &&
		       !VideoForRewardsManager.Instance.VideoAdCapHit(VideoForRewardConfiguration.eRewardID.VideoForExtraCashPrize) &&
		       !RaceEventInfo.Instance.CurrentEvent.IsTutorial() &&
		       (RaceEventInfo.Instance.CurrentEvent.IsCrewBattle() || RaceEventInfo.Instance.CurrentEvent.IsCrewRace());
	}

	protected override void Update()
	{
        base.Update();
        //bool flag = UnityReplayKit.IsShowingPreviewView();
        //if (this.isShowingPreviewView != flag)
        //{
        //    this.isShowingPreviewView = flag;
        //    if (this.isShowingPreviewView)
        //    {
        //        MenuAudio.Instance.setMuteMusic(true);
        //        AudioManager.SetMute("Audio_SFXAudio", true);
        //    }
        //    else
        //    {
        //        MenuAudio.Instance.setMuteMusic(PlayerProfileManager.Instance.ActiveProfile.OptionMusicMute);
        //        AudioManager.SetMute("Audio_SFXAudio", PlayerProfileManager.Instance.ActiveProfile.OptionSoundMute);
        //    }
        //}
	}

	protected override void OnDestroy()
	{
		this.MenuItemTexturesHit = null;
		VideoCapture.ClearRecording();
        base.OnDestroy();
	}

	private bool ShouldShowMechanic()
	{
		return RaceEventInfo.Instance.CurrentEvent.IsMechanicAllowed() && (PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().GetIsFullyUpgraded() || RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent());
	}

	protected void PopulateItemLists()
	{
        //ShareButton.CurrentState = BaseRuntimeControl.State.Active;
        //ProfileButton.CurrentState = BaseRuntimeControl.State.Active;
        //if (this.ShouldShowMechanic())
        //{
        //    MechanicButton.CurrentState = BaseRuntimeControl.State.Active;
        //}
        //else
        //{
        //    TuningButton.CurrentState = BaseRuntimeControl.State.Active;
        //}
        //NextButton.CurrentState = BaseRuntimeControl.State.Active;
	}

	private bool DidPlayerWin()
	{
		if (RelayManager.IsCurrentEventRelay())
		{
			return RelayManager.HumanWon();
		}
		return PostRaceScreen.ShownResults.You.IsWinner;
	}

    public bool IsButtonValid()
    {
        if (PlayerProfileManager.Instance.ActiveProfile == null)
        {
            return false;
        }

        if (!RaceRewardScreen.ScreenFinished)
        {
            ForceFinishScreen();
            return false;
        }

        if (XPStats.DoForceUpdateAndCheck())
        {
            return false;
        }
        return true;
    }


    public void OnRestart()
    {
	    CommonUI.Instance.FuelStats.FuelLockedState(true);
        bool playerWon = this.DidPlayerWin();
        if (!IsButtonValid()) return;
        //SocialController.Instance.ClearSocialNagTrigger();
        if (this.UseFakeRaceData)
        {
            ScreenManager.Instance.PushScreen(ScreenID.RaceRewards);
        }
        else
        {
            this.CommonReUIElements.OnRetry(playerWon);
        }
    }

    public void OnShare()
    {
        if (!IsButtonValid())
            return;
        if (!this.CommonReUIElements.ShouldBlockLeaveScreen())
        {
            this.DismissTwitterNagger();
            if (VideoCapture.IsSupportedAndEnabled)
            {
                this.ShowWinSharePopup();
            }
            else
            {
                this.TriggerStandardSocialFlow();
            }
        }
    }

    public void OnProfile()
    {
        if (!IsButtonValid())
            return;

        RaceReCommon.OnPlayerProfileActivate();
        ScreenManager.Instance.PushScreen(ScreenID.MultiplayerProfile);
    }

    public void OnMechanic()
    {
        bool playerWon = this.DidPlayerWin();
        if (!IsButtonValid())
            return;
        this.CommonReUIElements.OnMechanicButton(playerWon);
    }

    public void OnTuning()
    {
        bool playerWon = this.DidPlayerWin();
        if (!IsButtonValid())
            return;
        this.CommonReUIElements.GoUpgrade(playerWon);
    }

    public void OnNext()
    {
        if (!IsButtonValid())
            return;
        this.OnNextButton();
    }

	private void TriggerVideoWinSocialFlow()
	{
        //SocialController.Instance.OnShareButton(SocialController.MessageType.WIN_RACE, null, false, true);
	}

	private void TriggerStandardSocialFlow()
	{
        if (this.DidPlayerWin())
        {
            RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
            if (currentEvent.IsBossRace() && currentEvent.BossRaceIndex() == 2)
            {
                eCarTier carTier = currentEvent.Parent.GetTierEvents().GetCarTier();
                string detail = LocalizationManager.GetTranslation(Chatter.GetCrewLeaderName(carTier)).ToUpper();
                SocialController.Instance.OnShareButton(SocialController.MessageType.WIN_RACE, detail, false, false);
            }
            else
            {
                SocialController.Instance.OnShareButton(SocialController.MessageType.WIN_RACE, null, false, false);
            }
        }
        else
        {
            SocialController.Instance.OnShareButton(SocialController.MessageType.INVITE, null, false, false);
        }
    }

	public void OnNextButton()
	{
		if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent()
            || RaceEventInfo.Instance.CurrentEvent.IsSMPRaceEvent())
		{
            //StreakManager.UpdateStreak(ShownResults.You.IsWinner);
		}
		this.CommonReUIElements.OnNextButton(ShownResults.You.IsWinner);
		this.KillTwitterNagger();
	}

	public virtual void ResetScreen()
	{
	}

	private void ForceFinishScreen()
	{
		if (!RaceRewardScreen.ScreenFinished)
		{
			this.FinishScreen(true);
		}
	}

	public virtual void OnScreenClick()
	{
	}

	public virtual void OnMoreInfo()
	{
        ScreenManager.Instance.PushScreen(ScreenID.RaceResults);
	}

	public virtual void FinishScreen(bool finishAnims)
	{
		if (this.NeedsFirstTimeVideoSharePrompt() && this.IsVideoShareAvailable() && !PopUpManager.Instance.isShowingPopUp)
		{
			this.ShowVideoShareTutorialPopup();
		}
        //(base.CarouselList.GetItem(1) as BasicMenuListItem).NotificationActive = this.ShouldShowShareNotificationIcon();
	}

	protected virtual void DismissTwitterNagger()
	{
	}

	protected virtual void KillTwitterNagger()
	{
	}

	private bool ShouldShowShareNotificationIcon()
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		return ((currentEvent.IsBossRace() || currentEvent.IsCrewRace()) && this.DidPlayerWin()) || this.IsVideoShareAvailable();
	}

	private bool IsVideoShareAvailable()
	{
	    return false;//SocialController.Instance.IsVideoReplayAvailable();
	}

	private bool NeedsFirstTimeVideoSharePrompt()
	{
	    return false;
		//RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		//RaceEventData crewBattleEvent = RaceEventQuery.Instance.GetCrewBattleEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(eCarTier.TIER_1), false);
		//return (currentEvent.IsFirstCrewMemberRace() || (crewBattleEvent != null && crewBattleEvent.Parent.NumEventsComplete() >= 1)) && !PlayerProfileManager.Instance.ActiveProfile.SharingTutorial_VideoShareCompleted && this.DidPlayerWin();
	}

	private bool ShowVideoShareTutorialPopup()
	{
		PopUp popUp = new PopUp();
		popUp.Title = "TEXT_POPUP_TUTORIAL_SHAREVIDEO_TITLE";
		popUp.BodyText = "TEXT_POPUP_TUTORIAL_SHAREVIDEO_BODY";
		popUp.IsBig = true;
        popUp.GraphicPath = PopUpManager.Instance.graphics_bloggerPrefab;
		popUp.ImageCaption = "TEXT_NAME_CONSUMABLES_PRAGENT";
		popUp.CancelText = "TEXT_BUTTON_OK";
		popUp.CancelAction = delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.SharingTutorial_VideoShareCompleted = true;
			PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
		};
		PopUp popup = popUp;
		return PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private bool ShowWinSharePopup()
	{
		List<ButtonDetails> buttons = new List<ButtonDetails>
		{
			new ButtonDetails
			{
				Label = LocalizationManager.GetTranslation("TEXT_POPUP_RACEWINSHARE_BUTTON_VIDEO"),
				Type = ButtonManager.CSRButtonType.BlackTextCompact,
				Disabled = !this.IsVideoShareAvailable(),
				Action = new PopUpButtonAction(this.TriggerVideoWinSocialFlow)
			},
			new ButtonDetails
			{
				Label = LocalizationManager.GetTranslation("TEXT_POPUP_RACEWINSHARE_BUTTON_NORMAL"),
				Type = ButtonManager.CSRButtonType.BlackTextCompact,
				Action = new PopUpButtonAction(this.TriggerStandardSocialFlow)
			},
			new ButtonDetails
			{
				Label = LocalizationManager.GetTranslation("TEXT_BUTTON_CANCEL"),
				Type = ButtonManager.CSRButtonType.BlackTextCompact,
				Action = new PopUpButtonAction(this.DismissPopup)
			}
		};
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUP_RACEWINSHARE_TITLE",
			BodyText = "TEXT_POPUP_RACEWINSHARE_BODY",
			IsBig = true,
            GraphicPath = PopUpManager.Instance.graphics_bloggerPrefab,
			ImageCaption = "TEXT_NAME_CONSUMABLES_PRAGENT",
			Buttons = buttons
		};
		return PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private void DismissPopup()
	{
		PopUpManager.Instance.KillPopUp();
	}
}
