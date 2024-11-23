using System;
using System.Collections.Generic;
using System.Linq;
using KingKodeStudio;
using UnityEngine;

public class WebRequestQueue : MonoBehaviour
{
	private List<WebRequestQueueWorkItem> queue = new List<WebRequestQueueWorkItem>();

	private bool sessionTimeout;

	private float offlineTimer;

	public string Name;

	public static WebRequestQueue Instance
	{
		get;
		private set;
	}

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

	public bool ShouldProcess()
    {
        return base.gameObject.activeInHierarchy &&
               (SceneLoadManager.Instance == null || SceneLoadManager.Instance.CurrentScene ==
                                                  SceneLoadManager.Scene.Frontend
                                                  || (SceneLoadManager.Instance.CurrentScene ==
                                                      SceneLoadManager.Scene.Race && !RaceController.RaceIsRunning()));
    }

	private void Awake()
	{
		WebRequestQueue.Instance = this;
		UserManager.UserChangedEvent += new UserChangedDelegate(this.OnUserChanged);
	}

	public void StartCall(string zFunctionName, string zDesc, JsonDict zPostdata, WebClientDelegate2 zCallback = null, object zUserData = null, string zHashString = "")
	{
		this.RemoveItems(zFunctionName);
		WebRequestQueueWorkItem zItem = new WebRequestQueueWorkItem(zFunctionName, zDesc, zPostdata, zCallback, zUserData, zHashString, null, 5);
		this.QueueItem(zItem);
	}

	public void RemoveItems(string zFunctionName)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
			WebRequestQueueWorkItem webRequestQueueWorkItem = this.queue[i];
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
		var rtwGETProductsName = "rtw_get_products";
		var rtwGetProductsItem = queue.FirstOrDefault(x=>
		{
			return x.functionName == rtwGETProductsName;
		});

		for (var i = 0; i < this.queue.Count; i++)
		{
			WebRequestQueueWorkItem current = this.queue[i];
			if (current.functionName != rtwGETProductsName)
				current.Callback(null, "WebRequestQueue::ResetQueue() abandoned request '" + current.functionName + "'",
					0);
		}

		queue.Clear();
		if (rtwGetProductsItem != null)
			queue.Add(rtwGetProductsItem);
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
		this.UpdateOfflineTimer();
		if (!this.ShouldProcess())
		{
			return;
		}
		this.ProcessQueue();
	}

	public void ProcessQueue()
	{
		if (this.isOffline)
        {
			return;
		}
		if (!UserManager.Instance.isLoggedIn || this.sessionTimeout)
		{
            this.ProcessLogin();
			return;
		}
		if (!PolledNetworkState.IsNetworkConnected)
		{
            return;
		}
		if (this.queue.Count == 0)
		{
            return;
		}
		WebRequestQueueWorkItem item = this.queue[0];
		this.ProcessQueueItem(item);
	}

	private void ProcessLogin()
	{
		if (UserManager.Instance.requestState == UserManager.RequestState.OK)
		{
			this.sessionTimeout = false;
			UserManager.Instance.StartConnect();
			return;
		}
		if (UserManager.Instance.requestState == UserManager.RequestState.ERROR_NO_NETWORK)
		{
			this.GoOfflineTime(5f);
			UserManager.Instance.ResetState();
			return;
		}
		if (UserManager.Instance.requestState > UserManager.RequestState.ERROR)
		{
			this.GoOfflineTime(900f);
			UserManager.Instance.ResetState();
			return;
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
				this.sessionTimeout = true;
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
