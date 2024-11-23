using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Z2HSharedLibrary.DatabaseEntity;
using Z2HSharedLibrary.Operation;

public class LeaderboardScreen : ZHUDScreen
{
    [SerializeField] private Toggle[] m_leaderboardNameToggles;
    [SerializeField] private UnityEvent m_startupEvent;
    [SerializeField] private LeaderboardScrollerController m_scroller;
    private RectTransform m_scrollerRect;
    private Vector2 m_scrollerRectSizeDelta;
    private Vector2 m_scrollerRectPosition;
    [SerializeField] private float m_weeklySizeDelta = -100;
    [SerializeField] private float m_weeklyPositionDelta = -150;

    [SerializeField] private RuntimeTextButton m_weeklyLeaderboardButton;
    [SerializeField] private RuntimeTextButton m_previousWeeklyLeaderboardButton;
    [SerializeField] private RuntimeTextButton m_globalLeaderboardButton;
    [SerializeField] private RuntimeTextButton m_quarterMileLeaderboardButton;
    [SerializeField] private RuntimeTextButton m_halfMileLeaderboardButton;
    [SerializeField] private RuntimeTextButton m_nought100LeaderboardButton;
    [SerializeField] private RuntimeButton goToMyRankButton;

    [SerializeField] private TextMeshProUGUI m_timeText;
    [SerializeField] private TextMeshProUGUI m_timeLabeText;
    [SerializeField] private TextMeshProUGUI m_rankLabelText;
    [SerializeField] private TextMeshProUGUI m_quarterMileLabelText;
    [SerializeField] private TextMeshProUGUI m_halfMileLabelText;
    [SerializeField] private TextMeshProUGUI m_nough60LabelText;

    private string m_selectedLeaderboardName;
    private static bool m_showingprofile;
    //private string m_lastGlobalLeaderboardName;

    private static readonly Dictionary<LegacyLeaderboardManager.LeaderboardType, CachedLeaderboard> m_cachedLeaderboards
         = new Dictionary<LegacyLeaderboardManager.LeaderboardType, CachedLeaderboard>();
    private DateTime m_endOfWeekDate;
    private bool m_loadingLeaderboard;
    private bool m_useCaching;
    private bool m_scrollDown;

    public static LeaderboardScreen Instance { get; private set; }

    public override ScreenID ID
    {
        get { return ScreenID.Leaderboards; }
    }

    public override void OnActivate(bool zAlreadyOnStack)
    {
        base.OnActivate(zAlreadyOnStack);
        Instance = this;
        SwitchLeaderboardName(true);
        m_startupEvent.Invoke();

        m_weeklyLeaderboardButton.AddValueChangedDelegate(SwitchLeaderboardToWeekly);
        m_globalLeaderboardButton.AddValueChangedDelegate(SwitchLeaderboardToGlobal);
        m_quarterMileLeaderboardButton.AddValueChangedDelegate(SwitchLeaderboardToQuarterMile);
        m_halfMileLeaderboardButton.AddValueChangedDelegate(SwitchLeaderboardToHalfMile);
        m_nought100LeaderboardButton.AddValueChangedDelegate(SwitchLeaderboardToNought100);
        m_previousWeeklyLeaderboardButton.AddValueChangedDelegate(SwitchLeaderboardToPastWeekly);
        goToMyRankButton.AddValueChangedDelegate(GotoMyRank);

        m_scrollerRect = m_scroller.GetComponent<RectTransform>();
        m_scrollerRectSizeDelta = m_scrollerRect.sizeDelta;
        m_scrollerRectPosition = m_scrollerRect.anchoredPosition;

        m_endOfWeekDate = ServerSynchronisedTime.Instance.GetDateTime().GetEndOfweek();
        m_rankLabelText.text =
            string.Format(LocalizationManager.GetTranslation("TEXT_LEADERBOARD_THIS_WEEK_RANK_LABEL"), "-");

        var useMile = PlayerProfileManager.Instance.ActiveProfile.UseMileAsUnit;
        var quarterMileText = "TEXT_LEADERBOARD_QUARTER_MILE";
        var halfMileText = "TEXT_LEADERBOARD_HALF_MILE";
        var nought60Text = "TEXT_LEADERBOARD_NOUGHT_60MPH";

        if (!useMile)
        {
            quarterMileText += "_IN_METER";
            halfMileText += "_IN_METER";
            nought60Text = "TEXT_LEADERBOARD_NOUGHT_100";
        }

        m_quarterMileLabelText.text = LocalizationManager.GetTranslation(quarterMileText);
        m_halfMileLabelText.text = LocalizationManager.GetTranslation(halfMileText);
        m_nough60LabelText.text = LocalizationManager.GetTranslation(nought60Text);
        SwitchLeaderboardToWeekly();
    }

