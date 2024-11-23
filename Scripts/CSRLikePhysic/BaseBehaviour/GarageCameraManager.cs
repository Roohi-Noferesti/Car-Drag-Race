using System.Collections.Generic;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.UI;

public class GarageCameraManager : MonoBehaviour
{
	private enum GarageCameraState
	{
		UnZoomed,
		Zoomed,
		ZoomedCameras,
		FixedSeasonPrizeCamera,
		SwipeSeasonPrizeCamera
	}

	private enum FadeState
	{
		FadingToBlack,
		FadingToAlpha,
		Alpha,
		Black
	}

	private enum InputType
	{
		None,
		Pinch,
		Split,
		Tap,
		Touched
	}

	private const float FadeToBlackTime = 0.3f;

	private const float FadeToAlphaTime = 0.4f;

	private const float DisableButtonTime = 0.5f;

	private const float SwitchToAutoCamDelay = 7f;

	public Transform startCameraPose;

	public Transform seasonPrizeEndCameraPose;

	public float swipeCamSwitchDuration;

	public AnimationCurve swipeCamSwitchCurve;

	public float swipeCamFromSeasonPrizeDuration;

	public AnimationCurve swipeCamFromSeasonPrizeCurve;

	private GarageCameraState CurrentState;

	private GarageCameraState NextState;

	private FadeState CurrentFadeState = FadeState.Alpha;

	private float IdleTimer;

    private List<Button> ignoredButtons = new List<Button>();

	private string LastNewCarShown;

	private bool pinchStarted;

	private float pinchStartDistance;

	private float lastPinchTime;

	private bool tapStarted;

	private float tapTimer;

    private bool newCar;

	private Vector2 tapStartPos;

	private Vector2 tapLastPos;

	private float lastTapTime;

    public event OnUnZoom UnZoomEvent;


	public static GarageCameraManager Instance
	{
		get;
		private set;
	}

	public FadeQuad FadePane
	{
		get;
		private set;
	}

	public bool SuppressAutoCams
	{
		get;
		set;
	}

	public bool SuppressZoomView
	{
		get;
		set;
	}

	public bool SuppressAutoFade
	{
		get;
		set;
	}

	public bool IsCurrentlySwiping
	{
		get
		{
			return this.CurrentState != GarageCameraState.ZoomedCameras;
		}
	}

    public bool IsInCarPornMode
    {
        get
        {
            return this.CurrentState == GarageCameraState.ZoomedCameras ||
                   this.NextState == GarageCameraState.ZoomedCameras;
        }
    }

    public bool IsZoomedIn
	{
		get
		{
			return this.CurrentState != GarageCameraState.UnZoomed;
		}
	}

	public void IgnoreButtonRegion(Button button)
	{
		this.ignoredButtons.Add(button);
	}

	private bool IsNotInIgnoredButtonRect(Vector2 touchPos)
	{
		bool flag = true;
        List<Button> list = new List<Button>();
        foreach (Button current in this.ignoredButtons)
		{
			if (!(current == null))
			{
				list.Add(current);
				if (current.gameObject.activeInHierarchy && flag)
				{
                    //if (GUICamera.Instance.GetScreenSpaceRect(current.transform.position, current.width, current.height).Contains(touchPos))
                    //{
                    //    flag = false;
                    //}
				}
			}
		}
		this.ignoredButtons = list;
		return flag;
	}

	private void Awake()
	{
		Instance = this;
        EasyTouch.On_SimpleTap += EasyTouch_On_SimpleTap;
        EasyTouch.On_PinchEnd += EasyTouch_On_PinchEnd;
        EasyTouch.On_Swipe += EasyTouch_On_Swipe;
        EasyTouch.On_TouchStart+=EasyTouch_On_TouchStart;
        EasyTouch.On_TouchDown += EasyTouch_On_TouchDown;
	}

