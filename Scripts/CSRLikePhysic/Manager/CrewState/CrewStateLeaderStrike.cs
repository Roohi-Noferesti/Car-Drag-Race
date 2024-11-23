using System;
using UnityEngine;

public class CrewStateLeaderStrike : BaseCrewState
{
	private int strike;

    private int crew;

    private GameObject defeatedGameObject;

	private float startScale;

	private float endScale;

	private float timeToScale;

	public CrewStateLeaderStrike(CrewProgressionScreen zParentScreen, int zCrew, int zStrike, bool zSkipAnimation) : this(zParentScreen, zCrew, zStrike)
	{
		if (zSkipAnimation)
		{
			this.timeToScale = 0.01f;
		}
	}

	public CrewStateLeaderStrike(CrewProgressionScreen zParentScreen, int zCrew, int zStrike)
        : base(zParentScreen)
	{
		this.startScale = 1.6f;
		this.endScale = 1f;
		this.timeToScale = 0.3f;
        this.crew = zCrew;
        this.strike = zStrike;
	}

	public CrewStateLeaderStrike(NarrativeSceneStateConfiguration config)
        : base(config)
	{
		this.startScale = 1.6f;
		this.endScale = 1f;
		this.timeToScale = 0.3f;
        this.crew = config.StateDetails.SlotIndex;
        this.strike = config.StateDetails.CrewLeaderStrikeTimes;
		this.timeToScale = config.StateDetails.TimeToScale;
		this.startScale = config.StateDetails.StartScale;
		this.endScale = config.StateDetails.EndScale;
	}

    public override void OnEnter()
    {
        //GameObject original = Resources.Load("CharacterCards/Crew/BossStrike") as GameObject;
        //this.defeatedGameObject = (UnityEngine.Object.Instantiate(original) as GameObject);
        var parent =
            this.parentScreen.charactersSlots[crew].GetMainCharacterGraphic().GetStrikeFrame(this.strike).transform;
        //this.defeatedGameObject.transform.SetParent(parent);
        //this.defeatedGameObject.transform.localPosition = Vector3.zero;
        this.defeatedGameObject = parent.GetChild(0).gameObject;
        this.defeatedGameObject.SetActive(true);
    }

    public override bool OnMain()
	{
		base.OnMain();
		float num = this.timeInState / this.timeToScale;
		bool result = num >= 1f;
		num = Mathf.Clamp(num, 0f, 1f);
		num = this.parentScreen.CurveLinear.Evaluate(num);
		float num2 = num * this.endScale + (1f - num) * this.startScale;
        this.defeatedGameObject.transform.localScale = new Vector3(num2, num2, 1f);
		return result;
	}

	public override void OnExit()
	{
	}
}
