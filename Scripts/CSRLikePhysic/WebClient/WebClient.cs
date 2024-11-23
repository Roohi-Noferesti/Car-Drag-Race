using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public class WebClient : MonoBehaviour
{
    public enum HttpStatusCode
	{
		HTTP_STATUS_FAILED_TO_REACH_HOST,
		HTTP_STATUS_OK = 200,
		HTTP_STATUS_CLIENT_ERROR = 400,
		HTTP_STATUS_SESSION_TIMEOUT,
		HTTP_STATUS_LOGIN_AUTHENTICATION = 403,
		HTTP_STATUS_DATA_INCOMPLETE = 406,
		HTTP_STATUS_SERVER_ERROR = 500,
		HTTP_STATUS_SERVER_UNAVAILABLE = 503
	}

	public const float RequestTimeout = 10f;

	private string mAgent;

	private string mLanguages;

	private string mSession;

	private string mLoadBalancerCookie;

	public static string LatestMultiplayerToken = string.Empty;

	private static Dictionary<GTAppStore, string> AppStoreToServerOSMap = new Dictionary<GTAppStore, string>
	{
		{
		    GTAppStore.iOS,
			"iOS"
		},
		{
		    GTAppStore.OSX,
			"OSX"
		},
		{
		    GTAppStore.Windows,
			"WINDOWS"
		},
		{
		    GTAppStore.Windows_Metro,
			"WINDOWS_METRO"
		},
		{
		    GTAppStore.GooglePlay,
			"ANDROID"
		},
		{
		    GTAppStore.Amazon,
			"AMAZON"
		},
		{
			GTAppStore.Zarinpal,
			"Zarinpal"
		},
#if UNITY_ANDROID
	    {
	        GTAppStore.Bazaar,
	        "ANDROID"
        },
	    {
	        GTAppStore.Iraqapps,
	        "ANDROID"
        },
	    {
	        GTAppStore.Myket,
	        "ANDROID"
        },
#endif
	    {
	        GTAppStore.None,
	        "None"
	    }
    };

	public string Session
	{
		get
		{
			return this.mSession;
		}
	}

	public void Init()
	{
		this.mAgent = string.Format("app={0}; ver={1}; dev={2}; os={3}; osver={4}; res={5}; uuid={6}", new object[]
		{
			BasePlatform.ActivePlatform.GetApplicationName(),
			ApplicationVersion.Current,
			BasePlatform.ActivePlatform.GetDeviceType(),
			WebClient.AppStoreToServerOSMap[BasePlatform.ActivePlatform.GetTargetAppStore()],
			BasePlatform.ActivePlatform.GetDeviceOSVersion(),
			Screen.width + "x" + Screen.height,
			BaseIdentity.ActivePlatform.GetUUID()
		});
	    this.mLanguages = LocalizationManager.CurrentLanguage;//ScriptLocalization.ISO6391ToString(LocalizationManager.GetTranslationSystemLanguage());
		//string eC2URL = Endpoint.GetEC2URL(null);
	}

	private WebRequest CreateRequest(string action)
	{
		return this.CreateRequest(action, false);
	}

	private WebRequest CreateRequest(string action, bool save_session)
	{
		string text = (!Endpoint.GetEC2ServerSecure()) ? "http" : "https";
		string text2 = Endpoint.GetEC2URL(null);
		bool flag = action.StartsWith("ryf_");
		if (flag)
		{
			text2 = Endpoint.GetEC2URL_RYF(null);
		}
		bool flag2 = action.StartsWith("rtw_");
		if (flag2)
		{
			text2 = Endpoint.GetEC2URL_RTW(null);
		}
		string url = string.Concat(new string[]
		{
			text,
			"://",
			text2,
			"/",
			action,
			"/"
		});
		WebRequest webRequest;
		if (save_session)
		{
			webRequest = new WebRequest(url, this, "POST");
		}
		else
		{
			webRequest = new WebRequest(url, null, "POST");
		}
		webRequest.SetHeader("Accept-Language", this.mLanguages);
		webRequest.SetHeader("User-Agent", this.mAgent);
		if (this.mSession != null)
		{
			string text3 = WebUtils.getSessionCookieName() + "=" + WWW.EscapeURL(this.mSession);
			if (this.mLoadBalancerCookie != null)
			{
				text3 = text3 + "; AWSELB=" + WWW.EscapeURL(this.mLoadBalancerCookie);
			}
			webRequest.SetHeader("Cookie", text3);
		}
		webRequest.SetTimeout(10f);
		return webRequest;
	}

	private WebRequest CreateRequestWithCustomURL(string url, bool save_session)
	{
		WebRequest webRequest;
		webRequest = save_session ? 
			new WebRequest(url, this, "POST") : 
			new WebRequest(url, null, "POST");
		webRequest.SetHeader("Accept-Language", this.mLanguages);
		webRequest.SetHeader("User-Agent", this.mAgent);
		if (this.mSession != null)
		{
			string text = WebUtils.getSessionCookieName() + "=" + WWW.EscapeURL(this.mSession);
			if (this.mLoadBalancerCookie != null)
			{
				text = text + "; AWSELB=" + WWW.EscapeURL(this.mLoadBalancerCookie);
			}
			webRequest.SetHeader("Cookie", text);
		}
		webRequest.SetTimeout(10f);
		return webRequest;
	}

	public void SetSession(string session, string loadBalancerCookie)
	{
		if (session != this.mSession)
		{
			this.mSession = session;
			this.mLoadBalancerCookie = loadBalancerCookie;
		}
	}

	public void EndSession()
	{
		this.SetSession(null, null);
	}

	public WebRequest ServerFunction(string functionName, JsonDict parameters)
	{
		BasePlatform.eSigningType signingTypeForRequest = WebClient.GetSigningTypeForRequest(functionName);
		WebRequest webRequest = this.CreateRequest(functionName, signingTypeForRequest != BasePlatform.eSigningType.Server_RTW && signingTypeForRequest != BasePlatform.eSigningType.Server_RYF);
		JsonList jsonList = new JsonList();
		string text = string.Empty;
		int num = 0;
		foreach (string current in parameters.Keys)
		{
			jsonList.Add(current);
			string @string = parameters.GetString(current);
			text += @string;
			if (@string==null)
			{
				GTDebug.Log(GTLogChannel.WebClient,"parameter function name : " + functionName + " , key :  " + current + " ,  value : " + @string);
			}
            webRequest.AddFormData("d" + num, BasePlatform.ActivePlatform.TX(@string));
			num++;
		}
		string inputString = jsonList.ToString();
		//Debug.Log("Server function : " + functionName+" , " + parameters.Keys.Count);
		webRequest.AddFormData("param", BasePlatform.ActivePlatform.TX(inputString));
		webRequest.SetRequestHash(BasePlatform.ActivePlatform.HMACSHA1_Hash(text, signingTypeForRequest));
		webRequest.Send();
		return webRequest;
	}

	public WebRequest StartFunctionCall(string zFunctionName, JsonDict zParams, string zStringToHash = "")
	{
		WebRequest webRequest = this.CreateRequest(zFunctionName);
		if (zParams != null)
		{
			foreach (string current in zParams.Keys)
			{
				webRequest.AddFormData(current, zParams.GetString(current));
			}
		}
		if (!string.IsNullOrEmpty(zStringToHash))
		{
			webRequest.SetRequestHash(BasePlatform.ActivePlatform.HMACSHA1_Hash(zStringToHash, BasePlatform.eSigningType.Server_Accounts));
		}
		webRequest.Send();
		return webRequest;
	}

	public WebRequest StartFunctionCallWithHeadersAndURL(string url, JsonDict headers, JsonDict zParams, string zStringToHash = "")
	{
		WebRequest webRequest = this.CreateRequestWithCustomURL(url, false);
		if (headers != null)
		{
			foreach (string current in headers.Keys)
			{
				webRequest.SetHeader(current, headers.GetString(current));
			}
		}
		if (zParams != null)
		{
			foreach (string current2 in zParams.Keys)
			{
				webRequest.AddFormData(current2, zParams.GetString(current2));
			}
		}
		if (!string.IsNullOrEmpty(zStringToHash))
		{
			webRequest.SetRequestHash(BasePlatform.ActivePlatform.HMACSHA1_Hash(zStringToHash, BasePlatform.eSigningType.Server_Accounts));
		}
		webRequest.Send();
		return webRequest;
	}

	public static BasePlatform.eSigningType GetSigningTypeForRequest(string request)
	{
		if (request.StartsWith("rtw_"))
		{
			return BasePlatform.eSigningType.Server_RTW;
		}
		if (request.StartsWith("ryf_"))
		{
			return BasePlatform.eSigningType.Server_RYF;
		}
		return BasePlatform.eSigningType.Server_Accounts;
	}

	public static void ProcessObfuscatedResponse(WebRequest request, BasePlatform.eSigningType signingType, ref string content, ref string error, ref int status)
	{
		status = request.Status;
		content = request.Content;
		error = request.Error;
		if (status != 200 || !string.IsNullOrEmpty(error))
		{
			return;
		}
		
		try
		{
			JsonDict jsonDict = new JsonDict();
			if (jsonDict.Read(content))
			{
				string text = BasePlatform.ActivePlatform.FX(jsonDict.GetString("d1"));
				string @string = jsonDict.GetString("d3");
				string a = BasePlatform.ActivePlatform.HMACSHA1_Hash(text + request.GetRequestHash(), signingType);
				if (a != @string)
				{
					error = "hash mismatch in response";
					status = 406;
				}
				else
				{
					content = text;
				}
			}
			else
			{
				error = "invalid response, not a json dictionary";
				status = 406;
			}
		}
		catch
		{
			MetricsIntegration.Instance.LogCrash(request.Error + " / " + request.Error + " / " + content);
		}
	}
}
