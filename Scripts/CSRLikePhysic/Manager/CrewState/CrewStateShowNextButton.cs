using System;

public class CrewStateShowNextButton : BaseCrewState
{
	public CrewStateShowNextButton(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateShowNextButton(NarrativeSceneStateConfiguration config) : base(config)
	{
	}

	public override void OnEnter()
	{
		this.parentScreen.ShowNextButton();
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
