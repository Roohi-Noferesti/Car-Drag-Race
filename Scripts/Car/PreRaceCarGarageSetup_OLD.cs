using System;
using System.Collections.Generic;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class PreRaceCarGarageSetup_OLD : MonoSingleton<PreRaceCarGarageSetup_OLD>
{
    public CarInfo HumanInfo { get; private set; }

    private GameObject _humanCar;

    public CarVisuals HumanCarVisuals { get; private set; }

    public Texture2D HumanSnapShot { get; private set; }

    public bool IsHumanCarFullySetup { get; private set; }

    public CarInfo AiInfo { get; private set; }

    private GameObject _aiCar;

    public CarVisuals AiCarVisuals { get; private set; }

    public Texture2D AiSnapShot { get; private set; }

    public bool IsAiCarFullySetup { get; private set; }

    private bool _ClearAISnapshot;
    private string _humanBodyShader;
    private string _humanRingShader;
    private string _humanHeadlightShader;
    private string _humanSticker;
    private string _humanSpoiler;
    private float _humanBodyHeight;
    private string _aiBodyShader;
    private string _aiHeadlightShader;
    private string _aiRingShader;
    private string _aiSticker;
    private string _aiSpoiler;
    private float _aiBodyHeight;

    public void SetupHumanCar(string numberPlate)
    {
        this.StartHumanCarLoading();
        LoadHumanCarVisual(null, numberPlate);
        HumanTakeSnapshot();
        this.IsHumanCarFullySetup = true;
    }

    public void SetupAiCar(string numberPlate)
    {
        StartAICarLoading();
        LoadAiCarVisual(null, numberPlate);
        AiTakeSnapshot();
        this.IsAiCarFullySetup = true;
    }

    private void StartHumanCarLoading()
    {
        this.HumanInfo = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.LocalPlayerCarDBKey);
        if (HumanInfo == null)
        {
            this.HumanInfo = CarDatabase.Instance.GetDefaultCar();
        }
        var carPrefab = ResourceManager.GetCarAsset<GameObject>(HumanInfo.Key, ServerItemBase.AssetType.garage_model);
        _humanCar = Instantiate(carPrefab);
        HumanCarVisuals = this._humanCar.GetComponent<CarVisuals>();
    }

    private void LoadHumanCarVisual(string liveryName,string numberPlate)
    {
        this._humanCar.name = "HUMAN CAR " + liveryName;
        this._humanCar.SetActive(true);
        this.HumanCarVisuals = this._humanCar.GetComponent<CarVisuals>();

        CarGarageInstance garageCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        if (garageCar == null || garageCar.CarDBKey != HumanInfo.Key)
        {
            garageCar = GetHumanCarGarageInstance();
        }
        SetupHumanCarShaders();
        garageCar.AppliedLiveryName = this.HumanCarVisuals.CurrentlyAppliedLiveryObjName;
        garageCar.EliteCar = RaceEventInfo.Instance.LocalPlayerCarElite;
        RaceEventInfo.Instance.HumanCarGarageInstance = garageCar;
        this.BuildCarVisuals(this._humanCar, garageCar,_humanBodyShader,_humanRingShader,
            _humanHeadlightShader, _humanSticker, _humanSpoiler, numberPlate, _humanBodyHeight);
    }


    private void SetupHumanCarShaders()
    {
        if (RaceEventInfo.Instance.CurrentEvent == null)
        {
            return;
        }
        if (RaceEventInfo.Instance.CurrentEvent.IsTutorial())
        {
            _humanBodyShader = GameDatabase.Instance.TutorialConfiguration.TutorialCarBodyShader;
            _humanRingShader = GameDatabase.Instance.TutorialConfiguration.TutorialCarRingShader;
            _humanHeadlightShader = GameDatabase.Instance.TutorialConfiguration.TutorialCarHeadlightShader;
            _humanSticker = GameDatabase.Instance.TutorialConfiguration.TutorialCarSticker;
            _humanSpoiler = GameDatabase.Instance.TutorialConfiguration.TutorialCarSpoiler;
            return;
        }
        if (RaceEventInfo.Instance.CurrentEvent.IsDailyBattle() && PlayerProfileManager.Instance.ActiveProfile.DailyBattlesLastEventAt == DateTime.MinValue)
        {
            _humanBodyShader = GameDatabase.Instance.TutorialConfiguration.FirstDailyBattleCarBodyShader;
            _humanRingShader = GameDatabase.Instance.TutorialConfiguration.FirstDailyBattleCarRingShader;
            _humanHeadlightShader = GameDatabase.Instance.TutorialConfiguration.FirstDailyBattleCarHeadlightShader;
            _humanSticker = GameDatabase.Instance.TutorialConfiguration.FirstDailyBattleCarSticker;
            _humanSpoiler = GameDatabase.Instance.TutorialConfiguration.FirstDailyBattleCarSpoiler;
            return;
        }
        if (this.HumanInfo.IsBossCarOverride)
        {
            _humanBodyShader = HumanInfo.BodyShaderOverride;
            _humanRingShader = HumanInfo.RingShaderOverride;
            _humanHeadlightShader = HumanInfo.HeadlightShaderOverride;
            _humanSticker = HumanInfo.StickerOverride;
            _humanSpoiler = HumanInfo.SpoilerOverride;
            //this._humanCarVisuals.ForceColor(forcedColor);
            return;
        }
        if (RaceEventInfo.Instance.IsFirstOfSwitchBackRace())
        {
            CarInfo car = CarDatabase.Instance.GetCar(this.HumanInfo.Key + "Boss");
            if (car != null)
            {
                //UnityEngine.Color forcedColor2 = new UnityEngine.Color(car.ColourOverrideR, car.ColourOverrideG, car.ColourOverrideB);
                //this._humanCarVisuals.ForceColor(forcedColor2);
                return;
            }
        }
        _humanBodyShader = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedBodyPaintItemID;
        _humanRingShader = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedRingColorItemID;
        _humanHeadlightShader = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedFrontlightColorItemID;
        _humanSticker = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedStickerItemID;
        _humanSpoiler = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedSpoilerItemID;
        _humanBodyHeight = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedBodyHeight;
    }

    private void BuildCarVisuals(GameObject car, CarGarageInstance carGarageInstance,
        string body, string ring, string headlight, string sticker, string spoiler, string numberplateText,float bodyHeight)
    {
        CarVisuals component = car.GetComponent<CarVisuals>();
        component.SetProperty(carGarageInstance.ID, ServerItemBase.AssetType.body_shader, body);

        component.SetProperty(carGarageInstance.ID, ServerItemBase.AssetType.ring_shader, ring);

        component.SetProperty(carGarageInstance.ID, ServerItemBase.AssetType.headlight_shader, headlight);

        component.SetProperty(carGarageInstance.ID, ServerItemBase.AssetType.sticker, sticker);

        component.SetProperty(carGarageInstance.ID, ServerItemBase.AssetType.spoiler, spoiler);

        component.GlassMaterial = ResourceManager.GetSharedAsset<Material>("Car_Glass_Garage_Shared",
            ServerItemBase.AssetType.car_shared_shader);
        component.ApplyNumberPlateText(numberplateText);

        component.BodyHeight = bodyHeight;
    }

    private void HumanTakeSnapshot()
    {
        if (!ScreenShot.Instance.SnapshotCacheExists(HumanCarVisuals))
        {
            ScreenShot.Instance.PlaceCarAndRender(HumanCarVisuals, HumanInfo, true,true);
        }

        HumanSnapShot = ScreenShot.Instance[HumanCarVisuals];
    }

    private CarGarageInstance GetHumanCarGarageInstance()
    {
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        var localPlayerCarKey = RaceEventInfo.Instance.LocalPlayerCarDBKey;
        var carinfo = CarDatabase.Instance.GetCar(localPlayerCarKey);
        CarGarageInstance garageCar = new CarGarageInstance();
        garageCar.SetupNewGarageInstance(carinfo);
        CarUpgradeSetup carUpgradeSetup = RaceEventInfo.Instance.LocalPlayerCarUpgradeSetup;
        if (carUpgradeSetup == null)
        {
            carUpgradeSetup = CarUpgradeSetup.NullCarSetup;
        }
        carUpgradeSetup.IsFettled = (RaceEventInfo.Instance.ShouldCurrentEventUseMechanic() && activeProfile.GetMechanicServicesAvailable());
        carUpgradeSetup.EngineConsumableActive = (RaceEventInfo.Instance.ShouldCurrentEventUseConsumables() && activeProfile.IsConsumableActive(eCarConsumables.EngineTune));
        carUpgradeSetup.NitrousConsumableActive = (RaceEventInfo.Instance.ShouldCurrentEventUseConsumables() && activeProfile.IsConsumableActive(eCarConsumables.Nitrous));
        carUpgradeSetup.TyreConsumableActive = (RaceEventInfo.Instance.ShouldCurrentEventUseConsumables() && activeProfile.IsConsumableActive(eCarConsumables.Tyre));
        int currentPPIndex = activeProfile.GetCurrentCar().CurrentPPIndex;
        carUpgradeSetup.CalculateFettle(carinfo, currentPPIndex);
        carUpgradeSetup.SetupConsumableParams(carinfo, currentPPIndex);
        bool flag = RaceEventInfo.Instance.CurrentEvent.IsHighStakesEvent();
        if (flag && BoostNitrous.HaveBoostNitrous())
        {
            CarUpgradeStatus carUpgradeStatus = carUpgradeSetup.UpgradeStatus[eUpgradeType.NITROUS];
            if (carUpgradeStatus.levelFitted == 0)
            {
                carUpgradeStatus.levelFitted = 1;
            }
        }

        return garageCar;
    }

    public void StartAICarLoading()
    {
        bool flag = CarDatabase.Instance.PeekGetCar(RaceEventInfo.Instance.OpponentCarDBKey);
        if (flag)
        {
            this.AiInfo = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.OpponentCarDBKey);
        }
        else
        {
            eCarTier baseCarTier = this.HumanInfo.BaseCarTier;
            List<CarInfo> carsOfTier = CarDatabase.Instance.GetCarsOfTier(baseCarTier);
            if (carsOfTier.Count > 0)
            {
                this.AiInfo = carsOfTier[0];
            }
            else
            {
                this.AiInfo = CarDatabase.Instance.GetDefaultCar();
            }
        }

        var carPrefab = ResourceManager.GetCarAsset<GameObject>(AiInfo.Key, ServerItemBase.AssetType.garage_model);
        _aiCar = Instantiate(carPrefab);
        AiCarVisuals = this._aiCar.GetComponent<CarVisuals>();
    }

    private void LoadAiCarVisual(string liveryName,string numberPlateText)
    {
        this._aiCar.name = "AI CAR " + liveryName;
        SetupAICarShaders();
        this.BuildAICarVisuals(numberPlateText);
    }


    private void SetupAICarShaders()
    {
        if (RaceEventInfo.Instance.IsCrewRaceEvent || RaceEventInfo.Instance.IsHighStakesEvent ||
            RaceEventInfo.Instance.CurrentEvent.UseCustomShader)
        {
            _aiBodyShader = RaceEventInfo.Instance.CurrentEvent.BodyShader;
            _aiHeadlightShader = RaceEventInfo.Instance.CurrentEvent.HeadLightShader;
            _aiRingShader = RaceEventInfo.Instance.CurrentEvent.RingShader;
            _aiSticker = RaceEventInfo.Instance.CurrentEvent.Sticker;
            _aiSpoiler = RaceEventInfo.Instance.CurrentEvent.Spoiler;
            this._ClearAISnapshot = true;
            return;
        }
        if (this.AiInfo.IsBossCarOverride)
        {
            _aiBodyShader = AiInfo.BodyShaderOverride;
            _aiHeadlightShader = AiInfo.HeadlightShaderOverride;
            _aiRingShader = AiInfo.RingShaderOverride;
            _aiSticker = AiInfo.StickerOverride;
            _aiSpoiler = AiInfo.SpoilerOverride;
            return;
            //this._aiCarVisuals.ForceColor(forcedColor);
        }
        if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent() ||
            RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent() ||
            RaceEventInfo.Instance.CurrentEvent.IsRandomRelay())
        {
            //RacePlayerInfoComponent component =
            //    CompetitorManager.Instance.OtherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
            //return component.AppliedColourIndex;
            return;
        }

        CarCustomiseUtility.SetupRandomFurthestShaders(AiCarVisuals,out _aiBodyShader, out _aiHeadlightShader,
            out _aiRingShader, out _aiSticker, out _aiSpoiler,out _aiBodyHeight);
    }

    private void BuildAICarVisuals(string numberPlate)
    {
        this.AiCarVisuals = this._aiCar.GetComponent<CarVisuals>();
        this._ClearAISnapshot = false;
        RaceEventInfo.Instance.AICarGarageInstance = GetAiCarGarageInstance();

        var bodyItem = CarCustomiseUtility.GetEquipItemByID(VirtualItemType.BodyShader,
            _aiBodyShader,AiInfo.Key);
        var ringItem = CarCustomiseUtility.GetEquipItemByID(VirtualItemType.RingShader,
            _aiRingShader, AiInfo.Key);
        var headlightItem = CarCustomiseUtility.GetEquipItemByID(VirtualItemType.HeadLighShader,
            _aiHeadlightShader, AiInfo.Key);
        var stickerItem = CarCustomiseUtility.GetEquipItemByID(VirtualItemType.CarSticker,
            _aiSticker, AiInfo.Key);
        var spoilerItem = CarCustomiseUtility.GetEquipItemByID(VirtualItemType.CarSpoiler,
            _aiSpoiler, AiInfo.Key);

        var aiCar = RaceEventInfo.Instance.AICarGarageInstance;
        aiCar.EquipItemCollection.Add(bodyItem);
        aiCar.EquipItemCollection.Add(ringItem);
        aiCar.EquipItemCollection.Add(headlightItem);
        if (stickerItem != null)
            aiCar.EquipItemCollection.Add(stickerItem);
        if (spoilerItem != null)
            aiCar.EquipItemCollection.Add(spoilerItem);

        BuildCarVisuals(_aiCar, RaceEventInfo.Instance.AICarGarageInstance, _aiBodyShader,
            _aiRingShader, _aiHeadlightShader, _aiSticker, _aiSpoiler, numberPlate,_aiBodyHeight);
    }

    private void AiTakeSnapshot()
    {
        if (this._ClearAISnapshot)
        {
            ScreenShot.Instance.ClearSnapshotFromCache(AiCarVisuals);
        }
        if (!ScreenShot.Instance.SnapshotCacheExists(AiCarVisuals))
        {
            ScreenShot.Instance.PlaceCarAndRender(AiCarVisuals, AiInfo, true, false);
        }

        AiSnapShot = ScreenShot.Instance[AiCarVisuals];
    }

    private CarGarageInstance GetAiCarGarageInstance()
    {
        CarGarageInstance carGarageInstance = new CarGarageInstance();
        var carinfo = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.OpponentCarDBKey);
        carGarageInstance.SetupNewGarageInstance(carinfo);
        carGarageInstance.EliteCar = RaceEventInfo.Instance.OpponentCarElite;
        carGarageInstance.AppliedLiveryName = this.AiCarVisuals.CurrentlyAppliedLiveryObjName;
        CarUpgradeSetup carUpgradeSetup = RaceEventInfo.Instance.OpponentCarUpgradeSetup;
        if (carUpgradeSetup == null)
        {
            carUpgradeSetup = CarUpgradeSetup.NullCarSetup;
        }
        int pPIndex = RaceEventInfo.Instance.CurrentEvent.GetAIPerformancePotentialIndex();
        carUpgradeSetup.CalculateFettle(carinfo, pPIndex);
        carUpgradeSetup.SetupConsumableParams(carinfo, pPIndex);
        return carGarageInstance;
    }

    public void ClearCachedGameObjects()
    {
        Destroy(_aiCar);
        Destroy(_humanCar);
        ScreenShot.Instance.ClearSnapshotFromCache(AiCarVisuals);
        ScreenShot.Instance.ClearSnapshotFromCache(HumanCarVisuals);
        HumanCarVisuals = null;
        AiCarVisuals = null;
    }
}
