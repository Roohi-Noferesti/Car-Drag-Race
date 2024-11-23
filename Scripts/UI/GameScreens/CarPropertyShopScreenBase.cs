using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class CarPropertyShopScreenBase : ShopScreenBase 
{
    [SerializeField]
    protected VirtualItemType m_itemType;

    [SerializeField] protected CarPropertyScrollerController m_scroller;
    [SerializeField]
    private Animator m_scrollAnimator;

    [SerializeField] private ScreenID m_screenID;

    private CarInfo m_cashedCarInfo;
    private string[] m_cashedStickerIDs;
    private string m_initSelectedItemID;

    [SerializeField] private AudioSfx m_fitBleeps;


    public VirtualItemType ItemType
    {
        get { return m_itemType; }
    }

    public override ScreenID ID
    {
        get { return m_screenID; }
    }

    private int m_bodyDivisionIndex;

    public void SetBodyShaderDivisionIndex(int index)
    {
        //m_bodyDivisionIndex = index;
        //m_scroller.IDs = GetIDs();
        //(m_scroller as BodyShaderScrollerController).SetDivision(index);

        StartCoroutine(_changeBodyShaderDivisionIndex(index));
    }

    private IEnumerator _changeBodyShaderDivisionIndex(int index)
    {
        m_scrollAnimator.Play("off");
        yield return StartCoroutine(_checkScrollerAnimationEnd(false));
        m_bodyDivisionIndex = index;
        m_scroller.IDs = GetIDs();
        (m_scroller as BodyShaderScrollerController).SetDivision(index);
        m_scrollAnimator.Play("on");
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        m_scroller.SelectedIndexChanged += m_scroller_SelectedIndexChanged;
        m_scroller.IDs = GetIDs();
        m_scroller.Reload();
        m_initSelectedItemID = GetOwnedItemID();
        StartCoroutine(_ToggleSelectedItem(m_initSelectedItemID));
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        if (IsSelectedItemOwned)
            EquipItemID(m_scroller.SelectedID, true);
        else
        {
            EquipItemID(m_initSelectedItemID, true);
        }
        m_scroller.SelectedIndexChanged -= m_scroller_SelectedIndexChanged;
    }

    public string[] GetIDs()
    {
        switch (m_itemType)
        {
            case VirtualItemType.BodyShader:
                return CarPropertyItemIDs.GetBodyShaderIDsByDivision(m_bodyDivisionIndex);
            case VirtualItemType.RingShader:
                return CarPropertyItemIDs.RingShaderIDs;
            case VirtualItemType.HeadLighShader:
                return CarPropertyItemIDs.HeadlightShaderIDs;
            case VirtualItemType.CarSpoiler:
                return CarPropertyItemIDs.SpoilerIDs;
            case VirtualItemType.CarSticker:
                var carInfo = CarDatabase.Instance.GetCar(CurrentCar.CarDBKey);
                var stickerIds = CarPropertyItemIDs.StickerIDs;
                if (carInfo != null)
                {
                    if (carInfo == m_cashedCarInfo)
                    {
                        return m_cashedStickerIDs;
                    }
                    m_cashedCarInfo = carInfo;
                    List<string> result = new List<string>();
                    var carStickers = carInfo.Stickers.OrderBy(i => i).ToArray();
                    for (int i = 0; i < carStickers.Length; i++)
                    {
                        var stickerNumber = carStickers[i];
                        result.Add(stickerIds[stickerNumber + 1]);
                    }
                    //var result = carInfo.Stickers.Select(i => stickerIds[i + 1]).OrderBy(s=>s).ToList();
                    result.Insert(0, stickerIds[0]);
                    return m_cashedStickerIDs = result.ToArray();
                }
                return stickerIds;
        }
        return new string[] { };
    }

    private IEnumerator _ToggleSelectedItem(string itemID)
    {
        yield return new WaitForSeconds(0.2F);
        EquipItemID(itemID,false);
    }

    protected override void Awake()
    {
        m_dataDriven = true;
        base.Awake();
    }

    void m_scroller_SelectedIndexChanged(int obj)
    {
        RefreshUIElement_BlueButtonMode();
        RefreshUIElement_CostContainer();
        OnItemChanged(m_scroller.SelectedID);
    }

    public void EquipItemID(string itemID,bool closing)
    {
        if (m_itemType == VirtualItemType.BodyShader && SceneManagerGarage.Instance != null
            && SceneManagerGarage.Instance.currentCarVisuals != null && closing)
        {
            switch (m_itemType)
            {
                    case VirtualItemType.BodyShader:
                    SceneManagerGarage.Instance.SetupBodyMaterial(itemID);
                    break;
                    case VirtualItemType.RingShader:
                    SceneManagerGarage.Instance.SetupRingMaterial(itemID);
                    break;
                    case VirtualItemType.HeadLighShader:
                    SceneManagerGarage.Instance.SetupHeadlightMaterial(itemID);
                    break;
                    case VirtualItemType.CarSticker:
                    SceneManagerGarage.Instance.SetupSticker(CurrentCar.ID,itemID);
                    break;
                    case VirtualItemType.CarSpoiler:
                    SceneManagerGarage.Instance.SetupSpoiler(itemID);
                    break;
            }
        }
        else
        {
            m_scroller.SelectedID = itemID;
        }

        CurrentCar.SetAppliedItem(itemID, false);
    }

    protected virtual string GetOwnedItemID()
    {
        if (CurrentCar == null) return null;
        var equipedItemID = CurrentCar.GetEquipedVirtualItemID(m_itemType);
        if (string.IsNullOrEmpty(equipedItemID) && SceneManagerGarage.Instance != null && SceneManagerGarage.Instance.currentCarVisuals != null)
        {
            equipedItemID = SceneManagerGarage.Instance.currentCarVisuals.GetDefaultAssetItemID(ItemType.ToAssetType());
        }
        return equipedItemID;
    }

    protected virtual CarGarageInstance CurrentCar
    {
        get
        {
            if (PlayerProfileManager.Instance != null)
                return PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
            return null;
        }
    }

    protected override void OnBlueEvoUnlockButton()
    {
        throw new NotImplementedException();
    }

    protected override void OnBlueDeliverButton()
    {
        throw new NotImplementedException();
    }

    protected override void OnBlueBuyButton()
    {
        //eCarTier highestUnlockedClass = RaceEventQuery.Instance.getHighestUnlockedClass();
        //eCarTier highestCarTierOwned = PlayerProfileManager.Instance.ActiveProfile.GetHighestCarTierOwned();
        //if (highestCarTierOwned < highestUnlockedClass && highestCarTierOwned < eCarTier.TIER_5 && !PlayerProfileManager.Instance.ActiveProfile.DoneUpgradeWarningOnNewTier)
        //{
        //    this.DoAgentWarningPopup();
        //    PlayerProfileManager.Instance.ActiveProfile.DoneUpgradeWarningOnNewTier = true;
        //    return;
        //}
        if (this.CurrentCar == null)
        {
            return;
        }

        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        int cashPrice = this.GetCashPrice();
        int goldPrice = this.GetGoldPrice();
        m_itemBeenPurchased = GetItemBeenPurchased();
        if (!CanSelectedItemBePurchased)
        {
            if ((this.m_currentBuyMode == CostType.CASH || this.m_currentBuyMode == CostType.CASHANDGOLD) &&
                activeProfile.GetCurrentCash() < cashPrice)
            {
                //MiniStoreController.Instance.ShowMiniStoreNotEnoughCash(
                //    new ItemTypeId("upg", m_itemBeenPurchased.VirtualItemID), new ItemCost
                //    {
                //        CashCost = cashPrice,
                //        GoldCost = goldPrice
                //    }, "TEXT_POPUPS_INSUFFICIENT_CASH_BODY_UPGRADE", delegate
                //    {
                //        //this.DoGoldPurchase(goldPrice, this.CurrentCar, m_itemBeenPurchased.VirtualItemID);
                //    }, null, null, null);
                PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Cash,
                    "Customise",m_itemBeenPurchased,
                    () =>
                {
                    ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Cash, ShopScreen.PurchaseType.Buy,
                        ShopScreen.PurchaseSelectionType.Select);
                    ScreenManager.Instance.PushScreen(ScreenID.Shop);
                });
            }
            else
            {
                //MiniStoreController.Instance.ShowMiniStoreNotEnoughGold(
                //    new ItemTypeId("upg", m_itemBeenPurchased.VirtualItemID), new ItemCost
                //    {
                //        GoldCost = goldPrice
                //    }, "TEXT_POPUPS_INSUFFICIENT_GOLD_BODY_UPGRADE", null, null, null);
                PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Gold,
                    "Customise",m_itemBeenPurchased,() =>
                {
                    ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Gold, ShopScreen.PurchaseType.Buy,
                        ShopScreen.PurchaseSelectionType.Select);
                    ScreenManager.Instance.PushScreen(ScreenID.Shop);
                });
            }
            return;
        }

        if (m_currentBuyMode != CostType.CASH)
            cashPrice = 0;

        if (m_currentBuyMode != CostType.GOLD)
            goldPrice = 0;

        PopUpDatabase.Common.ShowBuyCarPropertyPopup(cashPrice, goldPrice,m_itemType,()=>OnConfirmBuyItem(false),null);
    }


    private void OnConfirmBuyItem(bool playSound)
    {
        CommonUI.Instance.XPStats.LevelUpLockedState(true);
        if (playSound)
        {
            MenuAudio.Instance.playSound(AudioSfx.Purchase);
            MenuAudio.Instance.playSound(m_fitBleeps);
        }
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //int currentCash = activeProfile.GetCurrentCash();
        //int currentGold = activeProfile.GetCurrentGold();
        int cashPrice = this.GetCashPrice();
        int goldPrice = this.GetGoldPrice();
        activeProfile.OrderPropertyItem(CurrentCar, m_currentBuyMode, cashPrice, goldPrice);
        CurrentCar.GivePropertyItem(m_scroller.SelectedID, false, m_itemType);
        if (CurrentCar.SetAppliedItem(m_scroller.SelectedID, false))
        {
            RefreshUIElement_CostContainer();
        }
        var xpPrize = GameDatabase.Instance.XPEvents.GetXPPrizeForPropertyPurchase();
        GameDatabase.Instance.XPEvents.AddPlayerXP(xpPrize);
        //AchievementChecks.CheckForAllUpgradesAchievement();
        //string hasShortCutDelivery = (m_currentBuyMode != CostType.FREE) ? "0" : "2";
        //this.CachePurchaseUpgradeMetricsEvent(activeProfile.GetCurrentCash() - currentCash,
        //    activeProfile.GetCurrentGold() - currentGold,
        //    this._purchaseInProgress.UpgradeBeingHandled.UpgradeType.ToString() +
        //    ((int)(this._purchaseInProgress.UpgradeBeingHandled.UpgradeLevel + 1)).ToString(), hasShortCutDelivery,
        //    string.Empty);
        CommonUI.Instance.XPStats.LevelUpLockedState(false);
        GameDatabase.Instance.ProgressionPopups.ShowProgressionPopupForScreen(ID);
    }

    protected override void OnBlueFitButton()
    {
        if (m_scroller.SelectedID != null)
        {
            if (IsSelectedItemOwned)
            {
                if (CurrentCar.SetAppliedItem(m_scroller.SelectedID, false))
                {
                    //MenuAudio.Instance.playSound(m_fitBleeps);
                    //var itemID = CurrentCar.GetCorrectItemID(m_scroller.SelectedID, false);
                    //ConnectionOrder.EquipItem(itemID, CurrentCar.CarDBKey);
                    RefreshUIElement_CostContainer();
                }
            }
        }
    }

    public override bool CanSelectedItemBePurchased
    {
        get
        {
            var playerCash = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash();
            var playerGold = PlayerProfileManager.Instance.ActiveProfile.GetCurrentGold();

            if (m_currentBuyMode == CostType.NONE)
            {
                return playerCash >= GetCashPrice() || playerGold >= GetGoldPrice();
            }

            return (m_currentBuyMode == CostType.CASH && playerCash >= GetCashPrice())
                   || (m_currentBuyMode == CostType.GOLD && playerGold >= GetGoldPrice());
        }
    }

    protected override bool IsSelectedItemFitted
    {
        get
        {
            return CurrentCar.GetEquipedVirtualItemID(m_itemType) == m_scroller.SelectedID;
        }
    }

    protected override bool IsSelectedItemOwned
    {
        get
        {
            //var defaultPropertyID = GarageManager.Instance.CarVisualInstance.GetDefaultAssetItemID(ItemType.ToAssetType());
            return CurrentCar.IsOwned(m_scroller.SelectedID, false);
        }
    }

    public override bool IsSelectedItemBeingDelivered
    {
        get { return false; }
    }
    protected override int GetCashPrice()
    {
        var defaultPropertyID = SceneManagerGarage.Instance.currentCarVisuals.GetDefaultAssetItemID(ItemType.ToAssetType());
        if (m_scroller.SelectedID == defaultPropertyID)
            return 0;

        var itemID = CurrentCar.GetCorrectItemID(m_scroller.SelectedID, false);
        if (string.IsNullOrEmpty(itemID))
            return 0;
        return GameDatabase.Instance.Prices.GetLiveryCashPrice(itemID);
        //return GameDatabase.Instance.ServerItemDatabase.GetCashPrice(itemID);
    }

    protected override string GetBonusID()
    {
        var itemID = CurrentCar.GetCorrectItemID(m_scroller.SelectedID, false);
        return itemID;
    }

    protected override int GetGoldPrice()
    {
        var defaultPropertyID = SceneManagerGarage.Instance.currentCarVisuals.GetDefaultAssetItemID(ItemType.ToAssetType());
        if (m_scroller.SelectedID == defaultPropertyID)
            return 0;
        var itemID = CurrentCar.GetCorrectItemID(m_scroller.SelectedID, false);

        if (string.IsNullOrEmpty(itemID))
            return 0;
        return GameDatabase.Instance.Prices.GetLiveryGoldPrice(itemID);
        //return GameDatabase.Instance.ServerItemDatabase.GetGoldPrice(itemID);
    }

    protected override string GetItemBeenPurchased()
    {
        var itemID = CurrentCar.GetCorrectItemID(m_scroller.SelectedID, false);
        return itemID;
    }


    private IEnumerator _checkScrollerAnimationEnd(bool open)
    {
        var endStateReached = false;
        while (!endStateReached)
        {
            if (!m_scrollAnimator.IsInTransition(0))
            {
                var stateInfo = m_scrollAnimator.GetCurrentAnimatorStateInfo(0);
                endStateReached =
                    stateInfo.IsName(open ? "on" : "off")
                    && stateInfo.normalizedTime >= 1;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    protected override void RefreshUIElement_BlueButtonMode()
    {
        this.m_currentBuyMode = CostType.NONE;
        this._blueButtonMode = TuningScreen.BlueButtonPressMode.None;
        if (this.IsSelectedItemOwned)
        {
            if (!this.IsSelectedItemFitted)
            {
                this._blueButtonMode = TuningScreen.BlueButtonPressMode.FitPart;
                OnBlueFitButton();
            }
        }
        else if (this.IsSelectedItemBeingDelivered)
        {
            this._blueButtonMode = TuningScreen.BlueButtonPressMode.DeliverPart;
        }
        else if (this.CanSelectedItemBePurchased)
        {
            this._blueButtonMode = TuningScreen.BlueButtonPressMode.BuyPart;
        }
    }

    protected override void RefreshUIElement_CostContainer()
    {
        if (this.IsSelectedItemOwned)
        {
            this.m_costContainer.SetupForAutoFit();
            //if (this.IsSelectedItemFitted)
            //{
            //    this.m_costContainer.SetupForBlueButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_FITTED"),
            //        BaseRuntimeControl.State.Disabled, OnBlueButton);
            //}
            //else
            //{
            //    this.m_costContainer.SetupForBlueButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_FIT"),
            //        BaseRuntimeControl.State.Active, OnBlueButton);
            //}
        }
        else
        {
            if (IsFree())
            {
                OnConfirmBuyItem(false);
            }
            else// if (this.CanSelectedItemBePurchased)
            {
                string title2 = (!TuningScreen.ExpressMode)
                    ? LocalizationManager.GetTranslation("TEXT_BUTTON_UPGRADE").ToUpper()
                    : CurrencyUtils.GetColouredGoldString(LocalizationManager.GetTranslation("TEXT_TRACKSIDE"));
                this.m_costContainer.SetupForCost(this.GetCashPrice(), this.GetGoldPrice(), title2,
                    OnBlueButtonCashOption, OnBlueButtonGoldOption, OnBlueButtonGoldOption);

            }
            //else
            //{
            //    this.m_costContainer.SetupForBlueButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_LOCKED"),
            //        BaseRuntimeControl.State.Disabled, OnBlueButton);
            //}
        }
    }
}
