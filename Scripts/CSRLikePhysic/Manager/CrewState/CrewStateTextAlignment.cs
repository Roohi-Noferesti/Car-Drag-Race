using UnityEngine;
using System.Collections;

public class CrewStateTextAlignment : BaseCrewState
{
    private string AnimationName;
    private int crew;
    public CrewStateTextAlignment(CrewProgressionScreen zParentScreen,int crew,string animationName) : base(zParentScreen)
    {
        AnimationName = animationName;
        this.crew = crew;
    }

    public CrewStateTextAlignment(NarrativeSceneStateConfiguration config) : base(config)
    {
        crew = 0;
    }

    public override void OnEnter()
    {
        this.parentScreen.charactersSlots[crew].PlayTextAnimation(AnimationName);
    }

    public override bool OnMain()
    {
        return true;
    }
}
