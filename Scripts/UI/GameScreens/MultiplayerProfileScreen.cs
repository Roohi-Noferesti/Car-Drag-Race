using Fabric;
using Metrics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerProfileScreen : ZHUDScreen
{
	public static PlayerInfo LocalPlayerInfo;

	public static CachedOpponentInfo ReplayData;

	public static bool isOwnProfile;

    //public EZScreenPlacement MiddleNode;

	public Transform StatsPlacement;

	public RuntimeButton GoRaceButton;

	public TextMeshProUGUI RankText;

	public TextMeshProUGUI userNameText;

	public RuntimeButton StopOutsideClicksButton;

	public AvatarPicture AvatarPic;

	public RuntimeTextButton ShareButton;

	public RawImage BackgroundSprite;

	public Transform carListParent;

	public GameObject StatListItemPrefab;

	public GameObject StatListItemCarListPrefab;

	public float RowHeight;

	public Vector2 statBoxPadding = new Vector2(0.1f, 0.1f);

	public StatBox.StatType[] StatsToDisplay;

	private List<StatBox> StatBoxes;

	private bool previousGestureState;

	private bool ScreenLocked;

	private StatListItemExtra carListItem;

	private ProfileShare ProfileSharer;

	private string ProfileSnapshotPath;

	private FullScreenFlash fullScreenFlash;

	private int currentAvatar = -1;

	private int lastAvatar = -1;

	private Color tintColor;

	public int StatsInRow = 4;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.MultiplayerProfile;
		}
	}

	public static PlayerInfo Info
	{
		get
		{
			if (MultiplayerProfileScreen.isOwnProfile)
			{
				return MultiplayerProfileScreen.LocalPlayerInfo;
			}
			return MultiplayerProfileScreen.ReplayData.PlayerReplay.playerInfo;
		}
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		if (zAlreadyOnStack)
		{
		}
        //ScreenManager.Instance.SetupBackground(BackgroundManager.BackgroundType.Black);
		base.gameObject.name = "MultiplayerProfileScreen";
		base.OnActivate(zAlreadyOnStack);
		this.previousGestureState = TouchManager.Instance.GesturesEnabled;
		TouchManager.Instance.GesturesEnabled = true;
		bool show = true;
		if (MultiplayerProfileScreen.isOwnProfile)
		{
			show = false;
		}
		this.ShowRaceButton(show);
		if (!MultiplayerUtils.DisableMultiplayer)
		{
			this.CreateRankText();
		}
		else
		{
			this.RankText.gameObject.SetActive(false);
		}
		this.DisableClickPrevention();
		this.SetTintColor();
		this.UpdateUsernameText(MultiplayerProfileScreen.Info.DisplayName);
		this.UpdateAvatar();
		this.SetupBackground();
		this.SetupCarsOwnedList();
		this.CreateStatBoxes();
		bool flag = BasePlatform.ActivePlatform.UsesNativeSharing() || BasePlatform.ActivePlatform.CanSendTweet();
		if (MultiplayerProfileScreen.isOwnProfile && flag)
		{
			this.SetupProfileSharer();
		}
		else
		{
			this.ShareButton.gameObject.SetActive(false);
		}
	}

	private void SetTintColor()
	{
		eCarTier tier;
		if (MultiplayerProfileScreen.isOwnProfile)
		{
			tier = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().CurrentTier;
		}
		else
		{
			PlayerReplay playerReplay = MultiplayerProfileScreen.ReplayData.PlayerReplay;
			RacePlayerInfoComponent component = playerReplay.playerInfo.GetComponent<RacePlayerInfoComponent>();
			tier = CarDatabase.Instance.GetCar(component.CarDBKey).BaseCarTier;
		}
		this.tintColor = GameDatabase.Instance.Colours.GetTierColour(tier);
	}

	private void SetupBackground()
	{
        //Texture2D texture2D = this.BackgroundSprite.renderer.material.GetTexture("_MainTex") as Texture2D;
        //Vector2 pixeldimensions = new Vector2((float)texture2D.width * (GUICamera.Instance.ScreenWidth / ((float)texture2D.width / 125f)), (float)texture2D.height);
        //this.BackgroundSprite.Setup(GUICamera.Instance.ScreenWidth, this.BackgroundSprite.height, new Vector2(0f, (float)texture2D.height - 1f), pixeldimensions);
        //this.BackgroundSprite.SetColor(this.tintColor);
	}

	private void SetupProfileSharer()
	{
		GameObject original = (GameObject)Resources.Load("Prefabs/ProfileShare");
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
		this.ProfileSharer = gameObject.GetComponent<ProfileShare>();
		this.ProfileSnapshotPath = Path.Combine(FileUtils.temporaryCachePath, "ProfileShare.png");
		GameObject original2 = Resources.Load<GameObject>("Flares/FullScreenFlash");
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(original2);
        //gameObject2.transform.SetParent(this.MiddleNode.transform, false);
		this.fullScreenFlash = gameObject2.GetComponent<FullScreenFlash>();
		if (!this.ProfileSharer.IsReadyToShare())
		{
			RuntimeTextButton componentInChildren = this.ShareButton.GetComponentInChildren<RuntimeTextButton>();
			if (componentInChildren != null)
			{
				componentInChildren.CurrentState = BaseRuntimeControl.State.Disabled;
			}
		}
	}

	private void PositionShareButton(float horizontalBoxSpace, float verticalBoxSpace)
	{
		if (this.ShareButton.gameObject.activeSelf)
		{
			Vector3 localPosition = this.ShareButton.transform.localPosition;
			localPosition.x = -this.statBoxPadding.x - horizontalBoxSpace * 0.5f;
			localPosition.y = this.statBoxPadding.y + verticalBoxSpace * 0.5f;
			this.ShareButton.transform.localPosition = localPosition;
		}
	}

	private void CalculateStatBoxesScaleAndWidth(float boxWidth, float horizontalBoxSpace, float verticalBoxSpace, out float scale, out float newWidth)
	{
		float num = horizontalBoxSpace / boxWidth;
		float num2 = verticalBoxSpace / this.RowHeight;
		scale = 1f;
		newWidth = boxWidth;
		if (num <= num2)
		{
			scale = num;
		}
		else
		{
			scale = num2;
			newWidth = boxWidth * (horizontalBoxSpace / (boxWidth * scale));
		}
	}

	private void CreateStatBoxes()
	{
		this.StatBoxes = new List<StatBox>();
		StatBox component = this.StatListItemPrefab.GetComponent<StatBox>();
		float boxWidth = component.GetBoxWidth();
		int num = Mathf.CeilToInt((float)this.StatsToDisplay.Length / (float)this.StatsInRow);
	    float num2 = 0;// GUICamera.Instance.ScreenWidth / (float)this.StatsInRow - this.statBoxPadding.x * 2f;
	    float num3 = 0;//(GUICamera.Instance.ScreenHeight - Mathf.Abs(this.StatsPlacement.localPosition.y)) / (float)num - this.statBoxPadding.y;
		float scale;
		float boxWidth2;
		this.CalculateStatBoxesScaleAndWidth(boxWidth, num2, num3, out scale, out boxWidth2);
		for (int i = 0; i < this.StatsToDisplay.Length; i += this.StatsInRow)
		{
			List<StatBox.StatType> statsToDisplay = this.StatsToDisplay.Skip(i).Take(this.StatsInRow).ToList<StatBox.StatType>();
			float y = -(num3 + this.statBoxPadding.y) * (float)(i / this.StatsInRow);
			this.CreateStatRow(new Vector2(num2 + this.statBoxPadding.x, y), scale, boxWidth2, statsToDisplay);
		}
		this.PositionShareButton(num2, num3);
	}

	private void CreateStatRow(Vector2 offset, float scale, float boxWidth, List<StatBox.StatType> statsToDisplay)
	{
		for (int i = 0; i < statsToDisplay.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.StatListItemPrefab) as GameObject;
			StatBox component = gameObject.GetComponent<StatBox>();
			gameObject.transform.SetParent(this.StatsPlacement, false);
			gameObject.transform.localPosition = new Vector3(this.statBoxPadding.x + offset.x * (float)i, offset.y, 0f);
			gameObject.transform.localScale = new Vector3(scale, scale, 1f);
			component.ResizeToWidth(boxWidth);
			component.SetStat(statsToDisplay[i]);
			component.SetTint(this.tintColor);
			this.StatBoxes.Add(component);
		}
	}

	private void ShowRaceButton(bool show)
	{
        //this.GoRaceButton.Hide(!show);
		this.GoRaceButton.gameObject.SetActive(show);
	}

	private bool SetupCarsOwnedList()
	{
		StatsPlayerInfoComponent component = MultiplayerProfileScreen.Info.GetComponent<StatsPlayerInfoComponent>();
		List<string> getTop10CarsDBKey = component.GetTop10CarsDBKey;
		List<int> getTop10CarsPPIndex = component.GetTop10CarsPPIndex;
		if (getTop10CarsDBKey.Count == 0 || getTop10CarsPPIndex.Count == 0)
		{
			return false;
		}
		if (component.CarsOwned < getTop10CarsDBKey.Count)
		{
			return false;
		}
		if (getTop10CarsDBKey.Count != getTop10CarsPPIndex.Count)
		{
			return false;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(this.StatListItemCarListPrefab) as GameObject;
		gameObject.transform.parent = this.carListParent;
		gameObject.transform.localPosition = Vector3.zero;
		this.carListItem = gameObject.GetComponent<StatListItemExtra>();
		this.carListItem.list.UseDivider = true;
		int entryCount = 5;
		int num = getTop10CarsDBKey.Count;
		if (num > 5)
		{
			num = 4;
		}
		this.carListItem.SetEntryCount(entryCount);
		for (int i = 0; i < num; i++)
		{
			string carNiceName = CarDatabase.Instance.GetCarNiceName(getTop10CarsDBKey[i]);
			this.carListItem.SetListEntry(i, carNiceName, getTop10CarsPPIndex[i].ToString());
		}
		if (num < component.CarsOwned)
		{
			this.carListItem.SetListEntry(num, string.Empty, string.Format(LocalizationManager.GetTranslation("TEXT_MP_PROFILE_X_MORE_CARS"), component.CarsOwned - num));
		}
		return true;
	}

	private void SetAvatarPreferenceInProfile(int selectionIndex)
	{
		int preferredCsrAvatarPicture = 0;
		AvatarPicture.eAvatarType eAvatarType = AvatarPicture.eAvatarType.NO_AVATAR;
		this.GetTypeAndAvatarPreferenceForIndex(selectionIndex, out eAvatarType, out preferredCsrAvatarPicture);
		PlayerProfileManager.Instance.ActiveProfile.PreferredCsrAvatarPicture = preferredCsrAvatarPicture;
		MultiplayerProfileScreen.Info.Persona.SetupAvatarPicture(this.AvatarPic);
	}

	private void SetNamePreferenceInProfile(int selectionIndex)
	{
		List<UserNameType> availableUserNameIndices = this.GetAvailableUserNameIndices();
		UserNameType name = availableUserNameIndices[selectionIndex];
		this.UpdateUsernameText(this.GetUsernameString(name));
	}

	private void UpdateUsernameText(string name)
	{
		this.userNameText.text = name;
	}

	private void UpdateAvatar()
	{
		MultiplayerProfileScreen.Info.Persona.SetupAvatarPicture(this.AvatarPic);
	}

	private void CreateRankText()
	{
		RTWPlayerInfoComponent component = MultiplayerProfileScreen.Info.GetComponent<RTWPlayerInfoComponent>();
		this.RankText.text = CurrencyUtils.GetRankPointsString(component.RankPoints, true, false);
	}

	public override void RequestBackup()
	{
		if (this.ScreenLocked)
		{
			return;
		}
		if (MultiplayerProfileScreen.isOwnProfile)
		{
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}
		this.ScreenLocked = true;
		base.RequestBackup();
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		TouchManager.Instance.GesturesEnabled = this.previousGestureState;
	}

	public void OnRace()
	{
		if (this.ScreenLocked)
		{
			return;
		}
		if (!PlayerProfileManager.Instance.ActiveProfile.HasCompletedAnOnlineRace())
		{
			Log.AnEvent(Events.SelectOpponent1);
		}
		VSDummy.BeginRace(MultiplayerProfileScreen.ReplayData, VSDummy.eVSMode.Multiplayer);
		MultiplayerUtils.SetUpReplayData(MultiplayerProfileScreen.ReplayData.PlayerReplay);
		PlayerListScreen.LastSelectedReplayData = MultiplayerProfileScreen.ReplayData;
		this.ScreenLocked = true;
	}

	private string[] GetAvatarPickerItems(List<AvatarPicture.eAvatarType> availableAvatars)
	{
		string[] array = new string[availableAvatars.Count];
		for (int i = 0; i < array.Length; i++)
		{
			int num = 0;
			AvatarPicture.eAvatarType eAvatarType = AvatarPicture.eAvatarType.NO_AVATAR;
			this.GetTypeAndAvatarPreferenceForIndex(i, out eAvatarType, out num);
			if (eAvatarType == AvatarPicture.eAvatarType.CSR_AVATAR)
			{
				array[i] = string.Format(this.GetAvatarString(availableAvatars[i]), num);
			}
			else
			{
				array[i] = this.GetAvatarString(availableAvatars[i]);
			}
		}
		return array;
	}

	private bool FacebookInList()
	{
		List<AvatarPicture.eAvatarType> availableAvatars = this.GetAvailableAvatars();
		return availableAvatars.Contains(AvatarPicture.eAvatarType.FACEBOOK_AVATAR);
	}

	private bool GameCenterInList()
	{
		List<AvatarPicture.eAvatarType> availableAvatars = this.GetAvailableAvatars();
		return availableAvatars.Contains(AvatarPicture.eAvatarType.GAME_CENTER_AVATAR);
	}

	private bool GooglePlayGamesInList()
	{
		List<AvatarPicture.eAvatarType> availableAvatars = this.GetAvailableAvatars();
		return availableAvatars.Contains(AvatarPicture.eAvatarType.GOOGLE_PLAY_GAMES_AVATAR);
	}

	private bool GetTypeAndAvatarPreferenceForIndex(int index, out AvatarPicture.eAvatarType aType, out int aPreference)
	{
		List<AvatarPicture.eAvatarType> availableAvatars = this.GetAvailableAvatars();
		if (index < availableAvatars.Count)
		{
			aType = availableAvatars[index];
			aPreference = index + 1;
			if (this.FacebookInList())
			{
				aPreference--;
			}
			if (this.GameCenterInList())
			{
				aPreference--;
			}
			if (this.GooglePlayGamesInList())
			{
				aPreference--;
			}
			return true;
		}
		aType = AvatarPicture.eAvatarType.NO_AVATAR;
		aPreference = 0;
		return false;
	}

	private List<AvatarPicture.eAvatarType> GetAvailableAvatars()
	{
		List<AvatarPicture.eAvatarType> list = new List<AvatarPicture.eAvatarType>();
		if (this.IsAvatarAvailable(AvatarPicture.eAvatarType.FACEBOOK_AVATAR))
		{
			list.Add(AvatarPicture.eAvatarType.FACEBOOK_AVATAR);
		}
		if (this.IsAvatarAvailable(AvatarPicture.eAvatarType.GAME_CENTER_AVATAR))
		{
			list.Add(AvatarPicture.eAvatarType.GAME_CENTER_AVATAR);
		}
		if (this.IsAvatarAvailable(AvatarPicture.eAvatarType.GOOGLE_PLAY_GAMES_AVATAR))
		{
			list.Add(AvatarPicture.eAvatarType.GOOGLE_PLAY_GAMES_AVATAR);
		}
		for (int i = 0; i < AvatarPicture.NumberOfCSRAvatars; i++)
		{
			list.Add(AvatarPicture.eAvatarType.CSR_AVATAR);
		}
		return list;
	}

	private string GetAvatarString(AvatarPicture.eAvatarType avatar)
	{
		if (avatar == AvatarPicture.eAvatarType.CSR_AVATAR)
		{
			return LocalizationManager.GetTranslation("TEXT_PROFILE_DEFAULT_PIC");
		}
		if (avatar == AvatarPicture.eAvatarType.FACEBOOK_AVATAR)
		{
			return LocalizationManager.GetTranslation("TEXT_PROFILE_FACEBOOK_PIC");
		}
		if (avatar == AvatarPicture.eAvatarType.GAME_CENTER_AVATAR)
		{
			return LocalizationManager.GetTranslation(LocalizationManager.GetTranslation("TEXT_PROFILE_GC_PIC"));
		}
		if (avatar == AvatarPicture.eAvatarType.GOOGLE_PLAY_GAMES_AVATAR)
		{
			return LocalizationManager.GetTranslation("TEXT_PROFILE_GC_PIC_ANDROID");
		}
		return string.Empty;
	}

	private string[] GetUsernamePickerItems(List<UserNameType> availableNames)
	{
		string[] array = new string[availableNames.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = string.Format(this.GetUsernamePrefix(availableNames[i]), this.GetUsernameString(availableNames[i]));
		}
		return array;
	}

	private List<UserNameType> GetAvailableUserNameIndices()
	{
		List<UserNameType> list = new List<UserNameType>();
		if (this.IsUsernameAvailable(UserNameType.Facebook))
		{
			list.Add(UserNameType.Facebook);
		}
		if (this.IsUsernameAvailable(UserNameType.Gamecenter))
		{
			list.Add(UserNameType.Gamecenter);
		}
		list.Add(UserNameType.CSR);
		return list;
	}

	private string GetUsernameString(UserNameType name)
	{
		switch (name)
		{
		case UserNameType.Default:
			return PlayerProfileManager.Instance.ActiveProfile.DisplayNameWithUserNameFallback();
		case UserNameType.Facebook:
			if (NameValidater.CanNameBeDisplayedInCurrentLanguage(SocialController.Instance.facebookName))
			{
				return SocialController.Instance.facebookName;
			}
			return PlayerProfileManager.Instance.ActiveProfile.DisplayNameWithUserNameFallback();
		case UserNameType.Gamecenter:
			if (GameCenterController.Instance.currentAliasCanBeDisplayed())
			{
				return GameCenterController.Instance.currentAlias();
			}
			return PlayerProfileManager.Instance.ActiveProfile.DisplayNameWithUserNameFallback();
		case UserNameType.CSR:
			return MultiplayerProfileScreen.Info.CsrUserName;
		}
		return string.Empty;
	}

	private string GetUsernamePrefix(UserNameType name)
	{
		if (name == UserNameType.CSR)
		{
			return LocalizationManager.GetTranslation("TEXT_PROFILE_DEFAULT_NAME");
		}
		if (name == UserNameType.Facebook)
		{
			return LocalizationManager.GetTranslation("TEXT_PROFILE_FACEBOOK_NAME");
		}
		if (name == UserNameType.Gamecenter)
		{
			return LocalizationManager.GetTranslation(LocalizationManager.GetTranslation("TEXT_PROFILE_GC_NAME"));
		}
		return string.Empty;
	}

	private bool IsUsernameAvailable(UserNameType name)
	{
		if (name == UserNameType.CSR)
		{
			return true;
		}
		if (name == UserNameType.Facebook)
		{
			return SocialController.Instance.isLoggedIntoFacebook && !string.IsNullOrEmpty(SocialController.Instance.facebookName);
		}
		return name == UserNameType.Gamecenter && GameCenterController.Instance.isPlayerLoggedIn();
	}

	private bool IsAvatarAvailable(AvatarPicture.eAvatarType avatar)
	{
		if (avatar == AvatarPicture.eAvatarType.CSR_AVATAR)
		{
			return true;
		}
		if (avatar == AvatarPicture.eAvatarType.FACEBOOK_AVATAR)
		{
			return SocialController.Instance.isLoggedIntoFacebook;
		}
		if (avatar == AvatarPicture.eAvatarType.GAME_CENTER_AVATAR)
		{
			return GameCenterController.Instance.isPlayerLoggedInAndGameCenterPicsAvailable();
		}
#if UNITY_ANDROID
        return avatar == AvatarPicture.eAvatarType.GOOGLE_PLAY_GAMES_AVATAR && GooglePlayGamesController.Instance.IsPlayerAuthenticated();
#endif
	    return false;
	}

	public void EnableClickPrevention()
	{
		this.StopOutsideClicksButton.gameObject.SetActive(true);
	}

	public void DisableClickPrevention()
	{
		this.StopOutsideClicksButton.gameObject.SetActive(false);
	}

	public void ShareProfile()
	{
		this.fullScreenFlash.StartFlashAnimation();
		EventManager.Instance.PostEvent("Snapshot", EventAction.PlaySound, null, null);
		CameraPostRender.Instance.AddProcess("Profile render profile share snapshot", delegate
		{
			Texture2D texture2D = this.ProfileSharer.Render();
			byte[] bytes = texture2D.EncodeToPNG();
			File.WriteAllBytes(this.ProfileSnapshotPath, bytes);
			base.StartCoroutine(this.ShareProfileImage());
		});
	}

	private IEnumerator ShareProfileImage()
	{
	    string message;
                Keyframe keyframe = fullScreenFlash.ExplosionCurve[fullScreenFlash.ExplosionCurve.length - 1];
                yield return new WaitForSeconds(keyframe.time);
                fullScreenFlash.StopFlashAnimation();
                message = LocalizationManager.GetTranslation("TEXT_TWITTER_STAT_SHARE");
                BasePlatform.ActivePlatform.ShareImage(string.Format(message.Replace("{LINK}", SocialController.Instance.GetTwitterGameURL()), BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty, ProfileSnapshotPath);
	}

	protected override void Update()
	{
		if (this.lastAvatar != this.currentAvatar)
		{
			this.SetAvatarPreferenceInProfile(this.currentAvatar);
			this.lastAvatar = this.currentAvatar;
		}
		if (this.ProfileSharer.IsReadyToShare())
		{
			RuntimeTextButton componentInChildren = this.ShareButton.GetComponentInChildren<RuntimeTextButton>();
			if (componentInChildren != null)
			{
				componentInChildren.CurrentState = BaseRuntimeControl.State.Active;
			}
		}
		base.Update();
	}

	protected override void OnDestroy()
	{
		if (this.ProfileSharer != null)
		{
			UnityEngine.Object.Destroy(this.ProfileSharer.gameObject);
		}
	}
}
