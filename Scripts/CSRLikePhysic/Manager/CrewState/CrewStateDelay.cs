using System;

public class CrewStateDelay : BaseCrewState
{
	private float timeToDelay;

	public CrewStateDelay(CrewProgressionScreen zParentScreen, float zDelayTime) : base(zParentScreen)
	{
		this.timeToDelay = zDelayTime;
	}

	public CrewStateDelay(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.timeToDelay = config.StateDetails.Delay;
	}

	public override void OnEnter()
	{
	}

	public override bool OnMain()
	{
		base.OnMain();
		return this.timeInState > this.timeToDelay;
	}

	public override void OnExit()
	{
	}
}
