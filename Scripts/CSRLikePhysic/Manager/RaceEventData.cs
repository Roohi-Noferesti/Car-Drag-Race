//using DataSerialization;

using System;
using System.Collections.Generic;
using System.Linq;
using DataSerialization;
using UnityEngine;
using Color = UnityEngine.Color;

[Serializable]
public class RaceEventData//:ICloneable
{
    public string EventName;

    public int EventID;

    public List<RaceEventRestriction> Restrictions = new List<RaceEventRestriction>();

	public RaceEventReward RaceReward = new RaceEventReward();

	public short EventOrder;

	public bool AutoDifficulty;

	public bool LoanCarRace;

	public string AICar;

	public string AIDriver;

	public string AIDriverCrew = string.Empty;

	public string AIDriverCrewNumber = string.Empty;

    public string AIDriverLivery = string.Empty;

    public string BodyShader;

    public string HeadLightShader;

    public string RingShader;

    public string Sticker;

    public string Spoiler;

    public float BodyHeight;

	public bool UseCustomShader;

	public bool IsHalfMile;

	public bool IsRelay;

	public bool ForceUserInCar;

	public string HumanCar;

	public byte EngineUpgradeLevel;

	public byte TurboUpgradeLevel;

    public byte IntakeUpgradeLevel;

    public byte NitrousUpgradeLevel;

    public byte BodyUpgradeLevel;

	public byte TyreUpgradeLevel;

	public byte TransmissionUpgradeLevel;

	public float ModifiedCarMass;

	public bool AutoHeadstart;

	public float AutoHeadstartOffset;

	public bool SwitchBackRace;

	public float UpgradePercentage = -1f;

	public int AIPerformancePotentialIndex;

	public int MaxPerformancePotentialIndex;

	[NonSerialized]
	public int AICsrAvatar = -1;

	[NonSerialized]
	private RaceEventGroup group;

	[NonSerialized]
	private RaceEventTopLevelCategory parent;

    [NonSerialized]
    private PinDetail worldTourPinDetail = new PinDetail();

    public bool UseRewardMultiplier;

	public RaceEventTypeMultipliers RewardsMultipliers;

	public BubbleMessageData PinBubbleMessage;

	[NonSerialized]
	private CarUpgradeSetup loanCarUpgradeSetup;

	[NonSerialized]
	private CarGarageInstance loanCarGarageInstance;


    public RaceEventGroup Group
	{
		get
		{
			return this.group;
		}
	}

	public RaceEventTopLevelCategory Parent
	{
		get
		{
			return this.parent;
		}
	}

	public CarUpgradeSetup LoanCarUpgradeSetup
	{
		get
		{
			return this.loanCarUpgradeSetup;
		}
	}

	public CarGarageInstance LoanCarGarageInstance
	{
		get
		{
			return this.loanCarGarageInstance;
		}
	}

	public string GetHumanCar()
	{
		if (!string.IsNullOrEmpty(this.HumanCar) && !this.AutoDifficulty)
		{
			return (!this.PreferBossCar(this.HumanCar)) ? this.HumanCar : (this.HumanCar + "Boss");
		}
		if (this.IsRandomRelay())
		{
			return this.HumanCar;
		}
		if (this.IsRelay && !this.IsRandomRelay())
		{
			return (!this.PreferBossCar(this.AICar)) ? this.AICar : (this.AICar + "Boss");
		}
		return string.Empty;
	}

	private bool PreferBossCar(string cardbkey)
	{
		CarGarageInstance carFromID = PlayerProfileManager.Instance.ActiveProfile.GetCarFromID(cardbkey + "Boss");
		CarGarageInstance carFromID2 = PlayerProfileManager.Instance.ActiveProfile.GetCarFromID(cardbkey);
		return carFromID != null && (carFromID2 == null || carFromID.CurrentPPIndex > carFromID2.CurrentPPIndex);
	}

