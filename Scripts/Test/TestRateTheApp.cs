using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRateTheApp : MonoBehaviour
{
#if UNITY_ANDROID

    void Start()
    {
        AndroidSpecific.Initialise();
    }

    public void RateGooglePlay()
    {
        //RateTheAppNagger._nativeTriggerRateAppPageGooglePlay();
    }


    public void RateBazaar()
    {
        //RateTheAppNagger._nativeTriggerRateAppPageBazaar();
    }


    public void RateIranapps()
    {
        //RateTheAppNagger._nativeTriggerRateAppPageIranapps();
    }


    public void RateMyket()
    {
        //RateTheAppNagger._nativeTriggerRateAppPageMyket();
    }
#endif
}
