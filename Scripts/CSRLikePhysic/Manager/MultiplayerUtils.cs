using System;
using System.Collections.Generic;
using KingKodeStudio;
using UnityEngine;

public static class MultiplayerUtils
{
	public const int UnlockConsumablesAfterBestStreakOf = 4;

	public static bool InPrizeCarSequence = false;

	public static MultiplayerMode SelectedMultiplayerMode = MultiplayerMode.NONE;

	public static bool DisableMultiplayer = false;

	private static bool _fakeAComsumableActive = false;

	private static ScreenID[] _playingMultiplayerScreens = new ScreenID[]
	{
		ScreenID.PlayerList,
		ScreenID.VSDummy,
		ScreenID.RespectRanking,
		ScreenID.Streak,
		ScreenID.PrizeOMatic,
		ScreenID.PrizePieceGive,
		ScreenID.SportsPack,
		ScreenID.PrizeList,
		ScreenID.SeasonInfo,
		ScreenID.SeasonIntro,
		ScreenID.SeasonResult,
		ScreenID.SeasonPrize,
		ScreenID.CarSeasonPrize,
		ScreenID.SeasonRPChange,
		ScreenID.MultiplayerModeSelect
	};

	public static bool GarageInMultiplayerMode
	{
		get;
		set;
	}

	public static bool FakeNoComsumablesActive
	{
		get;
		set;
	}

	public static bool FakeAComsumableActive
	{
		get
		{
			return MultiplayerUtils._fakeAComsumableActive;
		}
		set
		{
			MultiplayerUtils._fakeAComsumableActive = value;
			MultiplayerUtils.FakeComsumable = (eCarConsumables)UnityEngine.Random.Range(0, 3);
		}
	}

	public static eCarConsumables FakeComsumable
	{
		get;
		private set;
	}

	public static void SetUpReplayData(PlayerReplay zPlayerReplay)
	{
		CompetitorManager.Instance.Clear();
		LocalCompetitor zCompetitor;
		if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
		{
			zCompetitor = new LocalCompetitorRYF();
		}
		else
		{
			zCompetitor = new LocalCompetitorRTW();
		}
		CompetitorManager.Instance.AddCompetitor(zCompetitor);
		NetworkCompetitor networkCompetitor = new NetworkCompetitor();
		networkCompetitor.LoadReplay(zPlayerReplay);
		CompetitorManager.Instance.AddCompetitor(networkCompetitor);
		RacePlayerInfoComponent component = networkCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
		RaceEventInfo.Instance.OpponentCarDBKey = component.CarDBKey;
		RaceEventInfo.Instance.OpponentCarElite = component.IsEliteCar;
		CarUpgradeSetup carUpgradeSetup = new CarUpgradeSetup();
		carUpgradeSetup.CarDBKey = component.CarDBKey;
		carUpgradeSetup.UpgradeStatus = new Dictionary<eUpgradeType, CarUpgradeStatus>(component.CarUpgradeStatus);
		if (networkCompetitor.PlayerInfo.DoesComponentExist<ConsumablePlayerInfoComponent>())
		{
			ConsumablePlayerInfoComponent component2 = networkCompetitor.PlayerInfo.GetComponent<ConsumablePlayerInfoComponent>();
			carUpgradeSetup.EngineConsumableActive = (component2.ConsumableEngine == 1);
			carUpgradeSetup.NitrousConsumableActive = (component2.ConsumableN2O == 1);
			carUpgradeSetup.TyreConsumableActive = (component2.ConsumableTyre == 1);
		}
		if (networkCompetitor.PlayerInfo.DoesComponentExist<MechanicPlayerInfoComponent>())
		{
			MechanicPlayerInfoComponent component3 = networkCompetitor.PlayerInfo.GetComponent<MechanicPlayerInfoComponent>();
			carUpgradeSetup.IsFettled = component3.MechanicEnabled;
		}
		RaceEventInfo.Instance.OpponentCarUpgradeSetup = carUpgradeSetup;
	}

	public static void RaceTheWorld(PlayerReplay zPlayerReplay)
	{
		MultiplayerUtils.SetUpReplayData(zPlayerReplay);
		SceneManagerFrontend.ButtonStart();
	}

	public static bool IsMultiplayerUnlocked()
	{
		if (GameDatabase.Instance.CareerConfiguration.LockMultiplayerForNewUsers)
		{
			return false;
		}
		eCarTier tierToUnlockMultiplayer = GameDatabase.Instance.CareerConfiguration.TierToUnlockMultiplayer;
		return RaceEventQuery.Instance.IsTierUnlocked(tierToUnlockMultiplayer);
	}

	public static bool IsMultiplayerOnline()
	{
		return BasePlatform.ActivePlatform.GetReachability() != BasePlatform.eReachability.OFFLINE && RTWStatusManager.NetworkStateValidToEnterRTW();
	}

	public static bool IsServerUnavailable()
	{
        WebClient.HttpStatusCode lastStatus = (WebClient.HttpStatusCode)RTWStatusManager.Instance.LastStatus;
        return lastStatus > WebClient.HttpStatusCode.HTTP_STATUS_CLIENT_ERROR && lastStatus != WebClient.HttpStatusCode.HTTP_STATUS_DATA_INCOMPLETE;
	}

