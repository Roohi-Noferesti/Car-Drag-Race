using System;
using System.Collections.Generic;
using System.Linq;
using DataSerialization;
using Fabric;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Vector3 = UnityEngine.Vector3;

public class EventPane : MonoBehaviour
{
    private enum AlphaState
    {
        Hide,
        Show,
        Stop
    }

    private AlphaState CurrentAlphaState;

    //public RawImage EventSprite;

    public RawImage EventSpriteOverlay;

    public RawImage EventSpriteBoss;

    public Image EventGlow;

    public Image FuelCost;

    public TextMeshProUGUI FuelCostNumber;
    public Color FuelEnoughColor;
    public Color FuelNotEnoughColor;

    public TextMeshProUGUI NameSpriteT;

    public TextMeshProUGUI DescriptionSpriteT;

    //public TextMeshProUGUI UnlockInfoT;

    public TextMeshProUGUI DistanceSpriteT;

    //public Image DistanceGraphic;

    //public Image DistanceShadowGraphic;

    public TextMeshProUGUI DifficultySpriteT;

    public Color[] DifficultyColors;

    public RawImage DifficultyGraphic;

    //public Image DifficultyShadowGraphic;

    public RuntimeTextButton EasyButton;

    public RuntimeTextButton NormalButton;

    public RuntimeTextButton HardButton;

    public GameObject RewardObjects;
    public TextMeshProUGUI GoldValueSpriteT;
    public TextMeshProUGUI EasyGoldValueSpriteT;
    public TextMeshProUGUI NormalGoldValueSpriteT;
    public TextMeshProUGUI HardGoldValueSpriteT;

    public TextMeshProUGUI CashValueSpriteT;
    public TextMeshProUGUI EasyCashValueSpriteT;
    public TextMeshProUGUI NormalCashValueSpriteT;
    public TextMeshProUGUI HardCashValueSpriteT;
    public GameObject FreeRacePane;

    public Sprite[] DailyBattleRewardIcons;
    public GameObject DailyBattlePane;
    public int DailyBattleDaysAheadToDisplay;
    public ScrollRect DailyBattleScrollRect;

    private Dictionary<int, DailyBattleRewardContainer> RewardContainersbyDay =
        new Dictionary<int, DailyBattleRewardContainer>();
    public Transform DailyBattleLayoutGroup;

    public Sprite QuestionMarkSprite;
    public Sprite[] LadderRewardIcons;
    public Sprite[] CarSpecificRewardIcons;
    public GameObject LadderPane;
    public int LadderRacesAheadToDisplay;
    public ScrollRect LadderScrollRect;
    public Transform LadderLayoutGroup;
    private List<GameObject> LadderRacesRewards = new List<GameObject>();

    public GameObject CrewLayoutPane;
    public EventPaneCrewBattle EventPaneCrewBattle;

    public RuntimeTextButton RaceButton;

    public Sprite BackgroundSprite;

    public Sprite BackgroundGlowSprite;

    public RuntimeTextButton CustomButtonT;

    public RuntimeTextButton CustomButtonB;

    //public IconTextButton FacebookInviteButton;

    public Transform BottomAlignedNode;

    public GameObject TimerParent;

    //public Sprite[] TimerBoxSprites;

    public TextMeshProUGUI TimerText;

    public TextMeshProUGUI DailyBattleTimerText;

    //public GameObject TimerTitle;

    //public TextMeshProUGUI TimerTitleText;

    public GameObject LockedRaceButton;

    public GameObject RYFPanelInfo;

    public GameObject RYFBottomAligned;

    public GameObject RYFTierLeader;

    public GameObject RYFLeaderBackground;

    public Sprite RYFLeaderBackgroundSprite;

    public GameObject RYFLeader;

    public GameObject[] LockedRYFObjects;

    public GameObject[] UnlockedRYFObjects;

    public TextMeshProUGUI LockedCriteriaText;

    public TextMeshProUGUI MyStarUnlocked;

    public TextMeshProUGUI MyStarUnlockedShadow;

    public TextMeshProUGUI MyStarToUnlock;

    public TextMeshProUGUI MyStarToUnlockShadow;

    //public TextMeshProUGUI GrindRelayCarsText;

    public EventPaneRestrictionPanel RestrictionPanel;

    public float YClipPosition;

    public float YClipPositionBoss;

    public float MarginUp;

    public float SpaceBetweenSprites;

    public AnimationCurve AlphaFadingCurve;

    public Action OnCustomPressTop;

    public Action OnCustomPressBot;

    private RaceEventData EventData;

    private RaceEventGroup EventGroup;

    private float PaneHeight;

    private Color TierColour;

    private float CurrentTime;

    private bool isItFirstEventSelected = true;

    public static int RaceGroupSelected { get; private set; }

    private bool isGroupSelected;

    private bool isTierLockedSelected;

    private bool isExtremeDifficultyEvent;

    private int _activeSeasonEventID = -1;

    private bool _dailyBattleEvent;

    private bool _isDailyBattleEventAvailable;

    private string m_DailyBattleNextDayTimerTitleText;

    private string m_DailyBattleThisDayTimerTitleText;

    private string m_SeasonDaysTimerTitleText;

    private string m_SeasonDaysWTThemeTimerTitleText;

    //private bool _ladderEvent;

    private BubbleMessage ExtremeRaceBubble;

    private BubbleMessage RestrictionBubble;

    private DateTime m_DailyBattleLastUpdate;

    private bool m_RaceStarted;

    public BubbleMessage GoRaceBubble;

    private bool _isWTThemeSelectedAndLocked;

    private TierXPin _lockedTierXThemePin;

    private int difficulty = -1;
    public GameObject WorldTourBG;

    public event OnEventPanePressed OnRacePressed;

    public event OnEventPanePressed OnRestrictionPressed;

    public DataDrivenPortrait WorldTourBackground;



    public float PaneWidth
    {
        get;
        private set;
    }

    public float PaneWidthTight
    {
        get
        {
            return PaneWidth - 0.03f;
        }
    }

    private bool IsRookieUnlocked
    {
        get
        {
            if (EventData == null)
                return false;
            var tierEvent = EventData.GetTierEvent();

            if (tierEvent != null)
            {
                if (tierEvent.GetCarTier() >= eCarTier.TIER_2)
                    return true;
                else
                {
                    return
                        PlayerProfileManager.Instance.ActiveProfile.IsEventCompleted(
                            tierEvent.CrewBattleEvents.RaceEventGroups[0].RaceEvents[1].EventID);
                }
            }
            return false;
        }
    }

    private bool IsProUnlocked
    {
        get
        {
            if (EventData == null)
                return false;
            var tierEvent = EventData.GetTierEvent();
            if (tierEvent != null)
            {
                if (tierEvent.GetCarTier() >= eCarTier.TIER_2)
                    return true;
                else
                {
                    return
                        PlayerProfileManager.Instance.ActiveProfile.IsEventCompleted(
                            tierEvent.CrewBattleEvents.RaceEventGroups[0].RaceEvents[2].EventID);
                }
            }
            return false;
        }
    }

    public bool IsVisible
    {
        get { return gameObject.activeInHierarchy; }
    }

    private void SetLockedRaceButtonActive(bool active)
    {
        //this.LockedRaceButton.SetActive(active);
    }

    private void SetRaceButtonActive(bool active)
    {
        RaceButton.gameObject.SetActive(active);
    }

    private void SetTimerParentActive(bool active)
    {
        TimerParent.gameObject.SetActive(active);
    }

    private void SetIsTierLockedSelected(bool locked)
    {
        isTierLockedSelected = locked;
    }

    private void Awake()
    {
        m_DailyBattleLastUpdate = GTDateTime.Now;
        SetBackgroundSize();
        //this.PaneWidth = this.BackgroundSprite.width;
        //this.PaneHeight = this.BackgroundSprite.height;
        SetClipPosition();
        //this.CustomButtonT.ForceAwake();
        //this.CustomButtonB.ForceAwake();
        //this.FacebookInviteButton.Button.ForceAwake();
        //this.FacebookInviteButton.SetPositions();
        FuelManager.Instance.OnFuelAutoReplenish += OnFuelReplenish;
        //UIUtils.YAdjustWithResolution(ref this.TimerTitle, 0.25f);
        TimerParent.gameObject.SetActive(false);
        _activeSeasonEventID = -1;
        m_RaceStarted = false;
        //JsonDict jsonDict = new JsonDict();
        //jsonDict.Set("uid", UserManager.Instance.currentAccount.UserID.ToString());
        //WebRequestQueueRTW.Instance.StartCall("dynamic_client_config", "Getting client config", jsonDict, new WebClientDelegate2(webSyncDynamicClientConfigFinished), this, string.Empty, 5);
      

        Hide();
    }

    private void Start()
    {
        AdjustDifficultyButtonTextSizes();
    }

    private void SetupRaceButtonBubbleMessage()
    {
        if (GoRaceBubble != null)
        {
            GoRaceBubble.Dismiss();
            GoRaceBubble = null;
        }
        PlayerProfile profile = PlayerProfileManager.Instance.ActiveProfile;
        if (profile.RacesWon == 0 && CareerModeMapScreen.CurrentGameMode == GameModeType.SP)
        {
            CreateDelayedBubbleMessage(0.5f, "TEXT_TAP_TO_RACE", RaceButton.transform, new Vector3(0f, 0.17f, -0.1f), 0.5f, () => GoRaceBubble == null && profile.RacesWon == 0 && CareerModeMapScreen.CurrentGameMode == GameModeType.SP);
        }
        else if (profile.RacesEntered == 1 && CareerModeMapScreen.CurrentGameMode == GameModeType.SP)
        {
            CreateDelayedBubbleMessage(0.5f, "TEXT_TAP_TO_RACE_FANGZ", RaceButton.transform, new Vector3(0f, 0.17f, -0.1f), 0.8f, () => GoRaceBubble == null && profile.RacesEntered == 1 && CareerModeMapScreen.CurrentGameMode == GameModeType.SP);
            CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
            EventPin rRPin = careerModeMapScreen.EventSelect.GetRRPin();
            rRPin.SetupCompleted(false);
        }
        //else if (CareerModeMapScreen.CurrentGameMode == GameModeType.RYF && SocialController.Instance.isLoggedIntoFacebook && profile.FriendRacesWon == 1)
        //{
        //    this.CreateDelayedBubbleMessage(0.5f, "TEXT_FRIENDS_GO_TO_NEXT_RACE", this.RaceButton.transform, new Vector3(0f, 0.17f, -0.1f), 1f, () => this.GoRaceBubble == null && CareerModeMapScreen.CurrentGameMode == GameModeType.RYF && SocialController.Instance.isLoggedIntoFacebook && profile.FriendRacesWon == 1);
        //}
    }

    //Moeen CloseWindow
    public void CloseWindow()
    {
        Hide();
    }


    private void CreateDelayedBubbleMessage(float delay, string message, Transform parent, Vector3 offset, float nipplePos, Func<bool> predicate)
    {
        StartCoroutine(BubbleManager.Instance.ShowMessageDelayed(delay, message, false, parent, offset,
            BubbleMessage.NippleDir.DOWN, nipplePos, BubbleMessageConfig.ThemeStyle.SMALL, false,
            delegate (BubbleMessage bubble)
            {
                if (predicate())
                {
                    GoRaceBubble = bubble;
                }
                else
                {
                    bubble.KillNow();
                }
            }));
    }

