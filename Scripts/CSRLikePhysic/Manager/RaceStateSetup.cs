//using DataSerialization;

using System;
using System.Collections.Generic;
using KingKodeStudio;
using UnityEngine;
using Object = UnityEngine.Object;

public class RaceStateSetup : RaceStateBase
{
    private enum SetupStates
    {
        WaitingForDatabase,
        WaitingForHumanCar,
        WaitingForAiCar
    }

    private CarInfo _humanInfo;

    private GameObject _humanCar;

    private CarVisuals _humanCarVisuals;

    private bool _isHumanCarFullySetup;

    private CarInfo _aiInfo;

    private GameObject _aiCar;

    private CarVisuals _aiCarVisuals;

    private bool _isAiCarFullySetup;

    //private bool _ClearAISnapshot;

    private SetupStates _state;

    private GameObject _humanLivery;

    private GameObject _aiLivery;


    private string _humanBodyShader;
    private string _humanRingShader;
    private string _humanHeadlightShader;
    private string _humanSticker;
    private string _humanSpoiler;
    private float _humanBodyHeight;

    private string _aiBodyShader;
    private string _aiRingShader;
    private string _aiHeadlightShader;
    private string _aiSticker;
    private string _aiSpoiler;
    private float _aiBodyHeight;

    public RaceStateSetup(RaceStateMachine inMachine) : base(inMachine, RaceStateEnum.setup)
    {

    }

    public override void Enter()
    {
        Camera.main.useOcclusionCulling = false;
        RaceCarAudio.FadeDownCarAudio(0f);
        //PinDetail worldTourPinPinDetail = RaceEventInfo.Instance.CurrentEvent.GetWorldTourPinPinDetail();
        //PhilsFlag.Instance.useBigFlag = (worldTourPinPinDetail != null && worldTourPinPinDetail.GetLoadingScreen() == ScreenID.VSDummy);
        //PhilsFlag.Instance.Show();
        WinLoseScreen winLoseFlagsGo = WinLoseScreen.Instance;
        if (winLoseFlagsGo == null)
        {
            var winLoseFlags = Resources.Load<WinLoseScreen>("Prefabs/HUDElements/WinLooseFlags");
            winLoseFlagsGo = Object.Instantiate(winLoseFlags);
        }

        winLoseFlagsGo.gameObject.SetActive(false);
        
        
        //TouchManager.Instance.GesturesEnabled = false;
        NitrousTutorial.Instance.ShouldActivate();
    }

    //private float m_t;

    public override void FixedUpdate()
    {
        //if (Time.time - m_t < 1) return;
        switch (this._state)
        {
            case SetupStates.WaitingForDatabase:
                if (CarDatabase.Instance.isReady)
                {
                    this.PreloadRaceRewardScreen();
                    this.StartHumanCarLoading();
                    this._state = SetupStates.WaitingForHumanCar;
                }
                break;
            case SetupStates.WaitingForHumanCar:
                if (this._isHumanCarFullySetup)
                {
                    if (CompetitorManager.Instance.OtherCompetitor == null)
                    {
                        this.machine.SetState(RaceStateEnum.enter);
                    }
                    else
                    {
                        this.StartAICarLoading();
                        this._state = SetupStates.WaitingForAiCar;
                    }
                    this.HideRaceRewardScreen();
                }
                break;
            case SetupStates.WaitingForAiCar:
                if (this._isAiCarFullySetup)
                {
                    //CarReflectionMapManager.UnloadCubemap(CarReflectionMapManager.ReflectionTexType.Showroom);
                    //PhilsFlag.Instance.Hide();
                    if (WinLoseScreen.Instance != null)
                    {
                        WinLoseScreen.Instance.gameObject.SetActive(false);
                    }
                    this.machine.SetState(RaceStateEnum.enter);
                }
                break;
        }

        //m_t = Time.time;
    }

