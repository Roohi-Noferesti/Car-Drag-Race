using System;
using UnityEngine;

public class CrewStateMysteryDonorFadeOut : BaseCrewState
{
	private float timeToFadeIn;

	//private bool changeName;

	public CrewStateMysteryDonorFadeOut(CrewProgressionScreen zParentScreen, float zTime, bool ChangeName = true) : base(zParentScreen)
	{
		this.timeToFadeIn = zTime;
		//this.changeName = ChangeName;
	}

	public CrewStateMysteryDonorFadeOut(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.timeToFadeIn = config.StateDetails.FadeInTime;
		//this.changeName = config.StateDetails.ChangeName;
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
		//float mysteryDonorAlpha = 1f - num;
        //NitroContainer mysteryDonor = this.GetMysteryDonor();
        //mysteryDonor.SetAlpha(1f, 1f);
        //mysteryDonor.SetMysteryDonorAlpha(mysteryDonorAlpha);
        //if (this.changeName)
        //{
        //    mysteryDonor.SwitchName(LocalizationManager.GetTranslation("TEXT_NAME_NITRO"));
        //}
		return result;
	}

	public override void OnExit()
	{
	}
}
