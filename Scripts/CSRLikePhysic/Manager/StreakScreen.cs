using System.Collections.Generic;
using I2.Loc;
using Metrics;
using UnityEngine;

public class StreakScreen : ZHUDScreen
{
	public static bool FAKE_DATA_FOR_TESTING;

	public static bool FAKE_WON;

    //public RaceReCommon RaceCommon;

    //public PrizeList PrizeList;

    //public BankSequence BankSequence;

    //public GoRaceButton ButtonNextRace;

	public GameObject BankParent;

    //public Text BankText;

    //public DummyTextButton ButtonBank;

    //public DummyTextButton Continue;

    //public Text TimeRemaining;

	public GameObject OrText;

	public ParticleSystem LoseStreakParticleSystem;

    //public WinStreakHeader Header;

	public Animation FlareFlickerAnimation;

    //public FullScreenFlash Flash;

	//private int CashWon;

	private int PrizesWon;

	private bool WonRace;

	//private int CardsWon;

	private bool displayedWinStreakPopup;

	private bool shouldDisplayWinStreakPopup;

	private bool displayedWinStreakLostPopup;

	private bool shouldDisplayWinStreakLossPopup;

	private bool shouldAutoBank;

	private int currentStreakNumber;

	//private int cachedEventID = -1;

	private bool oldGestureValue;

	private bool tappingEnabled;

	public AnimationCurve CameraShakeStreakLost;

	public float CameraShakeStreakLostDuration;

	public AnimationCurve CameraShakeBanked;

	public float CameraShakeBankedDuration;

	private GUICameraShake cameraShake;

	private static bool triggeredBrokenStreakRetry;

	private static bool cancelledBrokenStreakRetry;

