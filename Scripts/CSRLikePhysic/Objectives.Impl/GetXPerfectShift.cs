using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class GetXPerfectShift : AbstractObjective
	{
		[SerializeInProfile]
		private int m_perfectShiftCount;

		public int PerfectShiftTarget = 1;

		internal override void Clear()
		{
			base.Clear();
			this.m_perfectShiftCount = 0;
		}

		[Command]
		public void CounterPerfectShift()
		{
			this.m_perfectShiftCount++;
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.PerfectShiftTarget.ToString());
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (base.CurrentProgress < this.m_perfectShiftCount)
			{
				base.CurrentProgress = this.m_perfectShiftCount;
			}
			if (this.m_perfectShiftCount >= this.PerfectShiftTarget)
			{
				base.ForceComplete();
			}
		}
	}
}
