using System;
using UnityEngine;

public class CrewStateFadeInNewsText : BaseCrewState
{
	private float timeToWait;

	private float timeToFade;

	//private string newsItem;

	public CrewStateFadeInNewsText(CrewProgressionScreen zParentScreen, string zNewsItem, float zTimeToWait, float zTimeToFade) : base(zParentScreen)
	{
		this.timeToWait = zTimeToWait;
		this.timeToFade = zTimeToFade;
		//this.newsItem = zNewsItem;
	}

	public CrewStateFadeInNewsText(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.timeToWait = config.StateDetails.TimeToWait;
		this.timeToFade = config.StateDetails.TimeToFade;
		//this.newsItem = config.StateDetails.GetTranslatedMessage(this.gameState);
	}

	public override void OnEnter()
	{
		this.parentScreen.NewsItem.gameObject.SetActive(true);
        //this.parentScreen.NewsItem.SetColor(new Color(1f, 1f, 1f, 0f));
        //this.parentScreen.NewsItem.Text = this.newsItem;
	}

	public override bool OnMain()
	{
		base.OnMain();
		if (this.timeInState > this.timeToWait)
		{
			//float a = Mathf.Clamp((this.timeInState - this.timeToWait) / this.timeToFade, 0f, 1f);
            //this.parentScreen.NewsItem.SetColor(new Color(1f, 1f, 1f, a));
			if (this.timeInState > this.timeToWait + this.timeToFade)
			{
				return true;
			}
		}
		return false;
	}

	public override void OnExit()
	{
	}
}
