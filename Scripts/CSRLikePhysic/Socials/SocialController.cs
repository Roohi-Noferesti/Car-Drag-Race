using Metrics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;

public class SocialController : MonoBehaviour
{
	public enum MessageType
	{
		LEVEL_UP,
		NEW_CAR,
		UNLOCK_TIER,
		WIN_RACE,
		INVITE,
		SNAPSHOT,
		WIN_CAR,
		UNLOCK_FRIENDS_TIER,
		FRIENDS_WIN_CAR,
		FRIENDS_WIN_GOLD,
		FRIENDS_ONE_STAR,
		FRIENDS_TWO_STARS,
		FRIENDS_THREE_STARS,
		FRIENDS_BEAT_FRIENDS,
		FRIENDS_BEAT_NEXT_FRIEND,
		FRIENDS_PERSONAL_BEST
	}

	private struct Shared
	{
		public DateTime mTwitter;
	}

	private const int profilePicCachedFilesLimit = 100;

	public string FriendsPicsPath;

	public bool isLoggedIntoFacebook;

	public bool attemptingFacebookStreamPost;

	public bool attemptingFacebookLike;

	public string FacebookStreamName;

	public string FacebookStreamCaption;

	public string FacebookStreamDescription;

	public string FacebookStreamIcon;

	public string BossImageBase = "http://185.81.99.186/images/gt/boss_tier{0}.jpg";

	private FacebookPermissions facebookPermissions;

	public static bool TwitterIsDisabled;

	public static bool FacebookIsDisabled;

	public GameObject TwitterSendWaiter;

	public GameObject FacebookSendWaiter;

	public float[] NaggableMilestones = new float[]
	{
		8f,
		9f,
		10f,
		12f
	};

	public string[] NaggableHashtags = new string[]
	{
		"#8sec",
		"#9sec",
		"#10sec",
		"#12sec"
	};

	public Dictionary<int, string> FBStarLocText = new Dictionary<int, string>
	{
		{
			1,
			"TEXT_FACEBOOK_FRIENDSGETSTAR_NAME"
		},
		{
			2,
			"TEXT_FACEBOOK_FRIENDSGETSTAR2_NAME"
		},
		{
			3,
			"TEXT_FACEBOOK_FRIENDSGETSTAR3_NAME"
		}
	};

	public Dictionary<int, string> TwStarLocText = new Dictionary<int, string>
	{
		{
			1,
			"TEXT_TWITTER_FRIENDSGETSTAR_NAME"
		},
		{
			2,
			"TEXT_TWITTER_FRIENDSGETSTAR2_NAME"
		},
		{
			3,
			"TEXT_TWITTER_FRIENDSGETSTAR3_NAME"
		}
	};

	public string facebookName = string.Empty;

	public string facebookFirstName = string.Empty;

	public string facebookLastName = string.Empty;

	public string facebookEmail = string.Empty;

	public string facebookPictureURL = string.Empty;

	public bool facebookPictureIsSilhouette;

	public GamecenterAvatarLoader GamecenterAvatarLoader = new GamecenterAvatarLoader();

	public FacebookAvatarLoader FacebookAvatarLoader = new FacebookAvatarLoader();

	public GooglePlayAvatarLoader GooglePlayAvatarLoader = new GooglePlayAvatarLoader();

	private string facebookID = string.Empty;

	private string facebookTokenForBusiness = string.Empty;

	private int facebookFriendLoadLimit = 100;

	private string metricsSocialEvent = "NotSet";

	public Action OnGivenSocialReward;

	private PopUp hiddenPopup;

	private bool waitingForLoginResponse;

	private Dictionary<GTAppStore, string> _appStoreToLinkTwitter = new Dictionary<GTAppStore, string>
	{
		{
			GTAppStore.iOS,
            "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link&utm_referralid=[user_id]"
        },
        {
	        GTAppStore.GooglePlay,
            "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link&utm_referralid=[user_id]"
        },
        {
            GTAppStore.UDP,
            "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link&utm_referralid=[user_id]"
        },
#if UNITY_ANDROID
        {
            GTAppStore.Zarinpal,
            "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link&utm_referralid=[user_id]"
        },
        {
	        GTAppStore.Bazaar,
            "https://cafebazaar.ir/app/com.kingkodestudio.z2h"//Due to Bazaar warning , we have to put bazaar link
        },
	    {
	        GTAppStore.Iraqapps,
            "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link&utm_referralid=[user_id]"
        },
	    {
	        GTAppStore.Myket,
            "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link&utm_referralid=[user_id]"
        },
#endif
	    {
	        GTAppStore.None,
	        "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link&utm_referralid=[user_id]"
	    },
        {
			GTAppStore.Amazon,
            "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link&utm_referralid=[user_id]"
        },
		{
			GTAppStore.OSX,
            "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link&utm_referralid=[user_id]"
        },
		{
			GTAppStore.Windows,
            "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link&utm_referralid=[user_id]"
        },
		{
			GTAppStore.Windows_Metro,
            "https://api.kingcodestudio.com/api/ad/track?utm_source=share_link&utm_referralid=[user_id]"
        }
	};


    private Dictionary<GTAppStore, string> _appStoreToLinkInvite = new Dictionary<GTAppStore, string>
    {
        {
            GTAppStore.iOS,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid="
        },
        {
            GTAppStore.GooglePlay,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid=0"
        },
        {
            GTAppStore.UDP,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid=0"
        },
        {
            GTAppStore.Amazon,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid="
        },
        {
            GTAppStore.OSX,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid="
        },
        {
            GTAppStore.Windows,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid="
        },
        {
            GTAppStore.Windows_Metro,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid="
        }
    };

    private Dictionary<GTAppStore, string> _appStoreToLinkFacebook = new Dictionary<GTAppStore, string>
	{
		{
			GTAppStore.iOS,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid="
        },
		{
			GTAppStore.GooglePlay,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid="
        },
		{
			GTAppStore.Amazon,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid="
        },
		{
			GTAppStore.OSX,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid="
        },
		{
			GTAppStore.Windows,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid="
        },
		{
			GTAppStore.Windows_Metro,
            "https://api.kingcodestudio.com/api/Ad/website_ad/1?advertid=invite_link&referralid="
        }
	};

	private PopUp MaintainPopUp;

	private SocialController.MessageType messageType;

	private string messageDetail;

	private Dictionary<string, SocialController.Shared> RecentlyTweeted = new Dictionary<string, SocialController.Shared>();

	private TimeSpan twitterRepostTimeOut = new TimeSpan(1, 0, 0);

    public event SocialController_BoolDelegate DoesTwitterUserFollowUsEvent;

	public static SocialController Instance
	{
		get;
		private set;
	}

	public string FacebookImage
	{
		get
		{
			return "http://download.kingcodestudio.com/images/gt/gt_icon256.png?v=" + ApplicationVersion.Current;
		}
	}

	public FacebookPermissions FacebookPermissions
	{
		get
		{
			return this.facebookPermissions ?? new FacebookPermissions();
		}
	}

	public bool TwitterIsDisabledForPosting
	{
		get
		{
			return this.RecentlySharedTwitter(this.MakeMessageKey()) || SocialController.TwitterIsDisabled;
		}
	}

