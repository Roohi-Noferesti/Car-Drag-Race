using System;
using UnityEngine;
using UnityEngine.UI;

public class CrewStateBossAndMember : BaseCrewState
{
    private int crew;

    private int member;

	private Vector3 memberStart;

	private Vector3 memberDestination;

	private Vector3 leaderStart;

	private Vector3 leaderDestination;

	private float delay = 0.2f;

	private float movementTime = 0.6f;

	public CrewStateBossAndMember(CrewProgressionScreen zParentScreen, int zCrew, int zMember) : base(zParentScreen)
	{
        this.crew = zCrew;
        this.member = zMember;
	}

	public CrewStateBossAndMember(NarrativeSceneStateConfiguration config) : base(config)
	{
        this.crew = config.StateDetails.SlotIndex;
        this.member = config.StateDetails.MemberIndex;
	}

	private Image GetMember()
	{
	    return this.parentScreen.charactersSlots[this.crew].GetBorder(this.member);
    }

	private GameObject GetLeader()
	{
	    return this.parentScreen.charactersSlots[this.crew].GetMainCharacterGameObject();
    }

	public override void OnEnter()
	{
        //this.memberStart = this.GetMember().transform.position;
        //this.memberDestination = this.parentScreen.transform.FindChild("CenterLeft").position + new Vector3(0.6f, 0f, -0.2f);
        //this.memberDestination.y = this.memberStart.y;
        //this.leaderStart = this.GetLeader().transform.position;
        //this.leaderDestination = this.parentScreen.transform.FindChild("CenterRight").position - new Vector3(0.88f, 0f, 0f);
        //this.leaderDestination = GameObjectHelper.MakeLocalPositionPixelPerfect(this.leaderDestination);
        //this.memberDestination = GameObjectHelper.MakeLocalPositionPixelPerfect(this.memberDestination);
	}

	public override bool OnMain()
	{
		base.OnMain();
		if (this.timeInState < this.delay)
		{
			return false;
		}
        //Image sprite = this.GetMember();
        //GameObject leader = this.GetLeader();
		float num = this.timeInState / this.movementTime - this.delay;
		bool result = num > 1f;
        //num = this.parentScreen.CurveS.Evaluate(Mathf.Clamp(num, 0f, 1f));
        //sprite.transform.position = this.memberDestination * num + this.memberStart * (1f - num);
        //leader.transform.position = this.leaderDestination * num + this.leaderStart * (1f - num);
		return result;
	}

	public override void OnExit()
	{
	}
}
