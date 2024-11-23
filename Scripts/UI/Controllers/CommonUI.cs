using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommonUI : MonoBehaviour
{
    [SerializeField] private RectTransform m_dashboard;
    [SerializeField] private RectTransform m_normalPanel;
    [SerializeField] private RectTransform m_backButton;
    [SerializeField] private RectTransform m_shopButton;
    [SerializeField] private RectTransform m_shopButtonPoint;
    [SerializeField] private Animator m_animator;
    private DashboardType m_dashboardType = DashboardType.None;
    private GraphicRaycaster m_graphicRaycaster;
    public bool IsIn { get; private set; }

    public RectTransform ShopButton
    {
        get { return m_shopButton; }
    }

    public RectTransform ShopButtonPoint
    {
        get { return m_shopButtonPoint; }
    }

    public CashStats CashStats;

    public CarNameStats mainNameStats;
    public CarNameStats zoomedNameStats;

    public XPStats XPStats;

    public LegacyStarStats StarStats;

    public StarLeagueStats StarLeagueStats;

    public ConnectionStats ConnectionStats;

    public FuelNavBarUI FuelStats;

    public NavBarInfoPaneManager NavBarInfoManager;

    public GameObject discountLabel;

    //public NavigationBar NavBar;

    public RPStats RPStats;

    public RPBonusStats RPBonusStats;


    private List<IPersistentUI> _allItems = new List<IPersistentUI>();
    private bool _hideAll;
    public static CommonUI Instance { get; private set; }

    public eRankBarMode RankBarMode
    {
        get;
        set;
    }

    public bool IsShowingFullNavBar
    {
        get { return IsIn && m_dashboardType == DashboardType.Normal; }
    }

    public bool IsShowingBackButton
    {
        get { return IsIn && (m_dashboardType == DashboardType.JustClose || m_dashboardType == DashboardType.Normal)
            && m_backButton.gameObject.activeInHierarchy; }
    }


    private void Start()
    {
        m_graphicRaycaster = GetComponent<GraphicRaycaster>();
        //this._allItems.Add(this.NavBar);
        //this._allItems.Add(this.NetworkActivity);
        this._allItems.Add(this.CashStats);
        this._allItems.Add(this.mainNameStats);
        //this._allItems.Add(this.MenuBackground);
        this._allItems.Add(this.XPStats);
        //this._allItems.Add(this.RPStats);
        this._allItems.Add(this.FuelStats);
        this._allItems.Add(this.ConnectionStats);
        //this._allItems.Add(this.ConnectionStats);
        this._allItems.Add(this.NavBarInfoManager);
        //this._allItems.Add(this.StarCountStats);
        //this._allItems.Add(this.RPBonusStats);
        //this.Show(false);//Dont uncomment this line because it hide commonui element permanently

        //this.XPStats.uiTxtLevel.Text = LocalizationManager.GetTranslation("TEXT_UI_XP_LEVEL");
    }

    public void TriggerShowFirstTime()
    {
        this.Show(true);
        foreach (IPersistentUI current in this._allItems)
        {
            current.OnScreenChanged(ScreenID.Home);
        }
    }

    private void StackManager_ScreenChanged(ScreenID zID)
    {
        //if (navBarVisible.ContainsKey(zID))
        //{
        //    this.Show(navBarVisible[targetScreen.GetScreenID()]);
        //}
        foreach (IPersistentUI current in this._allItems)
        {
            current.OnScreenChanged(zID);
        }

        //this.UpdateRankBars(zNewScreenID);
        //this.UpdateStarCountBar(zNewScreenID);
    }

    public void OnAnimationOpen()
    {
    }

    public void OnAnimationClose()
    {
        zoomedNameStats.gameObject.SetActive(GarageCameraManager.Instance!=null
            && GarageCameraManager.Instance.gameObject.activeInHierarchy && GarageCameraManager.Instance.IsZoomedIn);
    }

    private void Show(bool v)
    {
        foreach (IPersistentUI current in this._allItems)
        {
            current.Show(v);
        }
    }

    private void Awake()
    {
        Instance = this;
        if (UICamera.Instance != null)
        {
            GetComponentInChildren<Canvas>().worldCamera = UICamera.Instance.Camera;
        }
        ScreenManager.ScreenChanged += StackManager_ScreenChanged;
        ScreenManager.ScreenStateChanged += ScreenManager_ScreenStateChanged;
    }

    private void ScreenManager_ScreenStateChanged(ScreenManager.State obj)
    {
        switch (obj)
        {
            case ScreenManager.State.SHUTTING_DOWN_CURRENT_SCREEN:
                TryOpenCloseDashboard(false);
                break;
            case ScreenManager.State.STARTING_UP_NEW_SCREEN:
                if ((ScreenManager.Instance.CurrentScreen == ScreenID.Workshop
                    && GarageScreen.Instance.ShowingIntro)
                    || (GarageCameraManager.Instance!=null
                    && GarageCameraManager.Instance.IsZoomedIn))
                {
                    return;
                }
                TryOpenCloseDashboard(true);
                break;
        }

        if (discountLabel == null)
            return;
        discountLabel.SetActive(false);
        if (ProductManager.Instance.discount != 0)
        {
            discountLabel.SetActive(true);
            if (ProductManager.Instance.discount == 1) {
                this.discountLabel.GetComponentInChildren<TextMeshProUGUI>().text = "%15";
            } else if (ProductManager.Instance.discount == 2) {
                this.discountLabel.GetComponentInChildren<TextMeshProUGUI>().text = "%30";
            }
        }
    }

    public void TryOpenCloseDashboard(bool open)
    {
        if (!open)
        {
            PlayAnimation(false);
            return;
        }
        if (!(ScreenManager.Instance.ActiveScreen is ZHUDScreen))
        {
            //m_dashboard.gameObject.SetActive(false);
            return;
        }
        var zHudScreen = (ZHUDScreen) ScreenManager.Instance.ActiveScreen;
        m_dashboardType = zHudScreen.DashboardType;
        //m_dashboard.SetAsLastSibling();

        switch (zHudScreen.DashboardType)
        {
            case DashboardType.None:
                PlayAnimation(false);
                //m_dashboard.gameObject.SetActive(false);
                break;
            case DashboardType.JustClose:
                //m_dashboard.gameObject.SetActive(true);
                m_normalPanel.gameObject.SetActive(false);
                m_backButton.gameObject.SetActive(true);
                PlayAnimation(true);
                break;
            case DashboardType.Normal:
                //m_dashboard.gameObject.SetActive(true);
                m_normalPanel.gameObject.SetActive(true);
                m_backButton.gameObject.SetActive(true);
                PlayAnimation(true);
                break;
            case DashboardType.JustNormal:
                //m_dashboard.gameObject.SetActive(true);
                m_normalPanel.gameObject.SetActive(true);
                m_backButton.gameObject.SetActive(false);
                PlayAnimation(true);
                break;
        }
    }

    public void PlayAnimation(bool open)
    {
        IsIn = open;
    }

    public void OnBack()
    {
        var activeScreen = ScreenManager.Instance.ActiveScreen;
        if (activeScreen!=null && !activeScreen.IgnoreHardwareBackButton() && activeScreen.ID != ScreenID.Splash)
        {
            activeScreen.OnHardwareBackButton();
        }
    }

    public void OpenScreen(ScreenID screenID)
    {
        ScreenManager.Instance.PushScreen(screenID);
    }

    private void Update()
    {
        if (m_animator.gameObject.activeInHierarchy)
        {
            if (!m_animator.IsInTransition(0))
            {
                var stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
                //Debug.Log(IsIn+"  "+Time.time);
                if (IsIn && !stateInfo.IsName("open"))
                {
                    m_animator.Play("open");
                }
                else if (!IsIn && stateInfo.IsName("open"))
                {
                    m_animator.Play("close");
                }
            }
            if(AppStore.Instance.ShouldHideIAPInterface)
                m_shopButton.gameObject.SetActive(false);
        }
        //m_animator.Play(open ? "open" : "close");
    }

    public void DisableAllButtonOperability()
    {
        m_graphicRaycaster.enabled = false;
    }

    public void EnableAllButtonOperability()
    {
        m_graphicRaycaster.enabled = true;
    }

    public void ShowBackButton()
    {
        m_backButton.gameObject.SetActive(true);
    }

    public void HideBackButton()
    {
        m_backButton.gameObject.SetActive(false);
    }

    public void SetRankBars(eRankBarMode rankBarMode, bool updateRPValue = true)
    {
        if (this._hideAll)
        {
            return;
        }
        this.SetStatsActivity(rankBarMode == eRankBarMode.XP_RANK, rankBarMode == eRankBarMode.MULTIPLAYER_RANK, rankBarMode == eRankBarMode.FRIENDS_RANK);
        this.RankBarMode = rankBarMode;
    }

    private void SetStatsActivity(bool XPStatsState, bool RPStatsState, bool StarCountState)
    {
        this.XPStats.Show(XPStatsState);
        this.RPStats.Show(RPStatsState);
        this.RPBonusStats.Show(RPStatsState);
        //this.StarCountStats.Show(StarCountState);
    }
}
