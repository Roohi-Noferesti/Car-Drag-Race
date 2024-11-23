using System;
using UnityEngine;

public class CrewStateFadeOutLeader : BaseCrewState
{
	//private int crew;

	private float timeToScale = 2.5f;

	public CrewStateFadeOutLeader(CrewProgressionScreen zParentScreen, int zCrew) : base(zParentScreen)
	{
		//this.crew = zCrew;
	}

	public CrewStateFadeOutLeader(NarrativeSceneStateConfiguration config) : base(config)
	{
		//this.crew = config.StateDetails.SlotIndex;
		this.timeToScale = config.StateDetails.TimeToScale;
	}

    //private MainCharacterGraphic GetLeader()
    //{
    //    return this.parentScreen.charactersSlots[this.crew].GetMainCharacterGameObject().GetComponent<MainCharacterGraphic>();
    //}

	public override void OnEnter()
	{
	}

	public override bool OnMain()
	{
		base.OnMain();
		float num = this.timeInState / this.timeToScale;
		bool result = false;
		if (num >= 1f)
		{
			result = true;
		}
		num = Mathf.Clamp(num, 0f, 1f);
		num = this.parentScreen.CurveLinear.Evaluate(num);
        //this.GetLeader().GetPortrait().renderer.material.SetFloat("_Greyness", num);
		return result;
	}

	public override void OnExit()
	{
	}
}
