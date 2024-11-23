using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Metrics;
public class AppsFlyerCallbacks : MonoBehaviour
{

	public PurchaseResult.PurchaseReport Report;

	// Use this for initialization
	void Start () {
		printCallback("AppsFlyerTrackerCallbacks on Start");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void didReceiveConversionData(string conversionData) {
		printCallback ("AppsFlyerTrackerCallbacks:: got conversion data = " + conversionData);
	}
	
	public void didReceiveConversionDataWithError(string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got conversion data error = " + error);
	}
	
	public void didFinishValidateReceipt(string validateResult) {
		printCallback ("AppsFlyerTrackerCallbacks:: got didFinishValidateReceipt  = " + validateResult);
		
	}
	
	public void didFinishValidateReceiptWithError (string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got idFinishValidateReceiptWithError error = " + error);
		
	}
	
	public void onAppOpenAttribution(string validateResult) {
		printCallback ("AppsFlyerTrackerCallbacks:: got onAppOpenAttribution  = " + validateResult);
		
	}
	
	public void onAppOpenAttributionFailure (string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got onAppOpenAttributionFailure error = " + error);
		
	}
	
	public void onInAppBillingSuccess () {
		printCallback ("AppsFlyerTrackerCallbacks:: got onInAppBillingSuccess succcess");
		// Log.AnEvent(Events.AppsFlyerValidationSuccess, new Dictionary<Parameters, string>()
		// {
		// 	{Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
		// 	{Parameters.ProductID, Report.prodID},
		// 	{Parameters.TransactionID, Report.transactionID},
		// 	{Parameters.Signature, Report.signature},
		// 	{Parameters.ErrorText, ""}
		// });
		
	}
	public void onInAppBillingFailure (string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got onInAppBillingFailure error = " + error);
		// Log.AnEvent(Events.AppsFlyerValidationFail, new Dictionary<Parameters, string>()
		// {
		// 	{Parameters.UserID, UserManager.Instance.currentAccount.UserID.ToString()},
		// 	{Parameters.ProductID, Report.prodID},
		// 	{Parameters.TransactionID, Report.transactionID},
		// 	{Parameters.Signature, Report.signature},
		// 	{Parameters.ErrorText, error}
		// });
		
	}

	public void onInviteLinkGenerated (string link) {
		printCallback("AppsFlyerTrackerCallbacks:: generated userInviteLink "+link);
	}

	public void onOpenStoreLinkGenerated (string link) {
		printCallback("onOpenStoreLinkGenerated:: generated store link "+link);
		Application.OpenURL(link);
	}

	void printCallback(string str) {
		Debug.Log(str);
	}
}
