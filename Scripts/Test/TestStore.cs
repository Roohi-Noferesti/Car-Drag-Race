using KingKodeStudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestStore : MonoBehaviour
{
    #if UNITY_ANDROID
    [SerializeField] private Text m_resultText;
    void Start()
    {
//        StoreEvents.OnMarketPurchase += Market_Purchased;
//        StoreEvents.OnSoomlaStoreInitialized += Soomla_Initialized;
//        StoreEvents.OnBillingNotSupported += Billing_NotSupported;

        AndroidSpecific.Initialise();

    }

    private void Billing_NotSupported()
    {
        m_resultText.text = "Billing not supported";
    }

    private void Soomla_Initialized()
    {
        m_resultText.text = "Billing initialized";
    }

//    private void Market_Purchased(PurchasableVirtualItem arg1, string arg2, Dictionary<string, string> arg3)
//    {
//        m_resultText.text = "market item purchased " + arg1.ID;
//    }

    public void Purchase()
    {
//        if (!SoomlaStore.Initialized)
//        {
//            SoomlaStore.Initialize(new StoreAssets());
//            return;
//        }
//        SoomlaStore.BuyMarketItem("gold_pack_1", "");
    }

    public void RateTheApp()
    {
        //  RateTheAppNagger.TriggerRateAppPage();
        ScreenManager.Instance.PushScreen(ScreenID.UserRatingGame);
    }

    public void UpdateTheGame()
    {

    }
#endif
}
