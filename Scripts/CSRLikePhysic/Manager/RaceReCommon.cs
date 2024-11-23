using DataSerialization;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;

public class RaceReCommon : MonoBehaviour
{
    public TextMeshProUGUI MechanicText;

    //public DummyTextButton CentralButton;

	public bool WaitingForFuelAnim;

	public float WaitingForFuelAnimTime;

	public bool WaitingForGoldAnim;

	public float WaitingForGoldAnimTime;

	private int t1r34_countdown = 2;

	public static int RepeatedEventId = -1;

	public static int RepeatedEventLoseCount;

	private ScreenID NextScreenID;

	private bool _triggerOnNextButtonPostInterstitial;

	private bool _triggerOnNextButtonPostInterstitial_playerWon;

	private bool WaitForPopup;

	public static bool JustFettledEngines;

	public bool ShouldBlockLeaveScreen()
	{
		return this.WaitingForFuelAnim || this.WaitingForGoldAnim;
	}

	private bool FailureNag(bool playerWinner)
	{
		if (PopUpManager.Instance.isShowingPopUp)
		{
			return false;
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (CarDatabase.Instance.GetCar(activeProfile.CurrentlySelectedCarDBKey).UsesEvoUpgrades())
		{
			return false;
		}
		if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent() && activeProfile.IsCarOwned("MiniCooperS_RWF") && activeProfile.FriendRacesPlayed < 2 && activeProfile.FriendRacesWon == 0)
		{
			return false;
		}
		this.CheckHasLostThreeSameEvents(playerWinner);
		if (!PopUpManager.Instance.isShowingPopUp)
		{
			this.DoCrewLoseUpsell(playerWinner, new PopUpButtonAction(this.OnRestartCallback_Complete));
		}
		if (!PopUpManager.Instance.isShowingPopUp)
		{
			this.DoProgressionNag(playerWinner);
		}
		if (!PopUpManager.Instance.isShowingPopUp)
		{
			this.DoUpgradeNag(playerWinner);
		}
		if (!PopUpManager.Instance.isShowingPopUp)
		{
			this.DoMechanicNag(playerWinner);
		}
		if (PopUpManager.Instance.isShowingPopUp || this.WaitForPopup)
		{
			this.WaitForPopup = false;
			return true;
		}
		return false;
	}

	public void ResetNags()
	{
		if (RaceResultsTracker.You.IsWinner)
		{
			PlayerProfileManager.Instance.ActiveProfile.ContiguousProgressionLosses = 0;
			PlayerProfileManager.Instance.ActiveProfile.ContiguousLosses = 0;
		}
	}

