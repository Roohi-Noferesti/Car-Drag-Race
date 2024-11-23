using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class CrewStateLeaderFadeOut : BaseCrewState
{
    private int crew;

    private float fadeTime = 0.4f;

	private bool fadeTextBox = true;

    private bool _isWorldTour;

    private string _animationName;

	public CrewStateLeaderFadeOut(CrewProgressionScreen zParentScreen, int zCrew) : base(zParentScreen)
	{
        this.crew = zCrew;
	    _animationName = "Crew_Exit";
	}

    public CrewStateLeaderFadeOut(NarrativeSceneStateConfiguration config) : base(config)
	{
        this.crew = config.StateDetails.SlotIndex;
        this.fadeTime = config.StateDetails.FadeOutTime;
		this.fadeTextBox = config.StateDetails.FadeTextBox;
        var exitAnimName = this.parentScreen.charactersSlots[crew].ExitAnimationName;
        _animationName = !string.IsNullOrEmpty(exitAnimName) ? exitAnimName : "Crew_Exit_WT";
        _isWorldTour = true;

    }

	private GameObject GetLeader()
	{
		return this.parentScreen.charactersSlots[crew].GetMainCharacterGameObject();
	}

	public override void OnEnter()
    {
        this.parentScreen.charactersSlots[crew].PlayBossAnimation(_animationName);
    }

    public override bool OnMain()
	{
        base.OnMain();
        GameObject leader = this.GetLeader();
        float num = this.timeInState / this.fadeTime;
        bool result = num > 1f;
        float num2 = 1f - Mathf.Clamp(num, 0f, 1f);
        if (_isWorldTour)//this.fadeTextBox)
        {
            //leader.GetComponent<MainCharacterGraphic>().SetTextBorderAlpha(num2);
            this.parentScreen.charactersSlots[crew].SetTextAlpha(num2);
            //leader.GetComponent<MainCharacterGraphic>().SetTextAlpha(num2);
        }
        //this.parentScreen.charactersSlots[this.crew].SetAlpha(4, num2);
        //return result;

        if (parentScreen.charactersSlots[crew].IsPlayingBossAnimation(_animationName))
	        return false;
	    return true;
    }

	public override void OnExit()
	{
	}
}
