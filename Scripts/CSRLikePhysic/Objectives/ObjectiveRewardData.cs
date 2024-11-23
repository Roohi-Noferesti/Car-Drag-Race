using System;
using I2.Loc;

namespace Objectives
{
	[Serializable]
	public class ObjectiveRewardData
	{
		public CSR2Reward Reward;

		public int Amount;

	    public string GetRewardText()
	    {
	        switch (Reward.rewardType)
	        {
	                case ERewardType.Cash:
	                return CurrencyUtils.GetCashString(Amount);
                    case ERewardType.Gold:
                    return CurrencyUtils.GetGoldStringWithIcon(Amount);
                    case ERewardType.FreeUpgrade:
	                return LocalizationManager.GetTranslation("TEXT_FREE_UPGRADE");
                    case ERewardType.FuelPip:
	                return string.Format(LocalizationManager.GetTranslation("TEXT_FILL_FUEL_PIPS"), Amount);
                    case ERewardType.Car:
	                return Reward.GetRewardTitleText(Amount);
	        }
	        return string.Empty;
	    }
	}
}
