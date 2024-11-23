//using DataSerialization;

using System;
using System.Collections.Generic;
using DataSerialization;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;
using Z2HSharedLibrary.DatabaseEntity;

public class RaceEventInfo
{
    public enum RaceTrack
    {
        Night,
        Day,
        CloudyDay,
        Airport
    }

	public static RaceEventInfo Instance;

	private HashSet<string> m_SeasonCars = new HashSet<string>();

    private RaceTrack? m_raceTrack;

	public string LocalPlayerCarDBKey
	{
		get;
		set;
	}

	public bool LocalPlayerCarElite
	{
		get;
		set;
	}

	public string OpponentCarDBKey
	{
		get;
		set;
	}

	public bool OpponentCarElite
	{
		get;
		set;
	}

	public AIDriverData AIDriverData
	{
		get;
		private set;
	}

	public CarGarageInstance AICarGarageInstance
	{
		get;
		set;
	}

	public CarGarageInstance HumanCarGarageInstance
	{
		get;
		set;
	}

	public RaceEventData CurrentEvent
	{
		get;
		private set;
	}

	public eCarTier CurrentEventTier
	{
		get;
		private set;
	}

	public CarUpgradeSetup OpponentCarUpgradeSetup
	{
		get;
		set;
	}

	public CarUpgradeSetup LocalPlayerCarUpgradeSetup
	{
		get;
		set;
	}


	public bool IsDailyBattleEvent
	{
		get;
		private set;
	}


	public bool IsRegulationRaceEvent
	{
		get;
		private set;
	}

	public bool IsCrewRaceEvent
	{
		get;
		private set;
	}

	public bool IsHighStakesEvent
	{
		get;
		private set;
	}

	public bool IsWorldTourEvent
	{
		get;
		private set;
	}

	public bool IsWorldTourLoanEvent
	{
		get;
		private set;
	}

    public bool IsSMPEvent
    {
        get;
        private set;
    }

	public float RaceDistanceMetres
	{
		get;
		private set;
	}

	public bool IsEngineConsumableActive
	{
		get;
		private set;
	}

	public bool IsPRConsumableActive
	{
		get;
		private set;
	}

	public bool IsNitrousConsumableActive
	{
		get;
		private set;
	}

	public bool IsTyresConsumableActive
	{
		get;
		private set;
	}

	public HashSet<string> SeasonCars
	{
		get
		{
			return m_SeasonCars;
		}
		set
		{
			m_SeasonCars = value;
		}
	}

    public RaceDetails.RaceType RaceType
    {
        get
        {
            if (IsRegulationRaceEvent)
                return RaceDetails.RaceType.OneByOne;
            if (IsDailyBattleEvent)
                return RaceDetails.RaceType.Daily;
            return RaceDetails.RaceType.OneByOne;
        }
    }

    //public string OpponentName { get; set; }

    //public string OpponentImageUrl { get; set; }
    //public long OpponentID { get; set; }

    public static void Create()
	{
		if (Instance == null)
		{
			Instance = new RaceEventInfo();
			Instance.LocalPlayerCarDBKey = "FordFocusST";
			Instance.LocalPlayerCarElite = false;
			Instance.OpponentCarDBKey = "FordFocusST";
			Instance.OpponentCarElite = false;
			Instance.AIDriverData = GameDatabase.Instance.AIPlayers.DefaultAISetupData;
			Instance.CurrentEvent = new RaceEventData();
			Instance.RaceDistanceMetres = 402.325f;
            Instance.m_raceTrack = null;
		    //Instance.CurrentEvent = null;
		    //LogUtility.Log("here");
		}
	}

	public void RefreshRaceEvent()
	{
		if (CurrentEvent != null && !CurrentEvent.IsRaceTheWorldOrClubRaceEvent() && !CurrentEvent.IsTutorial())
		{
			PopulateFromRaceEvent(CurrentEvent, eCarTier.BASE_EVENT_TIER, false);
		}
	}

	public void PopulateFromRaceEvent(RaceEventData zRaceEventData)
	{
		PopulateFromRaceEvent(zRaceEventData, eCarTier.BASE_EVENT_TIER, true);
	}