    void EasyTouch_On_TouchDown(Gesture gesture)
    {
        //if (!GarageManager.Instance.CarIsLoaded)
        //{
        //    return;
        //}
        if (PopUpManager.Instance.isShowingPopUp)
        {
            return;
        }

        if (LeftSidePanelContainer.Instance==null || LeftSidePanelContainer.Instance.IsLeftSidePanelOpen())
        {
            return;
        }
        //if (IsGesturevalid(gesture))
        //    UpdateStateBasedOnInput(InputType.Touched);
    }

    private bool IsGesturevalid(Gesture gesture)
    {
        return (gesture.pickedUIElement != null && gesture.pickedUIElement.CompareTag("UITap"));
    }

    void EasyTouch_On_TouchStart(Gesture gesture)
    {
        //if (!GarageManager.Instance.CarIsLoaded)
        //{
        //    return;
        //}
        if (PopUpManager.Instance.isShowingPopUp)
        {
            return;
        }
        if (IsGesturevalid(gesture))
            UpdateStateBasedOnInput(InputType.Touched);
        this.IdleTimer = 0f;
    }

    void EasyTouch_On_Swipe(Gesture gesture)
    {
        //if (!GarageManager.Instance.CarIsLoaded)
        //{
        //    return;
        //}
        if (PopUpManager.Instance.isShowingPopUp)
        {
            return;
        }

        if (LeftSidePanelContainer.Instance==null || LeftSidePanelContainer.Instance.IsLeftSidePanelOpen())
        {
            return;
        }
        if (IsGesturevalid(gesture))
        UpdateStateBasedOnInput(InputType.Touched);
    }

    void EasyTouch_On_PinchEnd(Gesture gesture)
    {
        //if (!GarageManager.Instance.CarIsLoaded)
        //{
        //    return;
        //}
        if (PopUpManager.Instance.isShowingPopUp)
        {
            return;
        }
        if (LeftSidePanelContainer.Instance == null || LeftSidePanelContainer.Instance.IsLeftSidePanelOpen())
        {
            return;
        }
        if (IsGesturevalid(gesture))
        UpdateStateBasedOnInput(InputType.Pinch);
    }

    public void EasyTouch_On_SimpleTap(Gesture gesture)
    {
        //if (!GarageManager.Instance.CarIsLoaded)
        //{
        //    return;
        //}
        if (PopUpManager.Instance.isShowingPopUp)
        {
            return;
        }

        if (BubbleManager.Instance.isShowingBubble && CurrentState != GarageCameraState.Zoomed)
        {
            return;
        }

        if (LeftSidePanelContainer.Instance==null || LeftSidePanelContainer.Instance.IsLeftSidePanelOpen())
        {
            return;
        }
        if (IsGesturevalid(gesture))
        UpdateStateBasedOnInput(InputType.Tap);
    }

    public void OnBackPressed()
    {
        //if (!GarageManager.Instance.CarIsLoaded)
        //{
        //    return;
        //}
        if (PopUpManager.Instance.isShowingPopUp)
        {
            return;
        }
        if (LeftSidePanelContainer.Instance == null || LeftSidePanelContainer.Instance.IsLeftSidePanelOpen())
        {
            return;
        }
        UpdateStateBasedOnInput(InputType.Tap);
    }

	public void OnEnable()
	{
		if (this.FadePane == null)
		{
			this.FadePane = FadeQuad.Instance;
			if (this.FadePane != null)
			{
				this.FadePane.SetColor(new Color(0f, 0f, 0f, 0f));
			}
		}
		this.UpdateCameraRect();
	}

	public void OnDisable()
	{
		this.IdleTimer = 0f;
		this.SetFadeState(FadeState.Alpha);
		if (this.FadePane != null)
		{
			this.FadePane.SetColor(new Color(0f, 0f, 0f, 0f));
		}
	}

	public void OnDestroy()
	{
	    ScreenManager.ScreenChanged -= this.ScreenChanged;
		this.SetFadeState(FadeState.Alpha);
		if (this.FadePane != null)
		{
			this.FadePane.SetColor(new Color(0f, 0f, 0f, 0f));
		}

        EasyTouch.On_SimpleTap -= EasyTouch_On_SimpleTap;
        EasyTouch.On_PinchEnd -= EasyTouch_On_PinchEnd;
        EasyTouch.On_Swipe -= EasyTouch_On_Swipe;
        EasyTouch.On_TouchDown += EasyTouch_On_TouchDown;
	}

