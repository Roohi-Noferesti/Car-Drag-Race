//using AnimationOrTween;
using Objectives;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using KingKodeStudio;
//using Tutorial;
//using Tutorial.Objectives.Commands;
using UnityEngine;

public class LeftSidePanelContainer : PlayerCrewUISubScreen
{
    //public UIButton BlockerBackground;

    //public UIDragObject DragObject_Tab;

    //public TweenPosition[] PanelEnableDisableTween;

	public float PanelEnableDisableTween_SlowDuration;

	public float PanelEnableDisableTween_FastDuration;

	public bool PanelEnableDisableTween_currentAnimationState;

    //public UIToggle PanelShowHideToggle;

    //public TweenPosition[] PanelShowHideTween;

    //public UIToggle Toggle_Tab0;

    //public UIToggle Toggle_Tab1;

    //public UIToggle Toggle_Tab2;

    //public UIToggle Toggle_Tab3;

	public GameObject MainContent;

	//////private bool FacebookDirty;

	private bool GCStatusDirty;

	private bool TabMetricsDirty;

	private bool ReportMetrics;

	private bool applicationForegroundDirty;

	private bool applicationBackgroundDirty;

	private List<PlayerCrewUISubScreen> SubScreens = new List<PlayerCrewUISubScreen>();

	public static LeftSidePanelContainer Instance;

	public bool IsPanelLockedByTutorial;

	private Vector3 originalPanelFrom;

	private Vector3 originalPanelTo;

    public static event Action<bool> OnLeftSidePanelToggled;

    public static event Action<bool> OnLeftSidePanelFinishedAnimatingOnOrOffScreen;

	private void Awake()
	{
		if (LeftSidePanelContainer.Instance != null)
		{
			return;
		}
        //Camera componentInChildren = base.gameObject.transform.GetComponentInChildren<Camera>();
        //componentInChildren.enabled = false;
		LeftSidePanelContainer.Instance = this;
        //this.DragObject_Tab.OnDragObjectPress += new UIDragObject.DragObjectPressDelegate(this.OnDragObjectPress);
        //GameCenterManager.playerLoggedOut += new GameCenterEventHandler(this.playerLoggedOut);
        //this.SetupFacebookListeners();
        //iOSEvents.ApplicationDidEnterBackgroundEvent += new iOSEvents_Delegate(this.applicationDidEnterBackground);
        //iOSEvents.ApplicationWillEnterForegroundEvent += new iOSEvents_Delegate(this.applicationWillEnterForeground);
        //EventDelegate.Add(this.Toggle_Tab0.onChange, new EventDelegate.Callback(this.OnTabChanged));
        //EventDelegate.Add(this.Toggle_Tab1.onChange, new EventDelegate.Callback(this.OnTabChanged));
        //EventDelegate.Add(this.Toggle_Tab2.onChange, new EventDelegate.Callback(this.OnTabChanged));
        //EventDelegate.Add(this.Toggle_Tab3.onChange, new EventDelegate.Callback(this.OnTabChanged));
	}

	private void OnDestroy()
	{
        //GameCenterManager.playerLoggedOut -= new GameCenterEventHandler(this.playerLoggedOut);
        //this.RemoveFacebookListeners();
        //iOSEvents.ApplicationDidEnterBackgroundEvent -= new iOSEvents_Delegate(this.applicationDidEnterBackground);
        //iOSEvents.ApplicationWillEnterForegroundEvent -= new iOSEvents_Delegate(this.applicationWillEnterForeground);
        //LeftSidePanelOptionsScreen.OnGameCenterAuthenticationCallBack = (Action<bool, string>)Delegate.Remove(LeftSidePanelOptionsScreen.OnGameCenterAuthenticationCallBack, new Action<bool, string>(this.OnGameCenterAuthenticated));
        //EventDelegate.Remove(this.Toggle_Tab0.onChange, new EventDelegate.Callback(this.OnTabChanged));
        //EventDelegate.Remove(this.Toggle_Tab1.onChange, new EventDelegate.Callback(this.OnTabChanged));
        //EventDelegate.Remove(this.Toggle_Tab2.onChange, new EventDelegate.Callback(this.OnTabChanged));
        //EventDelegate.Remove(this.Toggle_Tab3.onChange, new EventDelegate.Callback(this.OnTabChanged));
		LeftSidePanelContainer.Instance = null;
		LeftSidePanelContainer.OnLeftSidePanelToggled = null;
		LeftSidePanelContainer.OnLeftSidePanelFinishedAnimatingOnOrOffScreen = null;
	}

