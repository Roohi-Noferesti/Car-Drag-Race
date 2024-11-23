using UnityEngine;
using UnityEngine.UI;

public class InternationalVersusScreen : BaseVersusScreen
{
	public Text HeadStartTextP1;

    public Text TimeDifferenceTextP1;

    public Text HeadStartTextP2;

    public Text TimeDifferenceTextP2;

	public Image RelayRaceArrowLeft;

    public Image RelayRaceArrowRight;

	private RelayRaceDifficulty difficulty;

	public Text RelayRoundText;

	public static InternationalVersusScreen Instance
	{
		get;
		private set;
	}

	protected override void SwitchState(ScreenState state)
	{
		if (this.screenState == ScreenState.PreparingToLoad && state != ScreenState.Loading)
		{
			return;
		}
		if (this.screenState == ScreenState.Loading)
		{
			return;
		}
		switch (state)
		{
		case ScreenState.Initialise:
			base.IsReadyToLoad = false;
			this.startLoading = false;
            //if (!BaseVersusScreen.StartAsFinished)
            //{
            //    MultiplayerUtils.FakeAComsumableActive = false;
            //    MultiplayerUtils.FakeNoComsumablesActive = false;
            //}
            //this.LoadingText.Text = LocalizationManager.GetTranslation("TEXT_LOADING");
            //this.LoadingTips.maxWidth = GUICamera.Instance.ScreenWidth - 0.3f;
			base.showLoadingText(string.Empty);
			this.RaceButton.SetActive(false);
			base.DisplayHireMechanicButton(false);
			this.DisplayTracksideUpgradeButton(false);
			CarSnapshotManager.Instance.SnapshotSize = this.SnapshotSize;
			this.VSGraphic.Anim_Play();
			RaceEventInfo.Instance.RefreshRaceEvent();
			break;
		case ScreenState.SetupPlayer2Box:
			base.SetupCompetitorBox();
			break;
		case ScreenState.Animating:
			base.hideLoadingText();
			this.SetupDifficultyRating();
			base.StartAnimations();
			break;
		case ScreenState.PlayerInteraction:
			this.dummyScreen.UnblockTutorialBubbles(true);
			break;
		case ScreenState.PreparingToLoad:
			if (this.difficulty != null)
			{
				this.difficulty.gameObject.SetActive(false);
			}
			break;
		case ScreenState.Loading:
			SceneManagerFrontend.ButtonStart();
			base.StopCameraShake();
			base.showLoadingText(TipsManager.Instance.GetRandomSingleplayerTip());
			base.IsReadyToLoad = true;
			break;
		}
		this.screenState = state;
	}

	private Vector2 ConvertUIPosition(Vector2 pos)
	{
	    return pos;
	    //return new UnityEngine.Vector2(pos.x * GUICamera.Instance.ScreenWidth * 0.5f, pos.y * GUICamera.Instance.ScreenHeight * 0.5f);
	}

	private RelayRaceDifficulty AddDifficultyPrefab()
	{
		Object original = Resources.Load("Screens/RelayRace/RelayRaceDifficulty");
		GameObject gameObject = Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
		Vector2 pos = new Vector2(0f, -0.8f);
		Vector2 vector = this.ConvertUIPosition(pos);
		gameObject.transform.parent = this.VSGraphic.transform;
		gameObject.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
		return gameObject.GetComponent<RelayRaceDifficulty>();
	}

	private void SetVersusGraphicColors()
	{
        //ThemeOptionLayoutDetails themeOptionLayoutDetails = TierXManager.Instance.ThemeDescriptor.GetThemeOptionLayoutDetails();
        //if (themeOptionLayoutDetails == null)
        //{
        //    return;
        //}
        //GameObject[] vSGraphicMaterialsWithColours = this.VSGraphicMaterialsWithColours;
        //for (int i = 0; i < vSGraphicMaterialsWithColours.Length; i++)
        //{
        //    GameObject gameObject = vSGraphicMaterialsWithColours[i];
        //    gameObject.renderer.material.SetColor("_Tint", themeOptionLayoutDetails.Colour.AsUnityColor());
        //}
	}

