using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

#if UNITY_ANDROID
public class GooglePlayAppStoreManager : IAppStoreManager
{
	private delegate void JavaCallback();

	public static string[] FakeProductRequestIds = new string[]
	{
		string.Empty,
		"android.test.purchased",
		"android.test.cancelled",
		"android.test.item_unavailable",
		"android.test.item_refunded"
	};

	public static int FakeProductRequestIdIndex = 0;

	private bool transactionInPaymentQueue;

	private AndroidJavaObject appStoreManager;

	private static GooglePlayAppStoreManager instance;

    public event Action OnReceivedProductDataResponse;

	public static string FakeProductRequestId
	{
		get
		{
			return GooglePlayAppStoreManager.FakeProductRequestIds[GooglePlayAppStoreManager.FakeProductRequestIdIndex];
		}
	}

	public bool IAPEnabled
	{
		get
		{
			return !string.IsNullOrEmpty(GooglePlayAppStoreManager.FakeProductRequestId) || this.appStoreManager.Call<bool>("IAPEnabled", new object[0]);
		}
	}

	public bool UsingSimulator
	{
		get
		{
			return false;
		}
	}

	public bool IsProcessingTransaction
	{
		get
		{
			return this.transactionInPaymentQueue;
		}
	}

	public string CurrencyCode { get; private set; }

	public GooglePlayAppStoreManager()
	{
		this.appStoreManager = AndroidSpecific.mActivityJavaObject.Get<AndroidJavaObject>("mGooglePlayAppStoreManager");
		GooglePlayAppStoreManager.instance = this;
	}

	public void SetProducts(List<GTProduct> products)
	{
		this.appStoreManager.Call("ClearProducts", new object[0]);
		foreach (GTProduct current in products)
		{
			this.appStoreManager.Call("AddProductId", new object[]
			{
				current.CodeWithIdentifier,
				current.IsConsumable
			});
		}
	}

	private AppStoreProduct GetProduct(int index)
	{
		return new AppStoreProduct
		{
			Title = this.appStoreManager.Call<string>("GetProductTitle", new object[]
			{
				index
			}),
			Description = this.appStoreManager.Call<string>("GetProductDescription", new object[]
			{
				index
			}),
			Price = null,
			LocalisedPrice = this.appStoreManager.Call<string>("GetFullProductPrice", new object[]
			{
				index
			}),
			Identifier = this.appStoreManager.Call<string>("GetProductIdentifier", new object[]
			{
				index
			}),
			CurrencySymbol = null,
			CurrencyCode = null
		};
	}

	public List<AppStoreProduct> GetProducts()
	{
		int num = this.appStoreManager.Call<int>("GetProductCount", new object[0]);
		List<AppStoreProduct> list = new List<AppStoreProduct>(num);
		for (int i = 0; i < num; i++)
		{
			list.Add(this.GetProduct(i));
		}
		return list;
	}

	public void StartProductRequest()
	{
		this.appStoreManager.Call("StartProductRequest", new object[0]);
	}

	public void Purchase(string productCode)
	{
		if (!string.IsNullOrEmpty(GooglePlayAppStoreManager.FakeProductRequestId))
		{
			productCode = GooglePlayAppStoreManager.FakeProductRequestId;
		}
		this.appStoreManager.Call("Purchase", new object[]
		{
			productCode
		});
		this.transactionInPaymentQueue = true;
	}

	public void RestorePurchases()
	{
		foreach (string current in GameDatabase.Instance.IAPs.GetRestorableProductCodes())
		{
			this.appStoreManager.Call("AddPurchaseToRestore", new object[]
			{
				current
			});
		}
		this.appStoreManager.Call("RestorePurchases", new object[0]);
		this.transactionInPaymentQueue = true;
	}

	private static void FireOnRecievedProductDataResponse()
	{
		if (GooglePlayAppStoreManager.instance != null && GooglePlayAppStoreManager.instance.OnReceivedProductDataResponse != null)
		{
			GooglePlayAppStoreManager.instance.OnReceivedProductDataResponse();
		}
	}

	public PurchaseResult GetPurchaseResult()
	{
		string text = this.appStoreManager.Call<string>("GetTransactionResult", new object[0]);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		string[] array = text.Split(new char[]
		{
			':'
		});
		PurchaseResult purchaseResult = new PurchaseResult();
		purchaseResult.Result = (PurchaseResult.eResult)Convert.ToInt32(array[0]);
		purchaseResult.TransactionID = Guid.NewGuid().ToString();
		if (purchaseResult.Result == PurchaseResult.eResult.FAILED)
		{
			purchaseResult.ErrorDomain = array[2];
			purchaseResult.ErrorCode = int.Parse(array[3]);
		}
		else
		{
			purchaseResult.Signature = this.appStoreManager.Call<string>("GetRecieptSignature", new object[0]);
			purchaseResult.Receipt = this.appStoreManager.Call<string>("GetRecieptSignedData", new object[0]);
			purchaseResult.ProductID = array[2];
		}
		this.appStoreManager.Call("ReleaseTransactionResult", new object[0]);
		this.transactionInPaymentQueue = false;
		return purchaseResult;
	}

	public void ProcessExistingTransactions()
	{
	}

	public void ConsumePurchase(string productCode,string authority)
	{
		throw new NotImplementedException();
	}

	public void Initialize()
	{
		
	}

	public List<string> GetPurchases()
	{
		return null;
	}

    public void Dispose()
    {
    }
}
#endif
