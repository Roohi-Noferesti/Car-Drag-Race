using System;

public class PurchaseUpgradeInput : DifficultyModifier
{
	private bool hasPurchasedUpgrade;

	private bool firstRace = true;

	public PurchaseUpgradeInput(DifficultySettings settings) : base(settings)
	{
	}

	public override void OnMultiplayerFinishedRace(ref float difficulty, float raceTime, bool wonRace)
	{
		if (this.firstRace)
		{
			this.firstRace = false;
			this.hasPurchasedUpgrade = false;
		}
	}

	public override void OnStreakFinished(ref float difficulty)
	{
		this.firstRace = true;
	}

	public override void OnUpgradePurchased(ref float difficulty)
	{
		if (!this.hasPurchasedUpgrade)
		{
			this.hasPurchasedUpgrade = true;
			difficulty += base.Settings.UpgradePurchaseDelta;
		}
	}
}
