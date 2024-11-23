using System;
using System.Collections.Generic;
using UnityEngine;

public class GTPlatform
{
	private class URLS
	{
		public string Update;

		public string Support;

		public string Problem;

		public string Legal;

		public string Suggestion;

        public string Telegram;

	    public string Instagram;

        public string Whatsapp;

        //public string Classics;
    }

    private static Dictionary<RuntimePlatform, GTPlatforms> map = new Dictionary<RuntimePlatform, GTPlatforms>
	{
		{
			RuntimePlatform.IPhonePlayer,
			GTPlatforms.iOS
		},
		{
			RuntimePlatform.Android,
			GTPlatforms.ANDROID
		},
		{
			RuntimePlatform.WindowsEditor,
			GTPlatforms.WINDOWS
		},
		{
			RuntimePlatform.WindowsPlayer,
			GTPlatforms.WINDOWS
		},
		{
			RuntimePlatform.OSXEditor,
			GTPlatforms.OSX
		},
		{
			RuntimePlatform.OSXPlayer,
			GTPlatforms.OSX
		},
		{
			RuntimePlatform.WSAPlayerARM,
			GTPlatforms.METRO
		},
		{
			RuntimePlatform.WSAPlayerX86,
			GTPlatforms.METRO
		},
		{
			RuntimePlatform.WSAPlayerX64,
			GTPlatforms.METRO
		}
	};

	private static List<RuntimePlatform> editorPlatforms = new List<RuntimePlatform>
	{
		RuntimePlatform.OSXEditor,
		RuntimePlatform.WindowsEditor
	};

	private static readonly Dictionary<GTAppStore, URLS> appStoreToURLs = new Dictionary<GTAppStore, URLS>
	{
		{
			GTAppStore.iOS,
			new URLS
			{
				Update = "https://apps.apple.com/app/id1458220954",
				Support = "https://t.me/GTclubAdmin",
				Problem = "https://apps.apple.com/app/id1458220954",
				Legal = "https://apps.apple.com/app/id1458220954",
				Suggestion = "https://apps.apple.com/app/id1458220954",
                Instagram = "https://www.instagram.com/gt.club.game/",
                Telegram = "https://t.me/gt_club_game",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"
            }
		},
		{
			GTAppStore.OSX,
			new URLS
			{
				Update = "https://apps.apple.com/app/id1458220954",
				Support = "https://t.me/GTclubAdmin",
				Problem = "https://apps.apple.com/app/id1458220954",
				Legal = "https://apps.apple.com/app/id1458220954",
				Suggestion = "https://apps.apple.com/app/id1458220954",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"

            }
        },
		{
			GTAppStore.GooglePlay,
			new URLS
			{
				Update = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
				Support = "https://t.me/GTclubAdmin",
				Problem = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
				Legal = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
				Suggestion = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
                Instagram = "https://www.instagram.com/gt.club.game/",
                Telegram = "https://t.me/gt_club_game",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"

            }
        },
        {
            GTAppStore.UDP,
            new URLS
            {
                Update = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
                Support = "https://t.me/GTclubAdmin",
                Problem = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
                Legal = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
                Suggestion = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
                Instagram = "https://www.instagram.com/gt.club.game/",
                Telegram = "https://t.me/gt_club_game",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"

            }
        },
        {
            GTAppStore.Zarinpal,
            new URLS
            {
                Update = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
                Support = "https://t.me/GTclubAdmin",
                Problem = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
                Legal = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
                Suggestion = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
                Instagram = "https://www.instagram.com/gt.club.game/",
                Telegram = "https://t.me/gt_club_game",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"

            }
        },
#if UNITY_EDITOR
        {
			GTAppStore.Amazon,
			new URLS
			{
				Update = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
				Support = "https://t.me/GTclubAdmin",
				Problem = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
				Legal = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
				Suggestion = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"

            }
        },
		{
			GTAppStore.Windows,
			new URLS
			{
				Update = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
				Support = "https://t.me/GTclubAdmin",
				Problem = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
				Legal = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
				Suggestion = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"

            }
        },
		{
			GTAppStore.Windows_Metro,
			new URLS
			{
				Update = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
				Support = "https://t.me/GTclubAdmin",
				Problem = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
				Legal = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
				Suggestion = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"

            }
        },
#endif
		
#if UNITY_ANDROID
	    {
	        GTAppStore.Iraqapps,
	        new URLS
	        {
	            Update = "http://iranapps.ir/app/com.kingkodestudio.z2h",
	            Support = "https://t.me/GTclubAdmin",
	            Problem = "http://iranapps.ir/app/com.kingkodestudio.z2h",
	            Legal = "http://iranapps.ir/app/com.kingkodestudio.z2h",
	            Suggestion = "http://iranapps.ir/app/com.kingkodestudio.z2h",
	            Instagram = "https://www.instagram.com/gt.club.game/",
	            Telegram = "https://t.me/gt_club_game",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"

            }
        },
	    {
	        GTAppStore.Myket,
	        new URLS
	        {
	            Update = "https://myket.ir/app/com.kingkodestudio.z2h",
	            Support = "https://t.me/GTclubAdmin",
	            Problem = "https://myket.ir/app/com.kingkodestudio.z2h",
	            Legal = "https://myket.ir/app/com.kingkodestudio.z2h",
	            Suggestion = "https://myket.ir/app/com.kingkodestudio.z2h",
	            Instagram = "https://www.instagram.com/gt.club.game/",
	            Telegram = "https://t.me/gt_club_game",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"

            }
        },
	    {
	        GTAppStore.Bazaar,
	        new URLS
	        {
	            Update = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
	            Support = "https://t.me/GTclubAdmin",
	            Problem = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
	            Legal = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
	            Suggestion = "https://cafebazaar.ir/app/com.kingkodestudio.z2h/?l=fa",
	            Instagram = "https://www.instagram.com/gt.club.game/",
	            Telegram = "https://t.me/gt_club_game",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"

            }
        },
#endif
	    {
	        GTAppStore.None,
	        new URLS
	        {
	            Update = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
	            Support = "https://t.me/GTclubAdmin",
	            Problem = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
	            Legal = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
	            Suggestion = "https://play.google.com/store/apps/details?id=com.kingkodestudio.z2h",
	            Instagram = "https://www.instagram.com/gtclubgame/",
	            Telegram = "https://t.me/gt_club_game",
                Whatsapp = "https://api.whatsapp.com/send?phone=+989056982357"
            }
        },
    };