    private void AdjustDifficultyButtonTextSizes()
    {
        //RuntimeTextButton runtimeTextButton = this.EasyButton.GetComponentsInChildren<RuntimeTextButton>(true)[0];
        //RuntimeTextButton runtimeTextButton2 = this.NormalButton.GetComponentsInChildren<RuntimeTextButton>(true)[0];
        //RuntimeTextButton runtimeTextButton3 = this.HardButton.GetComponentsInChildren<RuntimeTextButton>(true)[0];
        //runtimeTextButton.spriteText.SetCharacterSize(0.126615f);
        //runtimeTextButton2.spriteText.SetCharacterSize(0.126615f);
        //runtimeTextButton3.spriteText.SetCharacterSize(0.126615f);
        //float num = this.AdjustCharacterSizeIfOver(runtimeTextButton.spriteText.TotalWidth, 0.126615f);
        //float num2 = this.AdjustCharacterSizeIfOver(runtimeTextButton2.spriteText.TotalWidth, 0.126615f);
        //float num3 = this.AdjustCharacterSizeIfOver(runtimeTextButton3.spriteText.TotalWidth, 0.126615f);
        //float characterSize = Mathf.Min(new float[]
        //{
        //    num,
        //    num2,
        //    num3
        //});
        //runtimeTextButton.spriteText.SetCharacterSize(characterSize);
        //runtimeTextButton2.spriteText.SetCharacterSize(characterSize);
        //runtimeTextButton3.spriteText.SetCharacterSize(characterSize);
        //this.m_DailyBattleNextDayTimerTitleText = LocalizationManager.GetTranslation("TEXT_DAILY_BATTLE_RACE_TOMORROW_TIMER");
        //this.m_DailyBattleThisDayTimerTitleText = LocalizationManager.GetTranslation("TEXT_DAILY_BATTLE_RACE_AGAIN_TIMER");
        //this.m_SeasonDaysTimerTitleText = LocalizationManager.GetTranslation("TEXT_SEASON_TIME_REMAINING");
        //this.m_SeasonDaysWTThemeTimerTitleText = LocalizationManager.GetTranslation("TEXT_SEASON_WT_THEME_TIME_REMAINING");
    }

    private float AdjustCharacterSizeIfOver(float width, float size)
    {
        float result = size;
        if (width > 0.38f)
        {
            result = size * (0.38f / width);
        }
        return result;
    }

    private void SetDifficultyButtonsText(string buttonAtext, string buttonBtext, string buttonCtext)
    {
        //RuntimeTextButton runtimeTextButton = this.EasyButton.GetComponentsInChildren<RuntimeTextButton>(true)[0];
        //RuntimeTextButton runtimeTextButton2 = this.NormalButton.GetComponentsInChildren<RuntimeTextButton>(true)[0];
        //RuntimeTextButton runtimeTextButton3 = this.HardButton.GetComponentsInChildren<RuntimeTextButton>(true)[0];
        EasyButton.spriteText.text = buttonAtext;
        NormalButton.spriteText.text = buttonBtext;
        HardButton.spriteText.text = buttonCtext;
    }

    private void SetDifficultyFromButtonState()
    {
    }

    private void OnDestroy()
    {
        FuelManager.Instance.OnFuelAutoReplenish -= OnFuelReplenish;
        //WebRequestQueueRTW.Instance.RemoveItems("dynamic_client_config");
        if (GoRaceBubble != null)
        {
            GoRaceBubble.Dismiss();
            GoRaceBubble = null;
        }
    }

    private void OnFuelReplenish()
    {
        if (isTierLockedSelected)
        {
            return;
        }
        NewEventToShow(EventData);
    }

    private Texture2D LoadTexture(string textureName)
    {
        Texture2D texture2D = (Texture2D)Resources.Load(textureName);
        if (texture2D == null)
        {
        }
        return texture2D;
    }

    private void SetUpSprite(RawImage sprite, string textureName, bool isDoubleWidth)
    {
        sprite.gameObject.SetActive(true);
        Texture2D texture2D = LoadTexture(textureName);
        sprite.texture = texture2D;
        //if (isDoubleWidth)
        //{
        //    EventPane.DoEPSpriteSetupDoubleWidth(sprite, texture2D);
        //}
        //else
        //{
        //    EventPane.DoEPSpriteSetup(sprite, texture2D);
        //}
    }

    //private void SetUpSpriteFromBundle(global::Sprite sprite, string texturePath, int eventID)
    //{
    //    sprite.gameObject.SetActive(false);
    //    TexturePack.RequestTextureFromBundle(texturePath, delegate(Texture2D tex)
    //    {
    //        if (this.EventData != null && this.EventData.EventID == eventID)
    //        {
    //            sprite.gameObject.SetActive(true);
    //            sprite.renderer.material.SetTexture("_MainTex", tex);
    //            EventPane.DoEPSpriteSetup(sprite, tex);
    //        }
    //    });
    //}

    //private void LoadAndSetTexture(Renderer objRenderer, string textureName)
    //{
    //    Texture2D texture = this.LoadTexture(textureName);
    //    objRenderer.material.SetTexture("_MainTex", texture);
    //}

    private bool IsDailyBattleAvailable()
    {
        TimeSpan timeUntilNextDailyBattle = PlayerProfileManager.Instance.ActiveProfile.GetTimeUntilNextDailyBattle();
        return (timeUntilNextDailyBattle.TotalSeconds < 0 ||
                PlayerProfileManager.Instance.ActiveProfile.RacesEntered < 3);
    }

    private void UpdateDailyBattles()
    {
        string text = TimerText.text;
        string text2;//= SeasonCountdownManager.GetRemainingTimeString(this._activeSeasonEventID, false);
        _isDailyBattleEventAvailable = IsDailyBattleAvailable();
        if (!_isDailyBattleEventAvailable && (!m_RaceStarted))// || timeUntilNextDailyBattle < TimeSpan.Zero))
        {
            SetRaceButtonActive(false);
            SetLockedRaceButtonActive(true);
        }
        else
        {
            if (!RestrictionPanel.IsRestrictionActive)
            {
                SetRaceButtonActive(true);
            }
            SetLockedRaceButtonActive(false);
        }
        bool isDailyBattleNotAvailable = _dailyBattleEvent && !_isDailyBattleEventAvailable;
        bool isNextDailyBattleAfterMidnight = PlayerProfileManager.Instance.ActiveProfile.GetIsNextDailyBattleAfterMidnight();
        if (m_DailyBattleLastUpdate.Day != GTDateTime.Now.Day || (isDailyBattleNotAvailable && isNextDailyBattleAfterMidnight))
        {
            PlayerProfileManager.Instance.ActiveProfile.ResetDailyBattleDaysIfRequired();
            int gold;
            int cash;
            GetRewardForDailyBattle(out gold, out cash);
            SetRewardObjectsPosition(GoldValueSpriteT, CashValueSpriteT, gold, cash, false, null);
        }
        m_DailyBattleLastUpdate = GTDateTime.Now;
        if (isDailyBattleNotAvailable)
        {
            TimeSpan timeUntilNextDailyBattle2 = PlayerProfileManager.Instance.ActiveProfile.GetTimeUntilNextDailyBattle();
            if ((timeUntilNextDailyBattle2 > TimeSpan.Zero && !m_RaceStarted) || timeUntilNextDailyBattle2 == TimeSpan.Zero)
            {
                DailyBattleTimerText.gameObject.SetActive(true);
                text = DailyBattleTimerText.text;
                if (timeUntilNextDailyBattle2 == TimeSpan.Zero)
                {
                    text2 = LocalizationManager.GetTranslation("TEXT_DAILYBATTLE_CLOCK_CHEAT_MESSAGE_TITLE");
                }
                else
                {
                    DateTime dateTime = new DateTime(timeUntilNextDailyBattle2.Ticks);
                    text2 = dateTime.ToString("HH:mm:ss");
                }
                if (text2 != text)
                {
                    DailyBattleTimerText.text = text2;
                }
                if (RestrictionPanel.gameObject.activeInHierarchy)
                {
                    RestrictionPanel.gameObject.SetActive(false);
                }
                //string text3 = (!isNextDailyBattleAfterMidnight) ? this.m_DailyBattleThisDayTimerTitleText : this.m_DailyBattleNextDayTimerTitleText;
                //if (this.TimerTitleText.text != text3)
                //{
                //    this.TimerTitleText.text = text3;
                //}
            }
            else
            {
                _isDailyBattleEventAvailable = true;
                RaceButton.CurrentState = BaseRuntimeControl.State.Hidden;
                //if (this.LockedRaceButton.activeInHierarchy)
                //{
                //    this.SetLockedRaceButtonActive(false);
                //}
                if (!RestrictionPanel.gameObject.activeInHierarchy && !IsFuelEnough(EventData))
                {
                    RestrictionPanel.gameObject.SetActive(true);
                }
                if (!RestrictionPanel.IsRestrictionActive && !RaceButton.gameObject.activeInHierarchy)
                {
                    SetRaceButtonActive(true);
                }
            }
        }
    }

    private void Update()
    {
        switch (CurrentAlphaState)
        {
            case AlphaState.Hide:
                {
                    CurrentTime += Time.deltaTime;
                    //float num = 1f - this.AlphaFadingCurve.Evaluate(this.CurrentTime);
                    //this.ChangeAllAlphas(num);
                    //this.MoveEventSprite(num);
                    if (IsFadeAnimationOver())
                    {
                        NewEventToShow(EventData);
                    }
                    break;
                }
            case AlphaState.Show:
                {
                    CurrentTime += Time.deltaTime;
                    //float num = this.AlphaFadingCurve.Evaluate(this.CurrentTime);
                    //this.ChangeAllAlphas(num);
                    //this.MoveEventSprite(num);
                    if (IsFadeAnimationOver())
                    {
                        CurrentAlphaState = AlphaState.Stop;
                    }
                    break;
                }
            case AlphaState.Stop:
                RestrictionPanel.AnimateRestriction();
                break;
        }
        if (_activeSeasonEventID == -1)
        {
            //this._activeSeasonEventID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonEventID();
        }
        string text = TimerText.text;
        string remainingTimeString = null;//= SeasonCountdownManager.GetRemainingTimeString(this._activeSeasonEventID, false);
        if (remainingTimeString != text)
        {
            TimerText.text = remainingTimeString;
        }
        TimerText.gameObject.SetActive(false);
        if (_dailyBattleEvent && !isTierLockedSelected)
        {
            UpdateDailyBattles();
        }
        else
        {
            if (DailyBattleTimerText.gameObject.activeSelf)
            {
                DailyBattleTimerText.gameObject.SetActive(false);
            }
            //if (this.TimerTitleText.text != this.m_SeasonDaysTimerTitleText)
            //{
            //    this.TimerTitleText.text = this.m_SeasonDaysTimerTitleText;
            //}
        }
        bool flag = _dailyBattleEvent && !_isDailyBattleEventAvailable;
        if (_isWTThemeSelectedAndLocked)
        {
            UpdateTierXLockedTheme();
        }
        bool flag2 = !string.IsNullOrEmpty(TimerText.text);
        bool flag3 = (flag && !isTierLockedSelected) || (_isWTThemeSelectedAndLocked && flag2);
        if (TimerParent.gameObject.activeInHierarchy != flag3)
        {
            SetTimerParentActive(flag3);
        }
    }

    public void UpdateTierXLockedTheme()
    {
        //if (this._lockedTierXThemePin.pinDetails.Lock.Type == "Season")
        //{
        //    int intValue = this._lockedTierXThemePin.pinDetails.Lock.Details.IntValue;
        //    int num = intValue - 1;
        //    int mostRecentActiveSeasonEventID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonEventID();
        //    SeasonEventMetadata @event = GameDatabase.Instance.SeasonEvents.GetEvent(mostRecentActiveSeasonEventID);
        //    bool flag = false;
        //    bool flag2 = false;
        //    bool flag3 = false;
        //    if (((@event == null || @event.SeasonDisplayNumber < num) && !flag3) || flag)
        //    {
        //        this.TimerTitleText.gameObject.SetActive(false);
        //        this.TimerText.gameObject.SetActive(true);
        //        this.TimerText.Text = LocalizationManager.GetTranslation("TEXT_SEASON_WT_THEME_TIME_COMING_SOON");
        //    }
        //    else if ((@event.SeasonDisplayNumber == num && !flag3) || flag2)
        //    {
        //        this.TimerTitleText.gameObject.SetActive(true);
        //        this.TimerTitleText.Text = this.m_SeasonDaysWTThemeTimerTitleText;
        //        this.TimerText.gameObject.SetActive(true);
        //        string text = this.TimerText.Text;
        //        string remainingTimeString = SeasonCountdownManager.GetRemainingTimeString(mostRecentActiveSeasonEventID, true);
        //        if (remainingTimeString != text)
        //        {
        //            this.TimerText.Text = remainingTimeString;
        //        }
        //    }
        //    else
        //    {
        //        this.TimerTitleText.gameObject.SetActive(false);
        //        this.TimerText.gameObject.SetActive(false);
        //    }
        //}
        //else
        //{
        //    this.TimerTitleText.gameObject.SetActive(false);
        //    this.TimerText.gameObject.SetActive(false);
        //    this.TimerTitleText.Text = null;
        //    this.TimerText.Text = null;
        //}
    }

