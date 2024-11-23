using System;
using UnityEngine;

public class CrewStateIntroductionPanBack : BaseCrewState
{
	private float panTime = 1.4f;

	private float totalWidth;

	public CrewStateIntroductionPanBack(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateIntroductionPanBack(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.panTime = config.StateDetails.PanTimePerCrew;
	}

	public override void OnEnter()
	{
		this.totalWidth = this.parentScreen.offsetX;
	}

	public override bool OnMain()
	{
	    base.OnMain();
	    float num = this.timeInState / this.panTime;
	    float num2 = this.parentScreen.CurveS.Evaluate(1f - num);
	    num2 = Mathf.Clamp(num2, 0f, 1f);
	    this.parentScreen.offsetX = num2 * this.totalWidth;
	    if (num < 0.5f)
	    {
	        float num3 = 1f - Mathf.Clamp(num * 10f, 0f, 1f);
	        this.parentScreen.charactersSlots[4].SetLogoAlpha(num3);
	        this.parentScreen.charactersSlots[4].SetIntroTextAlpha(num3);
	    }
	    else
	    {
	        float num4 = 1f - Mathf.Clamp((1f - num) * 5f, 0f, 1f);
	        this.parentScreen.charactersSlots[0].SetLogoAlpha(num4);
	        this.parentScreen.charactersSlots[0].SetIntroTextAlpha(num4);
	        this.parentScreen.charactersSlots[0].SetIntroScale(1f);
	    }
	    return this.timeInState > this.panTime;
    }

	public override void OnExit()
	{
	}
}
