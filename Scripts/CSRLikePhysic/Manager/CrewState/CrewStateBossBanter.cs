using System;
using UnityEngine;

public class CrewStateBossBanter : BaseCrewState
{
    private int leader1;

    private int leader2;

	private Vector3 leader1Start;

	private Vector3 leader1Destination;

	private Vector3 leader2Start;

	private Vector3 leader2Destination;

	private float delay;

    private const string animationName = "Crew_Enter_WT";

	public CrewStateBossBanter(CrewProgressionScreen zParentScreen, int zLeader1, int zLeader2) : this(zParentScreen, zLeader1, zLeader2, 0.8f)
	{
	}

    public CrewStateBossBanter(CrewProgressionScreen zParentScreen, int zLeader1, int zLeader2, float zDelay)
        : base(zParentScreen)
	{
		this.delay = 0.8f;
        this.leader1 = zLeader1;
        this.leader2 = zLeader2;
		this.delay = zDelay;
	}

	public CrewStateBossBanter(NarrativeSceneStateConfiguration config)
        : base(config)
	{
		this.delay = 0.8f;
        this.leader1 = config.StateDetails.SlotIndex;
        this.leader2 = config.StateDetails.SecondarySlotIndex;
		this.delay = config.StateDetails.Delay;
	}

	private GameObject GetLeader1()
	{
	    return this.parentScreen.charactersSlots[this.leader1].GetMainCharacterGameObject();
    }

	private GameObject GetLeader2()
	{
	    if (this.leader2 < 0)
	    {
	        return null;
	    }
	    return this.parentScreen.charactersSlots[this.leader2].GetMainCharacterGameObject();
    }

    public override void OnEnter()
    {
        //GameObject leaderObject1 = this.GetLeader1();
        //GameObject leaderObject2 = this.GetLeader2();
        //Vector3 position = this.parentScreen.transform.FindChild("CenterLeft").position;
        //Vector3 position2 = this.parentScreen.transform.FindChild("CenterRight").position;
        //Vector3 b = new Vector3(1f, -0.04f, 0f);
        //this.leader1Destination = position + new Vector3(0.8f, -0.04f, 0f);
        //      //this.leader1Destination = GameObjectHelper.MakeLocalPositionPixelPerfect(this.leader1Destination);
        //if (leaderObject1.transform.position == this.leader1Destination)
        //{
        //	this.leader1Start = leaderObject1.transform.position;
        //}
        //else
        //{
        //	this.leader1Start = position2 + b;
        //	leaderObject1.transform.position = this.leader1Start;
        //}
        //this.leader2Destination = position2 + new Vector3(-0.88f, -0.04f, 0f);
        //      //this.leader2Destination = GameObjectHelper.MakeLocalPositionPixelPerfect(this.leader2Destination);
        //if (leaderObject2 != null)
        //{
        //	if (leaderObject2.transform.position == this.leader2Destination)
        //	{
        //		this.leader2Start = leaderObject2.transform.position;
        //	}
        //	else
        //	{
        //		this.leader2Start = position2 + b;
        //		leaderObject2.transform.position = this.leader2Start;
        //	}
        //}


        this.parentScreen.charactersSlots[leader2].PlayBossAnimation(animationName);
    }

    public override bool OnMain()
	{
        //base.OnMain();
        //if (this.timeInState < this.delay)
        //{
        //	return false;
        //}
        //GameObject leaderObject1 = this.GetLeader1();
        //GameObject leaderObject2 = this.GetLeader2();
        //float num = this.timeInState - this.delay;
        //bool result = num > 1f;
        //num = this.parentScreen.CurveS.Evaluate(Mathf.Clamp(num, 0f, 1f));
        //leaderObject1.transform.position = this.leader1Destination * num + this.leader1Start * (1f - num);
        //if (leaderObject2 != null)
        //{
        //	leaderObject2.transform.position = this.leader2Destination * num + this.leader2Start * (1f - num);
        //}
        //return result;


	    if (parentScreen.charactersSlots[leader2].IsPlayingBossAnimation(animationName))
	        return false;
	    return true;
	}

    public override void OnExit()
	{
	}
}
