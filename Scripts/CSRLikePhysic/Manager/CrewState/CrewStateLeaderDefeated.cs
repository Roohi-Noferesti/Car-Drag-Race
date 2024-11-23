using System;
using UnityEngine;

public class CrewStateLeaderDefeated : BaseCrewState
{
    private int crew;

    private GameObject defeatedGameObject;

	private float startScale;

	private float endScale;

	private float timeToScale;

	public CrewStateLeaderDefeated(CrewProgressionScreen zParentScreen, int zCrew, bool zSkipAnimation) : this(zParentScreen, zCrew)
	{
		if (zSkipAnimation)
		{
			this.timeToScale = 0.01f;
		}
	}

	public CrewStateLeaderDefeated(CrewProgressionScreen zParentScreen, int zCrew)
        : base(zParentScreen)
	{
		this.startScale = 1.6f;
		this.endScale = 1f;
		this.timeToScale = 0.3f;
        this.crew = zCrew;
    }

	public CrewStateLeaderDefeated(NarrativeSceneStateConfiguration config)
        : base(config)
	{
		this.startScale = 1.6f;
		this.endScale = 1f;
		this.timeToScale = 0.3f;
        this.crew = config.StateDetails.SlotIndex;
        this.timeToScale = config.StateDetails.TimeToScale;
		this.startScale = config.StateDetails.StartScale;
		this.endScale = config.StateDetails.EndScale;
	}

	private GameObject GetLeader()
	{
	    return this.parentScreen.charactersSlots[this.crew].GetMainCharacterGameObject();
    }

	public override void OnEnter()
	{
		GameObject original = Resources.Load("CharacterCards/Crew/BossCross") as GameObject;
		this.defeatedGameObject = (UnityEngine.Object.Instantiate(original) as GameObject);
	    this.defeatedGameObject.transform.SetParent(this.GetLeader().transform, false);
	    this.defeatedGameObject.transform.localPosition = Vector3.zero;
        AudioManager.Instance.PlaySound("CrewDefeat", null);
	}

	public override bool OnMain()
	{
		base.OnMain();
		float num = this.timeInState / this.timeToScale;
		bool result = num >= 1f;
		num = Mathf.Clamp(num, 0f, 1f);
		num = this.parentScreen.CurveLinear.Evaluate(num);
		float num2 = num * this.endScale + (1f - num) * this.startScale;
		float num3 = num2 * 1f;
		this.defeatedGameObject.transform.localScale = new Vector3(num3, num3, 1f);
		return result;
	}

	public override void OnExit()
	{
	}
}