    private void TryShowOnTapPopup(ScheduledPin pin)
    {
        if (pin != null)
        {
            IGameState gs = new GameStateFacade();
            PopupData popupData = pin.GetOnMapPinTapPopup();
            if (popupData.IsEligible(gs) && PopUpManager.Instance.TryShowPopUp(popupData.GetPopup(delegate
            {
                SendPopupConfirmMetric(popupData.TitleText, gs);
            }, null), PopUpManager.ePriority.Default, null))
            {
            }
        }
    }

    private void TierXThemeLockChanged(TierXPin pin, Color themeColour)
    {
        LoadTierAction clickAction = pin.pinDetails.ClickAction;
        if (clickAction == null)
        {
            return;
        }
        TryShowOnTapPopup(pin.pinDetails.WorldTourScheduledPinInfo);
        OnTierXThemeLoading(clickAction, themeColour);
    }

    public void OnTierXthemeUnlocked(TierXPin pin, Color themeColour)
    {
        WorldTourBG.gameObject.SetActive(true);
        TierXThemeLockChanged(pin, themeColour);
        SetLockedRaceButtonActive(false);
        SetRaceButtonActive(true);
        RaceButton.CurrentState = BaseRuntimeControl.State.Active;
        RaceButton.SetText("TEXT_BUTTON_NEXT", false, true);
        _isWTThemeSelectedAndLocked = false;
        _lockedTierXThemePin = null;

        TextureDetail textureDetails = pin.pinDetails.GetTextureDetails(PinDetail.TextureKeys.EventPaneBackground);
        if (textureDetails != null)
        {
            var eventDesc = LocalizationManager.GetTranslation(pin.pinDetails.EventDescription);
            WorldTourBackground.Init(textureDetails.GetName(), eventDesc, null);
            //this.WorldTourCar.gameObject.transform.localPosition = textureDetails.Offset.AsUnityVector3();
            //this.WorldTourCar.gameObject.transform.localScale = textureDetails.Scale.AsUnityVector3();
        }
        else
        {
            WorldTourBackground.gameObject.SetActive(false);
        }

    }

    public void OnTierXThemeLocked(TierXPin pin, Color themeColour)
    {
        TierXThemeLockChanged(pin, themeColour);
        SetLockedRaceButtonActive(true);
        _isWTThemeSelectedAndLocked = true;
        _lockedTierXThemePin = pin;
    }

    private void OnTierXThemeLoading(LoadTierAction details, Color themeColour)
    {
        if (BuildType.IsAppTuttiBuild && !details.eventPaneSprite.Contains("apptutti") &&
            (details.eventPaneSprite.ToLower().Contains("crew-it") || details.eventPaneSprite=="country-select-logo-IT"))
                details.eventPaneSprite += "_apptutti";
        
        Texture2D texture2D = TierXManager.Instance.PinTextures[details.eventPaneSprite];
        EventSpriteBoss.texture = texture2D;
        EventSpriteBoss.gameObject.SetActive(true);
        //EventPane.DoEPSpriteSetup(this.EventSpriteBoss, texture2D);
        OnTierDisabled(LocalizationManager.GetTranslation(details.eventPaneTitle), LocalizationManager.GetTranslation(details.eventPaneDesc), themeColour);
        SetIsTierLockedSelected(false);
        SetClipPosition();
        DeactivateForLockedTier();
        //this.BackgroundGlowSprite.gameObject.renderer.material.SetColor("_Tint", themeColour);
        NewEventGlowToUpdate(Color.white);
        SetEventSpritePositions();
        CurrentAlphaState = AlphaState.Show;
        CurrentTime = 0f;
        TierXOverviewPositionUpdate();
        _dailyBattleEvent = false;
    }

    public void OnTierXLocked()
    {
        NewEventGlowToUpdate(Color.white);
        SetUpSprite(EventSpriteBoss, "CharacterCards/Crew/Logos/WorldTour_Badge", false);
        OnTierDisabled(LocalizationManager.GetTranslation("TEXT_WORLDTOUR_MODE_LOCKED_TITLE"), LocalizationManager.GetTranslation("TEXT_WORLDTOUR_MODE_LOCKED_DESCRIPTION"), GameDatabase.Instance.Colours.GetTierColour(eCarTier.TIER_X));
        SetLockedRaceButtonActive(false);
    }

    public void OnStoryTierLocked(int inCarTier)
    {
        //int num = inCarTier + 1;
        //string text = LocalizationManager.GetTranslation("TEXT_CHATTER_T" + num + "_C_PRERACE_TITLE");
        //text += "\n";
        //text += LocalizationManager.GetTranslation("TEXT_TIER_" + CarTierHelper.TierToString[inCarTier]);
        //string descriptionText = LocalizationManager.GetTranslation("TEXT_TIER_LOCKED_" + CarTierHelper.TierToString[inCarTier]);
        //string text2 = "CharacterCards/Crew/Logos/Crew_{0}_Badge";
        //text2 = string.Format(text2, CarTierHelper.TierToString[inCarTier]);
        //UnityEngine.Color tierColour = GameDatabase.Instance.Colours.GetTierColour(inCarTier);
        //this.NewEventGlowToUpdate(tierColour);
        //this.SetUpSprite(this.EventSpriteBoss, text2, false);
        //this.OnTierDisabled(text, descriptionText, tierColour);
    }

    public void OnTierDisabled(string nameText, string descriptionText, Color color)
    {
        SetIsTierLockedSelected(true);
        SetClipPosition();
        DeactivateForLockedTier();
        SetEventName(nameText);
        SetDescription(descriptionText);
        //this.BackgroundGlowSprite.gameObject.renderer.material.SetColor("_Tint", color);
        SetEventSpritePositions();
        CurrentAlphaState = AlphaState.Show;
    }

    public void SetEventSpritePositions()
    {
        //float x = this.PaneWidth / 2f;
        //UnityEngine.Vector3 vector = new UnityEngine.Vector3(x, -this.MarginUp, -0.01f);
        //this.EventSprite.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //this.EventSpriteBoss.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //float num = (CareerModeMapScreen.carTierSelected != eCarTier.TIER_X) ? this.YClipPositionBoss : this.YClipPosition;
        //vector.y += num + (this.MarginUp - 0.04f);
        //vector.z -= 0.1f;
        //this.EventGlow.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //vector.y -= this.SpaceBetweenSprites;
        //this.NameSpriteT.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //vector.y -= (float)this.NameSpriteT.GetDisplayLineCount() * this.NameSpriteT.BaseHeight + this.SpaceBetweenSprites;
        //this.DescriptionSpriteT.gameObject.transform.localPosition = vector;
    }

    public void OnGroupSelected(RaceEventGroup raceEventGroup)
    {
        SetIsTierLockedSelected(false);
        isGroupSelected = true;
        EventGroup = raceEventGroup;
        if (!IsSelectedRaceGroupOK(raceEventGroup))
        {
            RaceGroupSelected = 0;
        }
        EventData = raceEventGroup.RaceEvents[RaceGroupSelected];
        if (isItFirstEventSelected)
        {
            ActivateAll();
            NewEventToShow(EventData);
            isItFirstEventSelected = false;
        }
        else
        {
            StartHideAnimation();
        }
        SetInitialDifficulty();
    }

    public void OnMultiplayerLocked()
    {
        //this.SetIsTierLockedSelected(true);
        //this.SetClipPosition();
        //this.DeactivateForLockedTier();
        //List<GameObject> list = new List<GameObject>
        //{
        //    this.CustomButtonB.gameObject,
        //    this.CustomButtonT.gameObject,
        //    this.EventSprite.gameObject,
        //    this.EventSpriteOverlay.gameObject,
        //    this.FuelCost.gameObject,
        //    this.CustomButtonB.gameObject,
        //    this.CustomButtonT.gameObject,
        //    this.CustomButtonB.gameObject,
        //    this.CustomButtonT.gameObject,
        //    this.FacebookInviteButton.gameObject
        //};
        //list.ForEach(delegate(GameObject go)
        //{
        //    go.SetActive(false);
        //});
        //List<GameObject> list2 = new List<GameObject>
        //{
        //    this.TimerText.gameObject,
        //    this.EventSpriteBoss.gameObject
        //};
        //list2.ForEach(delegate(GameObject go)
        //{
        //    go.SetActive(true);
        //});
        //string textureName = "CharacterCards/Crew/Logos/Crew_1_Badge";
        //this.SetUpSprite(this.EventSpriteBoss, textureName, false);
        //this.NewEventGlowToUpdate(GameDatabase.Instance.Colours.GetTierColour("Multiplayer"));
        //this.SetEventName(LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_MULTIPLAYER"));
        //this.SetDescription(LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_MAP_LOCKED"));
        //this.TierColour = GameDatabase.Instance.Colours.GetTierColour("Multiplayer");
        //this.BackgroundGlowSprite.gameObject.renderer.material.SetColor("_Tint", this.TierColour);
        //this.SetEventSpritePositions();
        //this.CurrentAlphaState = EventPane.AlphaState.Show;
        //this._activeSeasonEventID = -1;
    }

    public void OnEventSelected(RaceEventData raceData)
    {
        if(CheatEngine.Instance != null) {
            CheatEngine.Instance.ScreenChangedTo("EventPane");
        }
        
        EventData = raceData;
        isGroupSelected = false;
        SetIsTierLockedSelected(false);
        //By clicking on event simply show the eventpane. The condition is for old event pane used in CSR Racing 1
        if (true)//isItFirstEventSelected)
        {
            ActivateAll();
            NewEventToShow(raceData);
            isItFirstEventSelected = false;
        }
        else
        {
            //this.StartHideAnimation();
        }
    }

    private void StartHideAnimation()
    {
        CurrentTime = 0f;
        CurrentAlphaState = AlphaState.Hide;
        if (EventData == null)
        {
            //this.RaceButton.button.SetControlState(UIButton.CONTROL_STATE.DISABLED);
        }
    }


    private void CrewBattleSetup()
    {
        CrewLayoutPane.SetActive(true);
        EventPaneCrewBattle.Initialise(EventData.GetTierEvent().GetCarTier(), EventData);
    }

    private void DailyBattleSetup()
    {
        _dailyBattleEvent = true;
        _isDailyBattleEventAvailable = IsDailyBattleAvailable();
        if (!_isDailyBattleEventAvailable)
        {
            SetRaceButtonActive(false);
            SetLockedRaceButtonActive(true);
        }
        else
        {
            if (!RestrictionPanel.IsRestrictionActive)
            {
                SetRaceButtonActive(true);
            }
            SetLockedRaceButtonActive(false);
        }

        DailyBattlePane.SetActive(true);
        CashValueSpriteT.gameObject.SetActive(false);
        UpdateDailyBattleCalendar();
    }


    private void UpdateDailyBattleCalendar()
    {
        //Clear
        foreach (var dailyBattleRewardContainer in RewardContainersbyDay)
        {
            Destroy(dailyBattleRewardContainer.Value.gameObject);
        }
        RewardContainersbyDay.Clear();
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        var CurrentDay = activeProfile.DailyBattlesConsecutiveDaysCount;
        if (PlayerProfileManager.Instance.ActiveProfile.GetDaysSinceLastDailyBattle() == 1 || PlayerProfileManager.Instance.ActiveProfile.GetIsNextDailyBattleAfterMidnight())
        {
            CurrentDay++;
        }
        var HighlightedDay = CurrentDay;
        var highestUnlockedClass = RaceEventQuery.Instance.getHighestUnlockedClass();
        var consecutiveDays = Math.Max(1, HighlightedDay);
        var rewardSequence =
            DailyBattleRewardManager.Instance.Sequence(consecutiveDays, highestUnlockedClass, true)
                .Take(DailyBattleDaysAheadToDisplay + 2)
                .ToList();
        var i = -1;
        var day = HighlightedDay;
        var num3 = 0;
        while (i <= DailyBattleDaysAheadToDisplay)
        {
            if (day >= 0)
            {
                var dailyBattleReward = rewardSequence[num3];
                num3++;
                var dailyBattleRewardContainer = DailyBattleRewardContainer.Create(day == HighlightedDay, true);
                dailyBattleRewardContainer.Init(DailyBattleRewardIcons[dailyBattleReward.RewardIcon]);
                dailyBattleRewardContainer.DayText.text = GetNameForDay(CurrentDay, day);
                //dailyBattleRewardContainer.DayFadeText.text = dailyBattleRewardContainer.DayText.text;
                dailyBattleRewardContainer.PrizeText.text = dailyBattleReward.GetRewardText();
                if (!RewardContainersbyDay.ContainsKey(day))
                    RewardContainersbyDay.Add(day, dailyBattleRewardContainer);
                dailyBattleRewardContainer.transform.SetParent(DailyBattleLayoutGroup, false);
            }
            i++;
            day++;
        }
        DailyBattleScrollRect.horizontalNormalizedPosition = 0;//GetNormalizedPosition(consecutiveDays - 1);
        //this.RewardListAnimPosition = GetNormalizedPosition(DaysAheadToFocus);
        //this.RewardListFinalPosition = GetNormalizedPosition(consecutiveDays - 1);
        //AudioManager.Instance.PlaySound(this.ScreenAppearAudioEvent, null);
    }

