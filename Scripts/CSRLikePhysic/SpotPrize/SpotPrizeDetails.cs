using System;

[Serializable]
public class SpotPrizeDetails
{
	public int Quantity;

	public float FloatQuantity;

	public int Duration;

	public string Car = string.Empty;

	public string Livery = string.Empty;

	[NonSerialized]
	public int EventID = -1;

	[NonSerialized]
	public int SpotPrizeID = -1;
}
