using System;
using UnityEngine;

public class CrewStateFadeOutMembers : BaseCrewState
{
	private float fadeInTime;

	private float timeBetweenFades;

	private int member;

    private int crew;

    private bool thisIsSecondRaceOf3thmember;

    public CrewStateFadeOutMembers(CrewProgressionScreen zParentScreen, int zCrew, int zMemberToLeave) : this(zParentScreen, zCrew, zMemberToLeave, 0.6f, 0.08f)
	{
	}

	public CrewStateFadeOutMembers(CrewProgressionScreen zParentScreen, int zCrew, int zMemberToLeave, float zFadeTime, float zTimeBetweenFades)
        : base(zParentScreen)

	{
		this.fadeInTime = 0.6f;
		this.timeBetweenFades = 0.08f;
        this.crew = zCrew;
        this.member = zMemberToLeave;
		this.fadeInTime = zFadeTime;
		this.timeBetweenFades = zTimeBetweenFades;
        thisIsSecondRaceOf3thmember = member == 3;

    }

	public CrewStateFadeOutMembers(NarrativeSceneStateConfiguration config)
        : base(config)

	{
		this.fadeInTime = 0.6f;
		this.timeBetweenFades = 0.08f;
        this.crew = config.StateDetails.SlotIndex;
        this.member = config.StateDetails.MemberIndex;
		this.fadeInTime = config.StateDetails.FadeInTime;
		this.timeBetweenFades = config.StateDetails.TimeBetweenFades;
        thisIsSecondRaceOf3thmember = member == 3;
    }

    public override void OnEnter()
	{
        if (member > -1)
        {
            var index = thisIsSecondRaceOf3thmember ? member - 1 : member;
            this.parentScreen.charactersSlots[crew].PlayAnimation(index, "Crew_Bold");
        }
    }

	public override bool OnMain()
	{
		base.OnMain();

        var index = thisIsSecondRaceOf3thmember ? member - 1 : member;


        if (index == -1)
            return true;

		bool result = true;
		for (int i = 0; i < 4; i++)
		{
            if (i > index)
			{
				float num = 1f - this.timeInState / this.fadeInTime + (float)(4 - i) * this.timeBetweenFades;
				if (num > 0.28F)
				{
					result = false;
				}
                num = Mathf.Clamp(num, 0.28F, 1f);
				this.parentScreen.charactersSlots[crew].SetColor(i, num);
			}
		}
		return result;
	}

	public override void OnExit()
	{
        //for (int i = 0; i < 4; i++)
        //{
        //    if (i != this.member)
        //    {
        //        this.parentScreen.charactersSlots[this.crew].SetActiveSlots(i, false);
        //    }
        //}
	}
}
