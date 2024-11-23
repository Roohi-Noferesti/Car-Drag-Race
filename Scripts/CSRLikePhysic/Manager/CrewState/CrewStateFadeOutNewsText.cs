using System;
using UnityEngine;

public class CrewStateFadeOutNewsText : BaseCrewState
{
	private float timeToWait;

	private float timeToFade;

	public CrewStateFadeOutNewsText(CrewProgressionScreen zParentScreen, float zTimeToWait, float zTimeToFade) : base(zParentScreen)
	{
		this.timeToWait = zTimeToWait;
		this.timeToFade = zTimeToFade;
	}

	public CrewStateFadeOutNewsText(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.timeToWait = config.StateDetails.TimeToWait;
		this.timeToFade = config.StateDetails.TimeToFade;
	}

	public override void OnEnter()
	{
	}

	public override bool OnMain()
	{
		base.OnMain();
		if (this.timeInState > this.timeToWait)
		{
			//float a = Mathf.Clamp(1f - (this.timeInState - this.timeToWait) / this.timeToFade, 0f, 1f);
            //this.parentScreen.NewsItem.SetColor(new Color(1f, 1f, 1f, a));
			if (this.timeInState > this.timeToWait + this.timeToFade)
			{
				this.parentScreen.NewsItem.gameObject.SetActive(false);
				return true;
			}
		}
		return false;
	}

	public override void OnExit()
	{
	}
}
