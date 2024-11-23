using System;
using System.Collections.Generic;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;
using Object = UnityEngine.Object;

public class PreRaceCarGarageSetup : MonoSingleton<PreRaceCarGarageSetup>
{
    private enum SetupStates
    {
        Idle,
        WaitingForDatabase,
        WaitingForHumanCar,
        WaitingForAiCar
    }

    private GameObject _humanCar;

    public bool IsHumanCarFullySetup { get; private set; }

    private GameObject _aiCar;

    public bool IsAiCarFullySetup { get; private set; }

    private bool _ClearAISnapshot;

    private SetupStates _state = SetupStates.Idle;

    private GameObject _humanLivery;

    //private GameObject _aiLivery;

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
    private CarReflectionMapManager.ReflectionTexType reflType = CarReflectionMapManager.ReflectionTexType.WorkshopNight;

    public CarInfo HumanInfo { get; private set; }
    public CarVisuals HumanCarVisuals { get; private set; }
    public Texture2D HumanSnapShot { get; private set; }
    public CarInfo AiInfo { get; private set; }
    public CarVisuals AiCarVisuals { get; private set; }
    public Texture2D AiSnapShot { get; private set; }
    public Texture2D AiSnapShotReverse { get; private set; }


    public void Setup(string displayName, string getRivalName)
    {
        _state = SetupStates.WaitingForDatabase;
        IsHumanCarFullySetup = false;
        IsAiCarFullySetup = false;
        enabled = true;
    }

    private void FixedUpdate()
    {
        switch (this._state)
        {
            case SetupStates.WaitingForDatabase:
                if (CarDatabase.Instance.isReady)
                {
                    this.StartHumanCarLoading();
                    this._state = SetupStates.WaitingForHumanCar;
                }
                break;
            case SetupStates.WaitingForHumanCar:
                if (this.IsHumanCarFullySetup)
                {
                    if (CompetitorManager.Instance.OtherCompetitor != null)
                    {
                        this.StartAICarLoading();
                        this._state = SetupStates.WaitingForAiCar;
                    }
                    else
                    {
                        enabled = false;
                    }
                }
                break;
            case SetupStates.WaitingForAiCar:
                if (this.IsAiCarFullySetup)
                {
                    CarReflectionMapManager.UnloadCubemap(CarReflectionMapManager.ReflectionTexType.Showroom);
                    enabled = false;
                }
                break;
        }
    }