    #region Off-For-Test
    //public void PopulateFromRaceEvent(RaceEventData zRaceEventData, eCarTier carTier, bool setPlayerCar = true)
    //{
    //    PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
    //    OpponentCarDBKey = string.Empty;
    //    OpponentCarElite = false;
    //    LocalPlayerCarElite = false;
    //    OpponentCarUpgradeSetup = null;
    //    IsEngineConsumableActive = false;
    //    IsPRConsumableActive = false;
    //    IsNitrousConsumableActive = false;
    //    IsTyresConsumableActive = false;
    //    string humanCar = zRaceEventData.GetHumanCar();
    //    if (!string.IsNullOrEmpty(humanCar) && setPlayerCar && activeProfile.IsCarOwned(humanCar))
    //    {
    //        activeProfile.CurrentlySelectedCarDBKey = humanCar;
    //        activeProfile.UpdateCurrentCarSetup();
    //        activeProfile.UpdateCurrentPhysicsSetup();
    //        //CarInfoUI.Instance.SetCurrentCarIDKey(humanCar);
    //    }
    //    CompetitorManager.Instance.Clear();
    //    SetEventType(zRaceEventData);
    //    if (zRaceEventData.IsRaceTheWorldOrClubRaceEvent())
    //    {
    //        PopulateForOnlineRace(zRaceEventData);
    //        return;
    //    }
    //    if (zRaceEventData.IsFriendRaceEvent())
    //    {
    //        PopulateForFriendRace(zRaceEventData);
    //        return;
    //    }
    //    CurrentEvent = zRaceEventData;
    //    if (carTier == eCarTier.BASE_EVENT_TIER)
    //    {
    //        var tierEvent = CurrentEvent.Parent.GetTierEvents();
    //        CurrentEventTier = tierEvent != null ? tierEvent.GetCarTier() : eCarTier.TIER_1;
    //    }
    //    RaceDistanceMetres = ((!zRaceEventData.IsHalfMile) ? 500 : 1000);
    //    LocalPlayerCarDBKey = activeProfile.CurrentlySelectedCarDBKey;
    //    LocalPlayerCarElite = activeProfile.GetCurrentCar().EliteCar;
    //    LocalPlayerCarUpgradeSetup = activeProfile.GetCurrentCarUpgradeSetup();
    //    CompetitorManager.Instance.AddCompetitor(eRaceCompetitorType.LOCAL_COMPETITOR);
    //    if (zRaceEventData.AIDriver != string.Empty && zRaceEventData.AICar != string.Empty)
    //    {
    //        OpponentCarUpgradeSetup = zRaceEventData.GetAICarUpgradeSetup();
    //        OpponentCarDBKey = zRaceEventData.AICar;
    //        if (CurrentEventTier == eCarTier.TIER_X)
    //        {
    //            ChooseMatchedAIDriverIfAppropriate(zRaceEventData.AICar, zRaceEventData);
    //        }
    //        else
    //        {
    //            AIDriverData = GameDatabase.Instance.AIPlayers.GetAIDriverData(zRaceEventData.AIDriver);
    //        }
    //    }
    //    if (zRaceEventData.IsTestDriveAndCarSetup() || (zRaceEventData.SwitchBackRace && IsFirstOfSwitchBackRace()))
    //    {
    //        LocalPlayerCarDBKey = zRaceEventData.LoanCarGarageInstance.CarDBKey;
    //        LocalPlayerCarUpgradeSetup = zRaceEventData.LoanCarUpgradeSetup;
    //    }
    //    else if (IsDailyBattleEvent)
    //    {
    //        CarUpgradeSetup carUpgradeSetup = null;
    //        string dailyBattleCarKey = string.Empty;
    //        string dailybattleAiDriver = string.Empty;
    //        bool flag = PlayerProfileManager.Instance.ActiveProfile.DailyBattlesLastEventAt == DateTime.MinValue;
    //        if (flag)
    //        {
    //            TutorialConfiguration tutorialConfiguration = GameDatabase.Instance.TutorialConfiguration;
    //            dailyBattleCarKey = tutorialConfiguration.FirstDailyBattleCar;
    //            carUpgradeSetup = new CarUpgradeSetup();
    //            carUpgradeSetup.UpgradeStatus[eUpgradeType.BODY].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Body;
    //            carUpgradeSetup.UpgradeStatus[eUpgradeType.ENGINE].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Engine;
    //            carUpgradeSetup.UpgradeStatus[eUpgradeType.INTAKE].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Intake;
    //            carUpgradeSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Nitrous;
    //            carUpgradeSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Transmission;
    //            carUpgradeSetup.UpgradeStatus[eUpgradeType.TURBO].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Turbo;
    //            carUpgradeSetup.UpgradeStatus[eUpgradeType.TYRES].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Tyres;
    //            LocalPlayerCarDBKey = dailyBattleCarKey;
    //            LocalPlayerCarUpgradeSetup = carUpgradeSetup;
    //            dailybattleAiDriver = tutorialConfiguration.FirstDailyBattleCarAIDriver;
    //        }
    //        else
    //        {
    //            PickRandomCarAndUpgradeSet(ref carUpgradeSetup, ref dailyBattleCarKey);
    //            LocalPlayerCarDBKey = dailyBattleCarKey;
    //            LocalPlayerCarUpgradeSetup = carUpgradeSetup;
    //        }
    //        if (zRaceEventData.AICar != null)
    //        {
    //            OpponentCarDBKey = dailyBattleCarKey;
    //            OpponentCarUpgradeSetup = carUpgradeSetup;
    //            OpponentCarUpgradeSetup.CarDBKey = dailyBattleCarKey;
    //            if (!string.IsNullOrEmpty(dailybattleAiDriver))
    //            {
    //                AIDriverData = GameDatabase.Instance.AIPlayers.GetAIDriverData(dailybattleAiDriver);
    //            }
    //            else
    //            {
    //                ChooseMatchedAIDriverIfAppropriate(OpponentCarDBKey, zRaceEventData);
    //            }
    //        }
    //        activeProfile.RaceDailyBattle();
    //        int num = PlayerProfileManager.Instance.ActiveProfile.DailyBattlesConsecutiveDaysCount;
    //        if (PlayerProfileManager.Instance.ActiveProfile.DailyBattlesDoneToday == 0)
    //        {
    //            num++;
    //        }
    //        DailyBattleReward reward = DailyBattleRewardManager.Instance.GetReward(num, RaceEventQuery.Instance.getHighestUnlockedClass(), true);
    //        zRaceEventData.RaceReward.GoldPrize = ((reward.RewardType != DailyBattleRewardType.Gold) ? 0 : reward.RewardValue);
    //        zRaceEventData.RaceReward.CashPrize = ((reward.RewardType != DailyBattleRewardType.Cash) ? 0 : reward.RewardValue);
    //    }
    //    else if (IsRegulationRaceEvent && CurrentEventTier != eCarTier.TIER_X)
    //    {
    //        //ChooseRandomCars chooseRandomCars = new ChooseRandomCars();
    //        //string text3 = chooseRandomCars.ChooseRegulationRaceCar(CurrentEventTier, zRaceEventData.IsHalfMile);
    //        //if (string.IsNullOrEmpty(zRaceEventData.AICar))
    //        //{
    //        //    //ChooseMatchedAIDriverIfAppropriate(text3, zRaceEventData);
    //        //    OpponentCarDBKey = text3;
    //        //    OpponentCarUpgradeSetup.CarDBKey = text3;
    //        //}
    //        ChooseMatchedAIDriverIfAppropriate(OpponentCarDBKey, zRaceEventData);
    //    }

