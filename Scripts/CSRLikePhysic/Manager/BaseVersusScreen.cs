using Fabric;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseVersusScreen : MonoBehaviour
{
	public enum ScreenState
	{
		Initialise,
		SetupPlayer1Box,
		SetupPlayer2Box,
		Animating,
		PlayerInteraction,
		PreparingToLoad,
		Loading
	}

	protected ScreenState screenState;

	public Text LoadingText;

    public Text LoadingPips;

    public Text LoadingTips;

	public PlayerBox Player1Box;

	public PlayerBox Player2Box;

	public GameObject CentreNode;

	public VersusGraphic VSGraphic;

	public Animation EQ;

	public Button RaceButtonControl;

	public GameObject UpgradeButton;

	public GameObject HireMechanicButton;

	public GameObject RaceButton;

    //public FullScreenFlash ScreenFlash;

	public AnimationCurve CameraShakeCurve;

	public Text DifficultySpriteT;

	public Image DifficultyGraphic;

    public Image DifficultyShadowGraphic;

	protected GUICameraShake cameraShake;

	public Vector3 TutorialOpponentHasCrewBubblePosition;

	public Vector3 TutorialDifficultyDecreasedBubblePosition;

	protected readonly string PipAdd = "•";

	protected readonly float TimeBetweenPips = 0.3f;

	protected readonly float PipResetTime = 0.6f;

	protected float CurrentPipTime;

	protected readonly int MaxPipLength = 5;

	protected int CurrentPipLength;

	public int SnapshotSize = 512;

	public static bool StartAsFinished;

	public static bool ShouldDisplayDifficultyDecreasedBubble;

	protected bool shouldShowTracksideBubble;

	protected bool startLoading;

	public GameObject[] VSGraphicMaterialsWithColours;

	protected VSDummy dummyScreen;

	public bool IsReadyToLoad
	{
		get;
		protected set;
	}

	public void SetDummyScreen(VSDummy dummyScreen)
	{
		this.dummyScreen = dummyScreen;
	}

	protected virtual void SwitchState(ScreenState state)
	{
	}

	private CarGarageInstance GetOpponentCarFromPlayerInfo(PlayerInfo info)
	{
		RacePlayerInfoComponent component = info.GetComponent<RacePlayerInfoComponent>();
		CarGarageInstance carGarageInstance = new CarGarageInstance();
		carGarageInstance.CarDBKey = component.CarDBKey;
		carGarageInstance.NumberPlate = new NumberPlate();
		CarInfo car = CarDatabase.Instance.GetCar(component.CarDBKey);
		carGarageInstance.AppliedLiveryName = car.DefaultLiveryBundleName;
		carGarageInstance.NumberPlate.Text = info.Persona.GetNumberPlate();
		return carGarageInstance;
	}

	protected virtual void Awake()
	{
	}

	private void Start()
	{
		this.SwitchState(ScreenState.Initialise);
	}

	protected void showLoadingText(string text)
	{
		Vector3 position = LoadingScreenManager.Instance.LoadingScreenBackground.transform.position;
		position.z = 0f;
		LoadingScreenManager.Instance.LoadingScreenBackground.transform.position = position;
		this.LoadingText.transform.parent.gameObject.SetActive(true);
		this.LoadingTips.transform.parent.gameObject.SetActive(true);
        //this.LoadingTips.Text = text;
	}

	protected void hideLoadingText()
	{
		Vector3 position = LoadingScreenManager.Instance.LoadingScreenBackground.transform.position;
		position.z = 0.2f;
		LoadingScreenManager.Instance.LoadingScreenBackground.transform.position = position;
		this.LoadingText.transform.parent.gameObject.SetActive(false);
		this.LoadingTips.transform.parent.gameObject.SetActive(false);
	}

	protected bool isLoadingTextShowing()
	{
		return this.LoadingText.transform.parent.gameObject.activeInHierarchy;
	}

	protected abstract void SetupButtonState();

	protected abstract void SetupDifficultyRating();

	protected void DisplayHireMechanicButton(bool display)
	{
        //this.HireMechanicButton.GetComponent<DummyTextButton>().Show(display);
	}

	protected virtual void DisplayTracksideUpgradeButton(bool display)
	{
        //this.UpgradeButton.GetComponent<DummyTextButton>().Show(display);
	}

	protected void SetupLocalPlayerBox()
	{
		PlayerInfo playerInfo = CompetitorManager.Instance.LocalCompetitor.PlayerInfo;
		CarGarageInstance playerCar;
		if (RaceEventInfo.Instance.IsFirstOfSwitchBackRace())
		{
			playerCar = RaceEventInfo.Instance.CurrentEvent.LoanCarGarageInstance;
		}
		else
		{
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			playerCar = activeProfile.GetCurrentCar();
		}
		CarSnapshotManager.Instance.CarSlot = AsyncBundleSlotDescription.HumanCar;
		CarSnapshotManager.Instance.LiverySlot = AsyncBundleSlotDescription.HumanCarLivery;
		CarSnapshotManager.Instance.SnapshotType = CarSnapshotType.VSScreenLeft;
		Texture2D texture2D = CarSnapshotManager.Instance.LoadSnapshotFromCache(playerCar);
		if (texture2D != null)
		{
			this.Player1Box.SetBoxProfile(playerInfo, playerCar, texture2D, base.GetType(), true);
		}
		else
		{
			CarSnapshotManager.Instance.GenerateSnapshot(playerCar, delegate(Texture2D snapshotTexture)
			{
				this.Player1Box.SetBoxProfile(playerInfo, playerCar, snapshotTexture, this.GetType(), true);
			});
		}
	}

	protected void SetupCompetitorBox()
	{
		PlayerInfo playerInfo = CompetitorManager.Instance.OtherCompetitor.PlayerInfo;
		CarGarageInstance playerCar = this.GetOpponentCarFromPlayerInfo(playerInfo);
		RacePlayerInfoComponent component = playerInfo.GetComponent<RacePlayerInfoComponent>();
        //playerCar.AppliedColourIndex = component.AppliedColourIndex;
		if (!string.IsNullOrEmpty(component.AppliedLivery))
		{
			playerCar.AppliedLiveryName = component.AppliedLivery;
		}
		playerCar.EliteCar = component.IsEliteCar;
		playerCar.SportsUpgrade = component.HasSportsUpgrade;
		if (RaceEventInfo.Instance.CurrentEvent.UseCustomShader)
		{
            //playerCar.ColorOverride = RaceEventInfo.Instance.CurrentEvent.GetAIColour();
			playerCar.UseColorOverride = true;
		}
		if (CompetitorManager.Instance.OtherCompetitor == null)
		{
		}
		if (CarSnapshotManager.Instance == null)
		{
		}
		CarSnapshotManager.Instance.CarSlot = AsyncBundleSlotDescription.AICar;
		CarSnapshotManager.Instance.LiverySlot = AsyncBundleSlotDescription.AICarLivery;
		CarSnapshotManager.Instance.SnapshotType = CarSnapshotType.VSScreenRight;
		Texture2D texture2D = CarSnapshotManager.Instance.LoadSnapshotFromCache(playerCar);
		if (texture2D != null)
		{
			this.Player2Box.SetBoxProfile(playerInfo, playerCar, texture2D, base.GetType(), false);
		}
		else
		{
			CarSnapshotManager.Instance.GenerateSnapshot(playerCar, delegate(Texture2D snapshotTexture)
			{
				this.Player2Box.SetBoxProfile(playerInfo, playerCar, snapshotTexture, this.GetType(), false);
			});
		}
	}

	protected void StartAnimations()
	{
		if (StartAsFinished)
		{
			this.SetupButtonState();
			this.SetToFinished();
		}
		else
		{
            //AnimationUtils.PlayAnim(base.animation);
		}
		this.Anim_PlayEQIn();
        //AsyncSwitching.Instance.ClearSlot(AsyncBundleSlotDescription.HumanCar);
        //AsyncSwitching.Instance.ClearSlot(AsyncBundleSlotDescription.HumanCarLivery);
        //AsyncSwitching.Instance.ClearSlot(AsyncBundleSlotDescription.AICar);
        //AsyncSwitching.Instance.ClearSlot(AsyncBundleSlotDescription.AICarLivery);
		Resources.UnloadUnusedAssets();
		CarSnapshotManager.Instance.ResetToDefaults();
	}

	protected abstract void Update();

	public virtual void OnRaceButton()
	{
		if (this.screenState == ScreenState.Initialise)
		{
		}
        //if (!TouchManager.AttemptToUseButton("VSRaceButton"))
        //{
        //    return;
        //}
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (!currentEvent.IsRandomRelay() && !currentEvent.AutoHeadstart)
		{
			int fuelCostForEvent = GameDatabase.Instance.Currencies.GetFuelCostForEvent(RaceEventInfo.Instance.CurrentEvent);
			FuelManager.Instance.SpendFuel(fuelCostForEvent);
		}
		if (this.screenState == ScreenState.Animating)
		{
			this.SetToFinished();
		}
		this.RaceButton.SetActive(false);
		this.DisplayHireMechanicButton(false);
		this.DisplayTracksideUpgradeButton(false);
		this.DifficultySpriteT.gameObject.SetActive(false);
        //MenuAudio.Instance.playSound(MenuBleeps.MenuClickForward);
		EventManager.Instance.PostEvent("VersusGoRace", EventAction.PlaySound, null, null);
		this.KillAllBubbles();
        //AnimationUtils.PlayAnim(base.animation, "VS_Screen_GoRace");
		this.SwitchState(ScreenState.PreparingToLoad);
        //ScreenManager.Instance.PushScreen(ScreenID.Dummy);
	}

	private void OnHireMechanic()
	{
		LoadingScreenManager.Instance.SnapCloseVSScreen();
        //MenuAudio.Instance.playSound(MenuBleeps.MenuClickForward);
        //ScreenManager.Instance.PushScreen(ScreenID.Mechanic);
        //ScreenManager.Instance.UpdateImmediately();
	}

	protected void OnUpgrade()
	{
		if (this.screenState == ScreenState.Initialise)
		{
			return;
		}
		this.KillAllBubbles();
		LoadingScreenManager.Instance.SnapCloseVSScreen();
        //TuningScreen.ExpressMode = this.GetTuningScreenExpressMode();
        //MenuAudio.Instance.playSound(MenuBleeps.MenuClickForward);
        //ScreenManager.Instance.PushScreen(ScreenID.Tuning);
        //ScreenManager.Instance.UpdateImmediately();
	}

	protected abstract bool GetTuningScreenExpressMode();

	protected void OnUpsellPopupDismissed()
	{
		if (this.shouldShowTracksideBubble)
		{
			this.DisplayTracksideUpgradesBubble();
		}
	}

	protected void DisplayTracksideUpgradesBubble()
	{
		Vector3 b = new Vector3(0.25f, -0.15f, -0.1f);
		BubbleManager.Instance.ShowMessage("TEXT_TUTORIAL_MESSAGE_VERSUS_SCREEN_TAP_UPGRADES", false, this.UpgradeButton.gameObject.transform.position + b, BubbleMessage.NippleDir.UP, 0.25f);
	}

	protected void DisplayDifficultyDecreasedBubble()
	{
		BubbleManager.Instance.ShowMessage("TEXT_TUTORIAL_MESSAGE_VERSUS_SCREEN_DIFFICULTY_DECREASED", false, this.TutorialDifficultyDecreasedBubblePosition, BubbleMessage.NippleDir.DOWN, 0.5f);
	}

	protected void KillAllBubbles()
	{
		BubbleManager.Instance.KillAllMessages();
	}

	private void SetToFinished()
	{
        //AnimationUtils.PlayLastFrame(base.animation);
        //AnimationUtils.PlayLastFrame(this.Player1Box.animation);
        //AnimationUtils.PlayLastFrame(this.Player2Box.animation);
		this.Anim_StartLoading();
	}

	protected void UpdatePips()
	{
		this.CurrentPipTime += Time.deltaTime;
		if (this.CurrentPipLength >= this.MaxPipLength)
		{
			if (this.CurrentPipTime >= this.PipResetTime)
			{
                //this.LoadingPips.Text = string.Empty;
				this.CurrentPipLength = 0;
				this.CurrentPipTime = 0f;
			}
			return;
		}
		if (this.CurrentPipTime >= this.TimeBetweenPips)
		{
			this.CurrentPipTime = 0f;
            //Text expr_79 = this.LoadingPips;
            //expr_79.Text += this.PipAdd;
			this.CurrentPipLength++;
		}
	}

	protected void Anim_Initialize()
	{
        //AnimationUtils.PlayFirstFrame(base.animation);
        //AnimationUtils.PlayFirstFrame(this.Player1Box.animation);
        //AnimationUtils.PlayFirstFrame(this.Player2Box.animation);
        //this.VSGraphic.Anim_Initialise();
        //AnimationUtils.PlayFirstFrame(this.EQ, "EQFadeIn");
	}

	protected abstract void Anim_PlayerBoxes();

	private void Anim_PlayVersusAudio()
	{
		EventManager.Instance.PostEvent("Versus", EventAction.PlaySound, null, null);
	}

	private void Anim_PlayVSGraphic()
	{
		this.VSGraphic.Anim_Play();
	}

	private void Anim_StartLoading()
	{
		this.SwitchState(ScreenState.PlayerInteraction);
	}

	private void Anim_FullscreenFlash()
	{
        //this.ScreenFlash.StartFlashAnimation();
	}

	private void Anim_DisplayButtons()
	{
		this.SetupButtonState();
	}

	private void Anim_DoCameraShake()
	{
		if (this.cameraShake == null)
		{
            //this.cameraShake = GUICamera.Instance.gameObject.AddComponent<GUICameraShake>();
		}
		this.cameraShake.SetCurve(this.CameraShakeCurve);
		this.cameraShake.ShakeTime = Time.time;
	}

	private void Anim_PlayEQIn()
	{
        //AnimationUtils.PlayAnim(this.EQ, "EQFadeIn");
	}

	private void Anim_PlayEQOut()
	{
        //AnimationUtils.PlayAnim(this.EQ, "EQFadeOut");
	}

	private void Anim_GoRace()
	{
		this.startLoading = true;
	}

	protected void StopCameraShake()
	{
		this.cameraShake.ShakeOver();
	}
}