    private float GetNormalizedPosition(int itemIndex)
    {
        var childCount = DailyBattleLayoutGroup.transform.childCount;
        return Mathf.InverseLerp(0, childCount - 1, itemIndex);
    }

    private float GetNormalizedPositionForLadder(int itemIndex)
    {
        var childCount = LadderLayoutGroup.transform.childCount;
        return Mathf.InverseLerp(0, childCount - 1, itemIndex);
    }


    private string GetNameForDay(int CurrentDay, int day)
    {
        int num = day - CurrentDay;
        switch (num + 1)
        {
            case 0:
                return LocalizationManager.GetTranslation("TEXT_YESTERDAY");
            case 1:
                return LocalizationManager.GetTranslation("TEXT_TODAY");
            case 2:
                return LocalizationManager.GetTranslation("TEXT_TOMORROW");
            default:
                return string.Format(LocalizationManager.GetTranslation("TEXT_DAY"), day);
        }
    }


    private string GetNameForRaceNumber(int CurrentRace, int race)
    {
        int num = race - CurrentRace;
        switch (num + 1)
        {
            //case 0:
            //    return LocalizationManager.GetTranslation("TEXT_PREV_RACE");
            //case 1:
            //    return LocalizationManager.GetTranslation("TEXT_CURRENT_RACE");
            //case 2:
            //    return LocalizationManager.GetTranslation("TEXT_NEXT_RACE");
            default:
                return string.Format(LocalizationManager.GetTranslation("TEXT_RACE"), race + 1);
        }
    }


    private void LadderSetup()
    {
        //this._ladderEvent = true;
        SetLockedRaceButtonActive(false);
        SetRaceButtonActive(true);//!this.RestrictionPanel.IsRestrictionActive);
        LadderPane.SetActive(true);
        CashValueSpriteT.gameObject.SetActive(false);
        UpdateLadderSetup();
    }


    private void UpdateLadderSetup()
    {
        //Clear
        foreach (var ladderRacesReward in LadderRacesRewards)
        {
            Destroy(ladderRacesReward);
        }
        LadderRacesRewards.Clear();
        //var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //var highestUnlockedClass = RaceEventQuery.Instance.getHighestUnlockedClass();
        BaseCarTierEvents tierEvents = EventData.GetTierEvent();

        short currentRace = 0;
        List<RaceEventData> rewardSequence = new List<RaceEventData>();
        //int consecutiveDays = 0;
        int aheadVisibleReward = 300;
        var eventsCompleted = PlayerProfileManager.Instance.ActiveProfile.EventsCompleted;
        if (EventData.IsLadderEvent())
        {
            currentRace = RaceEventQuery.Instance.GetLadderEvent(tierEvents).EventOrder;
            rewardSequence = tierEvents.LadderEvents.RaceEventGroups[0].RaceEvents.ToList();
            //consecutiveDays = Math.Max(1, (int) CurrentRace);
        }
        if (EventData.IsRestrictionEvent())
        {
            rewardSequence = tierEvents.RestrictionEvents.RaceEventGroups.SelectMany(g => g.RaceEvents).ToList();
            currentRace = (short) rewardSequence.Count(e => eventsCompleted
                .Contains(e.EventID));
            aheadVisibleReward = EventData.Group.NumOfEvents();
        }
        else if (EventData.IsCarSpecificEvent())
        {
            rewardSequence = tierEvents.CarSpecificEvents.RaceEventGroups.SelectMany(g => g.RaceEvents).ToList();
            currentRace = (short)rewardSequence.Count(e => eventsCompleted
                .Contains(e.EventID));
            //consecutiveDays = Math.Max(1, (int)CurrentRace);
            aheadVisibleReward = EventData.Group.NumOfEvents();
        }
        else if (EventData.IsManufacturerSpecificEvent())
        {
            rewardSequence = tierEvents.ManufacturerSpecificEvents.RaceEventGroups.SelectMany(g => g.RaceEvents).ToList();
            currentRace = (short)rewardSequence.Count(e => eventsCompleted
                .Contains(e.EventID));
            //consecutiveDays = Math.Max(1, (int)CurrentRace);
            aheadVisibleReward = EventData.Group.NumOfEvents();
        }
        else if (EventData.IsWorldTourRace())
        {
            currentRace = RaceEventQuery.Instance.GetWorldTourEvent(EventData).EventOrder;
            rewardSequence = EventData.Group.RaceEvents.Take(30).ToList();//skip last event because it is super nitrous event
            //consecutiveDays = Math.Max(1, (int)CurrentRace);
            aheadVisibleReward = EventData.Group.NumOfEvents() - 1; //last event is super nitrous;
        }

        for (int i = currentRace, j = EventData.EventOrder; i < rewardSequence.Count; i++, j++)
        {
            var showReward = j < aheadVisibleReward;
            //if this is a restriction race , we want to show first reward only because other races order is not deterministic
            if (EventData.IsRestrictionEvent() && i > currentRace)
            {
                showReward = false;
            }
            var raceData =
                (EventData.IsRestrictionEvent() || EventData.IsCarSpecificEvent() ||
                EventData.IsManufacturerSpecificEvent()) && j<EventData.Group.RaceEvents.Count
                    ? EventData.Group.RaceEvents[j]
                    : rewardSequence[i];
            var dailyBattleRewardContainer = DailyBattleRewardContainer.Create(i == currentRace, false);
            dailyBattleRewardContainer.Init(showReward
                ? (EventData.IsLadderEvent() ? LadderRewardIcons[i] : CarSpecificRewardIcons[i])
                : QuestionMarkSprite);
            dailyBattleRewardContainer.DayText.text = GetNameForRaceNumber(currentRace, i);
            dailyBattleRewardContainer.PrizeText.text = raceData.GetRewardText();
            dailyBattleRewardContainer.PrizeText.gameObject.SetActive(showReward);
            dailyBattleRewardContainer.transform.SetParent(LadderLayoutGroup, false);
            LadderRacesRewards.Add(dailyBattleRewardContainer.gameObject);
        }
        LadderScrollRect.horizontalNormalizedPosition = 0;//GetNormalizedPositionForLadder(consecutiveDays - 1);
    }

    private void NewEventToShow(RaceEventData raceData)
    {
        if (raceData == null)
        {
            return;
        }
        DailyBattlePane.SetActive(false);
        LadderPane.SetActive(false);
        CrewLayoutPane.SetActive(false);
        FreeRacePane.SetActive(true);

        Show();
        //eCarTier tier = (raceData.Parent != null) ? raceData.Parent.GetTierEvents().GetCarTier() : eCarTier.TIER_5;
        SetClipPosition();
        SetGroupButtons();
        //this.CustomButtonB.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
        //this.CustomButtonT.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
        //this.FacebookInviteButton.Button.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
        //this.FacebookInviteButton.gameObject.SetActive(false);
        //this.RYFPanelInfo.SetActive(false);
        //this.RYFBottomAligned.SetActive(false);
        SetLockedRaceButtonActive(false);
        NewEventPinToUpdate(false, raceData);
        //Color tierColour = GameDatabase.Instance.Colours.GetTierColour(tier);
        //this.NewEventGlowToUpdate(tierColour);
        //this.TierColour = this.GetTierColor(raceData);
        //this.BackgroundGlowSprite.gameObject.renderer.material.SetColor("_Tint", this.TierColour);
        if (EventData.IsCrewBattle())
        {
            NameSpriteT.gameObject.SetActive(false);
        }
        else
        {
            NameSpriteT.gameObject.SetActive(true);
        }
        NewEventNameToUpdate(raceData);
        NewEventDescriptionToUpdate(raceData);
        NewEventLockInfoToUpdate(raceData);
        bool hasfuel = IsFuelEnough(raceData);
        NewEventFuelCostObjectsToUpdate(raceData, hasfuel);
        RestrictionPanel.Setup(raceData, hasfuel);
        //if (this.RestrictionBubble != null)
        //{
        //this.RestrictionBubble.Dismiss();
        //this.RestrictionBubble = null;
        RestrictionPanel.RestrictionBubble.HideRestrictionBubble();
        //}
        if (RestrictionPanel.IsRestrictionActive)
        {
            //this.RestrictionBubble = this.RestrictionPanel.RestrictionBubble.ShowRestrictionBubbleMessage();
            RestrictionPanel.RestrictionBubble.ShowRestrictionBubbleMessage();
        }
        NewEventDifficultyToUpdate(raceData);
        NewEventDistanceToUpdate(raceData);
        NewEventObjectsPositionToUpdate(raceData);
        CurrentAlphaState = AlphaState.Show;
        RaceButton.CurrentState = BaseRuntimeControl.State.Active;
        CurrentTime = 0f;
        _dailyBattleEvent = false;
        _isWTThemeSelectedAndLocked = false;
        _lockedTierXThemePin = null;
        _isDailyBattleEventAvailable = false;
        RaceButton.SetText("TEXT_BUTTON_CONTINUE", false, false);
        WorldTourBG.SetActive(false);
        if (raceData.IsCrewBattle())
        {
            CrewBattleSetup();
        }
        else if (raceData.IsFriendRaceEvent())
        {
            RaceYourFriendsSetup();
        }
        else if (raceData.IsDailyBattle())
        {
            DailyBattleSetup();
        }
        else if (raceData.IsLadderEvent() || raceData.IsRestrictionEvent() || raceData.IsCarSpecificEvent() || raceData.IsManufacturerSpecificEvent())
        {
            LadderSetup();
        }
        else if (raceData.IsWorldTourRace())
        {
            if (CareerModeMapScreen.mapPaneSelected == MapPaneType.SinglePlayer)
            {
                WorldTourBG.SetActive(true);
            }
            else
            {
                WorldTourBG.SetActive(false);
                PinDetail worldTourPinPinDetail = raceData.GetWorldTourPinPinDetail();
                if (worldTourPinPinDetail != null)
                {
                    TryShowOnTapPopup(worldTourPinPinDetail.WorldTourScheduledPinInfo);

                    if (worldTourPinPinDetail.WorldTourScheduledPinInfo != null
                        && worldTourPinPinDetail.WorldTourScheduledPinInfo.ParentSequence != null &&
                        worldTourPinPinDetail.WorldTourScheduledPinInfo.ParentSequence.Pins.Count > 5)
                        LadderSetup();
                }
            }

        }
        //float alpha = this.AlphaFadingCurve.Evaluate(this.CurrentTime);
        //this.ChangeAllAlphas(alpha);
        SetDifficultyButtonsText(LocalizationManager.GetTranslation("TEXT_RACE_DIFFICULTY_ROOKIE"),
            LocalizationManager.GetTranslation("TEXT_RACE_DIFFICULTY_AMATEUR"), LocalizationManager.GetTranslation("TEXT_RACE_DIFFICULTY_PRO"));
        AdjustDifficultyButtonTextSizes();
        //this.SetupRaceButtonBubbleMessage();
    }

    private void SendPopupConfirmMetric(string popupTitle, IGameState gs)
    {
        string currentThemeName = TierXManager.Instance.CurrentThemeName;
        Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
        {
            {
                Parameters.RTier,
                currentThemeName
            },
            {
                Parameters.Title,
                popupTitle
            }
        };
        Log.AnEvent(Events.WorldTourTutorial, data);
    }

    private static void webSyncDynamicClientConfigFinished(string content, string error, int status, object userData)
    {
        if (status != 200 || string.IsNullOrEmpty(content) || error != null)
        {
            return;
        }
        EventPane eventPane = (EventPane)userData;
        eventPane.OnDynamicConfgReceived(content);
    }