	//private bool continueButtonEnabled;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.Streak;
		}
	}

    //private NetworkReplayManager Replay
    //{
    //    get
    //    {
    //        return NetworkReplayManager.Instance;
    //    }
    //}

	public override void OnActivate(bool zAlreadyOnStack)
	{
        //this.HasLowerCarousel = false;
		if (FAKE_DATA_FOR_TESTING)
		{
			this.WonRace = FAKE_WON;
			this.currentStreakNumber = 6;
			this.PrizesWon = 6;
			//this.CashWon = 100000;
			//this.CardsWon = 3;
		}
		else
		{
			this.PrizesWon = StreakManager.CurrentStreak();
			this.currentStreakNumber = StreakManager.CurrentStreak();
            //this.WonRace = (RaceResultsTracker.You.IsWinner || StreakScreen.triggeredBrokenStreakRetry);
			this.shouldAutoBank = StreakManager.HasBanked();
			//this.CashWon = StreakManager.CashBank();
			//this.CardsWon = StreakManager.CardsBank();
		}
		if (!triggeredBrokenStreakRetry && !StreakManager.OpponentListIsEmpty())
		{
            //StreakManager.SetDefeated(PlayerListScreen.LastSelectedReplayData.EntryID, RaceResultsTracker.You.IsWinner);
		}
		this.displayedWinStreakPopup = false;
		this.shouldDisplayWinStreakPopup = false;
		this.displayedWinStreakLostPopup = false;
		this.shouldDisplayWinStreakLossPopup = false;
		this.HideAllButtons();
		if (this.WonRace)
		{
			int bestEverMultiplayerWinStreak = PlayerProfileManager.Instance.ActiveProfile.BestEverMultiplayerWinStreak;
			if (this.currentStreakNumber > bestEverMultiplayerWinStreak)
			{
				PlayerProfileManager.Instance.ActiveProfile.BestEverMultiplayerWinStreak = this.currentStreakNumber;
				this.shouldDisplayWinStreakPopup = true;
			}
		}
		else if (this.currentStreakNumber > 0 && !PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_LostWinStreakCompleted)
		{
			this.shouldDisplayWinStreakLossPopup = true;
		}
		if (this.WonRace)
		{
			if (this.currentStreakNumber == 6)
			{
                //this.Header.SetBG(WinStreakHeader.eBackground.Complete);
			}
			else
			{
                //this.Header.SetBG(WinStreakHeader.eBackground.Neutral);
			}
            //if (SeasonFlowManager.Instance.EndOfRaceSeasonChangeTriggered)
            //{
            //    this.shouldAutoBank = true;
            //}
		}
		else
		{
            //this.Header.SetBG(WinStreakHeader.eBackground.Lost);
		}
        //this.PrizeList.Initialize(this.PrizesWon, this.WonRace);
        //this.BankSequence.Setup(this.CashWon, this.CardsWon, this.currentStreakNumber == 6);
		if (this.PrizesWon > 0)
		{
			Vector3 localPosition = this.LoseStreakParticleSystem.transform.localPosition;
            //localPosition.y = this.PrizeList.Prizes[this.PrizesWon - 1].transform.localPosition.y - this.PrizeList.Prizes[5].transform.localPosition.y;
			this.LoseStreakParticleSystem.transform.localPosition = localPosition;
		}
		this.CreateStreakButtons();
		this.Anim_Initialize();
		if (this.shouldAutoBank)
		{
			this.shouldDisplayWinStreakLossPopup = false;
			this.shouldDisplayWinStreakPopup = false;
			this.DoBankSequence();
		}
		base.OnActivate(zAlreadyOnStack);
		this.Anim_Play();
		if (zAlreadyOnStack && !this.WonRace && !cancelledBrokenStreakRetry)
		{
			this.Anim_LoseStreakAnimEnd();
			this.Anim_Finish_Prizes();
		}
	}

	protected override void OnDestroy()
	{
		if (this.cameraShake != null)
		{
			this.cameraShake.ShakeOver();
		}
		FAKE_DATA_FOR_TESTING = false;
		this.DisableTapping();
		base.OnDestroy();
	}

    //private void OnScreenTapped(GenericTouch touchData)
    //{
    //    if (this.PrizesWon < 1 || this.PrizesWon > this.PrizeList.Prizes.Length)
    //    {
    //        return;
    //    }
    //    if (PopUpManager.Instance.isShowingPopUp)
    //    {
    //        return;
    //    }
    //    PrizeEntry prizeEntry = this.PrizeList.Prizes[this.PrizesWon - 1];
    //    Renderer renderer = prizeEntry.BarOnCombined.renderer;
    //    Bounds bounds = renderer.bounds;
    //    Vector2 vector = GUICamera.Instance.WorldToScreenSpace(bounds.min);
    //    Vector2 vector2 = GUICamera.Instance.WorldToScreenSpace(bounds.max);
    //    if (touchData.Position.x >= vector.x && touchData.Position.x <= vector2.x && touchData.Position.y >= vector.y && touchData.Position.y <= vector2.y)
    //    {
    //        PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_BANKING_TITLE", "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_BANKING_BODY", null, false);
    //    }
    //}

	private void OnRespectRankingTutorialPopupDismissed()
	{
		this.Anim_Play();
	}

	private void SendMetricsEvent()
	{
		Dictionary<int, Events> dictionary = new Dictionary<int, Events>
		{
			{
				1,
				Events.GreatJob
			},
			{
				2,
				Events.TwoDown
			},
			{
				3,
				Events.NextOneCard
			},
			{
				4,
				Events.OneCardGood
			},
			{
				5,
				Events.GonnaBeRich
			},
			{
				6,
				Events.FrankieFeelsFine
			}
		};
		Log.AnEvent(dictionary[this.currentStreakNumber]);
	}

	private void OnWinStreakTutorialPopupDismissed()
	{
		this.SendMetricsEvent();
		this.displayedWinStreakPopup = true;
		if (this.PrizesWon == 6)
		{
			this.DoBankSequence();
			return;
		}
		this.ShowRaceAndBankButtons();
	}

	private void OnLostWinStreakTutorialPopupDismissed()
	{
		this.displayedWinStreakLostPopup = true;
		Log.AnEvent(Events.IKnowHowToLose);
		this.ShowContinueButton();
	}

	private void CreateStreakButtons()
	{
		//string text = string.Format(LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_RPSCREEN_RACEBUTTON"), this.PrizesWon + 1);
        //this.ButtonNextRace.RaceText.Text = text;
        //this.BankText.Text = LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_RPSCREEN_BANKBUTTON");
        //this.ButtonBank.SetText(string.Empty, true, true);
	}

	private void SetupTimeRemaining()
	{
        //switch (MultiplayerUtils.SelectedMultiplayerMode)
        //{
        //case MultiplayerMode.RACE_THE_WORLD:
        //case MultiplayerMode.PRO_CLUB:
        //    if (this.cachedEventID == -1)
        //    {
        //        this.cachedEventID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonEventID();
        //    }
        //    this.TimeRemaining.Text = SeasonCountdownManager.GetRemainingTimeString(this.cachedEventID, false);
        //    this.SetNextButtonActive(!SeasonCountdownManager.HasEventEnded(this.cachedEventID));
        //    break;
        //case MultiplayerMode.EVENT:
        //{
        //    MultiplayerEventData data = MultiplayerEvent.Saved.Data;
        //    this.TimeRemaining.Text = data.GetTimeRemainingString(false);
        //    this.SetNextButtonActive(data.IsActive());
        //    break;
        //}
        //default:
        //    this.TimeRemaining.Text = LocalizationManager.GetTranslation("TEXT_ENDING_SOON");
        //    break;
        //}
        //this.TimeRemaining.transform.parent.gameObject.SetActive(true);
	}

	private void SetNextButtonActive(bool active)
	{
        //if (this.continueButtonEnabled)
        //{
        //    this.Continue.Runtime.CurrentState = BaseRuntimeControl.State.Active;
        //    this.ButtonNextRace.gameObject.SetActive(false);
        //    this.OrText.gameObject.SetActive(false);
        //}
        //else
        //{
        //    this.ButtonNextRace.gameObject.SetActive(active);
        //    this.OrText.gameObject.SetActive(active);
        //}
	}

	public override void OnDeactivate()
	{
		this.DisableTapping();
        //if (SeasonFlowManager.Instance.EndOfRaceSeasonChangeTriggered)
        //{
        //    TouchManager.Instance.GesturesEnabled = true;
        //}
		base.OnDeactivate();
	}

	protected override void Update()
	{
        //if (this.TimeRemaining.gameObject.activeInHierarchy)
        //{
        //    this.SetupTimeRemaining();
        //}
		StreakManager.Chain.CheckBonusTimeOut();
	}

	private void OnContinue()
	{
        //MenuAudio.Instance.playSound(MenuBleeps.MenuClickForward);
        //AudioManager.Instance.PlaySound("GoRaceStreak", null);
		if (triggeredBrokenStreakRetry)
		{
			triggeredBrokenStreakRetry = false;
            //DailyBattleCalenderScreen.SetScreenToPushWhenInFrontend(ScreenID.PlayerList);
            //CSRAdManager.Instance.PrepareAdForDefaultInterstitialFlow(true);
            //RaceReCommon.GoBackToFrontEnd();
		}
		else
		{
			cancelledBrokenStreakRetry = false;
            //RaceReCommon.FinishOnNextButtonRaceTheWorld(ScreenID.Streak);
		}
	}

	private void OnBank()
	{
		if (!PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_SuccessfullyCompletedStreak)
		{
			Log.AnEvent(Events.ConfirmBank);
		}
        //MenuAudio.Instance.playSound(MenuBleeps.MenuClickForward);
		PopUpDatabase.Common.ShowStargazerPopupCancelConfirm("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_BANK_IT_CONFIRM_TITLE", "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_BANK_IT_CONFIRM_BODY", new PopUpButtonAction(this.OnPopupCancelBank), new PopUpButtonAction(this.OnPopupConfirmBank));
	}

	private void OnPopupConfirmBank()
	{
		if (!PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_SuccessfullyCompletedStreak)
		{
			Log.AnEvent(Events.Bank);
		}
		triggeredBrokenStreakRetry = false;
		cancelledBrokenStreakRetry = false;
		StreakManager.Chain.Reset();
		this.DoBankSequence();
	}

	private void OnPopupCancelBank()
	{
		if (!PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_SuccessfullyCompletedStreak)
		{
			Log.AnEvent(Events.UnBank);
		}
	}

	private void ShowRaceAndBankButtons()
	{
		if (this.shouldDisplayWinStreakPopup && !this.displayedWinStreakPopup)
		{
			this.HideAllButtons();
		}
		else
		{
			this.EnableTapping();
			this.OrText.SetActive(true);
			this.SetupTimeRemaining();
			this.BankParent.gameObject.SetActive(true);
            //this.ButtonBank.Runtime.CurrentState = BaseRuntimeControl.State.Active;
            //this.ButtonNextRace.gameObject.SetActive(true);
		}
	}

	private void ShowContinueButton()
	{
		this.EnableTapping();
        //this.Continue.Runtime.CurrentState = BaseRuntimeControl.State.Active;
		//this.continueButtonEnabled = true;
		this.SetupTimeRemaining();
	}

	private void EnableTapping()
	{
		if (!this.tappingEnabled)
		{
            //GestureEventSystem.Instance.Tap += new GestureEventSystem.GestureEventHandler(this.OnScreenTapped);
            //this.oldGestureValue = TouchManager.Instance.GesturesEnabled;
            //TouchManager.Instance.GesturesEnabled = true;
			this.tappingEnabled = true;
		}
	}

	private void DisableTapping()
	{
		if (this.tappingEnabled)
		{
            //GestureEventSystem.Instance.Tap -= new GestureEventSystem.GestureEventHandler(this.OnScreenTapped);
            //TouchManager.Instance.GesturesEnabled = this.oldGestureValue;
			this.tappingEnabled = false;
		}
	}

	private void HideAllButtons()
	{
        //this.OrText.SetActive(false);
        //this.TimeRemaining.transform.parent.gameObject.SetActive(false);
        //this.ButtonBank.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
        //this.BankParent.gameObject.SetActive(false);
        //this.ButtonNextRace.gameObject.SetActive(false);
        //this.Continue.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
	}

	private void Anim_Play()
	{
        //AnimationUtils.PlayAnim(base.animation, this.GetAnimationPrizeScreen());
	}

	private void Anim_Initialize()
	{
        //this.PrizeList.Anim_Initialize();
        //this.BankSequence.ShowEverything(false);
        //AnimationUtils.PlayFirstFrame(base.animation, this.GetAnimationPrizeScreen());
        //base.animation.Sample();
	}

	private void Anim_Finish_Prizes()
	{
        //AnimationUtils.PlayLastFrame(base.animation, this.GetAnimationPrizeScreen());
        //this.PrizeList.Anim_Finish();
	}

	private void Anim_PrizesAnimEnd()
	{
		this.Anim_Finish_Prizes();
		this.DisplayMultiplayerStreakPopupIfAppropriate();
		if (this.PrizesWon == 6)
		{
			if (!this.shouldDisplayWinStreakPopup)
			{
				this.DoBankSequence();
			}
		}
		else if (this.WonRace)
		{
			this.ShowRaceAndBankButtons();
		}
		else
		{
			this.ShowContinueButton();
		}
	}

	private void Anim_BankAnimEnd()
	{
        //RaceReCommon.FinishOnNextButtonRaceTheWorld(ScreenID.Streak);
	}

	private void Anim_LoseStreakAnimEnd()
	{
        //AnimationUtils.PlayLastFrame(base.animation, this.GetAnimationPrizeScreen());
        //if (GameDatabase.Instance.OnlineConfiguration.IsStreakRescueActive() && !SeasonFlowManager.Instance.EndOfRaceSeasonChangeTriggered && !StreakScreen.cancelledBrokenStreakRetry)
        //{
        //    this.DisplayStreakRetryPopup(true);
        //}
        //else
        //{
        //    this.ShowContinueButton();
        //}
	}

	public void DisplayStreakRetryPopup(bool withAdverts = true)
	{
        //StreakRescueCostData.StreakRescueCostType costType = StreakRescueCostData.StreakRescueCostType.COST_CASH;
        //int rescueCost = this.CalculateStreakRetryCost(withAdverts, out costType);
        //string arg = string.Empty;
        //string bodyText = string.Empty;
        //bool flag = false;
        //switch (costType)
        //{
        //case StreakRescueCostData.StreakRescueCostType.COST_CASH:
        //    arg = CurrencyUtils.GetColouredCashString(rescueCost);
        //    bodyText = string.Format(LocalizationManager.GetTranslation(this.GetStreakRescuePopupBodyStringID()), arg);
        //    break;
        //case StreakRescueCostData.StreakRescueCostType.COST_GOLD:
        //    arg = CurrencyUtils.GetColouredGoldString(rescueCost);
        //    bodyText = string.Format(LocalizationManager.GetTranslation(this.GetStreakRescuePopupBodyStringID()), arg);
        //    break;
        //case StreakRescueCostData.StreakRescueCostType.COST_AD:
        //    if (rescueCost == 0)
        //    {
        //        bodyText = LocalizationManager.GetTranslation("TEXT_POPUPS_STREAK_RESCUE_WITH_AD_BODY");
        //        flag = true;
        //    }
        //    else
        //    {
        //        arg = CurrencyUtils.GetColouredCashString(rescueCost);
        //        bodyText = string.Format(LocalizationManager.GetTranslation(this.GetStreakRescuePopupBodyStringID()), arg);
        //    }
        //    break;
        //}
        //if (flag)
        //{
        //    PopUp preAdPromptPopup = VideoForRewardsManager.Instance.GetPreAdPromptPopup(VideoForRewardConfiguration.eRewardID.WatchForStreakRescue);
        //    PopUp expr_FD = preAdPromptPopup;
        //    expr_FD.CancelAction = (PopUpButtonAction)Delegate.Combine(expr_FD.CancelAction, new PopUpButtonAction(this.OnBrokenStreakCancel));
        //    PopUpManager.Instance.TryShowPopUp(preAdPromptPopup, PopUpManager.ePriority.Default, null);
        //}
        //else
        //{
        //    PopUp popup = new PopUp
        //    {
        //        Title = "TEXT_POPUPS_STREAK_RESCUE_TITLE",
        //        BodyText = bodyText,
        //        BodyAlreadyTranslated = true,
        //        IsBig = true,
        //        CancelAction = new PopUpButtonAction(this.OnBrokenStreakCancel),
        //        ConfirmAction = delegate
        //        {
        //            this.OnBrokenStreakRetry(rescueCost, costType);
        //        },
        //        CancelText = "TEXT_BUTTON_MISS_OUT",
        //        ConfirmText = "TEXT_BUTTON_OK",
        //        ShouldCoverNavBar = true,
        //        BundledGraphicPath = PopUpManager.Instance.graphics_bloggerPrefab,
        //        ImageCaption = "TEXT_NAME_CONSUMABLES_PRAGENT"
        //    };
        //    PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
        //}
	}

	public void OnBrokenStreakRetry(int rescueCost, StreakRescueCostData.StreakRescueCostType costType)
	{
        //this.HideAllButtons();
        //bool flag = costType == StreakRescueCostData.StreakRescueCostType.COST_AD;
        //bool flag2 = false;
        //PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //if (rescueCost > 0)
        //{
        //    switch (costType)
        //    {
        //    case StreakRescueCostData.StreakRescueCostType.COST_CASH:
        //    case StreakRescueCostData.StreakRescueCostType.COST_AD:
        //        if (activeProfile.GetCurrentCash() < rescueCost)
        //        {
        //            MiniStoreController arg_7E_0 = MiniStoreController.Instance;
        //            PopUpButtonAction closeButtonAction = delegate
        //            {
        //                this.DisplayStreakRetryPopup(true);
        //            };
        //            arg_7E_0.ShowMiniStoreNotEnoughCash(new ItemTypeId("src"), new ItemCost
        //            {
        //                CashCost = rescueCost
        //            }, "TEXT_POPUPS_INSUFFICIENT_CASH_BODY_STREAK_RESCUE", null, closeButtonAction, null, null);
        //            return;
        //        }
        //        activeProfile.SpendCash(rescueCost);
        //        break;
        //    case StreakRescueCostData.StreakRescueCostType.COST_GOLD:
        //        if (activeProfile.GetCurrentGold() < rescueCost)
        //        {
        //            MiniStoreController.Instance.ShowMiniStoreNotEnoughGold(new ItemTypeId("src"), new ItemCost
        //            {
        //                GoldCost = rescueCost
        //            }, "TEXT_POPUPS_INSUFFICIENT_GOLD_BODY_STREAK_RESCUE", delegate
        //            {
        //                this.DisplayStreakRetryPopup(true);
        //            }, null, null);
        //            return;
        //        }
        //        flag2 = true;
        //        activeProfile.SpendGold(rescueCost);
        //        break;
        //    }
        //}
        //StreakManager.StreakWasRescued();
        //int num = (flag || !flag2) ? 0 : rescueCost;
        //int num2 = (!flag && !flag2) ? rescueCost : 0;
        //Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
        //{
        //    {
        //        Parameters.Type,
        //        "StreakRescue"
        //    },
        //    {
        //        Parameters.CostGold,
        //        num.ToString()
        //    },
        //    {
        //        Parameters.CostCash,
        //        num2.ToString()
        //    },
        //    {
        //        Parameters.DGld,
        //        (-num).ToString()
        //    },
        //    {
        //        Parameters.DCsh,
        //        (-num2).ToString()
        //    },
        //    {
        //        Parameters.CostAd,
        //        (!flag) ? "0" : "1"
        //    },
        //    {
        //        Parameters.StreakType,
        //        MultiplayerUtils.GetMultiplayerStreakType()
        //    }
        //};
        //Log.AnEvent(Events.MultiplayerPurchase, data);
        //Dictionary<Parameters, string> data2 = new Dictionary<Parameters, string>
        //{
        //    {
        //        Parameters.Rescue,
        //        "1"
        //    }
        //};
        //Log.AnEvent(Events.StreakRescue, data2);
        //StreakScreen.triggeredBrokenStreakRetry = true;
        //ScreenManager.Instance.RefreshTopScreen();
	}

	public void OnBrokenStreakCancel()
	{
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.Rescue,
				"0"
			},
            //{
            //    Parameters.StreakType,
            //    MultiplayerUtils.GetMultiplayerStreakType()
            //}
		};
		Log.AnEvent(Events.StreakRescue, data);
		cancelledBrokenStreakRetry = true;
		this.ShowContinueButton();
	}

	private int CalculateStreakRetryCost(bool withAdverts, out StreakRescueCostData.StreakRescueCostType costType)
	{
		return StreakManager.GetStreakRescueCost(PlayerProfileManager.Instance.ActiveProfile.IsConsumableActive(eCarConsumables.PRAgent), withAdverts, out costType);
	}

	private void OnInterstitialAdFinished(AdFinishedEventArgs args, object userData)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_STREAK_RESCUE_TITLE",
			BodyText = "TEXT_VIDEO_FOR_STREAK_RESCUE_POST_BODY",
			IsBig = true,
			ConfirmAction = delegate
			{
				this.OnBrokenStreakRetry(0, StreakRescueCostData.StreakRescueCostType.COST_AD);
			},
			ConfirmText = "TEXT_BUTTON_OK",
			ShouldCoverNavBar = true,
            GraphicPath = PopUpManager.Instance.graphics_bloggerPrefab,
			ImageCaption = "TEXT_NAME_CONSUMABLES_PRAGENT"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private void OnPausedAppAfterRescueAdTriggered()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUP_TITLE_VIDEO_AD_FAILED_USER_DIDNT_FINISH_TITLE",
			BodyText = LocalizationManager.GetTranslation("TEXT_POPUP_TITLE_VIDEO_AD_FAILED_USER_DIDNT_FINISH_BODY"),
			IsBig = true,
			BodyAlreadyTranslated = true,
			ConfirmAction = new PopUpButtonAction(this.OnBrokenStreakCancel),
			ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
			ImageCaption = "TEXT_NAME_AGENT"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private string GetStreakRescuePopupBodyStringID()
	{
		return (!PlayerProfileManager.Instance.ActiveProfile.IsConsumableActive(eCarConsumables.PRAgent)) ? "TEXT_POPUPS_STREAK_RESCUE_BODY" : "TEXT_POPUPS_STREAK_RESCUE_WITH_BLOGGER_BODY";
	}

	private void DoBankSequence()
	{
		this.HideAllButtons();
		this.Anim_Finish_Prizes();
		this.shouldAutoBank = true;
		this.Anim_Play();
		if (!StreakManager.HasBanked())
		{
			StreakManager.UpdateMetricsOnBankit();
			StreakManager.BankIt();
            CommonUI.Instance.CashStats.CashLockedState(true);
		}
	}

	private void Anim_RevealPrize(int prize)
	{
        //this.PrizeList.DoPrizeRevealAnimation(prize, true);
	}

	private void Anim_LosePrize(int prize)
	{
		int num = this.PrizesWon - 1 - prize;
		if (num >= 0)
		{
            //this.PrizeList.DoPrizeRevealAnimation(num, false);
			this.LoseStreakParticleSystem.Play();
            //AnimationUtils.PlayAnim(this.FlareFlickerAnimation);
		}
		else
		{
			this.Anim_EndStreakLoseParticles();
		}
	}

	private void Anim_StreakComplete()
	{
		if (this.PrizesWon == 6)
		{
            //this.Header.SetTextStreakComplete();
            //this.Header.PlayWonParticles();
		}
	}

	private void Anim_StreakLost()
	{
        //this.Header.SetTextStreakLost();
	}

	private void Anim_EndStreakLoseParticles()
	{
		this.LoseStreakParticleSystem.Stop();
		this.FlareFlickerAnimation.Stop();
        //AnimationUtils.PlayLastFrame(this.FlareFlickerAnimation);
	}

	public void Anim_LosePrizesAudio()
	{
        //AudioManager.Instance.PlaySound("StreakLoss", null);
	}

	public void Anim_StartWinStreakAudio()
	{
        //AudioManager.FadeIn("Audio_SFXAudio_WinStreakStart", 1f, 0.5f);
        //AudioManager.Instance.PlaySound("WinStreakStart", null);
	}

	private void Anim_ShowBankSequence()
	{
        //this.BankSequence.ShowEverything(true);
	}

	private void Anim_DoBankedCards()
	{
        //this.BankSequence.RevealBankedCards();
	}

	private void Anim_BankSequenceCountdown()
	{
        //this.BankSequence.StartCountdown();
	}

	private void Anim_AwardBankReward()
	{
		if (FAKE_DATA_FOR_TESTING)
		{
			PlayerProfileManager.Instance.ActiveProfile.AddCash(100000,"test","test");
		}
		else
		{
            //CommonUI.Instance.CashStats.CashLockedState(false);
		}
	}

	private void Anim_CameraShake_Banked()
	{
		GUICameraShake gUICameraShake = this.GetCameraShake();
		gUICameraShake.SetCurve(this.CameraShakeBanked);
		gUICameraShake.ShakeTime = Time.time;
		gUICameraShake.ShakingDuration = this.CameraShakeBankedDuration;
	}

	private void Anim_CameraShake_Streak_Lost()
	{
		GUICameraShake gUICameraShake = this.GetCameraShake();
		gUICameraShake.SetCurve(this.CameraShakeStreakLost);
		gUICameraShake.ShakeTime = Time.time;
		gUICameraShake.ShakingDuration = this.CameraShakeStreakLostDuration;
	}

	private void Anim_Fullscreen_Flash()
	{
        //this.Flash.StartFlashAnimation();
	}

	private void Anim_BankedParticles()
	{
        //this.BankSequence.BankedTextParticles.Play();
	}

	private void Anim_BankedCashParticles()
	{
        //this.BankSequence.BankedTotalTextParticles.Play();
	}

	private void Anim_PlayCashOutSound()
	{
        //AudioManager.Instance.PlaySound("CashOut", null);
	}

	private void DisplayMultiplayerStreakPopupIfAppropriate()
	{
		if (this.shouldDisplayWinStreakPopup && !this.displayedWinStreakPopup)
		{
			PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_WINSTREAK_TITLE_" + this.currentStreakNumber, "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_WINSTREAK_BODY_" + this.currentStreakNumber, new PopUpButtonAction(this.OnWinStreakTutorialPopupDismissed), false);
		}
		else if (this.shouldDisplayWinStreakLossPopup && !this.displayedWinStreakLostPopup)
		{
			PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_WINSTREAK_LOST_TITLE", "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_WINSTREAK_LOST_BODY", new PopUpButtonAction(this.OnLostWinStreakTutorialPopupDismissed), false);
			PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_LostWinStreakCompleted = true;
		}
	}

	private string GetAnimationPrizeScreen()
	{
		if (this.shouldAutoBank)
		{
			return "RespectRankingScreen_Prizes_Bank";
		}
		if (this.WonRace)
		{
			return "RespectRankingScreen_Prizes";
		}
		return "RespectRankingScreen_Prizes_Lost";
	}

	public override bool HasBackButton()
	{
		return FAKE_DATA_FOR_TESTING;
	}

	private GUICameraShake GetCameraShake()
	{
		if (this.cameraShake != null)
		{
			return this.cameraShake;
		}
        //this.cameraShake = GUICamera.Instance.gameObject.AddComponent<GUICameraShake>();
		return this.cameraShake;
	}
}
