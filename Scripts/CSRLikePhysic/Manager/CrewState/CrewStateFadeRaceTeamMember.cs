using System;

public class CrewStateFadeRaceTeamMember : CrewStateFade
{
	public CrewStateFadeRaceTeamMember(CrewProgressionScreen zParentScreen, float a, float b, float t) : base(zParentScreen, a, b, t)
	{
	}

	public CrewStateFadeRaceTeamMember(NarrativeSceneStateConfiguration config, float a, float b, float t) : base(config, a, b, t)
	{
	}

	protected override void SetAlpha(float alpha)
	{
        //NitroContainer mysteryDonor = base.GetMysteryDonor();
        //mysteryDonor.SetAlpha(alpha, alpha);
        //mysteryDonor.SetMysteryDonorAlpha(alpha);
        //mysteryDonor.SetTextAlpha(alpha);
	}
}
