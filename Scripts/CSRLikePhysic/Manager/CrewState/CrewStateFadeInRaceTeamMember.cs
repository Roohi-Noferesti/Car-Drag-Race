using System;

public class CrewStateFadeInRaceTeamMember : CrewStateFadeRaceTeamMember
{
	public CrewStateFadeInRaceTeamMember(CrewProgressionScreen zParentScreen, float zTime) : base(zParentScreen, 0f, 1f, zTime)
	{
	}

	public CrewStateFadeInRaceTeamMember(NarrativeSceneStateConfiguration config) : base(config, 0f, 1f, config.StateDetails.FadeInTime)
	{
	}
}