    //    //Debug.Log(AIDriverData);
    //    if (zRaceEventData.AICar != string.Empty)
    //    {
    //        AICompetitor zCompetitor = new AICompetitor();
    //        CompetitorManager.Instance.AddCompetitor(zCompetitor);
    //    }
    //}
    #endregion


    public void PopulateFromRaceEvent(RaceEventData zRaceEventData, eCarTier carTier, bool setPlayerCar = true)
    {
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        this.OpponentCarDBKey = string.Empty;
        this.OpponentCarElite = false;
        this.LocalPlayerCarElite = false;
        this.OpponentCarUpgradeSetup = null;
        this.IsEngineConsumableActive = false;
        this.IsPRConsumableActive = false;
        this.IsNitrousConsumableActive = false;
        this.IsTyresConsumableActive = false;
        string humanCar = zRaceEventData.GetHumanCar();
        if (!string.IsNullOrEmpty(humanCar) && setPlayerCar && activeProfile.IsCarOwned(humanCar))
        {
            activeProfile.CurrentlySelectedCarDBKey = humanCar;
            activeProfile.UpdateCurrentCarSetup();
            activeProfile.UpdateCurrentPhysicsSetup();
            CarInfoUI.Instance.SetCurrentCarIDKey(humanCar);
        }
        CompetitorManager.Instance.Clear();
        this.SetEventType(zRaceEventData);
        if (zRaceEventData.IsRaceTheWorldOrClubRaceEvent())
        {
            this.PopulateForOnlineRace(zRaceEventData);
            return;
        }
        if (zRaceEventData.IsSMPRaceEvent())
        {
            this.PopulateForStakeOnlineRace(zRaceEventData);
            return;
        }
        if (zRaceEventData.IsFriendRaceEvent())
        {
            this.PopulateForFriendRace(zRaceEventData);
            return;
        }
        this.CurrentEvent = zRaceEventData;
        Instance.m_raceTrack = null;

        if (carTier == eCarTier.BASE_EVENT_TIER)
        {
            this.CurrentEventTier = this.CurrentEvent.Parent.GetTierEvents().GetCarTier();
        }
        this.RaceDistanceMetres = ((!zRaceEventData.IsHalfMile) ? 402.325f : 804.65f);
        this.LocalPlayerCarDBKey = activeProfile.CurrentlySelectedCarDBKey;
        this.LocalPlayerCarElite = activeProfile.GetCurrentCar().EliteCar;
        this.LocalPlayerCarUpgradeSetup = activeProfile.GetCurrentCarUpgradeSetup();
        CompetitorManager.Instance.AddCompetitor(eRaceCompetitorType.LOCAL_COMPETITOR);
        if (Instance.CurrentEvent.EventID == GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.CrewBattleEvents.RaceEventGroups[0].RaceEvents[0].EventID)
        {
	        if(GameDatabase.Instance.TutorialConfiguration.IsOn)
		        IngameTutorial.Instance.StartTutorial(IngameTutorial.TutorialPart.Tutorial2);
        }
        if (zRaceEventData.AIDriver != string.Empty && zRaceEventData.AICar != string.Empty)
        {
            this.OpponentCarUpgradeSetup = zRaceEventData.GetAICarUpgradeSetup();
            this.OpponentCarDBKey = zRaceEventData.AICar;
            if (this.CurrentEventTier == eCarTier.TIER_X)
            {
                this.ChooseMatchedAIDriverIfAppropriate(zRaceEventData.AICar, zRaceEventData);
            }
            else
            {
	            var aiDriver = string.Empty;
	            if (GameDatabase.Instance.TutorialConfiguration.IsOn)
	            {
		            if (Instance.CurrentEvent.IsCrewBattle())
		            {
			            if (Instance.CurrentEvent.EventID == GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.CrewBattleEvents.RaceEventGroups[0].RaceEvents[0].EventID)
			            {
				            if (PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstUpgrade)
				            {
					            if(GameDatabase.Instance.TutorialConfiguration.IsOn)
									aiDriver = "baddriver";
				            }
			            }
		            }
	            }
	            AIDriverData =
		            GameDatabase.Instance.AIPlayers.GetAIDriverData(string.IsNullOrEmpty(aiDriver)
			            ? zRaceEventData.AIDriver
			            : aiDriver);
            }
        }
        if (zRaceEventData.IsTestDriveAndCarSetup() || (zRaceEventData.SwitchBackRace && this.IsFirstOfSwitchBackRace()))
        {
            this.LocalPlayerCarDBKey = zRaceEventData.LoanCarGarageInstance.CarDBKey;
            this.LocalPlayerCarUpgradeSetup = zRaceEventData.LoanCarUpgradeSetup;
        }
        else if (this.IsDailyBattleEvent)
        {
            CarUpgradeSetup carUpgradeSetup = null;
            string text = string.Empty;
            string text2 = string.Empty;
            bool isTutotrialDailyBattle = PlayerProfileManager.Instance.ActiveProfile.DailyBattlesLastEventAt == DateTime.MinValue;
            if (isTutotrialDailyBattle)
            {
                TutorialConfiguration tutorialConfiguration = GameDatabase.Instance.TutorialConfiguration;
                text = tutorialConfiguration.FirstDailyBattleCar;
                carUpgradeSetup = new CarUpgradeSetup();
                carUpgradeSetup.UpgradeStatus[eUpgradeType.BODY].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Body;
                carUpgradeSetup.UpgradeStatus[eUpgradeType.ENGINE].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Engine;
                carUpgradeSetup.UpgradeStatus[eUpgradeType.INTAKE].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Intake;
                carUpgradeSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Nitrous;
                carUpgradeSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Transmission;
                carUpgradeSetup.UpgradeStatus[eUpgradeType.TURBO].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Turbo;
                carUpgradeSetup.UpgradeStatus[eUpgradeType.TYRES].levelFitted = tutorialConfiguration.FirstDailyBattleCarUpgrades.Tyres;
                this.LocalPlayerCarDBKey = text;
                this.LocalPlayerCarUpgradeSetup = carUpgradeSetup;
                text2 = tutorialConfiguration.FirstDailyBattleCarAIDriver;
            }
            else
            {
                this.PickRandomCarAndUpgradeSet(ref carUpgradeSetup, ref text);
                this.LocalPlayerCarDBKey = text;
                this.LocalPlayerCarUpgradeSetup = carUpgradeSetup;
            }
            if (zRaceEventData.AICar != null)
            {
                this.OpponentCarDBKey = text;
                this.OpponentCarUpgradeSetup = carUpgradeSetup;
                this.OpponentCarUpgradeSetup.CarDBKey = text;
                if (!string.IsNullOrEmpty(text2))
                {
                    this.AIDriverData = GameDatabase.Instance.AIPlayers.GetAIDriverData(text2);
                }
                else
                {
                    this.ChooseMatchedAIDriverIfAppropriate(this.OpponentCarDBKey, zRaceEventData);
                }
            }
            activeProfile.RaceDailyBattle();
            int num = PlayerProfileManager.Instance.ActiveProfile.DailyBattlesConsecutiveDaysCount;
            if (PlayerProfileManager.Instance.ActiveProfile.DailyBattlesDoneToday == 0)
            {
                num++;
            }
            DailyBattleReward reward = DailyBattleRewardManager.Instance.GetReward(num, RaceEventQuery.Instance.getHighestUnlockedClass(), true);
            zRaceEventData.RaceReward.GoldPrize = ((reward.RewardType != DailyBattleRewardType.Gold) ? 0 : reward.RewardValue);
            zRaceEventData.RaceReward.CashPrize = ((reward.RewardType != DailyBattleRewardType.Cash) ? 0 : reward.RewardValue);
        }
        else if (this.IsRegulationRaceEvent && this.CurrentEventTier != eCarTier.TIER_X)
        {
            #region CSRRacing3.3.1
            ////ChooseRandomCars chooseRandomCars = new ChooseRandomCars();
            ////string randomCarKey = chooseRandomCars.ChooseRegulationRaceCar(this.CurrentEventTier, zRaceEventData.IsHalfMile);
            //if (zRaceEventData.AICar != string.Empty)
            //{
            //    this.ChooseMatchedAIDriverIfAppropriate(/*randomCarKey*/zRaceEventData.AICar, zRaceEventData);
            //    //this.OpponentCarDBKey = randomCarKey;
            //    //this.OpponentCarUpgradeSetup.CarDBKey = randomCarKey;
            //}
            #endregion

            #region CsrRacing 5.0.1
            int aIPerformancePotentialIndex = zRaceEventData.AIPerformancePotentialIndex;
            List<AutoDifficulty.AutoDifficultyCarUpgradeSetup> opponentsForCarAtPP =
                AutoDifficulty.GetOpponentsForCarAtPP(aIPerformancePotentialIndex, activeProfile.GetCurrentCar());
            int index = UnityEngine.Random.Range(0, opponentsForCarAtPP.Count - 1);
            AutoDifficulty.AutoDifficultyCarUpgradeSetup autoDifficultyCarUpgradeSetup = opponentsForCarAtPP[index];
            OpponentCarDBKey = autoDifficultyCarUpgradeSetup.UpgradeSet.CarDBKey;
            OpponentCarUpgradeSetup = autoDifficultyCarUpgradeSetup.UpgradeSet;
            ChooseMatchedAIDriverIfAppropriate(OpponentCarDBKey, zRaceEventData);
            #endregion
        }
        if (zRaceEventData.AICar != string.Empty)
        {
            AICompetitor zCompetitor = new AICompetitor();
            CompetitorManager.Instance.AddCompetitor(zCompetitor);
        }
    }

