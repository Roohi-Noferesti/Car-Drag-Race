using System;
using UnityEngine;
using UnityEngine.UI;

public class CrewStateMarkMemberAsDefeated : BaseCrewState
{
    private int crew;

    private int member;

	private GameObject defeatedGameObject;

	private float startScale = 1.6f;

	private float endScale = 1f;

	private float timeToScale = 0.3f;

    private bool thisIsFirstRaceOf3thMember;


    public CrewStateMarkMemberAsDefeated(CrewProgressionScreen zParentScreen, int zCrew, int zMember) : base(zParentScreen)
	{
        this.crew = zCrew;
        this.member = zMember;
        thisIsFirstRaceOf3thMember = member == 2;
    }

    public CrewStateMarkMemberAsDefeated(NarrativeSceneStateConfiguration config) : base(config)
	{
        this.crew = config.StateDetails.SlotIndex;
        this.member = config.StateDetails.MemberIndex;
		this.timeToScale = config.StateDetails.TimeToScale;
		this.startScale = config.StateDetails.StartScale;
		this.endScale = config.StateDetails.EndScale;
        thisIsFirstRaceOf3thMember = member == 2;

    }

	private RawImage GetMember()
    {
        var index = member;
        var thisIsSecondRaceOf3thMember = member == 3;
        if (thisIsSecondRaceOf3thMember)
        {
            index = member - 1;
        }
        return this.parentScreen.charactersSlots[crew].CharactersSprites[index];
	}

	private GameObject GetLeader()
	{
		return this.parentScreen.charactersSlots[crew].GetMainCharacterGameObject();
	}

	public override void OnEnter()
    {

        if (!thisIsFirstRaceOf3thMember)
        {
            GameObject original = Resources.Load("CharacterCards/Crew/CrewCross") as GameObject;
            this.defeatedGameObject = (UnityEngine.Object.Instantiate(original) as GameObject);
            this.defeatedGameObject.transform.SetParent(this.GetMember().transform.parent, false);
            this.defeatedGameObject.transform.localPosition = Vector3.zero;
        }

        
    }

	public override bool OnMain()
	{
		base.OnMain();
        //Do not scale cross object because its null
        if (thisIsFirstRaceOf3thMember)
            return true;
		float num = this.timeInState / this.timeToScale;
		bool result = num >= 1f;
		num = Mathf.Clamp(num, 0f, 1f);
		float num2 = num * this.endScale + (1f - num) * this.startScale;
		float num3 = num2 * 1f;
		this.defeatedGameObject.transform.localScale = new Vector3(num3, num3, 1f);
        //this.GetMember().gameObject.renderer.material.SetFloat("_Greyness", num);
		return result;
	}

	public override void OnExit()
	{
	}
}
