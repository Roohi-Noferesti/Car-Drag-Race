using System;

public class CrewStateAllInvisible : BaseCrewState
{
	public CrewStateAllInvisible(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateAllInvisible(NarrativeSceneStateConfiguration config) : base(config)
	{
	}

    public override void OnEnter()
    {
        foreach (NarrativeSceneCharactersContainer current in this.parentScreen.charactersSlots)
        {
            current.SetAlpha(0f);
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