    private void StartHumanCarLoading()
    {
        this.HumanInfo = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.LocalPlayerCarDBKey);
        if (this.HumanInfo == null)
        {
            this.HumanInfo = CarDatabase.Instance.GetDefaultCar();
        }
        AsyncBundleSlotDescription desc = AsyncBundleSlotDescription.HumanCar;
        string name = this.HumanInfo.ModelPrefabString;
        RacePlayerInfoComponent component =
            CompetitorManager.Instance.LocalCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
        bool flag = component.IsEliteLiveryApplied || component.IsEliteSportsLiveryApplied;
        if (!string.IsNullOrEmpty(this.HumanInfo.EliteOverrideCarAssetID) && flag)
        {
            name = this.HumanInfo.EliteOverrideCarAssetID;
        }
        AsyncSwitching.Instance.RequestAsset(desc, name, this.HumanCarLoaded, gameObject, true,
            null);
    }

    private void HumanCarLoaded(bool carLoadedOk, string carName)
    {
        this.HumanLoadLivery();
    }

    private string SelectRandomLiveryForCar(string carDBKey)
    {
        return null;
        //List<AssetDatabaseAsset> list = AssetDatabaseClient.Instance.Data.GetAssetsOfType(CSRAssetTypes.livery);
        //list = list.FindAll((AssetDatabaseAsset x) => x.code.Contains(carDBKey) && !x.code.Contains("Tier") && !x.code.Contains("PrizeOMaticOnly") && !x.code.Contains("Elite"));
        //int count = list.Count;
        //if (count == 0)
        //{
        //    return string.Empty;
        //}
        //int index = UnityEngine.Random.Range(0, count);
        //return list[index].code;
    }

    private void HumanLoadLivery()
    {
        string text = null;
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (activeProfile != null)
        {
            text = activeProfile.GetCurrentCar().AppliedLiveryName;
        }
        if (!AsyncSwitching.IsLiveryName(text) && !string.IsNullOrEmpty(this.HumanInfo.DefaultLiveryBundleName))
        {
            text = this.HumanInfo.DefaultLiveryBundleName;
        }
        if (RaceEventInfo.Instance.CurrentEvent.IsTutorial())
        {
            text = GameDatabase.Instance.TutorialConfiguration.TutorialCarLivery;
        }
        else if (RaceEventInfo.Instance.CurrentEvent.IsDailyBattle() &&
                 PlayerProfileManager.Instance.ActiveProfile.DailyBattlesLastEventAt == DateTime.MinValue)
        {
            text = GameDatabase.Instance.TutorialConfiguration.FirstDailyBattleCarLivery;
        }
        else if (RaceEventInfo.Instance.CurrentEvent.IsLocalCarLoaned())
        {
            text = this.SelectRandomLiveryForCar(RaceEventInfo.Instance.LocalPlayerCarDBKey);
        }
        else if (RaceEventInfo.Instance.IsFirstOfSwitchBackRace())
        {
            text = RaceEventInfo.Instance.CurrentEvent.LoanCarGarageInstance.AppliedLiveryName;
        }
        if (AsyncSwitching.IsLiveryName(text))
        {
            AsyncSwitching.Instance.RequestAsset(AsyncBundleSlotDescription.HumanCarLivery, text, this.HumanLiveryLoaded,
                gameObject, true, null);
        }
        else
        {
            this.HumanLiveryLoaded(true, text);
        }
    }

    private void HumanLiveryLoaded(bool liveryLoadedOk, string liveryName)
    {
        this._humanCar = AsyncSwitching.Instance.GetCar(AsyncBundleSlotDescription.HumanCar);
        this._humanCar.name = "HUMAN CAR " + liveryName;
        //this.SetLayerRecursively(this._humanCar, LayerMask.NameToLayer("RaceHumanCar"));
        this._humanCar.SetActive(true);
        this.HumanCarVisuals = this._humanCar.GetComponent<CarVisuals>();
        CarGarageInstance garageCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        if (garageCar == null || garageCar.CarDBKey != HumanInfo.Key)
        {
            garageCar = new CarGarageInstance();
            garageCar.SetupNewGarageInstance(HumanInfo);
        }
        GarageCarVisuals garageCarVisuals = HumanCarVisuals.GetComponent<GarageCarVisuals>();
        if (garageCarVisuals == null)
        {
            garageCarVisuals = this._humanCar.AddComponent<GarageCarVisuals>();
            garageCarVisuals.Setup(this.HumanCarVisuals, CarDatabase.Instance.GetCar(HumanInfo.ID));
        }
        garageCar.AppliedColourIndex = this.HumanCarVisuals.GetCurrentColorIndex();
        garageCar.AppliedLiveryName = this.HumanCarVisuals.CurrentlyAppliedLiveryObjName;
        garageCar.EliteCar = RaceEventInfo.Instance.LocalPlayerCarElite;
        if (garageCar.EliteCar && PlayerProfileManager.Instance.ActiveProfile != null)
        {
            CarGarageInstance carFromID =
                PlayerProfileManager.Instance.ActiveProfile.GetCarFromID(RaceEventInfo.Instance.LocalPlayerCarDBKey);
            if (carFromID != null)
            {
                garageCar.SportsUpgrade = carFromID.SportsUpgrade;
            }
        }
        RaceEventInfo.Instance.HumanCarGarageInstance = garageCar;
        GameObject gameObject = null;
        if (AsyncSwitching.IsLiveryName(liveryName) && liveryLoadedOk)
        {
            gameObject = AsyncSwitching.Instance.GetLivery(AsyncBundleSlotDescription.HumanCarLivery);
            if (gameObject != null)
            {
                gameObject.name = liveryName;
            }
        }
        this._humanLivery = gameObject;
        this.SetupHumanCarShaders();
        _humanSticker = string.IsNullOrEmpty(_humanSticker) ? HumanCarVisuals.DefaultStickerID : _humanSticker;
        this.BuildCarVisuals(this._humanCar, garageCar, this._humanLivery, _humanBodyShader,
            _humanRingShader,
            _humanHeadlightShader, _humanSticker, _humanSpoiler
            , PlayerProfileManager.Instance.ActiveProfile.DisplayName, _humanBodyHeight);
        if (RaceEventInfo.Instance.IsFirstOfSwitchBackRace())
        {
            //this._humanCarVisuals.ApplyNumberPlate(NumberPlateManager.Instance.RenderImmediate(RaceEventInfo.Instance.CurrentEvent.LoanCarGarageInstance.NumberPlate));
        }
        this.HumanTakeSnapshot();
        IsHumanCarFullySetup = true;
    }

    private void BuildCarVisuals(GameObject car, CarGarageInstance carGarageInstance,
        GameObject livery, string body, string ring, string headlight
        , string sticker, string spoiler, string numberPlateText, float bodyHeight)
    {
        CarVisuals carVisual = car.GetComponent<CarVisuals>();
        //component.SetCurrentColor(colourIndex);
        //component.MakeBakeDirty();
        //component.ApplyLivery(livery, true);

        var carID = carGarageInstance.ID;

        //BodyShader
        SetupBodyMaterial(carVisual, body);
        //Ring
        SetupRingMaterial(carVisual, ring);
        //Headlight
        SetupHeadlightMaterial(carVisual, headlight);
        //Sticker
        SetupSticker(carVisual, carID, sticker);
        //Spoiler
        SetupSpoiler(carVisual, spoiler);
        //BodyHeight
        carVisual.BodyHeight = bodyHeight;
        carVisual.ApplyNumberPlateText(numberPlateText);
    }


    public void SetupBodyMaterial(CarVisuals carVisual, string item)
    {
        if (string.IsNullOrEmpty(item))
            item = carVisual.DefaultBodyShaderID;
        var material = CarReflectionMapManager.GetCarBodyMaterial(item, reflType);
        carVisual.BodyMaterial = material;

        if (carVisual.HaveAdditionalRenderer)
        {
            var additionalMaterial = CarReflectionMapManager.GetCarBodyMaterial(carVisual.AdditionalShaderID, CarReflectionMapManager.ReflectionTexType.WorkshopNight);
            carVisual.AdditionalMaterial = additionalMaterial;
        }
    }

    public void SetupRingMaterial(CarVisuals carVisual, string item)
    {
        if (string.IsNullOrEmpty(item))
            item = carVisual.DefaultRingShaderID;
        var material = CarReflectionMapManager.GetCarRingMaterial(item, reflType);
        carVisual.RingMaterial = material;
    }

    public void SetupHeadlightMaterial(CarVisuals carVisual, string item)
    {
        if (string.IsNullOrEmpty(item))
            item = carVisual.DefaultHeadlightShaderID;
        var material = CarReflectionMapManager.GetCarHeadlightMaterial(item, reflType);
        carVisual.HeadLightMaterial = material;
    }

    public void SetupSticker(CarVisuals carVisual, string carID, string item)
    {
        Texture2D sticker;
        Vector2 texScale;
        if (string.IsNullOrEmpty(item) || item.ToLower() == "sticker_no")
        {
            texScale = Vector2.one;
            sticker = CarReflectionMapManager.GetSharedItemID<Texture2D>(item, ServerItemBase.AssetType.sticker,
                reflType);
        }
        else
        {
            CarReflectionMapManager.GetCarStickerTexture(carID, item, reflType, out sticker, out texScale);
        }
        carVisual.CacheStickerScale(texScale);
        carVisual.Sticker = sticker;
    }

    public void SetupSpoiler(CarVisuals carVisual, string item)
    {
        if (string.IsNullOrEmpty(item) || item.ToLower() == "spoiler_no")
        {
            carVisual.ClearSpoiler();
        }
        else
        {
            var spoiler = CarReflectionMapManager.GetSharedItemID<GameObject>(item, ServerItemBase.AssetType.spoiler,
                reflType);

            if (spoiler == null)
            {
                GTDebug.LogWarning(GTLogChannel.RaceState, string.Format("No spoiler found width ID '{0}'", item));
                return;
            }
            var spoilerInstance = Object.Instantiate(spoiler);
            carVisual.SetSpoiler(item, spoilerInstance);
        }
    }

    public bool SetupHeight(CarVisuals carVisual, float height)
    {
        if (carVisual.BodyHeight != height)
        {
            carVisual.BodyHeight = height;
            return true;
        }
        return false;
    }

    private void SetupHumanCarShaders()
    {
        if (RaceEventInfo.Instance.CurrentEvent == null)
        {
            return;
        }
        if (RaceEventInfo.Instance.CurrentEvent == GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial)
        {
            _humanBodyShader = GameDatabase.Instance.TutorialConfiguration.TutorialCarBodyShader;
            _humanRingShader = GameDatabase.Instance.TutorialConfiguration.TutorialCarRingShader;
            _humanHeadlightShader = GameDatabase.Instance.TutorialConfiguration.TutorialCarHeadlightShader;
            _humanSticker = GameDatabase.Instance.TutorialConfiguration.TutorialCarSticker;
            _humanSpoiler = GameDatabase.Instance.TutorialConfiguration.TutorialCarSpoiler;
            //_bodyHeight = GameDatabase.Instance.TutorialConfiguration.TutorialBodyHeight;
            return;
        }
        if (RaceEventInfo.Instance.CurrentEvent.IsDailyBattle() &&
            PlayerProfileManager.Instance.ActiveProfile.DailyBattlesLastEventAt == DateTime.MinValue)
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

        if (RaceEventInfo.Instance.CurrentEvent.IsDailyBattle())
        {
            CarCustomiseUtility.SetupRandomFurthestShaders(HumanCarVisuals, out _humanBodyShader, out _humanHeadlightShader,
                out _humanRingShader, out _humanSticker, out _humanSpoiler, out _humanBodyHeight);
            return;
        }

        _humanBodyShader = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedBodyPaintItemID;
        _humanRingShader = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedRingColorItemID;
        _humanHeadlightShader = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedFrontlightColorItemID;
        _humanSticker = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedStickerItemID;
        _humanSpoiler = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedSpoilerItemID;
        _humanBodyHeight = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedBodyHeight;
    }

    private void HumanTakeSnapshot()
    {
        //if (!CarSnapshotManager.Instance.SnapshotCacheExists(garageCar))
        //{
        //    Texture2D numPlateTex = this._humanCarVisuals.CurrentlyAppliedNumberPlateTex;
        //    CameraPostRender.Instance.AddProcess("Human snapshot " + garageCar.CarDBKey, delegate
        //    {
        //        CarSnapshotManager.Instance.GenerateSnapshot(garageCar, this._humanCar, numPlateTex, delegate(Texture2D snapshotTex)
        //        {
        //            this.HumanSetupForRace();
        //        });
        //    });
        //}
        //else
        //{
        //    this.HumanSetupForRace();
        //}

        if (!ScreenShot.Instance.SnapshotCacheExists(HumanCarVisuals))
        {
            ScreenShot.Instance.PlaceCarAndRender(HumanCarVisuals, HumanInfo, true, true);
        }
        HumanSnapShot = ScreenShot.Instance[HumanCarVisuals];
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

        AsyncBundleSlotDescription desc = AsyncBundleSlotDescription.AICar;
        string name = this.AiInfo.ModelPrefabString;
        RacePlayerInfoComponent component =
            CompetitorManager.Instance.OtherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
        bool isEliteApplied = component.IsEliteLiveryApplied || component.IsEliteSportsLiveryApplied;
        if (!string.IsNullOrEmpty(this.AiInfo.EliteOverrideCarAssetID) && isEliteApplied)
        {
            name = this.AiInfo.EliteOverrideCarAssetID;
        }
        AsyncSwitching.Instance.RequestAsset(desc, name, this.AICarLoaded, gameObject);
    }

    public void AICarLoaded(bool loadedOk, string name)
    {
        this.AiLoadLivery();
    }

    private void AiLoadLivery()
    {
        string text = string.Empty;
        if (RaceEventInfo.Instance != null && RaceEventInfo.Instance.CurrentEvent != null)
        {
            text = RaceEventInfo.Instance.CurrentEvent.AIDriverLivery;
        }
        if (!string.IsNullOrEmpty(this.AiInfo.DefaultLiveryBundleName))
        {
            text = this.AiInfo.DefaultLiveryBundleName;
        }
        RacePlayerInfoComponent component =
            CompetitorManager.Instance.OtherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
        if (RaceEventInfo.Instance != null &&
            (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent() ||
             RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent()))
        {
            string appliedLivery = component.AppliedLivery;
            bool flag = !string.IsNullOrEmpty(text);
            bool flag2 = !string.IsNullOrEmpty(appliedLivery);
            if (!flag && flag2)
            {
                text = appliedLivery;
            }
        }

        if (false)//AsyncSwitching.IsLiveryName(text))
        {
            AsyncSwitching.Instance.RequestAsset(AsyncBundleSlotDescription.AICarLivery, text, this.AiLiveryLoaded,
                gameObject);
        }
        else
        {
            this.AiLiveryLoaded(true, text);
        }
    }

    private void AiLiveryLoaded(bool liveryLoadedOk, string liveryName)
    {
        this._aiCar = AsyncSwitching.Instance.GetCar(AsyncBundleSlotDescription.AICar);
        this._aiCar.name = "AI CAR " + liveryName;
        //this.SetLayerRecursively(this._aiCar, LayerMask.NameToLayer("RaceAICar"));
        GameObject gameObject = null;
        if (AsyncSwitching.IsLiveryName(liveryName) && liveryLoadedOk)
        {
            gameObject = AsyncSwitching.Instance.GetLivery(AsyncBundleSlotDescription.AICarLivery);
            if (gameObject != null)
            {
                gameObject.name = liveryName;
            }
        }
        //this._aiLivery = gameObject;
        this.BuildAICarVisuals(null);
    }

    private void BuildAICarVisuals(GameObject livery)
    {
        this.AiCarVisuals = this._aiCar.GetComponent<CarVisuals>();
        this._ClearAISnapshot = false;
        //this._aiCarVisuals.SetCurrentColor(this._aiCarColour);
        //this._aiCarVisuals.SetUpAsCheap();
        this.ChooseAICarShaders();
        CarGarageInstance aiCarGarage = new CarGarageInstance();
        aiCarGarage.SetupNewGarageInstance(AiInfo);
        var bodyItem = CarCustomiseUtility.GetEquipItemByID(VirtualItemType.BodyShader,
            _aiBodyShader, AiInfo.Key);
        var ringItem = CarCustomiseUtility.GetEquipItemByID(VirtualItemType.RingShader,
            _aiRingShader, AiInfo.Key);
        var headlightItem = CarCustomiseUtility.GetEquipItemByID(VirtualItemType.HeadLighShader,
            _aiHeadlightShader, AiInfo.Key);
        var stickerItem = CarCustomiseUtility.GetEquipItemByID(VirtualItemType.CarSticker,
            _aiSticker, AiInfo.Key);
        var spoilerItem = CarCustomiseUtility.GetEquipItemByID(VirtualItemType.CarSpoiler,
            _aiSpoiler, AiInfo.Key);

        aiCarGarage.EquipItemCollection.Add(bodyItem);
        aiCarGarage.EquipItemCollection.Add(ringItem);
        aiCarGarage.EquipItemCollection.Add(headlightItem);
        if (stickerItem != null)
            aiCarGarage.EquipItemCollection.Add(stickerItem);
        if (spoilerItem != null)
            aiCarGarage.EquipItemCollection.Add(spoilerItem);
        aiCarGarage.BodyHeight = _aiBodyHeight;

        GarageCarVisuals garageCarVisuals = AiCarVisuals.GetComponent<GarageCarVisuals>();
        if (garageCarVisuals == null)
        {
            garageCarVisuals = this._aiCar.AddComponent<GarageCarVisuals>();
            garageCarVisuals.Setup(this.AiCarVisuals, CarDatabase.Instance.GetCar(AiInfo.ID));
        }
        //carGarageInstance.AppliedColourIndex = this._aiCarVisuals.GetCurrentColorIndex();
        aiCarGarage.AppliedLiveryName = this.AiCarVisuals.CurrentlyAppliedLiveryObjName;
        aiCarGarage.EliteCar = RaceEventInfo.Instance.OpponentCarElite;
         RaceEventInfo.Instance.AICarGarageInstance = aiCarGarage;

        //this._aiCarVisuals.MakeBakeDirty();
        //this._aiCarVisuals.ApplyLivery(livery, true);
        var carID = RaceEventInfo.Instance.AICarGarageInstance.CarDBKey;

        //Debug.Log(_aiBodyShader);
        //Debug.Log(_aiRingShader);
        //Debug.Log(_aiHeadlightShader);
        //Debug.Log(_aiSticker);
        //Debug.Log(_aiSpoiler);
        //Debug.Log(_aiBodyHeight);

        _aiSticker = string.IsNullOrEmpty(_aiSticker) ? AiCarVisuals.DefaultStickerID : _aiSticker;

        //BodyShader
        SetupBodyMaterial(AiCarVisuals, _aiBodyShader);
        //Ring
        SetupRingMaterial(AiCarVisuals, _aiRingShader);
        //Headlight
        SetupHeadlightMaterial(AiCarVisuals, _aiHeadlightShader);
        //Sticker
        SetupSticker(AiCarVisuals, carID, _aiSticker);
        //Spoilerset
        SetupSpoiler(AiCarVisuals, _aiSpoiler);
        //BodyHeight
        AiCarVisuals.BodyHeight = AiCarVisuals.DefaultBodyHeight;//_aiBodyHeight;

        this.AiCarVisuals.ApplyNumberPlateText(RaceEventInfo.Instance.GetRivalName());
        this.AiSetupNumberPlate();
    }

    private void ChooseAICarShaders()
    {
        if (RaceEventInfo.Instance.IsCrewRaceEvent || RaceEventInfo.Instance.IsHighStakesEvent ||
            RaceEventInfo.Instance.CurrentEvent.UseCustomShader)
        {
            _aiBodyShader = RaceEventInfo.Instance.CurrentEvent.BodyShader;
            _aiHeadlightShader = RaceEventInfo.Instance.CurrentEvent.HeadLightShader;
            _aiRingShader = RaceEventInfo.Instance.CurrentEvent.RingShader;
            _aiSticker = RaceEventInfo.Instance.CurrentEvent.Sticker;
            _aiSpoiler = RaceEventInfo.Instance.CurrentEvent.Spoiler;
            _aiBodyHeight = RaceEventInfo.Instance.CurrentEvent.BodyHeight;
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
            out _aiRingShader, out _aiSticker, out _aiSpoiler, out _aiBodyHeight);
    }


    private void AiSetupNumberPlate()
    {
        //NumberPlate numberPlate = new NumberPlate();
        //PlayerInfo playerInfo = CompetitorManager.Instance.OtherCompetitor.PlayerInfo;
        //numberPlate.Text = playerInfo.Persona.GetNumberPlate();
        //numberPlate.TextColor = UnityEngine.Color.white;
        //numberPlate.BackgroundColor = new UnityEngine.Color(0.8f, 0f, 0.1f, 1f);
        //numberPlate.BorderColor = UnityEngine.Color.white;
        //NumberPlateManager.Instance.RenderOpponentNumberPlate(numberPlate, delegate(Texture2D tex)
        //{
        //    this._aiCarVisuals.ApplyNumberPlate(tex);
        //    this.AiTakeSnapshot();
        //});
        this.AiTakeSnapshot();
    }

    private void AiTakeSnapshot()
    {
        //if (this._ClearAISnapshot)
        //{
        //    CarSnapshotManager.Instance.ClearSnapshotFromCache(carGarageInstance);
        //}
        //if (!CarSnapshotManager.Instance.SnapshotCacheExists(carGarageInstance))
        //{
        //    Texture2D currentlyAppliedNumberPlateTex = this._aiCarVisuals.CurrentlyAppliedNumberPlateTex;
        //    CarSnapshotManager.Instance.GenerateSnapshot(carGarageInstance, this._aiCar, currentlyAppliedNumberPlateTex, delegate(Texture2D snapshotTex)
        //    {
        //        this.AiSetupForRace();
        //    });
        //}
        //else
        //{
        //    this.AiSetupForRace();
        //}

        _humanCar.SetActive(false);

        if (this._ClearAISnapshot)
        {
            ScreenShot.Instance.ClearSnapshotFromCache(AiCarVisuals);
        }
        if (!ScreenShot.Instance.SnapshotCacheExists(AiCarVisuals))
        {
            ScreenShot.Instance.PlaceCarAndRender(AiCarVisuals, AiInfo, true, false,false);
        }

        AiSnapShot = ScreenShot.Instance[AiCarVisuals];
        AiSnapShotReverse = ScreenShot.Instance.PlaceCarAndRender(AiCarVisuals, AiInfo, false, false, true);
        IsAiCarFullySetup = true;
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform transform in obj.transform)
        {
            this.SetLayerRecursively(transform.gameObject, layer);
        }
    }
}
