using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class ObtainXNewCars : AbstractObjective
	{
		[SerializeInProfile]
		private int m_carsObtainedCount;

		public int m_carsObtainedTarget;

		internal override void Clear()
		{
			base.Clear();
			this.m_carsObtainedCount = 0;
		}

		[Command]
		public void CounterObtainCar()
		{
			this.m_carsObtainedCount++;
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.m_carsObtainedTarget.ToString());
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (base.CurrentProgress < this.m_carsObtainedCount)
			{
				base.CurrentProgress = this.m_carsObtainedCount;
			}
			if (this.m_carsObtainedCount == this.m_carsObtainedTarget)
			{
				base.ForceComplete();
			}
		}
	}
}
