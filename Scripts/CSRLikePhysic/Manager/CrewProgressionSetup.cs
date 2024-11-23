using KingKodeStudio;
using Metrics;

public class CrewProgressionSetup
{
	public static bool PreRaceSetupCrewProgressionScreen()
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (!currentEvent.IsCrewBattle())
		{
			return false;
		}
		int crew = GetCrew(currentEvent);
	    CrewProgressionScreen.BackgroundImageText = null;
        CrewProgressionScreen.Crew = crew;
        bool crewProgressionIntroductionPlayed = PlayerProfileManager.Instance.ActiveProfile.CrewProgressionIntroductionPlayed[crew];
		CrewProgressionScreen crewProgressionScreen = Activate(ScreenID.Invalid);
		if (currentEvent.IsFirstCrewMemberRace() && !crewProgressionIntroductionPlayed)
		{
            crewProgressionScreen.SetupIntroduction(crew);
            PlayerProfileManager.Instance.ActiveProfile.CrewProgressionIntroductionPlayed[crew] = true;
			Log.AnEvent(Events.IntroToCrews);
		}
		else if (currentEvent.IsCrewRace())
		{
		    int member = GetMember(currentEvent);
            crewProgressionScreen.SetupRaceAgainstCrewMember(crew, member);
        }
        else
		{
			if (!currentEvent.IsBossRace())
			{
				return false;
			}
			int zBossRaceIndex = currentEvent.BossRaceIndex();
            crewProgressionScreen.SetupRaceAgainstLeader(crew, zBossRaceIndex);

            //for testing
            //crewProgressionScreen.SetupLeaderDefeated(crew, 1);
		}
		return true;
	}

    public static bool CheckPostRaceCrewProgression()
    {
        RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
        return currentEvent != null && currentEvent.IsCrewBattle() && RaceResultsTracker.You != null &&
               RaceResultsTracker.You.IsWinner &&
               RaceResultsTracker.You.EventID == RaceEventInfo.Instance.CurrentEvent.EventID &&
               (currentEvent.IsCrewRace() || currentEvent.IsBossRace());
    }

    public static bool PostRaceSetupCrewProgressionScreen(ScreenID underScreen = ScreenID.Invalid)
	{
		if (!CheckPostRaceCrewProgression())
		{
			return false;
		}
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		int crew = GetCrew(currentEvent);
		CrewProgressionScreen crewProgressionScreen = Activate(underScreen);
		if (currentEvent.IsCrewRace())
		{
			int member = GetMember(currentEvent);
			crewProgressionScreen.SetupCrewMemberDefeated(crew, member);
		}
		else
		{
			if (!currentEvent.IsBossRace())
			{
				return false;
			}
			int num = currentEvent.BossRaceIndex();
			if (num < 2)
			{
				crewProgressionScreen.SetupLeaderDefeatedStrike(crew, num);
			}
			//else if (currentEvent.Parent.GetTierEvents().GetCarTier() == eCarTier.TIER_5)
			//{
			//	crewProgressionScreen.SetupLeaderErrolDefeated(3);
			//}
			else
			{
				crewProgressionScreen.SetupLeaderDefeated(crew, num);
			}
		}
		return true;

	}

	public static bool PreRaceSetupForNarrativeScene(ScreenID underScreen = ScreenID.Invalid)
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		NarrativeScene scene = null;
		if (!currentEvent.GetPreRaceSceneToDisplay(out scene))
		{
			return false;
		}

        if (scene.CharactersDetails.CharacterGroups.Count > 0 &&
            !string.IsNullOrEmpty(scene.CharactersDetails.CharacterGroups[0].LogoTextureName))
        {
            CrewProgressionScreen.BackgroundImageText = scene.CharactersDetails.CharacterGroups[0].LogoTextureName;
        }

        CrewProgressionScreen crewProgressionScreen = Activate(underScreen);
		crewProgressionScreen.SetupForNarrativeScene(scene);
		return true;
	}

	public static bool PostRaceSetupForNarrativeScene(ScreenID underScreen = ScreenID.Invalid)
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent == null)
		{
			return false;
		}
		if (RaceResultsTracker.You == null)
		{
			return false;
		}
		if (RaceResultsTracker.You.EventID != RaceEventInfo.Instance.CurrentEvent.EventID)
		{
			return false;
		}
		if (currentEvent.IsHighStakesEvent())
		{
			if (underScreen != ScreenID.Invalid || !ScreenManager.Instance.IsScreenOnStack(underScreen))
			{
                ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.HighStakesFinished, new[]{underScreen});
			}
			else
			{
                ScreenManager.Instance.PushScreen(ScreenID.HighStakesFinished);
			}
            //ScreenManager.Instance.UpdateImmediately();
			return true;
		}
		NarrativeScene scene = null;
		if (RaceResultsTracker.You.IsWinner)
		{
			if (!currentEvent.GetPostRaceSceneWinToDisplay(out scene))
			{
				return false;
			}
		}
		else if (!currentEvent.GetPostRaceSceneLoseToDisplay(out scene))
		{
			return false;
		}
		CrewProgressionScreen crewProgressionScreen = Activate(underScreen);
		crewProgressionScreen.SetupForNarrativeScene(scene);
		return true;
	}

	public static int GetCrew(RaceEventData zRaceEvent)
	{
		return (int)zRaceEvent.Parent.GetTierEvents().GetCarTier();
	}

	private static int GetMember(RaceEventData zRaceEvent)
	{
		return zRaceEvent.GetProgressionRaceEventNumber();
	}

    private static CrewProgressionScreen Activate(ScreenID underScreen = ScreenID.Invalid)
    {
        //if (underScreen != ScreenID.Invalid)
        //{
        //    ScreenManager.Instance.PushScreenWithFakedHistory(underScreen);
        //}

        ////ScreenManager.Active.DestroyScreen(ScreenID.CrewProgression);

        //return null;//(CrewProgressionScreen) ScreenManager.Active.pushScreen(ScreenID.CrewProgression);
        ////if (ScreenManager.Active.HasScreen(ScreenID.CrewProgression))
        ////{
        ////}
        ////else
        ////{
        ////    return (CrewProgressionScreen) ScreenManager.Active.pushScreenImmediately(ScreenID.CrewProgression.ToString());
        ////}





        ScreenID[] zFakeHistory = {
            underScreen
        };
        if (underScreen != ScreenID.Invalid)
        {
            ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.CrewProgression, zFakeHistory);
        }
        else
        {
            ScreenManager.Instance.PushScreen(ScreenID.CrewProgression);
        }
        ScreenManager.Instance.UpdateImmediately();
        return ScreenManager.Instance.ActiveScreen as CrewProgressionScreen;
    }
}
