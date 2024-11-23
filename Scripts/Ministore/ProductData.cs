public class ProductData
{
    public AppStoreProduct AppStoreProduct
    {
        get;
        private set;
    }

    //public VirtualCurrencyPack VirtualCurrencyPack
    //{
    //    get;
    //    private set;
    //}

    public GTProduct GtProduct
	{
		get;
		private set;
	}

	public int AnimFrameIndex
	{
		get;
		set;
	}

    public ProductData(AppStoreProduct appStoreProduct, GTProduct zGtProduct)
	{
        this.AppStoreProduct = appStoreProduct;
		this.GtProduct = zGtProduct;
	}
}
