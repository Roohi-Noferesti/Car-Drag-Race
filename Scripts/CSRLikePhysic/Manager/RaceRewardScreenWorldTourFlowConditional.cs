using System;

public class RaceRewardScreenWorldTourFlowConditional : FlowConditionalBase
{
	public RaceRewardScreenWorldTourFlowConditional()
	{
        base.AddCondition(new WorldTourPostRaceLostCondition(), 10, ConditionShowMode.AlwaysShow);
	}

	protected override bool IsConditionalActive()
	{
	    return true;
	}
}