	public bool FacebookIsDisabledForPosting
	{
		get
		{
			return SocialController.FacebookIsDisabled;
		}
	}

	public bool RecievedFBUserInfo
	{
		get;
		private set;
	}

	public SocialTriggers CurrentSocialNag
	{
		get;
		private set;
	}

	public string CurrentSocialNagValue
	{
		get;
		private set;
	}

    private void ReInstateUI()
    {
        if (this.hiddenPopup != null)
        {
            PopUpManager.Instance.TryShowPopUp(this.hiddenPopup, PopUpManager.ePriority.Default, null);
        }
        var zScreen = ScreenManager.Instance.ActiveScreen as ZHUDScreen;
        switch (this.messageType)
        {
            case SocialController.MessageType.LEVEL_UP:
            {
                LevelUpScreen levelUpScreen = zScreen as LevelUpScreen;
                if (levelUpScreen != null)
                {
                    levelUpScreen.SetShareButtonString(false);
                    levelUpScreen.FinishAnimation();
                }
                break;
            }
            case SocialController.MessageType.UNLOCK_TIER:
            {
                //TierUnlockedScreen tierUnlockedScreen = zScreen as TierUnlockedScreen;
                //if (tierUnlockedScreen != null)
                //{
                //    tierUnlockedScreen.SetShareButtonString(false);
                //}
                break;
            }
            case SocialController.MessageType.WIN_CAR:
            {
                PrizePieceGiveScreen prizePieceGiveScreen = zScreen as PrizePieceGiveScreen;
                if (prizePieceGiveScreen != null)
                {
                    prizePieceGiveScreen.SetShareButtonString(false);
                }
                break;
            }
            case SocialController.MessageType.UNLOCK_FRIENDS_TIER:
            case SocialController.MessageType.FRIENDS_WIN_CAR:
            case SocialController.MessageType.FRIENDS_WIN_GOLD:
            case SocialController.MessageType.FRIENDS_ONE_STAR:
            case SocialController.MessageType.FRIENDS_TWO_STARS:
            case SocialController.MessageType.FRIENDS_THREE_STARS:
            case SocialController.MessageType.FRIENDS_BEAT_FRIENDS:
            case SocialController.MessageType.FRIENDS_BEAT_NEXT_FRIEND:
            case SocialController.MessageType.FRIENDS_PERSONAL_BEST:
            {
                //FriendRewardScreen friendRewardScreen = zScreen as FriendRewardScreen;
                //if (friendRewardScreen != null)
                //{
                //    friendRewardScreen.SetShareButtonString(false);
                //}
                break;
            }
        }
    }

    private void Awake()
	{
		this.FriendsPicsPath = Path.Combine(FileUtils.temporaryCachePath, "FriendsPics");
		if (SocialController.Instance != null)
		{
			return;
		}
		SocialController.Instance = this;
		this.CurrentSocialNag = SocialTriggers.None;
		this.CurrentSocialNagValue = string.Empty;
		NativeEvents.fbDidLoginEvent += new NativeEvents_DelegateToken(this.fbDidLogin);
		NativeEvents.fbDidLogoutEvent += new NativeEvents_Delegate(this.fbDidLogout);
		NativeEvents.fbDidNotLoginEvent += new NativeEvents_DelegateInt(this.fbDidNotLogin);
		NativeEvents.fbGotUserInfoEvent += new NativeEvents_DelegateString(this.fbGotUserInfo);
		NativeEvents.fbGotTokenForBusinessEvent += new NativeEvents_DelegateString(this.fbGotTokenForBusiness);
		NativeEvents.fbGotUserPermissionsEvent += new NativeEvents_DelegateString(this.fbGotUserPermissions);
		NativeEvents.fbAppRequestDialogueFailedEvent += new NativeEvents_DelegateInt(this.fbFailed);
		NativeEvents.fbPostDialogueFailedEvent += new NativeEvents_DelegateInt(this.fbFailed);
		NativeEvents.fbPostDialogueSucceededEvent += new NativeEvents_Delegate(this.fbPostSucceeded);
		NativeEvents.fbPostDialogueCancelledEvent += new NativeEvents_Delegate(this.fbPostCancelled);
		NativeEvents.fbInviteFriendsSucceededEvent += new NativeEvents_DelegateInt(this.fbInviteFriendsSucceeded);
		NativeEvents.fbRequestFriendsPermissionEvent += new NativeEvents_DelegateBool(this.fbRequestFriendsPermission);
		NativeEvents.fbRequestPermissionEvent += new NativeEvents_DelegateBool(this.fbRequestPermission);
		NativeEvents.fbRequestFailedEvent += new NativeEvents_DelegateInt(this.fbRequestFailed);
		NativeEvents.twitterUserIsFollowingUsEvent += new NativeEvents_DelegateBool(this.GotTwitterUserFollowsUsOnFacebook);
		this.isLoggedIntoFacebook = false;
		this.MaintainPopUp = null;
		this.ClearProfilePicCache();
		this.waitingForLoginResponse = false;
	}

	public string GetTwitterGameURL()
	{
	    var userid = UserManager.Instance.currentAccount.UserID;
		var shareLink =  this._appStoreToLinkTwitter[BasePlatform.ActivePlatform.GetTargetAppStore()];
	    return shareLink.Replace("[user_id]", userid.ToString());
	    //return string.Format(shareLink, userid);
	}

    public string GetInviteGameURL()
    {
        return this._appStoreToLinkInvite[BasePlatform.ActivePlatform.GetTargetAppStore()]+UserManager.Instance.currentAccount.UserID;
    }

    private string GetTwitterMessage(string textID)
	{
		string text = LocalizationManager.GetTranslation(textID);
		return text.Replace("{LINK}", this.GetTwitterGameURL());
	}


    private string GetInviteMessage(string textID)
    {
        string text = LocalizationManager.GetTranslation(textID);
        return text.Replace("{LINK}", this.GetInviteGameURL());
    }

    public string GetFacebookGameURL()
	{
		return this._appStoreToLinkFacebook[BasePlatform.ActivePlatform.GetTargetAppStore()];
	}

	private string GetFacebookMessage(string textID)
	{
		string text = LocalizationManager.GetTranslation(textID);
		return text.Replace("{LINK}", this.GetFacebookGameURL());
	}

	public void SetTwitterDisabled(bool zState)
	{
		SocialController.TwitterIsDisabled = zState;
	}

	public void SetFacebookDisabled(bool zState)
	{
		SocialController.FacebookIsDisabled = zState;
	}

	public void ShutdownSocialMedia()
	{
		this.SetTwitterDisabled(true);
		this.SetFacebookDisabled(true);
		GameCenterController.Instance.SetGameCenterDisabled(true);
	}

	public void OnShareButton(SocialController.MessageType inMessageType, string detail = null, bool WasTriggeredFromPopup = false, bool video = false)
	{
		this.MaintainPopUp = null;
		this.messageType = inMessageType;
		this.messageDetail = detail;
		if (WasTriggeredFromPopup)
		{
			this.MaintainPopUp = PopUpManager.Instance.GetCurrentPopUp();
		}
		this.HandleShareButton((!video) ? SharePlatform.Native : SharePlatform.NativeVideo);
	}

