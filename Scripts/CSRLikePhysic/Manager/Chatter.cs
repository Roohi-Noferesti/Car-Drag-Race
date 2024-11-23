using System;

public class Chatter
{
    public static bool PostRace(PopUpButtonAction methodToInvoke, RaceEventData raceEvent, bool playerWon)
    {
        return raceEvent != null && raceEvent.Parent != null &&
               ((!playerWon && CrewTauntChatter.PostRace(methodToInvoke, raceEvent, playerWon)) ||
                LadderChatter.PostRace(methodToInvoke, raceEvent, playerWon));
    }

    public static bool PreRace(PopUpButtonAction methodToInvoke, RaceEventData raceEvent)
	{
		return raceEvent != null && raceEvent.Parent != null && LadderChatter.PreRace(methodToInvoke, raceEvent);
	}

	public static string GetCrewLeaderGraphic(eCarTier carTier)
	{
		switch (carTier)
		{
		case eCarTier.TIER_1:
			return PopUpManager.Instance.graphics_crewLeaderTier1;
		case eCarTier.TIER_2:
			return PopUpManager.Instance.graphics_crewLeaderTier2 + (BasePlatform.ActivePlatform.ShouldShowFemaleHair ? "_World" : "");
		case eCarTier.TIER_3:
			return PopUpManager.Instance.graphics_crewLeaderTier3;
		case eCarTier.TIER_4:
			return PopUpManager.Instance.graphics_crewLeaderTier4;
		case eCarTier.TIER_5:
			return PopUpManager.Instance.graphics_crewLeaderTier5;
		default:
			return PopUpManager.Instance.graphics_crewLeaderTier1;
		}
	}


    public static string GetCrewMemberGraphic(eCarTier carTier,int member)
    {
        if (member == 4)
            member = 3;
        if (carTier == eCarTier.TIER_2) {
	        return "CrewPortraitsTier" + CarTierHelper.TierToString[(int)carTier] + ".Crew " + member + (BasePlatform.ActivePlatform.ShouldShowFemaleHair ? "_World" : "");
        } else {
	        return "CrewPortraitsTier" + CarTierHelper.TierToString[(int)carTier] + ".Crew " + member;
        }
    }

    public static string GetCrewLeaderName(eCarTier carTier)
    {
        switch (carTier)
        {
            case eCarTier.TIER_1:
                return "TEXT_NAME_CREWLEADER_TIER1";
            case eCarTier.TIER_2:
                return "TEXT_NAME_CREWLEADER_TIER2";
            case eCarTier.TIER_3:
                return "TEXT_NAME_CREWLEADER_TIER3";
            case eCarTier.TIER_4:
                return "TEXT_NAME_CREWLEADER_TIER4";
            case eCarTier.TIER_5:
                return "TEXT_NAME_CREWLEADER_TIER5";
            default:
                return "TEXT_NAME_CREWLEADER_TIER1";
        }
    }
}
