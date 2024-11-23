using System;
using UnityEngine;

public class CrewStateFadeOut : BaseCrewState
{
    private int crew;

    private float timeToFadeOut = 0.6f;

	private float timeBetweenFades = 0.15f;

	public CrewStateFadeOut(CrewProgressionScreen zParentScreen, int zCrew) : base(zParentScreen)
	{
        this.crew = zCrew;
    }

	public CrewStateFadeOut(NarrativeSceneStateConfiguration config) : base(config)
	{
        this.crew = config.StateDetails.SlotIndex;
    }

	public override void OnEnter()
	{
	}

	public override bool OnMain()
	{
	    base.OnMain();
	    bool result = true;
	    for (int i = 0; i <= 4; i++)
	    {
	        float num = this.timeInState / this.timeToFadeOut + (float)i * this.timeBetweenFades;
	        if (num > 0f)
	        {
	            result = false;
	        }
	        num = 1f - Mathf.Clamp(num, 0f, 1f);
	        this.parentScreen.charactersSlots[this.crew].SetAlpha(i, num);
	    }
	    return result;
    }

	public override void OnExit()
	{
	}
}
