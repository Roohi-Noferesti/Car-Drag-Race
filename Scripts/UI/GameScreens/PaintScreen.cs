using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using Objectives;
using UnityEngine;
using UnityEngine.UI;
using Z2HSharedLibrary.DatabaseEntity;

public class PaintScreen : ShopScreenBase
{
    [SerializeField] private RuntimeTextButton m_bodyButton;
    [SerializeField] private RuntimeTextButton m_ringButton;
    [SerializeField] private RuntimeTextButton m_stickerButton;
    [SerializeField] private RuntimeTextButton m_headlightButton;
    [SerializeField] private RuntimeTextButton m_spoilerButton;
    [SerializeField] private RuntimeTextButton m_heightButton;

    [SerializeField] private GameObject m_mainPanel;
    [SerializeField] private GameObject m_bodyPanel;
    [SerializeField] private GameObject m_ringPanel;
    [SerializeField] private GameObject m_stickerPanel;
    [SerializeField] private GameObject m_headlightPanel;
    [SerializeField] private GameObject m_spoilerPanel;
    [SerializeField] private GameObject m_heightPanel;

    [SerializeField] protected CarPropertyScrollerController m_bodyPaintScroller;
    [SerializeField] protected CarPropertyScrollerController m_ringScroller;
    [SerializeField] protected CarPropertyScrollerController m_stickerScroller;
    [SerializeField] protected CarPropertyScrollerController m_headlightScroller;
    [SerializeField] protected CarPropertyScrollerController m_spoilerScroller;
    [SerializeField] private AudioSfx m_fitBleeps;
    [SerializeField] private Slider m_heightSlider;


    private VirtualItemType m_itemType;
    private CarInfo m_cashedCarInfo;
    private string[] m_cashedStickerIDs;
    private string m_initSelectedItemID;
    private int m_bodyDivisionIndex;
    protected CarPropertyScrollerController m_activeScroller;
    private bool m_initiated;
    private float m_initialHeight;


    public override ScreenID ID
    {
        get { return ScreenID.Customise; }
    }

    public VirtualItemType ItemType
    {
        get { return m_itemType; }
    }

    protected override void Awake()
    {
        m_dataDriven = true;
        base.Awake();
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        m_spoilerButton.CurrentState = SceneManagerGarage.Instance.currentCarVisuals.HasSpoiler?BaseRuntimeControl.State.Active : BaseRuntimeControl.State.Disabled;
        m_stickerButton.CurrentState =
            CarDatabase.Instance.GetCar(PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().CarDBKey).HasSticker
            ? BaseRuntimeControl.State.Active
            : BaseRuntimeControl.State.Disabled;
        m_bodyButton.CurrentState = !SceneManagerGarage.Instance.currentCarVisuals.HasFixedBodyColor ? BaseRuntimeControl.State.Active : BaseRuntimeControl.State.Disabled;
        m_bodyButton.AddValueChangedDelegate(OnBodyButton);
        m_ringButton.AddValueChangedDelegate(OnRingButton);
        m_stickerButton.AddValueChangedDelegate(OnStickerButton);
        m_headlightButton.AddValueChangedDelegate(OnHeadlightButton);
        m_spoilerButton.AddValueChangedDelegate(OnSpoilerButton);
        m_heightButton.AddValueChangedDelegate(OnHeightButton);
        ReturnToMainPanel(false);
    }


    public override void OnDeactivate()
    {
        base.OnDeactivate();
        if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
        {
            var currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
            if (SceneManagerGarage.Instance != null && SceneManagerGarage.Instance.currentCarVisuals != null
                && currentCar != null)
            {
                var carVisual = SceneManagerGarage.Instance.currentCarVisuals;
                var carinfo = CarDatabase.Instance.GetCar(currentCar.CarDBKey);
                ScreenShot.Instance.PlaceCarAndRender(carVisual, carinfo, true);
            }
        }

        m_bodyButton.RemoveValueChangedDelegate(OnBodyButton);
        m_ringButton.RemoveValueChangedDelegate(OnRingButton);
        m_stickerButton.RemoveValueChangedDelegate(OnStickerButton);
        m_headlightButton.RemoveValueChangedDelegate(OnHeadlightButton);
        m_spoilerButton.RemoveValueChangedDelegate(OnSpoilerButton);
        m_heightButton.RemoveValueChangedDelegate(OnHeightButton);
    }


    private void ReturnToMainPanel(bool saveProfile)
    {
        if(saveProfile)
            OnPaintDeactivate();
        HideAllPanel();
        m_mainPanel.SetActive(true);
        m_itemType = VirtualItemType.None;
        m_costContainer.gameObject.SetActive(false);
    }


