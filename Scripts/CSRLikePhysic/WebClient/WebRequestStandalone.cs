using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class WebRequestStandalone : MonoBehaviour
{
	public static WebRequestStandalone Instance;

	private void Awake()
	{
		WebRequestStandalone.Instance = this;
	}

	public void StartCall(string zFunctionName, string zDesc, JsonDict zPostdata, WebClientDelegate2 zCallback = null, object zUserData = null, string zHashString = "")
	{
		base.StartCoroutine(this.WebRequestCoroutine(zFunctionName, zDesc, zPostdata, zCallback, zUserData, zHashString));
	}

	public void StartCallWithHeaders(string zFunctionName, string zDesc, JsonDict zPostdata, WebClientDelegate2 zCallback = null, object zUserData = null, string zHashString = "", JsonDict zHeaders = null)
	{
		base.StartCoroutine(this.WebRequestWithHeadersCoroutine(zFunctionName, zDesc, zPostdata, zCallback, zUserData, zHashString, zHeaders));
	}

    private IEnumerator WebRequestCoroutine(string zFunctionName, string zDesc, JsonDict zPostdata,
        WebClientDelegate2 zCallback = null, object zUserData = null, string zHashString = "")
    {
        var request = new WebRequestQueueWorkItem(zFunctionName, zDesc, zPostdata, zCallback, zUserData, zHashString,
            null, 5);
        request.StartCall();

        while (request.webRequest.IsDone)
        {
            yield return new WaitForEndOfFrame();
        }

        request.FinishedCall();
    }

    private IEnumerator WebRequestWithHeadersCoroutine(string zFunctionName, string zDesc, JsonDict zPostdata,
        WebClientDelegate2 zCallback = null, object zUserData = null, string zHashString = "", JsonDict zHeaders = null)
    {
        var request = new WebRequestQueueWorkItem(zFunctionName, zDesc, zPostdata, zCallback, zUserData, zHashString,
            zHeaders, 5);
        request.StartCallWithHeaders();

        if (request.webRequest.IsDone)
        {
            yield return new WaitForEndOfFrame();
        }
        request.FinishedCall();
    }
}
