using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class BuyXCars : AbstractObjective
	{
		[SerializeInProfile]
		private int m_boughtCars;

		public int m_boughtCarsTarget;

		internal override void Clear()
		{
			base.Clear();
			this.m_boughtCars = 0;
		}

	    public override string GetDescription()
	    {
	        return string.Format(LocalizationManager.GetTranslation(Description), m_boughtCarsTarget);
	    }

	    [Command]
		public void CounterBuyCar()
		{
			this.m_boughtCars++;
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (base.CurrentProgress < this.m_boughtCars)
			{
				base.CurrentProgress = this.m_boughtCars;
			}
			if (this.m_boughtCars >= this.m_boughtCarsTarget)
			{
				base.ForceComplete();
			}
		}
	}
}
