using System;
using UnityEngine;

public class CrewStateMysteryDonorTalkFadeOut : BaseCrewState
{
	private float timeForState = 0.4f;

	public CrewStateMysteryDonorTalkFadeOut(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateMysteryDonorTalkFadeOut(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.timeForState = config.StateDetails.TotalTime;
	}

	public GameObject GetNitroContainer()
	{
		return this.parentScreen.GetContainer().transform.Find("MysteryDonor").gameObject;
	}

    //public NitroContainer GetNitroComponent()
    //{
    //    return this.GetNitroContainer().GetComponent<NitroContainer>();
    //}

	public override void OnEnter()
	{
	}

	public override bool OnMain()
	{
		base.OnMain();
        //NitroContainer nitroComponent = this.GetNitroComponent();
        //float num = 1f - Mathf.Clamp(this.timeInState / this.timeForState, 0f, 1f);
        //nitroComponent.Leader.SetTextAlpha(num);
        //nitroComponent.Leader.SetTextBorderAlpha(num);
		bool result = false;
		if (this.timeInState > this.timeForState)
		{
			result = true;
		}
		return result;
	}

	public override void OnExit()
	{
	}
}
