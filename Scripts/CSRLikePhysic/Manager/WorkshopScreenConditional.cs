using System;

public class WorkshopScreenConditional : FlowConditionalBase
{
	public WorkshopScreenConditional()
	{
        base.AddCondition(new WorkshopCarePackageCondition(), 20, ConditionShowMode.AlwaysShow);
        //base.AddCondition(new FriendsWonAllCarsCondition(), 30, ConditionShowMode.AlwaysShow);
        //base.AddCondition(new WorkshopEliteCarCondition(), 40, ConditionShowMode.AlwaysShow);
        //base.AddCondition(new WorkshopFacebookFuelCondition(), 50, ConditionShowMode.ShowIfFirstCondition);
		base.AddCondition(new WorkshopUpgradeCondition(), 60, ConditionShowMode.ShowIfFirstCondition);
        base.AddCondition(new WorkshopCarDealCondition(), 70, ConditionShowMode.ShowIfFirstCondition);
        //base.AddCondition(new WorldTourWorkshopPopupsCondition(), 45, ConditionShowMode.AlwaysShow);
	}

	protected override bool IsConditionalActive()
	{
		return true;
	}
}