	protected override void Awake()
	{
		if (Instance != null)
		{
		}
		Instance = this;
		this.SetVersusGraphicColors();
		base.Anim_Initialize();
        //this.UpgradeButton.GetComponent<DummyTextButton>().ForceAwake();
        //this.HireMechanicButton.GetComponent<DummyTextButton>().ForceAwake();
		base.showLoadingText(string.Empty);
		this.RaceButton.SetActive(false);
		base.DisplayHireMechanicButton(false);
		this.DisplayTracksideUpgradeButton(false);
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		int racesDone = RelayManager.GetRacesDone();
		float timeDifference = currentEvent.GetTimeDifference();
		string str = timeDifference.ToString("0.000;0.000;0");
		this.TimeDifferenceTextP1.text = str + "s";
		this.HeadStartTextP1.gameObject.SetActive(false);
		this.TimeDifferenceTextP1.gameObject.SetActive(false);
        this.TimeDifferenceTextP2.text = str + "s";
		this.HeadStartTextP2.gameObject.SetActive(false);
		this.TimeDifferenceTextP2.gameObject.SetActive(false);
		this.RelayRaceArrowLeft.gameObject.SetActive(false);
		this.RelayRaceArrowRight.gameObject.SetActive(false);
		this.DifficultySpriteT.gameObject.SetActive(false);
		if (currentEvent.IsRelay)
		{
			this.difficulty = this.AddDifficultyPrefab();
			this.difficulty.Setup(currentEvent, racesDone, 1f, true, false);
            //this.RelayRoundText.Text = string.Format(LocalizationManager.GetTranslation("TEXT_UI_ROUND_X"), racesDone + 1);
		}
		else
		{
			this.RelayRoundText.transform.parent.gameObject.SetActive(false);
		}
		if (currentEvent.IsRelay || currentEvent.AutoHeadstart)
		{
			if (timeDifference > 0f)
			{
				this.RelayRaceArrowLeft.gameObject.SetActive(true);
				this.TimeDifferenceTextP1.gameObject.SetActive(true);
				this.HeadStartTextP1.gameObject.SetActive(true);
                //this.HeadStartTextP1.Text = LocalizationManager.GetTranslation("TEXT_HEADSTART");
			}
			else if (timeDifference < 0f)
			{
				this.RelayRaceArrowRight.gameObject.SetActive(true);
				this.TimeDifferenceTextP2.gameObject.SetActive(true);
				this.HeadStartTextP2.gameObject.SetActive(true);
                //this.HeadStartTextP2.Text = LocalizationManager.GetTranslation("TEXT_HEADSTART");
			}
		}
		base.SetupLocalPlayerBox();
	}

	protected override void SetupButtonState()
	{
        //GoRaceButton.eState eState = GoRaceButton.eState.Normal;
        //UIButton.CONTROL_STATE controlState = UIButton.CONTROL_STATE.NORMAL;
		PlayerInfo playerInfo = CompetitorManager.Instance.LocalCompetitor.PlayerInfo;
		MechanicPlayerInfoComponent component = playerInfo.GetComponent<MechanicPlayerInfoComponent>();
		bool display = RaceEventInfo.Instance.CurrentEvent.IsMechanicAllowed() && !component.MechanicEnabled && RaceEventDifficulty.Instance.GetRating(RaceEventInfo.Instance.CurrentEvent, false) > RaceEventDifficulty.Rating.Easy;
		base.DisplayHireMechanicButton(display);
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		CarGarageInstance currentCar = activeProfile.GetCurrentCar();
		bool display2 = !RaceEventInfo.Instance.CurrentEvent.IsRelay && !RaceEventInfo.Instance.CurrentEvent.AutoHeadstart && !currentCar.GetIsFullyFitted() && !CarDatabase.Instance.GetCar(currentCar.CarDBKey).UsesEvoUpgrades() && RaceEventDifficulty.Instance.GetRating(RaceEventInfo.Instance.CurrentEvent, false) > RaceEventDifficulty.Rating.Easy;
		this.DisplayTracksideUpgradeButton(display2);
		this.RaceButton.SetActive(true);
        //this.RaceButtonControl.DefaultState = eState;
        //this.RaceButtonControl.SetState(eState);
        //this.RaceButtonControl.button.SetControlState(controlState);
		if (ShouldDisplayDifficultyDecreasedBubble)
		{
			base.DisplayDifficultyDecreasedBubble();
			ShouldDisplayDifficultyDecreasedBubble = false;
		}
	}

