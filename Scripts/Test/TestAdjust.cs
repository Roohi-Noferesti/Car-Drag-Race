using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class TestAdjust : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		StartAdjust();
	}

    private void StartAdjust()
    {
        Debug.Log("starting adjust");
#if GT_DEBUG_LOGGING
        AdjustConfig adjustConfig = new AdjustConfig("fkzyvr087qbk", AdjustEnvironment.Sandbox);
        adjustConfig.setLogLevel(AdjustLogLevel.Verbose);
#else
        AdjustConfig adjustConfig = new AdjustConfig("fkzyvr087qbk", AdjustEnvironment.Production);
        adjustConfig.setLogLevel(AdjustLogLevel.Error);
#endif
        adjustConfig.setLogDelegate(msg => GTDebug.Log(GTLogChannel.Android, msg));
        adjustConfig.setSendInBackground(false);
        adjustConfig.setLaunchDeferredDeeplink(true);
        adjustConfig.setEventSuccessDelegate(EventSuccessCallback);
        adjustConfig.setEventFailureDelegate(EventFailureCallback);
        adjustConfig.setSessionSuccessDelegate(SessionSuccessCallback);
        adjustConfig.setSessionFailureDelegate(SessionFailureCallback);
        adjustConfig.setDeferredDeeplinkDelegate(DeferredDeeplinkCallback);
        adjustConfig.setAttributionChangedDelegate(AttributionChangedCallback);
        Adjust.start(adjustConfig);
        Debug.Log("adjust started");
    }

    private void AttributionChangedCallback(AdjustAttribution obj)
    {
        Debug.Log("AttributionChangedCallback : " + obj.trackerToken+" , "+ obj.clickLabel+"  , "+obj.campaign+"  ,  "+obj.adgroup);
    }

    private void DeferredDeeplinkCallback(string obj)
    {
        Debug.Log("DeferredDeeplinkCallback : " + obj);
    }

    private void SessionFailureCallback(AdjustSessionFailure obj)
    {
        Debug.Log("SessionFailureCallback : " + obj.Message + "\nand json string is : " + obj.GetJsonResponse());
    }

    private void SessionSuccessCallback(AdjustSessionSuccess obj)
    {
        Debug.Log("SessionSuccessCallback : " + obj.Message + "\nand json string is : " + obj.GetJsonResponse());
    }

    private void EventFailureCallback(AdjustEventFailure obj)
    {
        Debug.Log("EventFailureCallback : " + obj.EventToken + "\nand json string is : " + obj.GetJsonResponse());
    }

    private void EventSuccessCallback(AdjustEventSuccess obj)
    {
        Debug.Log("EventSuccessCallback : " + obj.EventToken + "\nand json string is : " + obj.GetJsonResponse());
    }
}
*/