	private void OnCloseButton()
	{
		PopUpManager.Instance.KillPopUp();
		this.ShowMaintainPopup();
	}

	private void ShowMaintainPopup()
	{
		if (this.MaintainPopUp != null)
		{
			PopUpManager.Instance.TryShowPopUp(this.MaintainPopUp, PopUpManager.ePriority.Default, null);
			this.MaintainPopUp = null;
		}
	}

	private SocialController.Shared GetRecentlyTweeted(string message)
	{
		if (!this.RecentlyTweeted.ContainsKey(message))
		{
			Dictionary<string, SocialController.Shared> dictionary = new Dictionary<string, SocialController.Shared>();
			foreach (KeyValuePair<string, SocialController.Shared> current in this.RecentlyTweeted)
			{
				TimeSpan t = GTDateTime.Now - current.Value.mTwitter;
				if (t < this.twitterRepostTimeOut)
				{
					dictionary.Add(current.Key, current.Value);
				}
			}
			dictionary.Add(message, default(SocialController.Shared));
			this.RecentlyTweeted = dictionary;
		}
		return this.RecentlyTweeted[message];
	}

	private void LogSharedTwitter(string message)
	{
		SocialController.Shared recentlyTweeted = this.GetRecentlyTweeted(message);
		recentlyTweeted.mTwitter = GTDateTime.Now;
		this.RecentlyTweeted[message] = recentlyTweeted;
	}

	public bool RecentlySharedTwitter(string messageKey)
	{
		bool result = false;
		if (this.RecentlyTweeted.ContainsKey(messageKey))
		{
			TimeSpan t = GTDateTime.Now - this.RecentlyTweeted[messageKey].mTwitter;
			result = (t < this.twitterRepostTimeOut);
		}
		return result;
	}

	private string MakeMessageKey()
	{
		return this.messageType.ToString() + ":" + this.messageDetail;
	}

	private void OnFacebookButton()
	{
		this.HandleShareButton(SharePlatform.Facebook);
	}

	private void OnTwitterButton()
	{
		this.HandleShareButton(SharePlatform.Twitter);
	}

	private void HandleShareButton(SharePlatform platform)
	{
		switch (this.messageType)
		{
		case SocialController.MessageType.LEVEL_UP:
			this.ShareLevelUp(platform, this.messageDetail);
			break;
		case SocialController.MessageType.NEW_CAR:
			this.ShareNewCar(platform, this.messageDetail);
			break;
		case SocialController.MessageType.UNLOCK_TIER:
			this.ShareTierUp(platform, this.messageDetail);
			break;
		case SocialController.MessageType.WIN_RACE:
			this.ShareRaceWon(platform, this.messageDetail);
			break;
		case SocialController.MessageType.INVITE:
			this.ShareInvite(platform);
			break;
		case SocialController.MessageType.SNAPSHOT:
			this.ShareSnapshot(string.Empty);
			break;
		case SocialController.MessageType.WIN_CAR:
			this.ShareCarWon(platform, this.messageDetail);
			break;
		case SocialController.MessageType.UNLOCK_FRIENDS_TIER:
			this.ShareFriendsTierUp(platform, this.messageDetail);
			break;
		case SocialController.MessageType.FRIENDS_WIN_CAR:
			this.ShareFriendsWinCar(platform, this.messageDetail);
			break;
		case SocialController.MessageType.FRIENDS_WIN_GOLD:
			this.ShareFriendsWinGold(platform, this.messageDetail);
			break;
		case SocialController.MessageType.FRIENDS_ONE_STAR:
			this.ShareFriendsGotStar(platform, this.messageDetail, 1);
			break;
		case SocialController.MessageType.FRIENDS_TWO_STARS:
			this.ShareFriendsGotStar(platform, this.messageDetail, 2);
			break;
		case SocialController.MessageType.FRIENDS_THREE_STARS:
			this.ShareFriendsGotStar(platform, this.messageDetail, 3);
			break;
		case SocialController.MessageType.FRIENDS_BEAT_FRIENDS:
			this.ShareFriendsBeatFriends(platform, this.messageDetail);
			break;
		case SocialController.MessageType.FRIENDS_BEAT_NEXT_FRIEND:
			this.ShareFriendsBeatNextFriend(platform, this.messageDetail);
			break;
		case SocialController.MessageType.FRIENDS_PERSONAL_BEST:
			this.ShareFriendsPersonalBest(platform, this.messageDetail);
			break;
		}
		if (platform != SharePlatform.Native)
		{
			this.ShowMaintainPopup();
		}
	}

