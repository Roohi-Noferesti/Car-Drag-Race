using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.Events;

public class AgentCarDealDiscount : AgentCarDeal
{
	private readonly int discountPercentage;

	public AgentCarDealDiscount(string car, int discountPercentage) : base(car)
	{
		this.discountPercentage = discountPercentage;
	}

	public override int GetGoldPrice()
	{
		return AgentCarDeal.ApplyDiscountToPrice(base.GetGoldPrice(), this.discountPercentage);
	}

	public override string GetWorkshopScreenPopupBodyText()
	{
		int num = UnityEngine.Random.Range(1, 4);
		string format = LocalizationManager.GetTranslation(string.Format("TEXT_POPUPS_AGENT_CAR_DEAL_BODY_{0}", num));
		return string.Format(format, base.LocalisedCarName(), this.discountPercentage, base.LocalisedManufacturerName());
	}

	public override void OnPopupShown()
	{
		PlayerProfileManager.Instance.ActiveProfile.CarDiscountDealShown(base.Car, this.discountPercentage);
	}

	public override void SetupCostContainer(CostContainer container, UnityAction buttonAction)
	{
		container.SetupForDiscountDeal(this.GetGoldPrice(), this.discountPercentage, buttonAction);
	}

	public override string GetDiscountMetricParam()
	{
		return this.discountPercentage.ToString();
	}

	public override string ToString()
	{
		return string.Format("{0} with {1}% discount", base.Car, this.discountPercentage);
	}
}