    private void HideAllPanel()
    {
        m_mainPanel.SetActive(false);
        m_bodyPanel.SetActive(false);
        m_ringPanel.SetActive(false);
        m_headlightPanel.SetActive(false);
        m_stickerPanel.SetActive(false);
        m_spoilerPanel.SetActive(false);
        m_heightPanel.SetActive(false);
    }

    private void OnPaintActivate()
    {
        m_costContainer.gameObject.SetActive(true);
        m_activeScroller.SelectedIndexChanged += m_scroller_SelectedIndexChanged;
        m_activeScroller.IDs = GetIDs();
        m_activeScroller.Reload();
        m_initSelectedItemID = GetOwnedItemID();
        StartCoroutine(_ToggleSelectedItem(m_initSelectedItemID));
    }

    private void OnPaintDeactivate()
    {
        if (!m_heightPanel.activeInHierarchy && m_activeScroller != null)
        {
            if (IsSelectedItemOwned)
                EquipItemID(m_activeScroller.SelectedID, true);
            else
            {
                EquipItemID(m_initSelectedItemID, true);
            }
            m_activeScroller.SelectedIndexChanged -= m_scroller_SelectedIndexChanged;
        }
        else if(m_heightPanel.activeInHierarchy)
        {
            m_initiated = false;
            var carVisual = SceneManagerGarage.Instance.currentCarVisuals;
            if (m_initialHeight != carVisual.BodyHeight)
            {
                PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().BodyHeight = carVisual.BodyHeight;
                PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
            }
        }
    }

    private void OnHeightButton()
    {
        m_itemType = VirtualItemType.None;
        HideAllPanel();
        m_heightPanel.SetActive(true);
        m_activeScroller = null;

        var carVisual = SceneManagerGarage.Instance.currentCarVisuals;
        m_heightSlider.minValue = carVisual.MinBodyHeight;
        m_heightSlider.maxValue = carVisual.MaxBodyHeight;
        m_heightSlider.value = carVisual.BodyHeight;
        m_initialHeight = carVisual.BodyHeight;
        m_initiated = true;
    }

    private void OnSpoilerButton()
    {
        m_itemType = VirtualItemType.CarSpoiler;
        HideAllPanel();
        m_spoilerPanel.SetActive(true);
        m_activeScroller = m_spoilerScroller;
        OnPaintActivate();
    }

    private void OnHeadlightButton()
    {
        m_itemType = VirtualItemType.HeadLighShader;
        HideAllPanel();
        m_headlightPanel.SetActive(true);
        m_activeScroller = m_headlightScroller;
        OnPaintActivate();
    }

    private void OnStickerButton()
    {
        m_itemType = VirtualItemType.CarSticker;
        HideAllPanel();
        m_stickerPanel.SetActive(true);
        m_activeScroller = m_stickerScroller;
        OnPaintActivate();
    }

    private void OnRingButton()
    {
        m_itemType = VirtualItemType.RingShader;
        HideAllPanel();
        m_ringPanel.SetActive(true);
        m_activeScroller = m_ringScroller;
        OnPaintActivate();
    }

    private void OnBodyButton()
    {
        m_itemType = VirtualItemType.BodyShader;
        HideAllPanel();
        m_bodyPanel.SetActive(true);
        m_activeScroller = m_bodyPaintScroller;
        OnPaintActivate();
    }

    public override void StartAnimIn()
    {
        m_animator.Play("Open");
        CommonUI.Instance.PlayAnimation(true);
    }

    public override void StartAnimOut()
    {
        m_animator.Play("Close");
        CommonUI.Instance.PlayAnimation(false);
    }

    public override void RequestBackup()
    {
        if (m_mainPanel.activeInHierarchy)
        {
            ObjectiveCommand.Execute(new CarColorChanged(), true);
            base.RequestBackup();
        }
        else
        {
            ReturnToMainPanel(true);
        }
    }


    public void SetBodyShaderDivisionIndex(int index)
    {
        //m_bodyDivisionIndex = index;
        //m_scroller.IDs = GetIDs();
        //(m_scroller as BodyShaderScrollerController).SetDivision(index);

        StartCoroutine(_changeBodyShaderDivisionIndex(index));
    }

    private IEnumerator _changeBodyShaderDivisionIndex(int index)
    {
        var scrollAnimator = m_activeScroller.GetComponent<Animator>();
        scrollAnimator.Play("off");
        yield return StartCoroutine(_checkScrollerAnimationEnd(false));
        m_bodyDivisionIndex = index;
        m_activeScroller.IDs = GetIDs();
        (m_activeScroller as BodyShaderScrollerController).SetDivision(index);
        scrollAnimator.Play("on");
    }

