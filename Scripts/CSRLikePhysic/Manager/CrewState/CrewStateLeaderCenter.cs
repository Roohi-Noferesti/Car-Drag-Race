using System;
using UnityEngine;

public class CrewStateLeaderCenter : BaseCrewState
{
    private int crew;

    private Vector3 leaderStart;

    private Vector3 leaderDestination;

    private float movementTime = 2f;

    private float greyness;

    public CrewStateLeaderCenter(CrewProgressionScreen zParentScreen, int zCrew, float zGreyness) : base(zParentScreen)
	{
        this.crew = zCrew;
        this.greyness = zGreyness;
    }

	public CrewStateLeaderCenter(NarrativeSceneStateConfiguration config) : base(config)
	{
        this.crew = config.StateDetails.SlotIndex;
        this.greyness = config.StateDetails.Greyness;
    }

	private GameObject GetLeader()
	{
	    return this.parentScreen.charactersSlots[this.crew].GetMainCharacterGameObject();
    }

	public override void OnEnter()
	{
	    //float screenWidth = GUICamera.Instance.ScreenWidth;
     //   GameObject leader = this.GetLeader();
	    //MainCharacterGraphic component = leader.GetComponent<MainCharacterGraphic>();
	    //this.leaderStart = this.parentScreen.transform.FindChild("CenterRight").position + new Vector3(1f, -0.12f, 0f);
	    //this.leaderDestination = this.parentScreen.transform.FindChild("CenterLeft").position;// + new Vector3(screenWidth * 0.5f - component.GetWidth() * 0.5f, -0.12f, 0f);
	    //this.leaderDestination = GameObjectHelper.MakeLocalPositionPixelPerfect(this.leaderDestination);
	    //this.parentScreen.charactersSlots[this.crew].SetAlpha(0f);
	    //this.parentScreen.charactersSlots[this.crew].SetAlpha(4, 1f);
	    ////component.GetPortrait().renderer.material.SetFloat("_Greyness", this.greyness);
	    //leader.transform.position = this.leaderStart;
    }

	public override bool OnMain()
	{
		base.OnMain();
  //      GameObject leader = this.GetLeader();
  //      float num = this.timeInState / this.movementTime;
		//bool result = num > 1f;
  //      num = this.parentScreen.CurveS.Evaluate(Mathf.Clamp(num, 0f, 1f));
  //      leader.transform.position = this.leaderDestination * num + this.leaderStart * (1f - num);
	    return true;//result;
	}

	public override void OnExit()
	{
	}
}