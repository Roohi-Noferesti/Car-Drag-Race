using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using Objectives;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class LegacyLeaderboardManager : MonoSingleton<LegacyLeaderboardManager>
{
    public enum LeaderboardType
    {
        Top,
        Weekly,
        PastWeekly,
        QuarterMile,
        HalfMile,
        Nought100
    }

    private bool m_checkingPreviousLeaderboardWinners;
    private WeeklyReward m_weeklyReward;
    private LeaderboardRecord m_playerRecord;
    private int currentWeeklyRank;
    public bool UserStarFetched { get; private set; }
    public bool FetchingUserStar { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        PolledNetworkState.JustWentOffline += PolledNetworkState_JustWentOffline;
    }

    private void PolledNetworkState_JustWentOffline()
    {
        UserStarFetched = false;
        FetchingUserStar = false;
    }

    public void OnProfileChanged()
    {
        FetchingUserStar = false;
        UserStarFetched = false;
        m_checkingPreviousLeaderboardWinners = false;
        m_weeklyReward = null;
        m_playerRecord = null;
    }

    private void Update()
    {
        //if (!Input.GetKeyDown(KeyCode.Space)) return;
        //m_checkingPreviousLeaderboardWinners = true;
        //GetUserLeaderboard(UserManager.Instance.currentAccount.Username, "A", LeaderboardType.PastWeekly, WeeklyLeaderboardResponse);
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space pressed");
            m_checkingPreviousLeaderboardWinners = true;
            GetUserLeaderboard(UserManager.Instance.currentAccount.Username, "A", LeaderboardType.PastWeekly, WeeklyLeaderboardResponse);
         //   UnityEngine.Debug.Log("currentleague:" + UserManager.Instance.currentAccount.CurrentLeagueProp);
         //   UnityEngine.Debug.Log("previous leawgues:" + UserManager.Instance.currentAccount.PreviousLeagueProp);
        }
#endif

        if (!PolledNetworkState.IsNetworkConnected || !UserManager.Instance.isLoggedIn && !ServerSynchronisedTime.Instance.IsServerTimeMatchClient) return;

        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;


        if (activeProfile.HasWeeklyRewardToClaim && !PopUpManager.Instance.isShowingPopUp && ScreenManager.Instance.CurrentScreen == ScreenID.Workshop)
        {
            CommonUI.Instance.CashStats.GoldLockedState(true);
            CommonUI.Instance.CashStats.CashLockedState(true);
            activeProfile.HasWeeklyRewardToClaim = false;
            activeProfile.PreviousWeeklyLeaderboardCheck = ServerSynchronisedTime.Instance.GetDateTime();
            ApplyReward();
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        }
        else
        {
            var previousWeekEndDate = ServerSynchronisedTime.Instance.GetDateTime().GetEndOfweek().AddDays(-7);

            if (UserManager.Instance.isLoggedIn && !m_checkingPreviousLeaderboardWinners && activeProfile.PreviousWeeklyLeaderboardCheck < previousWeekEndDate)
            {
                m_checkingPreviousLeaderboardWinners = true;
                GetUserLeaderboard(UserManager.Instance.currentAccount.Username, "A", LeaderboardType.PastWeekly, WeeklyLeaderboardResponse);
            }
        }

        if (!UserStarFetched)
        {
            if (!FetchingUserStar)
            {
                if (UserManager.Instance.isLoggedIn)
                {
                    FetchingUserStar = true;
                    JsonDict parameters = new JsonDict();
                    parameters.Set("username", UserManager.Instance.currentAccount.Username);
                 //   parameters.Set("current_league_id", UserManager.Instance.currentAccount.CurrentLeagueProp);
                    WebRequestQueue.Instance.StartCall("rtw_getstar_league", "get user stars", parameters, StarResponse, null, null);
                }
            }
        }
    }

    private void WeeklyLeaderboardResponse(string content, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting weekly leaderboard : " + zerror);
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(content))
        {
            GTDebug.LogError(GTLogChannel.RPBonus,
                "error getting weekly leaderboard : server send malformed json in response");
            return;
        }

        var records = parameters.GetObjectList<LeaderboardRecord>("records", GetLeaderboardRecord);


        if (m_checkingPreviousLeaderboardWinners)
        {
            m_checkingPreviousLeaderboardWinners = false;
            if (records.Count > 0)
            {
                m_playerRecord = records.FirstOrDefault(r => r.UserID == UserManager.Instance.currentAccount.UserID);
                if (m_playerRecord == null)
                {
                    PlayerProfileManager.Instance.ActiveProfile.PreviousWeeklyLeaderboardCheck = ServerSynchronisedTime.Instance.GetDateTime();
                    return;
                }
                var rank = m_playerRecord.Rank;

                long UsersInPreviousLeague = 0;
              //  bool ParseResult = long.TryParse(parameters.GetString("league_players_count"), out UsersInPreviousLeague);
              //  UnityEngine.Debug.Log("league  rank====>>>>" + rank);
              //  UserManager.Instance.currentAccount.PreviousLeagueProp = UserManager.Instance.currentAccount.CurrentLeagueProp;
                if (m_playerRecord != null && m_playerRecord.LongScoreValue > 0)
                {
                    //       m_weeklyReward = GameDatabase.Instance.CurrenciesConfiguration.WeeklyRewardData.GetRewardByRank((int)m_playerRecord.Rank);
                    //  var leagueRewardCategory = GameDatabase.Instance.CurrenciesConfiguration.LeagueRewardData.FirstOrDefault(x => x.LeagueName == UserManager.Instance.currentAccount.PreviousLeagueProp);
                    var leagueRewardCategory = GameDatabase.Instance.CurrenciesConfiguration.WeeklyRewardData;
                    if (leagueRewardCategory != null)
                    {
                        m_weeklyReward = leagueRewardCategory.GetRewardByRank((int)m_playerRecord.Rank);
                        //UnityEngine.Debug.Log("league==> rewards count>>" + m_weeklyReward.Rewards.Count);
                    }

                    if (m_weeklyReward != null)
                    {
                        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
                        activeProfile.HasWeeklyRewardToClaim = true;
                    }
                }

                //  UserManager.Instance.currentAccount.PreviousLeagueProp = UserManager.Instance.currentAccount.CurrentLeagueProp;
                //   var PreviousLeagueData = LeaguesHelper.Leagues.FirstOrDefault(x => x.ID == UserManager.Instance.currentAccount.PreviousLeagueProp);
                //if (rank < PreviousLeagueData.AscendingCount)
                //{
                //    UserManager.Instance.currentAccount.CurrentLeagueProp = LeaguesHelper.Leagues[PreviousLeagueData.Index + 1].ID;
                //}
                //  else if (rank > UsersInPreviousLeague - PreviousLeagueData.DescendingCount)
                //  {
                //var newLeagueIndex = 0;
                //if (PreviousLeagueData.Index > 0)
                //{
                //    newLeagueIndex = PreviousLeagueData.Index - 1;
                //}

                //   UserManager.Instance.currentAccount.CurrentLeagueProp = LeaguesHelper.Leagues[newLeagueIndex].ID;
                //  }
                //   UnityEngine.Debug.Log("Current user rank is====>>>>"+rank);
                //    UnityEngine.Debug.Log("previous league data===>>>"+PreviousLeagueData.AscendingCount);

            }
            PlayerProfileManager.Instance.ActiveProfile.PreviousWeeklyLeaderboardCheck = ServerSynchronisedTime.Instance.GetDateTime();
            //league
            //   UserManager.Instance.SaveCurrentAccount();
            //   JsonDict AccountParams = new JsonDict();
            //  AccountParams.Set("username", "user" + UserManager.Instance.currentAccount.UserID);
            //   AccountParams.Set("current_league_id", UserManager.Instance.currentAccount.CurrentLeagueProp);
            //   AccountParams.Set("previous_league_id", UserManager.Instance.currentAccount.PreviousLeagueProp);
            //   WebRequestQueue.Instance.StartCall("save_league_settings", "Save user Data", AccountParams, SaveUserDataResponseCallback, null, ProduceHashSource(AccountParams));
        }
    }


    private string ProduceHashSource(JsonDict dict)
    {
        string text = string.Empty;
        foreach (string current in dict.Keys)
        {
            text += dict.GetString(current);
        }
        return text;
    }
    public void GetLeaderboardRecord(JsonDict jsondict, ref LeaderboardRecord record)
    {
        record.UserID = jsondict.GetLong("urid");
        record.Rank = jsondict.GetLong("rank");
        record.AvatarID = jsondict.GetString("avid");
        record.FloatScoreValue = jsondict.GetFloat("fsvl");
        record.LongScoreValue = jsondict.GetLong("lsvl");
        record.UserDisplayName = jsondict.GetString("udpn");
        record.CarKey = jsondict.GetString("crky");
        record.Level = jsondict.GetInt("plvl");
    }

    private void StarResponse(string content, string zerror, int zstatus, object zuserdata)
    {
        FetchingUserStar = false;
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting user star : " + zerror);
            return;
        }

        JsonDict jsonDict = new JsonDict();
        if (jsonDict.Read(content))
        {
            UserStarFetched = true;
            var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            activeProfile.PlayerStar = jsonDict.GetInt("global_star");
            activeProfile.PlayerLeagueStar = jsonDict.GetInt("weekly_star");
            activeProfile.LastEndOfWeekCheck = ServerSynchronisedTime.EndDateOfLeague;
            GTDebug.Log(GTLogChannel.Account,
                "user star fetched : global_star:" + activeProfile.PlayerStar + ",weekly_star:" +
                activeProfile.PlayerLeagueStar);
        }

    }


    private void ShowWeeklyRewardPopupPopup()
    {
        PopUpManager.Instance.TryShowPopUp(new PopUp
        {
            Title = "TEXT_POPUPS_COLLECT_WEEKLY_REWARD_TITLE",
            BodyText =
                string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_COLLECT_WEEKLY_REWARD_BODY"), m_playerRecord.Rank,
                    m_weeklyReward.GetRewardText()),
            BodyAlreadyTranslated = true,
            IsBig = true,
            ConfirmText = "TEXT_BUTTON_GETPRIZE",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT",
            ConfirmAction = () =>
            {
                CommonUI.Instance.CashStats.GoldLockedState(false);
                CommonUI.Instance.CashStats.CashLockedState(false);
                FuelManager.Instance.FuelLockedState(false);
            }
        }, PopUpManager.ePriority.Default, null);
    }


    private void ApplyReward()
    {
        if (m_weeklyReward == null)
        {
            return;
        }

        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        int currentGold = activeProfile.GetCurrentGold();
        int currentCash = activeProfile.GetCurrentCash();
        foreach (ObjectiveRewardData current in m_weeklyReward.Rewards)
        {
            CSR2ApplyableReward cSR2ApplyableReward = new CSR2ApplyableReward(current.Reward, current.Amount);
            cSR2ApplyableReward.FillAnyRandomPrizes(current.Reward);
            cSR2ApplyableReward.ApplyAwardToPlayerProfile(false, false);

            CommonUI.Instance.CashStats.CashLockedState(true);
            CommonUI.Instance.CashStats.GoldLockedState(true);
            FuelManager.Instance.FuelLockedState(true);
            //RewardPopup rewardPopup = new RewardPopup("TEXT_PRIZE_CONGRATULATIONS", "TEXT_BUTTON_COLLECT", cSR2ApplyableReward, null, null, null, false, false, true, false, PopUpCharacterRole.MECHANIC);
            //rewardPopup.DisableBackButton = true;
            //this.activeRewardPopUpEventRequest = Singleton<PopUpManager>.Instance.RequestPopUp(rewardPopup, PopUpPriorityGroup.OBJECTIVE_NOTIFCATION, new PopUpEvent.OnRequestResult(this.OnRewardPopUpEventRequestResult), new PopUpEvent.OnComplete(this.OnRewardPopUpEventComplete), true, false);
        }
        ShowWeeklyRewardPopupPopup();


        JsonDict parameters = new JsonDict();
        parameters.Set("userid", UserManager.Instance.currentAccount.UserID.ToString());
        parameters.Set("rank", m_playerRecord != null ? m_playerRecord.Rank.ToString() : "null");
        parameters.Set("prevgold", currentGold);
        parameters.Set("currentgold", activeProfile.GetCurrentGold());
        parameters.Set("prevcash", currentCash);
        parameters.Set("currentcash", activeProfile.GetCurrentCash());
        parameters.Set("data", "data test");
        WebRequestQueue.Instance.StartCall("rtw_save_user_award_leaderboard_logs", "descrption", parameters, LeagueRewardLogResponseCallback, UserManager.Instance.currentAccount.UserID, string.Empty);







        Log.AnEvent(Events.LeaderboardReward, new Dictionary<Parameters, string>
        {
            {Parameters.BGld, (currentGold).ToString()},
            {Parameters.BCsh, (currentCash).ToString()},
            {Parameters.DGld, (activeProfile.GetCurrentGold() - currentGold).ToString()},
            {Parameters.DCsh, (activeProfile.GetCurrentCash() - currentCash).ToString()},
            {Parameters.tme, (ServerSynchronisedTime.Instance.GetDateTime()).ToString(CultureInfo.InvariantCulture)},
            {Parameters.rnk, m_playerRecord != null ? m_playerRecord.Rank.ToString() : "null"}
        });


    }

    private void LeagueRewardLogResponseCallback(string zHTTPContent, string zError, int zStatus, object zUserData)
    {

    }

    public bool IsUnlocked()
    {
        return PlayerProfileManager.Instance.ActiveProfile.GetPlayerLevel() >= GameDatabase.Instance.CareerConfiguration.LeaderboardUnlockLevel;
    }

    public void ShutDown()
    {
        UserStarFetched = false;
        FetchingUserStar = false;
    }

    public void GetUserLeaderboard(string username, string leaderboardname, LeaderboardType leaderboardType,
        WebClientDelegate2 ResponseCallback)
    {
        JsonDict parameters = new JsonDict();
        parameters.Set("username", username);
        parameters.Set("leaderboard_name", leaderboardname);
        parameters.Set("leaderboard_type", ((int)leaderboardType).ToString());
        WebRequestQueue.Instance.StartCall("rtw_get_weekly_leaderboard", "get Weekly Leaderboard for previous week", parameters, ResponseCallback, null, null);
    }

    public void SubmitRaceReward(string username, int totalStar, int playerLevel, bool isHalfMile, float raceTime, float noght100, string currentCar)
    {
        var playerProfileStar = PlayerProfileManager.Instance.ActiveProfile.PlayerStar;
        var playerProfileLeagueStar = PlayerProfileManager.Instance.ActiveProfile.PlayerLeagueStar;
        JsonDict parameters = new JsonDict();
        parameters.Set("username", UserManager.Instance.currentAccount.Username);
        parameters.Set("star", playerProfileStar >= 0 ? totalStar.ToString() : (-(playerProfileStar - totalStar)).ToString());
        if (IsUnlocked())
        {
            parameters.Set("weekly_star", playerProfileLeagueStar >= 0 ? totalStar.ToString() : (-(playerProfileLeagueStar - totalStar)).ToString());
        }
        else
        {
            parameters.Set("weekly_star", 0.ToString());
        }
        parameters.Set("player_level", playerLevel.ToString());
        parameters.Set("is_half_mile", isHalfMile.ToString());
        parameters.Set("race_time", raceTime.ToString(CultureInfo.InvariantCulture));
        parameters.Set("nought100_time", noght100.ToString(CultureInfo.InvariantCulture));
        parameters.Set("selected_car", currentCar);
        //  parameters.Set("current_league_id", UserManager.Instance.currentAccount.CurrentLeagueProp);
        WebRequestQueue.Instance.StartCall("rtw_submit_race_reward_Ver_02", "submit race reward", parameters, null, null,
            null);
    }
}
