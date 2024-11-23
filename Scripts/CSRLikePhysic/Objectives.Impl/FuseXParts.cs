using System;

namespace Objectives.Impl
{
	public class FuseXParts : AbstractObjective
	{
		[SerializeInProfile]
		private int m_fuseXPartTarget;

		public int TargetValue;

		[Command]
		public void CounterPartFused()
		{
			this.m_fuseXPartTarget++;
		}

		public override void UpdateState()
		{
			if (this.IsActive && !this.IsComplete)
			{
				if (base.CurrentProgress < this.m_fuseXPartTarget)
				{
					base.CurrentProgress = this.m_fuseXPartTarget;
				}
				if (this.m_fuseXPartTarget >= this.TargetValue)
				{
					base.ForceComplete();
				}
			}
		}
	}
}
