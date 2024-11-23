using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class TuneCarXTimes : AbstractObjective
	{
		[SerializeInProfile]
		private int m_tuneCount;

		public int UpgradeLevel;

		internal override void Clear()
		{
			base.Clear();
			this.m_tuneCount = 0;
		}

		[Command]
		public void CounterTuneCar()
		{
			this.m_tuneCount++;
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.UpgradeLevel.ToString());
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (base.CurrentProgress < this.m_tuneCount)
			{
				base.CurrentProgress = this.m_tuneCount;
			}
			if (this.m_tuneCount >= this.UpgradeLevel)
			{
				base.ForceComplete();
			}
		}
	}
}
