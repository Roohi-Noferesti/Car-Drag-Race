public class CarDealsData
{
	public int RacesSinceLastDeal;

	public string LastCarOffered;

	public int LastDiscount;

	public int LastDiscountRepeatCount;

	public bool LastDealWasCashback;

	public void ToJson(JsonDict dict)
	{
		dict.Set("cdrl", this.RacesSinceLastDeal);
		dict.Set("cdco", this.LastCarOffered);
		dict.Set("cdld", this.LastDiscount);
		dict.Set("cddr", this.LastDiscountRepeatCount);
		dict.Set("cdlc", this.LastDealWasCashback);
	}

	public void FromJson(JsonDict dict)
	{
		dict.TryGetValue("cdrl", out this.RacesSinceLastDeal);
		dict.TryGetValue("cdco", out this.LastCarOffered);
		dict.TryGetValue("cdld", out this.LastDiscount);
		dict.TryGetValue("cddr", out this.LastDiscountRepeatCount);
		dict.TryGetValue("cdlc", out this.LastDealWasCashback);
	}
}