    private void StartHumanCarLoading()
    {
        this._humanInfo = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.LocalPlayerCarDBKey);
        if (_humanInfo == null)
        {
            this._humanInfo = CarDatabase.Instance.GetDefaultCar();
        }
        //var carPrefab = ResourceManager.GetCarAsset<GameObject>(_humanInfo.Key, ServerItemBase.AssetType.garage_model);
        //_humanCar = Object.Instantiate(carPrefab);
        // var pos = RaceEnvironmentSettings.Instance.HumanStartPosition.position;
        ////pos.y += carPrefab.transform.position.y;
        //_humanCar.transform.localPosition = pos;
        //_humanCarVisuals = this._humanCar.GetComponent<CarVisuals>();
        //HumanCarLoaded(true,null);
        AsyncBundleSlotDescription desc = AsyncBundleSlotDescription.HumanCar;
        string name = this._humanInfo.ModelPrefabString;
        RacePlayerInfoComponent component =
            CompetitorManager.Instance.LocalCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
        bool flag = component.IsEliteLiveryApplied || component.IsEliteSportsLiveryApplied;
        if (!string.IsNullOrEmpty(this._humanInfo.EliteOverrideCarAssetID) && flag)
        {
            name = this._humanInfo.EliteOverrideCarAssetID;
        }
        AsyncSwitching.Instance.RequestAsset(desc, name, this.HumanCarLoaded, RaceController.Instance.gameObject, true,
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
        if (!AsyncSwitching.IsLiveryName(text) && !string.IsNullOrEmpty(this._humanInfo.DefaultLiveryBundleName))
        {
            text = this._humanInfo.DefaultLiveryBundleName;
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
        if (false)//AsyncSwitching.IsLiveryName(text))
        {
            AsyncSwitching.Instance.RequestAsset(AsyncBundleSlotDescription.HumanCarLivery, text, this.HumanLiveryLoaded,
                RaceController.Instance.gameObject, true, null);
        }
        else
        {
            this.HumanLiveryLoaded(true, text);
        }
        //HumanLiveryLoaded(true,null);
    }

    private void HumanLiveryLoaded(bool liveryLoadedOk, string liveryName)
    {
        this._humanCar = AsyncSwitching.Instance.GetCar(AsyncBundleSlotDescription.HumanCar);
        this._humanCar.name = "HUMAN CAR " + liveryName;
        //this.SetLayerRecursively(this._humanCar, LayerMask.NameToLayer("RaceHumanCar"));
        this._humanCar.SetActive(true);
        this._humanCarVisuals = this._humanCar.GetComponent<CarVisuals>();
        CarGarageInstance garageCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        if (garageCar == null || garageCar.CarDBKey != _humanInfo.Key)
        {
            garageCar = new CarGarageInstance();
            garageCar.SetupNewGarageInstance(_humanInfo);
        }
        garageCar.AppliedColourIndex = this._humanCarVisuals.GetCurrentColorIndex();
        garageCar.AppliedLiveryName = this._humanCarVisuals.CurrentlyAppliedLiveryObjName;
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
        this.BuildCarVisuals(this._humanCar, garageCar, this._humanLivery, _humanBodyShader, _humanRingShader,
            _humanHeadlightShader, _humanSticker, _humanSpoiler
            , PlayerProfileManager.Instance.ActiveProfile.DisplayName, _humanBodyHeight);
        if (RaceEventInfo.Instance.IsFirstOfSwitchBackRace())
        {
            //this._humanCarVisuals.ApplyNumberPlate(NumberPlateManager.Instance.RenderImmediate(RaceEventInfo.Instance.CurrentEvent.LoanCarGarageInstance.NumberPlate));
        }
        this.HumanTakeSnapshot();
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
        //carVisual.ApplyShaders(carID, body, ring, headlight, sticker, spoiler);
        
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


    public void SetupBodyMaterial(CarVisuals carVisual,string item)
    {
        if (string.IsNullOrEmpty(item))
            item = carVisual.DefaultBodyShaderID;
        var material = CarReflectionMapManager.GetCarBodyMaterial(item, RaceEnvironmentSettings.Instance.ReflectionTextureType);
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
        var material = CarReflectionMapManager.GetCarRingMaterial(item, CarReflectionMapManager.ReflectionTexType.RaceDay);
        carVisual.RingMaterial = material;
    }

    public void SetupHeadlightMaterial(CarVisuals carVisual, string item)
    {
        if (string.IsNullOrEmpty(item))
            item = carVisual.DefaultHeadlightShaderID;
        var material = CarReflectionMapManager.GetCarHeadlightMaterial(item, RaceEnvironmentSettings.Instance.ReflectionTextureType);
        carVisual.HeadLightMaterial = material;
    }

    public void SetupSticker(CarVisuals carVisual, string carID, string item)
    {
        Texture2D sticker;
        Vector2 texScale;
        
        if (string.IsNullOrEmpty(item) || item.ToLower() == "sticker_no")
        {
            texScale = Vector2.one;
            sticker = CarReflectionMapManager.GetSharedItemID<Texture2D>(item, ServerItemBase.AssetType.sticker, RaceEnvironmentSettings.Instance.ReflectionTextureType);
        }
        else
        {
            CarReflectionMapManager.GetCarStickerTexture(carID, item, RaceEnvironmentSettings.Instance.ReflectionTextureType, out sticker, out texScale);
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
            var spoiler = CarReflectionMapManager.GetSharedItemID<GameObject>(item, ServerItemBase.AssetType.spoiler, RaceEnvironmentSettings.Instance.ReflectionTextureType);

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



private void ResetCarVisuals(GameObject car, GameObject livery,string body,string ring,string headlight
        ,string sticker,string spoiler,bool isHuman,float bodyHeight)
	{
	    this.BuildCarVisuals(car, null, livery,body,ring,headlight,sticker,spoiler,
            PlayerProfileManager.Instance.ActiveProfile.DisplayName, bodyHeight);
        var racecarVisual = car.GetComponent<RaceCarVisuals>();
        var carVisual = car.GetComponent<CarVisuals>();
        var raceCar = car.GetComponent<RaceCar>();
        if (racecarVisual == null || carVisual == null || raceCar == null)
        {
            return;
        }
        racecarVisual.Reset();
        racecarVisual.Setup(carVisual, raceCar, isHuman);
        carVisual.BakeCarShadows();
        carVisual.SwichToRaceWheels();
	}

	public void RebuildHumanCarVisuals()
	{
        SetupHumanCarShaders();
		this.ResetCarVisuals(this._humanCar, this._humanLivery,_humanBodyShader,_humanRingShader,_humanHeadlightShader
            ,_humanSticker,_humanSpoiler, true,_humanBodyHeight);
	}

	public void RebuildAICarVisuals()
	{
		if (this._aiCar != null)
		{
            SetupAICarShaders();
		    this.ResetCarVisuals(this._aiCar, this._aiLivery, _aiBodyShader, _aiRingShader, _aiHeadlightShader
		        , _aiSticker, _aiSpoiler, false,_aiBodyHeight);
		}
	}

	private void SetupHumanCarShaders()
	{
        if (RaceEventInfo.Instance.CurrentEvent == null)
        {
            return;
        }
        if (RaceEventInfo.Instance.CurrentEvent == GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial
            || RaceEventInfo.Instance.CurrentEvent == GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial2)
        {
            _humanBodyShader = GameDatabase.Instance.TutorialConfiguration.TutorialCarBodyShader;
            _humanRingShader = GameDatabase.Instance.TutorialConfiguration.TutorialCarRingShader;
            _humanHeadlightShader = GameDatabase.Instance.TutorialConfiguration.TutorialCarHeadlightShader;
            _humanSticker = GameDatabase.Instance.TutorialConfiguration.TutorialCarSticker;
            _humanSpoiler = GameDatabase.Instance.TutorialConfiguration.TutorialCarSpoiler;
            //_bodyHeight = GameDatabase.Instance.TutorialConfiguration.TutorialBodyHeight;
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
        if (this._humanInfo.IsBossCarOverride)
        {
            _humanBodyShader = _humanInfo.BodyShaderOverride;
            _humanRingShader = _humanInfo.RingShaderOverride;
            _humanHeadlightShader = _humanInfo.HeadlightShaderOverride;
            _humanSticker = _humanInfo.StickerOverride;
            _humanSpoiler = _humanInfo.SpoilerOverride;
            //this._humanCarVisuals.ForceColor(forcedColor);
            return;
        }
        if (RaceEventInfo.Instance.IsFirstOfSwitchBackRace())
        {
            CarInfo car = CarDatabase.Instance.GetCar(this._humanInfo.Key + "Boss");
            if (car != null)
            {
                //UnityEngine.Color forcedColor2 = new UnityEngine.Color(car.ColourOverrideR, car.ColourOverrideG, car.ColourOverrideB);
                //this._humanCarVisuals.ForceColor(forcedColor2);
                return;
            }
        }

	    var sticker = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedStickerItemID;
        _humanBodyShader = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedBodyPaintItemID;
        _humanRingShader = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedRingColorItemID;
        _humanHeadlightShader = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedFrontlightColorItemID;
	    _humanSticker = string.IsNullOrEmpty(sticker) ? _humanCarVisuals.DefaultStickerID : sticker;
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
        HumanSetupForRace();
	}

	private void HumanSetupForRace()
	{
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        RaceCar raceCar = this._humanCar.AddComponent<RaceCar>();
        RaceCarVisuals raceCarVisuals = this._humanCar.AddComponent<RaceCarVisuals>();
        raceCarVisuals.Setup(this._humanCarVisuals, raceCar, true);
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
        carUpgradeSetup.CalculateFettle(this._humanInfo, currentPPIndex);
        carUpgradeSetup.SetupConsumableParams(this._humanInfo, currentPPIndex);
	    bool flag = RaceEventInfo.Instance.CurrentEvent.IsHighStakesEvent();
        if (flag && BoostNitrous.HaveBoostNitrous())
        {
            CarUpgradeStatus carUpgradeStatus = carUpgradeSetup.UpgradeStatus[eUpgradeType.NITROUS];
            if (carUpgradeStatus.levelFitted == 0)
            {
                carUpgradeStatus.levelFitted = 1;
            }
        }
        bool isLocalPlayer = true;
        raceCar.Initialise(this._humanInfo, this._humanCarVisuals, isLocalPlayer, carUpgradeSetup);
        RaceCompetitor localCompetitor = CompetitorManager.Instance.LocalCompetitor;
        localCompetitor.SetUpObjectReferences(this._humanCar,raceCar.physics);
        var relRotateBody = _humanCarVisuals.gameObject.AddComponent<ReflectionVectorRotate_Final>();
        relRotateBody.Init(raceCar.physics);
	    relRotateBody.AddMaterial(_humanCarVisuals.BodyMaterial, 1);
	    relRotateBody.AddMaterial(_humanCarVisuals.GlassMaterial, -1);
        ////CompetitorManager.Instance.LocalCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>().PopulatePostRaceSetup();
        float raceDistance;
        if (RaceEventInfo.Instance.CurrentEvent == null)
        {
            raceDistance = 402.325f;
        }
        else
        {
            raceDistance = ((!RaceEventInfo.Instance.CurrentEvent.IsHalfMile) ? 402.325f : 804.65f);
        }
        RaceHUDManager.Initialise();
        RaceHUDManager.Instance.CreateHUD(CompetitorManager.Instance.LocalCompetitor.CarPhysics, raceDistance);
        RaceHUDManager.Instance.ResetHUD();
	    RaceController.Instance.Inputs = RaceHUDController.Instance.GetInputManager();
        this.HumanReady();
	}

	private void HumanReady()
	{
		this._isHumanCarFullySetup = true;
	}

	public void StartAICarLoading()
	{
        bool flag = CarDatabase.Instance.PeekGetCar(RaceEventInfo.Instance.OpponentCarDBKey);
        if (flag)
        {
            this._aiInfo = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.OpponentCarDBKey);
        }
        else
        {
            eCarTier baseCarTier = this._humanInfo.BaseCarTier;
            List<CarInfo> carsOfTier = CarDatabase.Instance.GetCarsOfTier(baseCarTier);
            if (carsOfTier.Count > 0)
            {
                this._aiInfo = carsOfTier[0];
            }
            else
            {
                this._aiInfo = CarDatabase.Instance.GetDefaultCar();
            }
        }

        //    var carPrefab = ResourceManager.GetCarAsset<GameObject>(_aiInfo.Key, ServerItemBase.AssetType.garage_model);
        //    _aiCar = Object.Instantiate(carPrefab);
        //    var pos = RaceEnvironmentSettings.Instance.AiStartPosition.position;
        ////pos.y += carPrefab.transform.position.y;
        //_aiCar.transform.position = pos;
        //    _aiCarVisuals = this._aiCar.GetComponent<CarVisuals>();
        //    //AiSetupForRace();
        //AICarLoaded(true,null);


        AsyncBundleSlotDescription desc = AsyncBundleSlotDescription.AICar;
        string name = this._aiInfo.ModelPrefabString;
        RacePlayerInfoComponent component = CompetitorManager.Instance.OtherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
        bool flag2 = component.IsEliteLiveryApplied || component.IsEliteSportsLiveryApplied;
        if (!string.IsNullOrEmpty(this._aiInfo.EliteOverrideCarAssetID) && flag2)
        {
            name = this._aiInfo.EliteOverrideCarAssetID;
        }
        AsyncSwitching.Instance.RequestAsset(desc, name, this.AICarLoaded, RaceController.Instance.gameObject);
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
        if (!string.IsNullOrEmpty(this._aiInfo.DefaultLiveryBundleName))
        {
            text = this._aiInfo.DefaultLiveryBundleName;
        }
        RacePlayerInfoComponent component = CompetitorManager.Instance.OtherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
        if (RaceEventInfo.Instance != null && (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent() || RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent()))
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
            AsyncSwitching.Instance.RequestAsset(AsyncBundleSlotDescription.AICarLivery, text, this.AiLiveryLoaded, RaceController.Instance.gameObject);
        }
        else
        {
            this.AiLiveryLoaded(true, text);
        }
        //this.AiLiveryLoaded(true, null);
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
        this._aiLivery = gameObject;
        this.BuildAICarVisuals(null);
//        Debug.Log("Ai Livery loaded");

	}

	private void BuildAICarVisuals(GameObject livery)
	{
        this._aiCarVisuals = this._aiCar.GetComponent<CarVisuals>();
        //this._ClearAISnapshot = false;
        //this._aiCarVisuals.SetCurrentColor(this._aiCarColour);
        //this._aiCarVisuals.SetUpAsCheap();

	    if (RaceEventInfo.Instance.AICarGarageInstance == null)
	    {
	        CarGarageInstance carGarageInstance = new CarGarageInstance();
	        carGarageInstance.SetupNewGarageInstance(_aiInfo);
            //carGarageInstance.AppliedColourIndex = this._aiCarVisuals.GetCurrentColorIndex();
	        carGarageInstance.AppliedLiveryName = this._aiCarVisuals.CurrentlyAppliedLiveryObjName;
	        carGarageInstance.EliteCar = RaceEventInfo.Instance.OpponentCarElite;
            RaceEventInfo.Instance.AICarGarageInstance = carGarageInstance;
            this.SetupAICarShaders();
	    }
	    else
	    {
	        var aiCar = RaceEventInfo.Instance.AICarGarageInstance;
            var sticker = aiCar.AppliedStickerItemID;
            _aiBodyShader = aiCar.AppliedBodyPaintItemID;
            _aiRingShader = aiCar.AppliedRingColorItemID;
            _aiHeadlightShader = aiCar.AppliedFrontlightColorItemID;
            _aiSticker = !string.IsNullOrEmpty(sticker)?aiCar.AppliedStickerItemID:_aiCarVisuals.DefaultStickerID;
            _aiSpoiler = aiCar.AppliedSpoilerItemID;
            _aiBodyHeight = aiCar.AppliedBodyHeight;
	    }

        //this._aiCarVisuals.MakeBakeDirty();
        //this._aiCarVisuals.ApplyLivery(livery, true);
	    var carID = RaceEventInfo.Instance.AICarGarageInstance.CarDBKey;
        //this._aiCarVisuals.ApplyShaders(carID, _aiBodyShader, _aiRingShader, _aiHeadlightShader
        //    , _aiSticker, _aiSpoiler);

        //BodyShader
        SetupBodyMaterial(_aiCarVisuals, _aiBodyShader);
        //Ring
        SetupRingMaterial(_aiCarVisuals, _aiRingShader);
        //Headlight
        SetupHeadlightMaterial(_aiCarVisuals, _aiHeadlightShader);
        //Sticker
        SetupSticker(_aiCarVisuals, carID, _aiSticker);
        //Spoiler
        SetupSpoiler(_aiCarVisuals, _aiSpoiler);
        //BodyHeight
        _aiCarVisuals.BodyHeight = _aiCarVisuals.DefaultBodyHeight;//_aiBodyHeight;

        this._aiCarVisuals.ApplyNumberPlateText(RaceEventInfo.Instance.GetRivalName());
        this.AiSetupNumberPlate();
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
            _aiBodyHeight = RaceEventInfo.Instance.CurrentEvent.BodyHeight;
            //this._ClearAISnapshot = true;
            return;
        }
        if (this._aiInfo.IsBossCarOverride)
        {
            _aiBodyShader = _aiInfo.BodyShaderOverride;
            _aiHeadlightShader = _aiInfo.HeadlightShaderOverride;
            _aiRingShader = _aiInfo.RingShaderOverride;
            _aiSticker = _aiInfo.StickerOverride;
            _aiSpoiler = _aiInfo.SpoilerOverride;
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

        CarCustomiseUtility.SetupRandomFurthestShaders(_aiCarVisuals,out _aiBodyShader, out _aiHeadlightShader,
            out _aiRingShader, out _aiSticker, out _aiSpoiler,out _aiBodyHeight);
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
        AiSetupForRace();
	}

	private void AiSetupForRace()
	{
        AIPlayer aIPlayer = this._aiCar.AddComponent<AIPlayer>();
        aIPlayer.SetupData =  RaceEventInfo.Instance.AIDriverData;
        aIPlayer.IsTightLoopRun = false;
        RaceCar raceCar = this._aiCar.AddComponent<RaceCar>();
        RaceCarVisuals raceCarVisuals = this._aiCar.AddComponent<RaceCarVisuals>();
        raceCarVisuals.Setup(this._aiCarVisuals, raceCar, false);
	    CarUpgradeSetup carUpgradeSetup =RaceEventInfo.Instance.OpponentCarUpgradeSetup;
        if (carUpgradeSetup == null)
        {
            carUpgradeSetup = CarUpgradeSetup.NullCarSetup;
        }
        bool isLocalPlayer = false;
        raceCar.Initialise(this._aiInfo, this._aiCarVisuals, isLocalPlayer, carUpgradeSetup);
        RaceCompetitor otherCompetitor = CompetitorManager.Instance.OtherCompetitor;
        otherCompetitor.SetUpObjectReferences(this._aiCar, raceCar.physics);
        var relRotate = _aiCarVisuals.gameObject.AddComponent<ReflectionVectorRotate_Final>();
	    relRotate.Init(raceCar.physics);
        relRotate.AddMaterial(_aiCarVisuals.BodyMaterial, 1);
	    relRotate.AddMaterial(_aiCarVisuals.GlassMaterial, -1);

        RacePlayerInfoComponent component = otherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
	    int pPIndex = component.PPIndex;
        carUpgradeSetup.CalculateFettle(this._aiInfo, pPIndex);
	    carUpgradeSetup.SetupConsumableParams(this._aiInfo, pPIndex);//component.PPIndex);

        RaceHUDController.Instance.SetOpponentPhysics(otherCompetitor.CarPhysics);
        this.AiReady();
	}

	private void AiReady()
	{
		this._isAiCarFullySetup = true;
	}

	private void SetLayerRecursively(GameObject obj, int layer)
	{
		obj.layer = layer;
		foreach (Transform transform in obj.transform)
		{
			this.SetLayerRecursively(transform.gameObject, layer);
		}
	}

	private void PreloadRaceRewardScreen()
	{
        RaceRewardScreen.DoNotUpdate = true;
        var screen = ScreenManager.Instance.CreateScreenGO(ScreenID.RaceRewards);
        screen.gameObject.SetActive(false);
        //RaceRewardScreen.Instance.Hide(new Vector3(0f, 0f, 5f));
	}

	private void HideRaceRewardScreen()
	{
        //RaceRewardScreen.DoNotUpdate = false;
        //RaceRewardScreen.Instance.Unhide();
        //RaceRewardScreen.Instance.ResetScreen();
        //RaceRewardScreen.Instance.gameObject.SetActive(false);
	}
}
