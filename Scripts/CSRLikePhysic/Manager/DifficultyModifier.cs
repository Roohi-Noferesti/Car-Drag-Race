public abstract class DifficultyModifier
{
	protected DifficultySettings Settings
	{
		get;
		private set;
	}

	public DifficultyModifier(DifficultySettings settings)
	{
		this.Settings = settings;
	}

	public virtual void OnMultiplayerFinishedRace(ref float difficulty, float raceTime, bool wonRace)
	{
	}

	public virtual void OnStreakFinished(ref float difficulty)
	{
	}

	public virtual void OnProfileChanged(ref float difficulty)
	{
	}

	public virtual void OnUpgradePurchased(ref float difficulty)
	{
	}

	public virtual void OnStreakStarted(ref float difficulty)
	{
	}
}