    public string[] GetIDs()
    {
        switch (m_itemType)
        {
            
            case VirtualItemType.BodyShader:
                return CarPropertyItemIDs.GetSelectedBodyShaderIDsByDivision(m_bodyDivisionIndex);
            case VirtualItemType.RingShader:
                return CarPropertyItemIDs.RingSelectedShaderIDs;
            case VirtualItemType.HeadLighShader:
                return CarPropertyItemIDs.HeadlightSelectedShaderIDs;
            case VirtualItemType.CarSpoiler:
                return CarPropertyItemIDs.SpoilerSelectedIDs;
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
        EquipItemID(itemID, false);
    }

    void m_scroller_SelectedIndexChanged(int obj)
    {
        RefreshUIElement_BlueButtonMode();
        RefreshUIElement_CostContainer();
        OnItemChanged(m_activeScroller.SelectedID);
        CarPropertyItemChanger_ItemChanged(m_activeScroller.SelectedID);
    }


    void CarPropertyItemChanger_ItemChanged(string obj)
    {
        var assetType = m_itemType.ToAssetType();

        switch (assetType)
        {
            case ServerItemBase.AssetType.body_shader:
                SceneManagerGarage.Instance.SetupBodyMaterial(obj);
                break;
            case ServerItemBase.AssetType.ring_shader:
                SceneManagerGarage.Instance.SetupRingMaterial(obj);
                break;
            case ServerItemBase.AssetType.headlight_shader:
                SceneManagerGarage.Instance.SetupHeadlightMaterial(obj);
                break;
            case ServerItemBase.AssetType.sticker:
                var currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
                SceneManagerGarage.Instance.SetupSticker(currentCar.ID, obj);
                break;
            case ServerItemBase.AssetType.spoiler:
                SceneManagerGarage.Instance.SetupSpoiler(obj);
                break;
        }
    }

    public void EquipItemID(string itemID, bool closing)
    {
        if (SceneManagerGarage.Instance != null
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
                    SceneManagerGarage.Instance.SetupSticker(CurrentCar.ID, itemID);
                    break;
                case VirtualItemType.CarSpoiler:
                    SceneManagerGarage.Instance.SetupSpoiler(itemID);
                    break;
            }
        }
        else
        {
            m_activeScroller.SelectedID = itemID;
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
                MiniStoreController.Instance.ShowMiniStoreNotEnoughCash(
                    new ItemTypeId("upg", m_itemBeenPurchased), new ItemCost
                    {
                        CashCost = cashPrice,
                        GoldCost = goldPrice
                    }, "TEXT_POPUPS_INSUFFICIENT_CASH_BODY_UPGRADE", delegate
                    {
                        //this.DoGoldPurchase(goldPrice, this.CurrentCar, m_itemBeenPurchased.VirtualItemID);
                    }, null, null, null);
                //PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Cash,
                //    "Customise", m_itemBeenPurchased,
                //    () =>
                //    {
                //        ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Cash, ShopScreen.PurchaseType.Buy,
                //            ShopScreen.PurchaseSelectionType.Select);
                //        ScreenManager.Instance.PushScreen(ScreenID.Shop);
                //    });
            }
            else
            {
                MiniStoreController.Instance.ShowMiniStoreNotEnoughGold(
                    new ItemTypeId("upg", m_itemBeenPurchased), new ItemCost
                    {
                        GoldCost = goldPrice
                    }, "TEXT_POPUPS_INSUFFICIENT_GOLD_BODY_UPGRADE", null, null, null);
                //PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Gold,
                //    "Customise", m_itemBeenPurchased, () =>
                //    {
                //        ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Gold, ShopScreen.PurchaseType.Buy,
                //            ShopScreen.PurchaseSelectionType.Select);
                //        ScreenManager.Instance.PushScreen(ScreenID.Shop);
                //    });
            }
            return;
        }

        if (m_currentBuyMode != CostType.CASH)
            cashPrice = 0;

        if (m_currentBuyMode != CostType.GOLD)
            goldPrice = 0;

