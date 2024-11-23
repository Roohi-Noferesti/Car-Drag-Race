using System;

public class CrewStateNitroFadeOut : CrewStateFade
{
	public CrewStateNitroFadeOut(CrewProgressionScreen zParentScreen, float zTime) : base(zParentScreen, 1f, 0f, zTime)
	{
	}

	public CrewStateNitroFadeOut(NarrativeSceneStateConfiguration config) : base(config, 1f, 0f, config.StateDetails.FadeInTime)
	{
	}

	protected override void SetAlpha(float alpha)
	{
        //NitroContainer mysteryDonor = base.GetMysteryDonor();
        //mysteryDonor.SetAlpha(alpha, alpha);
        //mysteryDonor.SetTextAlpha(alpha);
	}
}
