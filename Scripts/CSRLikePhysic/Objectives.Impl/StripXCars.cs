using System;

namespace Objectives.Impl
{
	public class StripXCars : AbstractObjective
	{
		[SerializeInProfile]
		private int m_strippedCars;

		public int m_stripCarsTarget;

		internal override void Clear()
		{
			base.Clear();
			this.m_strippedCars = 0;
		}

		[Command]
		public void CounterStripCar()
		{
			this.m_strippedCars++;
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (base.CurrentProgress < this.m_strippedCars)
			{
				base.CurrentProgress = this.m_strippedCars;
			}
			if (this.m_strippedCars >= this.m_stripCarsTarget)
			{
				base.ForceComplete();
			}
		}
	}
}