    private void OnlineRaceGameEvents_OperationFailed(DatabaseOperationCode arg1, ResponseCode arg2, string arg3)
    {
        switch (arg1)
        {
            case DatabaseOperationCode.GetPlayerDataByID:
                if (CurrentState == VisibleState.Visible && m_showingprofile)
                {
                    GTDebug.Log(GTLogChannel.Screens,"failed to show");
                    m_showingprofile = false;
                    PopUpManager.Instance.KillPopUp();
                    PopUpDatabase.Common.ShowTimeoutPopop();
                }
                break;
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        Instance = null;
        m_quarterMileLeaderboardButton.RemoveValueChangedDelegate(SwitchLeaderboardToQuarterMile);
        m_halfMileLeaderboardButton.RemoveValueChangedDelegate(SwitchLeaderboardToHalfMile);
        m_nought100LeaderboardButton.RemoveValueChangedDelegate(SwitchLeaderboardToNought100);
        m_globalLeaderboardButton.RemoveValueChangedDelegate(SwitchLeaderboardToGlobal);
        m_weeklyLeaderboardButton.RemoveValueChangedDelegate(SwitchLeaderboardToWeekly);
        m_previousWeeklyLeaderboardButton.RemoveValueChangedDelegate(SwitchLeaderboardToPastWeekly);
        goToMyRankButton.RemoveValueChangedDelegate(GotoMyRank);
    }

    private void GotoMyRank()
    {
        var records = m_scroller.GetRecords();
        int i = 0;

        var playerID = UserManager.Instance.currentAccount.UserID;

        for (int j = 0; j < records.Length; j++)
        {
            if (records[j].UserID == playerID)
            {
                i = j;
            }
        }

        m_scrollDown = !m_scrollDown;
        goToMyRankButton.transform.GetChild(0).localScale = new Vector3(1, m_scrollDown ? -1 : 1, 1);
        //SwitchScrollRectSize(false);
        //m_targetScrollPosition = i;
        m_scroller.SelectedPosition = m_scrollDown ? i : 0;
    }

    public void SwitchLeaderboardName(bool value)
    {
        //m_selectedLeaderboardName = m_leaderboardNameToggles.FirstOrDefault(n => n.isOn).name;
    }

    public void UpdateLeaderboard(LegacyLeaderboardManager.LeaderboardType type)
    {
        if (!PolledNetworkState.IsNetworkConnected)
        {
            PopUpDatabase.Common.ShowNoInternetConnectionPopup(Close);
            return;
        }

        GetUserLeaderboard(type);
    }

    private IEnumerator _timeoutOperation()
    {
        float timout;
        try
        {
            timout = GameDatabase.Instance.OnlineConfiguration.LeaderboardTimeout;
        }
        catch (Exception)
        {
            timout = 30;
        }

        yield return new WaitForSeconds(timout);
        if (m_loadingLeaderboard || m_showingprofile)
        {
            PopUpManager.Instance.KillPopUp();
            PopUpDatabase.Common.ShowTimeoutPopop(Close);
        }
    }

    public void SwitchLeaderboardToGlobal()
    {
        m_globalLeaderboardButton.CurrentState = BaseRuntimeControl.State.Highlight;
        m_quarterMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_halfMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_nought100LeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_weeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_previousWeeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_timeText.gameObject.SetActive(false);
        m_timeLabeText.gameObject.SetActive(false);
        UpdateLeaderboard(LegacyLeaderboardManager.LeaderboardType.Top);
    }

    public void SwitchLeaderboardToQuarterMile()
    {
        m_quarterMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Highlight;
        m_halfMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_nought100LeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_globalLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_weeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_previousWeeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_timeText.gameObject.SetActive(false);
        m_timeLabeText.gameObject.SetActive(false);
        UpdateLeaderboard(LegacyLeaderboardManager.LeaderboardType.QuarterMile);
    }

    public void SwitchLeaderboardToHalfMile()
    {
        m_quarterMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_halfMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Highlight;
        m_nought100LeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_globalLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_weeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_previousWeeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_timeText.gameObject.SetActive(false);
        m_timeLabeText.gameObject.SetActive(false);
        UpdateLeaderboard(LegacyLeaderboardManager.LeaderboardType.HalfMile);
    }

    public void SwitchLeaderboardToNought100()
    {
        m_quarterMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_halfMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_nought100LeaderboardButton.CurrentState = BaseRuntimeControl.State.Highlight;
        m_globalLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_weeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_previousWeeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_timeText.gameObject.SetActive(false);
        m_timeLabeText.gameObject.SetActive(false);
        UpdateLeaderboard(LegacyLeaderboardManager.LeaderboardType.Nought100);
    }

    public void SwitchLeaderboardToWeekly()
    {
        m_quarterMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_halfMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_nought100LeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_globalLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_weeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Highlight;
        m_previousWeeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_timeText.gameObject.SetActive(true);
        m_timeLabeText.gameObject.SetActive(true);
        UpdateLeaderboard(LegacyLeaderboardManager.LeaderboardType.Weekly);
    }

    public void SwitchLeaderboardToPastWeekly()
    {
        m_quarterMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_halfMileLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_nought100LeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_globalLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_weeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Active;
        m_previousWeeklyLeaderboardButton.CurrentState = BaseRuntimeControl.State.Highlight;
        m_timeText.gameObject.SetActive(false);
        m_timeLabeText.gameObject.SetActive(false);
        UpdateLeaderboard(LegacyLeaderboardManager.LeaderboardType.PastWeekly);
    }

    public void GetUserLeaderboard(LegacyLeaderboardManager.LeaderboardType leaderboardType)
    {
        if (!m_cachedLeaderboards.ContainsKey(leaderboardType))
        {
            m_cachedLeaderboards[leaderboardType] = new CachedLeaderboard();
        }

        if (m_cachedLeaderboards[leaderboardType].IsCachedAvailableAndNotExpire())
        {
            m_useCaching = true;

            RefreshLeaderboardRecords(leaderboardType,
                m_cachedLeaderboards[leaderboardType].CachedRecords,
                m_cachedLeaderboards[leaderboardType].TopCount);
        }
        else
        {
            m_useCaching = false;
            PopUpDatabase.Common.ShowWaitSpinnerPopup();
            LegacyLeaderboardManager.Instance.GetUserLeaderboard(UserManager.Instance.currentAccount.Username, "A", leaderboardType, LeaderboardResponse);
            m_cachedLeaderboards[leaderboardType].RenewTime();

            m_loadingLeaderboard = true;
            StopCoroutine("_timeoutOperation");
            StartCoroutine("_timeoutOperation");
        }

        goToMyRankButton.transform.GetChild(0).localScale = new Vector3(1, 1 , 1);
    }

    private void LeaderboardResponse(string content, string zerror, int zstatus, object zuserdata)
    {
        m_loadingLeaderboard = false;
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting weekly leaderboard : " + zerror);
            PopUpManager.Instance.KillPopUp();
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(content))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting weekly leaderboard : server send malformed json in response");
            PopUpManager.Instance.KillPopUp();
            return;
        }

