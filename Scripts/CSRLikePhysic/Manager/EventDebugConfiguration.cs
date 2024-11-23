using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDebugConfiguration : ScriptableObject
{
    public bool ShowAllMapPins;
    public bool DontShowAnyProgressionPopup;
    public bool UseDebugEvents;
    public bool UseDebugCarePackage;

    public int TutorialEventIndex;
    public BaseTierEventDebug Tier1Events;
    public BaseTierEventDebug Tier2Events;
    public BaseTierEventDebug Tier3Events;
    public BaseTierEventDebug Tier4Events;
    public BaseTierEventDebug Tier5Events;
    public string CarePackageID;
    public int CarePackageLevel;

    public void ResetAllEvents()
    {
        ResetTierEvents(Tier1Events);
        ResetTierEvents(Tier2Events);
        ResetTierEvents(Tier3Events);
        ResetTierEvents(Tier4Events);
        ResetTierEvents(Tier5Events);
    }

    private void ResetTierEvents(BaseTierEventDebug tierEvents)
    {
        tierEvents.LadderEventIndex = -1;
        tierEvents.SpecificEventIndex = -1;
        tierEvents.CrewMemberEventIndex = -1;
        tierEvents.Active = false;
    }
}


[System.Serializable]
public class BaseTierEventDebug
{
    public bool Active;
    public int CrewMemberEventIndex = -1;
    public int LadderEventIndex = -1;
    public int SpecificEventIndex = -1;
}
