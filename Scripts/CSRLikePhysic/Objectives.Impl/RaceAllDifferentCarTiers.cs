using System;
using System.Collections.Generic;
using I2.Loc;

namespace Objectives.Impl
{
	public class RaceAllDifferentCarTiers : AbstractObjective
	{
		[SerializeInProfile]
		private List<int> m_racedCarTiers = new List<int>();

		private int m_carsRacedTiersTarget = 5;

		internal override void Clear()
		{
			base.Clear();
			if (this.m_racedCarTiers != null)
			{
				this.m_racedCarTiers.Clear();
			}
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.m_carsRacedTiersTarget.ToString());
		}

		private void Start()
		{
			this.TotalProgressSteps = this.m_carsRacedTiersTarget;
		}

		[Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
		{
			CarInfo car = CarDatabase.Instance.GetCar(raceEventInfo.LocalPlayerCarDBKey);
			if (car != null)
			{
				int baseCarTier = (int)car.BaseCarTier;
				if (this.m_racedCarTiers != null && !this.m_racedCarTiers.Contains(baseCarTier))
				{
					this.m_racedCarTiers.Add(baseCarTier);
				}
			}
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (this.m_racedCarTiers != null)
			{
				if (base.CurrentProgress < this.m_racedCarTiers.Count)
				{
					base.CurrentProgress = this.m_racedCarTiers.Count;
				}
				if (this.m_racedCarTiers.Count >= this.m_carsRacedTiersTarget)
				{
					base.ForceComplete();
				}
			}
		}
	}
}