    public void PopulateForOnlineRace(RaceEventData zRaceEventData)
	{
        Instance.m_raceTrack = null;

		RaceDistanceMetres = ((!zRaceEventData.IsHalfMile) ? 402.325f : 804.65f);
		CurrentEvent = zRaceEventData;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		string currentlySelectedCarDBKey = activeProfile.CurrentlySelectedCarDBKey;
		CurrentEventTier = activeProfile.PlayerPhysicsSetup.BaseCarTier;
		LocalPlayerCarDBKey = currentlySelectedCarDBKey;
		LocalPlayerCarElite = activeProfile.GetCurrentCar().EliteCar;
		LocalPlayerCarUpgradeSetup = activeProfile.GetCurrentCarUpgradeSetup();
		IsEngineConsumableActive = activeProfile.IsConsumableActive(eCarConsumables.EngineTune);
		IsPRConsumableActive = activeProfile.IsConsumableActive(eCarConsumables.PRAgent);
		IsNitrousConsumableActive = activeProfile.IsConsumableActive(eCarConsumables.Nitrous);
		IsTyresConsumableActive = activeProfile.IsConsumableActive(eCarConsumables.Tyre);
	}


    public void PopulateForStakeOnlineRace(RaceEventData zRaceEventData)
    {
        Instance.m_raceTrack = null;

        var humanCarTier = CarDatabase.Instance.GetCar(PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().CarDBKey).BaseCarTier;
        RaceDistanceMetres = humanCarTier < eCarTier.TIER_4 ? 402.325f : 804.65f;
        zRaceEventData.IsHalfMile = humanCarTier >= eCarTier.TIER_4;
        CurrentEvent = zRaceEventData;
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        string currentlySelectedCarDBKey = activeProfile.CurrentlySelectedCarDBKey;
        CurrentEventTier = activeProfile.PlayerPhysicsSetup.BaseCarTier;
        LocalPlayerCarDBKey = currentlySelectedCarDBKey;
        LocalPlayerCarElite = activeProfile.GetCurrentCar().EliteCar;
        LocalPlayerCarUpgradeSetup = activeProfile.GetCurrentCarUpgradeSetup();
        IsEngineConsumableActive = activeProfile.IsConsumableActive(eCarConsumables.EngineTune);
        IsPRConsumableActive = activeProfile.IsConsumableActive(eCarConsumables.PRAgent);
        IsNitrousConsumableActive = activeProfile.IsConsumableActive(eCarConsumables.Nitrous);
        IsTyresConsumableActive = activeProfile.IsConsumableActive(eCarConsumables.Tyre);
        CompetitorManager.Instance.AddCompetitor(new LocalCompetitorRTW());
        if (zRaceEventData.AIDriver != string.Empty && zRaceEventData.AICar != string.Empty)
        {
            this.OpponentCarUpgradeSetup = zRaceEventData.GetAICarUpgradeSetup();
            this.OpponentCarDBKey = zRaceEventData.AICar;
            this.ChooseMatchedAIDriverIfAppropriate(zRaceEventData.AICar, zRaceEventData);
        }
        AICompetitor zCompetitor = new AICompetitor();
        CompetitorManager.Instance.AddCompetitor(zCompetitor);
    }

