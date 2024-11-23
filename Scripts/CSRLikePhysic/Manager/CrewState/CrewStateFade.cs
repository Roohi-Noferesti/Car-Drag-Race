using System;
using UnityEngine;

public abstract class CrewStateFade : BaseCrewState
{
	private float timeToFade;

	private float startAlpha;

	private float endAlpha;

	public CrewStateFade(CrewProgressionScreen zParentScreen, float a, float b, float t) : base(zParentScreen)
	{
		this.timeToFade = t;
		this.startAlpha = a;
		this.endAlpha = b;
	}

	public CrewStateFade(NarrativeSceneStateConfiguration config, float a, float b, float t) : base(config)
	{
		this.timeToFade = t;
		this.startAlpha = a;
		this.endAlpha = b;
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

	protected abstract void SetAlpha(float alpha);

	public override bool OnMain()
	{
		base.OnMain();
		float num = this.timeInState / this.timeToFade;
		bool result = false;
		if (num >= 1f)
		{
			result = true;
		}
		num = Mathf.Clamp(num, 0f, 1f);
		this.SetAlpha(this.startAlpha * (1f - num) + this.endAlpha * num);
		return result;
	}

	public override void OnExit()
	{
	}
}
