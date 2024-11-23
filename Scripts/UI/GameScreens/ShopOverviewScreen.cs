using Metrics;
using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.UI;

public class ShopOverviewScreen : ZHUDScreen
{
    public float xSpaceBetweenGoldButtons;

    public Transform Logo;

    public ScrollRect ScrollRect;

    public RuntimeTextButton GoldButton;

    public RuntimeTextButton CashButton;

    public RuntimeTextButton OfferwallButton;

    public RuntimeTextButton BundleOfferButton;

    public RuntimeTextButton OfferPackButton;
    public RuntimeTextButton OfferPackButton_AppTutti;

    public RuntimeTextButton InstagramButton;

    public RuntimeTextButton TelegramButton;

    public RuntimeTextButton EarnCoinButton;
    public RuntimeTextButton FriendInvitationButton;


    public override ScreenID ID
    {
        get
        {
            return ScreenID.ShopOverview;
        }
    }

    public override void OnActivate(bool zAlreadyOnStack)
    {
        if (!zAlreadyOnStack)
        {
            try {
                Dictionary<Parameters, string> eventData = new Dictionary<Parameters, string>
                {
                    {Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
                    {Parameters.UIScreenID, ScreenManager.Instance.ActiveScreen.ID.ToString()},
                    {Parameters.Market, BasePlatform.ActivePlatform.GetTargetAppStore().ToString()},
                    {Parameters.TimeZone, BasePlatform.ActivePlatform.GetCity()}
                };
                Log.AnEvent(Events.PurchaseShopOpen, eventData);
            } catch {}
        }
        
        
        OfferWallConfiguration offerWallConfiguration = GameDatabase.Instance.Ad.GetOfferWallConfiguration();
        IGameState gameState = new GameStateFacade();
        if (offerWallConfiguration == null || offerWallConfiguration.Requirements == null || !offerWallConfiguration.Requirements.IsEligible(gameState))
        {
            this.OfferwallButton.gameObject.SetActive(false);
        }
        else
        {
            this.OfferwallButton.gameObject.SetActive(true);
            PlayerProfileManager.Instance.ActiveProfile.SeenOfferWallButton = true;
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        }



        if (BasePlatform.ActivePlatform.InsideCountry)
        {
            TelegramButton.CurrentState = BaseRuntimeControl.State.Hidden;
            //TelegramButton.CurrentState = PlayerProfileManager.Instance.ActiveProfile.HasAwardTelegramVisit
            //    ? BaseRuntimeControl.State.Hidden
            //    : BaseRuntimeControl.State.Active;
        }
        else
        {
            TelegramButton.CurrentState = BaseRuntimeControl.State.Hidden;
        }

        InstagramButton.CurrentState = PlayerProfileManager.Instance.ActiveProfile.HasAwardInstagramVisit
            ? BaseRuntimeControl.State.Hidden
            : BaseRuntimeControl.State.Active;
        this.OfferPackButton.gameObject.SetActive(!BuildType.IsAppTuttiBuild && GameDatabase.Instance.OfferPacks.GetEligibleOfferPacks().Count > 0);
        this.OfferPackButton_AppTutti.gameObject.SetActive(BuildType.IsAppTuttiBuild && GameDatabase.Instance.OfferPacks.GetEligibleOfferPacks().Count > 0);
        GoldButton.AddValueChangedDelegate(OnGold);
        CashButton.AddValueChangedDelegate(OnCash);
        OfferwallButton.AddValueChangedDelegate(OnOfferwall);
        BundleOfferButton.AddValueChangedDelegate(OnBundleOffer);
        OfferPackButton.AddValueChangedDelegate(OnOfferPacks);
        OfferPackButton_AppTutti.AddValueChangedDelegate(OnOfferPacks);
        InstagramButton.AddValueChangedDelegate(OnInstagram);
        TelegramButton.AddValueChangedDelegate(OnTelegram);
        EarnCoinButton.AddValueChangedDelegate(OnEarnCoin);
        FriendInvitationButton.AddValueChangedDelegate(OnFriendInvitation);
        base.OnActivate(zAlreadyOnStack);
        Log.AnEvent(Events.EnterShop);
        bool offerButtons = BundleOfferController.Instance.IsOfferActive();
        this.SetOfferButtons(offerButtons);
        ScrollRect.horizontalNormalizedPosition = 0;
    }

    public void SetOfferButtons(bool showBundleOffer)
    {
        //this.Logo.gameObject.SetActive(!showBundleOffer);
        this.BundleOfferButton.gameObject.SetActive(showBundleOffer);
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        GoldButton.RemoveValueChangedDelegate(OnGold);
        CashButton.RemoveValueChangedDelegate(OnCash);
        OfferwallButton.RemoveValueChangedDelegate(OnOfferwall);
        BundleOfferButton.RemoveValueChangedDelegate(OnBundleOffer);
        OfferPackButton.RemoveValueChangedDelegate(OnOfferPacks);
        OfferPackButton_AppTutti.RemoveValueChangedDelegate(OnOfferPacks);
        InstagramButton.RemoveValueChangedDelegate(OnInstagram);
        TelegramButton.RemoveValueChangedDelegate(OnTelegram);
        EarnCoinButton.RemoveValueChangedDelegate(OnEarnCoin);
        FriendInvitationButton.RemoveValueChangedDelegate(OnFriendInvitation);
    }

    private void OnGold()
    {
        LogClickOnShopCategoryEvent("Gold");
        
        ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Gold, ShopScreen.PurchaseType.Buy, ShopScreen.PurchaseSelectionType.Select);
        ScreenManager.Instance.PushScreen(ScreenID.Shop);
    }

