using System;
using System.Collections.Generic;
using System.Linq;

public class WebRequestQueueWorkItemEditor
{
	public WebRequest webRequest;

	private WebClientDelegate2 callback2;

	private JsonDict headers;

	private JsonDict postdata;

	private object userData;

	public string desc;

	public int retries;

	public readonly int max_retries;

	public DateTime lastRequestTime = DateTime.MinValue;

	public string hashString;

	public bool silent;

	public List<string> FilterMethods = new List<string>
	{
		"rtw_",
		"acc_",
		"ryf_",
		"dynamic_client_config"
	};

	public string functionName
	{
		get;
		private set;
	}

    public WebRequestQueueWorkItemEditor(string zFunctionName, string zDesc, JsonDict zPostdata, WebClientDelegate2 zCallback, object zUserData = null, string zHashString = "", JsonDict zHeaders = null, int zMaxRetries = 5)
	{
		this.functionName = zFunctionName;
		this.postdata = zPostdata;
		this.callback2 = zCallback;
		this.userData = zUserData;
		this.desc = zDesc;
		this.hashString = zHashString;
		this.silent = true;
		this.headers = zHeaders;
		this.max_retries = zMaxRetries;
	}

	public void ClearCallback()
	{
		this.callback2 = null;
	}

	public void ClearTokens()
	{
		if (this.postdata.Exists("ryft"))
		{
			this.postdata.Set("ryft", string.Empty);
		}
		if (this.postdata.Exists("mpt"))
		{
			this.postdata.Set("mpt", string.Empty);
		}
	}

	public bool IsRTW()
	{
		return this.postdata.Exists("mpt");
	}

	public bool IsRYF()
	{
		return this.postdata.Exists("ryft");
	}

	public void StartCall()
	{

	    this.ReleaseWebRequest();
		if (this.FilterMethods.Any((string f) => this.functionName.StartsWith(f)))
		{
			this.webRequest = WebRequestQueueEditor.Instance.webClient.ServerFunction(this.functionName, this.postdata);
		}
		else
		{
            this.webRequest = WebRequestQueueEditor.Instance.webClient.StartFunctionCall(this.functionName, this.postdata, this.hashString);
		}
		this.lastRequestTime = GTDateTime.Now;
	}

	public void StartCallWithHeaders()
	{
		this.ReleaseWebRequest();
        this.webRequest = WebRequestQueueEditor.Instance.webClient.StartFunctionCallWithHeadersAndURL(this.functionName, this.headers, this.postdata, this.hashString);
        this.lastRequestTime = GTDateTime.Now;
	}

	public void FinishedCall()
	{
		if (!this.webRequest.IsDone)
		{
			return;
		}
		string error = this.webRequest.Error;
		int status = this.webRequest.Status;
		string content = this.webRequest.Content;
        if (this.FilterMethods.Any((string f) => this.functionName.StartsWith(f)))
        {
            WebClient.ProcessObfuscatedResponse(this.webRequest, WebClient.GetSigningTypeForRequest(this.functionName), ref content, ref error, ref status);
        }
		this.FinishedCall(content, error, status);
		this.ReleaseWebRequest();
	}

	public void ReleaseWebRequest()
	{
		if (this.webRequest != null)
		{
			this.webRequest.Release();
			this.webRequest = null;
		}
	}

	private void FinishedCall(string httpContent, string error, int status)
	{
		this.Callback(httpContent, error, status);
	}

	public void Callback(string httpContent, string error, int status)
	{
		if (this.callback2 != null)
		{
			this.callback2(httpContent, error, status, this.userData);
			this.callback2 = null;
			return;
		}
	}

	public void LogRequest(bool zAsError)
	{
		string text = "functionName=" + this.functionName + "\n";
		foreach (string current in this.postdata.Keys)
		{
			string text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				current,
				"=",
				this.postdata.GetString(current),
				"\n"
			});
		}
		if (zAsError)
		{
		}
	}
}
