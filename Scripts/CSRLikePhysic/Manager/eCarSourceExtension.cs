using System;

public static class eCarSourceExtension
{
	public static bool IsSuperRare(this eCarSource source)
	{
		return source == eCarSource.Prize || source == eCarSource.IAPSpecial || source == eCarSource.High_Stakes;
	}

	public static bool IsIAP(this eCarSource source)
	{
		return source == eCarSource.IAP || source == eCarSource.IAPSpecial;
	}
}