	public void PopulateForFriendRace(RaceEventData zRaceEventData)
	{
        Instance.m_raceTrack = null;

		RaceDistanceMetres = ((!zRaceEventData.IsHalfMile) ? 402.325f : 804.65f);
		CurrentEvent = zRaceEventData;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		string currentlySelectedCarDBKey = activeProfile.CurrentlySelectedCarDBKey;
		CurrentEventTier = activeProfile.PlayerPhysicsSetup.BaseCarTier;
		LocalPlayerCarDBKey = currentlySelectedCarDBKey;
		LocalPlayerCarElite = activeProfile.GetCurrentCar().EliteCar;
		LocalPlayerCarUpgradeSetup = activeProfile.GetCurrentCarUpgradeSetup();
	}

	private void SetEventType(RaceEventData raceEventData)
	{
	    IsSMPEvent = false;
		IsDailyBattleEvent = false;
		IsRegulationRaceEvent = false;
		IsCrewRaceEvent = false;
		IsWorldTourLoanEvent = false;
		IsHighStakesEvent = raceEventData.IsHighStakesEvent();
		if (raceEventData.Parent == null)
		{
			return;
		}
		RegulationRaceEvents regulationRaceEvents = raceEventData.Parent as RegulationRaceEvents;
		if (regulationRaceEvents != null)
		{
			IsRegulationRaceEvent = true;
		}
		DailyBattleEvents dailyBattleEvents = raceEventData.Parent as DailyBattleEvents;
		if (dailyBattleEvents != null)
		{
			IsDailyBattleEvent = true;
		}
		CrewBattleEvents crewBattleEvents = raceEventData.Parent as CrewBattleEvents;
		if (crewBattleEvents != null)
		{
			IsCrewRaceEvent = true;
		}
		WorldTourRaceEvents worldTourRaceEvents = raceEventData.Parent as WorldTourRaceEvents;
		if (worldTourRaceEvents != null)
		{
			IsWorldTourEvent = true;
		}
		if (raceEventData.IsTestDrive())
		{
			IsWorldTourLoanEvent = true;
		}
        if (raceEventData.IsSMPRaceEvent())
        {
            IsSMPEvent = true;
        }
	}

