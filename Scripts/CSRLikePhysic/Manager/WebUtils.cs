using System;

public static class WebUtils
{
	public static bool IsUsingWWWClass()
	{
		return false;
	}

	public static string getSessionCookieName()
	{
		if (BasePlatform.ActivePlatform.GetDeviceType() == "unity")
		{
			return "unity_session";
		}
		return "session";
	}
}
