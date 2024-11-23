using System.Collections.Generic;

public static class CarQualitySettings
{
	public static CarShaderLod GarageLod
	{
		get;
		set;
	}

	public static CarShaderLod ShowroomLod
	{
		get;
		set;
	}

	public static CarShaderLod RaceLod
	{
		get;
		set;
	}

	public static void HighQuality()
	{
		GarageLod = CarShaderLod.FrontendHigh;
		ShowroomLod = CarShaderLod.FrontendHigh;
		RaceLod = CarShaderLod.RaceHigh;
	}

	public static void MedQuality()
	{
		GarageLod = CarShaderLod.FrontendMed;
		ShowroomLod = CarShaderLod.FrontendMed;
		RaceLod = CarShaderLod.RaceMed;
	}

	public static void LowQuality()
	{
		GarageLod = CarShaderLod.FrontendLow;
		ShowroomLod = CarShaderLod.FrontendLow;
		RaceLod = CarShaderLod.RaceLow;
	}

	public static bool RenderSnapshotsWithDownsampledAA()
	{
		List<string> list = new List<string>
		{
			"GT-P3110",
			"KFOT",
			"Nexus S"
		};
		string deviceModel = BasePlatform.ActivePlatform.GetDeviceModel();
		return !list.Contains(deviceModel);
	}
}