    public static bool IsAppOutOfDate()
	{
		return AssetSystemManager.Instance.IsClientOutOfDate;
	}

	public static bool IsMultiplayerObjectiveActive()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		return (MultiplayerUtils.IsMultiplayerUnlocked() && !activeProfile.HasPlayedMultiplayer) || activeProfile.OnlineRacesWon == 0;
	}

	public static bool IsMultiplayerObjectiveComplete()
	{
		return PlayerProfileManager.Instance.ActiveProfile.OnlineRacesWon > 0;
	}

	public static bool IsPlayingMultiplayer()
	{
		if (SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Race)
		{
			return RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent();
		}
		if (SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend)
		{
			for (int i = 0; i < MultiplayerUtils._playingMultiplayerScreens.Length; i++)
			{
				if (ScreenManager.Instance.IsScreenOnStack(MultiplayerUtils._playingMultiplayerScreens[i]))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool HasNeverTakenAMultiplayerRace()
	{
		return PlayerProfileManager.Instance.ActiveProfile.OnlineRacesWon == 0 && PlayerProfileManager.Instance.ActiveProfile.OnlineRacesLost == 0;
	}

	public static bool HasPlayedAtLeastOneMultiplayerRace()
	{
		return PlayerProfileManager.Instance.ActiveProfile.HasPlayedMultiplayer;
	}

	private static ScreenID GetMultiplayerHubScreenID()
	{
		switch (MultiplayerUtils.SelectedMultiplayerMode)
		{
		case MultiplayerMode.RACE_THE_WORLD:
		case MultiplayerMode.PRO_CLUB:
			return ScreenID.MultiplayerHub;
		case MultiplayerMode.EVENT:
			return ScreenID.MultiplayerEvent;
		default:
			return ScreenID.Invalid;
		}
	}

	public static void GoToMultiplayerHubScreen()
	{
		ScreenManager.Instance.PopToScreen(ScreenID.Home);
		ScreenManager.Instance.PushScreen(ScreenID.Workshop);
		ScreenManager.Instance.PushScreen(ScreenID.MultiplayerModeSelect);
		ScreenID multiplayerHubScreenID = MultiplayerUtils.GetMultiplayerHubScreenID();
		if (multiplayerHubScreenID != ScreenID.Invalid)
		{
			ScreenManager.Instance.PushScreen(multiplayerHubScreenID);
		}
	}

	public static void GoToMultiplayerHubScreenWhenInFrontend()
	{
		ScreenID multiplayerHubScreenID = MultiplayerUtils.GetMultiplayerHubScreenID();
		if (multiplayerHubScreenID == ScreenID.Invalid)
		{
			SceneManagerFrontend.SetScreenToPushWhenInFrontend(ScreenID.MultiplayerModeSelect);
		}
		else
		{
			ScreenID[] zScreenHistory = new ScreenID[]
			{
				ScreenID.MultiplayerModeSelect
			};
			SceneManagerFrontend.SetScreenToPushWhenInFrontend(multiplayerHubScreenID, zScreenHistory);
		}
	}

	public static void OfferStreakChainFrontend()
	{
		MultiplayerUtils.GoToMultiplayerHubScreen();
		StreakManager.Chain.OfferStreakChainIfAvailable(delegate
		{
			ScreenManager.Instance.SwapScreen(ScreenID.PlayerList);
		}, delegate
		{
			ScreenManager.Instance.PopScreen();
		});
	}

	public static Color GetMultiplayerColor()
	{
        MultiplayerMode selectedMultiplayerMode = MultiplayerUtils.SelectedMultiplayerMode;
        Color primary;
        if (selectedMultiplayerMode != MultiplayerMode.PRO_CLUB)
        {
            if (selectedMultiplayerMode != MultiplayerMode.EVENT)
            {
                primary = StreakManager.StreakData.RaceTheWorldInfo.Theme.GetSwatch().Primary;
            }
            else
            {
                primary = MultiplayerEvent.Saved.Data.Theme.GetSwatch().Primary;
            }
        }
        else
        {
            primary = StreakManager.StreakData.EliteClubInfo.Theme.GetSwatch().Primary;
        }
		return primary;
	}

	public static string GetMultiplayerStreakType()
	{
		switch (MultiplayerUtils.SelectedMultiplayerMode)
		{
		case MultiplayerMode.RACE_THE_WORLD:
			return "0";
		case MultiplayerMode.PRO_CLUB:
			return "1";
		case MultiplayerMode.EVENT:
			return "2";
		default:
			return "NA";
		}
	}

	public static RaceEventData GetEventData(MultiplayerMode mode)
	{
		if (mode != MultiplayerMode.PRO_CLUB)
		{
			return GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.RaceTheWorldEvents.RaceEventGroups[0].RaceEvents[0];
		}
		return GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.OnlineClubRacingEvents.RaceEventGroups[0].RaceEvents[0];
	}
}
