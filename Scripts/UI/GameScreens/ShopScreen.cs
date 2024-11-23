using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
//using KingKodeStudio.IAB;
using Metrics;
using TMPro;
using UnityEngine;

public class ShopScreen : ZHUDScreen,IBundleOwner
{
    public enum ItemType
    {
        Cash,
        Gold,
        SuperNitro,
        RaceCrew,
        Restore,
        DirectPurchase,
        OfferPack
    }

    private enum State
    {
        None,
        Initialisation,
        WaitingForAppStoreProducts,
        ChooseAProduct,
        PurchasingFromAppStore,
        NoNetwork,
        CannotMakePaymentsOnDevice,
        ThereAreNoProducts,
        WaitingForAppStoreType
    }

    public enum PurchaseType
    {
        Buy,
        Renew
    }

    public enum PurchaseSelectionType
    {
        Select,
        Automatic
    }

    public Sprite[] GoldSprites;
    public Sprite[] CashSprites;
    public Sprite[] OfferPackSprites;

    private const string IAP_ID = "IAPMsg";

    private static string DirectPurchaseIdentifier;

    private static Action DirectPurchaseCallback;

    private static Action DirectPurchaseFailureCallback;

    public GameObject StopClicksButton;

    public GameObject LoadingIcon;

    public TextMeshProUGUI status;

    public GameObject SuperNitrousUI;

    //public DataDrivenPortrait SuperNitrousCharacter;

    public GameObject RaceCrewUI;

    //public DataDrivenPortrait RaceCrewCharacter;

    public TextMeshProUGUI RaceCrewDescription;

    public TextMeshProUGUI RaceCrewButtonTitle;

    public TextMeshProUGUI RaceCrewButtonText;

    public ShopListItem ShopItemPrefab;

    public OfferPackShopListItem OfferPackItemPrefab;

    public GameObject Layout;

    private State _currentState;

    private string _activePurchaseTitle = string.Empty;

    private ProductData _superNitroProduct;

    private ProductData _raceCrewProduct;

    private ProductData _directPurchaseProduct;

    private bool _resetNextUpdateFlag;

    private bool _isWaitingForUIAssetBundle;

    private GameObject _uiScreenPrefab;

    private GameObject _uiShopList;

    private List<ShopListItem> m_itemList = new List<ShopListItem>();

    public static ItemType ItemTypeToShow
    {
        get;
        private set;
    }

    public static PurchaseType PurchaseTypeToShow
    {
        get;
        private set;
    }

    public static PurchaseSelectionType SelectionTypeToUse
    {
        get;
        private set;
    }

    private GameObject _uiPoster;

