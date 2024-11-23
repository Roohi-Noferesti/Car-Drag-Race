using System;
using UnityEngine;

public static class ResolutionManager
{
	private static float OneToOnePixelDensity;

	private static float MinOneToOneWidth;

	private static float MinOneToOneHeight;

	private static float MaxOneToOneHeight;

	public static float PixelDensity
	{
		get;
		private set;
	}

	static ResolutionManager()
	{
		ResolutionManager.OneToOnePixelDensity = 200f;
		ResolutionManager.MinOneToOneWidth = 960f;
		ResolutionManager.MinOneToOneHeight = 640f;
		ResolutionManager.MaxOneToOneHeight = 800f;
		ResolutionManager.RecalculatePixelDensity();
	}

	public static void RecalculatePixelDensity()
	{
		ResolutionManager.PixelDensity = ResolutionManager.OneToOnePixelDensity;
		float num = (float)BaseDevice.ActiveDevice.GetScreenWidth();
		float num2 = (float)BaseDevice.ActiveDevice.GetScreenHeight();
		if (!ResolutionManager.CheckAndApplyCertainDevicesFix())
		{
			if (num < ResolutionManager.MinOneToOneWidth || num2 < ResolutionManager.MinOneToOneHeight)
			{
				float a = num / ResolutionManager.MinOneToOneWidth * ResolutionManager.OneToOnePixelDensity;
				float b = num2 / ResolutionManager.MinOneToOneHeight * ResolutionManager.OneToOnePixelDensity;
				ResolutionManager.PixelDensity = Mathf.Min(a, b);
			}
			else if (num2 > ResolutionManager.MaxOneToOneHeight)
			{
				ResolutionManager.PixelDensity = num2 / ResolutionManager.MaxOneToOneHeight * ResolutionManager.OneToOnePixelDensity;
			}
		}
	}

	public static bool CheckAndApplyCertainDevicesFix()
	{
		if (GTPlatform.Runtime == GTPlatforms.iOS)
		{
			if (BaseDevice.ActiveDevice.GetScreenWidth() == 2048 && BaseDevice.ActiveDevice.GetScreenHeight() == 1536)
			{
				ResolutionManager.PixelDensity *= 2f;
				return true;
			}
			if (BaseDevice.ActiveDevice.GetScreenWidth() == 1920 && BaseDevice.ActiveDevice.GetScreenHeight() == 1080)
			{
				ResolutionManager.PixelDensity *= 1.5f;
				return true;
			}
		}
		return false;
	}

	public static int WorldSpaceSizeToPixelSize(float val)
	{
		return (int)(val * ResolutionManager.PixelDensity);
	}

	public static float PixelSizeToWorldSpaceSize(int val)
	{
		return (float)val / ResolutionManager.PixelDensity;
	}
}
