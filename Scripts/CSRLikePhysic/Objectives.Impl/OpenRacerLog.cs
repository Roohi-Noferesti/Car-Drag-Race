using System;

namespace Objectives.Impl
{
	public class OpenRacerLog : AbstractObjective
	{
		public void RacerLogOpened()
		{
			base.ForceComplete();
		}

		public override void UpdateState()
		{
            //if (this.IsActive && !this.IsComplete && LeftSidePanelContainer.Instance != null && LeftSidePanelContainer.Instance.IsLeftSidePanelOpen())
            //{
            //    this.RacerLogOpened();
            //}
		}
	}
}
