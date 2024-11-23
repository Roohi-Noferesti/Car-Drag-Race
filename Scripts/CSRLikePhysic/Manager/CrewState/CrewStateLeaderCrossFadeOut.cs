using System;
using UnityEngine;
using UnityEngine.UI;

public class CrewStateLeaderCrossFadeOut : BaseCrewState
{
    private int crew;

    private float timeToScale;

	public CrewStateLeaderCrossFadeOut(CrewProgressionScreen zParentScreen, int zCrew, float zTimeScale) : this(zParentScreen, zCrew)
	{
		this.timeToScale = zTimeScale;
	}

	public CrewStateLeaderCrossFadeOut(CrewProgressionScreen zParentScreen, int zCrew)
        : base(zParentScreen)
	{
		this.timeToScale = 2.5f;
        this.crew = zCrew;
    }

	public CrewStateLeaderCrossFadeOut(NarrativeSceneStateConfiguration config)
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
	}

	public override bool OnMain()
	{
	    base.OnMain();
	    float num = this.timeInState / this.timeToScale;
	    bool result = num >= 1f;
	    num = Mathf.Clamp(num, 0f, 1f);
	    float a = 1f - num;
	    this.GetLeaderCrossGraphic().GetComponent<RawImage>().color = new Color(1f, 1f, 1f, a);
	    return result;
    }

	public override void OnExit()
	{
	}
}
