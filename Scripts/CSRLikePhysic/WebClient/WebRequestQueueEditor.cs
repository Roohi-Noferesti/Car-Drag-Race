using System;
using System.Collections.Generic;
using UnityEngine;

public class WebRequestQueueEditor
{
    private List<WebRequestQueueWorkItemEditor> queue = new List<WebRequestQueueWorkItemEditor>();

	private float offlineTimer;

	public string Name;
    public WebClient webClient;

    public static WebRequestQueueEditor Instance;

    public bool IsBusy
	{
		get
		{
			return this.queue.Count > 0 && this.queue[0].webRequest != null;
		}
	}

	public bool isRetrying
	{
		get
		{
			return this.queue.Count > 0 && this.queue[0].retries > 0;
		}
	}

	public bool isOffline
	{
		get
		{
			return this.offlineTimer > 0f;
		}
	}

    public WebRequestQueueEditor(WebClient webClient)
    {
        Instance = this;
        this.webClient = webClient;
        webClient.Init();
    }

    public void StartCall(string zFunctionName, string zDesc, JsonDict zPostdata, WebClientDelegate2 zCallback = null, object zUserData = null, string zHashString = "")
	{
		this.RemoveItems(zFunctionName);
        WebRequestQueueWorkItemEditor zItem = new WebRequestQueueWorkItemEditor(zFunctionName, zDesc, zPostdata, zCallback, zUserData, zHashString, null, 5);
		this.QueueItem(zItem);
	}

	public void RemoveItems(string zFunctionName)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
            WebRequestQueueWorkItemEditor webRequestQueueWorkItem = this.queue[i];
			if (webRequestQueueWorkItem.webRequest == null && webRequestQueueWorkItem.functionName == zFunctionName)
			{
				this.queue.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
	}

	public void ResetQueue()
	{
        foreach (WebRequestQueueWorkItemEditor current in this.queue)
		{
			current.Callback(null, "WebRequestQueue::ResetQueue() abandoned request '" + current.functionName + "'", 0);
		}
		this.queue.Clear();
	}

    private void QueueItem(WebRequestQueueWorkItemEditor zItem)
	{
		this.queue.Add(zItem);
	}

    public void Update()
	{
		this.UpdateOfflineTimer();
		this.ProcessQueue();
	}

	public void ProcessQueue()
	{
		if (this.isOffline)
		{
			return;
		}
		if (this.queue.Count == 0)
		{
			return;
		}
        WebRequestQueueWorkItemEditor item = this.queue[0];
		this.ProcessQueueItem(item);
	}

    private void ProcessQueueItem(WebRequestQueueWorkItemEditor item)
	{
		if (item.webRequest == null)
		{
			item.LogRequest(false);
			item.StartCall();
		}
		else if (item.webRequest.IsDone)
		{
			bool flag = false;
			if (item.webRequest.Status == 0)
			{
				if (item.retries >= item.max_retries)
				{
					this.GoOfflineTime(15f);
				}
			}
			else if (item.webRequest.Status == 401)
			{
				flag = true;
			}
			else if (item.webRequest.Status >= 400)
			{
				flag = (item.webRequest.Status >= 500);
				item.LogRequest(true);
				this.GoOfflineTime(900f);
			}
			if (flag && item.retries < item.max_retries)
			{
				item.ReleaseWebRequest();
				item.retries++;
			}
			else
			{
				item.FinishedCall();
				if (this.queue.Count > 0)
				{
					this.queue.RemoveAt(0);
				}
			}
		}
	}

	private void UpdateOfflineTimer()
	{
		this.offlineTimer -= Time.deltaTime;
		if (this.offlineTimer < 0f)
		{
			this.offlineTimer = 0f;
		}
	}

	public void GoOfflineTime(float zTime)
	{
		this.offlineTimer = zTime;
	}
}
