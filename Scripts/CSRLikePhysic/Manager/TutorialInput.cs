using System;

public class TutorialInput : DifficultyModifier
{
	public TutorialInput(DifficultySettings settings) : base(settings)
	{
	}

	public override void OnMultiplayerFinishedRace(ref float difficulty, float raceTime, bool wonRace)
	{
		this.OnProfileChanged(ref difficulty);
	}

	public override void OnStreakFinished(ref float difficulty)
	{
		this.OnProfileChanged(ref difficulty);
	}

	public override void OnUpgradePurchased(ref float difficulty)
	{
		this.OnProfileChanged(ref difficulty);
	}

	public override void OnProfileChanged(ref float difficulty)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile != null && activeProfile.BestEverMultiplayerWinStreakBanked < 4)
		{
			difficulty = base.Settings.InitialDifficulty;
		}
	}
}
