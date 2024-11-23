using System;
using UnityEngine;

public class ConsumableExpiredInput : DifficultyModifier
{
	private int numConsumablesActiveAtLastStreakEnd;

	public ConsumableExpiredInput(DifficultySettings settings) : base(settings)
	{
	}

	public override void OnStreakFinished(ref float difficulty)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		this.numConsumablesActiveAtLastStreakEnd = activeProfile.NumTimedConsumablesActive();
	}

	public override void OnStreakStarted(ref float difficulty)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int num = activeProfile.NumTimedConsumablesActive();
		int num2 = Mathf.Max(0, this.numConsumablesActiveAtLastStreakEnd - num);
		difficulty += (float)num2 * base.Settings.ConsumableLossDelta;
		this.numConsumablesActiveAtLastStreakEnd = num;
	}
}
