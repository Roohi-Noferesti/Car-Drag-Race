using System;
using System.Collections.Generic;
using UnityEngine;

public class WebRequestQueueRTW : MonoBehaviour
{
	private List<WebRequestQueueWorkItem> queue = new List<WebRequestQueueWorkItem>();

	public static WebRequestQueueRTW Instance
	{
		get;
		private set;
	}

	public int QueueCount
	{
		get
		{
			return this.queue.Count;
		}
	}

	public bool IsBusy
	{
		get
		{
			return this.queue.Count > 0 && this.queue[0].webRequest != null;
		}
	}

	public bool ShouldProcess()
	{
		return base.gameObject.activeInHierarchy;
	}

	private void Awake()
	{
		WebRequestQueueRTW.Instance = this;
		UserManager.UserChangedEvent += new UserChangedDelegate(this.OnUserChanged);
	}

	public void StartCall(string zFunctionName, string zDesc, JsonDict zPostdata, WebClientDelegate2 zCallback = null, object zUserData = null, string zHashString = "", int zMaxRetries = 5)
	{
		this.QueueItem(new WebRequestQueueWorkItem(zFunctionName, zDesc, zPostdata, zCallback, zUserData, zHashString, null, zMaxRetries));
	}

	public bool IsQueued(string zFunctionName)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
			WebRequestQueueWorkItem webRequestQueueWorkItem = this.queue[i];
			if (webRequestQueueWorkItem.functionName == zFunctionName)
			{
				return true;
			}
		}
		return false;
	}

	public int RetryAttempts(string zFunctionName)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
			WebRequestQueueWorkItem webRequestQueueWorkItem = this.queue[i];
			if (webRequestQueueWorkItem.functionName == zFunctionName)
			{
				return webRequestQueueWorkItem.retries;
			}
		}
		return 0;
	}

	public void RemoveItems(string zFunctionName)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
			WebRequestQueueWorkItem webRequestQueueWorkItem = this.queue[i];
			if (webRequestQueueWorkItem.functionName == zFunctionName)
			{
				if (webRequestQueueWorkItem.webRequest == null)
				{
					this.queue.RemoveAt(i);
				}
				else
				{
					webRequestQueueWorkItem.ClearCallback();
				}
			}
			else
			{
				i++;
			}
		}
	}

	public bool IsBusyWith(string zFunctionName)
	{
		return this.queue.Count > 0 && this.queue[0].webRequest != null && this.queue[0].functionName == zFunctionName;
	}

	public void ResetQueue()
	{
		foreach (WebRequestQueueWorkItem current in this.queue)
		{
			string error = "WebRequestQueueRTW::ResetQueue() abandoned request '" + current.functionName + "'";
			current.Callback(null, error, 0);
		}
		this.queue.Clear();
	}

	private void QueueItem(WebRequestQueueWorkItem zItem)
	{
		this.queue.Add(zItem);
	}

	private void OnUserChanged()
	{
		this.ResetQueue();
	}

	private void Update()
	{
		if (!this.ShouldProcess())
		{
			return;
		}
		this.ProcessQueue();
	}

	public void ProcessQueue()
	{
		if (!PolledNetworkState.IsNetworkConnected)
		{
			return;
		}
		if (this.queue.Count == 0)
		{
			return;
		}
		WebRequestQueueWorkItem webRequestQueueWorkItem = this.queue[0];
		if (webRequestQueueWorkItem.HasEmptyTokens())
		{
			webRequestQueueWorkItem.InjectTokens();
		}
        else if (webRequestQueueWorkItem.retries == 0 || GTDateTime.Now > webRequestQueueWorkItem.lastRequestTime.AddSeconds((double)Mathf.Pow(2f, (float)(webRequestQueueWorkItem.retries + 1))))
		{
			this.ProcessQueueItem(webRequestQueueWorkItem);
		}
		else if (this.queue.Count > 1)
		{
			this.queue.Add(webRequestQueueWorkItem);
			this.queue.RemoveAt(0);
		}
	}

	private void ProcessQueueItem(WebRequestQueueWorkItem item)
	{
		if (item.webRequest == null)
		{
			item.LogRequest(false);
			item.StartCall();
		}
		else if (item.webRequest.IsDone)
		{
			int status = item.webRequest.Status;
			bool flag = false;
			if (status == 0 || status == 406)
			{
				if (status == 0)
				{
				}
				flag = true;
			}
			else if (status == 401)
			{
				item.ClearTokens();
				UserManager.Instance.ResetTokens();
				UserManager.Instance.StartConnect();
				flag = true;
				item.LogRequest(false);
			}
			else if (status >= 400)
			{
				item.LogRequest(true);
			}
			if (this.queue.Count > 0)
			{
				this.queue.RemoveAt(0);
			}
			if (flag && item.retries < item.max_retries)
			{
				item.retries++;
				item.ReleaseWebRequest();
				this.queue.Add(item);
			}
			else
			{
				item.FinishedCall();
			}
		}
	}

    internal void StartCall(string v1, string v2, object responseCallback, object userID, string empty, int v3)
    {
        throw new NotImplementedException();
    }
}
