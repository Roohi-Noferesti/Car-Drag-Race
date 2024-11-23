using System;

public class CrewStateFadeOutRaceTeamMember : CrewStateFadeRaceTeamMember
{
	public CrewStateFadeOutRaceTeamMember(CrewProgressionScreen zParentScreen, float zTime) : base(zParentScreen, 1f, 0f, zTime)
	{
	}

	public CrewStateFadeOutRaceTeamMember(NarrativeSceneStateConfiguration config) : base(config, 1f, 0f, config.StateDetails.FadeInTime)
	{
	}
}
