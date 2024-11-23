using System;

public class CrewStateTierUnlocked : BaseCrewState
{
	public CrewStateTierUnlocked(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateTierUnlocked(NarrativeSceneStateConfiguration config) : base(config)
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
        //ScreenManager.Instance.SwapScreen(ScreenID.TierUnlocked);
	}
}
