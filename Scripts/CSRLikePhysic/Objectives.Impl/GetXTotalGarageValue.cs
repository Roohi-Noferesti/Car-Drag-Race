using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class GetXTotalGarageValue : AbstractObjective
	{
		public int GarageValueTarget = 1;

		private void Start()
		{
			this.TotalProgressSteps = this.GarageValueTarget;
		}

		public override string GetDescription()
		{
            return string.Format(LocalizationManager.GetTranslation(this.Description), CurrencyUtils.GetCostStringBrief(this.GarageValueTarget, 0));
		}

	    public override void UpdateState()
		{
            if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
            {
                return;
            }
            PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
	        int totalPurchasePriceOfCarsOwned =  activeProfile.GetTotalPurchasePriceOfCarsOwned();
            if (base.CurrentProgress < totalPurchasePriceOfCarsOwned)
            {
                base.CurrentProgress = totalPurchasePriceOfCarsOwned;
            }
            if (totalPurchasePriceOfCarsOwned >= this.GarageValueTarget)
            {
                base.ForceComplete();
            }
		}
	}
}
