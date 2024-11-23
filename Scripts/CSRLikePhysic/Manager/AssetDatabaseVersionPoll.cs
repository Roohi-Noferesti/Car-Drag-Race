using KingKodeStudio;
using UnityEngine;

public sealed class AssetDatabaseVersionPoll : MonoBehaviour
{
	public delegate void PollCompleteDelegate(int serverVersion);

	public static int LatestVersion;

	public WebRequest webRequest;

	private float timeSinceLastPoll;

	public static float pollFrequency = 900f;

    public static event PollCompleteDelegate PollCompleteEvent;

	public static AssetDatabaseVersionPoll Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
		timeSinceLastPoll = 0f;
	}

	public void PollNow()
	{
		if (webRequest == null)
		{
			string zFileName = AssetDatabaseClient.Instance.GetAssetDatabaseFilename(UserManager.Instance.currentAccount.AssetDatabaseBranch) + ".version";
			string s3URL = Endpoint.GetS3URL(zFileName);
			StartRequest(s3URL);
		}
	}

	private void StartRequest(string url)
	{
		FinishRequest();
		webRequest = new WebRequest(url, null, "GET");
		webRequest.Send();
	}

	private void FinishRequest()
	{
		if (webRequest != null)
		{
			webRequest.Release();
			webRequest = null;
		}
	}

	private void Update()
	{
		if (ScreenManager.Instance.CurrentScreen == ScreenID.Splash)
		{
			return;
		}
		if ((SceneLoadManager.Instance != null && SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Frontend) )//|| MultiplayerUtils.IsPlayingMultiplayer())
		{
			return;
		}

	    if (!PolledNetworkState.IsNetworkConnected)
	    {
	        return;
	    }

		timeSinceLastPoll += Time.deltaTime;
		if (webRequest == null)
		{
			if (timeSinceLastPoll >= pollFrequency)
			{
				PollNow();
				timeSinceLastPoll = 0f;
			}
			return;
		}
		if (webRequest.IsDone)
		{
			if (webRequest.Error == null)
			{
				if (webRequest.Status == 200)
				{
                    if (string.IsNullOrEmpty(webRequest.Content) || webRequest.Content.Contains("<") ||
                        webRequest.Content.Contains(">"))
                    {
                        FinishRequest();
                        return;
                    }

                    JsonDict jsonDict = new JsonDict();
                    if (jsonDict.Read(webRequest.Content))
                    {
                        string url = webRequest.Url;
                        FinishRequest();
                        if (PollCompleteEvent != null)
                        {
                            int num = 0;
                            jsonDict.TryGetValue("version", out num);
                            if (num > 0)
                            {
                                LatestVersion = num;
                                PollCompleteEvent(num);
                            }
                        }
                    }
                }
			}
			FinishRequest();
		}
	}
}
