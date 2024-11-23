using System;
using System.Collections.Generic;

public interface IAppStoreManager: IDisposable
{
	event Action OnReceivedProductDataResponse;

	bool IAPEnabled
	{
		get;
	}

	bool UsingSimulator
	{
		get;
	}

	bool IsProcessingTransaction
	{
		get;
	}

	string CurrencyCode { get;}

	void SetProducts(List<GTProduct> products);

	void StartProductRequest();

	List<AppStoreProduct> GetProducts();

	void Purchase(string productCode);

	void RestorePurchases();

	PurchaseResult GetPurchaseResult();

	void ProcessExistingTransactions();

	void ConsumePurchase(string productCode,string authority);
	
	void Initialize();
}
