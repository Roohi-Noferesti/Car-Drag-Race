using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class DonateXCrewToken : AbstractObjective
	{
		[SerializeInProfile]
		private int m_targetCount;

		public int TargetValue;

		internal override void Clear()
		{
			base.Clear();
			this.m_targetCount = 0;
		}

		[Command]
		public void CounterDonatedCrewTokens(int donated)
		{
			this.m_targetCount += donated;
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.TargetValue.ToString());
		}

		public override void UpdateState()
		{
			if (this.IsActive && !this.IsComplete)
			{
				if (base.CurrentProgress < this.m_targetCount)
				{
					base.CurrentProgress = this.m_targetCount;
				}
				if (this.m_targetCount >= this.TargetValue)
				{
					base.ForceComplete();
				}
			}
		}
	}
}
