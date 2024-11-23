using System;

public class CrewStateShowWorldTourThemeLoadingButton : BaseCrewState
{
	private string themeID = string.Empty;

	public CrewStateShowWorldTourThemeLoadingButton(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateShowWorldTourThemeLoadingButton(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.themeID = config.StateDetails.WorldTourThemeToLoadID;
	}

	public override void OnEnter()
	{
		this.parentScreen.ShowNextButton();
	}

	public override bool OnMain()
	{
		base.OnMain();
		return true;
	}

	public override void OnExit()
	{
		TierXManager.Instance.LoadTheme(this.themeID, null, null);
	}
}
