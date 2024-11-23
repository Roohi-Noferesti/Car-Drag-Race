using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class GetFullFreshness : AbstractObjective
	{
		//[SerializeInProfile]
		//private int m_targetCount;

		public int TargetValue;

		internal override void Clear()
		{
			base.Clear();
			//this.m_targetCount = 0;
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.TargetValue.ToString());
		}

		public override void UpdateState()
		{
            //if (this.IsActive && !this.IsComplete)
            //{
            //    int carUID = -1;
            //    if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar() != null)
            //    {
            //        carUID = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().UniqueID;
            //    }
            //    CustomisationRPBonus carRPBonus = FreshnessManager.Instance.GetCarRPBonus(carUID);
            //    if (carRPBonus != null && carRPBonus.BonusPercent >= FreshnessManager.Instance.MaxRPBonus)
            //    {
            //        base.ForceComplete();
            //    }
            //}
		}

		public override bool IsPossibleToComplete()
		{
		    //return FreshnessManager.Instance.FreshnessEnabled;
		    return false;
		}
	}
}
