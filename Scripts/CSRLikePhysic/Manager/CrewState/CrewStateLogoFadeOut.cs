using System;

public class CrewStateLogoFadeOut : BaseCrewState
{
    private int crew;

    private float timeToScale = 0.5f;

	public CrewStateLogoFadeOut(CrewProgressionScreen zParentScreen, int zCrew) : base(zParentScreen)
	{
        this.crew = zCrew;
    }

	public CrewStateLogoFadeOut(NarrativeSceneStateConfiguration config) : base(config)
	{
        this.crew = config.StateDetails.SlotIndex;
        this.timeToScale = config.StateDetails.TimeToScale;
	}

	public override void OnEnter()
	{
	}

	public override bool OnMain()
	{
		base.OnMain();
		float num = this.timeInState / this.timeToScale;
		bool result = num >= 1f;
		float logoAlpha = 1f - num;
		this.parentScreen.charactersSlots[crew].SetLogoAlpha(logoAlpha);
		return result;
	}

	public override void OnExit()
	{
	}
}
