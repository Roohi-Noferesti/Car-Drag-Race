using Metrics;
using System;
using System.Collections.Generic;
using System.Globalization;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class PrizeOMaticScreen : ZHUDScreen
{
	public enum ScreenState
	{
		WaitingForDeal,
		Dealing,
		WaitingForTap,
		ShowingRewards,
		WaitingToShowPopUp,
		ShowingPopUp,
		Complete,
		Buying,
		Finished
	}

	public const int MAX_NO_OF_CARDS = 8;

	private const float BELOW_SCREEN = -2.62f;

	private const float POP_UP_DELAY = 0.3f;

	private const int MAX_CARD_AUDIO_NUMBER = 8;

	private const int CARDS_ACROSS = 4;

	private const string PRIZE_WON_AUDIO = "PrizeomaticPrizeWon";

	private const float WIDTH_OF_BACKGROUND_SPRITE = 1.2f;

	private const float HEIGHT_OF_BACKGROUND_SPRITE = 1.39f;

	private const float HEIGHT_OF_IPAD_SCREEN = 3.84f;

	private const float DEFAULT_SCALE_OF_CARD = 1.2f;

	public static bool DidAwardCarPartLastTimeOnScreen = false;

	public static bool IsTestRunning = false;

	public static PrizeOMaticScreen.ScreenState screenState;

	private static List<PrizeOMaticCard> ListOfCardsAwarded = new List<PrizeOMaticCard>();

	private int selectedCardIndex;

	private float timeSinceCardFlip;

	//private float xAxisOffset;

	//private float heightOffScreen;

	private int currentCard;

	public GameObject CardsHolder;

	public GameObject BottomOfScreen;

	public GameObject BottomCentre;

	public GameObject LeftOfScreen;

	public GameObject RightOfScreen;

	public GameObject BackGround;

	public GameObject TopOfScreen;

	public GameObject TopRow;

	public GameObject BottomRow;

    //public global::Sprite BottomFade;

	public RaceReCommon Common;

    public TextMeshProUGUI WaitText;

    public Button m_closeButton;

    public Sprite normalCardSprite;
    public Sprite bonusCardSprite;

    //public SpriteText TextTapToStop;

    public static bool ScreenHasBeenTapped;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.PrizeOMatic;
		}
	}

	public static List<PrizeOMaticCard> LastCardsAwarded
	{
		get
		{
			return PrizeOMaticScreen.ListOfCardsAwarded;
		}
	}

	public static bool IsAwardingPrize
	{
		get
		{
			return PrizeOMaticScreen.screenState != PrizeOMaticScreen.ScreenState.WaitingForTap;
		}
	}

    private PrizeOMaticScreen.ScreenState ChangeState(PrizeOMaticScreen.ScreenState state)
    {
        switch (state)
        {
            case PrizeOMaticScreen.ScreenState.WaitingForTap:
                this.SetTapText(true);
                break;
            case PrizeOMaticScreen.ScreenState.ShowingRewards:
                //AudioManager.Instance.PlaySound("PrizeomaticPrizeWon", null);
                break;
            case PrizeOMaticScreen.ScreenState.ShowingPopUp:
                this.SetTapText(false);
                break;
            case PrizeOMaticScreen.ScreenState.Finished:
                this.UnlockNavBarAnimations();
                break;
        }
        return state;
    }

    //protected override void OnScreenVisibilityChanged(HudScreenEventArgs obj)
    //{
    //    base.OnScreenVisibilityChanged(obj);
    //    if (obj.Visible)
    //    {
    //        OnActivate(obj.TargetScreenWasVisibleBefore);
    //    }
    //}

    public override void OnActivate(bool zAlreadyOnStack)
    {
        PrizeomaticController.Create();
        base.OnActivate(zAlreadyOnStack);
        PrizeOMaticScreen.screenState = PrizeOMaticScreen.ScreenState.WaitingForDeal;
        if (!zAlreadyOnStack)
        {
            //if (StreakManager.CardsBank() <= 8 && StreakManager.CardsBank() > 0)
            //{
            //    PrizeomaticController.Instance.numOfAttempts = StreakManager.CardsBank();
            //}
            //else
            //{
            //    PrizeomaticController.Instance.numOfAttempts = 3;
            //}
            PrizeomaticController.Instance.numOfCarPieces = 0;
            PrizeomaticController.Instance.numOfAttempts = 2;//PlayerProfileManager.Instance.ActiveProfile.NumberOfPrizeCardRemaining;
            if(GTAdManager.Instance.CanShowAdForExtraReward()) 
	            PrizeomaticController.Instance.numOfBonusAttempts = 2;
            else 
	            PrizeomaticController.Instance.numOfBonusAttempts = 0;
            WaitText.text = LocalizationManager.GetTranslation("TEXT_PRIZEOMATIC_SCREEN_TITLE");
            foreach (var card in PrizeomaticController.Instance.cards)
            {
	            card.SetSprite(normalCardSprite);
	            card.SetAdIcon(PrizeomaticController.Instance.IsBonusRewardsTurn);
            }
        }
        m_closeButton.gameObject.SetActive(false);
        //float num = this.RightOfScreen.transform.position.x - this.LeftOfScreen.transform.position.x;
        //this.xAxisOffset = num/5f;
        //this.heightOffScreen = this.TopOfScreen.transform.position.y - this.BottomOfScreen.transform.position.y;
        //this.TextTapToStop.gameObject.SetActive(false);
        MenuAudio.Instance.fadeMusic(0.4f, 0.5f);
        //CommonUI.Instance.NavBar.DisableAllButtonOperability();
        if (false)//!PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_FirstPrizeCompleted)
        {
            //PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PRIZEOMATIC_INTRO_TITLE",
            //    "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PRIZEOMATIC_INTRO_BODY",
            //    new PopUpButtonAction(this.OnIntroPopupDismissed), false);
            //PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_FirstPrizeCompleted = true;
        }
        else
        {
            PrizeOMaticScreen.screenState = PrizeOMaticScreen.ScreenState.Dealing;
            this.SetUpCards(false);
        }
        this.SetupBackground();
        if (!zAlreadyOnStack)
        {
            PrizeomaticController.Instance.HasWonFuelTank = false;
            PrizeomaticCarPieceChooser.UpdateCarPool(false);
            PrizeomaticController.Instance.carPartsWon.Clear();
            PrizeOMaticScreen.ListOfCardsAwarded.Clear();
        }
    }

    public void TestRun(int maxCards)
	{
		PrizeOMaticScreen.IsTestRunning = true;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		bool flag = false;
		//int currentGold = activeProfile.GetCurrentGold();
		//int currentCash = activeProfile.GetCurrentCash();
        //int fuel = activeProfile.GetFuel();
		if (maxCards < 0)
		{
			flag = true;
			maxCards = 15000;
		}
		int i = 0;
		while (i < maxCards)
		{
			int num = UnityEngine.Random.Range(1, 3);
			this.TestRunReset();
			for (int j = 0; j < num; j++)
			{
				this.selectedCardIndex = j;
				activeProfile.NumberOfStargazerMoments++;
				this.AssignRewardToCard(j);
			}
			for (int k = 0; k < num - 1; k++)
			{
				bool flag2 = PrizeOMaticRewardsManager.IsRewardACarReward(PrizeomaticController.Instance.cards[k].CardReward);
				int num2 = k + 1;
				while (num2 < num && flag2)
				{
					if (!PrizeOMaticRewardsManager.IsRewardACarReward(PrizeomaticController.Instance.cards[num2].CardReward) || PrizeomaticController.Instance.cards[k].CardReward != PrizeomaticController.Instance.cards[num2].CardReward)
					{
					}
					num2++;
				}
			}
			i += num;
			if (flag && PrizeomaticCarPieceChooser.AllCarsCompleted())
			{
				for (int l = 0; l < 10; l++)
				{
					this.TestRunReset();
					this.AssignRewardToCard(0);
					this.AssignRewardToCard(1);
					this.AssignRewardToCard(2);
					if (PrizeomaticController.Instance.numOfCarPieces != 0)
					{
						break;
					}
				}
				break;
			}
		}
		PrizeOMaticScreen.IsTestRunning = false;
	}

	private void TestRunReset()
	{
		PrizeomaticController.Instance.numOfCarPieces = 0;
		PrizeomaticController.Instance.HasWonFuelTank = false;
		PrizeomaticCarPieceChooser.UpdateCarPool(false);
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < activeProfile.CarsOwned.Count; i++)
		{
			if (activeProfile.CarsOwned[i].CurrentPPIndex > num2)
			{
				num = -1;
				num2 = activeProfile.CarsOwned[i].CurrentPPIndex;
			}
		}
		if (num2 > 0 && num > 0)
		{
			activeProfile.CurrentlySelectedCarDBKey = activeProfile.CarsOwned[num].CarDBKey;
		}
	}

	private void SetupBackground()
	{
        //int num = Mathf.CeilToInt(GUICamera.Instance.ScreenWidth / 1.2f) + 1;
        //int num2 = Mathf.CeilToInt(GUICamera.Instance.ScreenHeight / 1.39f) + 1;
        //UnityEngine.Object cardBackGroundPrefab = Resources.Load("Multiplayer/PrizeomaticBackGround");
        //float num3 = 0f;
        //float num4 = 0f;
        //for (int i = 0; i < num; i++)
        //{
        //    float num5 = num4;
        //    for (int j = 0; j < num2; j++)
        //    {
        //        this.SetupPartOfBackground(cardBackGroundPrefab, num3, num5);
        //        num5 += 1.39f;
        //    }
        //    num3 += 1.2f;
        //    num4 = ((num4 != 0f) ? 0f : 0.7f);
        //}
        //this.BottomFade.SetSize(GUICamera.Instance.ScreenWidth, this.BottomFade.height);
	}

	private void SetupPartOfBackground(UnityEngine.Object cardBackGroundPrefab, float xOffset, float yOffset)
	{
		float z = 0.9f;
		GameObject gameObject = UnityEngine.Object.Instantiate(cardBackGroundPrefab) as GameObject;
		gameObject.gameObject.transform.parent = this.BottomOfScreen.transform;
		Vector3 position = this.BottomOfScreen.transform.position + new Vector3(xOffset, yOffset, z);
		gameObject.gameObject.transform.position = position;
	}

	protected override void OnDestroy()
	{
		PrizeomaticController.Instance.cards.Clear();
		this.UnlockNavBarAnimations();
		base.OnDestroy();
	}

	public override void OnDeactivate()
	{
        base.OnDeactivate();
        //CommonUI.Instance.NavBar.EnableAllButtonOperability();
        this.UnlockNavBarAnimations();
	}

    protected override void Update()
    {
        base.Update();
        switch (PrizeOMaticScreen.screenState)
        {
            case PrizeOMaticScreen.ScreenState.Dealing:
                this.DealingCardUpdate();
                break;
            case PrizeOMaticScreen.ScreenState.WaitingForTap:
                this.WaitingForPrizeToBeSelectedUpdate();
                break;
            case PrizeOMaticScreen.ScreenState.ShowingRewards:
                if (PrizeomaticController.Instance.cards[this.selectedCardIndex].CurrentCardState ==
                    PrizeOMaticCard.CardState.CardFlipped)
                {
                    PrizeOMaticScreen.screenState = this.ChangeState(PrizeOMaticScreen.ScreenState.WaitingToShowPopUp);
                }
                break;
            case PrizeOMaticScreen.ScreenState.WaitingToShowPopUp:
                if (this.timeSinceCardFlip > 0.2f)
                {
                    PrizeOMaticRewardsManager.DisplayAPrizePopup(
                        PrizeomaticController.Instance.cards[this.selectedCardIndex].CardReward,
                        PrizeomaticController.Instance.cards[this.selectedCardIndex].CarCardDBKey,
                        new PopUpButtonAction(this.OnPostPopupState));
                    PrizeOMaticScreen.screenState = this.ChangeState(PrizeOMaticScreen.ScreenState.ShowingPopUp);
                    this.timeSinceCardFlip = 0f;
                }
                else
                {
                    this.timeSinceCardFlip += Time.deltaTime;
                }
                break;
            case PrizeOMaticScreen.ScreenState.Complete:
                this.OnComplete();
                SetupScreenForNormalRewards();
                PrizeOMaticScreen.screenState = PrizeOMaticScreen.ScreenState.Finished;
                break;
                case ScreenState.Finished:
                var time = TimeToNextReward();
                SetTimerText(time);
                break;
        }
    }

    private void DealingCardUpdate()
	{
		if (PrizeomaticController.Instance.cards[this.currentCard].CurrentCardState == PrizeOMaticCard.CardState.Dealt)
		{
			this.currentCard++;
			if (this.currentCard < 8)
			{
				PrizeomaticController.Instance.cards[this.currentCard].ChangeState(PrizeOMaticCard.CardState.Dealing);
			}
			else
			{
				PrizeOMaticScreen.screenState = this.ChangeState(PrizeOMaticScreen.ScreenState.WaitingForTap);
			}
		}
	}

	private void WaitingForPrizeToBeSelectedUpdate()
	{
		if (PrizeOMaticScreen.ScreenHasBeenTapped)
		{
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			if (!PrizeomaticCardQueue.IsThereAPrizeQueued())
			{
				activeProfile.NumberOfStargazerMoments++;
			}
			PrizeOMaticScreen.ScreenHasBeenTapped = false;
			this.selectedCardIndex = this.FindIndexOfSelectedCard();
			this.AssignRewardToCard(this.selectedCardIndex);
			PrizeOMaticCard prizeOMaticCard = PrizeomaticController.Instance.cards[this.selectedCardIndex];
			prizeOMaticCard.SetPrizeAlreadyAwarded();
			prizeOMaticCard.PlayRevealAnimation();
			PrizeOMaticScreen.screenState = this.ChangeState(PrizeOMaticScreen.ScreenState.ShowingRewards);
		}
	}

	private bool IsScriptedMoment()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		return PrizeOMaticScriptingManager.IsScriptedMoment(activeProfile.NumberOfStargazerMoments);
	}

	private Reward GetScriptedMomentReward()
	{
		Reward reward = PrizeOMaticScriptingManager.GetRewardForScriptedMoment(PlayerProfileManager.Instance.ActiveProfile.NumberOfStargazerMoments);
		if (PrizeOMaticRewardsManager.IsRewardACarReward(reward))
		{
			reward = this.SetUpScriptedCarMoment(reward);
		}
		return reward;
	}

	private Reward SetUpScriptedCarMoment(Reward scriptedReward)
	{
		if (!this.CanWeAwardCarPiece(scriptedReward))
		{
			return Reward.GoldSmall;
		}
		PrizeomaticController.Instance.AwardedCarPartType = scriptedReward;
		this.SetUpCardToAwardCarPart(scriptedReward);
		return scriptedReward;
	}

	private bool CanWeAwardCarPiece(Reward typeOfCarReward)
	{
		PrizeomaticCarPieceChooser.CARSLOT slot = PrizeomaticCarPieceChooser.ConvertRewardToCarslot(typeOfCarReward);
		string carKey = PrizeomaticCarPieceChooser.GetCarKey(slot);
		return this.CanCarPartCanBeAwarded(carKey);
	}

	private bool CanCarPartCanBeAwarded(string carToAward)
	{
		if (string.IsNullOrEmpty(carToAward))
		{
			return false;
		}
		int numPiecesOfMultiplyerCarWon = PlayerProfileManager.Instance.ActiveProfile.GetNumPiecesOfMultiplyerCarWon(carToAward);
		int num = GameDatabase.Instance.Online.NumTotalCarPiecesToWin(carToAward);
		int num2 = num - numPiecesOfMultiplyerCarWon;
		return num2 > 0;
	}

	public int FindIndexOfSelectedCard()
	{
		int num = -1;
		foreach (PrizeOMaticCard current in PrizeomaticController.Instance.cards)
		{
			if (current.CardSelected)
			{
				current.CardSelected = false;
				if (num == -1)
				{
					num = current.CardNo;
				}
			}
		}
		if (num != -1)
		{
			return num;
		}
		return 0;
	}

	private void OnIntroPopupDismissed()
	{
		this.SetUpCards(false);
		PrizeOMaticScreen.screenState = PrizeOMaticScreen.ScreenState.Dealing;
		PrizeomaticController.Instance.cards[0].ChangeState(PrizeOMaticCard.CardState.Dealing);
		Log.AnEvent(Events.HeresYourCards);
	}

	private void OnOutroPopupDismissed()
	{
		this.BackOutOfScreen();
		Log.AnEvent(Events.FirstCarPartPrize);
	}

	public void OnPostPopupState()
	{
		this.AwardPrize();
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	private void AwardPrize()
	{
		this.AwardPrize(this.selectedCardIndex);
		if (PrizeomaticController.Instance.numOfAttempts > 0) {
			PrizeomaticController.Instance.numOfAttempts--;
		} else {
			PrizeomaticController.Instance.numOfBonusAttempts--;
		}
		
	    PlayerProfileManager.Instance.ActiveProfile.NumberOfPrizeCardRemaining--;
        var activeProfiledata = PlayerProfileManager.Instance.ActiveProfile.GetProfileData();
        var parameters = new Dictionary<string, string>()
	    {
            {"NumberOfPrizeCardRemaining",activeProfiledata.NumberOfPrizeCardRemaining.ToString()},
            {"NumberOfStargazerMoments",activeProfiledata.NumberOfStargazerMoments.ToString()},
            {"NumberOfSportsCarPiecesRemaining",activeProfiledata.NumberOfSportsCarPiecesRemaining.ToString()},
            {"NumberOfDesiribleCarPiecesRemaining",activeProfiledata.NumberOfDesiribleCarPiecesRemaining.ToString()},
            {"NumberOfCommonCarPiecesRemaining",activeProfiledata.NumberOfCommonCarPiecesRemaining.ToString()},
            {"NumberOfTinyCashRewardsRemaining",activeProfiledata.NumberOfTinyCashRewardsRemaining.ToString()},
            {"NumberOfSmallCashRewardsRemaining",activeProfiledata.NumberOfSmallCashRewardsRemaining.ToString()},
            {"NumberOfMediumCashRewardsRemaining",activeProfiledata.NumberOfMediumCashRewardsRemaining.ToString()},
            {"NumberOfLargeCashRewardsRemaining",activeProfiledata.NumberOfLargeCashRewardsRemaining.ToString()},
            {"NumberOfHugeCashRewardsRemaining",activeProfiledata.NumberOfHugeCashRewardsRemaining.ToString()},
            {"NumberOfTinyGoldRewardsRemaining",activeProfiledata.NumberOfTinyGoldRewardsRemaining.ToString()},
            {"NumberOfSmallGoldRewardsRemaining",activeProfiledata.NumberOfSmallGoldRewardsRemaining.ToString()},
            {"NumberOfMediumGoldRewardsRemaining",activeProfiledata.NumberOfMediumGoldRewardsRemaining.ToString()},
            {"NumberOfLargeGoldRewardsRemaining",activeProfiledata.NumberOfLargeGoldRewardsRemaining.ToString()},
            {"NumberOfHugeGoldRewardsRemaining",activeProfiledata.NumberOfHugeGoldRewardsRemaining.ToString()},
            {"NumberOfUpgradeRewardsRemaining",activeProfiledata.NumberOfUpgradeRewardsRemaining.ToString()},
            {"NumberOfFuelRefillsRemaining",activeProfiledata.NumberOfFuelRefillsRemaining.ToString()},
            {"NumberOfFuelPipsRewardsRemaining",activeProfiledata.NumberOfFuelPipsRewardsRemaining.ToString()},
            {"NumberOfTinyRPRewardsRemaining",activeProfiledata.NumberOfTinyRPRewardsRemaining.ToString()},
            {"NumberOfSmallRPRewardsRemaining",activeProfiledata.NumberOfSmallRPRewardsRemaining.ToString()},
            {"NumberOfMediumRPRewardsRemaining",activeProfiledata.NumberOfMediumRPRewardsRemaining.ToString()},
            {"NumberOfLargeRPRewardsRemaining",activeProfiledata.NumberOfLargeRPRewardsRemaining.ToString()},
            {"NumberOfHugeRPRewardsRemaining",activeProfiledata.NumberOfHugeRPRewardsRemaining.ToString()},
            {"NumberOfProTunerRewardsRemaining",activeProfiledata.NumberOfProTunerRewardsRemaining.ToString()},
            {"NumberOfN20ManiacRewardsRemaining",activeProfiledata.NumberOfN20ManiacRewardsRemaining.ToString()},
            {"NumberOfTireCrewRewardsRemaining",activeProfiledata.NumberOfTireCrewRewardsRemaining.ToString()},
	    };
	    if (PrizeomaticController.Instance.numOfAttempts <= 0)
	    {
		    if (PrizeomaticController.Instance.numOfBonusAttempts <= 0) {
			    PrizeOMaticScreen.screenState = this.ChangeState(PrizeOMaticScreen.ScreenState.Complete);
			    parameters.Add("DailyPrizeCardLastEventAt", ServerSynchronisedTime.Instance.GetDateTime().ToString(CultureInfo.InvariantCulture));
		    } else {
			    PrizeOMaticScreen.screenState = this.ChangeState(PrizeOMaticScreen.ScreenState.WaitingForTap);
			    if (!PrizeomaticController.Instance.IsBonusRewardsTurn) {
				    SetupScreenForBonusRewards();
			    }
		    }
	    } else {
	        PrizeOMaticScreen.screenState = this.ChangeState(PrizeOMaticScreen.ScreenState.WaitingForTap);
	    }
	}

	public void OnComplete()
	{
		if (false)//!PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_CardsButNoCarPartCompleted)
		{
			//PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PRIZEOMATIC_OUTRO_TITLE", "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PRIZEOMATIC_OUTRO_BODY", new PopUpButtonAction(this.OnOutroPopupDismissed), false);
			//PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_CardsButNoCarPartCompleted = true;
		}
		else
		{
			this.BackOutOfScreen();
		}
	}

	private void SetupScreenForBonusRewards()
	{
		m_closeButton.gameObject.SetActive(true);
		WaitText.text = LocalizationManager.GetTranslation("TEXT_PRIZEOMATIC_BONUS_SCREEN_TITLE");
		PrizeomaticController.Instance.IsBonusRewardsTurn = true;
		
		foreach (var card in PrizeomaticController.Instance.cards)
		{
			card.SetSprite(bonusCardSprite);
			card.SetAdIcon(PrizeomaticController.Instance.IsBonusRewardsTurn);
			card.SetColor(screenState==ScreenState.Complete);
		}
	}
	
	private void SetupScreenForNormalRewards()
	{
		foreach (var card in PrizeomaticController.Instance.cards)
		{
			card.SetSprite(PrizeomaticController.Instance.IsBonusRewardsTurn?bonusCardSprite:normalCardSprite);
			card.SetAdIcon(PrizeomaticController.Instance.IsBonusRewardsTurn);
			card.SetColor(screenState==ScreenState.Complete);
		}
	}

	private void BackOutOfScreen()
	{
		PrizeOMaticScreen.DidAwardCarPartLastTimeOnScreen = (PrizeomaticController.Instance.numOfCarPieces > 0);
		PrizePieceGiveScreen.OnLoadPrizeGained = PrizeomaticController.Instance.carToAwardDBKey;
		PrizePieceGiveScreen.PiecesToAward = PrizeomaticController.Instance.numOfCarPieces;
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        m_closeButton.gameObject.SetActive(true);
        //RaceReCommon.FinishOnNextButtonRaceTheWorld(ScreenID.PrizeOMatic);
        //ScreenManager.Active.popScreen();

        //TODO
        //DateTime dateTime = this.TimeToNextReward();
        //NotificationManager.Active.UpdateDailyRewardNotification(dateTime);
    }

    private DateTime TimeToNextReward()
    {
        return ServerSynchronisedTime.Instance.GetDateTime().AddDays(1);
    }

    public void SetTimerText(DateTime when)
    {
        //Debug.Log(text);
        var midNight = new DateTime(when.Year, when.Month, when.Day, 0, 0, 0);
        var remainingTime = midNight - ServerSynchronisedTime.Instance.GetDateTime();


        var remainTimeText = string.Empty;
        if (remainingTime.TotalHours > 0)
        {
            remainTimeText = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_HOURS_AND_MINUTES_AND_SECONDS"),
                (int) remainingTime.TotalHours, remainingTime.Minutes, remainingTime.Seconds);
        }
        else if (remainingTime.TotalMinutes > 0)
        {
            remainTimeText = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_MINUTES_AND_SECONDS"),
                (int) remainingTime.TotalMinutes, remainingTime.Seconds);
        }
        else if (remainingTime.TotalSeconds > 0)
        {
            remainTimeText = remainingTime.Seconds.ToString();
        }
        WaitText.text = string.Format(LocalizationManager.GetTranslation("TEXT_NEXT_PRIZEOMATIC_REWARD_BODY"), remainTimeText);
    }



    private void SetTapText(bool isActive)
	{
        //this.TextTapToStop.gameObject.SetActive(isActive);
        //if (PrizeomaticController.Instance.numOfAttempts == 1)
        //{
        //    this.TextTapToStop.Text = LocalizationManager.GetTranslation("TEXT_PRIZEOMATIC_LAST_PRIZE");
        //}
        //else
        //{
        //    this.TextTapToStop.Text = string.Format(LocalizationManager.GetTranslation("TEXT_PRIZEOMATIC_PRIZES_REMAINING"), PrizeomaticController.Instance.numOfAttempts.ToString());
        //}
	}

	private void SetUpCards(bool alreadyDealt = false)
	{
        UnityEngine.Object @object = Resources.Load("Prefabs/PrizeOMaticCard");
	    float screenHeight =0;//GUICamera.Instance.ScreenHeight;
		float num = screenHeight / 3.84f;
		float num2 = 1.2f * num;
		Vector3 position = Vector3.zero ;//= this.TopRow.gameObject.transform.position;
		//float num3 = this.heightOffScreen / 3.5f;
		//position.y -= num3;
		//position.x += this.xAxisOffset;
		for (int i = 0; i < 4; i++)
		{
		    this.SetUpIndividualCard(position, ref @object, i, new Vector3(num2, num2, num2));
			//position.x += this.xAxisOffset;
		}
        Vector3 position2 = Vector3.zero;//= this.BottomRow.gameObject.transform.position;
		//float num4 = this.heightOffScreen / 5f;
		//position2.y += num4;
		//position2.x += this.xAxisOffset;
		for (int j = 4; j < 8; j++)
		{
		    this.SetUpIndividualCard(position2, ref @object, j, new Vector3(num2, num2, num2));
			//position2.x += this.xAxisOffset;
		}
		if (!alreadyDealt && PrizeOMaticScreen.screenState != PrizeOMaticScreen.ScreenState.WaitingForDeal)
		{
			PrizeomaticController.Instance.cards[0].ChangeState(PrizeOMaticCard.CardState.Dealing);
		}
	}

	private void SetUpIndividualCard(Vector3 EndPos, ref Object cardPrefab, int i, Vector3 scale)
	{
        //float z = -0.6f;
        GameObject gameObject = UnityEngine.Object.Instantiate(cardPrefab) as GameObject;
	    PrizeomaticController.Instance.cards.Add(gameObject.GetComponent<PrizeOMaticCard>());
		PrizeomaticController.Instance.cards[i].gameObject.name = "Card " + (i + 1);
        PrizeomaticController.Instance.cards[i].transform.SetParent(this.CardsHolder.transform,false);
        PrizeomaticController.Instance.cards[i].transform.localScale = Vector3.one;//scale;
        //Vector3 position = this.BottomCentre.transform.position;
        //float num = this.heightOffScreen / 5f;
        //position.y -= num;
        //PrizeomaticController.Instance.cards[i].transform.position = position;
        //PrizeomaticController.Instance.cards[i].RandomRotation = (float)UnityEngine.Random.Range(-30, 30);
        //if (i > 0)
        //{
        //    PrizeomaticController.Instance.cards[i].RandomRotation -= PrizeomaticController.Instance.cards[i - 1].RandomRotation * UnityEngine.Random.Range(0f, 0.3f);
        //}
        //Vector3 position2 = PrizeomaticController.Instance.cards[i].transform.position;
        //position2.z = z;
        //EndPos.z = z;
        //PrizeomaticController.Instance.cards[i].StartPos = position2;
        //PrizeomaticController.Instance.cards[i].EndPos = EndPos;
		PrizeomaticController.Instance.cards[i].CardNo = i;
	}

	private void AssignRewardToCard(int cardIndex)
	{
		Reward reward;
		if (this.IsScriptedMoment() && !PrizeomaticCardQueue.IsThereAPrizeQueued())
		{
			reward = this.GetScriptedMomentReward();
		}
		else
		{
			reward = PrizeOMaticRewardsManager.GetRandomReward(GameDatabase.Instance.OnlineConfiguration.PrizeoMatic.PrizeomaticRewards);
			PrizeomaticController.Instance.cards[this.selectedCardIndex].CardReward = reward;
			reward = this.CheckForMultipleCarPartTypesBeingAwarded(cardIndex, reward);
			reward = this.CheckCarHasNotAlreadyBeenCompleted(cardIndex, reward);
			if (PrizeOMaticRewardsManager.IsRewardACarReward(reward))
			{
				this.SetUpCardToAwardCarPart(reward);
			}
			reward = this.CheckForMultipleFuelTanksBeingAwarded(cardIndex, reward);
		}
		if (PrizeOMaticRewardsManager.IsRewardACarReward(reward))
		{
			if (PrizeomaticController.Instance.numOfCarPieces == 0)
			{
				PrizeomaticController.Instance.AwardedCarPartType = reward;
			}
			PrizeomaticController.Instance.numOfCarPieces++;
		}
		PrizeomaticController.Instance.cards[this.selectedCardIndex].QuickPrizeChange(reward);
	}

	private void SetUpCardToAwardCarPart(Reward ScriptedReward)
	{
		PrizeomaticCarPieceChooser.CARSLOT cARSLOT = PrizeomaticCarPieceChooser.ConvertRewardToCarslot(ScriptedReward);
		if (cARSLOT != PrizeomaticCarPieceChooser.CARSLOT.MAX)
		{
			PrizeomaticController.Instance.carToAwardDBKey = PrizeomaticCarPieceChooser.GetCarKey(cARSLOT);
			PrizeomaticController.Instance.cards[this.selectedCardIndex].CarCardDBKey = PrizeomaticController.Instance.carToAwardDBKey;
		}
	}

	private void AwardPrize(int cardIndex)
	{
		PrizeomaticController.Instance.cards[cardIndex].AwardPrize();
		PrizeOMaticScreen.ListOfCardsAwarded.Add(PrizeomaticController.Instance.cards[cardIndex]);
	}

	private Reward CheckForMultipleFuelTanksBeingAwarded(int cardIndex, Reward selectedReward)
	{
		if (selectedReward == Reward.FuelRefill)
		{
			if (PrizeomaticController.Instance.HasWonFuelTank)
			{
				selectedReward = this.FindReplacementPrize(cardIndex);
			}
			else
			{
				PrizeomaticController.Instance.HasWonFuelTank = true;
			}
		}
		return selectedReward;
	}

	private Reward CheckForMultipleCarPartTypesBeingAwarded(int cardIndex, Reward selectedReward)
	{
		if (!PrizeOMaticRewardsManager.IsRewardACarReward(selectedReward) || PrizeomaticController.Instance.numOfCarPieces == 0)
		{
			return selectedReward;
		}
		if (selectedReward == PrizeomaticController.Instance.AwardedCarPartType)
		{
			return selectedReward;
		}
		return this.FindReplacementPrize(cardIndex);
	}

	private Reward CheckCarHasNotAlreadyBeenCompleted(int cardIndex, Reward selectedReward)
	{
		if (!PrizeOMaticRewardsManager.IsRewardACarReward(selectedReward))
		{
			return selectedReward;
		}
		if (this.CanWeAwardCarPiece(selectedReward))
		{
			return selectedReward;
		}
		return this.FindReplacementPrize(cardIndex);
	}

	private Reward FindReplacementPrize(int cardIndex)
	{
		PrizeomaticController.Instance.cards[cardIndex].TakePrizeAwayFromPrizePool();
		PlayerProfileManager.Instance.ActiveProfile.NumberOfTinyRPRewardsRemaining++;
		return Reward.RPTiny;
	}

	private void UnlockNavBarAnimations()
	{
		if (CommonUI.Instance == null || FuelManager.Instance == null)
		{
			return;
		}
		CommonUI.Instance.XPStats.XPLockedState(false);
		CommonUI.Instance.CashStats.CashLockedState(false);
		CommonUI.Instance.CashStats.GoldLockedState(false);
		FuelManager.Instance.FuelLockedState(false);
	}

    public override void Close()
    {
        if (PrizeOMaticScreen.screenState == ScreenState.Complete)
        {
            HomeScreen.CheckForDailyPrize = true;
        }
        base.Close();
    }

    public override bool HasBackButton()
	{
		return false;
	}
}
