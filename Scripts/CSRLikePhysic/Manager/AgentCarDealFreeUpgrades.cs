using System;
using System.Collections;
using I2.Loc;
using UnityEngine;
using UnityEngine.Events;

public class AgentCarDealFreeUpgrades : AgentCarDeal
{
	private readonly int freeUpgradeLevel;

	public AgentCarDealFreeUpgrades(CarTransitionDeal transitionDeal) : base(transitionDeal.OfferCar)
	{
		this.freeUpgradeLevel = transitionDeal.FreeUpgradeLevel;
	}

	public override string GetWorkshopScreenPopupBodyText()
	{
		return string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_AGENT_CAR_DEAL_BODY_FREE_UPGRADES"), base.LocalisedCarName(), this.freeUpgradeLevel);
	}

	public override void OnPopupShown()
	{
		PlayerProfileManager.Instance.ActiveProfile.CarFreeUpgradeDealShown();
	}

	public override void OnCompleted()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		CarGarageInstance carFromID = activeProfile.GetCarFromID(base.Car);
		carFromID.SetAndApplyUpgradeLevelToAllUpgrades(this.freeUpgradeLevel);
		base.OnCompleted();
	}

	private int CalculateSaving()
	{
		int num = 0;
		IEnumerator enumerator = Enum.GetValues(typeof(eUpgradeType)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				eUpgradeType zUpgradeType = (eUpgradeType)((int)enumerator.Current);
			    if (zUpgradeType != eUpgradeType.INVALID)
			    {
			        for (int i = 0; i <= this.freeUpgradeLevel; i++)
			        {
			            CarUpgradeData carUpgrade = CarDatabase.Instance.GetCarUpgrade(base.Car, zUpgradeType, i+1);
			            if (carUpgrade != null)
			            {
			                num += carUpgrade.CostInCash;
			            }
			        }
			    }
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return num;
	}

	public override void SetupCostContainer(CostContainer container, UnityAction buttonAction)
	{
		container.SetupForFreeUpgradeDeal(this.GetGoldPrice(), this.freeUpgradeLevel, buttonAction, this.CalculateSaving());
	}

	public override string GetFreeUpgradesMetricParam()
	{
		return this.freeUpgradeLevel.ToString();
	}
}
