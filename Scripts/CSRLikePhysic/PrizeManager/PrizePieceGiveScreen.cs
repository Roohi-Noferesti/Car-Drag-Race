using Metrics;
using System;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrizePieceGiveScreen : ZHUDScreen
{
    public static bool USE_FAKE_PRIZE_DATA;

    public static int FAKE_DATA_PARTS_UNCOVERED = 4;

    public static int FAKE_DATA_X_PARTS = 2;

    public static int FAKE_DATA_Y_PARTS = 2;

    public static string OnLoadPrizeGained = "FordFocusST";

    public TextMeshProUGUI CarName;

    public Image CarSprite;

    public Image GlowSprite;

    public Color GlowColor;

    public Image MovingGlowSprite;

    public Color MovingGlowColor;

    public float PartSeparationDistance;

    //public Blockeriser ourBlockeriser;

    public Animation PartRevealTimeline;

    public Animation CarRevealTimeline;

    public GameObject CarRevealedNode;

    public RuntimeTextButton ConfirmButton;

    public RuntimeTextButton ShareButton;

    public RuntimeTextButton ContinueButton;

    public GameObject UnlockNowParent;

    public RuntimeTextButton UnlockNowButton;

    public TextMeshProUGUI UnlockNowGoldText;

    public TextMeshProUGUI UnlockNowCashText;

    public TextMeshProUGUI UnlockNowText;

    public TextMeshProUGUI Title;

    public TextMeshProUGUI SubTitle;

    public GameObject LocalAnimBlock;

    public AnimationCurve screenShakeCurve;

    public static int PiecesToAward;

    private GUICameraShake cameraShake;

    private bool cameFromStreetView;

    private float blockeriserStartWidth;

    private float blockeriserStartSeparation;

    private Vector3 blockeriserStartPosition;

    private int PiecesUncovered;

    public static int PiecesPaidFor;

    //private EliteCarOverlay EliteCarBanner;

    public Vector3 EliteCarOverlayOffsetPieces = new Vector3(-0.15f, 0.525f, -0.2f);

    public Vector3 EliteCarOverlayOffsetFull = new Vector3(-0.803f, 0.525f, -0.2f);

    //private int CostToComplete;

    private CostType CostType = CostType.CASH;

    private Texture2D carTexture;

    public static bool AwardedNewPiece
    {
        get
        {
            return PrizePieceGiveScreen.PiecesToAward > 0;
        }
    }

    public override ScreenID ID
    {
        get
        {
            return ScreenID.PrizePieceGive;
        }
    }

    //protected override void Update()
    //{
    //    base.Update();
    //}

    //public override void OnActivate(bool zAlreadyOnStack)
    //{
    //    this.blockeriserStartWidth = this.ourBlockeriser.Width;
    //    this.blockeriserStartSeparation = this.ourBlockeriser.SeparationDistance;
    //    this.blockeriserStartPosition = this.ourBlockeriser.transform.localPosition;
    //    this.SetUpPrize(PrizePieceGiveScreen.OnLoadPrizeGained);
    //    this.GlowSprite.renderer.material.SetColor("_Tint", this.GlowColor);
    //    this.MovingGlowSprite.renderer.material.SetColor("_Tint", this.MovingGlowColor);
    //    this.SetShareButtonString(false);
    //    SocialController expr_8F = SocialController.Instance;
    //    expr_8F.OnGivenSocialReward = (Action)Delegate.Combine(expr_8F.OnGivenSocialReward, new Action(this.SetShareButtonStringAction));
    //    this.Title.Text = LocalizationManager.GetTranslation("TEXT_CAR_REWARD_CONGRATULATIONS_TITLE");
    //    this.SubTitle.Text = LocalizationManager.GetTranslation("TEXT_CAR_REWARD_CONGRATULATIONS_SUB_TITLE_1");
    //    this.ConfirmButton.SetText(LocalizationManager.GetTranslation("TEXT_CAR_REWARD_COLLECT_BUTTON"), true, true);
    //    this.ContinueButton.SetText(LocalizationManager.GetTranslation("TEXT_CAR_PART_CONTINUE"), true, true);
    //    this.UnlockNowText.Text = LocalizationManager.GetTranslation("TEXT_CAR_REWARD_UNLOCK_NOW");
    //    if (this.EliteCarBanner == null)
    //    {
    //        this.EliteCarBanner = EliteCarOverlay.Create(false);
    //    }
    //    this.EliteCarBanner.Setup(this.EliteCarOverlayOffsetPieces, this.ourBlockeriser.gameObject.transform, 0.48f, 0.32f);
    //    this.EliteCarBanner.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
    //    this.cameFromStreetView = !PrizePieceGiveScreen.AwardedNewPiece;
    //    this.SetDefaultState();
    //}

    //public override void OnDeactivate()
    //{
    //    base.OnDeactivate();
    //    SocialController expr_0B = SocialController.Instance;
    //    expr_0B.OnGivenSocialReward = (Action)Delegate.Remove(expr_0B.OnGivenSocialReward, new Action(this.SetShareButtonStringAction));
    //    PlayerProfileManager.Instance.ActiveProfile.UpdateCurrentPhysicsSetup();
    //    this.EndCameraShake();
    //}

    protected override void OnDestroy()
    {
        PrizePieceGiveScreen.USE_FAKE_PRIZE_DATA = false;
        base.OnDestroy();
    }

    private void ShowRewardText(bool show)
    {
        //this.CarRevealedNode.SetActive(show);
        //if (show)
        //{
        //    this.ConfirmButton.Runtime.CurrentState = BaseRuntimeControl.State.Active;
        //    this.ShareButton.Runtime.CurrentState = BaseRuntimeControl.State.Active;
        //    this.EliteCarBanner.gameObject.SetActive(true);
        //}
        //else
        //{
        //    this.ConfirmButton.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
        //    this.ShareButton.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
        //}
    }

    private void ShowContinue(bool show)
    {
        //if (show)
        //{
        //    this.ContinueButton.Runtime.CurrentState = BaseRuntimeControl.State.Active;
        //}
        //else
        //{
        //    this.ContinueButton.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
        //}
    }

    private void ShowUnlockNow(bool show)
    {
        //this.UnlockNowParent.gameObject.SetActive(show);
        //if (!show)
        //{
        //    return;
        //}
        //this.UnlockNowButton.Runtime.CurrentState = BaseRuntimeControl.State.Active;
        //if (this.CostType == CostType.CASH)
        //{
        //    string cashString = CurrencyUtils.GetCashString(this.CostToComplete);
        //    this.UnlockNowCashText.Text = cashString;
        //    this.UnlockNowCashText.gameObject.SetActive(true);
        //    this.UnlockNowGoldText.gameObject.SetActive(false);
        //}
        //else
        //{
        //    string goldString = CurrencyUtils.GetGoldString(this.CostToComplete);
        //    this.UnlockNowGoldText.Text = goldString;
        //    this.UnlockNowGoldText.gameObject.SetActive(true);
        //    this.UnlockNowCashText.gameObject.SetActive(false);
        //}
    }

    private void EndCameraShake()
    {
        if (this.cameraShake != null)
        {
            this.cameraShake.ShakeOver();
            this.cameraShake = null;
        }
    }

    public void SetShareButtonStringAction()
    {
        this.SetShareButtonString(false);
    }

    public void SetShareButtonString(bool forceGenericString = false)
    {
        //string text;
        //if (SocialController.Instance.SocialRewardAllowed() && !forceGenericString)
        //{
        //    string colouredCashString = CurrencyUtils.GetColouredCashString(GameDatabase.Instance.Social.GetCashRewardForTwitter());
        //    text = string.Format(LocalizationManager.GetTranslation("TEXT_SHARE_TO_GET"), colouredCashString);
        //}
        //else
        //{
        //    text = LocalizationManager.GetTranslation("TEXT_MENU_ICON_SHARE");
        //}
        //this.ShareButton.SetText(text, true, true);
    }

    private void SetDefaultState()
    {
        //base.animation.Stop();
        //this.PartRevealTimeline.Stop();
        //this.CarRevealTimeline.Stop();
        //this.EndCameraShake();
        //this.ShowRewardText(false);
        //this.ShowContinue(false);
        //this.ShowUnlockNow(false);
        //this.ourBlockeriser.transform.localPosition = this.blockeriserStartPosition;
        //this.ourBlockeriser.Width = this.blockeriserStartWidth;
        //this.ourBlockeriser.SeparationDistance = this.blockeriserStartSeparation;
    }

    public void LoadedAsset(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
    {
        this.carTexture = (zAssetBundle.mainAsset as Texture2D);
        AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
        this.SetUpBlockeriser(this.carTexture);
    }

    public void UnlockPart(CarPiece part)
    {
        //this.ourBlockeriser.UnlockPart(part);
        //this.PiecesUncovered++;
        //PrizePieceGiveScreen.PiecesToAward = 1;
        //AudioManager.PlaySound("RevealPiece", null);
    }

    private void SetUpPrize(string carId)
    {
        //this.CarName.Text = CarDatabase.Instance.GetCarNiceName(PrizePieceGiveScreen.OnLoadPrizeGained);
        //AssetProviderClient.Instance.RequestAsset("PrizeTex_" + PrizePieceGiveScreen.OnLoadPrizeGained, new BundleLoadedDelegate(this.LoadedAsset), this);
    }

    private void SetUpBlockeriser(Texture2D carTexture)
    {
        //int num = GameDatabase.Instance.Online.NumTotalCarPiecesToWin(PrizePieceGiveScreen.OnLoadPrizeGained);
        //int xDiv = num / 2;
        //int yDiv = 2;
        //PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //List<int> list = activeProfile.GetPiecesOfMultiplayerCarWon(PrizePieceGiveScreen.OnLoadPrizeGained);
        //if (PrizePieceGiveScreen.USE_FAKE_PRIZE_DATA)
        //{
        //    xDiv = PrizePieceGiveScreen.FAKE_DATA_X_PARTS;
        //    yDiv = PrizePieceGiveScreen.FAKE_DATA_Y_PARTS;
        //    this.PiecesUncovered = PrizePieceGiveScreen.FAKE_DATA_PARTS_UNCOVERED;
        //    list = new List<int>();
        //    for (int i = 0; i < this.PiecesUncovered; i++)
        //    {
        //        list.Add(i);
        //    }
        //}
        //List<int> list2 = new List<int>();
        //int count = list.Count;
        //PrizePieceGiveScreen.PiecesPaidFor = num - count;
        //for (int j = 0; j < PrizePieceGiveScreen.PiecesToAward; j++)
        //{
        //    list2.Add(list[list.Count - 1]);
        //    list.RemoveAt(list.Count - 1);
        //    this.PiecesUncovered = list.Count;
        //}
        //int num2 = GameDatabase.Instance.Online.GoldCostToCompleteCar(PrizePieceGiveScreen.OnLoadPrizeGained, count);
        //if (num2 > 0)
        //{
        //    this.CostType = CostType.GOLD;
        //    this.CostToComplete = num2;
        //}
        //else
        //{
        //    this.CostToComplete = GameDatabase.Instance.Online.CashCostToCompleteCar(PrizePieceGiveScreen.OnLoadPrizeGained, count);
        //    this.CostType = CostType.CASH;
        //}
        //this.ourBlockeriser.SetUp(xDiv, yDiv, list, carTexture);
        //foreach (int current in list2)
        //{
        //    this.UnlockPart(this.ourBlockeriser.GetPart(current));
        //}
        //this.DoAnimationTimeline();
    }

    private void OnContinue()
    {
        //if (SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend)
        //{
        //    while (ScreenManager.Instance.NominalNextScreen() == ScreenID.PrizePieceGive || ScreenManager.Instance.NominalNextScreen() == ScreenID.PrizeOMatic)
        //    {
        //        ScreenManager.Instance.PopScreen();
        //    }
        //}
        //else
        //{
        //    RaceController.Instance.BackToFrontend();
        //}
    }

    private void OnBuyCar()
    {
        string bodyText = string.Empty;
        if (this.CostType == CostType.GOLD)
        {
            //if (PlayerProfileManager.Instance.ActiveProfile.GetCurrentGold() < this.CostToComplete)
            //{
            //    MiniStoreController.Instance.ShowMiniStoreNotEnoughGold(new ItemTypeId("pcar", PrizePieceGiveScreen.OnLoadPrizeGained), new ItemCost
            //    {
            //        GoldCost = this.CostToComplete
            //    }, "TEXT_POPUPS_INSUFFICIENT_GOLD_BODY_PRIZE", null, null, null);
            //    return;
            //}
            //bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_GET_PRIZE_CAR"), CurrencyUtils.GetColouredCostStringBrief(0, this.CostToComplete,0));
        }
        else
        {
            //if (PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash() < this.CostToComplete)
            //{
            //    MiniStoreController.Instance.ShowMiniStoreNotEnoughCash(new ItemTypeId("pcar", PrizePieceGiveScreen.OnLoadPrizeGained), new ItemCost
            //    {
            //        CashCost = this.CostToComplete
            //    }, "TEXT_POPUPS_INSUFFICIENT_CASH_BODY", null, null, null, null);
            //    return;
            //}
            //bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_GET_PRIZE_CAR"), CurrencyUtils.GetColouredCostStringBrief(this.CostToComplete, 0,0));
        }
        PopUp popup = new PopUp
        {
            Title = "TEXT_POPUPS_BUY_CAR",
            BodyText = bodyText,
            BodyAlreadyTranslated = true,
            IsBig = true,
            ConfirmAction = new PopUpButtonAction(this.ConfirmBuyCar),
            CancelText = "TEXT_BUTTON_CANCEL",
            ConfirmText = "TEXT_BUTTON_BUY"
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
    }

    private void ConfirmBuyCar()
    {
        //PlayerProfileManager.Instance.ActiveProfile.BuyPrizeomaticCar(PrizePieceGiveScreen.OnLoadPrizeGained);
        //this.ourBlockeriser.RevealRemainingParts();
        //PrizePieceGiveScreen.PiecesToAward = 1;
        //if (PrizePieceGiveScreen.USE_FAKE_PRIZE_DATA)
        //{
        //    this.PiecesUncovered = PrizePieceGiveScreen.FAKE_DATA_X_PARTS * PrizePieceGiveScreen.FAKE_DATA_Y_PARTS;
        //}
        //AudioManager.PlaySound("RevealPiece", null);
        //if (!PrizePieceGiveScreen.USE_FAKE_PRIZE_DATA)
        //{
        //    CommonUI.Instance.NavBar.HideBackButton();
        //}
        //this.DoAnimationTimeline();
    }

    private void DoAnimationTimeline()
    {
        //this.SetDefaultState();
        //if (PrizePieceGiveScreen.AwardedNewPiece)
        //{
        //    int numPiecesOfMultiplyerCarWon = PlayerProfileManager.Instance.ActiveProfile.GetNumPiecesOfMultiplyerCarWon(PrizePieceGiveScreen.OnLoadPrizeGained);
        //    int num = GameDatabase.Instance.Online.NumTotalCarPiecesToWin(PrizePieceGiveScreen.OnLoadPrizeGained);
        //    int num2 = num - numPiecesOfMultiplyerCarWon;
        //    if (PrizePieceGiveScreen.USE_FAKE_PRIZE_DATA)
        //    {
        //        num2 = PrizePieceGiveScreen.FAKE_DATA_X_PARTS * PrizePieceGiveScreen.FAKE_DATA_Y_PARTS - this.PiecesUncovered;
        //    }
        //    if (num2 == 0)
        //    {
        //        this.EliteCarBanner.Setup(this.EliteCarOverlayOffsetFull, this.ourBlockeriser.gameObject.transform, 0.48f, 0.32f);
        //        this.EliteCarBanner.gameObject.SetActive(false);
        //        this.CarName.renderer.enabled = false;
        //        base.animation.Play("PrizeGiveScreen_GetCar");
        //    }
        //    else
        //    {
        //        base.animation.Play("PrizeGiveScreen_GetPiece");
        //        this.EliteCarBanner.Setup(this.EliteCarOverlayOffsetPieces, this.ourBlockeriser.gameObject.transform, 0.48f, 0.32f);
        //    }
        //}
        //else
        //{
        //    this.EliteCarBanner.Setup(this.EliteCarOverlayOffsetPieces, this.ourBlockeriser.gameObject.transform, 0.48f, 0.32f);
        //    this.ShowUnlockNow(true);
        //}
        //this.EliteCarBanner.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
    }

    private void Anim_RevealPart()
    {
        this.PartRevealTimeline.Play("PrizeGiveScreen_RevealPiece");
    }

    private void Anim_StartRevealAudio()
    {
        //AudioManager.PlaySound("RevealCar", null);
    }

    private void Anim_RevealCar()
    {
        this.CarRevealTimeline.Play("PrizeGiveScreen_RevealCar");
    }

    private void OnCollectCar()
    {
        //PrizeomaticCarPieceChooser.UpdateCarPool(true);
        //CommonUI.Instance.RPBonusStats.SetRPMultiplier(RPBonusManager.NavBarValue(), true);
        //if (ScreenManager.Instance.IsScreenOnStack(ScreenID.PrizeList))
        //{
        //    ScreenManager.Instance.RemoveScreenFromStack(ScreenID.PrizeList);
        //}
        //ScreenManager.Instance.SwapScreen(ScreenID.SportsPack);
        //PrizePieceGiveScreen.PiecesToAward = 0;
    }

    private void Anim_RevealCarButtons()
    {
        //CommonUI.Instance.NavBar.HideBackButton();
        //this.ShowRewardText(true);
    }

    private void Anim_EnableContinueButton()
    {
        if (!PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_FirstCarPartCompleted)
        {
            PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_FIRST_CAR_PIECE_TITLE", "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_FIRST_CAR_PIECE_BODY", new PopUpButtonAction(this.OnFirstCarPiecePopupDismissed), false);
            PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_FirstCarPartCompleted = true;
        }
        else
        {
            this.ShowContinue(true);
        }
        this.ShowUnlockNow(true);
    }

    private void Anim_ShakeCamera()
    {
        //this.cameraShake = GUICamera.Instance.gameObject.AddComponent<GUICameraShake>();
        //this.cameraShake.SetCurve(this.screenShakeCurve);
        //this.cameraShake.ShakeTime = Time.time;
    }

    private void OnFirstCarPiecePopupDismissed()
    {
        Log.AnEvent(Events.ComeBackSoon);
        this.ShowContinue(true);
        this.ShowUnlockNow(true);
    }

    private void OnShare()
    {
        //SocialController.Instance.OnShareButton(SocialController.MessageType.WIN_CAR, CarDatabase.Instance.GetCarNiceName(PrizePieceGiveScreen.OnLoadPrizeGained), false, false);
    }

    private void OnContinueTap()
    {
        //if (ScreenManager.Instance.IsScreenOnStack(ScreenID.PrizeList))
        //{
        //    ScreenManager.Instance.PopScreen();
        //}
        //else
        //{
        //    ScreenManager.Instance.SwapScreen(ScreenID.PrizeList);
        //}
    }

    //public override bool HasBackButton()
    //{
    //    if (PrizePieceGiveScreen.USE_FAKE_PRIZE_DATA)
    //    {
    //        return true;
    //    }
    //    int num = GameDatabase.Instance.Online.NumTotalCarPiecesToWin(PrizePieceGiveScreen.OnLoadPrizeGained);
    //    List<int> piecesOfMultiplayerCarWon = PlayerProfileManager.Instance.ActiveProfile.GetPiecesOfMultiplayerCarWon(PrizePieceGiveScreen.OnLoadPrizeGained);
    //    return this.cameFromStreetView && piecesOfMultiplayerCarWon.Count != num;
    //}
}
