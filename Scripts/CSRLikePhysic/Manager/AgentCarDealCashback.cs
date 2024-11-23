using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.Events;

public class AgentCarDealCashback : AgentCarDeal
{
	private readonly int cashbackAmount;

	public AgentCarDealCashback(string car, int cashbackAmount) : base(car)
	{
		this.cashbackAmount = cashbackAmount;
	}

	public override string GetWorkshopScreenPopupBodyText()
	{
		string colouredCashString = CurrencyUtils.GetCashString(this.cashbackAmount);
		return string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_AGENT_CAR_DEAL_BODY_CASHBACK"), base.LocalisedCarName(), base.LocalisedManufacturerName(), colouredCashString);
	}

	public override void OnPopupShown()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		activeProfile.CarCashbackDealShown(base.Car);
	}

	public override void OnCompleted()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		activeProfile.AddCash(this.cashbackAmount,"AgentCarDealCashBack","AgentCarDealCashBack");
		base.OnCompleted();
	}

	public override void SetupCostContainer(CostContainer container, UnityAction buttonAction)
	{
		container.SetupForCashbackDeal(this.GetGoldPrice(), this.cashbackAmount, buttonAction);
	}

	public override string GetCashbackMetricParam()
	{
		return this.cashbackAmount.ToString();
	}

	public override string ToString()
	{
		return string.Format("{0} with ${1} cashback", base.Car, this.cashbackAmount);
	}
}
