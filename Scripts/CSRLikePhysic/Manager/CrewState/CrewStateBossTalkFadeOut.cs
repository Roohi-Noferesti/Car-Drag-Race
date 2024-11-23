using System;
using UnityEngine;

public class CrewStateBossTalkFadeOut : BaseCrewState
{
	private const float timeForState = 0.4f;

    private int crew;

    public CrewStateBossTalkFadeOut(CrewProgressionScreen zParentScreen, int zCrew) : base(zParentScreen)
	{
        this.crew = zCrew;
    }

	public CrewStateBossTalkFadeOut(NarrativeSceneStateConfiguration config) : base(config)
	{
        this.crew = config.StateDetails.SlotIndex;
    }

	private GameObject GetLeader()
	{
	    return this.parentScreen.charactersSlots[this.crew].GetMainCharacterGameObject();
    }

	public override void OnEnter()
	{
	}

	public override bool OnMain()
	{
	    base.OnMain();
	    //MainCharacterGraphic component = this.GetLeader().GetComponent<MainCharacterGraphic>();
	    float num = 1f - Mathf.Clamp(this.timeInState / 0.4f, 0f, 1f);
        //component.SetTextAlpha(num);
        //component.SetTextBorderAlpha(num);
	    this.parentScreen.charactersSlots[crew].SetTextAlpha(num);
        return this.timeInState > 0.4f;
    }

	public override void OnExit()
	{
	}
}