	private void Start()
	{
        ScreenManager.ScreenChanged += this.ScreenChanged;
	}

	public void SetStateClear()
	{
		this.SetFadeState(FadeState.Alpha);
	}

	private void SetFadeState(FadeState newstate)
	{
		if (newstate == this.CurrentFadeState)
		{
			return;
		}
		if (this.FadePane == null)
		{
			return;
		}
		switch (newstate)
		{
		case FadeState.FadingToBlack:
			if (this.CurrentFadeState != FadeState.Black)
			{
				this.FadePane.FadeTo(Color.black, 0.3f);
			}
			break;
		case FadeState.FadingToAlpha:
			if (this.CurrentFadeState != FadeState.Alpha)
			{
				this.FadePane.FadeTo(new Color(0f, 0f, 0f, 0f), 0.4f);
			}
			break;
		case FadeState.Alpha:
			this.FadePane.SetColor(new Color(0f, 0f, 0f, 0f));
			break;
		case FadeState.Black:
			this.FadePane.SetColor(Color.black);
			break;
		}
		this.CurrentFadeState = newstate;
	}

	private void SetGarageCameraState(GarageCameraState newstate)
	{
		if (newstate == this.CurrentState)
		{
			return;
		}
		switch (newstate)
		{
		case GarageCameraState.UnZoomed:
			if (this.CurrentState == GarageCameraState.ZoomedCameras)
			{
				SequenceManager.Instance.PlaySequence("Swipe_Sequence", true);
			}
			break;
		case GarageCameraState.Zoomed:
			if (this.CurrentState == GarageCameraState.UnZoomed)
			{
				this.ActivateZoomed();
			}
			else if (this.CurrentState == GarageCameraState.ZoomedCameras || this.CurrentState == GarageCameraState.FixedSeasonPrizeCamera)
			{
				SequenceManager.Instance.TransitionToSequence("Swipe_Sequence", this.swipeCamSwitchDuration, this.swipeCamSwitchCurve);
			}
			break;
		case GarageCameraState.ZoomedCameras:
			SequenceManager.Instance.PlaySequence("Garage_Sequence", true);
			if (this.CurrentState == GarageCameraState.UnZoomed || this.CurrentState == GarageCameraState.FixedSeasonPrizeCamera || this.CurrentState == GarageCameraState.SwipeSeasonPrizeCamera)
			{
				this.ActivateZoomed();
			}
			break;
		case GarageCameraState.FixedSeasonPrizeCamera:
			SequenceManager.Instance.PlaySequence("SeasonPrize_Sequence", false);
			break;
		case GarageCameraState.SwipeSeasonPrizeCamera:
			SequenceManager.Instance.TransitionToSequence("SeasonPrize_Swipe_Sequence", 0f);
			break;
		}
		this.IdleTimer = 0f;
		this.CurrentState = newstate;
		this.NextState = newstate;
		if (this.CurrentState == GarageCameraState.UnZoomed && this.UnZoomEvent != null)
		{
			this.UnZoomEvent();
		}
	}

	public void ScreenChanged(ScreenID zNewScreenId)
	{
		this.IdleTimer = 0f;
        if (ScreenManager.Instance.NominalNextScreen()!=ScreenID.Invalid)
        {
            if (!newCar)
            {
                this.SwitchToUnZoomed();
            }
            newCar = false;
        }
	}

	public void SwitchToSwiping()
	{
		if (this.CurrentState != this.NextState)
		{
			return;
		}
		if (this.CurrentState != GarageCameraState.ZoomedCameras)
		{
			return;
		}
		this.MoveToState(GarageCameraState.Zoomed);
	}

