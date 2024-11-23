using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class SpendXSCInCrewPerk : AbstractObjective
	{
		[SerializeInProfile]
		private int m_SCSpendCount;

		public int SCToSpendTarget = 1000;

		internal override void Clear()
		{
			base.Clear();
			this.m_SCSpendCount = 0;
		}

		[Command]
		public void SpendSCInCrew(int SCSpent)
		{
			this.m_SCSpendCount += SCSpent;
		}

		public override string GetDescription()
		{
            return string.Format(LocalizationManager.GetTranslation(this.Description), CurrencyUtils.GetCostStringBrief(this.SCToSpendTarget, 0));
		}

	    public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (base.CurrentProgress < this.m_SCSpendCount)
			{
				base.CurrentProgress = this.m_SCSpendCount;
			}
			if (this.m_SCSpendCount >= this.SCToSpendTarget)
			{
				base.ForceComplete();
			}
		}
	}
}
