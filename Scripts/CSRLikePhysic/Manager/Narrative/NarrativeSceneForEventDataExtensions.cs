using DataSerialization;
using System;

public static class NarrativeSceneForEventDataExtensions
{
	public static string GetPreRaceSceneID(this NarrativeSceneForEventData data)
	{
		return data.ConditionalPreRaceSceneID.GetText(new GameStateFacade()) ?? data.PreRaceSceneID;
	}

	public static string GetPostRaceWinSceneID(this NarrativeSceneForEventData data)
	{
		return data.ConditionalPostRaceWinSceneID.GetText(new GameStateFacade()) ?? data.PostRaceWinSceneID;
	}

	public static string GetPostRaceLoseSceneID(this NarrativeSceneForEventData data)
	{
		return data.ConditionalPostRaceLoseSceneID.GetText(new GameStateFacade()) ?? data.PostRaceLoseSceneID;
	}

	public static string GetIntroSceneID(this NarrativeSceneForEventData data)
	{
		return data.ConditionalIntroSceneID.GetText(new GameStateFacade()) ?? data.IntroSceneID;
	}

	public static void Initialise(this NarrativeSceneForEventData data)
	{
		data.PreRaceRequirements.Initialise();
		data.PostRaceRequirements.Initialise();
		data.IntroRequirements.Initialise();
		data.ConditionalPreRaceSceneID.Initialise();
		data.ConditionalPostRaceWinSceneID.Initialise();
		data.ConditionalPostRaceLoseSceneID.Initialise();
		data.ConditionalIntroSceneID.Initialise();
	}

	public static bool IsPreRaceSceneEligible(this NarrativeSceneForEventData data, IGameState gs)
	{
		return data.PreRaceRequirements.IsEligible(gs);
	}

	public static bool IsPostRaceSceneEligible(this NarrativeSceneForEventData data, IGameState gs)
	{
		return data.PostRaceRequirements.IsEligible(gs);
	}

	public static bool IsIntroSceneEligible(this NarrativeSceneForEventData data, IGameState gs)
	{
		return data.IntroRequirements.IsEligible(gs);
	}
}