	public void ResetCameraToStartPos()
	{
		if (this.CurrentState != GarageCameraState.FixedSeasonPrizeCamera && this.CurrentState != GarageCameraState.SwipeSeasonPrizeCamera)
		{
			Camera.main.transform.localPosition = this.startCameraPose.position;
			SequenceManager.Instance.PlaySequence("Swipe_Sequence", true);
			this.SetGarageCameraState(GarageCameraState.UnZoomed);
		}
	}

	private void Update()
	{
	    if (FadePane == null)
	    {
	        FadePane = FadeQuad.Instance;
	    }
        this.UpdateCameraRect();

        if (SceneManagerGarage.Instance==null || !SceneManagerGarage.Instance.CarIsLoaded)
        {
            return;
        }
        if (PopUpManager.Instance.isShowingPopUp)
		{
			return;
		}

        if (LeftSidePanelContainer.Instance==null || LeftSidePanelContainer.Instance.IsLeftSidePanelOpen())
        {
            return;
        }

        if (BubbleManager.Instance.isShowingBubble && CurrentState != GarageCameraState.Zoomed)
        {
            return;
        }

        var currentScreen = ScreenManager.Instance.CurrentScreen;
        if (currentScreen == ScreenID.Credits || this.CurrentFadeState != FadeState.Alpha
            || currentScreen == ScreenID.ChooseName
            || currentScreen == ScreenID.BodyHeight)
        {
            return;
        }
		if (this.CurrentState != GarageCameraState.ZoomedCameras && this.NextState != GarageCameraState.ZoomedCameras && this.CurrentState != GarageCameraState.FixedSeasonPrizeCamera && !this.SuppressAutoCams)
		{
			this.IdleTimer += Time.deltaTime;
			if (this.IdleTimer > 9)
			{
				this.MoveToState(GarageCameraState.ZoomedCameras);
				return;
			}
		}
		if (!this.SuppressAutoFade && this.FadePane.GetCurrentColor() == Color.black)
		{
			this.FadePane.FadeTo(new Color(0f, 0f, 0f, 0f), 0.4f);
			if (this.NextState != this.CurrentState)
			{
				this.SetGarageCameraState(this.NextState);
			}
		}
	}

    private void UpdateStateBasedOnInput(InputType inp)
	{
		if (inp != InputType.None)
		{
			this.IdleTimer = 0f;
			if (this.CurrentState == GarageCameraState.FixedSeasonPrizeCamera)
			{
				this.SetGarageCameraState(GarageCameraState.SwipeSeasonPrizeCamera);
			}
		}
		if (this.CurrentFadeState == FadeState.Alpha)
		{
			if (inp == InputType.Touched && this.CurrentState == GarageCameraState.ZoomedCameras && !this.SuppressZoomView)
			{
				this.MoveToState(GarageCameraState.Zoomed);
				return;
			}
			if (inp == InputType.Tap)
			{
				if (this.CurrentState == GarageCameraState.UnZoomed && !this.SuppressZoomView)
				{
					this.MoveToState(GarageCameraState.Zoomed);
				}
				else if (this.CurrentState == GarageCameraState.Zoomed || this.CurrentState == GarageCameraState.ZoomedCameras)
				{
					this.MoveToState(GarageCameraState.UnZoomed);
				}
				return;
			}
			if (inp == InputType.Pinch)
			{
				this.MoveToState(GarageCameraState.UnZoomed);
			}
			if (inp == InputType.Split)
			{
				this.MoveToState(GarageCameraState.Zoomed);
			}
		}
	}

	public void ResetIdleTimer()
	{
		this.IdleTimer = 0f;
	}