	private bool DoCrewLoseUpsell(bool playerWon, PopUpButtonAction OnIgnore)
	{
		if (playerWon)
		{
			return false;
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return false;
		}
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent == null || currentEvent.Parent == null || currentEvent.Group == null)
		{
			return false;
		}
		if (currentEvent.EventID != 536 && currentEvent.EventID != 537)
		{
			return false;
		}
		if (this.t1r34_countdown <= 0)
		{
			return false;
		}
        if (MultiplayerUtils.IsPlayingMultiplayer())
        {
            return false;
        }
		if (activeProfile.HasPaidForSomething)
		{
			return false;
		}
		this.t1r34_countdown--;
		PopUp popup = this.upsellShop_PopUp(OnIgnore);
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		this.WaitForPopup = true;
		return true;
	}

	public void DoProgressionNag(bool playerWon)
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return;
		}
		if (PlayerProfileManager.Instance.ActiveProfile.ContiguousProgressionLossesTriggered >= 2)
		{
			return;
		}
        if (MultiplayerUtils.IsPlayingMultiplayer())
        {
            return;
        }
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent == null || currentEvent.Parent == null || currentEvent.Group == null)
		{
			return;
		}
		if (!currentEvent.IsLadderEvent() && !currentEvent.IsCrewBattle())
		{
			return;
		}
		PlayerProfileManager.Instance.ActiveProfile.ContiguousProgressionLosses++;
		if (PlayerProfileManager.Instance.ActiveProfile.ContiguousProgressionLosses < 3)
		{
			return;
		}
		if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
		{
			CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
			if (currentCar.GetIsFullyFitted())
			{
				return;
			}
		}
		PopUp popup = this.goProgression_Popup(playerWon);
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		PlayerProfileManager.Instance.ActiveProfile.ContiguousProgressionLosses = 0;
		PlayerProfileManager.Instance.ActiveProfile.ContiguousProgressionLossesTriggered++;
		this.WaitForPopup = true;
	}

	public void DoUpgradeNag(bool playerWon)
	{
		if (PlayerProfileManager.Instance.ActiveProfile != null)
		{
			PlayerProfileManager.Instance.ActiveProfile.ContiguousLosses++;
			RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
			if (currentEvent == null || currentEvent.Parent == null)
			{
				return;
			}
			if (RaceEventQuery.Instance.getHighestUnlockedClass() != eCarTier.TIER_1)
			{
				return;
			}
			if (PlayerProfileManager.Instance.ActiveProfile.ContiguousLosses < 3 || PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash() < 1000)
			{
				return;
			}
            if (MultiplayerUtils.IsPlayingMultiplayer())
            {
                return;
            }
			CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
			if (currentCar.GetIsFullyUpgraded())
			{
				return;
			}
			bool flag = false;
			if (currentEvent != null)
			{
				flag = !currentEvent.IsDailyBattle();
			}
			int currentPPIndex = currentCar.CurrentPPIndex;
			CarPerformanceIndexData carPPData = GameDatabase.Instance.CarsConfiguration.CarPPData;
			bool flag2;
			if (currentPPIndex > carPPData.Tier5LowerLimit)
			{
				flag2 = ((float)currentPPIndex < (float)carPPData.Tier5HigherLimit - (float)(carPPData.Tier5HigherLimit - carPPData.Tier5LowerLimit) / 10f);
			}
			else if (currentPPIndex > carPPData.Tier4LowerLimit)
			{
				flag2 = ((float)currentPPIndex < (float)carPPData.Tier4HigherLimit - (float)(carPPData.Tier4HigherLimit - carPPData.Tier4LowerLimit) / 10f);
			}
			else if (currentPPIndex > carPPData.Tier3LowerLimit)
			{
				flag2 = ((float)currentPPIndex < (float)carPPData.Tier3HigherLimit - (float)(carPPData.Tier3HigherLimit - carPPData.Tier3LowerLimit) / 10f);
			}
			else if (currentPPIndex > carPPData.Tier2LowerLimit)
			{
				flag2 = ((float)currentPPIndex < (float)carPPData.Tier2HigherLimit - (float)(carPPData.Tier2HigherLimit - carPPData.Tier2LowerLimit) / 10f);
			}
			else
			{
				flag2 = ((float)currentPPIndex < (float)carPPData.Tier1HigherLimit - (float)(carPPData.Tier1HigherLimit - carPPData.Tier1LowerLimit) / 10f);
			}
			if (flag && flag2)
			{
				PopUp popup = this.goUpgrade_Popup();
				PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
				PlayerProfileManager.Instance.ActiveProfile.ContiguousLosses = 0;
				this.WaitForPopup = true;
			}
		}
	}

	public bool TryMechanicIntro(bool playerWon)
	{
	    return false;
		//if (!RaceCameraFinishLine.closeRace)
		//{
		//	return false;
		//}
		//if (playerWon)
		//{
		//	return false;
		//}
		//if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstUpgrade)
		//{
		//	return false;
		//}
		//if (PlayerProfileManager.Instance.ActiveProfile.HasHadMechanicSlowMotionIntroduction)
		//{
		//	return false;
		//}
		//if (PlayerProfileManager.Instance.ActiveProfile.MechanicTuningRacesRemaining > 0)
		//{
		//	return false;
		//}
		//if (!RaceEventInfo.Instance.ShouldCurrentEventUseMechanic())
		//{
		//	return false;
		//}
		//if (RaceEventInfo.Instance.CurrentEvent.IsHighStakesEvent())
		//{
		//	return false;
		//}
		//if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
		//{
		//	return false;
		//}
		//PlayerProfileManager.Instance.ActiveProfile.HasHadMechanicSlowMotionIntroduction = true;
		//PopUp popup = new PopUp
		//{
		//	Title = "TEXT_MECHANIC_POST_RACE_INTRO_TITLE",
		//	BodyText = "TEXT_MECHANIC_POST_RACE_INTRO_BODY",
		//	IsBig = true,
		//	CancelAction = new PopUpButtonAction(this.FinishOnNextButton),
		//	ConfirmAction = delegate
		//	{
		//		this.OnMechanicButton(playerWon);
		//	},
		//	CancelText = "TEXT_MECHANIC_POST_RACE_CANCEL",
		//	ConfirmText = "TEXT_MECHANIC_POST_RACE_CONFIRM",
  //          GraphicPath = PopUpManager.Instance.graphics_mechanicPrefab,
		//	ImageCaption = "TEXT_NAME_MECHANIC"
		//};
		//PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		//return true;
	}

	public void DoMechanicNag(bool playerWon)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return;
		}
		if (activeProfile.MechanicTuningRacesRemaining > 0)
		{
			return;
		}
		if (!activeProfile.HaveShownFirstMechanicPopUp)
		{
			return;
		}
		if (Random.Range(1, 5) > 2)
		{
			return;
		}
		if (!RaceEventInfo.Instance.ShouldCurrentEventUseMechanic())
		{
			return;
		}
		PopUp popup = this.goMechanic_PopUp(playerWon);
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		this.WaitForPopup = true;
	}

	public void CheckHasLostThreeSameEvents(bool playerWon)
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent == null)
		{
			return;
		}
		if (RaceEventDifficulty.Instance.GetRating(currentEvent, false) == RaceEventDifficulty.Rating.Easy)
		{
			return;
		}
		if (currentEvent.EventID != RepeatedEventId)
		{
			RepeatedEventId = currentEvent.EventID;
			RepeatedEventLoseCount = 0;
			return;
		}
        if (MultiplayerUtils.IsPlayingMultiplayer())
        {
            return;
        }
		if (currentEvent.IsFriendRaceEvent())
		{
			return;
		}
		RepeatedEventLoseCount++;
		if (RepeatedEventLoseCount > 2)
		{
			this.go_repeatedLossPopup(playerWon);
			this.WaitForPopup = true;
			RepeatedEventLoseCount = 0;
		}
	}

	public static bool IsAfterRaceMechanicAvailable()
	{
		return PlayerProfileManager.Instance.ActiveProfile.MechanicIsUnlocked && !RaceEventInfo.Instance.IsDailyBattleEvent;
	}

	public void SetButtonFunctionality()
	{
		this.SetMechanicText();
		this.Update();
	}

	private void Update()
	{
		if (this.WaitingForFuelAnim)
		{
			this.WaitingForFuelAnimTime -= Time.deltaTime;
			if (this.WaitingForFuelAnimTime <= 0f)
			{
				this.ActuallyRestartTheRace();
			}
		}
		if (this._triggerOnNextButtonPostInterstitial)
		{
			this._triggerOnNextButtonPostInterstitial = false;
			this.OnNextButtonPostInterstitial(this._triggerOnNextButtonPostInterstitial_playerWon);
		}
	}

	private void SetMechanicText()
	{
        if (this.MechanicText == null)
        {
            return;
        }
	    if (!RaceReCommon.IsAfterRaceMechanicAvailable())// && this.CentralButton != null)
        {
            //this.CentralButton.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
            this.MechanicText.gameObject.SetActive(false);
            return;
        }
        if (RaceReCommon.JustFettledEngines)
        {
            string text = string.Empty;
            int mechanicTuningRacesRemaining = PlayerProfileManager.Instance.ActiveProfile.MechanicTuningRacesRemaining;
            if (mechanicTuningRacesRemaining > 0)
            {
                if (mechanicTuningRacesRemaining != 1)
                {
                    string format = LocalizationManager.GetTranslation("TEXT_X_FETTLED_RACES_REMAINING_UNTIL_YOU_SHOULD_TUNE");
                    text = string.Format(format, mechanicTuningRacesRemaining);
                }
                else
                {
                    text = LocalizationManager.GetTranslation("TEXT_ONE_FETTLED_RACE_REMAINING_UNTIL_YOU_SHOULD_TUNE");
                }
                //if (this.CentralButton != null)
                //{
                //    this.CentralButton.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
                //}
                UnityEngine.Vector3 localPosition = this.MechanicText.transform.localPosition;
                this.MechanicText.transform.localPosition = localPosition;
                this.MechanicText.gameObject.SetActive(true);
            }
            else
            {
                this.MechanicText.gameObject.SetActive(false);
            }
            this.MechanicText.text = text;
        }
        else
        {
            this.MechanicText.gameObject.SetActive(false);
        }
	}

	private void FirstCrewNag()
	{
		if (RaceEventInfo.Instance.CurrentEvent != null && RaceResultsTracker.You.IsWinner && !PopUpManager.Instance.isShowingPopUp && RaceEventInfo.Instance.CurrentEvent.IsFirstCrewMemberRace())
		{
			RateTheAppNagger.TryShowPrompt(RateTheAppTrigger.BEATCREWMEMBER);
		}
	}

	private bool TryMechanicForce()
	{
		int mechanicTuningRacesRemaining = PlayerProfileManager.Instance.ActiveProfile.MechanicTuningRacesRemaining;
		if (RaceEventInfo.Instance.IsCrewRaceEvent)
		{
			return false;
		}
		if (!RaceEventInfo.Instance.ShouldCurrentEventUseMechanic())
		{
			return false;
		}
		if (PlayerProfileManager.Instance.ActiveProfile.RacesWon < 14)
		{
			return false;
		}
        if (MultiplayerUtils.IsPlayingMultiplayer())
        {
            return false;
        }
        //if (!PlayerProfileManager.Instance.ActiveProfile.HaveShownFirstMechanicPopUp && mechanicTuningRacesRemaining <= 0)
        //{
        //    PlayerProfileManager.Instance.ActiveProfile.HaveShownFirstMechanicPopUp = true;
        //    Log.AnEvent(Events.MechanicTune);
        //    PopUpManager.Instance.TryShowPopUp(AdhocTutorials.FettleEngine(new PopUpButtonAction(this.FinishOnNextButton)), PopUpManager.ePriority.Default, null);
        //    return true;
        //}
		return false;
	}

	private bool TryCustomiseCarNag(bool playerWon)
	{
		if (RaceEventInfo.Instance.CurrentEvent.IsTestDriveAndCarSetup())
		{
			return false;
		}
		if (RaceEventInfo.Instance.IsDailyBattleEvent)
		{
			return false;
		}
		if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
		{
			return false;
		}
		CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
		if (RaceEventInfo.Instance.IsCrewRaceEvent)
		{
			return false;
		}
        if (MultiplayerUtils.IsPlayingMultiplayer())
        {
            return false;
        }
		if (!PlayerProfileManager.Instance.ActiveProfile.HasCompletedFirstCrewRace())
		{
			return false;
		}
		if (RaceEventQuery.Instance.IsTierUnlocked(eCarTier.TIER_X))
		{
			return false;
		}
        //if (!string.IsNullOrEmpty(currentCar.AppliedLiveryName) || currentCar.NumberPlate.Text != string.Empty)
        //{
        //    return false;
        //}
		if (currentCar.CustomCarNags >= 2)
		{
			return false;
		}
		if (currentCar.CustomCarNags == 1)
		{
			bool flag = false;
			foreach (eUpgradeType current in CarUpgrades.ValidUpgrades)
			{
				CarUpgradeStatus carUpgradeStatus = currentCar.UpgradeStatus[current];
				if (carUpgradeStatus.levelOwned == 3)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		//string carDBKey = currentCar.CarDBKey;
		//string value = carDBKey + "_Livery";
		bool hasyAnyLivery = false;
        //List<AssetDatabaseAsset> assetsOfType = AssetDatabaseClient.Instance.Data.GetAssetsOfType(CSRAssetTypes.livery);
        //foreach (AssetDatabaseAsset current2 in assetsOfType)
        //{
        //    string code = current2.code;
        //    if (code.StartsWith(value))
        //    {
        //        if (code != CarDatabase.Instance.GetCar(carDBKey).DefaultLiveryBundleName)
        //        {
        //            if (!code.Contains("Tier"))
        //            {
        //                hasyAnyLivery = true;
        //                break;
        //            }
        //        }
        //    }
        //}
		if (!hasyAnyLivery)
		{
			return false;
		}
		currentCar.CustomCarNags++;
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_NAG_CUSTOMISECAR_TITLE",
			BodyText = "TEXT_POPUPS_NAG_CUSTOMISECAR_BODY",
			IsBig = true,
			CancelAction = new PopUpButtonAction(this.FinishOnNextButton),
			ConfirmAction = delegate
			{
				this.GoCustomise(playerWon);
			},
			CancelText = "TEXT_BUTTON_MISS_OUT",
			ConfirmText = "TEXT_BUTTON_CUSTOMIZE",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
			ImageCaption = "TEXT_NAME_AGENT"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		return true;
	}

	public bool ReplaceAdvanceWithLevelUpCheck()
	{
		if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent())
		{
			return false;
		}
		if (!CommonUI.Instance.XPStats.IsLevelUpLocked())
		{
			return false;
		}
		CommonUI.Instance.XPStats.XPLockedState(false);
        CommonUI.Instance.XPStats.LevelUpLockedState(false);
        CommonUI.Instance.StarLeagueStats.StarLockedState(false);
        CommonUI.Instance.StarStats.StarLockedState(false);
        CommonUI.Instance.StarStats.NewLeagueLockedState(false);
		return true;
	}

	public void OnNextButton(bool playerWon)
	{
		if (this.ShouldBlockLeaveScreen())
		{
			return;
		}
		if (this.ReplaceAdvanceWithLevelUpCheck())
		{
			return;
		}
		this.NextScreenID = ScreenID.Invalid;
		if (ScreenManager.Instance.CurrentScreen == ScreenID.RaceRewards)
		{
            if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent())
            {
                //if (GameDatabase.Instance.OnlineConfiguration.IsStreakRescueActive() || StreakManager.CurrentStreak() > 0)
                //{
                //    ScreenManager.Instance.PushScreen(ScreenID.Streak);
                //    return;
                //}
                //ScreenManager.Instance.PushScreen(ScreenID.RespectRanking);
                return;
            }
            else if (RaceEventInfo.Instance.CurrentEvent.IsRelay)
            {
                //ScreenManager.Instance.PopToScreen(ScreenID.Workshop);
                //CrewProgressionSetup.PostRaceSetupForNarrativeScene(ScreenID.Invalid);
                return;
            }
		}
        //NarrativeScene narrativeScene;
        //if (((playerWon && !RaceEventInfo.Instance.CurrentEvent.GetPostRaceSceneWinToDisplay(out narrativeScene)) || (!playerWon && !RaceEventInfo.Instance.CurrentEvent.GetPostRaceSceneLoseToDisplay(out narrativeScene))) && CSRAdManager.Instance.TriggerAdvert(AdManager.AdSpace.Default, delegate(AdFinishedEventArgs x, object y)
        //{
        //    this.TriggerOnNextButtonPostInterstitial(playerWon);
        //}, null))
        //{
        //    return;
        //}
		this.OnNextButtonPostInterstitial(playerWon);
	}

	private void TriggerOnNextButtonPostInterstitial(bool playerWon)
	{
		this._triggerOnNextButtonPostInterstitial_playerWon = playerWon;
		this._triggerOnNextButtonPostInterstitial = true;
	}

	private void OnNextButtonPostInterstitial(bool playerWon)
	{
        if (this.NextScreenID != ScreenID.Invalid)
        {
            ScreenManager.Instance.PushScreen(this.NextScreenID);
        }
		if (this.OnExitScreenTakeControl())
		{
			return;
		}
		if (this.TryMechanicForce())
		{
			return;
		}
		if (this.TryCustomiseCarNag(playerWon))
		{
			return;
		}
		if (this.TryMechanicIntro(playerWon))
		{
			return;
		}
		if (this.DoCrewLoseUpsell(playerWon, new PopUpButtonAction(this.FinishOnNextButton)))
		{
			return;
		}
		this.FinishOnNextButton();
	}

	public bool OnExitScreenTakeControl()
	{
		if (RaceEventInfo.Instance.CurrentEvent == null || !RaceEventInfo.Instance.CurrentEvent.IsHighStakesEvent())
		{
			return false;
		}
        //SceneManagerFrontend.SetScreenToPushWhenInFrontend(ScreenID.HighStakesFinished);
		RaceController.Instance.BackToFrontend();
		return true;
	}

	public void FinishOnNextButton()
	{
		GoBackToFrontEnd();
	}

	public static void StartBankFlow()
	{
        ScreenManager.Instance.PushScreen(ScreenID.Streak);
	}

	public static void FinishOnNextButtonRaceTheWorld(ScreenID currentScreen)
	{
		if (PlayerProfileManager.Instance == null || PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return;
		}
		//bool flag = false;
		if (currentScreen == ScreenID.Streak)
		{
			if (StreakManager.HasBanked())
			{
                //if (StreakManager.CardsBank() > 0)
                //{
                //    ScreenManager.Instance.PushScreen(ScreenID.PrizeOMatic);
                //    return;
                //}
				//flag = true;
			}
			else if (!RaceResultsTracker.You.IsWinner)
			{
				//flag = true;
			}
		}
		else if (currentScreen == ScreenID.PrizeOMatic)
		{
			//flag = true;
		}
        //if (SeasonFlowManager.Instance.EndOfRaceSeasonChangeTriggered)
        //{
        //    flag = false;
        //}
        //if (flag)
        //{
        //    ScreenManager.Instance.PushScreen(ScreenID.RespectRanking);
        //    return;
        //}
        //if (currentScreen == ScreenID.RespectRanking || flag || SeasonFlowManager.Instance.EndOfRaceSeasonChangeTriggered)
        //{
        //    if (!StreakManager.HasBanked() && !RaceResultsTracker.You.IsWinner)
        //    {
        //        StreakManager.LoseStreak();
        //    }
        //    if (PrizeOMaticScreen.DidAwardCarPartLastTimeOnScreen && StreakManager.CardsBank() > 0)
        //    {
        //        RaceReCommon.DoPrizeCarSequence();
        //        return;
        //    }
        //    if (StreakManager.Chain.OfferStreakChainIfAvailable(delegate
        //    {
        //        SceneManagerFrontend.SetScreenToPushWhenInFrontend(ScreenID.PlayerList);
        //        RaceReCommon.GoBackToFrontEnd();
        //    }, delegate
        //    {
        //        RaceReCommon.FinishOnNextButtonRaceTheWorld(ScreenID.RespectRanking);
        //    }))
        //    {
        //        return;
        //    }
        //}
        //if (StreakManager.OpponentListIsEmpty() || StreakManager.AllOpponentsDefeated())
        //{
        //    MultiplayerUtils.GoToMultiplayerHubScreenWhenInFrontend();
        //    StreakManager.ResetBank();
        //}
        //else
        //{
        //    SceneManagerFrontend.SetScreenToPushWhenInFrontend(ScreenID.PlayerList);
        //}
        //if (SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Frontend)
        //{
        //    CSRAdManager.Instance.PrepareAdForDefaultInterstitialFlow(true);
        //    RaceReCommon.GoBackToFrontEnd();
        //}
        //else
        //{
        //    MultiplayerUtils.GoToMultiplayerHubScreen();
        //}
	}

	private static void DoPrizeCarSequence()
	{
        //MultiplayerUtils.InPrizeCarSequence = true;
        //if (SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Frontend)
        //{
        //    SceneManagerFrontend.SetScreenToPushWhenInFrontend(ScreenID.PrizePieceGive);
        //    RaceReCommon.GoBackToFrontEnd();
        //}
        //else
        //{
        //    MultiplayerUtils.GoToMultiplayerHubScreen();
        //    ScreenManager.Instance.PushScreen(ScreenID.PrizePieceGive);
        //}
	}

	public static void GoBackToFrontEnd()
	{
		if (SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend)
		{
			return;
		}
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent.IsRelay)
		{
            //CareerModeMapScreen.mapPaneSelected = 5;
			SceneManagerFrontend.SetScreenToPushWhenInFrontend(ScreenID.CareerModeMap);
		}
		else if (currentEvent.IsFriendRaceEvent())
		{
            //if (string.IsNullOrEmpty(FriendsRewardManager.Instance.CarToAwardID))
            //{
            //    CareerModeMapScreen.mapPaneSelected = -2;
            //    SceneManagerFrontend.SetScreenToPushWhenInFrontend(ScreenID.CareerModeMap);
            //}
            //else
            //{
            //    CareerModeMapScreen.mapPaneSelected = 0;
            //}
		}
		else
		{
            PinDetail worldTourPinPinDetail = currentEvent.GetWorldTourPinPinDetail();
            if (worldTourPinPinDetail != null && worldTourPinPinDetail.WorldTourScheduledPinInfo != null && worldTourPinPinDetail.WorldTourScheduledPinInfo.GetScreenToPushAfterResult() != ScreenID.Invalid)
            {
                SceneManagerFrontend.SetScreenToPushWhenInFrontend(worldTourPinPinDetail.WorldTourScheduledPinInfo.GetScreenToPushAfterResult());
            }
		}
		RaceController.Instance.BackToFrontend();
	}

	private void GotoFrontendScreen(bool playerWon, ScreenID zScreenID)
	{
		if (SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend)
		{
            ScreenManager.Instance.PopToScreen(ScreenID.Workshop);
		    ScreenManager.Instance.PushScreen(zScreenID);
			this.OnNextButtonPostInterstitial(playerWon);
		}
		else
		{
            SceneManagerFrontend.SetScreenToPushWhenInFrontend(zScreenID);
			this.OnNextButtonPostInterstitial(playerWon);
		}
	}

	public void GoCustomise(bool playerWon)
	{
		this.GotoFrontendScreen(playerWon, ScreenID.BodyPaint);
	}

	public void OnMechanicButton(bool playerWon)
	{
		this.GotoFrontendScreen(playerWon, ScreenID.Mechanic);
	}

	public void GoUpgrade(bool playerWon)
	{
		this.GotoFrontendScreen(playerWon, ScreenID.Tuning);
	}

	public void OnRetry(bool playerWinner)
	{
		if (this.ShouldBlockLeaveScreen())
		{
			return;
		}
		if (this.ReplaceAdvanceWithLevelUpCheck())
		{
			return;
		}
		if (!this.FailureNag(playerWinner))
		{
			this.OnRestartCallback_Complete();
		}
	}

	public void OnRestartCallback_Complete()
	{
		this.TakeRestartFuelCost();
	}

	public void TakeRestartFuelCost()
	{
		if (!FuelManager.Instance.SpendFuel(GameDatabase.Instance.Currencies.GetFuelCostForEvent(RaceEventInfo.Instance.CurrentEvent)))
		{
			PopUpDatabase.Common.ShowGoGetFuelPopUp(new PopUpButtonAction(this.GoGetFuel));
		}
		else
		{
			this.WaitingForFuelAnim = true;
			this.WaitingForFuelAnimTime = 1.8f;
			if (JustFettledEngines)
			{
				PlayerProfileManager.Instance.ActiveProfile.MechanicTuningRacesRemaining++;
			}
		}
	}

	public void GoGetFuel()
	{
        ScreenManager.Instance.PushScreen(ScreenID.GetMoreFuel);
	}

	public void ActuallyRestartTheRace()
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent.IsRelay)
		{
			RelayManager.ResetRelayData();
			RaceEventData zRaceEventData = currentEvent.Group.RaceEvents[0];
			RaceEventInfo.Instance.PopulateFromRaceEvent(zRaceEventData);
            ScreenManager.Instance.PopToScreen(ScreenID.CareerModeMap);
            ScreenManager.Instance.PushScreen(ScreenID.RelayResults);
		}
		else
		{
			MenuAudio.Instance.fadeMusic(1f, 0.3f);
			this.OnRestartEvent(currentEvent);
		}
	}

	public static void OnPlayerProfileActivate()
	{
        LocalPlayerInfo localPlayerInfo = new LocalPlayerInfo();
        //MultiplayerProfileScreen.LocalPlayerInfo = localPlayerInfo;
        //MultiplayerProfileScreen.isOwnProfile = true;
	}

	private void OnStartEvent(RaceEventData zRaceEventData)
	{
		RaceController.Instance.Machine.SetState(RaceStateEnum.exit);
		SceneLoadManager.Instance.RequestScene(SceneLoadManager.Scene.Race);
		RaceEventInfo.Instance.PopulateFromRaceEvent(zRaceEventData);
	}

	public void OnRestartEvent(RaceEventData zRaceEventData)
	{
		RaceController.Instance.ResetRace();
	}

	private PopUp goUpgrade_Popup()
	{
		return new PopUp
		{
			Title = "TEXT_POPUPS_ADVICE_TITLE",
			BodyText = "TEXT_POPUPS_ADVICE_UPGRADE",
			IsBig = true,
			CancelAction = new PopUpButtonAction(this.OnRestartCallback_Complete),
			ConfirmAction = new PopUpButtonAction(this.goToUpgrade),
			CancelText = "TEXT_BUTTON_NO",
			ConfirmText = "TEXT_BUTTON_UPGRADE",
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
	}

	public void goToUpgrade()
	{
		RaceController.Instance.BackToFrontend();
		SceneManagerFrontend.SetScreenToPushWhenInFrontend(ScreenID.Tuning);
	}

	private PopUp goProgression_Popup(bool playerWon)
	{
		return new PopUp
		{
			Title = "TEXT_POPUPS_ADVICE_TITLE",
			BodyText = "TEXT_POPUPS_ADVICE_GARAGE",
			IsBig = true,
			CancelAction = new PopUpButtonAction(this.OnRestartCallback_Complete),
			ConfirmAction = delegate
			{
				this.goToGarage(playerWon);
			},
			CancelText = "TEXT_BUTTON_NO",
			ConfirmText = "TEXT_BUTTON_GARAGE",
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
	}

	private PopUp goMechanic_PopUp(bool playerWon)
	{
		return new PopUp
		{
			Title = "TEXT_POPUPS_ADVICE_TITLE",
			BodyText = "TEXT_POPUPS_ADVICE_MECHANIC",
			IsBig = true,
			CancelAction = new PopUpButtonAction(this.OnRestartCallback_Complete),
			ConfirmAction = delegate
			{
				this.goToMechanic(playerWon);
			},
			CancelText = "TEXT_BUTTON_NO",
			ConfirmText = "TEXT_BUTTON_MECHANIC",
			ImageCaption = "TEXT_NAME_AGENT",
			GraphicPath = PopUpManager.Instance.graphics_agentPrefab
		};
	}

	public PopUp upsellShop_PopUp(PopUpButtonAction OnIgnore)
	{
		return new PopUp
		{
			Title = "TEXT_POPUPS_UPSELLGOLD_T1R34_TITLE",
			BodyText = "TEXT_POPUPS_UPSELLGOLD_T1R34_BODY",
			IsBig = true,
			CancelAction = OnIgnore,
			ConfirmAction = new PopUpButtonAction(this.goToShop),
			CancelText = "TEXT_BUTTON_NO",
			ConfirmText = "TEXT_BUTTON_SHOP",
			ImageCaption = "TEXT_NAME_AGENT",
			GraphicPath = PopUpManager.Instance.graphics_agentPrefab
		};
	}

	public void go_repeatedLossPopup(bool playerWon)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_LOSENAG_THREEINROW_TITLE",
			BodyText = "TEXT_LOSENAG_THREEINROW_BODY",
			IsBig = true,
			CancelAction = new PopUpButtonAction(this.OnRestartCallback_Complete),
			ConfirmAction = delegate
			{
				this.goToGarage(playerWon);
			},
			CancelText = "TEXT_BUTTON_NO",
			ConfirmText = "TEXT_BUTTON_OK",
			ImageCaption = "TEXT_NAME_AGENT",
			GraphicPath = PopUpManager.Instance.graphics_agentPrefab
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public void goToGarage(bool playerWon)
	{
		this.GotoFrontendScreen(playerWon, ScreenID.Workshop);
	}

	public void goToMechanic(bool playerWon)
	{
        this.GotoFrontendScreen(playerWon, ScreenID.Mechanic);
	}

	public void goToShop()
	{
        SceneManagerFrontend.SetScreenToPushWhenInFrontend(ScreenID.ShopOverview);
		RaceController.Instance.BackToFrontend();
	}
}
