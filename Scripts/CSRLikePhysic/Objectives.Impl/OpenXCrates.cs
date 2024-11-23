using System;

namespace Objectives.Impl
{
	public class OpenXCrates : AbstractObjective
	{
		[SerializeInProfile]
		private int m_openedCrates;

		public int m_openedCratesTarget;

		internal override void Clear()
		{
			base.Clear();
			this.m_openedCrates = 0;
		}

		[Command]
		public void CounterGatchaSpin()
		{
			this.m_openedCrates++;
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (base.CurrentProgress < this.m_openedCrates)
			{
				base.CurrentProgress = this.m_openedCrates;
			}
			if (this.m_openedCrates >= this.m_openedCratesTarget)
			{
				base.ForceComplete();
			}
		}
	}
}
