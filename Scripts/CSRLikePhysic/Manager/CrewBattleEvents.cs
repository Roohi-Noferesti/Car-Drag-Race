using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public class CrewBattleEvents : RaceEventTopLevelCategory
{
	public override string ToString()
	{
		string str = "    CrewBattleEvents\n";
		return str + base.ToString();
	}

    public override string GetName()
    {
        return "CrewBattle";
    }

	public override string GetProgressBarText(RaceEventData zEvent)
	{
		return LocalizationManager.GetTranslation("TEXT_EVENTTYPE_CREW_BATTLE");
	}

	public override Vector2 GetPinPosition()
	{
		return new Vector2(250.7f, 190.5f);
	}

	public override Vector2 GetOverlayOffset()
	{
		return new Vector2(0f, 0.06f);
	}

	public override string GetPinString(RaceEventData zEvent)
	{
		if (zEvent.Parent.GetTierEvents().GetCarTier() == eCarTier.TIER_5)
		{
			return LocalizationManager.GetTranslation("TEXT_RACE_PIN_CREW_BATTLE_FINAL");
		}
		return LocalizationManager.GetTranslation("TEXT_RACE_PIN_CREW_BATTLE");
	}

	public override string GetOverlayTextureName(RaceEventData zEvent)
	{
		return string.Empty;
	}

	public override string GetBossTextureName(RaceEventData zEvent)
	{
		eCarTier tier = zEvent.Parent.GetTierEvents().GetCarTier();
	    var text = CarTierHelper.TierToString[(int) tier];
	    return "CrewPortraitsTier" + text + ".Boss Card_s" + (tier==eCarTier.TIER_2 && BasePlatform.ActivePlatform.ShouldShowFemaleHair? "_World":"");
        //return "CrewPins/crew_leader_pin_" + text.Substring(text.Length - 1);
	}

	public override string GetBackgroundTextureName(RaceEventData zEvent)
	{
		return string.Empty;
	}

	public override RaceEventTypeMultipliers GetRaceEventTypeRewardMultipliers(RewardsMultipliers rewardsMultipliers, RaceEventData raceEvent)
	{
		if (!raceEvent.IsFinalRaceInGroup())
		{
			return rewardsMultipliers.CrewBattleRaceMultipliers;
		}
		RaceEventTypeMultipliers finalCrewBattleMultipliers = rewardsMultipliers.FinalCrewBattleMultipliers;
		if (finalCrewBattleMultipliers == null)
		{
			return rewardsMultipliers.CrewBattleRaceMultipliers;
		}
		return finalCrewBattleMultipliers;
	}
}