	private void Start()
	{
		this.ReportMetrics = false;
        //TweenPosition tweenPosition = this.PanelShowHideTween[0];
        //if (tweenPosition)
        //{
        //    this.originalPanelFrom = tweenPosition.from;
        //    this.originalPanelTo = tweenPosition.to;
        //}
        //EventDelegate.Add(this.BlockerBackground.onClick, new EventDelegate.Callback(this.HidePanel));
        //EventDelegate.Add(this.PanelShowHideToggle.onChange, new EventDelegate.Callback(this.OnShowHidePanel));
        //this.PanelShowHideToggle.value = false;
		this.PanelEnableDisableTween_currentAnimationState = false;
        //for (int i = 0; i < this.PanelEnableDisableTween.Length; i++)
        //{
        //    float duration = this.PanelEnableDisableTween[i].duration;
        //    this.PanelEnableDisableTween[i].duration = 0f;
        //    this.PanelEnableDisableTween[i].PlayForward();
        //    this.PanelEnableDisableTween[i].duration = duration;
        //    this.PanelEnableDisableTween[i].enabled = true;
        //}
        //for (int j = 0; j < this.PanelShowHideTween.Length; j++)
        //{
        //    if (j == 0)
        //    {
        //        EventDelegate.Add(this.PanelShowHideTween[j].onFinished, new EventDelegate.Callback(this.OnFinishedPanelShowHideTween));
        //    }
        //    float duration2 = this.PanelShowHideTween[j].duration;
        //    this.PanelShowHideTween[j].duration = 0f;
        //    this.PanelShowHideTween[j].PlayForward();
        //    this.PanelShowHideTween[j].duration = duration2;
        //    this.PanelShowHideTween[j].enabled = true;
        //}
		base.Invoke("UnAnchorPanelShowHideTween", 1f);

        //Add by Mojtaba
        HideLeftSidePanel();
	}

	private void UnAnchorPanelShowHideTween()
	{
        //TweenPosition[] panelShowHideTween = this.PanelShowHideTween;
        //for (int i = 0; i < panelShowHideTween.Length; i++)
        //{
        //    TweenPosition tweenPosition = panelShowHideTween[i];
        //    UIWidget component = tweenPosition.gameObject.GetComponent<UIWidget>();
        //    if (component != null)
        //    {
        //        component.SetAnchor(null);
        //    }
        //}
        //this.PanelShowHideToggle.value = false;
	}