        var leaderboardType = (LegacyLeaderboardManager.LeaderboardType)parameters.GetInt("leaderboard_type");
        var records = parameters.GetObjectList<LeaderboardRecord>("records", LegacyLeaderboardManager.Instance.GetLeaderboardRecord).ToArray();
        var topLeaderboardCount = parameters.GetInt("top_leaderboard_count");

        RefreshLeaderboardRecords(leaderboardType, records, topLeaderboardCount);
    }


    private void RefreshLeaderboardRecords(LegacyLeaderboardManager.LeaderboardType leaderboardType, LeaderboardRecord[] records, int topLeaderboardCount)
    {
        if (!m_useCaching)
        {
            if (!m_cachedLeaderboards.ContainsKey(leaderboardType))
            {
                m_cachedLeaderboards.Add(leaderboardType, new CachedLeaderboard());
            }
            m_cachedLeaderboards[leaderboardType].UpdateCache(records, topLeaderboardCount);
        }


        m_scroller.RealodData(records, leaderboardType, topLeaderboardCount);

        m_scroller.SelectedPosition = 0;
        m_scrollDown = false;

        PopUpManager.Instance.KillPopUp();

        var playerRecord = records.FirstOrDefault(rec => rec.UserID == UserManager.Instance.currentAccount.UserID);
        var rank = playerRecord != null && (playerRecord.LongScoreValue > 0
            || playerRecord.FloatScoreValue > 0)
            ? playerRecord.Rank.ToString().ToNativeNumber()
            : LocalizationManager.GetTranslation("TEXT_RANK_NA");
        //Debug.Log("last record id : " + playerRecord.UserID);
        var leaderboardName = leaderboardType.ToString().ToUpper();

        var rankText = "TEXT_" + leaderboardName + "_RANK_LABEL";
        if (!PlayerProfileManager.Instance.ActiveProfile.UseMileAsUnit)
        {
            rankText += "_IN_METER";
        }

        m_rankLabelText.text =
            string.Format(LocalizationManager.GetTranslation(rankText), rank);
    }

    private void SwitchScrollRectSize(bool setToOriginal)
    {
        if (setToOriginal)
        {
            m_scrollerRect.sizeDelta = m_scrollerRectSizeDelta;
            m_scrollerRect.anchoredPosition = m_scrollerRectPosition;
        }
        else
        {
            var sizeDelta = m_scrollerRectSizeDelta;
            sizeDelta.y += m_weeklySizeDelta;

            var position = m_scrollerRectPosition;
            position.y += m_weeklyPositionDelta;
            m_scrollerRect.anchoredPosition = position;
        }
    }

    public static void ShowProfile(long userID)
    {
        if (UserManager.Instance.currentAccount.UserID == userID)
        {
            ShowProfile(PlayerProfileManager.Instance.ActiveProfile.GetProfileData());
        }
        else
        {
            var username = Account.GetUsername(userID);
            PopUpDatabase.Common.ShowWaitSpinnerPopup();
            m_showingprofile = true;
            PlayerProfileWeb.GetPlayerProfileData(username,false, ProfileDataResponseCallback);
            Instance.StopCoroutine("_timeoutOperation");
            Instance.StartCoroutine("_timeoutOperation");
        }
    }

    private static void ProfileDataResponseCallback(string content, string zerror, int zstatus, object zuserdata)
    {
        PopUpManager.Instance.KillPopUp();

        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            m_showingprofile = false;
            GTDebug.LogError(GTLogChannel.Leaderboards, "error getting profile data : " + zerror);
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(content))
        {
            m_showingprofile = false;
            GTDebug.LogError(GTLogChannel.Leaderboards, "error getting profile data : server send malformed json in response");
            return;
        }

        var username = parameters.GetString("username");
        var profileDataJson = parameters.GetString("profile_data");
        if (string.IsNullOrEmpty(profileDataJson))
        {
            m_showingprofile = false;
            GTDebug.LogError(GTLogChannel.Leaderboards, "error getting profile data : profile data json is empty or null");
            return;
        }

        var profileData = PlayerProfileMapper.FromJson(profileDataJson);
        var userID = Account.GetUserID(username);

        if (Instance.CurrentState == VisibleState.Visible && m_showingprofile)
        {
            GTDebug.Log(GTLogChannel.Leaderboards, "showing profile for userid : " + userID);
            ShowProfile(profileData);
            m_showingprofile = false;
        }
    }

    private static void ShowProfile(PlayerProfileData user)
    {
        ProfileScreen.SetUser(user);
        ScreenManager.Instance.PushScreen(ScreenID.Profile);
    }

    protected override void Update()
    {
        base.Update();
        if (m_timeText.gameObject.activeInHierarchy)
        {
            var timeSpan = m_endOfWeekDate - ServerSynchronisedTime.Instance.GetDateTime();
            if (timeSpan > TimeSpan.Zero)
            {
                if (timeSpan.TotalDays > 0)
                    m_timeText.text =
                        string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_DAYS_AND_HOURS_AND_MINUTES_AND_SECONDS_BY_WORD"),
                            (int)timeSpan.TotalDays, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                else if (timeSpan.TotalHours>0)
                {
                    m_timeText.text =
                        string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_HOURS_AND_MINUTES_AND_SECONDS_BY_WORD"),
                            timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                }
                else if (timeSpan.TotalMinutes > 0)
                {
                    m_timeText.text =
                        string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_MINUTES_AND_SECONDS_BY_WORD"), timeSpan.Minutes, timeSpan.Seconds);
                }
                else
                {
                    m_timeText.text =
                        string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_SECONDS_BY_WORD"), timeSpan.Seconds);
                }
            }
            else
            {
                m_timeText.text = LocalizationManager.GetTranslation("TEXT_LEADERBOARD_END_OF_LEAGUE");
            }
        }
    }

    public static void ClearCache()
    {
        m_cachedLeaderboards.Clear();
    }
}