	public bool IsRandomRelay()
	{
		return this.IsRelay && this.AutoDifficulty;
	}

	public bool IsGrindRelay()
	{
		return this.IsRandomRelay() && this.Group.IsGrindRelay;
	}

	public float GetTimeDifference()
	{
		return (!this.IsRelay) ? this.AutoHeadstartTimeDifference() : RelayManager.GetTimeDifference();
	}

	public float AutoHeadstartTimeDifference()
	{
		if (!this.AutoHeadstart)
		{
			return 0f;
		}
		float qMTimeForPPIndex = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(this.GetAIPerformancePotentialIndex());
		float qMTimeForPPIndex2;
		if (string.IsNullOrEmpty(this.GetHumanCar()))
		{
			qMTimeForPPIndex2 = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().CurrentPPIndex);
		}
		else
		{
			qMTimeForPPIndex2 = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(PlayerProfileManager.Instance.ActiveProfile.GetCarFromID(this.GetHumanCar()).CurrentPPIndex);
		}
		return qMTimeForPPIndex2 - qMTimeForPPIndex + this.AutoHeadstartOffset;
	}

	public Color GetHumanTierColor()
	{
        //CarInfo car = CarDatabase.Instance.GetCar(this.GetHumanCar());
	    return Color.white;
        //return GameDatabase.Instance.Colours.GetTierColour(car.BaseCarTier);
	}

	public bool IsAIDriverAvatarAvailable()
	{
		return !string.IsNullOrEmpty(this.AIDriverCrew) && !string.IsNullOrEmpty(this.AIDriverCrewNumber);
	}

	public bool UseCsrAvatarForAI()
	{
		return this.IsRandomRelay();
	}

	public int GetAIPerformancePotentialIndex()
	{
        if (this.AIPerformancePotentialIndex > 0)
        {
            return this.AIPerformancePotentialIndex;
        }
        CarInfo car = CarDatabase.Instance.GetCar(this.AICar);
        int result = 0;
		if (car != null)
		{
            result = car.PPIndex;
			CarUpgradeSetup aICarUpgradeSetup = this.GetAICarUpgradeSetup();
			PredefinedUpgradeSetsData predefinedUpgradeSetsData = new PredefinedUpgradeSetsData();
			predefinedUpgradeSetsData.SetFromUpgradeSetup(0, aICarUpgradeSetup);
			string aiCarUpgradeData = predefinedUpgradeSetsData.UpgradeData;
		    PredefinedUpgradeSetsData predefinedUpgradeSetsData2 = car.PredefinedUpgradeSets.Length == 0
		        ? null
		        : car.PredefinedUpgradeSets.FirstOrDefault(set => set.UpgradeData == aiCarUpgradeData);
		    if (predefinedUpgradeSetsData2 != null)
		    {
		        result = predefinedUpgradeSetsData2.PPIndex;
		    }
		    else
		    {
                GTDebug.Log(GTLogChannel.RaceEventDatabase, 
		            string.Format(
		                "Warning! upgradesetup '{0}' for car '{1}' not found . it may show wrong difficulty label in event pane.Add it to 'PredefinedUpgradeSet' in {1} 'Car Info' to prevent this to happen"
                        +"\nMore details about upgrade : {2}"
                        , aiCarUpgradeData, car.Key, aICarUpgradeSetup));
		    }
		}
		return result;
	}

	public IEnumerable<CarGarageInstance> GetCompatibleCars()
	{
        eCarTier tier = (this.parent == null) ? eCarTier.TIER_5 : this.parent.GetTierEvents().GetCarTier();
        List<CarGarageInstance> carsOwned = PlayerProfileManager.Instance.ActiveProfile.CarsOwned;
	    return carsOwned.Where(delegate(CarGarageInstance car)
        {
            var baseCarTier = CarDatabase.Instance.GetCar(car.CarDBKey).BaseCarTier;
            if (
                /*MultiplayerUtils.SelectedMultiplayerMode != MultiplayerMode.EVENT &&
                MultiplayerUtils.SelectedMultiplayerMode != MultiplayerMode.PRO_CLUB &&*/
                !this.IsWorldTourRace() && baseCarTier != tier)
            {
                return false;
            }
            RaceEventRestriction.RestrictionMet restrictionMet =
	            this.DoesMeetRestrictions(CarDatabase.Instance.GetCar(car.CarDBKey));
	        return restrictionMet == RaceEventRestriction.RestrictionMet.TRUE ||
	               (restrictionMet == RaceEventRestriction.RestrictionMet.UNKNOWN &&
	                !CarDataDefaults.IsBossCar(car.CarDBKey));
	    });
	}

	public CarUpgradeSetup GetAICarUpgradeSetup(CarInfo aiCarInfo = null)
	{
		CarUpgradeSetup carUpgradeSetup = new CarUpgradeSetup();
		if (this.UpgradePercentage >= 0f && this.UpgradePercentage <= 100f)
		{
            if(aiCarInfo==null)
                aiCarInfo = CarDatabase.Instance.GetCar(this.AICar);
            if (aiCarInfo != null)
			{
                IEnumerable<PredefinedUpgradeSetsData> source = from upgrade in aiCarInfo.PredefinedUpgradeSets
				where !upgrade.HasModifiedCarMass
				select upgrade;
				if (source.Count<PredefinedUpgradeSetsData>() <= 0)
				{
				    return null;
				}
				int index = (int)Math.Round((double)(this.UpgradePercentage / 100f * (float)(source.Count<PredefinedUpgradeSetsData>() - 1)));
				PredefinedUpgradeSetsData predefinedUpgradeSetsData = source.ElementAt(index);
                predefinedUpgradeSetsData.FillUpgradeSetup(aiCarInfo, ref carUpgradeSetup);
				if (this.ModifiedCarMass > 0f)
				{
					carUpgradeSetup.ModifiedCarMass = this.ModifiedCarMass;
				}
			}
		}
		else
		{
			carUpgradeSetup.UpgradeStatus[eUpgradeType.BODY].levelFitted = this.BodyUpgradeLevel;
			carUpgradeSetup.UpgradeStatus[eUpgradeType.ENGINE].levelFitted = this.EngineUpgradeLevel;
			carUpgradeSetup.UpgradeStatus[eUpgradeType.INTAKE].levelFitted = this.IntakeUpgradeLevel;
			carUpgradeSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted = this.NitrousUpgradeLevel;
			carUpgradeSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted = this.TransmissionUpgradeLevel;
			carUpgradeSetup.UpgradeStatus[eUpgradeType.TURBO].levelFitted = this.TurboUpgradeLevel;
			carUpgradeSetup.UpgradeStatus[eUpgradeType.TYRES].levelFitted = this.TyreUpgradeLevel;
			carUpgradeSetup.ModifiedCarMass = this.ModifiedCarMass;
			carUpgradeSetup.CarDBKey = this.AICar;
		}
		return carUpgradeSetup;
	}

	public void SetUpgradeStatus(Dictionary<eUpgradeType, CarUpgradeStatus> setup)
	{
		this.BodyUpgradeLevel = setup[eUpgradeType.BODY].levelFitted;
		this.EngineUpgradeLevel = setup[eUpgradeType.ENGINE].levelFitted;
		this.IntakeUpgradeLevel = setup[eUpgradeType.INTAKE].levelFitted;
		this.NitrousUpgradeLevel = setup[eUpgradeType.NITROUS].levelFitted;
		this.TransmissionUpgradeLevel = setup[eUpgradeType.TRANSMISSION].levelFitted;
		this.TurboUpgradeLevel = setup[eUpgradeType.TURBO].levelFitted;
		this.TyreUpgradeLevel = setup[eUpgradeType.TYRES].levelFitted;
	}

    public void SetWorldTourPinPinDetail(PinDetail pinInfo)
    {
        this.worldTourPinDetail = pinInfo;
    }

    public PinDetail GetWorldTourPinPinDetail()
    {
        return this.worldTourPinDetail;
    }

    public RaceEventTypeMultipliers GetRewardsMultipliers(RewardsMultipliers rewardsMultipliers)
    {
        if (this.UseRewardMultiplier)
        {
            return this.RewardsMultipliers;
        }
        if (this.Parent == null)
        {
            return rewardsMultipliers.RegulationRaceMultipliers;
        }
        return this.Parent.GetRaceEventTypeRewardMultipliers(rewardsMultipliers, this);
    }


    public bool IsAutoStartEvent()
	{
        PinDetail worldTourPinPinDetail = this.GetWorldTourPinPinDetail();
        return worldTourPinPinDetail != null && worldTourPinPinDetail.IsAutoStartPin();
	}

	public void SetLoanCarDetails(CarUpgradeSetup upgradeSetup, CarGarageInstance garageInstance)
	{
		this.loanCarUpgradeSetup = upgradeSetup;
		this.loanCarGarageInstance = garageInstance;
	}

    //[Conditional("CSR_DEBUG_LOGGING")]
    //public void Validate()
    //{
    //    if (AssetDatabaseClient.Instance == null)
    //    {
    //        return;
    //    }
    //    if (string.IsNullOrEmpty(this.AIDriverLivery) || this.AIDriverLivery != "No Livery")
    //    {
    //    }
    //    if (string.IsNullOrEmpty(this.AICar) || this.AICar != "AutoDifficulty")
    //    {
    //    }
    //}

	private void InternStrings()
	{
        //InternTracker.Intern(ref this.EventName);
        //InternTracker.Intern(ref this.AICar);
        //InternTracker.Intern(ref this.AIDriver);
        //InternTracker.Intern(ref this.AIDriverLivery);
        //this.Restrictions.ForEach(delegate(RaceEventRestriction r)
        //{
        //    InternTracker.Intern(ref r.Manufacturer);
        //    InternTracker.Intern(ref r.Classes);
        //    InternTracker.Intern(ref r.Region);
        //    InternTracker.Intern(ref r.Model);
        //});
	}

	public void Process(RaceEventGroup zGroup, RaceEventTopLevelCategory zParent)
	{
		this.InternStrings();
		this.group = zGroup;
		this.parent = zParent;
        if (Application.isPlaying)
            GameDatabase.Instance.Career.RegisterEvent(this);
    }

	public bool DoesMeetRestrictions(CarPhysicsSetupCreator zCarPhysicsSetup)
	{
        foreach (RaceEventRestriction current in this.Restrictions)
        {
            if (!current.DoesMeetRestriction(zCarPhysicsSetup))
            {
                return false;
            }
        }
	    return true;
	}

	public RaceEventRestriction.RestrictionMet DoesMeetRestrictions(CarInfo info)
	{
		RaceEventRestriction.RestrictionMet restrictionMet = RaceEventRestriction.RestrictionMet.TRUE;
		foreach (RaceEventRestriction current in this.Restrictions)
		{
			RaceEventRestriction.RestrictionMet restrictionMet2 = current.DoesMeetRestrictionNaive(info);
			if (restrictionMet2 == RaceEventRestriction.RestrictionMet.FALSE)
			{
				restrictionMet = RaceEventRestriction.RestrictionMet.FALSE;
			}
			if (restrictionMet2 == RaceEventRestriction.RestrictionMet.UNKNOWN && restrictionMet != RaceEventRestriction.RestrictionMet.FALSE)
			{
				restrictionMet = RaceEventRestriction.RestrictionMet.UNKNOWN;
			}
		}
		return restrictionMet;
	}

	public bool IsLocalCarLoaned()
	{
		return this.IsDailyBattle() || this.IsTestDriveAndCarSetup();
	}

	public bool IsDailyBattle()
	{
        return this.parent != null && this.parent.GetType() == typeof(DailyBattleEvents);
	}

	public bool IsTestDrive()
	{
		return this.LoanCarRace;
	}

	public bool IsTestDriveAndCarSetup()
	{
		return this.IsTestDrive() && this.loanCarUpgradeSetup != null && this.loanCarGarageInstance != null;
	}

	public bool IsCrewBattle()
	{
		return this.parent != null && this.parent.GetType() == typeof(CrewBattleEvents);
	}

	public bool IsLadderEvent()
	{
		return this.parent != null && this.parent.GetType() == typeof(LadderEvents);
	}

	public bool IsRestrictionEvent()
	{
		return this.parent != null && this.parent.GetType() == typeof(RestrictionEvents);
	}

	public bool IsCarSpecificEvent()
	{
		return this.parent != null && this.parent.GetType() == typeof(CarSpecificEvents);
	}

	public bool IsManufacturerSpecificEvent()
	{
		return this.parent != null && this.parent.GetType() == typeof(ManufacturerSpecificEvents);
	}

	public bool IsRegulationRace()
	{
		return this.parent != null && this.parent.GetType() == typeof(RegulationRaceEvents);
	}

	public bool IsWorldTourRace()
	{
		return this.parent != null && this.parent.GetType() == typeof(WorldTourRaceEvents);
	}

	public bool IsWorldTourHighStakesEvent()
	{
        return this.IsWorldTourRace() && this.worldTourPinDetail != null && this.worldTourPinDetail.IsSuperNitrous;
	}

	public bool IsRepeatableEvent()
	{
		return this.parent != null && (this.IsRegulationRace() || this.IsDailyBattle());
	}

	public bool IsDifficultySelectEvent()
	{
		return this.parent != null && this.IsRegulationRace();
	}

	public bool IsHighStakesEvent()
	{
		return BoostNitrous.IsLegacyBoostNitrousEvent(this.EventID) || this.IsWorldTourHighStakesEvent();
	}

	public bool IsRaceTheWorldOrClubRaceEvent()
	{
		return this.IsRaceTheWorldEvent() || this.IsOnlineClubRacingEvent() || this.IsRaceTheWorldWorldTourEvent();
	}

	public bool IsRaceTheWorldEvent()
	{
		return this.parent != null && this.parent.GetType() == typeof(RaceTheWorldEvents);
	}

	public bool IsRaceTheWorldWorldTourEvent()
	{
		return this.parent != null && this.parent.GetType() == typeof(RaceTheWorldWorldTourEvents);
	}

	public bool IsOnlineClubRacingEvent()
	{
		return this.parent != null && this.parent.GetType() == typeof(OnlineClubRacingEvents);
	}

	public bool IsFriendRaceEvent()
	{
		return this.parent != null && this.parent.GetType() == typeof(FriendRaceEvents);
	}

	public bool IsMechanicAllowed()
	{
		if (CarDatabase.Instance.GetCar(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey).UsesEvoUpgrades())
		{
			return false;
		}
        //if (this.IsFriendRaceEvent())
        //{
        //    return GameDatabase.Instance.Friends.AllowUseMechanic() && PlayerProfileManager.Instance.ActiveProfile.FriendRacesWon > 0 && PlayerProfileManager.Instance.ActiveProfile.FriendRacesPlayed > 1;
        //}
		return !this.IsRaceTheWorldOrClubRaceEvent() && !this.IsLocalCarLoaned();
	}

	public int GetRelayRaceIndex()
	{
		return this.Group.RaceEvents.FindIndex((RaceEventData e) => e.EventID == this.EventID);
	}

	public RaceTimeType GetRaceTimeType()
	{
		RaceTimeType result = RaceTimeType.SINGLEPLAYER;
		if (this.IsRaceTheWorldOrClubRaceEvent())
		{
			result = RaceTimeType.MULTIPLAYER;
		}
		else if (this.IsFriendRaceEvent())
		{
			result = RaceTimeType.RYF;
		}
		return result;
	}

    public int GetRaceGroupIndex()
    {
        if (this.Group == null)
        {
            return -1;
        }
        return Group.RaceEvents.FindIndex(e => e == this);
    }


	public int GetProgressionRaceEventNumber()
	{
		if (this.Parent == null)
		{
			return -1;
		}
        CrewBattleEvents crewBattleEvents = this.Parent.GetTierEvents().CrewBattleEvents;
        if (crewBattleEvents.RaceEventGroups.Count>0 && crewBattleEvents.RaceEventGroups[0].RaceEvents.Contains(this))
        {
            return (int)this.EventOrder;
        }
		return -1;
	}


	public int GetNumberOfProgressionRacesTotal()
	{
		CrewBattleEvents crewBattleEvents = this.Parent.GetTierEvents().CrewBattleEvents;
		if (crewBattleEvents.RaceEventGroups[0].RaceEvents.Contains(this))
		{
			return crewBattleEvents.RaceEventGroups[0].RaceEvents.Count;
		}
		return 0;
	}

	public bool IsFirstCrewMemberRace()
	{
		return this.GetProgressionRaceEventNumber() == 0;
	}

	public int BossRaceIndex()
	{
		if (!this.IsBossRace())
		{
			return -1;
		}
		int progressionRaceEventNumber = this.GetProgressionRaceEventNumber();
		return progressionRaceEventNumber - 4;
	}

	public bool IsBossRace()
	{
		int progressionRaceEventNumber = this.GetProgressionRaceEventNumber();
		if (progressionRaceEventNumber < 0)
		{
			return false;
		}
		int num = this.GetNumberOfProgressionRacesTotal() - 3;
		return progressionRaceEventNumber >= num;
	}

	public bool IsFinalRaceInGroup()
	{
		int index = this.Group.RaceEvents.Count - 1;
		return this.Group.RaceEvents[index] == this;
	}

	public bool IsCrewRace()
	{
		return this.GetProgressionRaceEventNumber() >= 0 && !this.IsBossRace();
	}

	private int IsLadderRace(out int numberOfLadderEvents)
	{
		numberOfLadderEvents = 0;
		if (this.Parent == null)
		{
			return -1;
		}
		LadderEvents ladderEvents = this.Parent.GetTierEvents().LadderEvents;
		if (ladderEvents.RaceEventGroups[0].RaceEvents.Contains(this))
		{
			numberOfLadderEvents = ladderEvents.RaceEventGroups[0].RaceEvents.Count;
			return (int)this.EventOrder;
		}
		return -1;
	}

	public bool IsLadderSemiFinal()
	{
		int num2;
		int num = this.IsLadderRace(out num2);
		return num >= 0 && num == num2 - 2;
	}

	public bool IsLadderFinal()
	{
		int num2;
		int num = this.IsLadderRace(out num2);
		return num >= 0 && num == num2 - 1;
	}

    public bool IsTutorial1stRace()
    {
        return RaceEventInfo.Instance.CurrentEvent ==
               GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial;
    }

    public bool IsTutorial2ndRace()
    {
        return RaceEventInfo.Instance.CurrentEvent ==
               GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial2;
    }

    public bool IsTutorial3thRace()
    {
        return RaceEventInfo.Instance.CurrentEvent ==
               GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial3;
    }

    public bool IsTutorial()
	{
        return this.IsTutorial1stRace() || this.IsTutorial2ndRace() || IsTutorial3thRace();
	}

	public bool RaceCostsFuel()
	{
		return !this.IsFriendRaceEvent() || 
            !PlayerProfileManager.Instance.ActiveProfile.IsCarOwned("MiniCooperS_RWF") || 
            PlayerProfileManager.Instance.ActiveProfile.FriendRacesWon != 0;
	}

    public bool GetPreRaceSceneToDisplay(out NarrativeScene scene)
    {
        scene = null;
        if (this.worldTourPinDetail.WorldTourScheduledPinInfo == null)
        {
            return false;
        }
        if (string.IsNullOrEmpty(this.worldTourPinDetail.WorldTourScheduledPinInfo.Narrative.GetPreRaceSceneID()))
        {
            return false;
        }
        IGameState gs = new GameStateFacade();
        return this.worldTourPinDetail.WorldTourScheduledPinInfo.Narrative.IsPreRaceSceneEligible(gs) && TierXManager.Instance.GetNarrativeScene(this.worldTourPinDetail.WorldTourScheduledPinInfo.Narrative.GetPreRaceSceneID(), out scene);
    }

    public bool GetPostRaceSceneWinToDisplay(out NarrativeScene scene)
    {
        scene = null;
        if (this.worldTourPinDetail.WorldTourScheduledPinInfo == null)
        {
            return false;
        }
        if (string.IsNullOrEmpty(this.worldTourPinDetail.WorldTourScheduledPinInfo.Narrative.GetPostRaceWinSceneID()))
        {
            return false;
        }
        IGameState gs = new GameStateFacade();
        return this.worldTourPinDetail.WorldTourScheduledPinInfo.Narrative.IsPostRaceSceneEligible(gs) && TierXManager.Instance.GetNarrativeScene(this.worldTourPinDetail.WorldTourScheduledPinInfo.Narrative.GetPostRaceWinSceneID(), out scene);
    }

    public bool GetPostRaceSceneLoseToDisplay(out NarrativeScene scene)
    {
        scene = null;
        if (this.worldTourPinDetail.WorldTourScheduledPinInfo == null)
        {
            return false;
        }
        if (string.IsNullOrEmpty(this.worldTourPinDetail.WorldTourScheduledPinInfo.Narrative.GetPostRaceLoseSceneID()))
        {
            return false;
        }
        IGameState gs = new GameStateFacade();
        return this.worldTourPinDetail.WorldTourScheduledPinInfo.Narrative.IsPostRaceSceneEligible(gs) && TierXManager.Instance.GetNarrativeScene(this.worldTourPinDetail.WorldTourScheduledPinInfo.Narrative.GetPostRaceLoseSceneID(), out scene);
    }

    public BaseCarTierEvents GetTierEvent()
    {
        if (this.Parent != null)
        {
            return this.Parent.GetTierEvents();
        }
        return null;
    }

    public string GetDifficultyLabel()
    {
        if (IsRegulationRace())
        {
            if (Parent.RaceEventGroups[0].RaceEvents[0] == this)
                return "Easy";
            else if (Parent.RaceEventGroups[0].RaceEvents[1] == this)
                return "Normal";
            else
            {
                return "Hard";
            }
        }
        else if (IsSMPRaceEvent())
        {
            return "SMP_" + EventOrder;
        }
        return "none";
    }

    //public static bool operator ==(RaceEventData a, RaceEventData b)
    //{
    //    if (object.ReferenceEquals(a, null))
    //    {
    //        return object.ReferenceEquals(b, null);
    //    }

    //    return a.Equals(b);
    //}

    //public static bool operator !=(RaceEventData a, RaceEventData b)
    //{
    //    return !(a == b);
    //}

    //public object Clone()
    //{
    //    return MemberwiseClone();
    //}
    public string GetRewardText()
    {
        var cashString =  CurrencyUtils.GetCashString(RaceReward.CashPrize);
        var goldString = CurrencyUtils.GetGoldStringWithIcon(RaceReward.GoldPrize);
        if (RaceReward.CashPrize > 0 && RaceReward.GoldPrize > 0)
        {
            return String.Format("{0}\n{1}", cashString, goldString);
        }
        else if (RaceReward.CashPrize > 0)
        {
            return cashString;
        }
        else
        {
            return goldString;
        }
    }

    public bool IsSMPRaceEvent()
    {
        return this.Parent != null && this.Parent.GetType() == typeof(SMPRaceEvents);
    }

}
