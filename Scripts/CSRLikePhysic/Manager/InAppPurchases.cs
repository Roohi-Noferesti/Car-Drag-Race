using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class InAppPurchases : ScriptableObject
{
    private GTProduct[] _abTestReadyGtProducts;

    public GTProduct[] ABTestReadyGtProducts
    {
        get
        {
            if (_abTestReadyGtProducts == null || _abTestReadyGtProducts.Length == 0)
                _abTestReadyGtProducts = GTProducts;
            return _abTestReadyGtProducts;
        }
    }
    [SerializeField] private GTProduct[] GTProducts;
    public GTProduct[] GTProducts_d15;
    public GTProduct[] GTProducts_d30;

    public GTProduct RetrieveFirstPurchaseItem(string offerItem)
    {
        return ABTestReadyGtProducts.FirstOrDefault(gtProduct => gtProduct.Code == offerItem);
    }

    public int GetProductsListLength()
    {
        return _abTestReadyGtProducts.Length;
    }
    
}
