using System;
using UnityEngine;

public class CrewStateMysteryDonorFadeIn : BaseCrewState
{
	private float timeToFadeIn;

	public CrewStateMysteryDonorFadeIn(CrewProgressionScreen zParentScreen, float zTime) : base(zParentScreen)
	{
		this.timeToFadeIn = zTime;
	}

	public CrewStateMysteryDonorFadeIn(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.timeToFadeIn = config.StateDetails.FadeInTime;
	}

	public override void OnEnter()
	{
	}

	public GameObject GetMysteryDonorContainer()
	{
		return this.parentScreen.GetContainer().transform.Find("MysteryDonor").gameObject;
	}

    //public NitroContainer GetMysteryDonor()
    //{
    //    return this.GetMysteryDonorContainer().GetComponent<NitroContainer>();
    //}

	public override bool OnMain()
	{
		base.OnMain();
		float num = this.timeInState / this.timeToFadeIn;
		bool result = false;
		if (num >= 1f)
		{
			result = true;
		}
		//num = Mathf.Clamp(num, 0f, 1f);
		//float num2 = num;
        //NitroContainer mysteryDonor = this.GetMysteryDonor();
        //mysteryDonor.SetAlpha(num2, 0f);
        //mysteryDonor.SetMysteryDonorAlpha(num2);
        //mysteryDonor.SetTextAlpha(num2);
		return result;
	}

	public override void OnExit()
	{
	}
}