	private void PickRandomCarAndUpgradeSet(ref CarUpgradeSetup driverBattleUpgradeSetup, ref string chosenCarDBKey)
	{
		driverBattleUpgradeSetup = new CarUpgradeSetup();
		driverBattleUpgradeSetup.PickRandomUpgrades();
		ChooseRandomCars chooseRandomCars = new ChooseRandomCars();
		chosenCarDBKey = chooseRandomCars.ChooseDriverBattleCar(CurrentEventTier);
	}

	private void ChooseMatchedAIDriverIfAppropriate(string zCarDBKey, RaceEventData zRaceEventData)
	{
		string text = zRaceEventData.AIDriver.ToLower();
		if (text.Contains("matchcar"))
		{
			AIDriverData = GameDatabase.Instance.AIPlayers.GetSpecificCarDriver("CarSpecificDriver_" + zCarDBKey);
		}
		else if (text.Contains("expertdriver"))
		{
			AIDriverData = GameDatabase.Instance.AIPlayers.GetSpecificCarDriver("ExpertDriver_" + zCarDBKey);
		}
		else if (text.Contains("baddriver"))
		{
			AIDriverData = GameDatabase.Instance.AIPlayers.GetSpecificCarDriver("RubbishDriver");
		}
		else
		{
			AIDriverData = GameDatabase.Instance.AIPlayers.GetAIDriverData(zRaceEventData.AIDriver);
		}
	}

