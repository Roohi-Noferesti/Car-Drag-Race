using System;
using I2.Loc;
using UnityEngine;

public class CrewStateIntroductionPan : BaseCrewState
{
	private int crewShowing;

	private float panTimePerCrew = 4.5f;

	private float panTimer;

	private float percentageTimeInSlow = 0.925f;

	private float distanceForSlow = 0.2f;

	private float delay = 1f;

	private bool wasSlow;

	private bool startedEndLoad;

	public CrewStateIntroductionPan(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateIntroductionPan(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.panTimePerCrew = config.StateDetails.PanTimePerCrew;
		this.percentageTimeInSlow = config.StateDetails.PercentageTimeInSlow;
		this.distanceForSlow = config.StateDetails.DistanceForSlow;
		this.delay = config.StateDetails.Delay;
	}

	public override void OnEnter()
	{
		if (this.crewShowing == 0)
		{
            TexturePack.PrecacheBundle("CrewPortraitsTier2");
        }
	}

	private void SetLastIntroTextForFirstCrew()
	{
		NarrativeSceneCharactersContainer narrativeSceneCharactersContainer = this.parentScreen.charactersSlots[0];
        narrativeSceneCharactersContainer.SetIntroText(LocalizationManager.GetTranslation("TEXT_ULTIMATE_GOAL_5"));
        narrativeSceneCharactersContainer.SetIntroTextAlpha(0f);
	}

	public override bool OnMain()
	{
        base.OnMain();
        bool flag = false;
        if (this.timeInState < this.delay)
        {
            this.panTimer += Time.deltaTime * (this.timeInState / this.delay) * (this.timeInState / this.delay);
            flag = true;
        }
        float num = this.parentScreen.charactersSlots[0].GetTotalWidth() + this.parentScreen.spacingBetweenEachTier;
        float num2 = this.panTimer / this.panTimePerCrew;
        this.parentScreen.charactersSlots[this.crewShowing].SetIntroScale(1f + num2 * 0.4f);
        if (num2 < 0.5f)
        {
            float num3 = Mathf.Clamp(num2 * 10f, 0f, 1f);
            this.parentScreen.charactersSlots[this.crewShowing].SetLogoAlpha(num3);
            this.parentScreen.charactersSlots[this.crewShowing].SetIntroTextAlpha(num3);
        }
        else
        {
            float num4 = Mathf.Clamp((1f - num2) * 10f, 0f, 1f);
            this.parentScreen.charactersSlots[this.crewShowing].SetLogoAlpha(num4);
            this.parentScreen.charactersSlots[this.crewShowing].SetIntroTextAlpha(num4);
        }
        float num5;
        if (num2 < this.percentageTimeInSlow)
        {
            num5 = this.parentScreen.CurveLinear.Evaluate(num2 / this.percentageTimeInSlow) * this.distanceForSlow;
            this.wasSlow = true;
        }
        else if (this.crewShowing == 4)
        {
            if (this.startedEndLoad && this.parentScreen.charactersSlots[0].isLoaded)
            {
                this.SetLastIntroTextForFirstCrew();
                return true;
            }
            if (!this.startedEndLoad)
            {
                this.parentScreen.LoadCharactersForCrew(0, null);
                this.startedEndLoad = true;
            }
            return false;
        }
        else
        {
            if (this.wasSlow)
            {
                this.parentScreen.LoadCharactersForCrew(this.crewShowing + 1, null);
                this.wasSlow = false;
            }
            num5 = this.distanceForSlow + this.parentScreen.CurveS.Evaluate((num2 - this.percentageTimeInSlow) / (1f - this.percentageTimeInSlow)) * (1f - this.distanceForSlow);
        }
        num5 = Mathf.Clamp(num5, 0f, 1f);
        this.parentScreen.offsetX = ((float)this.crewShowing + num5) * num * -1f;
        if (this.panTimer > this.panTimePerCrew)
        {
            this.parentScreen.UnloadCharactersForCrew(this.crewShowing);
            this.crewShowing++;
            this.panTimer = -0.1f;
            if (this.crewShowing < 4)
            {
                TexturePack.PrecacheBundle("CrewPortraitsTier" + (this.crewShowing + 2));
            }
            else
            {
                TexturePack.PrecacheBundle("CrewPortraitsTier1");
            }
        }
        else if (!flag)
        {
            this.panTimer += Mathf.Clamp(Time.deltaTime, 0f, 0.1f);
        }
        return false;
    }

	public override void OnExit()
	{
	}
}
