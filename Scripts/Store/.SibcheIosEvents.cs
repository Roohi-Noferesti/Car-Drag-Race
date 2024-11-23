
#if APPLE_RELEASE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SibcheIosEvents : MonoBehaviour {

    public static SibcheIosEvents _instance;
    public StoreManagerSibche SibcheReference;
    public bool AnswerReturned=false;
    void Awake()
    {
        _instance = this;
    }


//    // this will get called by android plugin
//         public void OnPurchaseResult(string result)
//         {
//          
//     
//             // check exactly what message is returned from android
//             if (result.ToLower() == "completed" )
//             {
//               // SibcheReference.SuccessFulResult = true;
//             }
//             else
//             {
//              //   fortumoReference.SuccessFulResult = false;
//             }
//         //    fortumoReference.HasResult = true;
//          //   fortumoReference.IsProcessingTransaction = false;
//             AnswerReturned = true;
//         }


    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        Debug.Log("return to game preesed");
        
//        if (SibcheReference.IsProcessingTransaction)
//        {
//            
//          SibcheReference.  FinalPurchaseResult= new PurchaseResult();
//          SibcheReference. FinalPurchaseResult.ProductID ="cash_pack_2";//todo ::::: im not sure
//          SibcheReference. FinalPurchaseResult.Market = "sibche";
//          SibcheReference.  FinalPurchaseResult.TransactionID = "sdsadsadasfsafsadsa";//;
//          SibcheReference.  FinalPurchaseResult.Result = PurchaseResult.eResult.SUCCEEDED;
//          SibcheReference.  FinalPurchaseResult.Receipt ="132312321";
//          SibcheReference.  IsProcessingTransaction = false;
//          SibcheReference.  PurchaseResultWaiting = true;
//            //  ConsumePurchase(purchasedPackage.code,"");
//            UnityEngine.Debug.Log("sibche purchase happened success fuly");
//            
//            
//            SibcheReference.IsProcessingTransaction = false;
//            SibcheReference.SibcheNothingHappened();
//           
//            SibcheReference.PurchaseResultWaiting = true;
//
//        }
      //  OnPurchaseResult("ddd");

      SibcheReference.StartProductRequest();
      //SibcheReference.responseWorking = true;
    }


//    private void OnApplicationFocus(bool focused)
//    {
//        Debug.Log("sibche application pause:::>>"+focused);
//
//        if (focused)
//        {
//        }
//
//        else
//        {
//            if (SibcheReference.IsProcessingTransaction)
//            {
//                
//                SibcheReference.IsProcessingTransaction = false;
//                SibcheReference.SibcheNothingHappened();
//                SibcheReference.PurchaseResultWaiting = true;
//            }
//        }
//    }
//    
//    
//    private void OnApplicationPause(bool paused)
//    {
//        Debug.Log("sibche application pause:::>>"+paused);
//
//        if (paused)
//        {
//        }
//
//        else
//        {
//            if (SibcheReference.IsProcessingTransaction)
//            {
//                
//                SibcheReference.IsProcessingTransaction = false;
//                SibcheReference.SibcheNothingHappened();
//                SibcheReference.PurchaseResultWaiting = true;
//            }
//        }
//        
//    }
}

#endif
