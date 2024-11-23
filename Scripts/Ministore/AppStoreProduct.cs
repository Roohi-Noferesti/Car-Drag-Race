public class AppStoreProduct
{
	public string Title;

	public string Description;

	public string LocalisedPrice;

	public string Price;

	public string Identifier;

	public string CurrencySymbol;

	public string CurrencyCode;

    public string FortumoServiceID;

	public bool Valid()
	{
#if UNITY_EDITOR
	    return !string.IsNullOrEmpty(this.LocalisedPrice) && !string.IsNullOrEmpty(this.Identifier);
#else
		bool flag = string.IsNullOrEmpty(this.Title) || string.IsNullOrEmpty(this.LocalisedPrice) || string.IsNullOrEmpty(this.Identifier);
		return !flag;
#endif
	}
}
