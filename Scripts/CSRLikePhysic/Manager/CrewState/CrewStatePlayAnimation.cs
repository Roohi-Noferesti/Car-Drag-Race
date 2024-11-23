using UnityEngine;
using System.Collections;

public class CrewStatePlayAnimation : BaseCrewState 
{
	private int member;

    private int crew;

    private string animationName;


	public CrewStatePlayAnimation(CrewProgressionScreen zParentScreen, int zCrew, int zMemberToLeave,string animationToPlay) : base(zParentScreen)
	{
        this.crew = zCrew;
        this.member = zMemberToLeave;
        this.animationName = animationToPlay;
	}

    public CrewStatePlayAnimation(NarrativeSceneStateConfiguration config)
        : base(config)

	{
        this.crew = config.StateDetails.SlotIndex;
        this.member = config.StateDetails.MemberIndex;
        this.animationName = config.StateDetails.AnimationName;

    }

    public override void OnEnter()
    {
        if (member != -1)
            this.parentScreen.charactersSlots[crew].PlayAnimation(animationName);
        this.parentScreen.charactersSlots[crew].PlayBossAnimation(animationName);
    }

    public override bool OnMain()
	{
		base.OnMain();

        if (parentScreen.charactersSlots[crew].IsPlayingBossAnimation(animationName))
            return false;

        if (member == -1)
            return true;

        if (this.parentScreen.charactersSlots[crew].IsPlayingAnimation(animationName))
        {
            return false;
        }
		
        return true;
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