	public static GTPlatforms Target
	{
		get
		{
#if UNITY_ANDROID
			return GTPlatforms.ANDROID;
#elif UNITY_IOS
			return GTPlatforms.iOS;
#endif
		}
	}

	public static GTPlatforms Runtime
	{
		get
		{
			RuntimePlatform platform = Application.platform;
			if (!map.ContainsKey(platform))
			{
				return GTPlatforms.OSX;
			}
			return map[platform];
		}
	}

	public static bool IsEditor
	{
		get
		{
			RuntimePlatform platform = Application.platform;
			return editorPlatforms.Contains(platform);
		}
	}

	public static string RuntimeName
	{
		get
		{
			return Runtime.ToString();
		}
	}

	public static GTPlatforms FromString(string name)
	{
		return (GTPlatforms)((int)Enum.Parse(typeof(GTPlatforms), name));
	}

	public static string GetPlatformUpdateURL()
    {
        return "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link";
        //return appStoreToURLs[BasePlatform.ActivePlatform.GetTargetAppStore()].Update;
    }

	public static string GetPlatformSupportURL()
	{
        return appStoreToURLs[BasePlatform.ActivePlatform.GetTargetAppStore()].Support;
	}

	public static string GetPlatformProblemURL()
	{
		return appStoreToURLs[BasePlatform.ActivePlatform.GetTargetAppStore()].Problem;
	}

	public static string GetPlatformLegalURL()
	{
		return appStoreToURLs[BasePlatform.ActivePlatform.GetTargetAppStore()].Legal;
	}

	public static string GetPlatformSuggestionURL()
	{
		return appStoreToURLs[BasePlatform.ActivePlatform.GetTargetAppStore()].Suggestion;
	}

    public static string GetPlatformTelegramURL()
    {
        return appStoreToURLs[BasePlatform.ActivePlatform.GetTargetAppStore()].Telegram;
    }

    public static string GetPlatformWhatsappURL()
    {
        return appStoreToURLs[BasePlatform.ActivePlatform.GetTargetAppStore()].Whatsapp;
    }

    public static string GetPlatformInstagramURL()
    {
#if UNITY_EDITOR
			return appStoreToURLs[BasePlatform.ActivePlatform.GetTargetAppStore()].Instagram;
#elif UNITY_ANDROID
			if (BasePlatform.ActivePlatform.InsideCountry)
				return appStoreToURLs[BasePlatform.ActivePlatform.GetTargetAppStore()].Instagram;
			else
			{
				return "https://www.instagram.com/gtclubgame/";
			}
#elif UNITY_IOS
			if (BasePlatform.ActivePlatform.InsideCountry)
				return appStoreToURLs[BasePlatform.ActivePlatform.GetTargetAppStore()].Instagram;
			else
			{
				return "https://www.instagram.com/gtclubgame/";
			}
#endif
    }

    public static string GetPlatformGTClassicsURL()
	{
	    return null;//appStoreToURLs[BasePlatform.ActivePlatform.GetTargetAppStore()].Classics ?? string.Empty;
	}
}
