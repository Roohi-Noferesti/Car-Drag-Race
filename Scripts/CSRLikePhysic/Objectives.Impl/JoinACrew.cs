using System;

namespace Objectives.Impl
{
	public class JoinACrew : AbstractObjective
	{
		public override void UpdateState()
		{
			if (this.IsComplete)
			{
				return;
			}
            //if (!string.IsNullOrEmpty(PlayerProfileManager.Instance.ActiveProfile.FirstCrewUID))
            //{
            //    base.ForceComplete();
            //}
		}
	}
}