    private RuntimeButton _uiNitrousButton;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.Shop;
        }
    }

    public static void InitialiseForPurchase(ItemType itemType, PurchaseType purchaseType, PurchaseSelectionType selectionType)
    {
        ItemTypeToShow = itemType;
        PurchaseTypeToShow = purchaseType;
        SelectionTypeToUse = selectionType;
    }

    public static void InitialiseForDirectPurchase(string iapIdentifier, Action SuccessCallback = null, Action FailureCallback = null)
    {
        ItemTypeToShow = ItemType.DirectPurchase;
        SelectionTypeToUse = PurchaseSelectionType.Automatic;
        PurchaseTypeToShow = PurchaseType.Buy;
        DirectPurchaseIdentifier = iapIdentifier;
        DirectPurchaseCallback = SuccessCallback;
        DirectPurchaseFailureCallback = FailureCallback;
    }

    public static string RequestPriceStringForIAP(string iapIdentifier)
    {
        List<AppStoreProduct> products = AppStore.Instance.GetProducts();
        if (products != null)
        {
            AppStoreProduct appStoreProduct = products.Find((AppStoreProduct item) => item.Identifier.Contains(iapIdentifier));
            if (appStoreProduct != null)
            {
                return appStoreProduct.LocalisedPrice;
            }
            return string.Empty;
        }
        else
        {
            return LocalizationManager.GetTranslation("TEXT_PRICE_UNKNOWN");
        }
    }

    private static List<ProductData> FilterProductList(ItemType type)
    {
        List<ProductData> list = new List<ProductData>();
        List<AppStoreProduct> products = AppStore.Instance.GetProducts();
        foreach (AppStoreProduct current in products)
        {
            GTProduct productWithID = ProductManager.Instance.GetProductWithID(current.Identifier.ToLower());
            if (productWithID != null)
            {
                ProductData item = new ProductData(current, productWithID);
                switch (type)
                {
                    case ItemType.Cash:
                        if (current.Identifier.Contains("cash"))
                        {
                            list.Add(item);
                        }
                        break;
                    case ItemType.Gold:
                        if (current.Identifier.Contains("gold"))
                        {
                            list.Add(item);
                        }
                        break;
                    case ItemType.SuperNitro:
                        if (current.Identifier.Contains("super"))
                        {
                            list.Add(item);
                        }
                        break;
                    case ItemType.RaceCrew:
                        {
                            string value = (PurchaseTypeToShow != PurchaseType.Renew) ? GameDatabase.Instance.IAPs.WholeTeamConsumableProductCode : GameDatabase.Instance.IAPs.WholeTeamRenewalProductCode;
                            if (current.Identifier.Contains(value))
                            {
                                list.Add(item);
                            }
                            break;
                        }
                    case ItemType.DirectPurchase:
                        if (current.Identifier.Contains(DirectPurchaseIdentifier))
                        {
                            list.Add(item);
                        }
                        break;
                    case ItemType.OfferPack:
                        if (current.Identifier.ToLower().Contains("offerpack"))
                        {
                            list.Add(item);
                        }
                        break;
                }
            }
        }
        
        Log.AnEvent(Events.ShopProductsArrived,new Dictionary<Parameters, string>()
        {
            {Parameters.Count , list.Count.ToString(CultureInfo.InvariantCulture)},
            {Parameters.Type , type.ToString()},
        });
        
        int num = 0;
        foreach (ProductData current2 in list)
        {
            current2.AnimFrameIndex = num;
            num++;
        }

        try
        {
            var abtestCode = UserManager.Instance.currentAccount.ABTestCode;
            if (string.IsNullOrEmpty(abtestCode) || abtestCode.ToLower().Contains("Default"))
            {
                list.Sort((ProductData x, ProductData y) => x.GtProduct.SortIndex.CompareTo(y.GtProduct.SortIndex));
                return list;
            }
            else
            {
                return list.OrderBy(i => i.GtProduct.SortIndex).ToList();
            }
        }
        catch (Exception e)
        {
            list.Sort((ProductData x, ProductData y) => x.GtProduct.SortIndex.CompareTo(y.GtProduct.SortIndex));
            return list;
        }
    }

    protected override void OnDestroy()
    {
        status = null;
        base.OnDestroy();
    }

    public override void OnActivate(bool zAlreadyOnStack)
    {
        base.OnActivate(zAlreadyOnStack);
        AppStore.Instance.TransactionResult += OnTransactionResult;
        //_uiNitrousButton.AddValueChangedDelegate(OnBuySuperNitrous);
        SetState(State.WaitingForAppStoreType);
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        /*if (GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.LadderEvents.NumEventsComplete() == 2 && 
            activeProfile.DailyBattlesLastEventAt > DateTime.MinValue && activeProfile.HasBoughtFirstUpgrade && activeProfile.HasSeenFacebookNag && 
            !activeProfile.HasVisitedManufacturerScreen && !LegacyObjectivesManager.IsLegacyObjectiveCompleted("IntroduceCarDealer"))
        {*/
            Log.AnEvent(Events.TapToShop);
        //}
      //  IAPCheck();
      //  ApplicationManager.DidBecomeActiveEvent += IAPCheck;
    }

    private void SetUpProductMenuForCurrentRes()
    {
        //this._uiShopList.DesiredMenuItemWidth = GUICamera.Instance.ScreenWidth - GUICamera.Instance.ScreenWidth / 8f;
        //this._uiShopList.Height = GUICamera.Instance.ScreenHeight - 0.2f - CommonUI.Instance.NavBar.Background.height;
        //float zNewY = (0.2f - CommonUI.Instance.NavBar.Background.height) / 2f;
        //GameObjectHelper.SetLocalY(this._uiShopList.gameObject, zNewY);
        if (ItemTypeToShow == ItemType.Cash)
        {
            Log.AnEvent(Events.EnterCashShop);
        }
        else if (ItemTypeToShow == ItemType.Gold)
        {
            Log.AnEvent(Events.EnterGoldShop);
        }
    }

    public override void OnDeactivate()
    {
        //_uiNitrousButton.RemoveValueChangedDelegate(OnBuySuperNitrous);
        _superNitroProduct = null;
        _raceCrewProduct = null;
        ApplicationManager.DidBecomeActiveEvent -= IAPCheck;
        AppStore.Instance.TransactionResult -= OnTransactionResult;
        base.OnDeactivate();
    }

    protected void OnItemClick(ShopListItem zItem)
    {
        Log.AnEvent(Events.ClickOnShopItem);

        try
        {
            Dictionary<Parameters, string> eventData = new Dictionary<Parameters, string>
            {
                {Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
                {Parameters.UIScreenID, ScreenManager.Instance.ActiveScreen.ID.ToString()},
                {Parameters.Market, BasePlatform.ActivePlatform.GetTargetAppStore().ToString()},
                {Parameters.TimeZone, BasePlatform.ActivePlatform.GetCity()},
                {Parameters.Itm, zItem.Product.GtProduct.Code}
            };
            Log.AnEvent(Events.PurchaseShopItemClick, eventData);
        } catch {}
        
        MakePurchase(zItem.Product.AppStoreProduct);//smkp
   
    }

    protected override void Update()
    {
        base.Update();
        if (_resetNextUpdateFlag && !AppStore.Instance.IsProcessingTransaction && SelectionTypeToUse == PurchaseSelectionType.Select)
        {
            _resetNextUpdateFlag = false;
            SetState(State.Initialisation);
            return;
        }
        if (BoostNitrous.HaveBoostNitrous() && ItemTypeToShow == ItemType.SuperNitro)
        {
            ScreenManager.Instance.PopScreen();
            return;
        }

        if (_currentState == State.WaitingForAppStoreType)
        {
            return;
        }

        if (CheckForConnectivityStateChangingEvents())
        {
            return;
        }

        if (_currentState == State.NoNetwork || _currentState == State.CannotMakePaymentsOnDevice)
        {
            return;
        }
        if (_currentState == State.Initialisation || _currentState == State.WaitingForAppStoreProducts)
        {
            ChangeStateWhenProductDataArrives();
            return;
        }
        if (SelectionTypeToUse == PurchaseSelectionType.Automatic && _currentState == State.ChooseAProduct && !PopUpManager.Instance.isShowingPopUp)
        {
            ItemType itemTypeToShow = ItemTypeToShow;
            if (itemTypeToShow != ItemType.Restore)
            {
                if (itemTypeToShow == ItemType.DirectPurchase)
                {
                    OnBuyDirectPurchase();
                }
            }
            else
            {
                RestorePurchases();
            }
            SelectionTypeToUse = PurchaseSelectionType.Select;
        }
    }

    private void ChangeStateWhenProductDataArrives()
    {
        if (AppStore.Instance.IsWaitingForProductData || _isWaitingForUIAssetBundle)
        {
            SetState(State.WaitingForAppStoreProducts);
        }
        else if (AppStore.Instance.GetProducts()==null || AppStore.Instance.GetProducts().Count == 0)
        {
            SetState(State.ThereAreNoProducts);
        }
        else if (AppStore.Instance.IsProcessingTransaction)
        {
            SetState(State.PurchasingFromAppStore);
        }
        else
        {
            this.InitUIForCurrentItemType(false);
            if (this.RebuildScreen())
            {
                this.SetState(ShopScreen.State.ChooseAProduct);
            }
        }
    }

    private bool ConnectivityStateChanged = false;

    private bool CheckForConnectivityStateChangingEvents()
    {
        if (!PolledNetworkState.IsNetworkConnected)
        {
            SetState(State.NoNetwork);
            ConnectivityStateChanged = false;
            return true;
        }

        if (!AppStore.Instance.IAPEnabled)
        {
            SetState(State.CannotMakePaymentsOnDevice);
            ConnectivityStateChanged = false;
            return true;
        }

        if (!ConnectivityStateChanged)
            SetState(State.WaitingForAppStoreProducts);
        ConnectivityStateChanged = true;
        return false;
    }

    private void SetState(State zState)
    {
        if (_currentState == zState)
        {
            return;
        }
        _currentState = zState;
        switch (_currentState)
        {
            case State.WaitingForAppStoreType:
                ShowAppStoreSelectionPopup(_currentState);
                break;
            case State.Initialisation:
                InitUIForCurrentItemType(true);
                break;
            case State.WaitingForAppStoreProducts:
                AppStore.Instance.StartProductRequestIfStillWaiting();
                Layout.gameObject.SetActive(false);
                CommonUI.Instance.ShowBackButton();
                status.text = LocalizationManager.GetTranslation("TEXT_SHOP_SCREEN_WAITING_APP_STORE_PRODUCTS");
                ShowOverlay(true, true);
                break;
            case State.ChooseAProduct:
                Layout.gameObject.SetActive(true);
                CommonUI.Instance.ShowBackButton();
                status.text = string.Empty;
                ShowOverlay(false, false);
                break;
            case State.PurchasingFromAppStore:
                Layout.gameObject.SetActive(false);
                CommonUI.Instance.HideBackButton();
                status.text = LocalizationManager.GetTranslation("TEXT_SHOP_SCREEN_PURCHASING_FROM_APP_STORE");
                ShowOverlay(true, true);
                break;
            case State.NoNetwork:
                Layout.gameObject.SetActive(false);
                CommonUI.Instance.ShowBackButton();
                status.text = LocalizationManager.GetTranslation("TEXT_SHOP_SCREEN_NO_NETWORK");
                ShowOverlay(true, false);
                break;
            case State.CannotMakePaymentsOnDevice:
                Layout.gameObject.SetActive(false);
                CommonUI.Instance.ShowBackButton();
                status.text = LocalizationManager.GetTranslation("TEXT_SHOP_SCREEN_CANNOT_MAKE_PAYMENTS");
                ShowOverlay(true, false);
                break;
            case State.ThereAreNoProducts:
                Layout.gameObject.SetActive(false);
                CommonUI.Instance.ShowBackButton();
                status.text = LocalizationManager.GetTranslation("TEXT_SHOP_SCREEN_NO_PRODUCTS");
                ShowOverlay(true, false);
                break;
        }
    }

    private void ShowAppStoreSelectionPopup(State currentState)
    {
//        bool isGooglePlay = false;
//#if UNITY_ANDROID
//        isGooglePlay = KingIAB.Setting.IsGooglePlay;
//#endif

//        var account = UserManager.Instance.currentAccount;
        //if (!isGooglePlay || !BasePlatform.ActivePlatform.InsideFortumoZone || account.UserConverted)
        //{
            AppStore.Instance.EnsureAppStoreCurrectSetting();
            IAPCheck();
            SetState(State.Initialisation);
        //}
        //else
        //{
        //    //CONFIRM= FORTUMO
        //    //CANCEL= BASE PAYMENT LIKE GOOGLE PLAY 
        //    PopUp popUp = new PopUp();
        //    popUp.Title = "TEXT_POPUP_FORTUMO_TITLE";
        //    popUp.BodyText = "TEXT_POPUP_FORTUMO_BODY";
        //    popUp.CancelText = "TEXT_POPUP_FORTUMO_CONFIRM";
        //    popUp.ConfirmText = "TEXT_POPUP_FORTUMO_CANCEL";
        //    popUp.CancelAction = () =>
        //    {
        //        AppStore.Instance.SwitchAppStoreToFortumo();
        //        UserManager.Instance.currentAccount.IsFortumo = true;
        //        SaveAppStoreSetting();
        //    };
        //    popUp.ConfirmAction = SaveAppStoreSetting;

        //    PopUpManager.Instance.TryShowPopUp(popUp, PopUpManager.ePriority.Default, null);
        //}
    }

    private void SaveAppStoreSetting()
    {
        var currentAccount = UserManager.Instance.currentAccount;
        currentAccount.HasChosenBaseStoreOrFortumo = true;
        SetState(State.Initialisation);
        IAPCheck();
        ApplicationManager.DidBecomeActiveEvent += IAPCheck;
        JsonDict AccountParams = new JsonDict();
        AccountParams.Set("username", "user" + currentAccount.UserID);
        AccountParams.Set("is_fortumo", currentAccount.IsFortumo);
        AccountParams.Set("has_chosen_base_or_fortumo", currentAccount.HasChosenBaseStoreOrFortumo);
        WebRequestQueue.Instance.StartCall("save_fortumo_settings", "Save user Data", AccountParams, SaveUserDataResponseCallback, null, ProduceHashSource(AccountParams));
    }

    private void SaveUserDataResponseCallback(string zHTTPContent, string zError, int zStatus, object zUserData)
    {
        if (zStatus != 200 || !string.IsNullOrEmpty(zError))
        {
            Debug.LogError("Error saving user data :" + zError);
        }
        else
        {
            UserManager.Instance.SaveCurrentAccount();
            GTDebug.Log(GTLogChannel.Screens,"saving user data successfully .");
        }
    }
    private string ProduceHashSource(JsonDict dict)
    {
        string text = string.Empty;
        foreach (string current in dict.Keys)
        {
            text += dict.GetString(current);
        }
        return text;
    }



    private bool RebuildScreen()
    {
        _superNitroProduct = null;
        _raceCrewProduct = null;
       
        foreach (var shopListItem in m_itemList)
        {
            DestroyImmediate(shopListItem.gameObject);
        }
        this.m_itemList.Clear();
        switch (ItemTypeToShow)
        {
            case ItemType.Cash:
            case ItemType.Gold:
                RebuildNormalProductListMenu();
                break;
            case ItemType.SuperNitro:
                RebuildNitroMenu();
                break;
            case ItemType.RaceCrew:
                RebuildRaceCrewMenu();
                break;
            case ItemType.DirectPurchase:
                GetDirectPurchaseProduct();
                break;
            case ItemType.OfferPack:
                return ReBuildOfferPackMenu();
        }
        return true;
    }

    private void GetDirectPurchaseProduct()
    {
        List<ProductData> list = FilterProductList(ItemTypeToShow);
        if (list.Count > 0)
        {
            _directPurchaseProduct = list[0];
        }
    }

    private void RebuildNitroMenu()
    {
        List<ProductData> list = FilterProductList(ItemTypeToShow);
        if (list.Count > 0)
        {
            _superNitroProduct = list[0];
        }
    }

    private void RebuildRaceCrewMenu()
    {
        List<ProductData> list = FilterProductList(ItemTypeToShow);
        if (list.Count > 0)
        {
            _raceCrewProduct = list[0];
            RaceCrewButtonText.text = _raceCrewProduct.AppStoreProduct.LocalisedPrice;
        }
    }

    private bool ReBuildOfferPackMenu()
    {
        List<ProductData> list = FilterProductList(ItemTypeToShow);
        List<OfferPackData> eligibleOfferPacks = GameDatabase.Instance.OfferPacks.GetEligibleOfferPacks();
        List<ProductData> list2 = new List<ProductData>();
        bool flag = false;
        foreach (ProductData product in list)
        {
            var offerPackData = eligibleOfferPacks.FirstOrDefault(q => product.GtProduct.Code.ToLower() == q.ProductCode.ToLower());
            if (offerPackData != null)
            {
                flag = true;
                product.AnimFrameIndex = offerPackData.AnimFrameIndex;
                list2.Add(product);
                if (list2.Count == 3)
                {
                    CreateItemList(list2, ItemType.OfferPack, ShopItemPrefab.gameObject);

                    //this.AddShopPageItemToList(list2, ShopScreen.ItemTypeToShow, this._uiShopList.DesiredMenuItemWidth, 0, 1);
                    list2.Clear();
                }
            }
        }
        if (list2.Count<ProductData>() != 0)
        {
            CreateItemList(list2, ItemType.OfferPack, ShopItemPrefab.gameObject);
            //this.AddShopPageItemToList(list2, ShopScreen.ItemTypeToShow, this._uiShopList.DesiredMenuItemWidth, 0, 1);
        }
        if (flag)
        {
            //base.MarkAllItemsAddedToCarousels();
            //this._uiPips.SetNumPips(this._uiShopList.NumItems);
            //this._uiPips.SetActivePip(this._uiShopList.SelectedIndex);
        }
        else
        {
            this.SetState(State.ThereAreNoProducts);
        }
        return flag;
    }

    private void RebuildNormalProductListMenu()
    {
        List<ProductData> list = FilterProductList(ItemTypeToShow);
        CreateItemList(list, ItemTypeToShow, _uiScreenPrefab);
        //this._uiPips.SetNumPips(this._uiShopList.NumItems);
        //this._uiPips.SetActivePip(this._uiShopList.SelectedIndex);
    }


    public void CreateItemList(List<ProductData> products, ItemType productType, GameObject uiPrefab)
    {
        //Debug.Log("creating shop length : " + products.Count + "  ,  " + productType);
        for (int i = 0; i < products.Count; i++)
        {
            //Debug.Log("creating shop item "+i+" , "+products[i].AppStoreProduct.Title);
            ShopListItem shopItem;
            if (productType == ItemType.OfferPack)
            {
                shopItem = GetOfferPackShopItem();
                var frameIndex = products[i].AnimFrameIndex;

                if (BuildType.IsAppTuttiBuild)
                {
                    if (frameIndex == 0)
                        frameIndex = 5;
                    if (frameIndex == 4)
                        frameIndex = 6;
                }
				
                (shopItem as OfferPackShopListItem).Create(products[i], uiPrefab,OfferPackSprites[frameIndex]);
            }
            else
            {
                shopItem = GetShopItem();
                var sprites = ItemTypeToShow == ItemType.Gold ? GoldSprites : CashSprites;
                shopItem.Create(products[i], productType, uiPrefab, sprites[products[i].AnimFrameIndex]);
            }
            shopItem.Tap += OnItemClick;
            shopItem.transform.SetParent(Layout.transform,false);
            m_itemList.Add(shopItem);
        }
    }


    public ShopListItem GetShopItem()
    {
        return Instantiate(ShopItemPrefab.gameObject).GetComponent<ShopListItem>();
    }

    public OfferPackShopListItem GetOfferPackShopItem()
    {
        return Instantiate(OfferPackItemPrefab.gameObject).GetComponent<OfferPackShopListItem>();
    }

    private string GetLocalisedTime(int minutes)
    {
        int num = minutes / 60;
        return (num <= 0) ? string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_MINUTES"), minutes) : string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_HOURS"), num);
    }

    private void InitUIForCurrentItemType(bool shouldRequestBundle = true)
    {
        ShowOverlay(false, false);
        status.text = string.Empty;
        //CommonUI.Instance.NavBar.HideBackButton();
        //this._uiShopList.Clear();
        _superNitroProduct = null;
        _raceCrewProduct = null;
        if (ItemTypeToShow != ItemType.Cash && ItemTypeToShow != ItemType.Gold)
        {
            //this._uiShopList.CurrentState = BaseRuntimeControl.State.Hidden;
        }
        SuperNitrousUI.SetActive(ItemTypeToShow == ItemType.SuperNitro);
        RaceCrewUI.SetActive(ItemTypeToShow == ItemType.RaceCrew);
        switch (ItemTypeToShow)
        {
            case ItemType.Cash:
            case ItemType.Gold:
            case ItemType.OfferPack:
                SetUpProductMenuForCurrentRes();
                //_isWaitingForUIAssetBundle = true;
                //if (shouldRequestBundle)
                //{
                //    RequestUIAssetBundle();
                //}
                break;
            case ItemType.SuperNitro:
                //this.SuperNitrousCharacter.Init(PopUpManager.Instance.graphics_mechanicPrefab, "TEXT_NAME_MECHANIC", null);
                break;
            case ItemType.RaceCrew:
                //this.RaceCrewCharacter.Init(PopUpManager.Instance.graphics_agentPrefab, "TEXT_NAME_AGENT", null);
                if (PurchaseTypeToShow == PurchaseType.Renew)
                {
                    RaceCrewDescription.text = LocalizationManager.GetTranslation(GameDatabase.Instance.IAPs.WholeTeamRenewalDescription);
                    RaceCrewButtonTitle.text = GetLocalisedTime(GameDatabase.Instance.IAPs.WholeTeamConsumableRenewalLengthMinutes);
                }
                else
                {
                    RaceCrewDescription.text = LocalizationManager.GetTranslation(GameDatabase.Instance.IAPs.WholeTeamConsumableDescription);
                    RaceCrewButtonTitle.text = GetLocalisedTime(GameDatabase.Instance.IAPs.WholeTeamConsumableLengthMinutes);
                }
                break;
        }
    }

    private void UIBundleReadyDelegate(string assetID, AssetBundle assetBundle, IBundleOwner owner)
    {
        _isWaitingForUIAssetBundle = false;
        this._uiScreenPrefab = DataDrivenObject.CreateDataDrivenScreenPrefab(assetBundle);
        AssetProviderClient.Instance.ReleaseAsset(assetID, owner);
    }

    private void ShowOverlay(bool show, bool withloadingIcon)
    {
        ScreenManager.Instance.Interactable = !withloadingIcon;
        StopClicksButton.gameObject.SetActive(show);
        //_uiPoster.SetActive(show);
        status.gameObject.SetActive(show);
        LoadingIcon.gameObject.SetActive(withloadingIcon);
    }

    private void OnJustBoughtSuperNitro()
    {
        //HighStakesChallengeScreen.TryGotoRace = true;
    }

    private void OnJustBoughtRaceCrew()
    {
        if (ScreenManager.Instance.IsScreenOnStack(ScreenID.ConsumableOverview))
        {
            ScreenManager.Instance.PopScreen();
        }
        else
        {
            ScreenManager.Instance.SwapScreen(ScreenID.ConsumableOverview);
        }
    }

    private void OnPurchaseFinishedExit()
    {
        if (DirectPurchaseFailureCallback != null)
        {
            DirectPurchaseFailureCallback();
        }
        ScreenManager.Instance.PopScreen();
    }

    private void OnJustBoughtDirectPurchase()
    {
        if (DirectPurchaseCallback != null)
        {
            DirectPurchaseCallback();
        }
        ScreenManager.Instance.PopScreen();
    }

    private void OnJustBoughtOfferPack()
    {
        if (_currentState == State.ThereAreNoProducts)
        {
            ScreenManager.Instance.PopScreen();
        }
    }

    private void OnBuySuperNitrous()
    {
        if (_superNitroProduct == null)
        {
            DisplayErrorPopup();
            return;
        }
        MakePurchase(_superNitroProduct.AppStoreProduct);
    }

    private void OnBuyDirectPurchase()
    {
        if (_directPurchaseProduct == null)
        {
            if (DirectPurchaseFailureCallback != null)
            {
                DirectPurchaseFailureCallback();
            }
            DisplayErrorPopup();
            return;
        }
        MakePurchase(_directPurchaseProduct.AppStoreProduct);
    }

    private void OnBuyRaceCrew()
    {
        if (_raceCrewProduct == null)
        {
            DisplayErrorPopup();
            return;
        }
        MakePurchase(_raceCrewProduct.AppStoreProduct);
    }

    private void DisplayErrorPopup()
    {
        PopUp popup = new PopUp
        {
            Title = "TEXT_UPGRADES_ERROR",
            BodyText = "TEXT_SHOP_SCREEN_NO_PRODUCTS",
            IsBig = false,
            ConfirmAction = RequestBackup,
            ConfirmText = "TEXT_BUTTON_OK",
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
    }

    private bool DoPurchasePreamble()
    {
        if (_currentState != State.ChooseAProduct)
        {
            return false;
        }
        if (AppStore.Instance.IsProcessingTransaction)
        {
            _resetNextUpdateFlag = true;
            return false;
        }
        SetState(State.PurchasingFromAppStore);
        return true;
    }

    private void MakePurchase(AppStoreProduct product)
    {
        _activePurchaseTitle = product.Title;
        if (!DoPurchasePreamble())
        {
            return;
        }
        _activePurchaseTitle = product.Title;
        AppStore.Instance.Purchase(product.Identifier);

     

   







        //    AppStore.Instance.Purchase(product.Identifier);
    }

    private void RestorePurchases()
    {
        if (!DoPurchasePreamble())
        {
            return;
        }
        _activePurchaseTitle = "..";
        AppStore.Instance.RestorePurchases();
    }

    private void OnTransactionResult(PurchaseResult.eResult result)
    {
        GTDebug.Log(GTLogChannel.Screens,"Purchase result here : " + result);
        _resetNextUpdateFlag = true;
        PopUpButtonAction confirmAction = null;
        PopUpButtonAction confirmAction2 = null;
        switch (ItemTypeToShow)
        {
            case ItemType.SuperNitro:
                confirmAction = OnJustBoughtSuperNitro;
                break;
            case ItemType.RaceCrew:
                confirmAction = OnJustBoughtRaceCrew;
                break;
            case ItemType.Restore:
                confirmAction = OnPurchaseFinishedExit;
                confirmAction2 = OnPurchaseFinishedExit;
                break;
            case ItemType.DirectPurchase:
                confirmAction = OnJustBoughtDirectPurchase;
                confirmAction2 = OnPurchaseFinishedExit;
                break;
            case ItemType.OfferPack:
                confirmAction = OnJustBoughtOfferPack;
                break;
        }
        if (result == PurchaseResult.eResult.SUCCEEDED)
        {
            //Make ui interactable because it was disabled when trying to purchase item
            ScreenManager.Instance.Interactable = true;
            string bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_BOUGHT_FROM_STORE_BODY"),
                string.IsNullOrEmpty(_activePurchaseTitle) ? string.Empty : _activePurchaseTitle);
            PopUp popup = new PopUp
            {
                Title = "TEXT_POPUPS_BOUGHT_FROM_STORE_TITLE",
                BodyText = bodyText,
                BodyAlreadyTranslated = true,
                IsBig = true,
                ConfirmAction = confirmAction,
                ConfirmText = "TEXT_BUTTON_OK",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                ImageCaption = "TEXT_NAME_AGENT"
            };
            PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
        }
        else if (result == PurchaseResult.eResult.RESTORED)
        {
            string bodyText2 = LocalizationManager.GetTranslation("TEXT_POPUPS_RESTORED_FROM_STORE_BODY");
            PopUp popup2 = new PopUp
            {
                Title = "TEXT_POPUPS_RESTORED_FROM_STORE_TITLE",
                BodyText = bodyText2,
                BodyAlreadyTranslated = true,
                IsBig = true,
                ConfirmAction = confirmAction,
                ConfirmText = "TEXT_BUTTON_OK",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                ImageCaption = "TEXT_NAME_AGENT"
            };
            PopUpManager.Instance.TryShowPopUp(popup2, PopUpManager.ePriority.Default, null);
        }
        else if (result == PurchaseResult.eResult.DEFERRED)
        {
            Dictionary<Parameters, string> eventData = new Dictionary<Parameters, string>
            {
                {Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
                {Parameters.UIScreenID, ScreenManager.Instance.ActiveScreen.ID.ToString()},
                {Parameters.Market, BasePlatform.ActivePlatform.GetTargetAppStore().ToString()},
                {Parameters.TimeZone, BasePlatform.ActivePlatform.GetCity()}
            };
            Log.AnEvent(Events.DeferredPurchase, eventData);
            string bodyText3 = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_DEFERRED_FROM_STORE_BODY"), _activePurchaseTitle);
            PopUp popup3 = new PopUp
            {
                Title = "TEXT_POPUPS_DEFERRED_FROM_STORE_TITLE",
                BodyText = bodyText3,
                BodyAlreadyTranslated = true,
                IsBig = true,
                ConfirmAction = confirmAction2,
                ConfirmText = "TEXT_BUTTON_OK",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                ImageCaption = "TEXT_NAME_AGENT"
            };
            PopUpManager.Instance.TryShowPopUp(popup3, PopUpManager.ePriority.Default, null);
        }
        else
        {
            PopUp popup4 = new PopUp
            {
                Title = "TEXT_BUTTON_SUPPORT",
                BodyText = "TEXT_DETECT_IAP_FAILURE",
                IsBig = true,
                ConfirmAction = confirmAction2,
                ConfirmText = "TEXT_BUTTON_OK",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                ImageCaption = "TEXT_NAME_AGENT"
            };
            PopUpManager.Instance.TryShowPopUp(popup4, PopUpManager.ePriority.Default, null);
        }
    }

    private void RequestUIAssetBundle()
    {
        string itemTypeAssetID = GetItemTypeAssetID(ItemTypeToShow);
        AssetProviderClient.Instance.RequestAsset(itemTypeAssetID, UIBundleReadyDelegate, this);
    }

    private static string GetItemTypeAssetID(ItemType type)
    {
        if (type == ItemType.Cash)
        {
            return "UI_Shop_Cash";
        }
        if (type == ItemType.Gold)
        {
            return "UI_Shop_Gold";
        }
        if (type == ItemType.OfferPack)
        {
            return "UI_Shop_OfferPack";
        }
        return null;
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


    public void OnContactButton()
    {
        ScreenManager.Instance.PushScreen(ScreenID.Contact);
    }
}