	private void Update()
	{
  //      ZHUDScreen cSRScreen;
		//bool flag;
        //if (ScreenManager.Active != null)
        //{
        //    cSRScreen = (ScreenManager.Active.ActiveScreen as ZHUDScreen);
        //    flag = (cSRScreen != null && cSRScreen.EnableChatPanel && cSRScreen.RequestChatPanelEnabled && !CarDetailsPanel.IsCurrentlyViewingPanel() && !RPBoostDropdownController.Instance.GetIsOrWillBeOpen() && !this.IsPanelLockedByTutorial);
        //}
        //else
        //{
        //    cSRScreen = null;
        //    flag = true;
        //    this.PanelShowHideToggle.value = true;
        //}
        //if (this.PanelEnableDisableTween_currentAnimationState != flag)
        //{
        //    this.PanelEnableDisableTween_currentAnimationState = flag;
        //    bool flag2 = cSRScreen == null || !cSRScreen.RequestChatPanelEnabled || Singleton<PopUpManager>.Instance.isShowingPopUp;
        //    float duration = (!flag2) ? this.PanelEnableDisableTween_FastDuration : this.PanelEnableDisableTween_SlowDuration;
        //    this.HidePanel();
        //    TweenPosition[] panelEnableDisableTween = this.PanelEnableDisableTween;
        //    for (int i = 0; i < panelEnableDisableTween.Length; i++)
        //    {
        //        TweenPosition tweenPosition = panelEnableDisableTween[i];
        //        tweenPosition.duration = duration;
        //        if (this.PanelEnableDisableTween_currentAnimationState)
        //        {
        //            tweenPosition.PlayReverse();
        //        }
        //        else
        //        {
        //            tweenPosition.PlayForward();
        //        }
        //        tweenPosition.gameObject.SetActive(true);
        //    }
        //}
        //if (!flag && this.PanelShowHideToggle.value)
        //{
        //    this.PanelShowHideToggle.value = false;
        //}
        //if (this.FacebookDirty)
        //{
        //    foreach (PlayerCrewUISubScreen current in this.SubScreens)
        //    {
        //        current.OnFacebookStateChanged();
        //    }
        //    this.FacebookDirty = false;
        //}
		if (this.GCStatusDirty)
		{
			foreach (PlayerCrewUISubScreen current2 in this.SubScreens)
			{
				current2.GCPlayerStatusChanged();
			}
			this.GCStatusDirty = false;
		}
		if (this.TabMetricsDirty)
		{
			this.LogActivePanelMetrics("view");
			this.TabMetricsDirty = false;
		}
		if (this.applicationBackgroundDirty)
		{
			foreach (PlayerCrewUISubScreen current3 in this.SubScreens)
			{
				current3.OnApplicationDidBackground();
			}
			this.applicationBackgroundDirty = false;
		}
		if (this.applicationForegroundDirty)
		{
			foreach (PlayerCrewUISubScreen current4 in this.SubScreens)
			{
				current4.OnApplicationWillForeground();
			}
			this.applicationForegroundDirty = false;
		}
	}

	public void OpenCrewsScreenAfterDelay()
	{
		base.Invoke("OpenCrewScreen", 1f);
	}

	private void OpenCrewScreen()
	{
        //PlayerCrewManager.Instance.OpenCrewScreen(false, false, false);
	}

	private void HidePanel()
	{
        //this.PanelShowHideToggle.value = false;

        //Add by mojtaba
        this.MainContent.SetActive(false);
	}

	private void ShowPanel()
	{
        //this.PanelShowHideToggle.value = true;
		this.MainContent.SetActive(true);
	}

	private void OnFinishedPanelShowHideTween()
	{
        //if (LeftSidePanelContainer.OnLeftSidePanelFinishedAnimatingOnOrOffScreen != null)
        //{
        //    LeftSidePanelContainer.OnLeftSidePanelFinishedAnimatingOnOrOffScreen(this.PanelShowHideToggle.value);
        //}
        //if (!this.PanelShowHideToggle.value)
        //{
        //    this.MainContent.SetActive(false);
        //}
        //this.ReportMetrics = this.PanelShowHideToggle.value;
        //TweenPosition[] panelShowHideTween = this.PanelShowHideTween;
        //for (int i = 0; i < panelShowHideTween.Length; i++)
        //{
        //    TweenPosition tweenPosition = panelShowHideTween[i];
        //    if (tweenPosition.direction == Direction.Forward)
        //    {
        //        tweenPosition.value = tweenPosition.to;
        //    }
        //    else if (tweenPosition.direction == Direction.Reverse)
        //    {
        //        tweenPosition.value = tweenPosition.from;
        //    }
        //}
	}

	private void OnDragObjectPress(bool pressed)
	{
        //if (!pressed)
        //{
        //    TweenPosition exists = this.PanelShowHideTween[0];
        //    if (exists)
        //    {
        //        this.PanelShowHideToggle.value = !this.PanelShowHideToggle.value;
        //    }
        //}
        //if (pressed || this.PanelShowHideToggle.value)
        //{
        //    this.MainContent.SetActive(true);
        //}
	}