	public void PopulateForTutorial(RaceEventData zRaceEventData, string zPlayerCarOverride,
        CarUpgradeSetup carUpgradeSetup=null)
	{
        Instance.m_raceTrack = null;
	    IsSMPEvent = false;
		IsDailyBattleEvent = false;
		IsRegulationRaceEvent = false;
		IsCrewRaceEvent = false;
		IsWorldTourLoanEvent = false;
		CompetitorManager.Instance.Clear();
		CurrentEvent = zRaceEventData;
		string text = GameDatabase.Instance.TutorialConfiguration.TutorialCar;
		if (!string.IsNullOrEmpty(zPlayerCarOverride))
		{
			text = zPlayerCarOverride;
		}
	    if (carUpgradeSetup == null)
	    {
	        carUpgradeSetup = new CarUpgradeSetup();
	        carUpgradeSetup.UpgradeStatus[eUpgradeType.BODY].levelFitted = 0;
            carUpgradeSetup.UpgradeStatus[eUpgradeType.ENGINE].levelFitted = 0;
            carUpgradeSetup.UpgradeStatus[eUpgradeType.INTAKE].levelFitted = 0;
            carUpgradeSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted = 0;
            carUpgradeSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted = 0;
            carUpgradeSetup.UpgradeStatus[eUpgradeType.TURBO].levelFitted = 0;
            carUpgradeSetup.UpgradeStatus[eUpgradeType.TYRES].levelFitted = 0;
            carUpgradeSetup.CarDBKey = text;
	    }
		LocalPlayerCarUpgradeSetup = carUpgradeSetup;
		LocalPlayerCarDBKey = text;
		LocalPlayerCarElite = false;
		CompetitorManager.Instance.AddCompetitor(eRaceCompetitorType.LOCAL_COMPETITOR);
        if (CurrentEvent.AIDriver != string.Empty && CurrentEvent.AICar != string.Empty)
        {
            OpponentCarDBKey = CurrentEvent.AICar;
            OpponentCarElite = false;
            ChooseMatchedAIDriverIfAppropriate(OpponentCarDBKey, zRaceEventData);
            //AIDriverData = GameDatabase.Instance.AIPlayers.GetAIDriverData(CurrentEvent.AIDriver);
            CarUpgradeSetup carUpgradeSetup2 = new CarUpgradeSetup();
            carUpgradeSetup2.UpgradeStatus[eUpgradeType.BODY].levelFitted = CurrentEvent.BodyUpgradeLevel;
            carUpgradeSetup2.UpgradeStatus[eUpgradeType.ENGINE].levelFitted = CurrentEvent.EngineUpgradeLevel;
            carUpgradeSetup2.UpgradeStatus[eUpgradeType.INTAKE].levelFitted = CurrentEvent.IntakeUpgradeLevel;
            carUpgradeSetup2.UpgradeStatus[eUpgradeType.NITROUS].levelFitted = CurrentEvent.NitrousUpgradeLevel;
            carUpgradeSetup2.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted = CurrentEvent.TransmissionUpgradeLevel;
            carUpgradeSetup2.UpgradeStatus[eUpgradeType.TURBO].levelFitted = CurrentEvent.TurboUpgradeLevel;
            carUpgradeSetup2.UpgradeStatus[eUpgradeType.TYRES].levelFitted = CurrentEvent.TyreUpgradeLevel;
            carUpgradeSetup2.CarDBKey = OpponentCarDBKey;
            OpponentCarUpgradeSetup = carUpgradeSetup2;
            AICompetitor zCompetitor = new AICompetitor();
            CompetitorManager.Instance.AddCompetitor(zCompetitor);
        }
	}

	public string GetRivalName()
	{
	    if (Instance.CurrentEvent.IsWorldTourRace())
	    {
	        PinDetail worldTourPinPinDetail = Instance.CurrentEvent.GetWorldTourPinPinDetail();
	        if (worldTourPinPinDetail != null && worldTourPinPinDetail.WorldTourScheduledPinInfo != null)
	        {
	            PinSchedulerAIDriverOverrides aIDriverOverrides = worldTourPinPinDetail.WorldTourScheduledPinInfo.AIDriverOverrides;
	            string name = aIDriverOverrides.Name;
	            if (!string.IsNullOrEmpty(name))
	            {
	                if (name.StartsWith("TEXT_"))
	                {
	                    return LocalizationManager.GetTranslation(name);
	                }
	                return name;
	            }
	        }
	    }

        bool flag = CurrentEvent.IsRaceTheWorldOrClubRaceEvent() || CurrentEvent.IsFriendRaceEvent() || CurrentEvent.IsRandomRelay()
            || CurrentEvent.IsRegulationRace() || CurrentEvent.IsDailyBattle() || CurrentEvent.IsManufacturerSpecificEvent()
            || CurrentEvent.IsLadderEvent() || CurrentEvent.IsSMPRaceEvent() || CurrentEvent.IsWorldTourRace();
		if (flag)
		{
			RaceCompetitor otherCompetitor = CompetitorManager.Instance.OtherCompetitor;
			if (otherCompetitor == null)
			{
				return "Error";
			}
			return otherCompetitor.PlayerInfo.DisplayName;
		}
        return AIDriverData.GetDisplayName();
	}