	private void MoveToState(GarageCameraState newstate)
	{
		if (newstate == this.NextState)
		{
			return;
		}
		if (this.CurrentState != this.NextState)
		{
			return;
		}
		bool flag = false;
		switch (newstate)
		{
		case GarageCameraState.UnZoomed:
			this.ActivateUnZoomed();
			flag = true;
			break;
		case GarageCameraState.Zoomed:
			if (this.CurrentState == GarageCameraState.ZoomedCameras || this.CurrentState == GarageCameraState.FixedSeasonPrizeCamera)
			{
				this.SetGarageCameraState(GarageCameraState.Zoomed);
			}
			else if (this.CurrentState == GarageCameraState.UnZoomed)
			{
				flag = true;
			}
			else if (this.CurrentState == GarageCameraState.SwipeSeasonPrizeCamera)
			{
				this.SetGarageCameraState(GarageCameraState.Zoomed);
			}
			break;
		case GarageCameraState.ZoomedCameras:
			this.ActivateZoomed();
			this.SetGarageCameraState(GarageCameraState.ZoomedCameras);
			break;
		case GarageCameraState.FixedSeasonPrizeCamera:
			flag = true;
			break;
		}
		if (!base.gameObject.activeInHierarchy || !base.enabled)
		{
			return;
		}
		if (!base.enabled)
		{
			try
			{
				base.enabled = true;
			}
			catch
			{
				return;
			}
		}
		this.NextState = newstate;
		if (flag)
		{
            this.FadePane.FadeTo(Color.black, 0.3f);
		}
	}

	private void ActivateZoomed()
	{
        //CarInfo car = CarDatabase.Instance.GetCar(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey);
        //CommonUI.Instance.NavBar.SetOverlayTitle(car.LongName);
        //CommonUI.Instance.NavBar.ShowOverlay(true);
        var activeScreen = (ZHUDScreen)ScreenManager.Instance.ActiveScreen;
        if (activeScreen != null)
        {
            activeScreen.StartAnimOut();
            BubbleManager.Instance.GarageIsZoomingIn();
        }
	}

	private void ActivateUnZoomed()
	{
        //CarInfo car = CarDatabase.Instance.GetCar(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey);
        //CommonUI.Instance.NavBar.SetOverlayTitle(car.MediumName);
        //CommonUI.Instance.NavBar.ShowOverlay(false);
        var activeScreen = (ZHUDScreen)ScreenManager.Instance.ActiveScreen;
        if (activeScreen != null)
        {
            activeScreen.StartAnimIn();
            BubbleManager.Instance.FadeAllIn();
            BubbleManager.Instance.GarageIsZoomingOut();
        }
	}

	public void SwitchToZoomed()
	{
		this.MoveToState(GarageCameraState.Zoomed);
	}

	public void SwitchToUnZoomed()
	{
		this.MoveToState(GarageCameraState.UnZoomed);
	}

	public void SwitchToZoomedCameras()
	{
		this.MoveToState(GarageCameraState.ZoomedCameras);
	}

	public void SwitchToSeasonPrizeCamera()
	{
		this.MoveToState(GarageCameraState.FixedSeasonPrizeCamera);
	}

	public void ForceUpdateCameraRect()
	{
		this.UpdateCameraRect();
	}

	private void UpdateCameraRect()
	{
        //float num = 0;//ScreenManager.Instance.GetHeightOfCurrentScreensUI() / GUICamera.Instance.ScreenHeight;
        //float num2 = 0f;
        //var isIn = !IsZoomedIn;//CommonUI.Instance.IsIn;
        //if (isIn && this.CurrentState != GarageCameraState.FixedSeasonPrizeCamera &&
        //    this.CurrentState != GarageCameraState.SwipeSeasonPrizeCamera)
        //{
        //    num2 = 0.1F;//(navBar.Background.height - 0.04f)/GUICamera.Instance.ScreenHeight;
        //}
        //var mainCamera = Camera.main;
        //if (mainCamera != null)
        //    mainCamera.rect = new Rect(0f, 0, 1f, 1f - (num + num2));
    }

    //private void OnTap(GenericTouch touch)
    //{
    //}

	public void TriggerNewCarCameraSequence(string carDBKey)
	{
		if (carDBKey != this.LastNewCarShown)
		{
		    newCar = true;
			this.MoveToState(GarageCameraState.ZoomedCameras);
			this.LastNewCarShown = carDBKey;
		}
	}
}