    private void OnDynamicConfgReceived(string content)
    {
        JsonDict jsonDict = new JsonDict();
        if (jsonDict.Read(content))
        {
            List<string> stringList = jsonDict.GetStringList("season_car_list");
            string @string = jsonDict.GetString("version");
            //string string2 = jsonDict.GetString("replayVersion");
            //string networkReplayVersion = GameDatabase.Instance.OnlineConfiguration.NetworkReplayVersion;
            if (ApplicationVersion.IsGreaterThanCurrent(@string))
            {
                return;
            }
            //if (ApplicationVersion.Compare(string2, networkReplayVersion) > 0)
            //{
            //    return;
            //}
            if (stringList == null)
            {
                return;
            }
            stringList.RemoveAll((string key) => CarDatabase.Instance.GetCarOrNull(key) == null);
            if (stringList.Count == 0)
            {
                return;
            }
            RaceEventData raceEventData = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.RaceTheWorldWorldTourEvents.RaceEventGroups[0].RaceEvents[0];
            raceEventData.Restrictions.Clear();
            RaceEventRestriction raceEventRestriction = new RaceEventRestriction
            {
                RestrictionType = eRaceEventRestrictionType.CAR_MODEL
            };
            raceEventRestriction.SetCarModels(stringList);
            raceEventData.Restrictions.Add(raceEventRestriction);
            RaceEventInfo.Instance.SeasonCars = new HashSet<string>(stringList);
        }
    }

    private void NewEventPinToUpdate(bool isTierLocked, RaceEventData raceData)
    {
        if (raceData.IsCrewBattle())
        {
            SetupEventPinCrewBattle(raceData);
        }
        else if (raceData.IsWorldTourRace())
        {
            SetupEventPinWorldTour(raceData);
        }
        else
        {
            SetupEventPinNormal(raceData);
        }
    }

    private void SetupEventPinCrewBattle(RaceEventData raceData)
    {
        //string text = CarTierHelper.TierToString[(int)raceData.Parent.GetTierEvents().GetCarTier()];
        if (raceData.IsBossRace())
        {
            //string text2 = "Career/HighResPins/CrewPins/crew_leader_pin_" + text;
            //this.SetUpSprite(this.EventSpriteBoss, text2, false);
        }
        else
        {
            //int num = raceData.GetProgressionRaceEventNumber() + 1;
            //string text2 = string.Concat(new object[]
            //{
            //    "CrewPortraitsTier",
            //    text,
            //    ".Crew ",
            //    num
            //});
            //this.SetUpSpriteFromBundle(this.EventSpriteBoss, text2, raceData.EventID);
        }
        //this.EventSprite.gameObject.SetActive(false);
        EventSpriteOverlay.gameObject.SetActive(false);
    }