    private void OnShowHidePanel()
	{
        //this.LogActivePanelMetrics((!this.PanelShowHideToggle.value) ? "close" : "view");
		this.ReportMetrics = false;
        if (LeftSidePanelContainer.OnLeftSidePanelToggled != null)
        {
            LeftSidePanelContainer.OnLeftSidePanelToggled(MainContent.activeInHierarchy);//this.PanelShowHideToggle.value);
        }
        //if (this.PanelShowHideToggle.value)
        //{
        //    LeftSidePanelOptionsScreen.OnGameCenterAuthenticationCallBack = (Action<bool, string>)Delegate.Combine(LeftSidePanelOptionsScreen.OnGameCenterAuthenticationCallBack, new Action<bool, string>(this.OnGameCenterAuthenticated));
        //    Singleton<NaggerManager>.Instance.EnableNagging(false);
        //}
        //else
        //{
        //    LeftSidePanelOptionsScreen.OnGameCenterAuthenticationCallBack = (Action<bool, string>)Delegate.Remove(LeftSidePanelOptionsScreen.OnGameCenterAuthenticationCallBack, new Action<bool, string>(this.OnGameCenterAuthenticated));
        //    Singleton<NaggerManager>.Instance.EnableNagging(true);
        //}
        //TweenPosition[] panelShowHideTween = this.PanelShowHideTween;
        //for (int i = 0; i < panelShowHideTween.Length; i++)
        //{
        //    TweenPosition tweenPosition = panelShowHideTween[i];
        //    if (this.PanelShowHideToggle.value)
        //    {
        //        tweenPosition.from = this.originalPanelFrom;
        //        tweenPosition.to = tweenPosition.value;
        //    }
        //    else
        //    {
        //        tweenPosition.from = tweenPosition.value;
        //        tweenPosition.to = this.originalPanelTo;
        //    }
        //    if (this.PanelShowHideToggle.value)
        //    {
        //        tweenPosition.PlayReverse();
        //    }
        //    else
        //    {
        //        tweenPosition.PlayForward();
        //    }
        //    tweenPosition.enabled = true;
        //    this.BlockerBackground.gameObject.SetActive(this.PanelShowHideToggle.value);
        //}
        //if (ScreenManager.Active.CurrentScreen != ScreenID.Dummy)
        //{
        //    if (this.PanelShowHideToggle.value)
        //    {
        //        Singleton<AudioManager>.Instance.PlaySound(AudioEvent.Frontend_LeftPanel_Open, null);
        //    }
        //    else
        //    {
        //        Singleton<AudioManager>.Instance.PlaySound(AudioEvent.Frontend_LeftPanel_Close, null);
        //    }
        //}
		if (true)//this.PanelShowHideToggle.value)
		{
		    int num = 0;//TutorialCommand.ExecuteOnObjective<int>(new OverrideLeftSidePanelTab());
			if (num >= 0)
			{
				//switch (num)
				//{
                //case 0:
                //    this.Toggle_Tab0.value = true;
                //    break;
                //case 1:
                //    this.Toggle_Tab1.value = true;
                //    break;
                //case 2:
                //    this.Toggle_Tab2.value = true;
                //    break;
                //case 3:
                //    this.Toggle_Tab3.value = true;
                //    break;
				//}
			}
			else
			{
				//int num2 = (!(ObjectiveManager.Instance == null)) ? ObjectiveManager.Instance.ObjectivesAwaitingCollectionCount() : 0;
                //int num3 = (!(PlayerProfileManager.Instance == null) || !PlayerProfileManager.Instance.ActiveProfile.UnseenChat) ? 0 : 1;
                //bool flag = TutorialQuery.IsStoryComplete("Crew");
                //if (num2 > 0 || !flag)
                //{
                //    this.Toggle_Tab1.value = true;
                //}
                //else if (num3 > 0)
                //{
                //    this.Toggle_Tab0.value = true;
                //}
			}
		}
	}