	public void ShareLevelUp(SharePlatform platform, string level = null)
	{
		switch (platform)
		{
		case SharePlatform.Facebook:
			this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_FACEBOOK_LEVELUP_TITLE"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_LEVELUP_NAME"), level, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), false);
			goto IL_C8;
		case SharePlatform.Twitter:
			this.TrySendTweet(string.Format(this.GetTwitterMessage("TEXT_TWITTER_LEVELUP_NAME"), level, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, string.Empty);
			goto IL_C8;
		}
        LevelUpScreen levelUpScreen = ScreenManager.Instance.ActiveScreen as LevelUpScreen;
        levelUpScreen.SetShareButtonString(true);
        levelUpScreen.PrepareForScreenshot();
        this.ShareNativeByScreenshot(string.Format(this.GetTwitterMessage("TEXT_TWITTER_LEVELUP_NAME"), level, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()));
		IL_C8:
		this.metricsSocialEvent = "LevelUp";
	}

	public void ShareNewCar(SharePlatform platform, string carname)
	{
		switch (platform)
		{
		case SharePlatform.Facebook:
			this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_FACEBOOK_NEWCAR_TITLE"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_NEWCAR_NAME"), carname, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), false);
			goto IL_F4;
		case SharePlatform.Twitter:
			this.TrySendTweet(string.Format(this.GetTwitterMessage("TEXT_TWITTER_NEWCAR_NAME"), carname, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, string.Empty);
			goto IL_F4;
		}
		if (ScreenManager.Instance.ActiveScreen.GetType() == typeof(ShowroomScreen))
		{
			this.ShareNativeByScreenshot(string.Format(this.GetTwitterMessage("TEXT_TWITTER_NEWCAR_NAME"), carname, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()));
		}
		else
		{
			this.ShareNativeWithOptionalImage(string.Format(this.GetTwitterMessage("TEXT_TWITTER_NEWCAR_NAME"), carname, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty);
		}
		IL_F4:
		this.metricsSocialEvent = "NewCar";
	}

	public void ShareSnapshot(string s = null)
	{
		string text = s;
		if (string.IsNullOrEmpty(text))
		{
			if (this.CurrentSocialNag == SocialTriggers.NewDecal)
			{
				text = string.Format(this.GetTwitterMessage("TEXT_TWITTER_NEWDECAL_PIC"), this.CurrentSocialNagValue, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
			}
			else if (this.CurrentSocialNag == SocialTriggers.NewNumberPlate)
			{
				text = string.Format(this.GetTwitterMessage("TEXT_TWITTER_NEWLICENCE_PIC"), BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
			}
			else if (this.CurrentSocialNag == SocialTriggers.NewSeasonPrize)
			{
				text = string.Format(this.GetTwitterMessage("TEXT_TWITTER_SEASONPRIZE_PIC"), this.CurrentSocialNagValue, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
			}
			else if (this.CurrentSocialNag == SocialTriggers.InternationalCarAward)
			{
				text = string.Format(this.GetTwitterMessage("TEXT_TWITTER_INTERNATIONAL_CARAWARD_PIC"), this.CurrentSocialNagValue, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
			}
			else if (this.CurrentSocialNag == SocialTriggers.InternationalLiveryAward)
			{
				text = string.Format(this.GetTwitterMessage("TEXT_TWITTER_INTERNATIONAL_LIVERYAWARD_PIC"), this.CurrentSocialNagValue, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
			}
			else if (this.CurrentSocialNag == SocialTriggers.LiveryAward)
			{
				text = string.Format(this.GetTwitterMessage("TEXT_TWITTER_LIVERYPRIZE_PIC"), this.CurrentSocialNagValue, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
			}
            //if (Utf32.LengthInTextElements(text) > 118)
            //{
            //    text = string.Empty;
            //}
			if (string.IsNullOrEmpty(text))
			{
				text = string.Format(this.GetTwitterMessage("TEXT_TWITTER_SEND_PIC"), BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
			}
		}
		this.ClearSocialNagTrigger();
		BasePlatform.ActivePlatform.ShareImage(text, string.Empty, ScreenshotCapture.Instance.CurrentFilename);
		this.metricsSocialEvent = "CarSnapshot";
        SendShareMetrics();
    }


    private void SendShareMetrics()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
			    /*AdjustEvent adjustEvent = new AdjustEvent("uhqrkn");
			    Adjust.trackEvent(adjustEvent);
			    */
#endif
    }

    public void ShareTierUp(SharePlatform platform, string tier = null)
	{
		switch (platform)
		{
		case SharePlatform.Facebook:
			this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_FACEBOOK_NEWTIER_TITLE"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_NEWTIER_NAME"), tier, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), false);
			goto IL_C2;
		case SharePlatform.Twitter:
			this.TrySendTweet(string.Format(this.GetTwitterMessage("TEXT_TWITTER_NEWTIER_NAME"), tier, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, string.Empty);
			goto IL_C2;
		}
        UnlockTierScreen tierUnlockedScreen = ScreenManager.Instance.ActiveScreen as UnlockTierScreen;
        tierUnlockedScreen.SetShareButtonString(true);
        this.ShareNativeByScreenshot(string.Format(this.GetTwitterMessage("TEXT_TWITTER_NEWTIER_NAME"), tier, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()));
		IL_C2:
		this.metricsSocialEvent = "TierUp";
	}

	public void ShareInvite(SharePlatform platform)
	{
		switch (platform)
		{
		case SharePlatform.Facebook:
			this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_EMAIL_APP_INVITE_P3"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_APP_INVITE"), BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), false);
			goto IL_AD;
		case SharePlatform.Twitter:
			this.TrySendTweet(string.Format(this.GetTwitterMessage("TEXT_TWITTER_APP_INVITE_2"), BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, string.Empty);
			goto IL_AD;
		}
		this.ShareNativeWithOptionalImage(string.Format(this.GetTwitterMessage("TEXT_TWITTER_APP_INVITE_2"), BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty);
		IL_AD:
		this.metricsSocialEvent = "Invite";
	}

	public void ShareRaceWon(SharePlatform platform, string bossname = null)
	{
		string name = string.Empty;
		string text = string.Empty;
		bool useBossIcon = false;
		switch (platform)
		{
		case SharePlatform.Facebook:
			if (!string.IsNullOrEmpty(bossname))
			{
				text = string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_BEATBOSS_NAME"), bossname, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
				name = LocalizationManager.GetTranslation("TEXT_FACEBOOK_BEATBOSS_TITLE");
				useBossIcon = true;
			}
			else if (this.CurrentSocialNag == SocialTriggers.QrtrMileMilestone)
			{
				text = string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_QTRMILE_TIME"), this.CurrentSocialNagValue, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
				name = LocalizationManager.GetTranslation("TEXT_FACEBOOK_QTRMILE_TIME_TITLE");
				this.ClearSocialNagTrigger();
			}
			else
			{
				text = string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_RACE_WON"), BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
				name = string.Format(LocalizationManager.GetTranslation("TEXT_FACEBOOK_RACE_WON_TITLE"), new object[0]);
			}
			this.PostToFacebookStream(name, LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), text, useBossIcon);
			goto IL_222;
		case SharePlatform.NativeVideo:
			if (!this.IsVideoReplayAvailable() || !VideoCapture.Share())
			{
			}
			goto IL_222;
		}
		if (!string.IsNullOrEmpty(bossname))
		{
			text = string.Format(this.GetTwitterMessage("TEXT_TWITTER_BEATBOSS_NAME"), bossname, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
		}
		else if (this.CurrentSocialNag == SocialTriggers.QrtrMileMilestone)
		{
			string arg = string.Empty;
			for (int i = 0; i < this.NaggableMilestones.Length; i++)
			{
				if (this.CurrentSocialNagValue == this.NaggableMilestones[i].ToString())
				{
					arg = this.NaggableHashtags[i];
				}
			}
			text = string.Format(this.GetTwitterMessage("TEXT_TWITTER_QTRMILE_TIME"), this.CurrentSocialNagValue, arg, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
			this.ClearSocialNagTrigger();
		}
		else
		{
			text = string.Format(this.GetTwitterMessage("TEXT_TWITTER_RACE_WON"), BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
		}
		if (platform == SharePlatform.Twitter)
		{
			this.TrySendTweet(text, string.Empty, string.Empty);
		}
		else if (bossname != null)
		{
			this.ShareNativeWithOptionalImage(text, this.GetBossImageURL());
		}
		else
		{
			this.ShareNativeByScreenshot(text);
		}
		IL_222:
		this.metricsSocialEvent = "RaceWon";
	}

	public bool IsVideoReplayAvailable()
	{
	    //return VideoCapture.IsSupportedAndEnabled && PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar && UnityReplayKit.IsPreviewAvailable();
	    return false;
	}

    public void ShareCarWon(SharePlatform platform, string carname)
	{
		switch (platform)
		{
		case SharePlatform.Facebook:
			this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_FACEBOOK_CAR_WON_TITLE"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_CAR_WON"), carname), false);
			goto IL_A4;
		case SharePlatform.Twitter:
			this.TrySendTweet(string.Format(this.GetTwitterMessage("TEXT_TWITTER_WON_CAR"), carname), string.Empty, string.Empty);
			goto IL_A4;
		}
        //PrizePieceGiveScreen prizePieceGiveScreen = CSRScreenManager.Instance.ActiveScreen as PrizePieceGiveScreen;
        //prizePieceGiveScreen.SetShareButtonString(true);
		this.ShareNativeByScreenshot(string.Format(this.GetTwitterMessage("TEXT_TWITTER_WON_CAR"), carname));
		IL_A4:
		this.metricsSocialEvent = "PrizeCar";
	}

	public void ShareFriendsTierUp(SharePlatform platform, string tier = null)
	{
		switch (platform)
		{
		case SharePlatform.Facebook:
			this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_FACEBOOK_FRIENDSTIER_TITLE"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_FRIENDSTIER_NAME"), tier, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), false);
			goto IL_C2;
		case SharePlatform.Twitter:
			this.TrySendTweet(string.Format(this.GetTwitterMessage("TEXT_TWITTER_FRIENDSTIER_NAME"), tier, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, string.Empty);
			goto IL_C2;
		}
        //FriendRewardScreen friendRewardScreen = CSRScreenManager.Instance.ActiveScreen as FriendRewardScreen;
        //friendRewardScreen.SetShareButtonString(true);
		this.ShareNativeByScreenshot(string.Format(this.GetTwitterMessage("TEXT_TWITTER_FRIENDSTIER_NAME"), tier, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()));
		IL_C2:
		this.metricsSocialEvent = "FriendsTierUp";
	}

	public void ShareFriendsWinCar(SharePlatform platform, string carname)
	{
		switch (platform)
		{
		case SharePlatform.Facebook:
			this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_FACEBOOK_FRIENDSWINCAR_TITLE"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_FRIENDSWINCAR_NAME"), carname, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), false);
			goto IL_C2;
		case SharePlatform.Twitter:
			this.TrySendTweet(string.Format(this.GetTwitterMessage("TEXT_TWITTER_FRIENDSWINCAR_NAME"), carname, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, string.Empty);
			goto IL_C2;
		}
        //FriendRewardScreen friendRewardScreen = CSRScreenManager.Instance.ActiveScreen as FriendRewardScreen;
        //friendRewardScreen.SetShareButtonString(true);
		this.ShareNativeByScreenshot(string.Format(this.GetTwitterMessage("TEXT_TWITTER_FRIENDSWINCAR_NAME"), carname, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()));
		IL_C2:
		this.metricsSocialEvent = "FriendsNewCar";
	}

	public void ShareFriendsWinGold(SharePlatform platform, string goldAmt)
	{
		switch (platform)
		{
		case SharePlatform.Facebook:
			this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_FACEBOOK_FRIENDSWINGOLD_TITLE"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_FRIENDSWINGOLD_NAME"), goldAmt, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), false);
			goto IL_C2;
		case SharePlatform.Twitter:
			this.TrySendTweet(string.Format(this.GetTwitterMessage("TEXT_TWITTER_FRIENDSWINGOLD_NAME"), goldAmt, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, string.Empty);
			goto IL_C2;
		}
        //FriendRewardScreen friendRewardScreen = CSRScreenManager.Instance.ActiveScreen as FriendRewardScreen;
        //friendRewardScreen.SetShareButtonString(true);
		this.ShareNativeByScreenshot(string.Format(this.GetTwitterMessage("TEXT_TWITTER_FRIENDSWINGOLD_NAME"), goldAmt, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()));
		IL_C2:
		this.metricsSocialEvent = "FriendsWonGold";
	}

	public void ShareFriendsGotStar(SharePlatform platform, string carName, int numStars)
	{
		switch (platform)
		{
		case SharePlatform.Facebook:
		{
			string facebookMessage = this.GetFacebookMessage(this.FBStarLocText[numStars]);
			this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_FACEBOOK_FRIENDSGETSTAR_TITLE"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), string.Format(facebookMessage, carName, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), false);
			goto IL_D9;
		}
		case SharePlatform.Twitter:
			this.TrySendTweet(string.Format(this.GetTwitterMessage(this.TwStarLocText[numStars]), carName, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, string.Empty);
			goto IL_D9;
		}
        //FriendRewardScreen friendRewardScreen = CSRScreenManager.Instance.ActiveScreen as FriendRewardScreen;
        //friendRewardScreen.SetShareButtonString(true);
		this.ShareNativeByScreenshot(string.Format(this.GetTwitterMessage(this.TwStarLocText[numStars]), carName, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()));
		IL_D9:
		this.metricsSocialEvent = "FriendsGotStar";
	}

	public void ShareFriendsBeatFriends(SharePlatform platform, string json)
	{
		JsonDict jsonDict = new JsonDict();
		string arg;
		int num;
		string arg2;
		if (jsonDict.Read(json) && jsonDict.TryGetValue("name", out arg) && jsonDict.TryGetValue("others_count", out num) && jsonDict.TryGetValue("car_name", out arg2))
		{
			switch (platform)
			{
			case SharePlatform.Facebook:
			{
				string description = string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_FRIENDSBEATFRIEND_NAME"), arg, arg2, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
				this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_FACEBOOK_FRIENDSBEATFRIENDS_TITLE"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), description, false);
				goto IL_11A;
			}
			case SharePlatform.Twitter:
				this.TrySendTweet(string.Format(this.GetTwitterMessage("TEXT_TWITTER_FRIENDSBEATFRIEND_NAME"), arg, arg2, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, string.Empty);
				goto IL_11A;
			}
            //FriendRewardScreen friendRewardScreen = CSRScreenManager.Instance.ActiveScreen as FriendRewardScreen;
            //friendRewardScreen.SetShareButtonString(true);
			this.ShareNativeByScreenshot(string.Format(this.GetTwitterMessage("TEXT_TWITTER_FRIENDSBEATFRIEND_NAME"), arg, arg2, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()));
		}
		IL_11A:
		this.metricsSocialEvent = "FriendsBeatFriends";
	}

	public void ShareFriendsBeatNextFriend(SharePlatform platform, string json)
	{
		JsonDict jsonDict = new JsonDict();
		string arg;
		if (jsonDict.Read(json) && jsonDict.TryGetValue("name", out arg))
		{
			switch (platform)
			{
			case SharePlatform.Facebook:
			{
				string description = string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_NEXTOPPONENT_NAME"), arg, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
				this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_FACEBOOK_FRIENDSBEATFRIENDS_TITLE"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), description, false);
				goto IL_EF;
			}
			case SharePlatform.Twitter:
				this.TrySendTweet(string.Format(this.GetTwitterMessage("TEXT_TWITTER_NEXTOPPONENT_NAME"), arg, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, string.Empty);
				goto IL_EF;
			}
            //FriendRewardScreen friendRewardScreen = CSRScreenManager.Instance.ActiveScreen as FriendRewardScreen;
            //friendRewardScreen.SetShareButtonString(true);
			this.ShareNativeByScreenshot(string.Format(this.GetTwitterMessage("TEXT_TWITTER_NEXTOPPONENT_NAME"), arg, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()));
		}
		IL_EF:
		this.metricsSocialEvent = "FriendsBeatFriends";
	}

	public void ShareFriendsPersonalBest(SharePlatform platform, string json)
	{
		JsonDict jsonDict = new JsonDict();
		string arg;
		float num;
		if (jsonDict.Read(json) && jsonDict.TryGetValue("car_name", out arg) && jsonDict.TryGetValue("time", out num))
		{
			switch (platform)
			{
			case SharePlatform.Facebook:
			{
				string description = string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_FRIENDSPERSONALBEST_NAME"), num, arg, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName());
				this.PostToFacebookStream(LocalizationManager.GetTranslation("TEXT_FACEBOOK_FRIENDSPERSONALBEST_TITLE"), LocalizationManager.GetTranslation("TEXT_GAME_TITLE_FULL"), description, false);
				goto IL_115;
			}
			case SharePlatform.Twitter:
				this.TrySendTweet(string.Format(this.GetTwitterMessage("TEXT_TWITTER_FRIENDSPERSONALBEST_NAME"), num, arg, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, string.Empty);
				goto IL_115;
			}
            //FriendRewardScreen friendRewardScreen = CSRScreenManager.Instance.ActiveScreen as FriendRewardScreen;
            //friendRewardScreen.SetShareButtonString(true);
			this.ShareNativeByScreenshot(string.Format(this.GetTwitterMessage("TEXT_TWITTER_FRIENDSPERSONALBEST_NAME"), num, arg, BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()));
		}
		IL_115:
		this.metricsSocialEvent = "FriendsPersonalBest";
	}

	public void ShareNativeWithOptionalImage(string message, string imageurl)
	{
		BasePlatform.ActivePlatform.ShareText(string.Empty, message, string.Empty, imageurl);
	    SendShareMetrics();
	}

	public void ShareNativeByScreenshot(string message)
	{
		this.hiddenPopup = PopUpManager.Instance.GetCurrentPopUp();
		if (this.hiddenPopup != null)
		{
			PopUpManager.Instance.KillPopUp();
		}
		BubbleManager.Instance.KillAllMessages();
		//AchievementsController.Instance.SetAchievementVisibilty(false);
		ScreenshotCapture.Instance.CaptureAndTweetIfPossible(message, new Action(this.ReInstateUI));
	}

	public string GetBossImageURL()
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent.IsBossRace() && currentEvent.BossRaceIndex() == 2)
		{
			eCarTier carTier = currentEvent.Parent.GetTierEvents().GetCarTier();
			int num = (int)(carTier + 1);
			return string.Format(this.BossImageBase, num);
		}
		return string.Empty;
	}

	public void SetNewSocialNagTrigger(SocialTriggers trigger, string data)
	{
		this.CurrentSocialNag = trigger;
		this.CurrentSocialNagValue = data;
	}

	public void ClearSocialNagTrigger()
	{
		this.CurrentSocialNag = SocialTriggers.None;
		this.CurrentSocialNagValue = string.Empty;
	}

	public void InitiateFacebookStreamPost()
	{
		this.attemptingFacebookStreamPost = true;
	}

	public void CancelFacebookStreamPost()
	{
		this.attemptingFacebookStreamPost = false;
	}

	public void InititateFacebookLike()
	{
		this.attemptingFacebookLike = true;
	}

	public void CancelFacebookLike()
	{
		this.attemptingFacebookLike = false;
	}

	private void fbDidLogin(string accessToken, string expirationToken)
	{
		this.SetAccessTokens(accessToken, expirationToken);
		this.isLoggedIntoFacebook = true;
        //NmgBinding.AddSocialNetworkData("facebookauth", accessToken);
		this.waitingForLoginResponse = false;
		if (this.attemptingFacebookStreamPost)
		{
			BasePlatform.ActivePlatform.PostToFacebook(this.FacebookStreamName, this.FacebookStreamCaption, this.FacebookStreamDescription, this.GetFacebookGameURL(), this.FacebookStreamIcon);
			this.attemptingFacebookStreamPost = false;
		}
		else if (this.attemptingFacebookLike)
		{
			BasePlatform.ActivePlatform.PerformLikeUsOnFacebook();
			this.attemptingFacebookLike = false;
		}
		this.GetFacebookUserInfo();
	}

	private void fbDidLogout()
	{
		this.facebookID = string.Empty;
		this.facebookName = string.Empty;
		this.facebookFirstName = string.Empty;
		this.facebookLastName = string.Empty;
		this.facebookEmail = string.Empty;
		this.facebookTokenForBusiness = string.Empty;
		this.facebookPermissions = null;
		this.SetAccessTokens(string.Empty, string.Empty);
		this.isLoggedIntoFacebook = false;
		this.waitingForLoginResponse = false;
		this.RecievedFBUserInfo = false;
	}

	private void fbDidNotLogin(int errorCode)
	{
		if (errorCode == 2)
		{
			PopUp popup = new PopUp
			{
				Title = "TEXT_FACEBOOK_DISABLED_TITLE",
				BodyText = "TEXT_FACEBOOK_DISABLED_MESSAGE",
				ConfirmAction = new PopUpButtonAction(this.OnCloseButton),
				ConfirmText = "TEXT_BUTTON_OK"
			};
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.SystemUrgent, null);
		}
		this.waitingForLoginResponse = false;
	}

	public void GetFacebookFriendsList()
	{
		BasePlatform.ActivePlatform.GetFacebookFriendsList(this.facebookFriendLoadLimit);
	}

	public void GetFacebookUserInfo()
	{
		BasePlatform.ActivePlatform.GetFacebookUserInfo();
		BasePlatform.ActivePlatform.GetFacebookTokenForBusiness();
	}

	public void GetFacebookUserPermissions()
	{
		BasePlatform.ActivePlatform.GetFacebookUserPermissions();
	}

	public void RequestFacebookUserPermissions(List<string> permissions)
	{
		BasePlatform.ActivePlatform.RequestFacebookUserPermissions(permissions);
	}

	public void CheckFacebookPermissionsBeforePerformingInvite(SocialConfiguration.FacebookPermissionList permissionList)
	{
		if (!SocialController.Instance.isLoggedIntoFacebook)
		{
			return;
		}
		List<string> list = GameDatabase.Instance.SocialConfiguration.GetFacebookPermissions(permissionList);
		if (SocialController.Instance.FacebookPermissions.Granted(list))
		{
			SocialController.Instance.PerformInviteFriendsOnFacebook();
		}
		else
		{
			BasePlatform.ActivePlatform.RequestFacebookUserFriendsPermissions(list);
		}
	}

	public void RevokeFacebookUserPermission(string permission)
	{
		BasePlatform.ActivePlatform.RevokeFacebookUserPermission(permission);
	}

	public void GetFacebookFriendProfilePic(string user)
	{
		BasePlatform.ActivePlatform.GetFacebookFriendProfilePic(user);
	}

	public void CheckAndShrinkProfilePicCache()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(this.FriendsPicsPath);
		if (!directoryInfo.Exists)
		{
			return;
		}
		FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
		IOrderedEnumerable<FileSystemInfo> source = from f in fileSystemInfos
		orderby f.CreationTime
		select f;
		int num = fileSystemInfos.Length - 100;
		if (num > 0)
		{
			List<FileSystemInfo> list = source.Take(num).ToList<FileSystemInfo>();
			list.ForEach(delegate(FileSystemInfo file)
			{
				file.Delete();
			});
		}
	}

	public void ClearProfilePicCache()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(this.FriendsPicsPath);
		if (!directoryInfo.Exists)
		{
			return;
		}
		List<FileSystemInfo> list = directoryInfo.GetFileSystemInfos().ToList<FileSystemInfo>();
		list.ForEach(delegate(FileSystemInfo file)
		{
			file.Delete();
		});
	}

	private void FacebookLogOutShowErrorPopup()
	{
		BasePlatform.ActivePlatform.FacebookLogout();
		PopUp popup = new PopUp
		{
			Title = "TEXT_FACEBOOK_UNKNOWN_ERROR_TITLE",
			BodyText = "TEXT_FACEBOOK_UNKNOWN_ERROR_MESSAGE",
			ConfirmAction = new PopUpButtonAction(this.OnCloseButton),
			ConfirmText = "TEXT_BUTTON_OK"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public void fbGotUserInfo(string userInfoJSON)
	{
		JsonDict jsonDict = new JsonDict();
		if (!jsonDict.Read(userInfoJSON) || string.IsNullOrEmpty(jsonDict.GetString("id")))
		{
			this.FacebookLogOutShowErrorPopup();
			return;
		}
		this.facebookID = jsonDict.GetString("id");
		this.facebookName = jsonDict.GetString("name");
		this.facebookFirstName = jsonDict.GetString("first_name");
		this.facebookLastName = jsonDict.GetString("last_name");
		this.facebookEmail = jsonDict.GetString("email");
		this.facebookPictureURL = string.Empty;
		this.facebookPictureIsSilhouette = true;
		JsonDict jsonDict2 = jsonDict.GetJsonDict("picture");
		if (jsonDict2 != null)
		{
			JsonDict jsonDict3 = jsonDict2.GetJsonDict("data");
			if (jsonDict3 != null)
			{
				this.facebookPictureURL = jsonDict3.GetString("url");
				this.facebookPictureIsSilhouette = jsonDict3.GetBool("is_silhouette");
			}
		}
        //NmgBinding.AddSocialNetworkData("facebook", this.facebookEmail);
        //NmgBinding.AddSocialNetworkData("facebookid", this.facebookID);
		if (!PlayerProfileManager.Instance.ActiveProfile.HasSignedIntoFacebookBefore)
		{
			PlayerProfileManager.Instance.ActiveProfile.HasSignedIntoFacebookBefore = true;
			PlayerProfileManager.Instance.ActiveProfile.Save();
		}
		this.RecievedFBUserInfo = true;
	}

	public void fbGotTokenForBusiness(string tfbJSON)
	{
		JsonDict jsonDict = new JsonDict();
		if (!jsonDict.Read(tfbJSON))
		{
			return;
		}
		this.facebookTokenForBusiness = jsonDict.GetString("token_for_business");
        //NmgBinding.AddSocialNetworkData("facebook_tfb", this.facebookTokenForBusiness);
	}

	public void fbGotUserPermissions(string userPermissionsJSON)
	{
		JsonDict jsonDict = new JsonDict();
		if (!jsonDict.Read(userPermissionsJSON))
		{
			return;
		}
		this.facebookPermissions = new FacebookPermissions(jsonDict);
	}

	public void fbRequestFailed(int errorCode)
	{
		if (errorCode == -1009)
		{
			return;
		}
		if (this.MaintainPopUp == null)
		{
			this.MaintainPopUp = PopUpManager.Instance.GetCurrentPopUp();
		}
		PopUpManager.Instance.KillPopUp();
		this.FacebookLogOutShowErrorPopup();
	}

	private void fbFailed(int errorCode)
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_NO_INTERNET_TITLE",
			BodyText = "TEXT_NO_INTERNET_MESSAGE",
			ConfirmAction = new PopUpButtonAction(this.OnCloseButton),
			ConfirmText = "TEXT_BUTTON_OK"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.SystemUrgent, null);
	}

	public void fbPostSucceeded()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.Pltfrm,
				"Facebook"
			},
			{
				Parameters.Intfrm,
				this.metricsSocialEvent
			}
		};
		Log.AnEvent(Events.SocialInteraction, data);
		activeProfile.CumulativeFBPosts++;
		this.FacebookSendWaiter = null;
		this.metricsSocialEvent = "NotSet";
	}

	public void fbPostCancelled()
	{
	}

	public void fbInviteFriendsSucceeded(int count)
	{
		PlayerProfileManager.Instance.ActiveProfile.FriendsInvited += count;
		Log.AnEvent(Events.FriendsInviteFriends, new Dictionary<Parameters, string>
		{
			{
				Parameters.FriendsInv,
				count.ToString()
			}
		});
	}

	public void fbRequestFriendsPermission(bool approved)
	{
		SocialController.Instance.GetFacebookUserPermissions();
		SocialController.Instance.PerformInviteFriendsOnFacebook();
	}

	public void fbRequestPermission(bool approved)
	{
	}

	public string GetFacebookID()
	{
		return this.facebookID;
	}

	public string GetFacebookTokenForBusiness()
	{
		return this.facebookTokenForBusiness;
	}

	public string GetFacebookName()
	{
		return NameValidater.ReplaceUnsupportedCharacters(this.facebookName);
	}

	public string GetFacebookNameWithUnsupportedCharacters()
	{
		return this.facebookName;
	}

	public string GetFacebookFirstName()
	{
		return NameValidater.ReplaceUnsupportedCharacters(this.facebookFirstName);
	}

	public string GetFacebookLastName()
	{
		return NameValidater.ReplaceUnsupportedCharacters(this.facebookLastName);
	}

	public string GetFacebookEmail()
	{
		return this.facebookEmail;
	}

	public string GetFacebookPictureURL()
	{
		return this.facebookPictureURL;
	}

	public void PostToFacebookStream(string Name, string Caption, string Description, bool useBossIcon = false)
	{
		this.FacebookStreamName = Name;
		this.FacebookStreamCaption = Caption;
		this.FacebookStreamDescription = Description;
		this.FacebookStreamIcon = this.FacebookImage;
		if (useBossIcon)
		{
			this.FacebookStreamIcon = this.GetBossImageURL();
		}
		string accessToken;
		string expirationDate;
		this.GetAccessTokens(out accessToken, out expirationDate);
		List<string> userPermissions = GameDatabase.Instance.SocialConfiguration.GetFacebookPermissions(SocialConfiguration.FacebookPermissionList.PostLogin);
		this.waitingForLoginResponse = true;
		BasePlatform.ActivePlatform.PerformFacebookSSO(accessToken, expirationDate, userPermissions);
		base.StartCoroutine("WaitForLoginResponse");
		SocialController.Instance.InitiateFacebookStreamPost();
		if (SocialController.Instance.isLoggedIntoFacebook)
		{
			SocialController.Instance.CancelFacebookStreamPost();
			BasePlatform.ActivePlatform.PostToFacebook(this.FacebookStreamName, this.FacebookStreamCaption, this.FacebookStreamDescription, this.GetFacebookGameURL(), this.FacebookStreamIcon);
		}
	}

	private IEnumerator WaitForLoginResponse()
	{
	    //SocialController.<WaitForLoginResponse>c__Iterator1F <WaitForLoginResponse>c__Iterator1F = new SocialController.<WaitForLoginResponse>c__Iterator1F();
        //<WaitForLoginResponse>c__Iterator1F.<>f__this = this;
        //return <WaitForLoginResponse>c__Iterator1F;
	    return null;
	}

    private void GetAccessTokens(out string accessToken, out string expirationToken)
	{
		accessToken = string.Empty;
		expirationToken = string.Empty;
		if (SocialController.Instance.isLoggedIntoFacebook && UserManager.Instance != null && UserManager.Instance.currentAccount != null)
		{
			accessToken = UserManager.Instance.currentAccount.FBAccessToken;
			expirationToken = UserManager.Instance.currentAccount.FBExpirationDate;
		}
	}

	private void SetAccessTokens(string accessToken, string expirationToken)
	{
		if (UserManager.Instance != null && UserManager.Instance.currentAccount != null)
		{
			UserManager.Instance.currentAccount.FBAccessToken = accessToken;
			UserManager.Instance.currentAccount.FBExpirationDate = expirationToken;
			UserManager.Instance.SaveCurrentAccount();
		}
	}

	public void PerformLikeUsOnFacebook()
	{
		string accessToken;
		string expirationDate;
		this.GetAccessTokens(out accessToken, out expirationDate);
		SocialController.Instance.isLoggedIntoFacebook = BasePlatform.ActivePlatform.PerformFacebookSSO(accessToken, expirationDate, GameDatabase.Instance.SocialConfiguration.GetFacebookPermissions(SocialConfiguration.FacebookPermissionList.HomeScreenLogin));
		SocialController.Instance.InititateFacebookLike();
		if (SocialController.Instance.isLoggedIntoFacebook)
		{
			SocialController.Instance.CancelFacebookLike();
			BasePlatform.ActivePlatform.PerformLikeUsOnFacebook();
		}
	}

	public void SendInvitationRequestsToFacebook(string title, string message)
	{
		if (SocialController.Instance.isLoggedIntoFacebook)
		{
			BasePlatform.ActivePlatform.SendInvitationRequestsToFacebook(title, message);
		}
	}

	public void PerformInviteFriendsOnFacebook()
	{
		this.SendInvitationRequestsToFacebook(LocalizationManager.GetTranslation("TEXT_FACEBOOK_INVITE_FRIENDS"), string.Format(this.GetFacebookMessage("TEXT_FACEBOOK_APP_INVITE"), BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()));
	}

	public void SendInviteTweetAction(TwitterSentCallBack OnTwitterFinished)
	{
		string empty = string.Empty;
		this.TrySendTweet(string.Format(this.GetInviteMessage("TEXT_TWITTER_APP_INVITE"), BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, empty, OnTwitterFinished);
		this.metricsSocialEvent = "Invite";
	}

	public bool TrySendTweet(string contents)
	{
		return this.TrySendTweet(contents, string.Empty, string.Empty, new TwitterSentCallBack(this.TwitterRewardCash));
	}

	public bool TrySendTweet(string contents, string url, string image)
	{
		return this.TrySendTweet(contents, url, image, new TwitterSentCallBack(this.TwitterRewardCash));
	}

	public bool TrySendTweet(string contents, string url, string image, TwitterSentCallBack OnTwitterCompletion)
	{
		if (SocialController.TwitterIsDisabled)
		{
			BasePlatform.ActivePlatform.Alert(LocalizationManager.GetTranslation("TEXT_TWITTER_DISABLED"));
			return false;
		}
		//if (!BasePlatform.ActivePlatform.CanSendTweet())
		//{
		//	BasePlatform.ActivePlatform.Alert(LocalizationManager.GetTranslation("TEXT_TWITTER_LOGIN"));
		//	return false;
		//}
		//if (this.TwitterSendWaiter == null)
		//{
		//	this.TwitterSendWaiter = new GameObject("TwitterSendWaiter");
		//	this.TwitterSendWaiter.AddComponent<TwitterListener>();
		//	TwitterListener component = this.TwitterSendWaiter.GetComponent<TwitterListener>();
		//	TwitterListener expr_7F = component;
		//	expr_7F.TwitterFinished = (TwitterSentCallBack)Delegate.Combine(expr_7F.TwitterFinished, new TwitterSentCallBack(this.TwitterActionDealtWith));
		//	TwitterListener expr_A1 = component;
		//	expr_A1.TwitterFinished = (TwitterSentCallBack)Delegate.Combine(expr_A1.TwitterFinished, OnTwitterCompletion);
		//}
		BasePlatform.ActivePlatform.SendTweet(contents, url, image);
		return true;
	}

	private void TwitterRewardCash(bool success)
	{
		if (success)
		{
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			int num = -activeProfile.GetCurrentCash();
			int num2 = -activeProfile.GetCurrentGold();
			this.HandleSocialCashReward();
			num += activeProfile.GetCurrentCash();
			num2 += activeProfile.GetCurrentGold();
			Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
			{
				{
					Parameters.Pltfrm,
					"Twitter"
				},
				{
					Parameters.Intfrm,
					this.metricsSocialEvent
				},
				{
					Parameters.DCsh,
					num.ToString()
				},
				{
					Parameters.DGld,
					num2.ToString()
				}
			};
			Log.AnEvent(Events.SocialInteraction, data);
			this.metricsSocialEvent = "NotSet";
			this.LogSharedTwitter(this.MakeMessageKey());
			activeProfile.CumulativeTweets++;
		}
	}

	private void GotTwitterUserFollowsUsOnFacebook(bool val)
	{
		if (this.DoesTwitterUserFollowUsEvent != null)
		{
			this.DoesTwitterUserFollowUsEvent(val);
		}
	}

	public void PerformFollowUsOnTwitter(TwitterSentCallBack OnTwitterCompletion)
	{
		if (!BasePlatform.ActivePlatform.CanSendTweet())
		{
			BasePlatform.ActivePlatform.Alert(LocalizationManager.GetTranslation("TEXT_TWITTER_LOGIN"));
			return;
		}
		if (this.TwitterSendWaiter == null)
		{
			this.TwitterSendWaiter = new GameObject("TwitterFollowWaiter");
			this.TwitterSendWaiter.AddComponent<TwitterListener>();
			TwitterListener component = this.TwitterSendWaiter.GetComponent<TwitterListener>();
			TwitterListener expr_5E = component;
			expr_5E.TwitterFinished = (TwitterSentCallBack)Delegate.Combine(expr_5E.TwitterFinished, new TwitterSentCallBack(this.TwitterActionDealtWith));
			TwitterListener expr_80 = component;
			expr_80.TwitterFinished = (TwitterSentCallBack)Delegate.Combine(expr_80.TwitterFinished, OnTwitterCompletion);
		}
		BasePlatform.ActivePlatform.FollowUsOnTwitter();
	}

	private void TwitterActionDealtWith(bool success)
	{
		this.TwitterSendWaiter = null;
	}

	public bool HandleSocialCashReward()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int twitterCashRewardTimeHours = GameDatabase.Instance.SocialConfiguration.TwitterCashRewardTimeHours;
		if (activeProfile.TwitterCashRewardsTime.AddHours((double)twitterCashRewardTimeHours) < GTDateTime.Now)
		{
			activeProfile.TwitterCashRewardsTime = GTDateTime.Now;
			activeProfile.TwitterCashRewardsCount = 0;
		}
		if (activeProfile.TwitterCashRewardsCount < GameDatabase.Instance.SocialConfiguration.TwitterCashRewardAllowed)
		{
			activeProfile.TwitterCashRewardsTime = GTDateTime.Now;
			activeProfile.TwitterCashRewardsCount++;
			PlayerProfileManager.Instance.ActiveProfile.AddCash(GameDatabase.Instance.Social.GetCashRewardForTwitter(),"reward","SocialTwitter");
			if (this.OnGivenSocialReward != null)
			{
				this.OnGivenSocialReward();
			}
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
			return true;
		}
		return false;
	}

	public bool SocialRewardAllowed()
	{
		return false;
	}
}