    private void SetupEventPinWorldTour(RaceEventData raceData)
    {
        PinDetail worldTourPinPinDetail = raceData.GetWorldTourPinPinDetail();
        //string eventPaneBossSprite = worldTourPinPinDetail.GetEventPaneBossSprite();
        //if (!string.IsNullOrEmpty(eventPaneBossSprite))
        //{
        //    Texture2D crewMemberEventPaneTexture = TierXManager.Instance.GetCrewMemberEventPaneTexture(eventPaneBossSprite);
        //    this.EventSpriteBoss.GetComponent<Renderer>().material.SetTexture("_MainTex", crewMemberEventPaneTexture);
        //    //EventPane.DoEPSpriteSetup(this.EventSpriteBoss, crewMemberEventPaneTexture);
        //    this.EventSpriteBoss.gameObject.SetActive(true);
        //    this.EventSprite.gameObject.SetActive(false);
        //    this.EventSpriteOverlay.gameObject.SetActive(false);
        //}
        //else if (!raceData.IsRelay && raceData.IsAIDriverAvatarAvailable())
        //{
        //    TexturePack.RequestTextureFromBundle(raceData.AIDriverCrew + ".Portrait" + raceData.AIDriverCrewNumber, delegate (Texture2D texture)
        //    {
        //        if (ScreenManager.Active.CurrentScreen != ScreenID.CareerModeMap)
        //        {
        //            return;
        //        }
        //        this.EventSpriteBoss.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
        //        EventPane.DoEPSpriteSetup(this.EventSpriteBoss, texture);
        //        this.EventSpriteBoss.gameObject.SetActive(true);
        //        this.EventSprite.gameObject.SetActive(false);
        //        this.EventSpriteOverlay.gameObject.SetActive(false);
        //    });
        //}
        //else
        //{
        //this.EventSprite.gameObject.SetActive(true);
        EventSpriteOverlay.gameObject.SetActive(true);
        //this.EventSpriteBoss.gameObject.SetActive(false);
        //Texture2D texture2D;
        //if (TierXManager.Instance.PinBackgrounds.ContainsKey(worldTourPinPinDetail.GetEventBackgroundTexture()))
        //{
        //    texture2D = TierXManager.Instance.PinBackgrounds[worldTourPinPinDetail.GetEventBackgroundTexture()];
        //}
        //else
        //{
        //    texture2D = this.LoadTexture(worldTourPinPinDetail.GetEventBackgroundTexture());
        //}
        Texture2D texture2D2;
        //if (TierXManager.Instance.PinTextures.ContainsKey(worldTourPinPinDetail.GetEventOverlayTexture()))
        //{
        //    texture2D2 = TierXManager.Instance.PinTextures[worldTourPinPinDetail.GetEventOverlayTexture()];
        //}
        //else
        //{
        texture2D2 = LoadTexture("Career/HighResPins/Restrictions/3_Pin_World_Tour_Pane");//worldTourPinPinDetail.GetEventOverlayTexture());
                                                                                               //}
                                                                                               //if (texture2D != null)
                                                                                               //{
                                                                                               //    this.EventSprite.GetComponent<Renderer>().material.SetTexture("_MainTex", texture2D);
                                                                                               //    EventPane.DoEPSpriteSetupDoubleWidth(this.EventSprite, texture2D);
                                                                                               //}
        if (texture2D2 != null)
        {
            EventSpriteOverlay.texture = texture2D2;
            //this.EventSpriteOverlay.GetComponent<Renderer>().material.SetTexture("_MainTex", texture2D2);
            //EventPane.DoEPSpriteSetup(this.EventSpriteOverlay, texture2D2);
        }
        //if (texture2D != null && texture2D2 != null)
        //{
        //    float x = (worldTourPinPinDetail.GetOverlayOffset().x - worldTourPinPinDetail.GetBackgroundOffset().x) / worldTourPinPinDetail.GetPinTextureScale().x * 2f;
        //    float num = (float)(texture2D2.height - texture2D.height) * 0.5f / 200f;
        //    float num2 = (worldTourPinPinDetail.GetOverlayOffset().y - worldTourPinPinDetail.GetBackgroundOffset().y) / worldTourPinPinDetail.GetPinTextureScale().y * 2f;
        //    float y = num + num2;
        //    this.EventSpriteOverlay.transform.localPosition = new Vector3(x, y, -0.1f);
        //}
        //}
        //ScheduledPin worldTourScheduledPinInfo = worldTourPinPinDetail.WorldTourScheduledPinInfo;
        //if (worldTourScheduledPinInfo.IsNextButtonLocked)
        //{
        //    this.SetLockedRaceButtonActive(true);
        //    this.SetRaceButtonActive(false);
        //}
        //else
        //{
        //    this.SetLockedRaceButtonActive(false);
        //    this.SetRaceButtonActive(true);
        //    this.RaceButton.SetState(GoRaceButton.eState.Normal);
        //    this.RaceButton.RaceText.Text = LocalizationManager.GetTranslation("TEXT_BUTTON_NEXT").ToUpper();
        //}
        //if (raceData.IsRandomRelay() && !raceData.IsGrindRelay() && RelayManager.GetRacesDone() == 0)
        //{
        //    this.EventGroup = raceData.Group;
        //    RelayManager.GenerateRelayOpponents(ref this.EventGroup);
        //}
        //if (raceData.SwitchBackRace)
        //{
        //    this.EventGroup = raceData.Group;
        //    RelayManager.SetupSwitchBack(ref this.EventGroup);
        //    this.EventData = this.EventGroup.RaceEvents[0];
        //}
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetupEventPinNormal(RaceEventData raceData)
    {

        //this.EventSprite.gameObject.SetActive(true);
        EventSpriteOverlay.gameObject.SetActive(true);
        EventSpriteBoss.gameObject.SetActive(false);
        if (raceData.Group.Parent != null)
        {
            string textureName;//= "Career/HighResPins/" + raceData.Group.Parent.GetBackgroundTextureName(raceData);
                               //Texture2D eventTexture = this.LoadTexture(textureName);
                               //this.EventSprite.texture = eventTexture;
                               ////EventPane.DoEPSpriteSetupDoubleWidth(this.EventSprite, texture2D);
            textureName = "Career/HighResPins/" + raceData.Group.Parent.GetOverlayTextureName(raceData) +
                          (!EventData.IsCarSpecificEvent() && !EventData.IsRestrictionEvent() && !EventData.IsManufacturerSpecificEvent() ? "_Pane" : "");
            var overlayTexture = LoadTexture(textureName);
            EventSpriteOverlay.texture = overlayTexture;
            ////EventPane.DoEPSpriteSetup(this.EventSpriteOverlay, texture2D2);
            //this.EventSpriteOverlay.gameObject.transform.localPosition = new Vector3(0f, (float)(texture2D2.height - texture2D.height) * 0.5f / 200f + raceData.Parent.GetOverlayOffset().y * 2f, -0.01f);
        }
    }

    private void NewEventGlowToUpdate(Color glowColor)
    {
        //this.EventGlow.GetComponent<Renderer>().material.SetColor("_Tint", glowColor);
    }

    private void NewEventFuelCostObjectsToUpdate(RaceEventData raceData, bool isFuelEnough)
    {
        FuelCost.gameObject.SetActive(true);
        FuelCostNumber.gameObject.SetActive(true);
        //string text = "Map_Screen/fuelcost_";
        Color color;
        if (isFuelEnough)
        {
            color = FuelEnoughColor;//new Color(0.0627451f, 0.407843143f, 0.65882355f);
            //text += "blue";
        }
        else
        {
            color = FuelNotEnoughColor;// new Color(0.572549045f, 0f, 0f);
            //text += "red";
        }
        //this.LoadAndSetTexture(this.FuelCost.GetComponent<Renderer>(), text);
        FuelCostNumber.text = GameDatabase.Instance.Currencies.GetFuelCostForEvent(EventData).ToString();
        FuelCostNumber.color = color;
        FuelCost.color = color;
    }

    private void SetEventName(string eventName)
    {
        NameSpriteT.text = eventName;
    }

    private void NewEventNameToUpdate(RaceEventData raceData)
    {
        //string text = LocalizationManager.GetTranslation(raceData.EventName.Trim());
        var parentName = raceData.Parent.GetName();
        string text = LocalizationManager.GetTranslation("TEXT_EVENT_PANE_NAME_" + parentName);

        SetEventName(text.ToUpper());
    }

    private void SetDescription(string description)
    {
        DescriptionSpriteT.text = description;
    }

    private void NewEventDescriptionToUpdate(RaceEventData raceData)
    {
        SetDescription(raceData.Parent.GetPinString(raceData));
    }

    public void OnInviteFriendsButtonPressed()
    {
        //SocialController.Instance.CheckFacebookPermissionsBeforePerformingInvite(SocialConfiguration.FacebookPermissionList.InviteButtonCheck);
    }

    private void NewEventLockInfoToUpdate(RaceEventData raceData)
    {
        //this.UnlockInfoT.gameObject.SetActive(false);
    }

    private void NewEventDistanceToUpdate(RaceEventData raceData)
    {
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (raceData.IsFriendRaceEvent())
        {
            DistanceSpriteT.gameObject.SetActive(false);
            return;
        }
        DistanceSpriteT.gameObject.SetActive(true);
        //string text = "Map_Screen/map_track_length_";
        if (raceData.IsHalfMile)
        {
            var text = activeProfile.UseMileAsUnit ? "TEXT_DISTANCE_HALF_MILE_NEW" : "TEXT_DISTANCE_HALF_MILE_IN_METER";
            DistanceSpriteT.text = LocalizationManager.GetTranslation(text);
            //text += "half";
        }
        else
        {
            var text = activeProfile.UseMileAsUnit ? "TEXT_DISTANCE_QUARTER_MILE_NEW" : "TEXT_DISTANCE_QUARTER_MILE_IN_METER";
            DistanceSpriteT.text = LocalizationManager.GetTranslation(text);
            //text += "quarter";
        }
        //this.SetUpSprite(this.DistanceGraphic, text, false);
    }

    private void NewEventDifficultyToUpdate(RaceEventData raceData)
    {
        //this.GrindRelayCarsText.gameObject.SetActive(raceData.IsGrindRelay());
        if (raceData.IsRaceTheWorldOrClubRaceEvent() || raceData.IsFriendRaceEvent() || !raceData.DoesMeetRestrictions(PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup) || raceData.IsRelay || raceData.ForceUserInCar)
        {
            DifficultySpriteT.gameObject.SetActive(false);
            DifficultyGraphic.gameObject.SetActive(false);
            //this.DifficultyShadowGraphic.gameObject.SetActive(false);
            return;
        }
        DifficultySpriteT.gameObject.SetActive(true);
        DifficultyGraphic.gameObject.SetActive(true);
        //this.DifficultyShadowGraphic.gameObject.SetActive(true);
        RaceEventDifficulty.Rating rating = RaceEventDifficulty.Instance.GetRating(raceData, false);
        //Debug.Log(rating+"   "+RaceGroupSelected);
        string @string = RaceEventDifficulty.Instance.GetString(rating);
        if (@string == "Easy")
        {
            difficulty = 0;
        }
        DifficultySpriteT.text = @string;
        DifficultySpriteT.color = DifficultyColors[(int)rating];
        string texture = RaceEventDifficulty.Instance.GetTexture(rating);
        SetUpSprite(DifficultyGraphic, texture, false);
        //this.isExtremeDifficultyEvent = this.ShouldShowDifficultyWarning(raceData);
        //if ((this.isExtremeDifficultyEvent && !raceData.IsWorldTourRace()) || (this.isExtremeDifficultyEvent && raceData.IsWorldTourRace() && !this.RestrictionPanel.IsRestrictionActive))
        //{
        //    if (this.ExtremeRaceBubble != null)
        //    {
        //        this.ExtremeRaceBubble.transform.parent.parent = null;
        //        this.ExtremeRaceBubble.Dismiss();
        //    }
        //    Vector3 offset = new Vector3(0f, 0.05f, -1f);
        //    float nipplePos = 0.9f;
        //    base.StartCoroutine(BubbleManager.Instance.ShowMessageDelayed(0.5f, "TEXT_BUBBLE_EXTREME_DIFFICULTY", false,
        //        /*this.DifficultyGraphic.transform*/transform, offset, BubbleMessage.NippleDir.DOWN, nipplePos,
        //        BubbleMessageConfig.ThemeStyle.SMALL,false, delegate(BubbleMessage bubble)
        //        {
        //            if (this.isExtremeDifficultyEvent && this.ExtremeRaceBubble == null)
        //            {
        //                this.ExtremeRaceBubble = bubble;
        //            }
        //            else
        //            {
        //                bubble.KillNow();
        //            }
        //        }));
        //}
        //else if (this.ExtremeRaceBubble != null)
        //{
        //    this.ExtremeRaceBubble.transform.parent.parent = null;
        //    this.ExtremeRaceBubble.Dismiss();
        //    this.ExtremeRaceBubble = null;
        //}
    }

    private void NewEventObjectsPositionToUpdate(RaceEventData raceData)
    {
        //float x = this.PaneWidth / 2f + 0.01f;
        //Vector3 vector = new Vector3(x, -this.MarginUp, -0.01f);
        //this.EventSprite.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //this.EventSpriteBoss.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //this.SavedEventSpritePosition = this.EventSprite.gameObject.transform.localPosition;
        //vector.y += this.YClipPosition + (this.MarginUp - 0.04f);
        //vector.z -= 0.1f;
        //this.EventGlow.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //vector.y -= this.SpaceBetweenSprites;
        //this.NameSpriteT.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //vector.y -= (float)this.NameSpriteT.GetDisplayLineCount() * this.NameSpriteT.BaseHeight + this.SpaceBetweenSprites;
        //this.DescriptionSpriteT.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        if (raceData.IsFriendRaceEvent())
        {
            //vector.y -= (float)this.DescriptionSpriteT.GetDisplayLineCount() * this.DescriptionSpriteT.BaseHeight + this.SpaceBetweenSprites * 5.5f;
            //vector.x = this.PaneWidth / 2f;
            //this.RYFLeaderBackground.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
            //vector.x += 0.18f;
            //vector.y -= 0.005f;
            //this.RYFLeader.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
            //vector.x = x;
            //vector.y -= this.RYFLeaderBackgroundSprite.height;
        }
        else
        {
            //vector.y -= (float)this.DescriptionSpriteT.GetDisplayLineCount() * this.DescriptionSpriteT.BaseHeight + this.SpaceBetweenSprites * 2f;
        }
        int easyCashReward;
        int normalCashReward;
        int hardCashReward;
        int easyGoldReward;
        int normalGoldReward;
        int hardGoldReward;
        if (isGroupSelected)
        {
            //vector.y -= 0.15f;
            //this.NormalButton.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
            //float num = this.NormalButton.Width - 0.15f;
            //vector.x -= num;
            //this.EasyButton.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector + new Vector3(0f, 0f, 0.01f));
            //vector.x += num * 2f;
            //this.HardButton.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector + new Vector3(0f, 0f, 0.02f));
            //vector.x -= num;
            //vector.y -= this.DistanceSpriteT.BaseHeight + this.SpaceBetweenSprites + 0.05f;
            var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            var basePPindex = CarDatabase.Instance.GetCar(activeProfile.GetCurrentCar().CarDBKey).PPIndex;
            var carTier = CarDatabase.Instance.GetCar(activeProfile.GetCurrentCar().CarDBKey).BaseCarTier;
            var baseCarTier = EventData.GetTierEvent().GetCarTier();
            easyGoldReward = EventGroup.RaceEvents[0].RaceReward.GoldPrize;
            normalGoldReward = EventGroup.RaceEvents[1].RaceReward.GoldPrize;
            hardGoldReward = EventGroup.RaceEvents[2].RaceReward.GoldPrize;
            if (EventGroup.RaceEvents[0].IsWorldTourRace())
            {
                easyCashReward = EventGroup.RaceEvents[0].RaceReward.CashPrize;
                normalCashReward = EventGroup.RaceEvents[1].RaceReward.CashPrize;
                hardCashReward = EventGroup.RaceEvents[2].RaceReward.CashPrize;
            }
            else
            {
                easyCashReward = EventGroup.RaceEvents[0].RaceReward.GetCashReward();//GameDatabase.Instance.Currencies.GetcashRewardByPPIndexAndDifficulty(basePPindex, 0, carTier, baseCarTier);//   EventGroup.RaceEvents[0].RaceReward.CashPrize;
                normalCashReward = EventGroup.RaceEvents[1].RaceReward.GetCashReward();//GameDatabase.Instance.Currencies.GetcashRewardByPPIndexAndDifficulty(basePPindex, (AutoDifficulty.DifficultyRating)1, carTier, baseCarTier); //EventGroup.RaceEvents[1].RaceReward.CashPrize;
                hardCashReward = EventGroup.RaceEvents[2].RaceReward.GetCashReward();//GameDatabase.Instance.Currencies.GetcashRewardByPPIndexAndDifficulty(basePPindex, (AutoDifficulty.DifficultyRating)2, carTier, baseCarTier); //EventGroup.RaceEvents[2].RaceReward.CashPrize;
            }


            var zTier = EventData.GetTierEvent().GetCarTier();
            var normalText = zTier == eCarTier.TIER_1
                ? string.Format(LocalizationManager.GetTranslation("TEXT_EVENT_PANE_DIFFICULTY_UNLOCK"),
                    LocalizationManager.GetTranslation(CrewChatter.GetMemberName((int)zTier, 2)))
                : null;
            var hardText = zTier == eCarTier.TIER_1
                ? string.Format(LocalizationManager.GetTranslation("TEXT_EVENT_PANE_DIFFICULTY_UNLOCK"),
                    LocalizationManager.GetTranslation(CrewChatter.GetMemberName((int)zTier, 3)))
                : null;
            SetRewardObjectsPosition(EasyGoldValueSpriteT, EasyCashValueSpriteT, easyGoldReward, easyCashReward, false, normalText);
            SetRewardObjectsPosition(NormalGoldValueSpriteT, NormalCashValueSpriteT, normalGoldReward,
                normalCashReward, !IsRookieUnlocked, normalText);
            SetRewardObjectsPosition(HardGoldValueSpriteT, HardCashValueSpriteT, hardGoldReward, hardCashReward, !IsProUnlocked, hardText);
            RewardObjects.SetActive(true);
            CashValueSpriteT.gameObject.SetActive(false);
        }
        else
        {
            RewardObjects.SetActive(false);
            CashValueSpriteT.gameObject.SetActive(true);
        }
        //this.DistanceSpriteT.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //this.SetDistanceObjectsPosition();
        //vector.y -= (float)this.DistanceSpriteT.GetDisplayLineCount() * this.DistanceSpriteT.BaseHeight;
        //vector.y -= this.DistanceGraphic.height + this.SpaceBetweenSprites;
        if (!EventData.IsRaceTheWorldOrClubRaceEvent() && !EventData.IsFriendRaceEvent())
        {
            //vector.y += this.SpaceBetweenSprites;
            //this.DifficultySpriteT.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
            //this.GrindRelayCarsText.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
            //this.SetDifficultyObjectsPosition();
            //vector.y -= (float)this.DifficultySpriteT.GetDisplayLineCount() * this.DifficultySpriteT.BaseHeight + this.SpaceBetweenSprites;
            //vector.y -= this.DifficultyGraphic.height;
        }
        //vector.y -= this.SpaceBetweenSprites - 0.05f;
        //this.RewardObjects.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        int gold = raceData.RaceReward.GoldPrize;
        int cash = raceData.RaceReward.GetCashReward();
        if (EventData.IsFriendRaceEvent())
        {
            gold = 0;
            cash = 0;
        }
        if (EventData.IsDailyBattle())
        {
            GetRewardForDailyBattle(out gold, out cash);
        }
        SetRewardObjectsPosition(GoldValueSpriteT, CashValueSpriteT, gold, cash, false, null);
        //vector.y = -this.PaneHeight + this.RaceButton.button.height / 2f + 0.02f;
        //this.BottomAlignedNode.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        SetupForRestrictionOrRaceButton();
       
    }

    private void TierXOverviewPositionUpdate()
    {
        //float x = this.PaneWidth / 2f + 0.01f;
        //Vector3 zPosition = new Vector3(x, -this.MarginUp, -0.01f);
        //this.EventSpriteBoss.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zPosition);
        //this.SavedEventSpritePosition = this.EventSpriteBoss.gameObject.transform.localPosition;
        //zPosition.y += this.YClipPosition + (this.MarginUp - 0.04f);
        //zPosition.z -= 0.1f;
        //this.EventGlow.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zPosition);
        //zPosition.y -= this.SpaceBetweenSprites;
        //this.NameSpriteT.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zPosition);
        //zPosition.y -= (float)this.NameSpriteT.GetDisplayLineCount() * this.NameSpriteT.BaseHeight + this.SpaceBetweenSprites;
        //this.DescriptionSpriteT.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zPosition);
        //zPosition.y = -this.PaneHeight + this.RaceButton.button.height / 2f + 0.02f;
        //this.BottomAlignedNode.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zPosition);
    }

    private void GetRewardForDailyBattle(out int gold, out int cash)
    {
        int num = PlayerProfileManager.Instance.ActiveProfile.DailyBattlesConsecutiveDaysCount;
        if (PlayerProfileManager.Instance.ActiveProfile.GetDaysSinceLastDailyBattle() == 1 || PlayerProfileManager.Instance.ActiveProfile.GetIsNextDailyBattleAfterMidnight())
        {
            num++;
        }
        DailyBattleReward reward = DailyBattleRewardManager.Instance.GetReward(num, RaceEventQuery.Instance.getHighestUnlockedClass(), true);
        gold = ((reward.RewardType != DailyBattleRewardType.Gold) ? 0 : reward.RewardValue);
        cash = ((reward.RewardType != DailyBattleRewardType.Cash) ? 0 : reward.RewardValue);
    }

    private void SetDistanceObjectsPosition()
    {
        //Vector3 zPosition = new Vector3(0f, 0f, 0.1f);
        //zPosition.y -= (float)this.DistanceSpriteT.GetDisplayLineCount() * this.DistanceSpriteT.BaseHeight;
        //zPosition.y -= this.DistanceGraphic.height / 2f;
        //zPosition.y += 0.02f;
        //this.DistanceGraphic.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zPosition);
    }

    private void SetDifficultyObjectsPosition()
    {
        //Vector3 zPosition = new Vector3(0f, 0f, 0.1f);
        //zPosition.y -= (float)this.DifficultySpriteT.GetDisplayLineCount() * this.DifficultySpriteT.BaseHeight;
        //zPosition.y -= this.DifficultyGraphic.height / 2f;
        //zPosition.y += 0.02f;
        //this.DifficultyGraphic.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zPosition);
    }

    private void SetRewardObjectsPosition(TextMeshProUGUI goldText, TextMeshProUGUI cashText, int gold, int cash, bool isLock, string lockString = null)
    {
        //Vector3 zero = Vector3.zero;
        //Vector3 zero2 = Vector3.zero;
        if (isLock)
        {
            cashText.color = Color.red;

            cashText.text = "<size=50%>" + lockString;
        }
        else if (gold == 0 && cash == 0)
        {
            //goldText.text = string.Empty;
            cashText.text = string.Empty;
        }
        else if (gold == 0)
        {
            cashText.text = CurrencyUtils.GetCashString(cash);
            //goldText.text = string.Empty;
            //this.CashValueSpriteT.Anchor = SpriteText.Anchor_Pos.Middle_Center;
        }
        else if (cash == 0)
        {
            //goldText.text = CurrencyUtils.GetGoldString(gold);
            cashText.text = string.Empty;
            //this.GoldValueSpriteT.Anchor = SpriteText.Anchor_Pos.Middle_Center;
        }
        else
        {
            cashText.text = CurrencyUtils.GetCashString(cash);
            //this.CashValueSpriteT.Anchor = SpriteText.Anchor_Pos.Middle_Left;
            //goldText.text = CurrencyUtils.GetGoldString(gold);
            //this.GoldValueSpriteT.Anchor = SpriteText.Anchor_Pos.Middle_Right;
            //float num = 0.03f;
            //float num2 = (this.CashValueSpriteT.GetWidth(this.CashValueSpriteT.Text) + this.GoldValueSpriteT.GetWidth(this.GoldValueSpriteT.Text)) / 2f;
            //zero.x = -num2 - num;
            //zero2.x = num2 + num;
        }
        //this.CashValueSpriteT.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zero);
        //this.GoldValueSpriteT.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zero2);
    }

    public void SetupForRestrictionOrRaceButton()
    {
        bool isRestrictionActive = RestrictionPanel.IsRestrictionActive;
        SetRaceButtonActive(true);//!isRestrictionActive);
        RaceButton.CurrentState = BaseRuntimeControl.State.Active;
        RestrictionPanel.gameObject.SetActive(isRestrictionActive);
    }

    private void SetClipPosition()
    {
        //float num = (this.YClipPosition - CommonUI.Instance.NavBar.GetHeight()) / GUICamera.Instance.ScreenHeight;
        //num = num * 2f + 1f;
        //this.EventSprite.gameObject.GetComponent<Renderer>().material.SetFloat("_ClipPos", num);
        //this.EventSpriteOverlay.gameObject.GetComponent<Renderer>().material.SetFloat("_ClipPos", num);
        //if (this.isTierLockedSelected)
        //{
        //    float num2 = (CareerModeMapScreen.carTierSelected != eCarTier.TIER_X) ? this.YClipPositionBoss : this.YClipPosition;
        //    num = (num2 - CommonUI.Instance.NavBar.GetHeight()) / GUICamera.Instance.ScreenHeight;
        //    num = num * 2f + 1f;
        //}
        //this.EventSpriteBoss.gameObject.GetComponent<Renderer>().material.SetFloat("_ClipPos", num);
    }

    private void SetBackgroundSize()
    {
        //float num = GUICamera.Instance.ScreenHeight - CommonUI.Instance.NavBar.GetHeight();
        //num += 0.04f;
        //this.BackgroundSprite.height = num;
        //this.BackgroundGlowSprite.height = num;
    }

    public void DeactivateAll()
    {
        gameObject.SetActive(false);
        isItFirstEventSelected = true;
    }

    public void ActivateAll()
    {
        gameObject.SetActive(true);
        ReactivateAfterLockedTier();
        isItFirstEventSelected = true;
    }

    public void ReactivateAfterLockedTier()
    {
        List<GameObject> list = new List<GameObject>
        {
            DifficultySpriteT.gameObject,
            DistanceSpriteT.gameObject,
            RaceButton.gameObject,
            RestrictionPanel.gameObject,
            RewardObjects,
            EasyButton.gameObject,
            NormalButton.gameObject,
            HardButton.gameObject,
            //this.CustomButtonB.gameObject,
            //this.CustomButtonT.gameObject,
            //this.EventSprite.gameObject,
			EventSpriteOverlay.gameObject,
            FuelCost.gameObject,
            //this.FacebookInviteButton.gameObject
		};
        list.ForEach(delegate (GameObject go)
        {
            go.SetActive(true);
        });
    }

    public void DeactivateForLockedTier()
    {
        List<GameObject> list = new List<GameObject>
        {
            DifficultySpriteT.gameObject,
            DistanceSpriteT.gameObject,
            RaceButton.gameObject,
            RestrictionPanel.gameObject,
            RewardObjects,
            EasyButton.gameObject,
            NormalButton.gameObject,
            HardButton.gameObject,
			//this.CustomButtonB.gameObject,
			//this.CustomButtonT.gameObject,
            //this.EventSprite.gameObject,
			EventSpriteOverlay.gameObject,
            FuelCost.gameObject,
            //this.FacebookInviteButton.gameObject,
			//this.RYFPanelInfo.gameObject,
			//this.RYFBottomAligned.gameObject,
            //this.LockedRaceButton.gameObject
		};
        list.ForEach(delegate (GameObject go)
        {
            go.SetActive(false);
        });
    }

    private Color GetTierColor(RaceEventData raceData)
    {
        eCarTier carTier = raceData.Parent.GetTierEvents().GetCarTier();
        return GameDatabase.Instance.Colours.GetTierColour(carTier);
    }

    public void OnAltButtonPressTop()
    {
        if (OnCustomPressTop != null)
        {
            OnCustomPressTop();
        }
    }

    public void OnAltButtonPressBottom()
    {
        if (OnCustomPressBot != null)
        {
            OnCustomPressBot();
        }
    }

    public void OnRacePress()
    {
        if (CurrentAlphaState == AlphaState.Hide)
        {
            return;
        }
        if (RestrictionPanel.IsRestrictionActive && RestrictionPanel.gameObject.activeSelf)
        {
            return;
        }
        if (OnRacePressed != null && TouchManager.AttemptToUseButton("GoRaceEvent"))
        {
            if (EventData != null)
            {
                if (ShouldShowDifficultyWarning(EventData))
                {
                    CarUpgradeSetup currentCarUpgradeSetup = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCarUpgradeSetup();
                    if (currentCarUpgradeSetup.IsFullyUpgraded())
                    {
                        PopUp popup = new PopUp
                        {
                            Title = "TEXT_POPUPS_NEED_BETTER_CAR_TITLE",
                            BodyText = "TEXT_POPUPS_NEED_BETTER_CAR_BODY",
                            IsBig = true,
                            ConfirmAction = StartRace,//new PopUpButtonAction(this.PopUpShowroom),
                            ConfirmText = "TEXT_BUTTON_RACE_ANYWAY",//"TEXT_BUTTON_SHOWROOM",
                            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                            ImageCaption = "TEXT_NAME_AGENT"
                        };
                        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
                        return;
                    }
                    PopUp popup2 = new PopUp
                    {
                        Title = "TEXT_POPUPS_EXTREME_DIFFICULTY_TITLE",
                        BodyText = "TEXT_POPUPS_EXTREME_DIFFICULTY_BODY",
                        ConfirmText = "TEXT_BUTTON_RACE_ANYWAY",//"TEXT_BUTTON_UPGRADE",
                        CancelText = "TEXT_BUTTON_BACK",
                        ConfirmAction = StartRace,//new PopUpButtonAction(this.JumpToUpgradeScreen),
                        GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                        ImageCaption = "TEXT_NAME_AGENT"
                    };
                    PopUpManager.Instance.TryShowPopUp(popup2, PopUpManager.ePriority.Default, null);
                    return;
                }

                if (EventData.IsDailyBattle())
                {
                    _isDailyBattleEventAvailable = IsDailyBattleAvailable();
                    if (!_isDailyBattleEventAvailable)
                    {
                        return;
                    }
                }

                if (EventData.IsRegulationRace())
                {
                    var carTier = EventData.Parent.GetTierEvents();
                    if (carTier == GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1)
                    {
                        Dictionary<Parameters, string> data = new Dictionary<Parameters, string>();
                        string first = "1";
                        
                        switch (difficulty)
                        {       
                            case 0:
                                if (!PlayerProfileManager.Instance.ActiveProfile.FirstEnterBeginnerRegulation)
                                {
                                    data.Add(Parameters.FirstEasyRegulationRace, first);
                                    LogEvent(Events.FirstBeginnerRegulationRaceAttemped, data);
                                    PlayerProfileManager.Instance.ActiveProfile.FirstEnterBeginnerRegulation = true;
                                    SaveProfile();
                                }
                                break;
                            case 1:
                                if (!PlayerProfileManager.Instance.ActiveProfile.FirstEnterNormalRegulation)
                                {
                                    data.Add(Parameters.FirstNormalRegulationRace, first);
                                    LogEvent(Events.FirstAmateurRegulationRaceAttemped, data);
                                    PlayerProfileManager.Instance.ActiveProfile.FirstEnterNormalRegulation = true;
                                    SaveProfile();
                                }
                                break;
                            case 2:
                                if (!PlayerProfileManager.Instance.ActiveProfile.FirstEnterHardRegulation)
                                {
                                    data.Add(Parameters.FirstHardRegulationRace, first);
                                    LogEvent(Events.FirstHardRegulationRaceAttemped, data);
                                    PlayerProfileManager.Instance.ActiveProfile.FirstEnterHardRegulation = true;
                                    SaveProfile();
                                }
                                break;
                        
                        }
                    }
                }
            }

            StartRace();
        }
    }
    
    private void SaveProfile()
    {
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
    }

    private void LogEvent(Events theEvent, Dictionary<Parameters, string> data)
    {
        #if !UNITY_EDITOR
        Log.AnEvent(theEvent, data);
        #endif
    }

    public void OnRestrictionPress()
    {
        if (CareerModeMapScreen.isDragging)
        {
            return;
        }
        if (TouchManager.AttemptToUseButton("RestrictionEventPressed"))
        {
            EventManager.Instance.PostEvent("MenuForward", EventAction.PlaySound, null);
            if (OnRestrictionPressed != null)
            {
                OnRestrictionPressed(EventData);
            }
            RestrictionPanel.NextPressed();
        }
    }

    public void StartRace()
    {
        RaceEventData raceEventData = SelectedRaceEvent();
        if (raceEventData != null && !raceEventData.IsFriendRaceEvent() && (CareerModeMapScreen.mapPaneSelected != MapPaneType.WorldTour || !TierXManager.Instance.IsOverviewThemeActive()))
        {
            m_RaceStarted = true;
            TouchManager.DisableButtonsFor(1.8f);
        }
        OnRacePressed(raceEventData);
        
        MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
    }

    public virtual RaceEventData SelectedRaceEvent()
    {
        return EventData;
    }

    public void JumpToUpgradeScreen()
    {
        ScreenManager.Instance.PushScreen(ScreenID.Tuning);
    }

    public static void ChangeRendererTint(Renderer aRenderer, float alpha)
    {
        if (aRenderer.material.HasProperty("_Tint"))
        {
            Color color = aRenderer.material.GetColor("_Tint");
            color.a = alpha;
            aRenderer.material.SetColor("_Tint", color);
        }
    }

    private bool IsFadeAnimationOver()
    {
        float time = AlphaFadingCurve[AlphaFadingCurve.length - 1].time;
        return CurrentTime >= time;
    }

    public void SkipAnimationIn()
    {
        ActivateAll();
        NewEventToShow(EventData);
        isItFirstEventSelected = false;
        float time = AlphaFadingCurve[AlphaFadingCurve.length - 1].time;
        CurrentTime = time;
        Update();
    }

    public void OnEasyButton()
    {
        SetNewGroupEvent(0, false);
        difficulty = 0;
    }

    public void OnNormalButton()
    {
        SetNewGroupEvent(1, false);
        difficulty = 1;
    }

    public void OnHardButton()
    {
        SetNewGroupEvent(2, false);
        difficulty = 2;
    }
    

    public void SetInitialDifficulty()
    {
        SetNewGroupEvent(RaceGroupSelected, true);
    }

    public void ClearEventData()
    {
        EventData = null;
        RestrictionPanel.ForceDisableRestrictions();
    }

    private void SetNewGroupEvent(int newGroup, bool setupOverride = false)
    {
        if (RaceGroupSelected == newGroup && !setupOverride)
        {
            return;
        }
        RaceGroupSelected = newGroup;
        EventData = EventGroup.RaceEvents[RaceGroupSelected];
        if (EventData.AutoDifficulty)
        {
            if (EventData.IsTestDrive())
            {
                CarUpgradeSetup upgradeSetup = new CarUpgradeSetup();
                ChooseRandomCars chooseRandomCars = new ChooseRandomCars();
                CarGarageInstance currentCar;
                chooseRandomCars.ChoosePlayerLoneCar(out currentCar, out upgradeSetup);
                EventData.SetLoanCarDetails(upgradeSetup, currentCar);
                AutoDifficulty.GetRandomOpponentForCarAtDifficulty((AutoDifficulty.DifficultyRating)RaceGroupSelected, ref EventData, currentCar);
                if (currentCar.Tier <= eCarTier.TIER_3)
                {
                    EventData.IsHalfMile = false;
                }
                else
                {
                    EventData.IsHalfMile = true;
                }
            }
            else if (EventData.IsGrindRelay())
            {
                if (RelayManager.GetRacesDone() != 0)
                {
                    return;
                }
                RelayManager.SetupGrindRelay(ref EventGroup, RaceGroupSelected);
                EventData = EventGroup.RaceEvents[RaceGroupSelected];
                //this.GrindRelayCarsText.Text = string.Format(LocalizationManager.GetTranslation("TEXT_RELAY_CAR_AMOUNT"), RelayManager.GetRaceCount(this.EventData));
            }
            else if (EventData.AutoHeadstart)
            {
                AutoDifficulty.GetRandomOpponent(ref EventData);
            }
            else
            {
                CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
                AutoDifficulty.GetRandomOpponentForCarAtDifficulty((AutoDifficulty.DifficultyRating)RaceGroupSelected, ref EventData, currentCar);
            }
        }
        ActivateAll();
        NewEventToShow(EventData);
    }

    private bool IsSelectedRaceGroupOK(RaceEventGroup eventGroup)
    {
        bool flag = RelayManager.IsGrindRelayGroup(eventGroup);
        if (RaceGroupSelected == 1)
        {
            return (!flag) ? IsRookieUnlocked : RelayManager.IsRookieUnlocked();
        }
        return RaceGroupSelected != 2 || ((!flag) ? IsProUnlocked : RelayManager.IsProUnlocked());
    }

    private void SetGroupButtons()
    {
        if (!isGroupSelected)
        {
            EasyButton.CurrentState = BaseRuntimeControl.State.Hidden;
            NormalButton.CurrentState = BaseRuntimeControl.State.Hidden;
            HardButton.CurrentState = BaseRuntimeControl.State.Hidden;
            return;
        }
        RaceEventData raceEventData = EventGroup.RaceEvents[0];
        EasyButton.CurrentState = BaseRuntimeControl.State.Active;
        if (raceEventData.IsGrindRelay())
        {
            NormalButton.CurrentState = ((!RelayManager.IsRookieUnlocked())
                ? BaseRuntimeControl.State.Disabled
                : BaseRuntimeControl.State.Active);
            HardButton.CurrentState = ((!RelayManager.IsProUnlocked())
                ? BaseRuntimeControl.State.Disabled
                : BaseRuntimeControl.State.Active);
        }
        else
        {
            NormalButton.CurrentState = ((!IsRookieUnlocked)
                ? BaseRuntimeControl.State.Disabled
                : BaseRuntimeControl.State.Active);
            HardButton.CurrentState = ((!IsProUnlocked)
                ? BaseRuntimeControl.State.Disabled
                : BaseRuntimeControl.State.Active);
        }
        switch (RaceGroupSelected)
        {
            case 0:
                EasyButton.CurrentState = BaseRuntimeControl.State.Highlight;
                break;
            case 1:
                NormalButton.CurrentState = BaseRuntimeControl.State.Highlight;
                break;
            case 2:
                HardButton.CurrentState = BaseRuntimeControl.State.Highlight;
                break;
        }
    }

    public static bool IsFuelEnough(RaceEventData raceData)
    {
        int fuel = FuelManager.Instance.GetFuel();
        int fuelCostForEvent = GameDatabase.Instance.Currencies.GetFuelCostForEvent(raceData);
        return fuel >= fuelCostForEvent;
    }

    public void SetUpForOnlineEvent(CustomPin.CustomType inCustomPinType)
    {
        if (inCustomPinType != CustomPin.CustomType.FriendRace)
        {
            return;
        }
        RaceEventData raceData =
            GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.FriendRaceEvents.RaceEventGroups[0].RaceEvents[0];
        OnEventSelected(raceData);
    }

    public void RaceYourFriendsSetup()
    {
        //this.RaceButton.RaceText.Text = LocalizationManager.GetTranslation("TEXT_BUTTON_NEXT").ToUpper();
        //if (CareerModeMapScreen.mapPaneSelected == -2)
        //{
        //    if (SocialController.Instance.isLoggedIntoFacebook && RYFStatusManager.HasBeenProcessed)
        //    {
        //        this.FacebookInviteButton.gameObject.SetActive(true);
        //        this.FacebookInviteButton.Button.Runtime.CurrentState = BaseRuntimeControl.State.Active;
        //        this.FacebookInviteButton.Button.SetText(LocalizationManager.GetTranslation("TEXT_INVITE_FRIENDS_BUTTON"), true, true);
        //    }
        //    else
        //    {
        //        this.FacebookInviteButton.gameObject.SetActive(false);
        //    }
        //    Color tierColour = GameDatabase.Instance.Colours.GetTierColour(CareerModeMapScreen.friendCarTier);
        //    this.NewEventGlowToUpdate(tierColour);
        //    this.RYFPanelInfo.SetActive(false);
        //    this.RYFBottomAligned.SetActive(false);
        //}
        //else
        //{
        //    int mapPaneSelected = CareerModeMapScreen.mapPaneSelected;
        //    this.RestrictionPanel.gameObject.SetActive(false);
        //    this.SetRaceButtonActive(true);
        //    this.TierColour = GameDatabase.Instance.Colours.GetTierColour(mapPaneSelected);
        //    this.BackgroundGlowSprite.gameObject.renderer.material.SetColor("_Tint", GameDatabase.Instance.Colours.GetTierColour(mapPaneSelected));
        //    this.NewEventGlowToUpdate(this.TierColour);
        //    if (SocialController.Instance.isLoggedIntoFacebook)
        //    {
        //        this.RYFPanelInfo.SetActive(true);
        //        RYFTierLeaderInfo component = this.RYFTierLeader.GetComponent<RYFTierLeaderInfo>();
        //        int num = component.Setup(mapPaneSelected);
        //        this.RYFTierLeader.SetActive(num > 0);
        //        this.RYFLeaderBackground.SetActive(num > 0);
        //    }
        //    else
        //    {
        //        this.RYFPanelInfo.SetActive(false);
        //    }
        //    this.RYFBottomAligned.SetActive(true);
        //    this.SetupLockedOrUnlocked(mapPaneSelected);
        //    this.FacebookInviteButton.gameObject.SetActive(false);
        //}
    }

    private void SetupLockedOrUnlocked(int tier)
    {
        //int totalStars = StarsManager.GetMyStarStats().TotalStars;
        //int num = GameDatabase.Instance.Friends.StarsRequiredToUnlockTier((eCarTier)tier);
        //if (totalStars >= num)
        //{
        //    int totalStars2 = StarsManager.GetMyStarStats((eCarTier)tier).TotalStars;
        //    int availableStarsForTier = StarsManager.GetAvailableStarsForTier((eCarTier)tier);
        //    int num2 = Math.Max(0, availableStarsForTier - totalStars2);
        //    string format = LocalizationManager.GetTranslation(string.Format("TEXT_TIER_{0}_STAR_COUNT", CarTierHelper.TierToString[tier]));
        //    string textID = (num2 != 1) ? "TEXT_RYF_STARS_REMAINING" : "TEXT_RYF_STAR_REMAINING";
        //    string str = string.Format(format, totalStars2).ToUpper();
        //    string str2 = string.Format(LocalizationManager.GetTranslation(textID), num2).ToUpper();
        //    this.MyStarUnlocked.Text = str + "\n" + str2;
        //    this.MyStarUnlockedShadow.Text = this.MyStarUnlocked.Text;
        //    GameObject[] lockedRYFObjects = this.LockedRYFObjects;
        //    for (int i = 0; i < lockedRYFObjects.Length; i++)
        //    {
        //        GameObject gameObject = lockedRYFObjects[i];
        //        gameObject.SetActive(false);
        //    }
        //    GameObject[] unlockedRYFObjects = this.UnlockedRYFObjects;
        //    for (int j = 0; j < unlockedRYFObjects.Length; j++)
        //    {
        //        GameObject gameObject2 = unlockedRYFObjects[j];
        //        gameObject2.SetActive(true);
        //    }
        //}
        //else
        //{
        //    this.LockedCriteriaText.Text = LocalizationManager.GetTranslation("TEXT_FRIEND_TIER_LOCKED");
        //    this.MyStarToUnlock.Text = (num - totalStars).ToString();
        //    this.MyStarToUnlockShadow.Text = this.MyStarToUnlock.Text;
        //    this.SetRaceButtonActive(false);
        //    GameObject[] lockedRYFObjects2 = this.LockedRYFObjects;
        //    for (int k = 0; k < lockedRYFObjects2.Length; k++)
        //    {
        //        GameObject gameObject3 = lockedRYFObjects2[k];
        //        gameObject3.SetActive(true);
        //    }
        //    GameObject[] unlockedRYFObjects2 = this.UnlockedRYFObjects;
        //    for (int l = 0; l < unlockedRYFObjects2.Length; l++)
        //    {
        //        GameObject gameObject4 = unlockedRYFObjects2[l];
        //        gameObject4.SetActive(false);
        //    }
        //}
    }

    private bool ShouldShowDifficultyWarning(RaceEventData eventData)
    {
        if (eventData.IsWorldTourRace()) // && TierXManager.Instance.IsOverviewThemeActive())
        {
            return false;
        }
        if (eventData.IsRelay)
        {
            return false;
        }
        if (eventData.ForceUserInCar)
        {
            return false;
        }
        if (eventData.IsRegulationRace())
        {
            return false;
        }
        CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        eCarTier currentTier = currentCar.CurrentTier;
        if (eventData.Parent.GetTierEvents().GetCarTier() != currentTier && !eventData.IsWorldTourRace() &&
            !eventData.IsRegulationRace())
        {
            return false;
        }
        RaceEventDifficulty.Rating rating = RaceEventDifficulty.Instance.GetRating(eventData, false);
        return rating == RaceEventDifficulty.Rating.Extreme &&
               PlayerProfileManager.Instance.ActiveProfile.RacesEntered > 1 && !eventData.IsFriendRaceEvent() &&
               !eventData.IsOnlineClubRacingEvent();
    }

    public void PopUpShowroom()
    {
        eCarTier tier = (EventData == null)
            ? PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup.BaseCarTier
            : CarDatabase.Instance.GetCar(EventData.AICar).BaseCarTier;
        ShowroomScreen.ShowScreenWithTierCarList(tier, false);
    }
}
