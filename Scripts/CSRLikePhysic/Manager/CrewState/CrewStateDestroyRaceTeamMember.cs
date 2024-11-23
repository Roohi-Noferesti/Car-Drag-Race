using System;
using UnityEngine;

public class CrewStateDestroyRaceTeamMember : BaseCrewState
{
	public CrewStateDestroyRaceTeamMember(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateDestroyRaceTeamMember(NarrativeSceneStateConfiguration config) : base(config)
	{
	}

	public override void OnEnter()
	{
		Transform transform = this.parentScreen.GetContainer().Find("MysteryDonor");
		UnityEngine.Object.Destroy(transform.gameObject);
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
