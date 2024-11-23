using System;

public class CrewTauntChatter
{
	public static bool PostRace(PopUpButtonAction methodToInvoke, RaceEventData raceEvent, bool playerWon)
	{
		if (playerWon)
		{
			return false;
		}
		if (raceEvent == null)
		{
			return false;
		}
		if (raceEvent.Parent == null)
		{
			return false;
		}
		eCarTier carTier = raceEvent.Parent.GetTierEvents().GetCarTier();
		if (raceEvent.IsCrewRace())
		{
		    var member = RaceEventInfo.Instance.CurrentEvent.EventOrder + 1;
			PopUpManager.Instance.TryShowPopUp(CrewTauntChatter.CrewMember_YouLose(methodToInvoke, carTier,member), PopUpManager.ePriority.Default, null);
			return true;
		}
		if (raceEvent.IsBossRace())
		{
			PopUpManager.Instance.TryShowPopUp(CrewTauntChatter.CrewBoss_YouLose(methodToInvoke, carTier), PopUpManager.ePriority.Default, null);
			return true;
		}
		return false;
	}

	public static PopUp CrewMember_YouLose(PopUpButtonAction methodToInvoke, eCarTier carTier,int member)
	{
		string crewLostRaceTitle = CrewChatter.GetCrewLostRaceTitle((int)carTier);
		string crewLostRace = CrewChatter.GetCrewLostRace((int)carTier);
        return CrewTauntChatter.CrewLosePopUp(crewLostRaceTitle, crewLostRace, methodToInvoke, carTier, member);
	}

	public static PopUp CrewBoss_YouLose(PopUpButtonAction methodToInvoke, eCarTier carTier)
	{
		string bossLoseRaceTitle = CrewChatter.GetBossLoseRaceTitle((int)carTier);
		string bossLoseRace = CrewChatter.GetBossLoseRace((int)carTier);
		return CrewTauntChatter.CrewLosePopUp(bossLoseRaceTitle, bossLoseRace, methodToInvoke, carTier);
	}

    private static PopUp CrewLosePopUp(string title, string body, PopUpButtonAction methodToInvoke, eCarTier carTier,
        int member = -1)
    {
        return new PopUp
        {
            Title = title,
            TitleAlreadyTranslated = true,
            BodyText = body,
            BodyAlreadyTranslated = true,
            IsBig = true,
            ConfirmAction = methodToInvoke,
            ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = 
                member == -1 ? Chatter.GetCrewLeaderGraphic(carTier) : Chatter.GetCrewMemberGraphic(carTier, member),
            //BundledGraphicPath = Chatter.GetCrewLeaderGraphic(carTier),
            ImageCaption = 
                member == -1
                    ? CrewChatter.GetLeaderName((int) carTier)
                    : CrewChatter.GetMemberName((int) carTier, member),
            ShouldCoverNavBar = true,
            IsCrewLeader = true,
            BossTier = (int) carTier
        };
    }
}
