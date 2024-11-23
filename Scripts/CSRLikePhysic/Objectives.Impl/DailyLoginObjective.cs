using System;

namespace Objectives.Impl
{
	public class DailyLoginObjective : AbstractObjective
	{
		public void DailyLoginClaimed()
		{
			base.ForceComplete();
		}

		public override void UpdateState()
		{
			if (this.IsActive && !this.IsComplete && false)//DailyLoginRewardsManager.Instance.GetHasClaimedCurrentItem())
			{
                //this.DailyLoginClaimed();
			}
		}
	}
}