    private void OnCash()
    {
        LogClickOnShopCategoryEvent("Cash");
        
        ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Cash, ShopScreen.PurchaseType.Buy, ShopScreen.PurchaseSelectionType.Select);
        ScreenManager.Instance.PushScreen(ScreenID.Shop);
    }

    private void OnOfferwall()
    {
        LogClickOnShopCategoryEvent("OfferWall");
        
        if (!OfferWallManager.Show())
        {
            PopUp popup = new PopUp
            {
                Title = "TEXT_POPUP_PLAYERLIST_NET_ERROR_TITLE",
                BodyText = "TEXT_NETWORK_UNAVAILABLE",
                IsBig = true,
                ConfirmText = "TEXT_BUTTON_OK"
            };
            PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
        }
    }

    private void OnOfferPacks()
    {
        LogClickOnShopCategoryEvent("OfferPacks");
        
        ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.OfferPack, ShopScreen.PurchaseType.Buy, ShopScreen.PurchaseSelectionType.Select);
        ScreenManager.Instance.PushScreen(ScreenID.Shop);
    }

    private void OnBundleOffer()
    {
        LogClickOnShopCategoryEvent("BundleOffer");
        
        BundleOfferController.Instance.TryShowIAPBundleForScreen(true);
    }

    private void LogClickOnShopCategoryEvent(string category)
    {
        try
        {
            Dictionary<Parameters, string> eventData = new Dictionary<Parameters, string>
            {
                {Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
                {Parameters.UIScreenID, ScreenManager.Instance.ActiveScreen.ID.ToString()},
                {Parameters.Market, BasePlatform.ActivePlatform.GetTargetAppStore().ToString()},
                {Parameters.TimeZone, BasePlatform.ActivePlatform.GetCity()},
                {Parameters.ShopCategory, category},
            };
            Log.AnEvent(Events.PurchaseShopCategoryClick, eventData);
        } catch {}
    }

    private void IAPCheck()
    {
        if (!AppStore.Instance.IAPEnabled && !PopUpManager.Instance.CurrentPopUpMatchesID(PopUpID.IAPMessage))
        {
            PopUpDatabase.Common.ShowIAPOffPopup();
        }
        else if (PopUpManager.Instance.CurrentPopUpMatchesID(PopUpID.IAPMessage))
        {
            PopUpManager.Instance.KillPopUp();
        }
    }


    public void OnTelegram()
    {
        Log.AnEvent(Events.TapTelegram);
        CommonUI.Instance.CashStats.CashLockedState(true);
        var url = GTPlatform.GetPlatformTelegramURL();
        GTDebug.Log(GTLogChannel.Screens,"telegram url : " + url);
        Application.OpenURL(url);
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (!activeProfile.HasAwardTelegramVisit)
        {
            var currentCash = activeProfile.GetCurrentCash();
            activeProfile.HasAwardTelegramVisit = true;
            StartCoroutine(m_delayedAwardCurrency(() =>
            {
                var cashReward = GameDatabase.Instance.SocialConfiguration.TelegramCashReward;
                PlayerProfileManager.Instance.ActiveProfile.AddCash(cashReward,"reward","telegram");
                TelegramButton.CurrentState = BaseRuntimeControl.State.Hidden;

                Log.AnEvent(Events.TelegramReward, new Dictionary<Parameters, string>()
                {
                    {Parameters.DGld, (activeProfile.GetCurrentCash() - currentCash).ToString()}
                });

                var body = string.Format(LocalizationManager.GetTranslation("TEXT_POPUP_TELEGRAM_AWARD_BODY"), CurrencyUtils.GetCashString(cashReward));
                PopUp popup = new PopUp
                {
                    Title = "TEXT_POPUP_TELEGRAM_AWARD_TITLE",
                    BodyText = body,
                    BodyAlreadyTranslated = true,
                    IsBig = true,
                    ConfirmText = "TEXT_BUTTON_COLLECT",
                    ConfirmAction = () =>
                    {
                        CommonUI.Instance.CashStats.CashLockedState(false);
                    },
                    ImageCaption = "TEXT_NAME_AGENT",
                    GraphicPath = PopUpManager.Instance.graphics_agentPrefab
                };
                PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
            }));
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        }
    }

    public void OnEarnCoin()
    {
        ScreenManager.Instance.PushScreen(ScreenID.Referral);
    }

    public void OnInstagram()
    {
        Log.AnEvent(Events.TapInstagram);
        CommonUI.Instance.CashStats.GoldLockedState(true);
        var url = GTPlatform.GetPlatformInstagramURL();
        GTDebug.Log(GTLogChannel.Screens,"instagram url : " + url);
        Application.OpenURL(url);
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (!activeProfile.HasAwardInstagramVisit)
        {
            int currentGold = activeProfile.GetCurrentGold();
            activeProfile.HasAwardInstagramVisit = true;
            StartCoroutine(m_delayedAwardCurrency(() =>
            {
                var goldReward = GameDatabase.Instance.SocialConfiguration.InstagramGoldReward;

                activeProfile.AddGold(goldReward,"reward","instagram");
                InstagramButton.CurrentState = BaseRuntimeControl.State.Hidden;

                Log.AnEvent(Events.InstagramReward, new Dictionary<Parameters, string>()
                {
                    {Parameters.DCsh, (activeProfile.GetCurrentGold() - currentGold).ToString()}
                });

                var body = string.Format(LocalizationManager.GetTranslation("TEXT_POPUP_INSTAGRM_REWARD_BODY"), CurrencyUtils.GetGoldStringWithIcon(goldReward));
                PopUp popup = new PopUp
                {
                    Title = "TEXT_POPUP_INSTAGRM_REWARD_TITLE",
                    BodyText = body,
                    BodyAlreadyTranslated = true,
                    IsBig = true,
                    ConfirmText = "TEXT_BUTTON_COLLECT",
                    ConfirmAction = () =>
                    {
                        CommonUI.Instance.CashStats.GoldLockedState(false);
                    },
                    ImageCaption = "TEXT_NAME_AGENT",
                    GraphicPath = PopUpManager.Instance.graphics_agentPrefab
                };
                PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
            }));
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        }
    }

    public void OnFriendInvitation()
    {
        ScreenManager.Instance.PushScreen(ScreenID.Referral);
        return;
        
        PopUpManager.Instance.TryShowPopUp(new PopUp
        {
            Title = "SHOPOVERVIEW_FRIENDINVITATION_BUTTON", 
            BodyText = LocalizationManager.GetTranslation("SHOPOVERVIEW_FRIENDINVITATION_POPUP_BODY"),
            BodyAlreadyTranslated = true,
            IsBig = true,
            ConfirmText = "TEXT_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT",
            ConfirmAction = () =>
            {
                string platformLink = GTPlatform.GetPlatformSuggestionURL();
                string userID = UserManager.Instance.currentAccount.UserID.ToString();
                string SharingText = string.Format(LocalizationManager.GetTranslation("SHOPOVERVIEW_FRIENDINVITATION_SHARE_MESSAGE"), platformLink, userID);
                SocialController.Instance.ShareNativeWithOptionalImage(SharingText, string.Empty);
            }
        }, PopUpManager.ePriority.Default, null);
    }

    private IEnumerator m_delayedAwardCurrency(Action action)
    {
        yield return new WaitForSeconds(3);
        action();
    }
}
