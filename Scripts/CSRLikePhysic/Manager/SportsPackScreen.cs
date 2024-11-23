using Metrics;
using System;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SportsPackScreen : ZHUDScreen,IBundleOwner
{
	private GUICameraShake cameraShake;

	public AnimationCurve CurveControlShake;

	public Image LeftCarSnapshot;

	public CarInfoUI carInfoLeft;

	public CarStatsElem LeftStatElement;

	public GameObject RightStickerOpen;

	public Image RightCarSnapshot;

	public CarInfoUI carInfoRight;

	public CarStatsElem RightStatElement;

	public GameObject LeftPack;

	public GameObject RightPack;

	public GameObject DealSticker;

	public GameObject ORText;

	public TextMeshProUGUI SportPackCostText;

	private CarInfo wonCarInfo;

	public GameObject BlankSwatchPrefab;

	public GameObject SwatchPrefab;

	public GameObject LeftSwatchStartNode;

	public GameObject RightSwatchStartNode;

	private AsyncBundleSlotDescription EliteLiverySlot = AsyncBundleSlotDescription.HumanCarLivery;

	private AsyncBundleSlotDescription EliteSportsLiverySlot = AsyncBundleSlotDescription.HumanCarLivery;

	private EliteVisuals elite_visuals;

	private EliteVisuals elite_sports_visuals;

	private int _sportsPackCost;

	private CarUpgradeSetup sportspackCarUpgradeSetup;

	public static string CarToAward;

	public static bool CarWonAlreadyOwned;

	public static int CarWonAlreadyOwnedPP;

	private CarGarageInstance carInstance;

	private string EliteLiveryName;

	private string EliteSportsLiveryName;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.SportsPack;
		}
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		base.OnActivate(zAlreadyOnStack);
		this.Anim_Initialize();
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		this.carInstance = activeProfile.GetCarFromID(SportsPackScreen.CarToAward);
		this.wonCarInfo = CarDatabase.Instance.GetCar(SportsPackScreen.CarToAward);
		this.SetupLiveryInfo(SportsPackScreen.CarToAward);
        //this.LeftCarSnapshot.renderer.enabled = false;
        //this.RightCarSnapshot.renderer.enabled = false;
		this.RightStickerOpen.SetActive(true);
		this.carInfoLeft.SetCurrentCarIDKey(SportsPackScreen.CarToAward, false);
		this.carInfoRight.SetCurrentCarIDKey(SportsPackScreen.CarToAward, false);
		this.carInfoLeft.ShowCarStats(true);
		this.carInfoRight.ShowCarStats(true);
		this.GetEliteCarSnap();
		this.GetSportsPackCarSnap();
		this.CalculateSportsPackSetup(this.carInstance);
		this.SportPackCostText.text = CurrencyUtils.GetGoldString(this._sportsPackCost);
		if (!this.ShowStargazerPopupIfAppropriate())
		{
			this.StartAnimation();
		}
	}

	private void CalculateSportsPackSetup(CarGarageInstance wonCarInstance)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int num = this.wonCarInfo.BasePerformanceIndex;
		int baseCarPP = this.wonCarInfo.BasePerformanceIndex;
		eCarTier currentTier = this.carInstance.CurrentTier;
		float num2 = (float)GameDatabase.Instance.OnlineConfiguration.PerformanceIncreasePercent * 0.01f;
		int num3;
		int num4;
		CarPerformanceIndexCalculator.GetPPRangeForCarTier(currentTier, out num3, out num4);
		int extraPPToGive = Mathf.RoundToInt((float)(num4 - num3) * num2);
		bool flag = false;
		int maxPPOfCarOwnedInThisTier = activeProfile.GetMaxPPOfCarOwnedInThisTier(currentTier, out flag);
		if (flag && maxPPOfCarOwnedInThisTier > num)
		{
			num = maxPPOfCarOwnedInThisTier;
		}
		if (SportsPackScreen.CarWonAlreadyOwned)
		{
			baseCarPP = SportsPackScreen.CarWonAlreadyOwnedPP;
			if (SportsPackScreen.CarWonAlreadyOwnedPP > num)
			{
				num = SportsPackScreen.CarWonAlreadyOwnedPP;
			}
		}
		int num5 = this.CalculateSportsPackUpgrade(num, extraPPToGive);
		this._sportsPackCost = this.CalculateSportsPackCost(this.wonCarInfo.BaseCarTier, num5, baseCarPP);
		this.LeftStatElement.Set(currentTier, this.wonCarInfo.BasePerformanceIndex);
		this.RightStatElement.Set(currentTier, num5);
		if (SportsPackScreen.CarWonAlreadyOwned)
		{
			Dictionary<eUpgradeType, CarUpgradeStatus> upgradeStatus = activeProfile.GetCarFromID(this.wonCarInfo.Key).UpgradeStatus;
			CarUpgradeSetup carUpgradeSetup = new CarUpgradeSetup();
			GameObject gameObject = new GameObject();
			CarPhysics carPhysics = gameObject.AddComponent<CarPhysics>();
			carPhysics.FrontendMode = true;
			carUpgradeSetup.UpgradeStatus = upgradeStatus;
			CarPhysicsSetupCreator carPhysicsSetupCreator = new CarPhysicsSetupCreator(this.wonCarInfo, carPhysics);
			carUpgradeSetup.IsFettled = false;
			carUpgradeSetup.CarDBKey = this.wonCarInfo.Key;
			carPhysicsSetupCreator.InitialiseCarPhysics(carUpgradeSetup);
			carPhysicsSetupCreator.SetStatsPriorToUpgrade();
			carPhysicsSetupCreator.ApplyCarUpgrades(carUpgradeSetup);
			carPhysicsSetupCreator.SetStatsAfterUpgrade();
			UpgradeScreenCarStats baseStats;
			baseStats.CurrentHP = Mathf.RoundToInt((float)carPhysicsSetupCreator.NewPeakHP);
			baseStats.CurrentGrip = Mathf.RoundToInt(carPhysicsSetupCreator.NewGrip);
			baseStats.CurrentGearShiftTime = Mathf.RoundToInt(carPhysicsSetupCreator.NewGearShiftTime * 1000f);
			baseStats.CurrentWeight = Mathf.RoundToInt(carPhysicsSetupCreator.NewWeight * 2.20462251f);
			baseStats.DeltaHP = 0;
			baseStats.DeltaGrip = 0;
			baseStats.DeltaGearShiftTime = 0;
			baseStats.DeltaWeight = 0;
			UnityEngine.Object.DestroyImmediate(gameObject);
			this.LeftStatElement.Set(currentTier, SportsPackScreen.CarWonAlreadyOwnedPP);
			this.RightStatElement.Set(currentTier, num5);
			CarStatsCalculator.Instance.OverrideStatsForSportsPackScreen(this.carInfoLeft, this.carInfoRight, baseStats);
			if (num5 < SportsPackScreen.CarWonAlreadyOwnedPP)
			{
				PlayerProfile activeProfile2 = PlayerProfileManager.Instance.ActiveProfile;
				this.sportspackCarUpgradeSetup = activeProfile2.GetUpgradeSetupForCar(activeProfile2.GetCarFromID(SportsPackScreen.CarToAward));
				this._sportsPackCost = GameDatabase.Instance.OnlineConfiguration.SportsPackTierBaseGoldCosts[(int)this.wonCarInfo.BaseCarTier];
				this.RightStatElement.Set(currentTier, SportsPackScreen.CarWonAlreadyOwnedPP);
			}
		}
	}

	private int CalculateSportsPackUpgrade(int basePerformanceIndex, int extraPPToGive)
	{
		int result = 0;
		int targetPP = basePerformanceIndex + extraPPToGive;
		this.sportspackCarUpgradeSetup = CarStatsCalculator.Instance.CalculateStatsForSportsPackCars(this.wonCarInfo, basePerformanceIndex, targetPP, this.carInfoLeft, this.carInfoRight, out result);
		return result;
	}

	private int CalculateSportsPackCost(eCarTier tier, int newPP, int baseCarPP)
	{
		int num;
		int num2;
		CarPerformanceIndexCalculator.GetPPRangeForCarTier(tier, out num, out num2);
		int num3 = GameDatabase.Instance.OnlineConfiguration.SportsPackTierBaseGoldCosts[(int)tier];
		float num4 = Mathf.Clamp01((float)(newPP - baseCarPP) / (float)(num2 - num));
		return num3 + Mathf.RoundToInt(num4 * (float)GameDatabase.Instance.OnlineConfiguration.SportsPackTierBaseGoldCosts[(int)tier]);
	}

	private bool ShowStargazerPopupIfAppropriate()
	{
		int num = 0;
		for (int i = 0; i < PlayerProfileManager.Instance.ActiveProfile.CarsOwned.Count; i++)
		{
			CarGarageInstance carGarageInstance = PlayerProfileManager.Instance.ActiveProfile.CarsOwned[i];
			if (carGarageInstance != null && carGarageInstance.EliteCar)
			{
				num++;
			}
		}
		if (num >= 3)
		{
			return false;
		}
		string title = "TEXT_STARGAZER_DEAL_TITLE_" + (num + 1).ToString();
		string body = "TEXT_STARGAZER_DEAL_BODY_" + (num + 1).ToString();
		PopUpDatabase.Common.ShowStargazerPopup(title, body, new PopUpButtonAction(this.OnNextButton), false);
		return true;
	}

	private void OnNextButton()
	{
		Log.AnEvent(Events.SportsPackNag);
		this.StartAnimation();
	}

	private void StartAnimation()
	{
		base.GetComponent<Animation>().Play();
	}

	private void GetEliteCarSnap()
	{
		AssetProviderClient.Instance.RequestAsset("PrizeTex_" + SportsPackScreen.CarToAward, new BundleLoadedDelegate(this.LoadedEliteAsset), this);
	}

	public void LoadedEliteAsset(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
	{
		Texture2D texture = zAssetBundle.mainAsset as Texture2D;
        //this.LeftCarSnapshot.SetTexture(texture);
        //this.LeftCarSnapshot.Setup(1.8f, 0.9f, new Vector2(0f, 255f), new Vector2(512f, 256f));
		this.LeftCarSnapshot.GetComponent<Renderer>().enabled = true;
		AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
	}

	private void GetSportsPackCarSnap()
	{
		AssetProviderClient.Instance.RequestAsset("SportsTex_" + SportsPackScreen.CarToAward, new BundleLoadedDelegate(this.LoadedSportsAsset), this);
	}

	public void LoadedSportsAsset(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
	{
		Texture2D texture = zAssetBundle.mainAsset as Texture2D;
        //this.RightCarSnapshot.SetTexture(texture);
        //this.RightCarSnapshot.Setup(1.8f, 0.9f, new Vector2(0f, 255f), new Vector2(512f, 256f));
		this.RightCarSnapshot.GetComponent<Renderer>().enabled = true;
		AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
	}

	protected override void Update()
	{
	}

	private void Anim_Initialize()
	{
		AnimationUtils.PlayFirstFrame(this.LeftPack.GetComponent<Animation>());
		AnimationUtils.PlayFirstFrame(this.RightPack.GetComponent<Animation>());
		AnimationUtils.PlayFirstFrame(this.DealSticker.GetComponent<Animation>());
		AnimationUtils.PlayFirstFrame(this.ORText.GetComponent<Animation>());
		this.DealSticker.SetActive(false);
		this.ORText.SetActive(false);
	}

	private void Anim_PlayLeft()
	{
		AnimationUtils.PlayAnim(this.LeftPack.GetComponent<Animation>());
		AudioManager.Instance.PlaySound("SportsPack", null);
	}

	private void Anim_PlayRight()
	{
		AnimationUtils.PlayAnim(this.RightPack.GetComponent<Animation>());
	}

	private void Anim_PlayDealSticker()
	{
		this.DealSticker.SetActive(true);
		AnimationUtils.PlayAnim(this.DealSticker.GetComponent<Animation>());
	}

	private void Anim_PlayOrText()
	{
		this.ORText.SetActive(true);
		AnimationUtils.PlayAnim(this.ORText.GetComponent<Animation>());
	}

	private void OnStandardButton()
	{
		string body = LocalizationManager.GetTranslation("TEXT_POPUPS_ACCEPT_STANDARD_ELITE_CAR");
		this.DisplayConfirmPopup(body, "TEXT_BUTTON_OK", new PopUpButtonAction(this.OnConfirmBuyStandard), null);
	}

	private void OnConfirmBuyStandard()
	{
		this.carInstance.EliteCar = true;
		this.carInstance.SportsUpgrade = false;
		PlayerProfileManager.Instance.ActiveProfile.GetCarFromID(SportsPackScreen.CarToAward).AppliedLiveryName = this.EliteLiveryName;
		this.SendMetricsEvent(this.wonCarInfo, false);
		if (MultiplayerUtils.InPrizeCarSequence)
		{
			MultiplayerUtils.InPrizeCarSequence = false;
			MultiplayerUtils.OfferStreakChainFrontend();
		}
		else
		{
			ScreenManager.Instance.PopToScreen(ScreenID.Workshop);
		}
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	private void OnEliteButton()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile.GetCurrentGold() < this._sportsPackCost)
		{
			MiniStoreController.Instance.ShowMiniStoreNotEnoughGold(new ItemTypeId("spug", this.EliteSportsLiveryName), new ItemCost
			{
				GoldCost = this._sportsPackCost
			}, "TEXT_POPUPS_INSUFFICIENT_GOLD_BODY_CAR", null, null, null);
			return;
		}
        string body = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_BUY_SPORTSPACK_ELITE_CAR"), CurrencyUtils.GetGoldStringWithIcon(this._sportsPackCost));
		this.DisplayConfirmPopup(body, "TEXT_BUTTON_BUY", new PopUpButtonAction(this.OnConfirmBuySports), null);
	}

	private void OnConfirmBuySports()
	{
		this.carInstance.EliteCar = true;
		this.carInstance.SportsUpgrade = true;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		activeProfile.GetCarFromID(SportsPackScreen.CarToAward).AppliedLiveryName = this.EliteSportsLiveryName;
		this.SendMetricsEvent(this.wonCarInfo, true);
		CarGarageInstance carFromID = activeProfile.GetCarFromID(SportsPackScreen.CarToAward);
		carFromID.UpgradeStatus = this.sportspackCarUpgradeSetup.UpgradeStatus;
		CarStatsCalculator.Instance.CalculateSportsPackScreenPerformanceIndex();
		if (MultiplayerUtils.InPrizeCarSequence)
		{
			MultiplayerUtils.InPrizeCarSequence = false;
			MultiplayerUtils.OfferStreakChainFrontend();
		}
		else
		{
			PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey = this.carInstance.CarDBKey;
			CarInfoUI.Instance.SetCurrentCarIDKey(this.carInstance.CarDBKey);
			ScreenManager.Instance.SwapScreen(ScreenID.LiveryCustomise);
		}

		activeProfile.SpendGold(this._sportsPackCost,"BuySports", "Livery");
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	protected void DisplayConfirmPopup(string body, string buyButtonTextID, PopUpButtonAction confirmed, PopUpButtonAction cancelled)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_ARE_YOU_SURE",
			BodyText = body,
			BodyAlreadyTranslated = true,
			CancelAction = cancelled,
			ConfirmAction = confirmed,
			CancelText = "TEXT_BUTTON_CANCEL",
			ConfirmText = buyButtonTextID
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private void SendMetricsEvent(CarInfo carInfo, bool isElite)
	{
		int num = (!isElite) ? 0 : this._sportsPackCost;
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.Type,
				(!isElite) ? "Standard pack" : "Sports pack"
			},
			{
				Parameters.CostGold,
				num.ToString()
			},
			{
				Parameters.DGld,
				(-num).ToString()
			},
			{
				Parameters.PieceID,
				PrizePieceGiveScreen.PiecesPaidFor.ToString()
			}
		};
		Log.AnEvent(Events.MultiplayerPurchase, data);
        string value = LocalizationManager.GetTranslation(carInfo.LongName);
		string value2 = (!isElite) ? "0" : "1";
		Dictionary<Parameters, string> data2 = new Dictionary<Parameters, string>
		{
			{
				Parameters.CarCompleted,
				value
			},
			{
				Parameters.PurchaseSportsPack,
				value2
			},
			{
				Parameters.PaidPuzzlePieces,
				PrizePieceGiveScreen.PiecesPaidFor.ToString()
			}
		};
		Log.AnEvent(Events.CarPuzzleCompleted, data2);
	}

	private void SetupLiveryInfo(string carKey)
	{
		string value = carKey + "_Livery";
		List<AssetDatabaseAsset> assetsOfType = AssetDatabaseClient.Instance.Data.GetAssetsOfType(GTAssetTypes.livery);
		foreach (AssetDatabaseAsset current in assetsOfType)
		{
			string code = current.code;
			if (code.StartsWith(value))
			{
				if (code.ToLower().Contains("elite"))
				{
					if (code.ToLower().Contains("sports"))
					{
						this.EliteSportsLiveryName = code;
					}
					else
					{
						this.EliteLiveryName = code;
					}
				}
			}
		}
		if (AsyncSwitching.IsLiveryName(this.EliteLiveryName))
		{
			AsyncSwitching.Instance.RequestAsset(this.EliteLiverySlot, this.EliteLiveryName, new BundleCallbackDelegate(this.EliteLiveryReady), base.gameObject, true, null);
		}
	}

	public void EliteLiveryReady(bool loadedOk, string liveryName)
	{
		GameObject gameObject = null;
		if (AsyncSwitching.IsLiveryName(liveryName))
		{
			gameObject = AsyncSwitching.Instance.GetLivery(this.EliteLiverySlot);
		}
		if (gameObject != null)
		{
			this.elite_visuals = gameObject.GetComponent<EliteVisuals>();
		}
		if (AsyncSwitching.IsLiveryName(this.EliteSportsLiveryName))
		{
			AsyncSwitching.Instance.RequestAsset(this.EliteSportsLiverySlot, this.EliteSportsLiveryName, new BundleCallbackDelegate(this.EliteSportsLiveryReady), base.gameObject, true, null);
		}
	}

	public void EliteSportsLiveryReady(bool loadedOk, string liveryName)
	{
		GameObject gameObject = null;
		if (AsyncSwitching.IsLiveryName(liveryName))
		{
			gameObject = AsyncSwitching.Instance.GetLivery(this.EliteSportsLiverySlot);
		}
		if (gameObject != null)
		{
			this.elite_sports_visuals = gameObject.GetComponent<EliteVisuals>();
		}
		Vector3 localPosition = new Vector3(0f, 0f, 0.06f);
		bool flag = true;
		int count = this.elite_sports_visuals.EliteColors.Count;
		for (int i = 0; i < count - 1; i++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(this.BlankSwatchPrefab) as GameObject;
			gameObject2.transform.parent = this.LeftSwatchStartNode.transform;
			gameObject2.transform.localPosition = localPosition;
			SportsSwatch component = gameObject2.GetComponent<SportsSwatch>();
			if (flag)
			{
				component.SetupSwatch(SportsSwatchType.TOP, this.elite_visuals.EliteColors[0]);
				flag = false;
				localPosition.y -= 0.11f;
			}
			else
			{
				component.SetupSwatch(SportsSwatchType.MIDDLE, this.elite_visuals.EliteColors[0]);
				localPosition.y -= 0.07f;
			}
			localPosition.z -= 0.01f;
		}
		GameObject gameObject3 = UnityEngine.Object.Instantiate(this.BlankSwatchPrefab) as GameObject;
		gameObject3.transform.parent = this.LeftSwatchStartNode.transform;
		gameObject3.transform.localPosition = localPosition;
		SportsSwatch component2 = gameObject3.GetComponent<SportsSwatch>();
		component2.SetupSwatch(SportsSwatchType.BOTTOM, this.elite_visuals.EliteColors[0]);
		Vector3 localPosition2 = new Vector3(0f, 0f, -0.01f);
		flag = true;
		for (int j = 0; j < count - 1; j++)
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate(this.SwatchPrefab) as GameObject;
			gameObject4.transform.parent = this.RightSwatchStartNode.transform;
			gameObject4.transform.localPosition = localPosition2;
			SportsSwatch component3 = gameObject4.GetComponent<SportsSwatch>();
			if (flag)
			{
				component3.SetupSwatch(SportsSwatchType.TOP, this.elite_sports_visuals.EliteColors[j]);
				flag = false;
				localPosition2.y -= 0.11f;
			}
			else
			{
				component3.SetupSwatch(SportsSwatchType.MIDDLE, this.elite_sports_visuals.EliteColors[j]);
				localPosition2.y -= 0.07f;
			}
		}
		GameObject gameObject5 = UnityEngine.Object.Instantiate(this.SwatchPrefab) as GameObject;
		gameObject5.transform.parent = this.RightSwatchStartNode.transform;
		gameObject5.transform.localPosition = localPosition2;
		SportsSwatch component4 = gameObject5.GetComponent<SportsSwatch>();
		component4.SetupSwatch(SportsSwatchType.BOTTOM, this.elite_sports_visuals.EliteColors[count - 1]);
	}

	private void StartCameraShake()
	{
        //if (this.cameraShake == null)
        //{
        //    this.cameraShake = GUICamera.Instance.gameObject.AddComponent<GUICameraShake>();
        //}
        //this.cameraShake.SetCurve(this.CurveControlShake);
        //this.cameraShake.ShakeTime = Time.time;
	}

	private void StopCameraShake()
	{
		if (this.cameraShake != null)
		{
			this.cameraShake.ShakeOver();
		}
	}

	public override bool HasBackButton()
	{
		return false;
	}
}
