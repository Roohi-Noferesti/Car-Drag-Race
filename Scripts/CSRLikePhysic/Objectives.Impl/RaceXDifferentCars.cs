using System;
using System.Collections.Generic;
using I2.Loc;

namespace Objectives.Impl
{
	public class RaceXDifferentCars : AbstractObjective
	{
		[SerializeInProfile]
        private List<string> m_racedCars = new List<string>();

		public int m_carsRacedTarget;

		internal override void Clear()
		{
			base.Clear();
			if (this.m_racedCars != null)
			{
				this.m_racedCars.Clear();
			}
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.m_carsRacedTarget.ToString());
		}

		[Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
		{
            CarGarageInstance carFromDBKey = PlayerProfileManager.Instance.ActiveProfile.GetCarFromID(raceEventInfo.LocalPlayerCarDBKey);
            if (this.m_racedCars == null)
            {
                this.m_racedCars = new List<string>();
            }
            if (carFromDBKey != null)
            {
                var uniqueID = carFromDBKey.CarDBKey;
                if (!this.m_racedCars.Contains(uniqueID))
                {
                    this.m_racedCars.Add(uniqueID);
                }
            }
            else if (raceEventInfo.IsDailyBattleEvent)
            {
                this.m_racedCars.Add("_daily");
            }
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (this.m_racedCars != null)
			{
				if (base.CurrentProgress < this.m_racedCars.Count)
				{
					base.CurrentProgress = this.m_racedCars.Count;
				}
				if (this.m_racedCars.Count >= this.m_carsRacedTarget)
				{
					base.ForceComplete();
				}
			}
		}
	}
}
