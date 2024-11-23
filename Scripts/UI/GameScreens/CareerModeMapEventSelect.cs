using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSerialization;
using EventPaneRestriction;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CareerModeMapEventSelect : MonoBehaviour
{
	public delegate bool EventPinMatchingDelegate(EventPin e);

	public CareerModeMapScreen MapScreen;

	public GameObject EventPinPrefab;

    private List<IMapPin> MapPins = new List<IMapPin>();

	private List<GameObject> EventPins = new List<GameObject>();

	private List<GameObject> GroupPins = new List<GameObject>();

	public GameObject CustomPinPrefab;

	private List<GameObject> CustomPins = new List<GameObject>();

	private List<GameObject> HubEventPins = new List<GameObject>();

	private List<GameObject> OldPins = new List<GameObject>();

	private List<GameObject> MultiplayerDisabledPins = new List<GameObject>();

    private List<TierXPin> TierXPins = new List<TierXPin>();

	private FlowConditionalBase worldTourThemeMapConditional;

	private static RaceEventData backupEventData;

	private BubbleMessage eliteClubBubbleMessage;

    private static readonly Vector3 eliteBubbleMessageOffset = new Vector3(0f, 0.1f, -0.27f);

    public  Vector3[] RegulationRace = new Vector3[]
    {
        new Vector3(120, -119, -2),
        new Vector3(120, -119, -2),
        new Vector3(120, -119, -2),
        new Vector3(120, -119, -2),
        new Vector3(120, -119, -2),
        new Vector3(120, -119, -2),
    };

    public  Vector3 DailyBattle = new Vector3(-85.8f, -187.9f, -2);

    public  Vector3[] LadderRace = new Vector3[]
    {
        new Vector3(-227, 19.4f, -2),
        new Vector3(-227, 19.4f, -2),
        new Vector3(-227, 19.4f, -2),
        new Vector3(-227, 19.4f, -2),
        new Vector3(-227, 19.4f, -2),
        new Vector3(-227, 19.4f, -2),
    };

    public Vector3[] TierPosition = new Vector3[]
    {
        new Vector3(-227, 19.4f, -2),
        new Vector3(-227, 19.4f, -2),
        new Vector3(-227, 19.4f, -2),
        new Vector3(-227, 19.4f, -2),
        new Vector3(-227, 19.4f, -2),
        new Vector3(-227, 19.4f, -2),
    };

    public Vector3[] RestrictionRace = new Vector3[]
    {
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
    };

    public  Vector3[] CarSpecific = new Vector3[]
    {
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
    };

    public Vector3[] Manufacturer = new Vector3[]
    {
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
    };

    public  Vector3 Challenge = new Vector3(-0.1f, 0.7f, -2);

    public  Vector3 CustomPinMechanic = new Vector3(311, 203, -2);

    public Vector3[] CrewRace = new Vector3[]
    {
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
        new Vector3(20, -210, -2),
    };

    public  Vector3 FriendRace = new Vector3(-0.1f, 0.6f, -2);

    public  Vector3 OnlineRace = new Vector3(-0.1f, 0.6f, -2);

    public Vector3 WorldTourRaces = new Vector3(2.342107F, 20, -19.83227F);

    public StringVectorDictionary WorldTourPins;



    public static readonly Vector2 MultiPrizeList;

    public static readonly Vector2 MultiLeaderboards;

    public static readonly Vector2 MultiSeasonInfo;

    public static readonly Vector2 MultiOffset;

	private bool highlightNextPopulatedEvent;

	public EventPin HighlightedPin
	{
		get;
		private set;
	}

	public void RemoveAll(bool zDeleteAll)
	{
		KillPinBubbles();
		KillTutorialMultiplayerPinBubbles();
		RemoveOld();
		foreach (TierXPin current in TierXPins)
		{
			current.RemoveAttachments();
			current.enabled = false;
		}
		TierXPins.Clear();
		foreach (GameObject current2 in EventPins)
		{
		    if (current2 == null)
		        continue;
			current2.GetComponent<EventPin>().RemoveCallbacks(this);
			OldPins.Add(current2);
		}
		EventPins.Clear();
		foreach (GameObject current3 in GroupPins)
		{
            if (current3 == null)
                continue;
			current3.GetComponent<EventPin>().RemoveCallbacks(this);
			OldPins.Add(current3);
		}
		GroupPins.Clear();
		foreach (GameObject current4 in CustomPins)
		{
            if(current4==null)continue;
            current4.GetComponent<CustomPin>().RemoveCallbacks(this);
			OldPins.Add(current4);
		}
		CustomPins.Clear();
        MapPins.Clear();
		OldPins.AddRange(HubEventPins);
		HubEventPins.Clear();
		OldPins.AddRange(MultiplayerDisabledPins);
		MultiplayerDisabledPins.Clear();
		if (zDeleteAll)
		{
			RemoveOld();
		}
	}

	public void RemoveOld()
	{
		foreach (GameObject current in OldPins)
		{
			UICacheManager.Instance.ReleaseItem(current);
		}
		OldPins.Clear();
	}

	private void CreateEventPin(RaceEventData myEvent, bool isCompleted, bool zShouldHighlight,
        ProgressionMapPinEventType type,eCarTier carTier)
	{
        EventPin eventPin = CreatePin(myEvent.Parent, myEvent, null, type, carTier);
		if (isCompleted)
		{
			eventPin.SetupCompleted(true);
		}
		eventPin.AddCallbacks(this);
		EventPins.Add(eventPin.gameObject);
        MapPins.Add(eventPin);
		if (highlightNextPopulatedEvent && zShouldHighlight)
		{
			HighlightPin(eventPin);
		}
		highlightNextPopulatedEvent = false;
	}

	public void PopulateTutorialCarSpecificEvents(eCarTier zCarTier, bool zShouldHighlight)
	{
		RaceEventData carSpecificEvent = RaceEventQuery.Instance.GetCarSpecificEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), false);
		bool isCompleted = false;
		if (carSpecificEvent == null)
		{
			carSpecificEvent = RaceEventQuery.Instance.GetCarSpecificEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), true);
			if (carSpecificEvent == null)
			{
				return;
			}
			isCompleted = true;
		}
		CreateEventPin(carSpecificEvent, isCompleted, zShouldHighlight,
            ProgressionMapPinEventType.CAR_SPECIFIC_RACES, zCarTier);
	}

	public void PopulateTutorialManufacturerSpecificEvents(eCarTier zCarTier, bool zShouldHighlight)
	{
		RaceEventData manufacturerSpecificRaceEvent = RaceEventQuery.Instance.GetManufacturerSpecificRaceEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), false);
		bool isCompleted = false;
		if (manufacturerSpecificRaceEvent == null)
		{
			manufacturerSpecificRaceEvent = RaceEventQuery.Instance.GetManufacturerSpecificRaceEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), true);
			if (manufacturerSpecificRaceEvent == null)
			{
				return;
			}
			isCompleted = true;
		}
		CreateEventPin(manufacturerSpecificRaceEvent, isCompleted, zShouldHighlight,
            ProgressionMapPinEventType.MANUFACTURER_SPECIFIC_RACES, zCarTier);
	}


    public void PopulateWorldTourFakeEvents(string pinID)
    {
        CreateCustomPin(CustomPin.CustomType.WorldTourFake,ProgressionMapPinEventType.WORLD_TOURS, pinID);
    }




    public void PopulateCrewRaces(eCarTier zCarTier, bool zShouldHighlight)
    {
        RaceEventData crewBattleEvent =
            RaceEventQuery.Instance.GetCrewBattleEvent(
                GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), false);
        bool isCompleted = false;
        if (crewBattleEvent == null)
        {
            crewBattleEvent =
                RaceEventQuery.Instance.GetCrewBattleEvent(
                    GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), true);
            isCompleted = true;
        }
        CreateEventPin(crewBattleEvent, isCompleted, zShouldHighlight,
            ProgressionMapPinEventType.CREW_RACES, zCarTier);
    }

    public void PopulateTutorialRestrictionRaces(eCarTier zCarTier, bool zShouldHighlight)
	{
		RaceEventData restrictionEvent = RaceEventQuery.Instance.GetRestrictionEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), false);
		bool isCompleted = false;
		if (restrictionEvent == null)
		{
			restrictionEvent = RaceEventQuery.Instance.GetRestrictionEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), true);
			isCompleted = true;
		}
		CreateEventPin(restrictionEvent, isCompleted, zShouldHighlight,
            ProgressionMapPinEventType.RESTRICTION_RACES, zCarTier);
	}

	public void PopulateTutorialLadderRaces(eCarTier zCarTier, bool zCanHighlight)
	{
		RaceEventData ladderEvent = RaceEventQuery.Instance.GetLadderEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), false);
		bool isCompleted = false;
		if (ladderEvent == null)
		{
			ladderEvent = RaceEventQuery.Instance.GetLadderEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), true);
			isCompleted = true;
		}
		CreateEventPin(ladderEvent, isCompleted, zCanHighlight,
            ProgressionMapPinEventType.LADDER_RACES, zCarTier);
	}


    public void PopulateRegulationRaces(eCarTier zCarTier)
    {
        var regulationRaceEvent =
            RaceEventQuery.Instance.GetRegulationRaceEvent(
                GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier));
        var eventPin = CreatePin(regulationRaceEvent.Parent, null, regulationRaceEvent,
            ProgressionMapPinEventType.REGULATION_RACES,zCarTier);
        eventPin.AddCallbacks(this);
        GroupPins.Add(eventPin.gameObject);
        MapPins.Add(eventPin);
        if (highlightNextPopulatedEvent)
        {
            HighlightPin(eventPin);
        }
        highlightNextPopulatedEvent = true;
    }

    public void PopulateDailyBattle(eCarTier zCarTier, bool zShouldHighlight)
	{
		RaceEventData dailyBattleEvent = RaceEventQuery.Instance.GetDailyBattleEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), false);
		if (dailyBattleEvent == null)
		{
			dailyBattleEvent = RaceEventQuery.Instance.GetDailyBattleEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(zCarTier), true);
		}
		EventPin eventPin = CreatePin(dailyBattleEvent.Parent, dailyBattleEvent, null,
            ProgressionMapPinEventType.DAILY_BATTLE_RACES,eCarTier.BASE_EVENT_TIER);
		eventPin.AddCallbacks(this);
		EventPins.Add(eventPin.gameObject);
        MapPins.Add(eventPin);
		if (highlightNextPopulatedEvent && zShouldHighlight)
		{
			HighlightPin(eventPin);
		}

		highlightNextPopulatedEvent = false;
	}

	public void PopulateFriendRaceEvent(eCarTier zCarTier, bool zShouldHighlight)
	{
		FriendRaceEvents friendRaceEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.FriendRaceEvents;
		EventPin eventPin = CreatePin(friendRaceEvents, friendRaceEvents.RaceEventGroups[0].RaceEvents[0], null,
            ProgressionMapPinEventType.FRIENDS_RACES,eCarTier.BASE_EVENT_TIER);
		eventPin.AddCallbacks(this);
		EventPins.Add(eventPin.gameObject);
		if (highlightNextPopulatedEvent && zShouldHighlight)
		{
			HighlightPin(eventPin);
		}
		highlightNextPopulatedEvent = false;
	}


    public void PopulateSMPEvents(eCarTier zCarTier, bool zShouldHighlight)
    {
        if (zCarTier < eCarTier.TIER_3)
        {
            zCarTier = eCarTier.TIER_3;
        }
        var onlineRaceEvent = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.SMPRaceEvents;
        var eventPin = CreatePin(onlineRaceEvent, onlineRaceEvent.RaceEventGroups[0].RaceEvents[0], null,
            ProgressionMapPinEventType.SMP_RACES, zCarTier);
        eventPin.AddCallbacks(this);
        GroupPins.Add(eventPin.gameObject);
        MapPins.Add(eventPin);
        if (highlightNextPopulatedEvent)
        {
            HighlightPin(eventPin);
        }
        highlightNextPopulatedEvent = true;
    }

    public EventPin GetRYFPin()
	{
		foreach (GameObject current in EventPins)
		{
			EventPin component = current.GetComponent<EventPin>();
			if (!component.IsGroupPin && component.EventData.IsFriendRaceEvent())
			{
				return component;
			}
		}
		return null;
	}

	private void DisplayTutorialMultiplayerPinBubble(EventPin elitePin)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return;
		}
		if (elitePin != null && !activeProfile.MultiplayerTutorial_EliteClubCompleted && elitePin.gameObject.activeInHierarchy)
		{
			Vector3 position = elitePin.transform.position + eliteBubbleMessageOffset;
			eliteClubBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_TUTORIAL_MESSAGE_ELITE_CAR_MAP", false, position, BubbleMessage.NippleDir.DOWN, 0.5f);
			eliteClubBubbleMessage.GetParentTransform().parent = elitePin.transform;
			Log.AnEvent(Events.EliteClubIntro);
		}
	}

	public void KillTutorialMultiplayerPinBubbles()
	{
		if (eliteClubBubbleMessage != null)
		{
			eliteClubBubbleMessage.KillNow();
			eliteClubBubbleMessage = null;
		}
	}

	public void KillPinBubbles()
	{
		EventPins.ForEach(delegate(GameObject e)
		{
		    if (e != null)
		        e.GetComponent<EventPin>().KillBubble();
		});
	}

	public void PopulateTutorialMultiplayer()
	{
		EventPin eventPin = null;
		CustomPin customPin = null;
		CustomPin customPin2 = null;
		//CustomPin customPin3 = null;
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject current in EventPins)
		{
			EventPin component = current.GetComponent<EventPin>();
			if (!(component == null))
			{
				if (component.EventData.IsOnlineClubRacingEvent())
				{
					if (component.EventData.IsOnlineClubRacingEvent())
					{
						eventPin = component;
						list.Add(current);
					}
					else if (component.EventData.IsRaceTheWorldEvent())
					{
						list.Add(current);
					}
					else if (component.EventData.IsRaceTheWorldWorldTourEvent())
					{
						list.Add(current);
					}
				}
				else if (component.EventData.IsRaceTheWorldEvent())
				{
					list.Add(current);
				}
				else if (component.EventData.IsRaceTheWorldWorldTourEvent())
				{
					list.Add(current);
				}
			}
		}
		foreach (GameObject current2 in CustomPins)
		{
			CustomPin component2 = current2.GetComponent<CustomPin>();
			if (!(component2 == null))
			{
				switch (component2.Type)
				{
				case CustomPin.CustomType.PrizeList:
					customPin = component2;
					list.Add(current2);
					break;
				case CustomPin.CustomType.Leaderboards:
					customPin2 = component2;
					list.Add(current2);
					break;
				case CustomPin.CustomType.SeasonInfo:
					//customPin3 = component2;
					list.Add(current2);
					break;
				}
			}
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return;
		}
		if (eventPin != null && !activeProfile.OwnsEliteCar())
		{
			eventPin.gameObject.SetActive(false);
		}
		if (customPin != null && !activeProfile.MultiplayerTutorial_PrizeScreenCompleted)
		{
			customPin.gameObject.SetActive(false);
		}
		if (customPin2 != null && !activeProfile.MultiplayerTutorial_HasSeenRespectScreen)
		{
			customPin2.gameObject.SetActive(false);
		}
        //if (!SeasonServerDatabase.Instance.AreStandingsUpToDate())
        //{
        //    if (customPin3 != null)
        //    {
        //        customPin3.gameObject.SetActive(false);
        //    }
        //    if (customPin2 != null)
        //    {
        //        customPin2.gameObject.SetActive(false);
        //    }
        //}
		DisplayTutorialMultiplayerPinBubble(eventPin);
		DisplayMultiplayerDisabledPins(list);
	}

	private void DisplayMultiplayerDisabledPins(List<GameObject> pinList)
	{
		List<GameObject> list = (from q in pinList
		where !q.gameObject.activeInHierarchy
		select q).ToList<GameObject>();
		while (list.Count > MultiplayerDisabledPins.Count)
		{
			GameObject item = UICacheManager.Instance.GetItem("Career/Pins/DisabledPinPlaceholder", false);
			item.transform.parent = transform;
			MultiplayerDisabledPins.Add(item);
		}
		int num = 0;
		foreach (GameObject current in list)
		{
			GameObject gameObject = MultiplayerDisabledPins[num];
			gameObject.transform.position = current.transform.position;
			num++;
		}
		MultiplayerDisabledPins.Skip(num).ToList().ForEach(delegate(GameObject pin)
		{
			pin.SetActive(false);
		});
	}

	private Vector3 GetPositionOfEvent(RaceEventData zEventData, RaceEventGroup zGroupData,eCarTier carTier)
	{
		if (zGroupData != null)
		{
            if(zGroupData.Parent is RegulationRaceEvents)
		        return RegulationRace[(int) carTier];
            else if (zGroupData.Parent is SMPRaceEvents)
            {
                return OnlineRace;
            }
		}
	    if (zEventData.IsCrewBattle())
	    {
	        return CrewRace[(int) carTier];
	    }
	    if (zEventData.IsDailyBattle())
		{
            //PinDetail worldTourPinPinDetail = zEventData.GetWorldTourPinPinDetail();
            //if (worldTourPinPinDetail.WorldTourScheduledPinInfo != null)
            //{
            //    return worldTourPinPinDetail.Position.AsUnityVector2();
            //}
            return DailyBattle;
		}
		else
		{
			if (zEventData.IsLadderEvent())
			{
                return LadderRace[(int)carTier];
			}
			if (zEventData.IsRestrictionEvent())
			{
                return RestrictionRace[(int)carTier];
			}
			if (zEventData.IsCarSpecificEvent())
			{
                return CarSpecific[(int)carTier];
			}
			if (zEventData.IsManufacturerSpecificEvent())
			{
                return Manufacturer[(int)carTier];
			}
			if (zEventData.IsRaceTheWorldEvent())
			{
                //return this.MultiRaceTheWorld + CareerModeMapEventSelect.ConvertToUIPosition(this.MultiOffset);
			}
			if (zEventData.IsFriendRaceEvent())
			{
                return FriendRace;
			}
            if (zEventData.IsSMPRaceEvent())
            {
                return OnlineRace;
            }
            return RegulationRace[(int)carTier];
		}
	    //return new Vector2();
	}

    private Vector3 GetPositionOfCustomPin(CustomPin.CustomType pintType,string pinIDName)
    {
        switch (pintType)
        {
                case CustomPin.CustomType.Mechanic:
                    return CustomPinMechanic;
            case CustomPin.CustomType.WorldTourFake:
                return WorldTourPins.dictionary[pinIDName];
        }
        return Vector3.zero;
    }

    private EventPin CreatePin(RaceEventTopLevelCategory zCategory, RaceEventData zEventData,
        RaceEventGroup zGroupData, ProgressionMapPinEventType type, eCarTier carTier)
    {

        string pinName;
        if (zEventData!=null && zEventData.IsCrewBattle())
        {
            pinName = "Career/CrewPin";
        }
        else if (zEventData!=null && zEventData.IsCarSpecificEvent())
        {
            pinName = "Career/Car_Specific_Pin";
        }
        else if ((zEventData != null && zEventData.IsDailyBattle()) || (zGroupData!=null && zGroupData.RaceEvents[0].IsRegulationRace()))
        {
            pinName = "Career/RegulationPin";
        }
        else if (zEventData != null && zEventData.IsSMPRaceEvent())
        {
            pinName = "Career/SMPPin";
        }
        else
        {
            pinName = "Career/EventPin";
        }


        GameObject item =
            UICacheManager.Instance.GetItem(pinName, false);
        Vector3 positionOfEvent = GetPositionOfEvent(zEventData, zGroupData, carTier);
        item.transform.SetParent(MapScreenCache.MapCanvas.transform, false);
        item.rectTransform().anchoredPosition3D = positionOfEvent;
        EventPin component = item.GetComponent<EventPin>();
        component.type = type;
        component.tier = carTier;
        if (zEventData != null)
        {
            component.Setup(zEventData);
        }
        else
        {
            component.Setup(zGroupData);
        }
        item.name = zCategory.GetType().Name + "_" + carTier;
        return component;
    }

    public Vector2 GetPinPositionWorldSpace(CustomPin.CustomType type)
	{
	    //UnityEngine.Vector2 pos;
        //switch (type)
        //{
        //case CustomPin.CustomType.RaceTheWorld:
        //    pos = this.MultiRaceTheWorld + CareerModeMapEventSelect.ConvertToUIPosition(this.MultiOffset);
        //    goto IL_69;
        //case CustomPin.CustomType.PrizeList:
        //    pos = this.MultiPrizeList + CareerModeMapEventSelect.ConvertToUIPosition(this.MultiOffset);
        //    goto IL_69;
        //}
        //pos = new UnityEngine.Vector2(0f, 0f);
        //IL_69:
        //return CareerModeMapEventSelect.ConvertUIPinPosition(pos);
	    return new Vector2();
	}

    public Vector2 GetPinPositionWorldSpace(RaceEventData zEventData, RaceEventGroup zGroupData)
	{
		return ConvertUIPinPosition(GetPositionOfEvent(zEventData, zGroupData,eCarTier.BASE_EVENT_TIER));
	}

	private static Vector2 ConvertToUIPosition(Vector2 pos)
	{
	    //float height = CommonUI.Instance.NavBar.GetHeight();
        //float num = GUICamera.Instance.ScreenHeight - height;
        //return new UnityEngine.Vector2(pos.x / (GUICamera.Instance.ScreenWidth * 0.5f), (pos.y + height * 0.5f) / (num * 0.5f));
	    return new Vector2();
	}

    private static Vector2 ConvertUIPinPosition(Vector2 pos)
    {
        //float height = CommonUI.Instance.NavBar.GetHeight();
        //float num = GUICamera.Instance.ScreenHeight - height;
        //return new UnityEngine.Vector2(pos.x * GUICamera.Instance.ScreenWidth * 0.5f, pos.y * num * 0.5f - height * 0.5f);
        return new Vector2();
    }

    public void OnEventPress(EventPin eventpin)
	{
		eventpin.DismissBubble();
        if (eventpin.IsCompleted() || eventpin.IsTapDisabled)
		{
			return;
		}
        if (eventpin.IsGroupPin)
		{
            MapScreen.OnGroupSelected(eventpin.GroupData);
		}
		else
		{
            MapScreen.OnEventSelected(eventpin.EventData, false);
		}
	}

	public CustomPin GetMechanicPin()
	{
		foreach (GameObject current in CustomPins)
		{
			CustomPin component = current.GetComponent<CustomPin>();
			if (component.Type == CustomPin.CustomType.Mechanic)
			{
				return component;
			}
		}
		return null;
	}

	public CustomPin CreateCustomPin(CustomPin.CustomType customType,ProgressionMapPinEventType type,string PinID)
	{
	    var customPrefabName = customType == CustomPin.CustomType.Mechanic ? "Career/CustomPinBig" : "Career/WorldTourPin_Bold_Fake";
        GameObject item = UICacheManager.Instance.GetItem(customPrefabName, false);
        item.transform.SetParent(MapScreenCache.MapCanvas.transform, false);
	    var zPosition = GetPositionOfCustomPin(customType, PinID);
        item.rectTransform().anchoredPosition3D = zPosition;
		CustomPin component = item.GetComponent<CustomPin>();
	    component.type = type;
	    component.Name = component.name = PinID;
		component.Setup(customType, PinID);
        component.AddCallbacks(this);
		CustomPins.Add(item);
	    MapPins.Add(component);
		return component;
	}

	private static Vector3 AdjustPositionForEventPane(Vector3 pinPosition)
	{
		Vector3 b = new Vector3(0f, 0f, 0f);
		float num = pinPosition.x;
		//bool flag = CareerModeMapScreen.mapPaneSelected == 5;
		if (/*flag &&*/ TierXManager.Instance.ThemeDescriptor.AllowForRightJustifiedPins)
		{
            //CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
            //EventPane eventPane = careerModeMapScreen.eventPane;
            //b.x = eventPane.PaneWidthTight / 2f;
            //num *= (GUICamera.Instance.ScreenWidth - eventPane.PaneWidthTight) / GUICamera.Instance.ScreenWidth;
		}

	    return new Vector3(num, pinPosition.y, -0.1f);// - CareerModeMapScreen.GetPanelPositionOffset(CareerModeMapScreen.mapPaneSelected) - b;
	}

	private void AddMultiplayerPin(RaceEventTopLevelCategory inCategory)
	{
	    EventPin eventPin = CreatePin(inCategory, inCategory.RaceEventGroups[0].RaceEvents[0], null,
	        ProgressionMapPinEventType.MULTIPLAYER_RACES,eCarTier.BASE_EVENT_TIER);
		eventPin.AddCallbacks(this);
		EventPins.Add(eventPin.gameObject);
	}

	public void PopulateMultiplayerPins()
	{
        //CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
        //UnityEngine.Vector2 vector = this.MultiLeaderboards;
        //UnityEngine.Vector2 vector2 = this.MultiPrizeList;
        //UnityEngine.Vector2 vector3 = this.MultiSeasonInfo;
        //if (careerModeMapScreen != null)
        //{
        //    EventPane eventPane = careerModeMapScreen.eventPane;
        //    float num = eventPane.PaneWidth / GUICamera.Instance.ScreenWidth;
        //    vector.x = (vector.x - num) * (1f - num);
        //    vector2.x = (vector2.x - num) * (1f - num);
        //    vector3.x = (vector3.x - num) * (1f - num);
        //}
        //vector += CareerModeMapEventSelect.ConvertToUIPosition(this.MultiOffset);
        //vector2 += CareerModeMapEventSelect.ConvertToUIPosition(this.MultiOffset);
        //vector3 += CareerModeMapEventSelect.ConvertToUIPosition(this.MultiOffset);
        //this.CreateCustomPin(CustomPin.CustomType.Leaderboards, vector, default(UnityEngine.Vector2));
        //this.CreateCustomPin(CustomPin.CustomType.PrizeList, vector2, default(UnityEngine.Vector2));
        //this.CreateCustomPin(CustomPin.CustomType.SeasonInfo, vector3, default(UnityEngine.Vector2));
        //RaceTheWorldEvents raceTheWorldEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.RaceTheWorldEvents;
        //this.AddMultiplayerPin(raceTheWorldEvents);
	}

    public void OnCustomPress(CustomPin customPin)
	{
        MapScreen.OnCustomSelected(customPin.Type,customPin.Name);
	}

	private void HighlightPin(EventPin zButton)
	{
		HighlightedPin = zButton;
	}

	public void RemoveHighlight()
	{
		HighlightedPin = null;
		highlightNextPopulatedEvent = true;
	}

	public EventPin GetRRPin()
	{
		foreach (GameObject current in GroupPins)
		{
			EventPin component = current.GetComponent<EventPin>();
			if (component.IsGroupPin && component.GroupData.RaceEvents[0].IsRegulationRace())
			{
				return component;
			}
		}
		return null;
	}

	public EventPin GetAnyPinMatchingCondition(EventPinMatchingDelegate condition)
	{
		foreach (GameObject current in EventPins)
		{
			EventPin component = current.GetComponent<EventPin>();
			if (condition(component))
			{
				return component;
			}
		}
		return null;
	}

	public EventPin GetEventPinMatchingCondition(EventPinMatchingDelegate condition)
	{
		return GetAnyPinMatchingCondition((EventPin eventPin) => !eventPin.IsGroupPin && condition(eventPin));
	}

	public EventPin GetGroupPinMatchingCondition(EventPinMatchingDelegate condition)
	{
		return GetAnyPinMatchingCondition((EventPin eventPin) => eventPin.IsGroupPin && condition(eventPin));
	}

	public List<EventPin> GetEventPins()
	{
		return (from go in EventPins
		select go.GetComponent<EventPin>()).ToList<EventPin>();
	}

    //public TierXPin GetTierXPinWithLabel(string label)
    //{
    //    return this.TierXPins.FirstOrDefault((TierXPin tx) => tx.pinDetails.Label == label);
    //}

	private void AttachTierXPin(GameObject pinGO, PinDetail pin)
	{
		TierXPin tierXPin = pinGO.GetComponent<TierXPin>();
		if (tierXPin == null)
		{
			tierXPin = pinGO.AddComponent<TierXPin>();
		}
		else
		{
			tierXPin.enabled = true;
		}
		tierXPin.SetPinDetails(pin);
		TierXPins.Add(tierXPin);
	}

	public EventPin PopulateTierX(ThemeLayout Theme, PinScheduleConfiguration pinSchedule)
	{
		if (TierXManager.Instance.ThemeDescriptor.PinDetails.Count < 1)
		{
		}
		List<PinDetail> pins = TierXManager.Instance.GetPins();
		TierXManager.Instance.IsPizzaPinActive = false;
		bool workshopPinActive = false;
		foreach (PinDetail current in pins)
		{
			switch (current.GetPinType())
			{
			case PinDetail.PinType.DAILYBATTLE:
			{
				RaceEventData dailyBattleEvent = RaceEventQuery.Instance.GetDailyBattleEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(eCarTier.TIER_X), false);
				if (dailyBattleEvent == null)
				{
					dailyBattleEvent = RaceEventQuery.Instance.GetDailyBattleEvent(GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(eCarTier.TIER_X), true);
				}
				EventPin eventPin = CreateTierXPin(pinSchedule, current);
				eventPin.GetComponent<EventPin>().Setup(dailyBattleEvent);
				eventPin.AddTierXCallbacks(this);
				EventPins.Add(eventPin.gameObject);
				break;
			}
			case PinDetail.PinType.MECHANIC:
            //this.CreateCustomPin(CustomPin.CustomType.Mechanic, current.Position.AsUnityVector2(), current.PositionOffset.AsUnityVector2());
				break;
			case PinDetail.PinType.PIZZAPIN:
				TierXManager.Instance.IsPizzaPinActive = true;
				break;
			case PinDetail.PinType.MULTIPLAYERPIN:
				break;
			case PinDetail.PinType.WORKSHOPPIN:
				workshopPinActive = true;
				break;
			default:
				if (TierXManager.Instance.ThemeDescriptor.UseButtonsForPins)
				{
                    //HubEventPin hubEventPin = this.CreateHubEventPin(pinSchedule, current);
                    //this.HubEventPins.Add(hubEventPin.gameObject);
				}
				else
				{
				    EventPin eventPin = CreateTierXPin(pinSchedule, current);
				    eventPin.type = ProgressionMapPinEventType.WORLD_TOURS;
				    eventPin.Name = current.PinID;
                    eventPin.AddTierXCallbacks(this);
					EventPins.Add(eventPin.gameObject);
                    MapPins.Add(eventPin);
					if (current.WorldTourScheduledPinInfo != null && current.WorldTourScheduledPinInfo.ShowAnimationIn)
					{
						if (Theme.IsOverviewTheme)
						{
                            //AnimationUtils.PlayLastFrame(eventPin.appearAnimation);
						}
						else
						{
                            //eventPin.gameObject.transform.localScale.Scale(current.WorldTourScheduledPinInfo.AppearAnimationInitialScale.AsUnityVector3());
							//eventPin.gameObject.transform.localScale = Vector3.one;
							string text = current.WorldTourScheduledPinInfo.SelectAnimationIn();
                            //AnimationUtils.PlayFirstFrame(eventPin.appearAnimation, text);
							StartCoroutine(PlayDelayedPinAnimation(eventPin.appearAnimation, text, Random.Range(0.25f, 0.5f)));
						}
					}
				}
				break;
			}
		}
		EventPin selectedtEventPin = null;
		if (Theme.UseButtonsForPins)
		{
			MapScreen.HidePinSelection();
		}
		else if (EventPins.Any())
		{
			MapScreen.ShowPinSelection();
			TierXPin tierXPin = null;
			string lastSelectedPinID = PlayerProfileManager.Instance.ActiveProfile.GetLastSelectedPinID(TierXManager.Instance.CurrentThemeName);
			for (int i = 0; i < EventPins.Count; i++)
			{
				EventPin eventPin = EventPins[i].GetComponent<EventPin>();
				if (eventPin!= null)
				{
					if (!eventPin.IsCompleted())
					{
						TierXPin tierxPin = eventPin.GetComponent<TierXPin>();
						if (tierxPin != null)
						{
							if (tierxPin.pinDetails.IsSelectable)
							{
								if (selectedtEventPin == null || tierxPin.pinDetails.PinID == lastSelectedPinID)
								{
									selectedtEventPin = eventPin;
									tierXPin = tierxPin;
									if (string.IsNullOrEmpty(lastSelectedPinID) || tierxPin.pinDetails.PinID == lastSelectedPinID)
									{
										break;
									}
								}
							}
						}
					}
				}
			}
			if (Theme.IsOverviewTheme)
			{
			    //These lines of code cause eventpane show up right after the current theme refreshed but we don't want this behaviour
                //this.MapScreen.SetUpTierXPanePreview(selectedtEventPin, tierXPin);
            }
            else if (selectedtEventPin != null)
			{
				if (selectedtEventPin.EventData == null)
				{
					if (selectedtEventPin.GroupData != null)
					{
						for (int j = 0; j < selectedtEventPin.GroupData.RaceEvents.Count; j++)
						{
                            selectedtEventPin.GroupData.RaceEvents[j].SetWorldTourPinPinDetail(tierXPin.pinDetails);
                        }
					}
				}
				else
				{
				    selectedtEventPin.EventData.SetWorldTourPinPinDetail(tierXPin.pinDetails);
                }
                //These line of code cause eventpane show up right after theme refreshed but we don't want this behaviour
			 //   if (tierXPin.pinDetails != null)
				//{
				//	if (tierXPin.pinDetails.GroupID != 0)
				//	{
				//		MapScreen.OnTierXGroupSelected(selectedtEventPin, true);
				//	}
				//	else
				//	{
    //                    selectedtEventPin.EventData.SetWorldTourPinPinDetail(tierXPin.pinDetails);
    //                    MapScreen.OnTierXEventSelected(selectedtEventPin, true);
				//	}
				//}
			}
		}
		CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
		if (careerModeMapScreen != null)
		{
			if (TierXManager.Instance.ThemeDescriptor.ShowTitle)
			{
				careerModeMapScreen.SetupTierText(TierXManager.Instance.ThemeDescriptor.Name, true);
			}
			else
			{
				careerModeMapScreen.SetupTierText(string.Empty, true);
			}
			if (TierXManager.Instance.ThemeDescriptor.ShowDescription)
			{
				careerModeMapScreen.SetupTierDescriptionText(TierXManager.Instance.ThemeDescriptor.Description, false);
			}
			careerModeMapScreen.SetWorkshopPinActive(workshopPinActive);
		}
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		if (!TierXManager.Instance.IsOverviewThemeActive())
		{
            this.worldTourThemeMapConditional = new WorldTourThemeMapConditional();
			worldTourThemeMapConditional.EvaluateAll();
			FlowConditionBase nextValidCondition = worldTourThemeMapConditional.GetNextValidCondition();
			if (nextValidCondition != null)
			{
				PopUp popup = nextValidCondition.GetPopup();
				if (popup != null)
				{
                    bool flag = PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
				}
			}
		}
		TierXManager.Instance.SetupThemeAnimations(this);
        IGameState gameState = new GameStateFacade();
		foreach (PinDetail current2 in pins)
		{
			if (!current2.IsPartOfTimeline())
			{
                gameState.SetPinToShown(TierXManager.Instance.CurrentThemeName, current2.WorldTourScheduledPinInfo);
			}
		}
		return selectedtEventPin;
	}

	private IEnumerator PlayDelayedPinAnimation(Animation anim, float delay)
	{
	    yield return 0;
	    //CareerModeMapEventSelect.<PlayDelayedPinAnimation>c__Iterator27 <PlayDelayedPinAnimation>c__Iterator = new CareerModeMapEventSelect.<PlayDelayedPinAnimation>c__Iterator27();
	    //<PlayDelayedPinAnimation>c__Iterator.delay = delay;
	    //<PlayDelayedPinAnimation>c__Iterator.anim = anim;
	    //<PlayDelayedPinAnimation>c__Iterator.<$>delay = delay;
	    //<PlayDelayedPinAnimation>c__Iterator.<$>anim = anim;
	    //return <PlayDelayedPinAnimation>c__Iterator;
	}

	private IEnumerator PlayDelayedPinAnimation(Animation anim, string selectedAnimation, float delay)
	{
	    yield return 0;
	    //CareerModeMapEventSelect.<PlayDelayedPinAnimation>c__Iterator28 <PlayDelayedPinAnimation>c__Iterator = new CareerModeMapEventSelect.<PlayDelayedPinAnimation>c__Iterator28();
	    //<PlayDelayedPinAnimation>c__Iterator.delay = delay;
	    //<PlayDelayedPinAnimation>c__Iterator.anim = anim;
	    //<PlayDelayedPinAnimation>c__Iterator.selectedAnimation = selectedAnimation;
	    //<PlayDelayedPinAnimation>c__Iterator.<$>delay = delay;
	    //<PlayDelayedPinAnimation>c__Iterator.<$>anim = anim;
	    //<PlayDelayedPinAnimation>c__Iterator.<$>selectedAnimation = selectedAnimation;
	    //return <PlayDelayedPinAnimation>c__Iterator;
	}

	private GameObject CreatePinGameObject(string prefab)
	{
		GameObject item = UICacheManager.Instance.GetItem(prefab, false);
	    item.transform.SetParent(MapScreenCache.MapCanvas.transform, false);
		return item;
	}

	private GameObject CreatePinGameObjectAndPositionOnScreen(string prefab, PinDetail pin)
	{
		GameObject pinObject = CreatePinGameObject(prefab);
	    pinObject.GetComponent<RectTransform>().anchoredPosition = pin.Position.AsUnityVector2();
        //CareerModeMapEventSelect.PositionPinOnScreen(pinObject, pin.Position.AsUnityVector2(), pin.PositionOffset.AsUnityVector2());
        return pinObject;
	}

    //private HubEventPin CreateHubEventPin(PinScheduleConfiguration pinSchedule, PinDetail pin)
    //{
    //    GameObject gameObject = this.CreatePinGameObjectAndPositionOnScreen("Career/HubEventPin", pin);
    //    HubEventPin component = gameObject.GetComponent<HubEventPin>();
    //    component.Setup(pin);
    //    this.AttachTierXPin(gameObject, pin);
    //    return component;
    //}

	private EventPin CreateTierXPin(PinScheduleConfiguration pinSchedule, PinDetail pin)
	{
        bool isHyperEvent = false;
	    string prefab = pin.Label.Contains("WorldTour") ? "Career/WorldTourPin_Bold" : "Career/WorldTourPin";
        GameObject pinObject = CreatePinGameObjectAndPositionOnScreen(prefab, pin);
	    pinObject.name = pin.PinID;
		EventPin eventPin = pinObject.GetComponent<EventPin>();
		int eventID = pin.EventID;
		RaceEventData raceEventData = (eventID == 0) ? null : GameDatabase.Instance.Career.GetEventByEventIndex(eventID);
		eventPin.EventData = raceEventData;
		int groupID = pin.GroupID;
		RaceEventGroup eventGroupsByGroupID = RaceEventQuery.Instance.GetEventGroupsByGroupID(eCarTier.TIER_X, groupID);
		eventPin.GroupData = eventGroupsByGroupID;
		bool isUnlock = !pin.IsLocked();
		if ((pin.IsSelectable || isUnlock) && !string.IsNullOrEmpty(pin.GetOverlaySprite()) && pin.GetOverlaySprite().ToLower().Contains("lock-icon"))
		{
			pin.Textures[PinDetail.TextureKeys.PinOverlay.ToString()].Name = string.Empty;
		}
        Texture2D overlayTex;
        if (string.IsNullOrEmpty(pin.GetOverlaySprite()))
		{
            overlayTex = null;
        }
		else if (TierXManager.Instance.PinTextures.ContainsKey(pin.GetOverlaySprite()))
		{
            overlayTex = TierXManager.Instance.PinTextures[pin.GetOverlaySprite()];
        }
		else
		{
            overlayTex = (Texture2D)Resources.Load(pin.GetOverlaySprite());
        }
        Texture2D sprite;
        var bossSprite = pin.GetBossSprite();
        if (string.IsNullOrEmpty(bossSprite))
		{
            sprite = null;
        }
		else
		{
            sprite = TierXManager.Instance.GetBossTexture(bossSprite);
            isHyperEvent = bossSprite.ToLower().Contains("hyper");
        }
        Texture2D backgroundTex;
        if (string.IsNullOrEmpty(pin.GetBackground()))
		{
            backgroundTex = null;
        }
		else if (TierXManager.Instance.PinBackgrounds.ContainsKey(pin.GetBackground()))
		{
            backgroundTex = TierXManager.Instance.PinBackgrounds[pin.GetBackground()];
        }
		else
		{
            backgroundTex = (Texture2D)Resources.Load(pin.GetBackground());
        }
        bool hasEventData = raceEventData != null;
        IGameState gameState = new GameStateFacade();
        BubbleMessageData bubbleMessageData = null;
        if (pin.WorldTourScheduledPinInfo != null)
		{
            bubbleMessageData = pin.WorldTourScheduledPinInfo.PinBubbleMessage;
        }
        Fraction progression = default(Fraction);
        ProgressionVisualisation progressionVisualisation = new ProgressionVisualisation();
        if (pin.WorldTourScheduledPinInfo != null)
		{
            progressionVisualisation = pin.WorldTourScheduledPinInfo.ProgressionVisualisation;
            if (progressionVisualisation.GetViewStyleAsEnum() == ProgressBarStyle.None && pin.ProgressIndicator)
            {
                progressionVisualisation = new ProgressionVisualisation
                {
                    Accumulator = "ProgressionThroughCurrentSequence",
                    ViewStyleString = ProgressBarStyle.Bar.ToString()
                };
            }
            string iD = pin.WorldTourScheduledPinInfo.ParentSequence.ID;
            progression = ProgressionAccumulator.Accumulate(progressionVisualisation, iD, TierXManager.Instance.ThemeDescriptor.ID, pinSchedule);
        }
		if (pinSchedule != null && pin.ProgressIndicator && pin.WorldTourScheduledPinInfo != null && pin.WorldTourScheduledPinInfo.ParentSequence != null)
		{
			string iD2 = pin.WorldTourScheduledPinInfo.ParentSequence.ID;
			if (TierXManager.Instance.ThemeDescriptor.EventIDsForAnimation.Contains(iD2))
			{
                MapScreenCache.WorldTourBossPin.SetEventPinForPiece(eventPin, iD2);
            }
		}
		bool isCompleted = pin.WorldTourScheduledPinInfo != null && pin.CanDisplayAsComplete && gameState.IsPinWon(TierXManager.Instance.CurrentThemeName, pin.WorldTourScheduledPinInfo);
        string textID = !isCompleted || string.IsNullOrEmpty(pin.CompletedTitle) ? pin.Title : pin.CompletedTitle;
        eventPin.SetupTierX(pin,raceEventData, hasEventData, backgroundTex, overlayTex, sprite, LocalizationManager.GetTranslation(textID), progressionVisualisation.GetViewStyleAsEnum(), isUnlock, bubbleMessageData, progression, isHyperEvent);
        AttachTierXPin(eventPin.gameObject, pin);
		eventPin.gameObject.SetActive(true);
		if (isCompleted)
		{
			string iD3 = pin.WorldTourScheduledPinInfo.ParentSequence.ID;
			bool eventIdHasAnimation = TierXManager.Instance.ThemeDescriptor.EventIDsForAnimation.Contains(iD3);
			bool isAnimationCompleted = false;
			if (eventIdHasAnimation)
			{
				isAnimationCompleted = PlayerProfileManager.Instance.ActiveProfile.IsAnimationCompletedForWorldTourEventID(TierXManager.Instance.ThemeDescriptor.ID, iD3);
			}
            if (!eventIdHasAnimation || isAnimationCompleted)
            {
                eventPin.SetupCompleted(true);
            }
        }
		if (pin.IsPreviousRaceInTimeline())
		{
			eventPin.SetupForPreviousInTimeline(pin.TimelineDetails);
		}
		else if (pin.IsNextRaceInTimeline())
		{
			eventPin.SetupForNextInTimeline(pin.TimelineDetails);
		}
        string currentThemeName = TierXManager.Instance.CurrentThemeName;
        IGameState gameState2 = new GameStateFacade();
        ThemeCompletionLevel worldTourThemeCompletionLevel = gameState2.GetWorldTourThemeCompletionLevel(currentThemeName);
        if (pin.IsProgressPin || worldTourThemeCompletionLevel == ThemeCompletionLevel.LEVEL_2)
        {
            MapScreenCache.WorldTourBossPin.SetUpProgressPinPosition(gameObject.transform.position);
        }
        return eventPin;
	}

	public void OnTierXPinPress(EventPin eventPin)
	{
		if (eventPin.IsCompleted() || eventPin.IsTapDisabled)
		{
			return;
		}
		TierXPin tierXPin = eventPin.GetComponent<TierXPin>();
		if (tierXPin != null && tierXPin.pinDetails.PushScreenAction == null)
		{
			PlayerProfileManager.Instance.ActiveProfile.RecordSelectedPinID(TierXManager.Instance.CurrentThemeName, tierXPin.pinDetails.PinID);
		}
		if (eventPin.EventData != null && eventPin.EventData.IsRelay)
		{
			RelayManager.ResetRelayData();
		}
		if ((eventPin.IsGroupPin || (eventPin.EventData!=null && eventPin.EventData.IsRelay)) && eventPin.GroupData != null)
		{
		    eventPin.GroupData.RaceEvents.ForEach(delegate (RaceEventData q)
            {
                q.SetWorldTourPinPinDetail(tierXPin.pinDetails);
            });
		}
		if (TierXManager.Instance.IsOverviewThemeActive())
		{
            this.MapScreen.SetUpTierXPanePreview(eventPin, tierXPin);
        }
		else if (tierXPin.pinDetails.PushScreenAction != null && !string.IsNullOrEmpty(tierXPin.pinDetails.PushScreenAction.ScreenID))
		{
			if (!TouchManager.AttemptToUseButton(eventPin.name))
			{
				return;
			}
			TouchManager.DisableButtonsFor(0.5f);
            ScreenID zID = EnumHelper.FromString<ScreenID>(tierXPin.pinDetails.PushScreenAction.ScreenID);
            ScreenManager.Instance.PushScreen(zID);
        }
		else if (eventPin.IsGroupPin)
		{
			MapScreen.OnTierXGroupSelected(eventPin, false);
		}
		else
		{
		    eventPin.EventData.SetWorldTourPinPinDetail(tierXPin.pinDetails);
            MapScreen.OnTierXEventSelected(eventPin, false);
		}
	}

	public void OnRestrictionPressed(RaceEventData e)
	{
		IRestriction activeRestriction = RestrictionHelper.GetActiveRestriction(e);
		if (activeRestriction is GasRequired)
		{
			return;
		}
		backupEventData = e;
	}

	public void TryRestoreSelectedEvent()
	{
		if (backupEventData != null)
		{
			RestoreBackupEventData();
		}
	}

	private void RestoreBackupEventData()
	{
		EventPin[] componentsInChildren = gameObject.GetComponentsInChildren<EventPin>();
		EventPin[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			EventPin eventPin = array[i];
			if (eventPin.EventData == backupEventData)
			{
				MapScreen.ResetHighlight();
				//if (CareerModeMapScreen.mapPaneSelected != 5)
				//{
				//	MapScreen.OnEventSelected(backupEventData, true);
				//	return;
				//}
				if (eventPin.IsCompleted())
				{
					return;
				}
				TierXPin component = eventPin.gameObject.GetComponent<TierXPin>();
				if (component == null)
				{
					return;
				}
                PinDetail worldTourPinPinDetail = eventPin.EventData.GetWorldTourPinPinDetail();
                ScheduledPin worldTourScheduledPinInfo = worldTourPinPinDetail.WorldTourScheduledPinInfo;
                if (worldTourScheduledPinInfo == null)
                {
                    return;
                }
                GameStateFacade gameStateFacade = new GameStateFacade();
                if (worldTourPinPinDetail.CanDisplayAsComplete && gameStateFacade.IsPinWon(TierXManager.Instance.CurrentThemeName, worldTourScheduledPinInfo))
                {
                    return;
                }
                eventPin.EventData.SetWorldTourPinPinDetail(component.pinDetails);
                MapScreen.OnTierXEventSelected(eventPin, true);
			}
		}
	}

	public void ResetThemeAnimation()
	{
        //GetComponent<Animation>().Stop();
        //if (GetComponent<Animation>().GetClip("ThemeAnimation") != null)
        //{
        //    GetComponent<Animation>().RemoveClip("ThemeAnimation");
        //}
	}

	public bool SetupThemeAnimation(ThemeAnimationDetail themeAnimation, Dictionary<string, GameObject> pinAnimations)
	{
		IGameState gameState = new GameStateFacade();
		AnimationClip animationClip = themeAnimation.GetAnimationClip(gameState);
		GetComponent<Animation>().AddClip(animationClip, "ThemeAnimation");
		IOrderedEnumerable<PinAnimationDetail> orderedEnumerable = from pd in themeAnimation.PinAnimations
		orderby pd.EventTime
		select pd;
		//HashSet<string> hashSet = new HashSet<string>();
		foreach (PinAnimationDetail current in orderedEnumerable)
		{
            //TierXPin tierXPinWithLabel = this.GetTierXPinWithLabel(current.PinLabel);
            //if (tierXPinWithLabel == null)
            //{
            //    if (current.Required)
            //    {
            //        bool result = false;
            //        return result;
            //    }
            //}
            //else
            //{
            //    if (!pinAnimations.ContainsKey(current.Name))
            //    {
            //        bool result = false;
            //        return result;
            //    }
            //    tierXPinWithLabel.SetupPinAnimation(current, pinAnimations[current.Name]);
            //    if (!hashSet.Contains(current.PinLabel))
            //    {
            //        AnimationUtils.PlayFirstFrame(tierXPinWithLabel.GetComponent<Animation>(), current.Name);
            //        hashSet.Add(current.PinLabel);
            //    }
            //}
		}
		return true;
	}

	public void PlayPinAnimation(string labelAndAnimation)
	{
		//string[] array = labelAndAnimation.Split(':');
		//string label = array[0];
		//string name = array[1];
        //TierXPin tierXPinWithLabel = this.GetTierXPinWithLabel(label);
        //if (tierXPinWithLabel != null)
        //{
        //    AnimationUtils.PlayAnim(tierXPinWithLabel.GetComponent<Animation>(), name);
        //}
	}

	public void EnableMapScreenPinSelection()
	{
		MapScreen.EnablePinSelection();
	}

	public void DisableMapScreenPinSelection()
	{
		MapScreen.DisablePinSelection();
	}

	public void BlockTutorialBubbles()
	{
		MapScreen.BlockTutorialBubbles();
	}

	public void UnblockTutorialBubbles()
	{
		MapScreen.UnblockTutorialBubbles(false);
		TutorialBubblesManager.Instance.TriggerEvent(TutorialBubblesEvent.RefreshThemeMap);
	}

	public void InitEventHubOpponentsAnimation()
	{
		if (MapScreenCache.InternationalEventHubGO == null)
		{
		}
        //MapScreenCache.InternationalEventHubGO.GetComponentInChildren<EventHubOpponents>().InitOpponentsAnimation();
	}

	public void PlayEventHubOpponentsAnimation()
	{
		if (MapScreenCache.InternationalEventHubGO == null)
		{
		}
        //AnimationUtils.PlayAnim(MapScreenCache.InternationalEventHubGO.GetComponentInChildren<EventHubOpponents>().animation);
	}

    public IMapPin GetMapPin(ProgressionMapPinEventType type)
    {
        return MapPins.FirstOrDefault(p => p.type == type);
    }

    public IMapPin GetMapPin(ProgressionMapPinEventType type,eCarTier tier,string pinName = null)
    {
        if (string.IsNullOrEmpty(pinName))
            return MapPins.FirstOrDefault(p => p.type == type && p.tier == tier);
        else
        {
            return MapPins.FirstOrDefault(p => p.type == type && p.Name == pinName);
        }
    }

    public void SetAllMApPinsInteractable(bool value)
    {
        foreach (var mapPin in MapPins)
        {
            mapPin.interactable = value;
        }
    }

    private static RectTransform dummyRect;

    public Vector3 GetTierPosition(int tier)
    {
        if (dummyRect == null)
        {
            var dummy = new GameObject("dummyRect");
            dummy.transform.SetParent(MapScreenCache.MapCanvas.transform, false);
            dummy.SetActive(false);
            dummyRect = dummy.AddComponent<RectTransform>();
        }

        dummyRect.anchoredPosition3D = TierPosition[tier];
        return dummyRect.position;
    }

    void OnDestroy()
    {
        if (dummyRect != null)
        {
            Destroy(dummyRect.gameObject);
            dummyRect = null;
        }
    }
}
