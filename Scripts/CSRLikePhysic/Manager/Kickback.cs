using System;
using System.Collections.Generic;
using KingKodeStudio;

public class Kickback
{
	private static List<ScreenID> _unsafeIDCheckScreens = new List<ScreenID>
	{
		ScreenID.RaceResults,
		ScreenID.RaceRewards,
		ScreenID.LevelUp,
		ScreenID.VSDummy
	};

	public static bool IsSafePlaceToKickback()
	{
		return !(SceneLoadManager.Instance == null) && SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend && !Kickback._unsafeIDCheckScreens.Contains(ScreenManager.Instance.CurrentScreen);
	}
}
