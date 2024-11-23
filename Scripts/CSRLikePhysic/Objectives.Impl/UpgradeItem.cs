using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class UpgradeItem : AbstractObjective
	{
		[SerializeInProfile]
		private int m_upgradeCount;

		public int UpgradeLevel;

		internal override void Clear()
		{
			base.Clear();
			this.m_upgradeCount = 0;
		}

		[Command]
		public void CounterUpgradeItem()
		{
			this.m_upgradeCount++;
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
			if (base.CurrentProgress < this.m_upgradeCount)
			{
				base.CurrentProgress = this.m_upgradeCount;
			}
			if (this.m_upgradeCount >= this.UpgradeLevel)
			{
				base.ForceComplete();
			}
		}
	}
}
