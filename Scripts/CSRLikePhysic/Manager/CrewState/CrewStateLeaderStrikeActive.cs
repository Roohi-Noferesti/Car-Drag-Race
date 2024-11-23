using UnityEngine;
using System.Collections;

public class CrewStateLeaderStrikeActive : BaseCrewState
{
    private int crew;
    public CrewStateLeaderStrikeActive(CrewProgressionScreen zParentScreen,int crewIndex) : base(zParentScreen)
    {
        crew = crewIndex;
    }

    public CrewStateLeaderStrikeActive(NarrativeSceneStateConfiguration config) : base(config)
    {
        crew = config.StateDetails.SlotIndex;
    }

    public override void OnEnter()
    {
        parentScreen.charactersSlots[crew].GetMainCharacterGraphic().SetStrikeFramesActive();
    }

    public override bool OnMain()
    {
        return true;
    }
}
