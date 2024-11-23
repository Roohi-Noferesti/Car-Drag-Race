using System;

public class CrewStateDismissScreen : BaseCrewState
{
	public CrewStateDismissScreen(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateDismissScreen(NarrativeSceneStateConfiguration config) : base(config)
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
        //if (this.parentScreen.ShouldTriggerHighStakesChallenge)
        //{
        //    HighStakesScreenBase.TierForChallenge = this.parentScreen.TierOfHighStakesTrigger;
        //    ScreenManager.Instance.SwapScreen(ScreenID.HighStakesChallenge);
        //    return;
        //}
        //ScreenManager.Instance.PopScreen();
	}
}
