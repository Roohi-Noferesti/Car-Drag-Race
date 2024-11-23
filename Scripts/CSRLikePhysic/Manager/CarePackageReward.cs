using System;
using System.Collections.Generic;

[Serializable]
public class CarePackageReward
{
    public enum eRewardCalculation
	{
        None,
		SubtractCashBalance,
		CashForTier2Car,
		CashForTier3Car,
		CashForTier4Car,
		CashForTier5Car
	}

    public eRewardCalculation Calculation;

	public CarePackageRewardDetails Details = new CarePackageRewardDetails();

	private IDictionary<CarePackageReward.eRewardCalculation, CarePackageRewardCalculationBase> conditionMapping = new Dictionary<CarePackageReward.eRewardCalculation, CarePackageRewardCalculationBase>
	{
		{
			CarePackageReward.eRewardCalculation.SubtractCashBalance,
			new SubtractCashBalanceCarePackageRewardCalculation()
		},
		{
			CarePackageReward.eRewardCalculation.CashForTier2Car,
			new CashForTierCarCarePackageRewardCalculation(eCarTier.TIER_2)
		},
		{
			CarePackageReward.eRewardCalculation.CashForTier3Car,
			new CashForTierCarCarePackageRewardCalculation(eCarTier.TIER_3)
		},
		{
			CarePackageReward.eRewardCalculation.CashForTier4Car,
			new CashForTierCarCarePackageRewardCalculation(eCarTier.TIER_4)
		},
		{
			CarePackageReward.eRewardCalculation.CashForTier5Car,
			new CashForTierCarCarePackageRewardCalculation(eCarTier.TIER_5)
		}
	};

	public CarePackageRewardDetails GetCalculatedDetails()
	{
        if (this.Calculation!=eRewardCalculation.None)
		{
            //CarePackageReward.eRewardCalculation key = EnumHelper.FromString<CarePackageReward.eRewardCalculation>(this.Calculation);
            //this.Calculation = string.Empty;
			CarePackageRewardDetails carePackageRewardDetails = new CarePackageRewardDetails(this.Details);
            this.conditionMapping[Calculation].PerformCalculation(carePackageRewardDetails);
			return carePackageRewardDetails;
		}
		return this.Details;
	}
}
