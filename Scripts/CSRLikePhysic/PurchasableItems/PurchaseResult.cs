public class PurchaseResult
{
    public enum eResult
    {
        FAILED,
        SUCCEEDED,
        RESTORED,
        DEFERRED,
        CANCELLED
    }

    public eResult Result;

    public string TransactionID;

    public string ProductID;

    public string Receipt;

    public string Signature;

    public string ErrorDomain;

    public int ErrorCode;

    public string Market;

    public string CurrencyCode;

    public double Price;

    public PurchaseReport Report;
    
    public class PurchaseReport
    {
	    public string prodID;
	    public string price;
	    public string currency;
	    public string receipt;
	    public string transactionID;
	    public string purchaseData;
	    public string signature;
    }

    public PurchaseResult Clone()
    {
        return base.MemberwiseClone() as PurchaseResult;
    }

    public bool IsValid()
    {
        return this.Result != eResult.SUCCEEDED || (!string.IsNullOrEmpty(this.TransactionID) && !string.IsNullOrEmpty(this.ProductID) && !string.IsNullOrEmpty(this.Receipt));
    }

    public override string ToString()
    {
        return string.Format("Result: {0} TransactionID: {1} ProductID: {2} Receipt: {3} Signature: {4} ErrorDomain: {5} ErrorCode: {6}", new object[]
		{
			this.Result,
			this.TransactionID,
			this.ProductID,
			this.Receipt,
			this.Signature,
			this.ErrorDomain,
			this.ErrorCode
		});
    }

    public string ToMetric()
    {
        return string.Format("Result: {0} TransactionID: {1} ProductID: {2} Receipt: {3} Signature: {4} ErrorDomain: {5} ErrorCode: {6}", new object[]
		{
			this.Result,
			(this.TransactionID != null) ? this.TransactionID.Length : -1,
			this.ProductID,
			(this.Receipt != null) ? this.Receipt.Length : -1,
			(this.Signature != null) ? this.Signature.Length : -1,
			this.ErrorDomain,
			this.ErrorCode
		});
    }
}
