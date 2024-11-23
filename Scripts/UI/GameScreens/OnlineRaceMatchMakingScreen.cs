using KingKodeStudio;
using UnityEngine;

public class OnlineRaceMatchMakingScreen : ZHUDScreen
{
    public static OnlineRace SelectedOnlineRace;
    private float m_setupTime;
    private float m_searchingTime;
    private bool m_findingOpponent;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.OnlineRaceMatchMaking;
        }
    }

    public override void OnActivate(bool zAlreadyOnStack)
    {
        base.OnActivate(zAlreadyOnStack);
        m_searchingTime = GameDatabase.Instance.Online.GetRandomSearchTime();
        if (!zAlreadyOnStack)
        {
            var onlineRacingEvent = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.SMPRaceEvents;
            var raceEventData = RaceEventQuery.Instance.GetOnlineRaceEvent(onlineRacingEvent,
                SelectedOnlineRace.EventIndex);
            var currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
            var baseTier = CarDatabase.Instance.GetCar(currentCar.CarDBKey).BaseCarTier;
            raceEventData.UpgradePercentage = -1;

            //Use these lines later to setup opponent as netwrokplayer
            ////string replay_data = jsonDict.GetString("replay_data");
            //PlayerReplay playerReplay = new PlayerReplay(new RTWPlayerInfo());// PlayerReplay.CreateFromJson(replay_data, null);
            //MultiplayerUtils.SetUpReplayData(playerReplay);
            AutoDifficulty.GetRandomOpponentForCarAtDifficultyOnline(SelectedOnlineRace, ref raceEventData, currentCar);
            RaceEventInfo.Instance.PopulateFromRaceEvent(raceEventData);
            var eventOrder = RaceEventInfo.Instance.CurrentEvent.EventOrder;
            var stakeReward = GameDatabase.Instance.Online.GetStake(eventOrder, baseTier);
            CommonUI.Instance.CashStats.CashLockedState(true);
            PlayerProfileManager.Instance.ActiveProfile.SpendCash(stakeReward,"reward","stake");
            //LogUtility.Log("Selected Opponent PPI:"+raceEventData.AIPerformancePotentialIndex);
            m_setupTime = Time.time;
            m_findingOpponent = true;
        }
    }


    protected override void Update()
    {
        if (Time.time - m_setupTime > m_searchingTime && m_findingOpponent)
        {
            m_findingOpponent = false;
            if (!PolledNetworkState.IsNetworkConnected || !ServerSynchronisedTime.Instance.ServerTimeValid)
            {
                PopUpDatabase.Common.ShowNoInternetConnectionPopup(OnDisconnectedFromServer);
            }
            else
            {
                ScreenManager.Instance.PushScreen(ScreenID.OnlineVs);
            }
        }
    }

    private void OnDisconnectedFromServer()
    {
        if (ScreenManager.Instance.IsScreenOnStack(ScreenID.CareerModeMap))
        {
            ScreenManager.Instance.PopToScreen(ScreenID.CareerModeMap);
        }
        else
        {
            ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.CareerModeMap,
                new ScreenID[] { ScreenID.Home,ScreenID.Workshop });
        }
    }
}