	protected override void SetupDifficultyRating()
	{
		if (RaceEventInfo.Instance.CurrentEvent.IsRelay)
		{
			this.DifficultySpriteT.gameObject.SetActive(false);
			return;
		}
		this.DifficultySpriteT.gameObject.SetActive(true);
		RaceEventDifficulty.Rating rating = RaceEventDifficulty.Instance.GetRating(RaceEventInfo.Instance.CurrentEvent, false);
		string @string = RaceEventDifficulty.Instance.GetString(rating);
		this.DifficultySpriteT.text = @string;
		string text = "Map_Screen/map_difficulty_";
		switch (rating)
		{
		case RaceEventDifficulty.Rating.Easy:
			text += "01_easy";
			break;
		case RaceEventDifficulty.Rating.Challenging:
			text += "02_challenging";
			break;
		case RaceEventDifficulty.Rating.Difficult:
			text += "03_hard";
			break;
		case RaceEventDifficulty.Rating.Extreme:
			text += "04_extreme";
			break;
		}
		Texture2D texture2D = (Texture2D)Resources.Load(text);
		if (texture2D == null)
		{
			return;
		}
		//float num = 200f;
        //this.DifficultyGraphic.renderer.material.SetTexture("_MainTex", texture2D);
        //this.DifficultyGraphic.Setup(232f / num, 24f / num, new UnityEngine.Vector2(0f, 23f), new UnityEngine.Vector2(232f, 24f));
		Vector3 zPosition = new Vector3(0f, 0f, 0.1f);
        //zPosition.y -= (float)this.DifficultySpriteT.GetDisplayLineCount() * this.DifficultySpriteT.BaseHeight;
        //zPosition.y -= this.DifficultyGraphic.height / 2f;
		zPosition.y += 0.02f;
        //this.DifficultyGraphic.gameObject.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zPosition);
	}

	protected override void DisplayTracksideUpgradeButton(bool display)
	{
        //this.UpgradeButton.GetComponent<DummyTextButton>().Show(display);
	}

	protected override void Update()
	{
		switch (this.screenState)
		{
		case ScreenState.Initialise:
			this.SwitchState(ScreenState.SetupPlayer1Box);
			break;
		case ScreenState.SetupPlayer1Box:
			if (this.Player1Box.HasBeenSetup)
			{
				this.SwitchState(ScreenState.SetupPlayer2Box);
			}
			break;
		case ScreenState.SetupPlayer2Box:
			if (this.Player2Box.HasBeenSetup)
			{
				this.SwitchState(ScreenState.Animating);
			}
			break;
		case ScreenState.PreparingToLoad:
            //if (!this.startLoading && !AnimationUtils.IsAnimationPlaying(base.animation, "VS_Screen_GoRace"))
            //{
            //    this.startLoading = true;
            //}
            //if (this.startLoading)
            //{
            //    this.SwitchState(BaseVersusScreen.ScreenState.Loading);
            //}
			break;
		}
		if (base.isLoadingTextShowing())
		{
			base.UpdatePips();
		}
	}

	protected override bool GetTuningScreenExpressMode()
	{
		return false;
	}

	public override void OnRaceButton()
	{
		if (RaceEventDifficulty.Instance.GetRating(RaceEventInfo.Instance.CurrentEvent, false) == RaceEventDifficulty.Rating.Extreme && !RaceEventInfo.Instance.CurrentEvent.IsRelay)
		{
			BubbleManager.Instance.DismissMessages();
			PopUp popup = new PopUp
			{
				Title = "TEXT_POPUPS_EXTREME_DIFFICULTY_TITLE",
				BodyText = "TEXT_POPUPS_EXTREME_DIFFICULTY_BODY",
				ConfirmText = "TEXT_BUTTON_UPGRADE",
				CancelText = "TEXT_BUTTON_BACK",
				ConfirmAction = new PopUpButtonAction(base.OnUpgrade),
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
				ImageCaption = "TEXT_NAME_AGENT"
			};
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
			return;
		}
		if (RaceEventInfo.Instance.CurrentEvent.IsRelay)
		{
            //CleanDownManager.Instance.OnRaceInternationalVersus();
		}
        //ScreenManager.Instance.PushScreen(ScreenID.Dummy);
		base.OnRaceButton();
	}

	protected override void Anim_PlayerBoxes()
	{
        //AnimationUtils.PlayAnim(this.Player1Box.animation, "VS_Screen_LeftPlayer_In_HeadStart");
        //AnimationUtils.PlayAnim(this.Player2Box.animation, "VS_Screen_RightPlayer_In_HeadStart");
	}
}
