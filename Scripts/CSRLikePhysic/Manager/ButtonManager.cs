using System.Collections.Generic;

public static class ButtonManager
{
	public enum CSRButtonType
	{
		Black,
		Blue,
		Dual,
		Green,
		Twitter,
		Back,
		Facebook,
		SmallFacebook,
		DebugScreenButton,
		Rumours_UNUSED,
		GreenCompact,
		BlackCompact,
		BlackTextCompact,
		FacebookInvite,
		GooglePlus,
		BoostListItem,
		GreenCompactGold,
		BlackTextTVIcon,
		Orange_TUNINGSCREEN_ONLY
	}

	public static Dictionary<CSRButtonType, string> map;

	static ButtonManager()
	{
		map = new Dictionary<CSRButtonType, string>
		{
			{
				CSRButtonType.Dual,
				"DualButton"
			},
			{
				CSRButtonType.Blue,
				"FancyBlueSmall"
			},
			{
				CSRButtonType.Black,
				"FancyBlackSmall"
			},
			{
				CSRButtonType.Green,
				"FancyGreenSmall"
			},
			{
				CSRButtonType.Twitter,
				"TwitterBlackSmall"
			},
			{
				CSRButtonType.Back,
				"BackTextButton"
			},
			{
				CSRButtonType.Facebook,
				"FacebookButton"
			},
			{
				CSRButtonType.SmallFacebook,
				"SmallFacebookButton"
			},
			{
				CSRButtonType.DebugScreenButton,
				"DebugBlackSmall"
			},
			{
				CSRButtonType.GreenCompact,
				"GreenCompact"
			},
			{
				CSRButtonType.BlackCompact,
				"BlackCompact"
			},
			{
				CSRButtonType.BlackTextCompact,
				"BlackTextCompact"
			},
			{
				CSRButtonType.FacebookInvite,
				"FacebookInviteButton"
			},
			{
				CSRButtonType.GooglePlus,
				"GooglePlusButton"
			},
			{
				CSRButtonType.BoostListItem,
				"BoostListItemButton"
			},
			{
				CSRButtonType.GreenCompactGold,
				"GreenCompactGold"
			},
			{
				CSRButtonType.BlackTextTVIcon,
				"BlackTextTVIcon"
			},
			{
				CSRButtonType.Orange_TUNINGSCREEN_ONLY,
				"FancyOrangeSmall"
			}
		};
	}

	public static string GetPrefabPath(CSRButtonType buttonType)
	{
		if (map.ContainsKey(buttonType))
		{
			return map[buttonType];
		}
		return string.Empty;
	}
}
