//using TapsellPlusSDK;
//using UnityEngine;

//public class TestTapselPlus : MonoBehaviour
//{
//    public ZoneIDHolder[] ZoneIDHolders;
//    private int networkIndex;
//    private string status;
//    private string zoneID;
//    void OnGUI()
//    {
//        GUILayout.BeginVertical();
//        GUILayout.Label("Network : " + ZoneIDHolders[networkIndex].NetworkName);
//        GUILayout.Label("Status : "+status);

//        if (GUILayout.Button("Initialize"))
//        {
//            TapsellPlus.Initialize("alsoatsrtrotpqacegkehkaiieckldhrgsbspqtgqnbrrfccrtbdomgjtahflchkqtqosa", success =>
//            {

//            }, failed =>
//            {

//            });
//            TapsellPlus.SetDebugMode(1);
//        }

//        GUILayout.Space(50);

//        if (GUILayout.Button("Next Network"))
//        {
//            networkIndex++;
//            if (networkIndex >= ZoneIDHolders.Length)
//            {
//                networkIndex = 0;
//            }
//        }

//        GUILayout.Space(50);

//        if (GUILayout.Button("Request Interstitial"))
//        {
//            zoneID = null;
//            TapsellPlus.RequestInterstitialAd(ZoneIDHolders[networkIndex].InterstitialZoneID, adModel =>
//            {

//            },reqError=>
//            {

//            });
//        }

//        GUILayout.Space(50);


//        if (GUILayout.Button("Request Rewarded"))
//        {
//            zoneID = null;
//            TapsellPlus.RequestRewardedVideoAd(ZoneIDHolders[networkIndex].RewardedZoneID, adModel =>
//            {

//            }, error =>
//            {
//            });
//        }

//        GUILayout.Space(50);


//        if (GUILayout.Button("Show"))
//        {
//            if (zoneID == null)
//            {
//                status = "Can not show video because zoneId is null";
//            }
//            else
//            {
//                TapsellPlus.ShowRewardedVideoAd(zoneID, adOpened =>
//                {

//                }, adRewarded =>
//                {

//                }, adClosed =>
//                {

//                }, adError =>
//                {

//                });
//            }
//        }
//    }
//}

//[System.Serializable]
//public class ZoneIDHolder
//{
//    public string NetworkName;
//    public string RewardedZoneID;
//    public string InterstitialZoneID;
//}
