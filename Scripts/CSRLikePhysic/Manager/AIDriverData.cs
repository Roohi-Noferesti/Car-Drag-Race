using System;
using I2.Loc;

[Serializable]
public class AIDriverData
{
	public string AIDriverDBKey;

	public float LaunchRPMVariation;

	public float TargetLaunchRPM;

	public float RPMFromPeakPowerAtGearChange;

	public float ReactionTime;

	public float FirstGearLimitChangeUpPercent;

	public string Name = string.Empty;

	public string NumberPlateString = string.Empty;

	public string GetDisplayName()
	{
        if (this.Name.StartsWith("TEXT_"))
        {
            return LocalizationManager.GetTranslation(this.Name);
        }
		return this.Name;
	}

	public override string ToString()
	{
		string text = string.Empty;
		text = text + "Driver DB key : " + this.AIDriverDBKey + "\n";
		text = text + "    Name:" + this.Name + "\n";
		string text2 = text;
		text = string.Concat(new object[]
		{
			text2,
			"    Launch RPM variation:",
			this.LaunchRPMVariation,
			"\n"
		});
		text2 = text;
		text = string.Concat(new object[]
		{
			text2,
			"    Target launch RPM:",
			this.TargetLaunchRPM,
			"\n"
		});
		text2 = text;
		text = string.Concat(new object[]
		{
			text2,
			"    RPM from peak power for gear change:",
			this.RPMFromPeakPowerAtGearChange,
			"\n"
		});
		text2 = text;
		text = string.Concat(new object[]
		{
			text2,
			"    Reaction Time:",
			this.ReactionTime,
			"\n"
		});
		text2 = text;
		text = string.Concat(new object[]
		{
			text2,
			"    First Gear Limit Change Up Percent:",
			this.FirstGearLimitChangeUpPercent,
			"\n"
		});
		return text + "    Number Plate String:" + this.NumberPlateString + "\n";
	}
}
