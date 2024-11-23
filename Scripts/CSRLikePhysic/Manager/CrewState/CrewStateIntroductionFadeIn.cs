using System;
using UnityEngine;

public class CrewStateIntroductionFadeIn : BaseCrewState
{
    private int crew;

    private float timeToFadeIn = 0.6f;

    private float timeBetweenFades = 0.15f;

    private bool firstUpdate = true;

    private int members;

    private bool tierX;

    private string animationName;

	public CrewStateIntroductionFadeIn(CrewProgressionScreen zParentScreen, int zCrew,int members) : base(zParentScreen)
	{
        this.crew = zCrew;
        this.members = members;
	    tierX = true;
	    animationName = "Crew_Enter";
        this.parentScreen.charactersSlots[crew].ExitAnimationName = "Crew_Exit";
    }

	public CrewStateIntroductionFadeIn(NarrativeSceneStateConfiguration config) : base(config)
	{
        this.crew = config.StateDetails.SlotIndex;
	    tierX = false;
	    animationName = "Crew_Enter2_WT";
        this.parentScreen.charactersSlots[crew].ExitAnimationName = "Crew_Exit2_WT";
    }

    public override void OnEnter()
	{
        //this.parentScreen.charactersSlots[this.crew].SetAlpha(0f);
        this.parentScreen.charactersSlots[crew].PlayBossAnimation(animationName);
	    if (members == 0)
	        this.parentScreen.charactersSlots[crew].PlayAnimation(animationName);
	    else
	    {
	        for (int i = 0; i < members; i++)
	        {
                this.parentScreen.charactersSlots[crew].PlayAnimation(members, animationName);
	        }
	    }
	}

	public override bool OnMain()
	{
		base.OnMain();

        //if (this.firstUpdate)
        //{
        //    this.parentScreen.PlayAnimation("IntroductionFadeIn");
        //    this.firstUpdate = false;
        //}

        if (parentScreen.charactersSlots[crew].IsPlayingBossAnimation(animationName))
            return false;

	    if (members == 0)
	        return !parentScreen.charactersSlots[crew].IsPlayingAnimation(animationName);
	    else
	    {
	        for (int i = 0; i < members; i++)
	        {
	            if (parentScreen.charactersSlots[crew].IsPlayingAnimation(i, animationName))
	                return false;
	        }
	    }
	    return true;
	}

	public override void OnExit()
	{
	}
}