    public bool IsWorldTourRaceHasOverrideDriver()
    {
        if (Instance.CurrentEvent.IsWorldTourRace())
        {
            PinDetail worldTourPinPinDetail = Instance.CurrentEvent.GetWorldTourPinPinDetail();
            if (worldTourPinPinDetail != null && worldTourPinPinDetail.WorldTourScheduledPinInfo != null)
            {
                PinSchedulerAIDriverOverrides aIDriverOverrides = worldTourPinPinDetail.WorldTourScheduledPinInfo.AIDriverOverrides;
                string name = aIDriverOverrides.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    return true;
                }
            }
        }

        return false;
    }

	public bool ShouldCurrentEventUseMechanic()
	{
		return CurrentEvent != null && CurrentEvent.IsMechanicAllowed();
	}

	public bool ShouldCurrentEventUseConsumables()
	{
		return CurrentEvent.IsRaceTheWorldOrClubRaceEvent();
	}

	public RaceTrack GetRaceTrack()
	{
	    if (m_raceTrack.HasValue)
	    {
            return m_raceTrack.Value;
	    }

	    var tracks = new[]
	    {
	        RaceTrack.Day,
	        RaceTrack.Day, //Chance = 2/9
	        RaceTrack.Night, 
	        RaceTrack.Night,
	        RaceTrack.Night, //Chance = 3/9
	        RaceTrack.CloudyDay,
	        RaceTrack.CloudyDay, //Chance = 2/9
            RaceTrack.Airport, 
            RaceTrack.Airport //Chance = 2/9
	    };

	    if (CurrentEvent.IsDailyBattle())
	    {
            m_raceTrack = RaceTrack.Day;
	    }
	    else if (CurrentEvent.IsLadderEvent())
	    {
		    var randomvalue = UnityEngine.Random.Range(0, tracks.Length-2); //-2 cause not including airport
		    m_raceTrack = tracks[randomvalue];
	    }
	    else if (CurrentEvent.IsRegulationRace())
	    {
	        var randomvalue = UnityEngine.Random.Range(0, tracks.Length-2); //-2 cause not including airport
	        m_raceTrack = tracks[randomvalue];
	    }
        else if (CurrentEvent.IsCrewBattle())
	    {
	        var randomvalue = UnityEngine.Random.Range(0, tracks.Length);
	        m_raceTrack = tracks[randomvalue];
        }
        else if (CurrentEvent.IsSMPRaceEvent())
        {
            m_raceTrack = RaceTrack.Airport;
        }
        else
        {
            m_raceTrack = RaceTrack.Night;
        }

	    return m_raceTrack.Value;
	}

	public bool ShouldCurrentEventUseMultiplayerLoadingScreen()
	{
		bool flag = CurrentEvent.IsRaceTheWorldOrClubRaceEvent() || CurrentEvent.IsFriendRaceEvent() || CurrentEvent.IsRelay;
        //if (CurrentEvent.GetWorldTourPinPinDetail() != null)
        //{
        //    flag |= (CurrentEvent.GetWorldTourPinPinDetail().GetLoadingScreen() == ScreenID.VSDummy);
        //}
		return flag;
	}

	public bool IsFirstOfSwitchBackRace()
	{
		return CurrentEvent != null && CurrentEvent.SwitchBackRace && CurrentEvent.Group.RaceEvents[0] == CurrentEvent;
	}

	public bool IsNonRestartable()
	{
		//TODO
	    return IsSMPEvent || IsHighStakesEvent || IsFirstCrewRaceEvent() || CurrentEvent.IsRaceTheWorldOrClubRaceEvent() || IsDailyBattleEvent || IsRegulationRaceEvent || (CurrentEvent.IsWorldTourRace() && !CurrentEvent.GetWorldTourPinPinDetail().WorldTourScheduledPinInfo.GetRootParentSequence().AllowRestarts);
	    //return false;
	}
	
	public bool IsNonPausable()
	{
		return IsSMPEvent;
	}

    public bool IsFirstCrewRaceEvent()
	{
		RaceEventDatabaseData careerRaceEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents;
		RaceEventData raceEventData = careerRaceEvents.Tier1.CrewBattleEvents.RaceEventGroups[0].RaceEvents[0];
		return Instance.CurrentEvent.EventID == raceEventData.EventID;
	}

	public bool IsRelayEvent()
	{
		return CurrentEvent != null && CurrentEvent.IsRelay;
	}

	public bool IsRelayEventInProgress()
	{
		return IsRelayEvent() && !RelayManager.HasSeenResults() && RelayManager.GetRacesDone() > 0;
	}
}
