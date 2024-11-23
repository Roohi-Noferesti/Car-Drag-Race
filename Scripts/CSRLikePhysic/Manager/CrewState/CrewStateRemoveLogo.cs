using System;

public class CrewStateRemoveLogo : BaseCrewState
{
	public CrewStateRemoveLogo(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateRemoveLogo(NarrativeSceneStateConfiguration config) : base(config)
	{
	}

	public override void OnEnter()
	{
		this.parentScreen.charactersSlots[4].LogoParent.gameObject.SetActive(false);
	}

	public override bool OnMain()
	{
		base.OnMain();
		return true;
	}

	public override void OnExit()
	{
	}
}
