using System;

public class CrewStatePrizeOMatic : BaseCrewState
{
	public CrewStatePrizeOMatic(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStatePrizeOMatic(NarrativeSceneStateConfiguration config) : base(config)
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
        //ScreenManager.Instance.SwapScreen(ScreenID.PrizeOMatic);
	}
}
