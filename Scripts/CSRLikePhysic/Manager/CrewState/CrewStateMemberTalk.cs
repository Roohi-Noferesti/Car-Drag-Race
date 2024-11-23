using System;
using UnityEngine;

public class CrewStateMemberTalk : BaseCrewState
{
    private const float timeForState = 3.5f;

    private string text;

    private bool hasMessage = true;

    private int member;

    private float StayTime = 1;

    private int crew;

    private bool thisIsSecondRaceOf3thMember;


    public CrewStateMemberTalk(CrewProgressionScreen zParentScreen, int crew, int zMemberToLeave, string zText)
        : base(zParentScreen)
    {
        this.text = zText;
        this.member = zMemberToLeave;
        this.crew = crew;
        thisIsSecondRaceOf3thMember = member == 3;
    }

    public CrewStateMemberTalk(NarrativeSceneStateConfiguration config)
        : base(config)
    {
        crew = 0;
        this.hasMessage = config.StateDetails.HasMessage;
        this.text = ((!this.hasMessage) ? string.Empty : config.StateDetails.GetTranslatedMessage(this.gameState));
        thisIsSecondRaceOf3thMember = member == 3;
    }

    public override void OnEnter()
    {
        var index = thisIsSecondRaceOf3thMember ? member - 1 : member;
        this.parentScreen.charactersSlots[crew].PlayAnimation(index, "Crew_BigSize");
        this.parentScreen.charactersSlots[crew].PlayBossAnimation("Crew_Exit");
        this.parentScreen.charactersSlots[crew].SetChatText(this.text);
        this.parentScreen.charactersSlots[crew].PlayTextAnimation("disable");
        timeInState = 0;

        for (int i = 0; i < 4; i++)
        {
            var thisIsForthMemberRace = i==2 && member == 3;
            if (i != this.member && !thisIsForthMemberRace)
            {
                this.parentScreen.charactersSlots[crew].PlayAnimation(i,"Crew_Exit");
            }
        }
    }

    public override bool OnMain()
    {
        base.OnMain();
        var index = thisIsSecondRaceOf3thMember ? member - 1 : member;
        return !this.parentScreen.charactersSlots[crew].IsPlayingAnimation(index, "Crew_BigSize")
               && timeInState > StayTime;
    }

    public override void OnExit()
    {
    }
}
