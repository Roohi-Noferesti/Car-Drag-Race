using System;
using UnityEngine;

public class CrewStateLeaderCrossGrayOut : BaseCrewState
{
    private int crew;

    private float timeToScale;

	public CrewStateLeaderCrossGrayOut(CrewProgressionScreen zParentScreen, int zCrew, float zTimeScale) : this(zParentScreen, zCrew)
	{
		this.timeToScale = zTimeScale;
	}

	public CrewStateLeaderCrossGrayOut(CrewProgressionScreen zParentScreen, int zCrew)
        : base(zParentScreen)
	{
		this.timeToScale = 2.5f;
        this.crew = zCrew;
    }

	public CrewStateLeaderCrossGrayOut(NarrativeSceneStateConfiguration config)
        : base(config)
	{
		this.timeToScale = 2.5f;
        this.crew = config.StateDetails.SlotIndex;
        this.timeToScale = config.StateDetails.TimeToScale;
	}

	private GameObject GetLeaderCrossGraphic()
	{
	    return this.parentScreen.charactersSlots[this.crew].GetMainCharacterGameObject().transform.Find("BossCross(Clone)").GetChild(0).gameObject;
	}

	public override void OnEnter()
	{
        AudioManager.Instance.PlaySound("CrewDesaturate", null);
	}

	public override bool OnMain()
	{
		base.OnMain();
		float num = this.timeInState / this.timeToScale;
		bool result = num >= 1f;
		num = Mathf.Clamp(num, 0f, 1f);
		num = this.parentScreen.CurveLinear.Evaluate(num);
        //this.GetLeaderCrossGraphic().renderer.material.SetFloat("_Greyness", num);
        return result;
	}

	public override void OnExit()
	{
	}
}
