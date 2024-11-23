using System;

public class CrewStateCredits : BaseCrewState
{
	public CrewStateCredits(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateCredits(NarrativeSceneStateConfiguration config) : base(config)
	{
	}

	public override void OnEnter()
	{
	}

	public override bool OnMain()
	{
		base.OnMain();
		return true;
	}

	public override void OnExit()
	{
        //ScreenManager.Instance.SwapScreen(ScreenID.Credits);
	}
}
