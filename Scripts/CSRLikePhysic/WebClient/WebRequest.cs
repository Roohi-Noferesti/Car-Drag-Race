using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class WebRequest : IWebRequest
{
    private class Locked
    {
        private HttpWebRequest httpRequest;

        private string responseString;

        private int responseStatus;

        private string responseError;

        private List<FormDataItem> formData = new List<FormDataItem>();

        private bool isDone;

        private WebClient parentWebClient;

        private bool hasParentWebClient;

        public bool IsDone
        {
            get
            {
                bool result;
                lock (this)
                {
                    result = isDone;
                }
                return result;
            }
            set
            {
                lock (this)
                {
                    isDone = value;
                }
            }
        }

        public HttpWebRequest HttpRequest
        {
            get
            {
                HttpWebRequest result;
                lock (this)
                {
                    result = httpRequest;
                }
                return result;
            }
            set
            {
                lock (this)
                {
                    httpRequest = value;
                }
            }
        }

        public List<FormDataItem> FormData
        {
            get
            {
                List<FormDataItem> result;
                lock (this)
                {
                    result = formData;
                }
                return result;
            }
            set
            {
                lock (this)
                {
                    formData = value;
                }
            }
        }

        public string ResponseString
        {
            get
            {
                string result;
                lock (this)
                {
                    result = responseString;
                }
                return result;
            }
            set
            {
                lock (this)
                {
                    responseString = value;
                }
            }
        }

        public int ResponseStatus
        {
            get
            {
                int result;
                lock (this)
                {
                    result = responseStatus;
                }
                return result;
            }
            set
            {
                lock (this)
                {
                    responseStatus = value;
                }
            }
        }

        public string ResponseError
        {
            get
            {
                string result;
                lock (this)
                {
                    result = responseError;
                }
                return result;
            }
            set
            {
                lock (this)
                {
                    responseError = value;
                }
            }
        }

        public WebClient ParentWebClient
        {
            get
            {
                WebClient result;
                lock (this)
                {
                    result = parentWebClient;
                }
                return result;
            }
            set
            {
                lock (this)
                {
                    hasParentWebClient = (value != null);
                    parentWebClient = value;
                }
            }
        }

        public bool HasParentWebClient
        {
            get
            {
                bool result;
                lock (this)
                {
                    result = hasParentWebClient;
                }
                return result;
            }
        }
    }

    private string requestHash = string.Empty;

    private bool inProgress;

    private Locked threadSafeAccessor = new Locked();
    private Dictionary<string, int> map2;

    public string Url
    {
        get
        {
            return threadSafeAccessor.HttpRequest.RequestUri.OriginalString;
        }
    }

    public string Error
    {
        get
        {
            return threadSafeAccessor.ResponseError;
        }
    }

    public int Status
    {
        get
        {
            return threadSafeAccessor.ResponseStatus;
        }
    }

    public string ContentType
    {
        get
        {
            return threadSafeAccessor.HttpRequest.ContentType;
        }
        private set
        {
            threadSafeAccessor.HttpRequest.ContentType = value;
        }
    }

    public string Content
    {
        get
        {
            return threadSafeAccessor.ResponseString;
        }
    }

    public bool IsDone
    {
        get
        {
            return threadSafeAccessor.IsDone;
        }
    }

    public WebRequest(string url, WebClient client, string method = "POST")
    {
        threadSafeAccessor.ParentWebClient = client;
        threadSafeAccessor.HttpRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
        threadSafeAccessor.HttpRequest.Method = method.ToUpper();
        ContentType = "application/x-www-form-urlencoded";
    }

    public void SetRequestHash(string requestHash)
    {
        this.requestHash = requestHash;
        SetHeader(BasePlatform.ActivePlatform.GetSecretName(), this.requestHash);
    }

    public string GetRequestHash()
    {
        return requestHash;
    }

    public void SetHeader(string name, string data)
    {
        if (name != null)
        {
            if (map2 == null)
            {
                map2 = new Dictionary<string, int>(1)
                {
                    {
                        "User-Agent",
                        0
                    }
                };
            }
            int num;
            if (map2.TryGetValue(name, out num))
            {
                if (num == 0)
                {
                    threadSafeAccessor.HttpRequest.UserAgent = data;
                    return;
                }
            }
        }
        try
        {
            threadSafeAccessor.HttpRequest.Headers.Add(name, data);
        }
        catch (ArgumentException ex)
        {
            Debug.LogError(string.Concat("Failed to set header: \"", name, "\" with ", data, " exception ", ex.Message));
        }
    }

    public void SetTimeout(float timeout)
    {
        threadSafeAccessor.HttpRequest.Timeout = 60000;// (int)timeout * 1000;
    }

    private static readonly Dictionary<string, FormDataItem> FormDataItemPool = new Dictionary<string, FormDataItem>();

    public void AddFormData(string name, string data)
    {

        threadSafeAccessor.FormData.Add(new FormDataItem(name, data));

    }

    public void Send()
    {
        if (inProgress)
        {
            return;
        }
        inProgress = true;
        threadSafeAccessor.IsDone = false;
        if (threadSafeAccessor.HttpRequest.Method == "GET")
            ThreadPool.QueueUserWorkItem(GetResponseThreaded);
        else
            ThreadPool.QueueUserWorkItem(SendRequestThreaded);
        //SendRequestThreaded(0);
        //ThreadPool.QueueUserWorkItem(SendRequestThreaded);
    }

    private void Log(string message, params object[] args)
    {
        if (GTDebug.ChannelIsActive(GTLogChannel.WebRequest))
        {
            GTDebug.Log(GTLogChannel.WebRequest,string.Format("WebClient " + message, args));
        }
    }

    private void LogException(Exception e, string url)
    {
        Debug.LogError(url);
        Debug.LogError(string.Concat("WebRequest Exception: ", e.Message, "\n", e.StackTrace, " (turn on WebRequest logging)"));
    }

    private void SendRequestThreaded(object state)
    {
        try
        {
            Log("WebRequest.SendRequestThreaded: Getting request stream.");
            Stream requestStream = threadSafeAccessor.HttpRequest.GetRequestStream();
            string text = string.Join("&", (from formDataItem in threadSafeAccessor.FormData
                select formDataItem.Name + "=" + formDataItem.Value).ToArray());
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
        }
        catch (Exception e)
        {
            LogException(e, threadSafeAccessor.HttpRequest.RequestUri.ToString());
        }
        GetResponseThreaded(null);
    }

    private void GetResponseThreaded(object state)
    {
        try
        {
            Log("WebRequest.GetResponseThreaded: Getting response stream.");
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)threadSafeAccessor.HttpRequest.GetResponse();
                Log("WebRequest.GetResponseThreaded: Got response stream.", httpWebResponse.ResponseUri);// new object[0]);
                string text = httpWebResponse.Headers.Get("Set-Cookie");
                string text2 = string.Empty;
                string text3 = string.Empty;
                if (!string.IsNullOrEmpty(text))
                {
                    string[] array = text.Split(';');
                    string text4 = WebUtils.getSessionCookieName() + "=";
                    for (int i = 0; i < array.Length; i++)
                    {
                        string text5 = array[i];
                        if (text5.Contains(text4))
                        {
                            text2 = text5.Replace(text4, string.Empty);
                        }
                        if (text5.Contains("AWSELB="))
                        {
                            text3 = text5.Substring(text5.IndexOf("AWSELB=") + "AWSELB=".Length);
                        }
                    }
                }
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string text6 = streamReader.ReadToEnd();
                Log("WebRequest.GetResponseThreaded: Got content {0}", text6);
                threadSafeAccessor.ResponseString = text6;
                threadSafeAccessor.ResponseStatus = (int)httpWebResponse.StatusCode;
                threadSafeAccessor.ResponseError = null;
                if (threadSafeAccessor.HasParentWebClient && !string.IsNullOrEmpty(text2))
                {
                    Log("WebRequest.GetResponseThreaded: Got cookies session={0} AWSELB={1}", text2, text3);
                    threadSafeAccessor.ParentWebClient.SetSession(WWW.UnEscapeURL(text2), WWW.UnEscapeURL(text3));
                }
                responseStream.Close();
                streamReader.Close();
                httpWebResponse.Close();
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.ProtocolError)
                {
                    throw ex;
                }
                HttpWebResponse httpWebResponse2 = (HttpWebResponse)ex.Response;
                threadSafeAccessor.ResponseError = httpWebResponse2.StatusDescription;
                threadSafeAccessor.ResponseStatus = (int)httpWebResponse2.StatusCode;
                threadSafeAccessor.ResponseString = new StreamReader(httpWebResponse2.GetResponseStream()).ReadToEnd();
                LogException(ex,threadSafeAccessor.HttpRequest.RequestUri.ToString());
                Debug.LogError("Response: " + threadSafeAccessor.ResponseString + ", Error: " + threadSafeAccessor.ResponseError + " , ErrorCode : "+ threadSafeAccessor.ResponseStatus
                    + " . url : " + httpWebResponse2.ResponseUri);
                int num = 0;
                while (true)
                {
                    string text7 = ex.Response.Headers.Get("X-GT-Trace-" + num);
                    if (string.IsNullOrEmpty(text7))
                    {
                        break;
                    }
                    Log(string.Concat("*** HTTP SERVER TRACE[", num, "] ***: ", WWW.UnEscapeURL(text7)));
                    num++;
                }
            }
            Log("Completed web request. Status '{0}' Error '{1}' Response '{2}'", threadSafeAccessor.ResponseStatus, threadSafeAccessor.ResponseError, threadSafeAccessor.ResponseString);
        }
        catch (Exception e)
        {
            LogException(e,threadSafeAccessor.HttpRequest.RequestUri.ToString());
        }
        threadSafeAccessor.IsDone = true;
    }

    public void Release()
    {
    }

    public void ClearFormData()
    {
        threadSafeAccessor.FormData.Clear();
    }
}