	private void LogActivePanelMetrics(string eventType)
	{
        //List<PlayerCrewUISubScreen> list = new List<PlayerCrewUISubScreen>
        //{
        //    LeftSidePanelChatScreen.Instance,
        //    LeftSidePanelAchievementsScreen.Instance,
        //    LeftSidePanelProfileScreen.Instance,
        //    LeftSidePanelOptionsScreen.Instance
        //};
        //foreach (PlayerCrewUISubScreen current in list)
        //{
        //    if (current != null && current.gameObject.activeInHierarchy)
        //    {
        //        current.LogMetricEvent(eventType);
        //        break;
        //    }
        //}
	}

	private void OnTabChanged()
	{
        if (this.ReportMetrics)
        {
            this.TabMetricsDirty = true;
        }
        if (ScreenManager.Instance.CurrentScreen != ScreenID.Dummy)
        {
            Singleton<AudioManager>.Instance.PlaySound(AudioEvent.Frontend_Forward, null);
        }
	}

	public bool IsLeftSidePanelOpen()
	{
		return this.MainContent.activeInHierarchy;
	}

	public void RemoveSubScreen(PlayerCrewUISubScreen screen)
	{
		this.SubScreens.Remove(screen);
	}

	public void AddSubScreen(PlayerCrewUISubScreen screen)
	{
		if (this.SubScreens.Find((PlayerCrewUISubScreen x) => x == screen) != null)
		{
			return;
		}
		this.SubScreens.Add(screen);
	}

	public void HideLeftSidePanel()
	{
		this.HidePanel();
		this.OnShowHidePanel();
	}

	public void ShowLeftSidePanel()
	{
		this.ShowPanel();
		this.OnShowHidePanel();
	}

	public void SetToggleFunctionality(bool bEnable)
	{
        //if (this.BlockerBackground != null)
        //{
        //    this.BlockerBackground.enabled = bEnable;
        //}
        //if (this.DragObject_Tab != null)
        //{
        //    this.DragObject_Tab.gameObject.SetActive(bEnable);
        //}
	}

	public void SetTabsFunctionality(bool bEnable)
	{
        //if (this.Toggle_Tab0 != null)
        //{
        //    this.Toggle_Tab0.enabled = bEnable;
        //}
        //if (this.Toggle_Tab1 != null)
        //{
        //    this.Toggle_Tab1.enabled = bEnable;
        //}
        //if (this.Toggle_Tab2 != null)
        //{
        //    this.Toggle_Tab2.enabled = bEnable;
        //}
        //if (this.Toggle_Tab3 != null)
        //{
        //    this.Toggle_Tab3.enabled = bEnable;
        //}
	}

	private void OnGameCenterAuthenticated(bool success, string err)
	{
		this.GCStatusDirty = success;
        //if (success)
        //{
        //    AdjustMetricsHelper.LogRegistration("GoogleGames");
        //}
	}

	private void playerLoggedOut()
	{
		this.GCStatusDirty = true;
	}

	private void SetupFacebookListeners()
	{
        //iOSEvents.fbDidLoginEvent += new iOSEvents_Delegate(this.fbDidLogin);
        //iOSEvents.fbDidLogoutEvent += new iOSEvents_Delegate(this.fbDidLogout);
	}

	private void RemoveFacebookListeners()
	{
        //iOSEvents.fbDidLoginEvent -= new iOSEvents_Delegate(this.fbDidLogin);
        //iOSEvents.fbDidLogoutEvent -= new iOSEvents_Delegate(this.fbDidLogout);
	}

	public void fbDidLogin()
	{
		//this.FacebookDirty = true;
	}

	public void fbDidLogout()
	{
		//this.FacebookDirty = true;
	}

	private void applicationDidEnterBackground()
	{
		this.applicationBackgroundDirty = true;
	}

	private void applicationWillEnterForeground()
	{
		this.applicationForegroundDirty = true;
	}
}
