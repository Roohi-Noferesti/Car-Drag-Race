using I2.Loc;
using KingKodeStudio;
using Metrics;
using TMPro;

public class FuelUpgradeShopScreen : ZHUDScreen
{
    public enum ProductType
    {
        UpgradeTank,
        UnlimitedFuel
    }

    public TextMeshProUGUI TitleText;

    public TextMeshProUGUI BodyText;

    public TextMeshProUGUI PriceText;

    public TextMeshProUGUI BuyButtonText;

    //public global::Sprite BuyButtonIcon;

    //public GameObject UnlimitedFuelBar;

    //public SpriteText UnlimitedFuelBarText;

    //public GameObject UpgradedTankBar;

    //public SpriteText UpgradedTankBarText;

    //public DataDrivenPortrait Mechanic;

    //public string UnlimitedFuelPortraitPath;

    //public string UpgradedTankPortraitPath;

    private static ProductType ProductToShow;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.FuelUpgradeShop;
        }
    }

    public static void PushWithProduct(ProductType product)
    {
        ProductToShow = product;
        ScreenManager.Instance.PushScreen(ScreenID.FuelUpgradeShop);
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        ProductType productToShow = ProductToShow;
        if (productToShow != ProductType.UpgradeTank)
        {
            if (productToShow == ProductType.UnlimitedFuel)
            {
                TitleText.text = LocalizationManager.GetTranslation("TEXT_UNLIMITED_FUEL_SHOP_TITLE");
                BodyText.text = LocalizationManager.GetTranslation("TEXT_UNLIMITED_FUEL_SHOP_BODY");
                //this.BuyButtonText.text = LocalizationManager.GetTranslation("TEXT_UNLIMITED_FUEL_SHOP_BUTTON");
                //this.UnlimitedFuelBar.SetActive(true);
                //this.UpgradedTankBar.SetActive(false);
                //this.UnlimitedFuelBarText.Text = string.Format(LocalizationManager.GetTranslation("TEXT_TEXT_UNITS_TIME_HOURS_SHORT"), GameDatabase.Instance.IAPs.UnlimitedFuelMinutes / 60);
                //this.Mechanic.Init(this.UnlimitedFuelPortraitPath, string.Empty, null);
            }
        }
        else
        {
            TitleText.text = LocalizationManager.GetTranslation("TEXT_GAS_TANK_SHOP_TITLE");
            BodyText.text = LocalizationManager.GetTranslation("TEXT_GAS_TANK_SHOP_BODY");
            //this.BuyButtonText.text = GameDatabase.Instance.IAPs.UpgradedGasTankSize().ToString();
            //this.UnlimitedFuelBar.SetActive(false);
            //this.UpgradedTankBar.SetActive(true);
            //this.UpgradedTankBarText.Text = GameDatabase.Instance.IAPs.UpgradedGasTankSize().ToString();
            //this.Mechanic.Init(this.UpgradedTankPortraitPath, LocalizationManager.GetTranslation("TEXT_NAME_MECHANIC").ToUpper(), null);
        }
        PriceText.text = ShopScreen.RequestPriceStringForIAP(GetProductCode());
        //Vector3 localPosition = this.BuyButtonIcon.transform.localPosition;
        //localPosition.x = -(this.BuyButtonText.GetWidth(this.BuyButtonText.Text) * 0.5f);
        //this.BuyButtonIcon.transform.localPosition = localPosition;
    }

    public void OnButtonTap()
    {
        Log.AnEvent(Events.OnBuyUpgradeFinesCapacityButtonClicked);
        
        if (!PolledNetworkState.IsNetworkConnected || !ServerSynchronisedTime.Instance.ServerTimeValid)
        {
            PopUpDatabase.Common.ShowNoInternetConnectionPopup(null);
            return;
        }

        string productCode = GetProductCode();
        if (!string.IsNullOrEmpty(productCode))
        {
            ShopScreen.InitialiseForDirectPurchase(productCode, delegate
            {
                ScreenManager.Instance.PopScreen();
            }, null);
            ScreenManager.Instance.PushScreen(ScreenID.Shop);
        }
        
    }

    private string GetProductCode()
    {
        IAPDatabase iAPs = GameDatabase.Instance.IAPs;
        ProductType productToShow = ProductToShow;
        if (productToShow == ProductType.UpgradeTank)
        {
            return iAPs.GetGasTankProductCode();
        }
        if (productToShow != ProductType.UnlimitedFuel)
        {
            return null;
        }
        return iAPs.UnlimitedFuelProductCode;
    }
}
