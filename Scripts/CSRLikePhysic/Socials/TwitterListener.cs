using System;
using UnityEngine;

public class TwitterListener : MonoBehaviour
{
	public TwitterSentCallBack TwitterFinished;

	public void Update()
	{
		int num = BasePlatform.ActivePlatform.TweetGetResolution();
		if (num != 0)
		{
			if (this.TwitterFinished != null)
			{
				this.TwitterFinished(num > 0);
			}
			this.TwitterFinished = null;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
