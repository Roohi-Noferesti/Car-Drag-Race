using System;
using DataSerialization;

public static class EligibilityConditionDetailsExtensions
{
	public static void Initialise(this EligibilityConditionDetails ecd)
	{
		InitialiseEnumValue(ref ecd.MatchRequirment, ref ecd.MatchRequirmentEnum);
		if (!string.IsNullOrEmpty(ecd.TimeDifference))
		{
			if (!TimeSpan.TryParse(ecd.TimeDifference, out ecd.TimeSpanDifference))
			{
			}
            //ecd.TimeDifference = null;
		}
		if (!string.IsNullOrEmpty(ecd.MinTime))
		{
			if (!DateTime.TryParse(ecd.MinTime, out ecd.MinDateTime))
			{
			}
            //ecd.MinTime = null;
		}
		if (!string.IsNullOrEmpty(ecd.MaxTime))
		{
			if (!DateTime.TryParse(ecd.MaxTime, out ecd.MaxDateTime))
			{
			}
            //ecd.MaxTime = null;
		}
	}

	private static void InitialiseEnumValue<TEnum>(ref string stringValue, ref TEnum enumValue) where TEnum : struct, IConvertible, IComparable, IFormattable
	{
		if (typeof(TEnum).IsEnum && !string.IsNullOrEmpty(stringValue))
		{
			enumValue = EnumHelper.FromString<TEnum>(stringValue);
            //stringValue = string.Empty;
		}
	}
}
