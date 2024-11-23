using System;

public class CrewStateShowRaceButton : BaseCrewState
{
	public CrewStateShowRaceButton(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateShowRaceButton(NarrativeSceneStateConfiguration config) : base(config)
	{
	}

	public override void OnEnter()
	{
		this.parentScreen.ShowRaceButton();
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
