using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class UseXCarsFromCrew : AbstractObjective
	{
		[SerializeInProfile]
		private int m_usedCrewCarsCount;

		public int UsedCrewCarTarget = 1;

		internal override void Clear()
		{
			base.Clear();
			this.m_usedCrewCarsCount = 0;
		}

		[Command]
		public void UseCrewCar()
		{
			this.m_usedCrewCarsCount++;
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.UsedCrewCarTarget.ToString());
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (base.CurrentProgress < this.m_usedCrewCarsCount)
			{
				base.CurrentProgress = this.m_usedCrewCarsCount;
			}
			if (this.m_usedCrewCarsCount >= this.UsedCrewCarTarget)
			{
				base.ForceComplete();
			}
		}
	}
}
