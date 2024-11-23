using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class GetXPerfectStarts : AbstractObjective
	{
		[SerializeInProfile]
		private int m_perfectStartCount;

		public int PerfectStartTarget = 1;

		internal override void Clear()
		{
			base.Clear();
			this.m_perfectStartCount = 0;
		}

		[Command]
		public void CounterPerfectStart()
		{
			this.m_perfectStartCount++;
		}

		public override string GetDescription()
		{
			var desc = LocalizationManager.GetTranslation(this.Description);
			var format = string.Format(desc, this.PerfectStartTarget.ToString());
			return format;
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (base.CurrentProgress < this.m_perfectStartCount)
			{
				base.CurrentProgress = this.m_perfectStartCount;
			}
			if (this.m_perfectStartCount >= this.PerfectStartTarget)
			{
				base.ForceComplete();
			}
		}
	}
}