        PopUpDatabase.Common.ShowBuyCarPropertyPopup(cashPrice, goldPrice, m_itemType, () => OnConfirmBuyItem(true), null);
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
        int cashPrice = this.GetCashPrice();
        int goldPrice = this.GetGoldPrice();
        activeProfile.OrderPropertyItem(CurrentCar, m_currentBuyMode, cashPrice, goldPrice);
        CurrentCar.GivePropertyItem(m_activeScroller.SelectedID, false, m_itemType);
        if (CurrentCar.SetAppliedItem(m_activeScroller.SelectedID, false))
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
        if (m_activeScroller.SelectedID != null)
        {
            if (IsSelectedItemOwned)
            {
                if (CurrentCar.SetAppliedItem(m_activeScroller.SelectedID, false))
                {
                    //MenuAudio.Instance.playSound(m_fitBleeps);
                    //var itemID = CurrentCar.GetCorrectItemID(m_scroller.SelectedID, false);
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
            return CurrentCar.GetEquipedVirtualItemID(m_itemType) == m_activeScroller.SelectedID;
        }
    }

    protected override bool IsSelectedItemOwned
    {
        get
        {
            return CurrentCar.IsOwned(m_activeScroller.SelectedID, false);
        }
    }

    public override bool IsSelectedItemBeingDelivered
    {
        get { return false; }
    }
    protected override int GetCashPrice()
    {
        var defaultPropertyID = SceneManagerGarage.Instance.currentCarVisuals.GetDefaultAssetItemID(ItemType.ToAssetType());
        if (m_activeScroller.SelectedID == defaultPropertyID)
            return 0;

        var itemID = CurrentCar.GetCorrectItemID(m_activeScroller.SelectedID, false);
        if (string.IsNullOrEmpty(itemID))
            return 0;
        return GameDatabase.Instance.Prices.GetLiveryCashPrice(itemID);
    }
    
    protected override string GetBonusID()
    {
        var itemID = CurrentCar.GetCorrectItemID(m_activeScroller.SelectedID, false);
        return itemID;
    }

    protected override int GetGoldPrice()
    {
        var defaultPropertyID = SceneManagerGarage.Instance.currentCarVisuals.GetDefaultAssetItemID(ItemType.ToAssetType());
        if (m_activeScroller.SelectedID == defaultPropertyID)
            return 0;
        var itemID = CurrentCar.GetCorrectItemID(m_activeScroller.SelectedID, false);

        if (string.IsNullOrEmpty(itemID))
            return 0;
        return GameDatabase.Instance.Prices.GetLiveryGoldPrice(itemID);
    }

    protected override string GetItemBeenPurchased()
    {
        var itemID = CurrentCar.GetCorrectItemID(m_activeScroller.SelectedID, false);
        return itemID;
    }


    private IEnumerator _checkScrollerAnimationEnd(bool open)
    {
        var endStateReached = false;
        var scrollAnimator = m_activeScroller.GetComponent<Animator>();
        if (scrollAnimator == null)
        {
            yield break;
        }
        while (!endStateReached)
        {
            if (!scrollAnimator.IsInTransition(0))
            {
                var stateInfo = scrollAnimator.GetCurrentAnimatorStateInfo(0);
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
        this._blueButtonMode = TuningScreen.BlueButtonPressMode.BuyPart;
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
        UpdateBonusElement();
    }

    private void UpdateBonusElement()
    {
        if (!RemoteConfigABTest.CheckRemoteConfigValue())
        {
            return;
        }
        var itemID = CurrentCar.GetCorrectItemID(m_activeScroller.SelectedID, false);
        int goldCostFromAssetID = PaintScreen.GetGoldCostFromAssetID(itemID);
        m_costContainer.SetupForBonus(CurrentCar.Tier, goldCostFromAssetID, itemID);
    }
    protected override void Update()
    {
        base.Update();
        if (!m_initiated)
            return;
        var heightValue = m_heightSlider.value;
        SceneManagerGarage.Instance.SetupHeight(heightValue);
    }
    
    public static int GetGoldCostFromAssetID(string zAssetBundleID)
    {
        return GameDatabase.Instance.Prices.GetLiveryGoldPrice(zAssetBundleID);
    }
    
    private static int GetCashCostFromAssetID(string zAssetBundleID)
    {
        return GameDatabase.Instance.Prices.GetLiveryCashPrice(zAssetBundleID);
    }
    
    public static int CalculateLiveryRaceBonus(eCarTier tier, int LiveryGoldCost, string itemId)
    {
        int baseLiveryBonus = 0;
        float liveryGoldMultiplier = 0f;
        float liveryTierMultiplier = 0f;
        float customizationTypeMultiplier = 0f;
        GameDatabase.Instance.Currencies.getLiveryBonusAmounts(tier, itemId, out baseLiveryBonus, out liveryGoldMultiplier, out liveryTierMultiplier, out customizationTypeMultiplier);
        int num = 0;
        if (LiveryGoldCost > 0)
        {
            num = Mathf.CeilToInt(Mathf.Log(LiveryGoldCost, 2.71828f));
        }
        return Mathf.CeilToInt(((float)baseLiveryBonus + liveryGoldMultiplier * (float)num) * liveryTierMultiplier * customizationTypeMultiplier);
    }
}
