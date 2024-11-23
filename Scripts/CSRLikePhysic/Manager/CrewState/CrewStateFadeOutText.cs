using System;
using UnityEngine;

public class CrewStateFadeOutText : BaseCrewState
{
	//private int crew;

	private float timeToFade;

	public CrewStateFadeOutText(CrewProgressionScreen zParentScreen, int zCrew, float zTimeToFade) : base(zParentScreen)
	{
		this.timeToFade = zTimeToFade;
		//this.crew = zCrew;
	}

	public CrewStateFadeOutText(NarrativeSceneStateConfiguration config) : base(config)
	{
		//this.crew = config.StateDetails.SlotIndex;
		this.timeToFade = config.StateDetails.TimeToFade;
	}

	public override void OnEnter()
	{
	}

	public override bool OnMain()
	{
		base.OnMain();
		//float a = Mathf.Clamp(1f - this.timeInState / this.timeToFade, 0f, 1f);
        //this.parentScreen.charactersSlots[this.crew].IntroText.SetColor(new Color(1f, 1f, 1f, a));
		return this.timeInState > this.timeToFade;
	}

	public override void OnExit()
	{
	}
}
