using System;

public class CrewStateAgentPopup : BaseCrewState
{
	private bool isFinished;

	public CrewStateAgentPopup(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateAgentPopup(NarrativeSceneStateConfiguration config) : base(config)
	{
	}

	public override void OnEnter()
	{
		PopUpManager.Instance.TryShowPopUp(this.CreateAgentPopUp(), PopUpManager.ePriority.Default, null);
	}

	public override bool OnMain()
	{
		base.OnMain();
		return this.isFinished;
	}

	public override void OnExit()
	{
	}

	public PopUp CreateAgentPopUp()
	{
		return new PopUp
		{
			Title = "TEXT_EMPTY_STRING",
			BodyText = "TEXT_POPUPS_ENDGAME_01",
			IsBig = true,
			ConfirmAction = new PopUpButtonAction(this.OnOK),
			ConfirmText = "TEXT_BUTTON_OK",
			GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
			ImageCaption = "TEXT_NAME_AGENT",
			ShouldCoverNavBar = true
		};
	}

	private void OnOK()
	{
		this.isFinished = true;
	}
}
