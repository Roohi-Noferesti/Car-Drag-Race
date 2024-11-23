using System;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarInfoUI : MonoBehaviour
{
    private const float STATBAR_WIDTH = 1.1f;

    //public GameObject StatBarPrefab;

    //public GameObject SkinnyStatBarPrefab;

    public Animator PositionAnimator;

    public Image Logo;

    public StatBar powerBar;

    public StatBar weightBar;

    public StatBar gripBar;

    public StatBar transmissionBar;

    private int cachedPower;

    private int cachedDeltaPower;

    private int cachedGrip;

    private int cachedDeltaGrip;

    private int cachedGearShiftTime;

    private int cachedDeltaGearShiftTime;

    private int cachedWeight;

    private int cachedDeltaWeight;

    //public WindowPaneBatched Background;

    public GameObject StatRoot;

    //public GameObject powerBarContainer;

    //public GameObject weightBarContainer;

    //public GameObject gripBarContainer;

    //public GameObject transmissionBarContainer;

    public Color barColour = Color.blue;

    public Color badColour = Color.red;

    public Color goodColour = Color.green;

    //public GameObject Offset;

    private PlayerProfile _cachedProfile;

    private bool carInfoUpdated;

    private string _currentCarID;
    public Sprite[] m_sprites;
    public TextMeshProUGUI m_carnameText;
    public TextMeshProUGUI m_carPPText;
    public TextMeshProUGUI m_carClassText;
    private bool m_visible;

    public bool isVisible
    {
        get { return m_visible; }
    }

    public string CurrentCarIDKey
    {
        get
        {
            return this._currentCarID;
        }
        private set
        {
            this._currentCarID = value;
        }
    }

    public static CarInfoUI Instance
    {
        get;
        private set;
    }

    public void RepositionFor(ScreenID id)
    {
        if (id == ScreenID.Tuning)
        {
            //if (this.Background != null)
            //{
            //    this.Background.Height = 1.74f;
            //    this.Background.UpdateSize();
            //}
            //this.Offset.transform.localPosition = new Vector3(0.78f, 0.2f, 0f);
            //this.powerBarContainer.transform.localPosition = new Vector3(-0.55f, 0.66f, 0f);
            //this.weightBarContainer.transform.localPosition = new Vector3(-0.55f, 0.23f, 0f);
            //this.gripBarContainer.transform.localPosition = new Vector3(-0.55f, -0.2f, 0f);
            //this.transmissionBarContainer.transform.localPosition = new Vector3(-0.55f, -0.61f, 0f);
            PositionAnimator.Play("Garage");
            Logo.gameObject.SetActive(false);
        }
        else
        {
            //if (this.Background != null)
            //{
            //    this.Background.Height = 1.6f;
            //    this.Background.UpdateSize();
            //}
            //this.Offset.transform.localPosition = new Vector3(0.78f, 0.38f, 0f);
            //this.powerBarContainer.transform.localPosition = new Vector3(-0.55f, 0.6f, 0f);
            //this.weightBarContainer.transform.localPosition = new Vector3(-0.55f, 0.21f, 0f);
            //this.gripBarContainer.transform.localPosition = new Vector3(-0.55f, -0.18f, 0f);
            //this.transmissionBarContainer.transform.localPosition = new Vector3(-0.55f, -0.57f, 0f);
            PositionAnimator.Play("Showroom");

            if (GameDatabase.Instance.CarsConfiguration.showCarsLogo) {
                Logo.gameObject.SetActive(true);
            } else {
                Logo.gameObject.SetActive(false);
            }
        }
        //if (this.Background != null)
        //{
        //    Vector3 localPosition = this.Background.transform.localPosition;
        //    localPosition.x = -this.Background.Width / 2f;
        //    localPosition.y = this.Background.Height / 2f;
        //    this.Background.transform.localPosition = localPosition;
        //}
    }

    public void SetCurrentCarIDKey(string val)
    {
        this.SetCurrentCarIDKey(val, true);
    }

    public void SetCurrentCarIDKey(string val, bool shouldHideCarStats)
    {
        this._currentCarID = val;
        this.carInfoUpdated = false;
        if (val == null)
        {
            this.ShowCarStats(false);
            return;
        }
        if (!shouldHideCarStats)
        {
            this.ShowCarStats(true);
        }
        this.ResetCarData();
    }

    public void ShowCarStats(bool zShow)
    {
        m_visible = zShow;
        //if (this.Background != null)
        //{
        //    this.Background.gameObject.SetActive(zShow);
        //}        
        if (this.StatRoot != null)
        {
            this.StatRoot.SetActive(zShow);
        }

        //take a look at line below , you may say layter what is this line of code mean . Make set logo to zShow always  
        //But the problem here is that logo visibilty is controlled by Reposition for UI , if you changed this line , it always controlled by this line of code
        if (!zShow)
            Logo.gameObject.SetActive(false);


        this.powerBar.Show(zShow);
        this.weightBar.Show(zShow);
        this.gripBar.Show(zShow);
        this.transmissionBar.Show(zShow);
    }

    private void Awake()
    {
        if (CarInfoUI.Instance == null)
        {
            CarInfoUI.Instance = this;
        }
        //GameObject original;
        //if (this.Background == null)
        //{
        //    original = this.SkinnyStatBarPrefab;
        //}
        //else
        //{
        //    original = this.StatBarPrefab;
        //}
        //this.powerBar = (UnityEngine.Object.Instantiate(original) as GameObject).GetComponent<StatBar>();
        //this.weightBar = (UnityEngine.Object.Instantiate(original) as GameObject).GetComponent<StatBar>();
        //this.gripBar = (UnityEngine.Object.Instantiate(original) as GameObject).GetComponent<StatBar>();
        //this.transmissionBar = (UnityEngine.Object.Instantiate(original) as GameObject).GetComponent<StatBar>();
        //this.powerBar.gameObject.transform.parent = this.powerBarContainer.transform;
        //this.weightBar.gameObject.transform.parent = this.weightBarContainer.transform;
        //this.gripBar.gameObject.transform.parent = this.gripBarContainer.transform;
        //this.transmissionBar.gameObject.transform.parent = this.transmissionBarContainer.transform;
        //this.powerBar.gameObject.transform.localPosition = Vector3.zero;
        //this.weightBar.gameObject.transform.localPosition = Vector3.zero;
        //this.gripBar.gameObject.transform.localPosition = Vector3.zero;
        //this.transmissionBar.gameObject.transform.localPosition = Vector3.zero;
        this.ShowCarStats(false);
        //if (this.Background == null)
        //{
        //    this.powerBar.showArrows = false;
        //    this.weightBar.showArrows = false;
        //    this.gripBar.showArrows = false;
        //    this.transmissionBar.showArrows = false;
        //}
    }

    private void SetHeadings()
    {
        this.powerBar.SetTitle(LocalizationManager.GetTranslation("TEXT_CAR_STATS_POWER:").ToUpper());
        this.weightBar.SetTitle(LocalizationManager.GetTranslation("TEXT_CAR_STATS_WEIGHT:").ToUpper());
        this.gripBar.SetTitle(LocalizationManager.GetTranslation("TEXT_CAR_STATS_GRIP:").ToUpper());
        this.transmissionBar.SetTitle(LocalizationManager.GetTranslation("TEXT_CAR_STATS_GEARS:").ToUpper());
    }

    private void OnDestroy()
    {
        if (ScreenManager.Instance.CurrentScreen != ScreenID.SportsPack)
        {
            CarInfoUI.Instance = null;
        }
        this._currentCarID = null;
    }

    private void Update()
    {
        if (!CarDatabase.Instance.isReady)
        {
            return;
        }
        if (PlayerProfileManager.Instance.ActiveProfile != null && PlayerProfileManager.Instance.ActiveProfile != this._cachedProfile)
        {
            this._cachedProfile = PlayerProfileManager.Instance.ActiveProfile;
            this._currentCarID = PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey;
            this.carInfoUpdated = false;
        }
        if (!this.carInfoUpdated)
        {
            this.ResetCarData();
        }
    }

    public void ResetDeltaStatCache()
    {
        this.cachedPower = (this.cachedWeight = (this.cachedDeltaGrip = (this.cachedDeltaGearShiftTime = -1)));
    }

    public void ResetCarDataWithUpgrades(UpgradeScreenCarStats stats)
    {
        int currentHP = stats.CurrentHP;
        int currentWeight = stats.CurrentWeight;
        int currentGrip = stats.CurrentGrip;
        int currentGearShiftTime = stats.CurrentGearShiftTime;
        int deltaHP = stats.DeltaHP;
        int deltaWeight = stats.DeltaWeight;
        int deltaGrip = stats.DeltaGrip;
        int deltaGearShiftTime = stats.DeltaGearShiftTime;
        FrontendStatBarData frontendCarStatBarData = GameDatabase.Instance.CarsConfiguration.FrontendCarStatBarData;
        frontendCarStatBarData.MaxCarWeight = 6500;
        if (currentHP != this.cachedPower || deltaHP != this.cachedDeltaPower)
        {
            this.cachedPower = currentHP;
            this.cachedDeltaPower = deltaHP;
            this.powerBar.Calibrate(1.1f, (float)currentHP, (float)frontendCarStatBarData.MaxHorsePower, (float)deltaHP, true);
        }
        if (currentWeight != this.cachedWeight || deltaWeight != this.cachedDeltaWeight)
        {
            this.cachedWeight = currentWeight;
            this.cachedDeltaWeight = deltaWeight;
            this.weightBar.Calibrate(1.1f, (float)currentWeight, (float)frontendCarStatBarData.MaxCarWeight, (float)deltaWeight, false);
        }
        if (currentGrip != this.cachedGrip || deltaGrip != this.cachedDeltaGrip)
        {
            this.cachedGrip = currentGrip;
            this.cachedDeltaGrip = deltaGrip;
            this.gripBar.Calibrate(1.1f, (float)currentGrip, (float)frontendCarStatBarData.MaxTyreGrip, (float)deltaGrip, true);
        }
        if (currentGearShiftTime != this.cachedGearShiftTime || deltaGearShiftTime != this.cachedDeltaGearShiftTime)
        {
            this.cachedGearShiftTime = currentGearShiftTime;
            this.cachedDeltaGearShiftTime = deltaGearShiftTime;
            this.transmissionBar.Calibrate(1.1f, (float)currentGearShiftTime, (float)frontendCarStatBarData.MaxGearShiftTime, (float)deltaGearShiftTime, false);
        }
    }

    public void ResetCarData()
    {
        if (!CarDatabase.Instance.isReady || this._currentCarID == null)
        {
            return;
        }
        CarInfo car = CarDatabase.Instance.GetCar(this._currentCarID);
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (activeProfile == null)
        {
            return;
        }
        bool flag = activeProfile.CarsOwned.Find((CarGarageInstance x) => x.CarDBKey == this._currentCarID) != null;
        if (flag && ScreenManager.Instance.CurrentScreen != ScreenID.Showroom &&
            ScreenManager.Instance.CurrentScreen != ScreenID.ShowroomFreeCam &&
            ScreenManager.Instance.CurrentScreen != ScreenID.SportsPack)
        {
            this.UpdateScreenStats();
            CarInfo car2 = CarDatabase.Instance.GetCar(activeProfile.GetCurrentCar().CarDBKey);

            CommonUI.Instance.mainNameStats.SetShortStatBarText(car2.ShortName,
                activeProfile.GetCurrentCar().CurrentTier, activeProfile.GetCurrentCar().CurrentPPIndex);
            CommonUI.Instance.zoomedNameStats.SetShortStatBarText(car2.ShortName,
                activeProfile.GetCurrentCar().CurrentTier, activeProfile.GetCurrentCar().CurrentPPIndex);
        }
        else if (ScreenManager.Instance.CurrentScreen == ScreenID.SportsPack)
        {
            this.SetHeadings();
        }
        else
        {
            CarStatsCalculator.Instance.CalculateStatsForStockCar(this._currentCarID);
            CarStatsCalculator.Instance.SetOutStats(eCarStatsType.STOCK_CAR);
            this.UpdateScreenStats();

            var baseCarTier = CarStatsCalculator.Instance.stockCarPhysicsSetup.BaseCarTier;
            CommonUI.Instance.mainNameStats.SetShortStatBarText(car.ShortName, baseCarTier,
                CarStatsCalculator.Instance.stockCarPhysicsSetup.NewPerformanceIndex);
            CommonUI.Instance.zoomedNameStats.SetShortStatBarText(car.ShortName, baseCarTier,
                CarStatsCalculator.Instance.stockCarPhysicsSetup.NewPerformanceIndex);
            m_carnameText.text = LocalizationManager.GetTranslation(car.ShortName);
            m_carPPText.text = CarStatsCalculator.Instance.stockCarPhysicsSetup.NewPerformanceIndex.ToNativeNumber();
            m_carClassText.text = LocalizationManager.GetTranslation(CarInfo.ConvertCarTierEnumToString(baseCarTier));

            //var baseReward = GameDatabase.Instance.Currencies.GetcashRewardByPPIndexAndDifficulty(car.PPIndex,
            //    AutoDifficulty.DifficultyRating.Easy, car.BaseCarTier, car.BaseCarTier);
            //m_baseRewardText.text = String.Format("{0:n0}", baseReward).ToNativeNumber();
            Logo.sprite = m_sprites.FirstOrDefault(s => "id_" + s.name == car.ManufacturerID.ToString().ToLower());
            Logo.SetNativeSize();
            
        }
    }

    private void UpdateScreenStats()
    {
        if (!GameDatabase.Instance.IsReady())
        {
            return;
        }
        int horsePower = CarStatsCalculator.Instance.HorsePower;
        int weight = CarStatsCalculator.Instance.Weight;
        int tyreGrip = CarStatsCalculator.Instance.TyreGrip;
        int gearShiftTime = CarStatsCalculator.Instance.GearShiftTime;
        FrontendStatBarData frontendCarStatBarData = GameDatabase.Instance.CarsConfiguration.FrontendCarStatBarData;
        frontendCarStatBarData.MaxCarWeight = 6500;
        this.SetHeadings();
        this.powerBar.Calibrate(1.1f, (float)horsePower, (float)frontendCarStatBarData.MaxHorsePower);
        this.weightBar.Calibrate(1.1f, (float)weight, (float)frontendCarStatBarData.MaxCarWeight);
        this.gripBar.Calibrate(1.1f, (float)tyreGrip, (float)frontendCarStatBarData.MaxTyreGrip);
        this.transmissionBar.Calibrate(1.1f, (float)gearShiftTime, (float)frontendCarStatBarData.MaxGearShiftTime);
        this.carInfoUpdated = true;
    }
}
