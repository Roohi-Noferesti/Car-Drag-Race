using System;

public class CrewStateDeactivateCrew : BaseCrewState
{
    private int crew;

    public CrewStateDeactivateCrew(CrewProgressionScreen zParentScreen, int zCrew) : base(zParentScreen)
	{
        this.crew = zCrew;
    }

	public CrewStateDeactivateCrew(NarrativeSceneStateConfiguration config) : base(config)
	{
        this.crew = config.StateDetails.SlotIndex;
    }

	public override void OnEnter()
	{
	    int num = 5;
	    for (int i = 0; i < num; i++)
	    {
	        this.parentScreen.charactersSlots[this.crew].SetActiveSlots(i, false);
	    }
    }

	public override bool OnMain()
	{
		base.OnMain();
		return true;
	}

	public override void OnExit()
	{
	}
}
