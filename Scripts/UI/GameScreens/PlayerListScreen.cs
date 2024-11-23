using Metrics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListScreen : ZHUDScreen
{
	public enum PlayerListFilter
	{
		All,
		Friends,
		Challenges,
		NonPlayers
	}

	private const int StreakNodes = 6;

	private const int ItemsInRow = 3;

	private const string RTWCall = "rtw_match_123";

	private const int RTWCarListLength = 20;

	private const string itemSlideLeft = "PlayerListItemSlideLeft";

	private const string itemSlideRight = "PlayerListItemSlideRight";

	public GameObject BankHeirarchy;

	public RuntimeTextButton BankButton;

    public RuntimeTextButton HireTeamButton;

	public GameObject PlayerListItemPrefab;

	public GameObject PlayerListNode;

	private List<PlayerListItemRow> PlayerList = new List<PlayerListItemRow>();

	public Texture2D[] ueMenuTextures;

	public float TopBarHeight;

	public float BottomBarHeight;

	public TextMeshProUGUI StatusText;

	public TextMeshProUGUI RaceTypeText;

	public GameObject LoadingNode;

	public Image BackBar;

	public static List<int> PlayerPPDeltaForMetrics = new List<int>();

	private int EntryFeeCash;

	private int EntryFeeGold;

	public GameObject RTWIcon;

	public GameObject EliteIcon;

	public GameObject EventIcon;

	public GameObject RTWWatermark;

	public GameObject EliteWatermark;

	public GameObject EventWatermark;

	private bool requestingMatches;

	private static List<CachedOpponentInfo> _replays = new List<CachedOpponentInfo>();

	public static PlayerListScreen.PlayerListFilter CurrentFilter = PlayerListScreen.PlayerListFilter.All;

	public static CachedOpponentInfo LastSelectedReplayData;

	private FlowConditionalBase screenConditional;

	public float SnapshotWaitTime = 2f;

	private float snapshotTimeWaited;

	private bool waitingForSnapshots;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.PlayerList;
		}
	}

	protected override void Awake()
	{
		this.screenConditional = new PlayerListScreenConditional();
		base.Awake();
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		switch (MultiplayerUtils.SelectedMultiplayerMode)
		{
		case MultiplayerMode.RACE_THE_WORLD:
            //ScreenManager.Instance.SetupBackground(BackgroundManager.BackgroundType.Gradient);
            //this.RTWWatermark.SetActive(true);
            //this.SetWatermarkTint(this.RTWWatermark, StreakManager.StreakData.RaceTheWorldInfo.Theme.GetSwatch().Primary);
			goto IL_DD;
		case MultiplayerMode.PRO_CLUB:
            //ScreenManager.Instance.SetupBackground(BackgroundManager.BackgroundType.Gradient);
			this.EliteWatermark.SetActive(true);
			this.SetWatermarkTint(this.EliteWatermark, StreakManager.StreakData.EliteClubInfo.Theme.GetSwatch().Primary);
			goto IL_DD;
		}
        //ScreenManager.Instance.SetupBackground(BackgroundManager.BackgroundType.Gradient);
		this.EventWatermark.SetActive(true);
		this.SetWatermarkTint(this.EventWatermark, MultiplayerEvent.Saved.Data.Theme.GetSwatch().Primary);
		IL_DD:
        //this.BackBar.SetSize(GUICamera.Instance.ScreenWidth, 0.64f);
		this.HideAllButtons();
		TouchManager.Instance.GesturesEnabled = true;
		this.requestingMatches = false;
        //this._name = this.ID.ToString().ToUpper();
		this.LoadingNode.SetActive(false);
		this.screenConditional.EvaluateAll();
		if (!zAlreadyOnStack)
		{
			if (StreakManager.OpponentListIsEmpty() || StreakManager.AllOpponentsDefeated())
			{
				StreakManager.ResetStreak();
				StreakManager.ResetBank();
				this.RebuildPlayerListFromWebRequest();
			}
			else
			{
				this.RebuildPlayerListFromStreakCacheOrWebRequest();
			}
		}
		else
		{
			this.RebuildPlayerListFromStreakCacheOrWebRequest();
			this.ShowAllButtons();
		}
		this.UpdateStreakInfo();
		CommonUI.Instance.SetRankBars(eRankBarMode.MULTIPLAYER_RANK, true);
		CommonUI.Instance.RPBonusStats.SetRPMultiplier(RPBonusManager.NavBarValue(), true);
		base.OnActivate(zAlreadyOnStack);
	}

	private void SetWatermarkTint(GameObject watermark, Color themeColor)
	{
		Image[] componentsInChildren = watermark.GetComponentsInChildren<Image>();
		Image[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Image sprite = array[i];
			themeColor.a = sprite.color.a;
            sprite.color = themeColor;
		}
	}

	public void SetHireTeamEnabled(bool val)
	{
		this.HireTeamButton.gameObject.SetActive(val);
		this.HireTeamButton.CurrentState = ((!val) ? BaseRuntimeControl.State.Hidden : BaseRuntimeControl.State.Active);
	}

	public void SetBankEnabled(bool val)
	{
        //this.BankHeirarchy.SetActive(val);
		this.BankButton.CurrentState = ((!val) ? BaseRuntimeControl.State.Hidden : BaseRuntimeControl.State.Active);
	}

	public void HideTheBackButton()
	{
        //CommonUI.Instance.NavBar.HideBackButton();
	}

	protected void PopulateItemLists()
	{
		foreach (PlayerListItem current in this.PlayerList.SelectMany((PlayerListItemRow p) => p.playerListItems))
		{
			UnityEngine.Object.Destroy(current);
		}
		this.PlayerList.Clear();
		this.PopulatePlayerList();
		this.PositionPlayerListItems();
	}

	private void PopulatePlayerList()
	{
		bool flag = true;
		for (int i = 0; i < PlayerListScreen._replays.Count<CachedOpponentInfo>(); i += 3)
		{
			IEnumerable<CachedOpponentInfo> replays = PlayerListScreen._replays.Skip(i).Take(3);
			if (flag)
			{
				this.AddPlayerListRow(PlayerListItemRow.SlideDirection.Left, replays);
			}
			else
			{
				this.AddPlayerListRow(PlayerListItemRow.SlideDirection.Right, replays);
			}
			flag = !flag;
		}
	}

	private void AddPlayerListRow(PlayerListItemRow.SlideDirection direction, IEnumerable<CachedOpponentInfo> replays)
	{
		List<PlayerListItem> list = new List<PlayerListItem>();
		foreach (CachedOpponentInfo current in replays)
		{
			PlayerListItem playerListItem = this.CreatePlayerListItem(current, true);
			if (playerListItem != null)
			{
				list.Add(playerListItem);
			}
		}
		PlayerListItemRow playerListItemRow = new PlayerListItemRow();
		playerListItemRow.Setup(direction, list);
		this.PlayerList.Add(playerListItemRow);
	}

	private PlayerListItem CreatePlayerListItem(CachedOpponentInfo zPlayerReplay, bool showPP)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.PlayerListItemPrefab) as GameObject;
		gameObject.transform.parent = this.PlayerListNode.transform;
		gameObject.transform.localPosition = Vector3.zero;
		PlayerListItem component = gameObject.GetComponent<PlayerListItem>();
		if (component != null)
		{
			component.CreateFromReplay(zPlayerReplay, showPP);
			component.OnTap += new OnItemTap(this.OnItemClick);
		}
		return component;
	}

	private void PositionPlayerListItems()
	{
		if (this.PlayerList.Count == 0)
		{
			return;
		}
        //WindowPaneBatched componentInChildren = this.PlayerList[0].playerListItems[0].GetComponentInChildren<WindowPaneBatched>();
        //for (int i = 0; i < this.PlayerList.Count; i++)
        //{
        //    List<PlayerListItem> playerListItems = this.PlayerList[i].playerListItems;
        //    Vector2 vector = new Vector2(GUICamera.Instance.ScreenWidth, GUICamera.Instance.ScreenHeight);
        //    float num = vector.x - componentInChildren.Width * 3f;
        //    float num2 = Mathf.Min(vector.x / 50f, num / 4f);
        //    float num3 = (vector.x - componentInChildren.Width * 3f - num2 * 4f) / 2f;
        //    float num4 = (vector.y - componentInChildren.Height * (float)this.PlayerList.Count - num2 * (float)(this.PlayerList.Count + 1)) / 2f;
        //    float y = vector.y / 2f - num4 - num2 * (float)(i + 1) - componentInChildren.Height * (float)i;
        //    for (int j = 0; j < this.PlayerList[i].playerListItems.Count; j++)
        //    {
        //        float num5 = (float)(j % 3);
        //        float x = -vector.x / 2f + num3 + num2 * (num5 + 1f) + componentInChildren.Width * num5;
        //        playerListItems[j].transform.localPosition = new Vector3(x, y, 0f);
        //    }
        //}
	}

	private void AssignAnimationsToItems()
	{
		foreach (PlayerListItemRow current in this.PlayerList)
		{
			current.AssignAnimations();
		}
	}

	private void StartListItemAnimations()
	{
		this.snapshotTimeWaited = 0f;
		this.waitingForSnapshots = true;
	}

	private void RebuildPlayerListFromStreakCacheOrWebRequest()
	{
		if (StreakManager.CachedOpponents().Count == 0)
		{
			this.RebuildPlayerListFromWebRequest();
		}
		else
		{
			this.RebuildPlayerListFromStreakCache();
		}
	}

	private void RebuildPlayerListFromStreakCache()
	{
		this.ClearReplayDataModels();
		foreach (CachedOpponentInfo current in StreakManager.CachedOpponents())
		{
			PlayerListScreen._replays.Add(current);
			RacePlayerInfoComponent component = current.PlayerReplay.playerInfo.GetComponent<RacePlayerInfoComponent>();
			PlayerListScreen.PlayerPPDeltaForMetrics.Add(component.PPIndex);
		}
		this.SetStatusText(string.Empty, false);
		this.PopulateItemLists();
		this.LoadingNode.SetActive(false);
		this.ShowAllButtons();
	}

	private void ClearReplayDataModels()
	{
		PlayerListScreen._replays.Clear();
		PlayerListScreen.PlayerPPDeltaForMetrics.Clear();
	}

	private List<CarInfo> GetEligibleMultiplayerCars(CarGarageInstance currentCar)
	{
		float bossCarProbability = 0f;
		ModeInfo modeCostInfo = StreakManager.GetModeCostInfo(RaceEventInfo.Instance.CurrentEvent);
		if (modeCostInfo != null)
		{
			bossCarProbability = modeCostInfo.BossCarProbability;
		}
		List<CarInfo> list = new List<CarInfo>();
		eCarTier currentTier = currentCar.CurrentTier;
		CarDatabase instance = CarDatabase.Instance;
		int currentPPIndex = currentCar.CurrentPPIndex;
		int num;
		int num2;
		CarPerformanceIndexCalculator.GetPPRangeForCarTier(currentTier, out num, out num2);
		float num3 = 1f;
		if (num2 != num)
		{
			num3 = Mathf.Clamp01((float)(currentPPIndex - num) / (float)(num2 - num));
		}
		int num4 = Mathf.Clamp(Mathf.FloorToInt(num3 * 3f), 0, 2);
		list.AddRange(instance.GetCarsOfTier(currentTier));
		if (currentTier > eCarTier.TIER_1 && num4 == 0)
		{
			list.AddRange(instance.GetCarsOfTier(currentTier - 1));
			list.AddRange(instance.GetCarsOfTier(currentTier));
		}
		if (currentTier < eCarTier.TIER_5 && num4 == 2)
		{
			list.AddRange(instance.GetCarsOfTier(currentTier + 1));
			list.AddRange(instance.GetCarsOfTier(currentTier));
		}
		MultiplayerMode selectedMultiplayerMode = MultiplayerUtils.SelectedMultiplayerMode;
		if (selectedMultiplayerMode != MultiplayerMode.PRO_CLUB)
		{
			if (selectedMultiplayerMode == MultiplayerMode.EVENT)
			{
				RaceEventData raceData = new RaceEventData();
				raceData.Restrictions = MultiplayerEvent.Saved.Data.Restrictions;
				list.RemoveAll(delegate(CarInfo car)
				{
					RaceEventRestriction.RestrictionMet restrictionMet = raceData.DoesMeetRestrictions(car);
					return restrictionMet == RaceEventRestriction.RestrictionMet.FALSE || restrictionMet == RaceEventRestriction.RestrictionMet.UNKNOWN;
				});
			}
		}
		else
		{
			list.RemoveAll((CarInfo car) => !car.SupportsElite());
		}
		System.Random rng = new System.Random();
		List<CarInfo> list2 = (from x in list
		where x.IsAvailableInMultiplayer()
		where !CarDataDefaults.IsBossCar(x.Key) || rng.NextDouble() < (double)bossCarProbability
		select x).ToList<CarInfo>();
		if (list2.Count<CarInfo>() == 0)
		{
			list2.Add(CarDatabase.Instance.GetCar(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey));
		}
		else
		{
			list2.Shuffle<CarInfo>();
		}
		return list2.GetRange(0, Math.Min(list2.Count, 20));
	}

	private void RebuildPlayerListFromWebRequest()
	{
		this.ClearReplayDataModels();
		if (!this.StatusText.gameObject.activeInHierarchy)
		{
			this.SetStatusText("TEXT_CONTACTING_MULTIPLAYER_SERVERS", true);
			this.LoadingNode.SetActive(true);
		}
		if (!ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			if (!ServerSynchronisedTime.Instance.RequestServerTime(new ServerSynchronisedTime.RequestCallback(this.OnServerTimeRequestFinished)))
			{
				this.DisplayNetworkErrorDialog();
			}
			return;
		}
		if (PlayerProfileManager.Instance.ActiveProfile != null)
		{
			PlayerProfileManager.Instance.ActiveProfile.UpdateConsumablesFromNetworkTime();
		}
		JsonDict param = new JsonDict();
		param.Set("uid", UserManager.Instance.currentAccount.UserID.ToString());
		DifficultyManager.OnStreakStarted();
		List<float> matches = DifficultyManager.GetMatches(DifficultyManager.CurrentEventType);
		int count = matches.Count;
		for (int i = 0; i < count; i++)
		{
			float num = matches[i];
			param.Set("replay_time_" + i, num.ToString(CultureInfo.InvariantCulture));
		}
		PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_EliteClubCompleted = (MultiplayerUtils.SelectedMultiplayerMode == MultiplayerMode.PRO_CLUB);
		param.Set("elite", (MultiplayerUtils.SelectedMultiplayerMode != MultiplayerMode.PRO_CLUB) ? "0" : "1");
		param.Set("wt", "0");
		param.Set("mpt", UserManager.Instance.currentAccount.MPToken);
		param.Set("platform", BasePlatform.TargetPlatform.ToString());
		param.Set("app_ver", ApplicationVersion.Current);
		param.Set("replay_version", GameDatabase.Instance.OnlineConfiguration.NetworkReplayVersion);
		CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
		param.Set("current_car_id", currentCar.CarDBKey);
		List<CarInfo> eligibleMultiplayerCars = this.GetEligibleMultiplayerCars(currentCar);
		param.Set("num_car_ids", eligibleMultiplayerCars.Count.ToString());
		eligibleMultiplayerCars.ForEachWithIndex(delegate(CarInfo car, int index)
		{
			param.Set("car_id_" + index.ToString(), car.Key);
		});
		if (!PolledNetworkState.IsNetworkConnected)
		{
			this.DisplayNetworkErrorDialog();
		}
		else
		{
			this.HideAllButtons();
			WebRequestQueueRTW.Instance.RemoveItems("rtw_match_123");
			Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
			{
				{
					Parameters.StreakType,
					MultiplayerUtils.GetMultiplayerStreakType()
				}
			};
			Log.AnEvent(Events.RequestMatches, data);
			this.requestingMatches = true;
			WebRequestQueueRTW.Instance.StartCall("rtw_match_123", "Finding replays", param, new WebClientDelegate2(this.OnPlayerMatchResults), null, string.Empty, 1);
		}
		PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
		CarSnapshotManager.Instance.ClearSnapshotsOfType(CarSnapshotType.PlayerList);
	}

	private void OnServerTimeRequestFinished(bool requestSuccessfull, DateTime serverTime)
	{
		if (ScreenManager.Instance.CurrentScreen != ScreenID.PlayerList)
		{
			return;
		}
		PlayerListScreen x = ScreenManager.Instance.ActiveScreen as PlayerListScreen;
		if (x == null)
		{
			return;
		}
		if (!requestSuccessfull)
		{
			this.DisplayNetworkErrorDialog();
		}
		else
		{
			this.RebuildPlayerListFromWebRequest();
			StreakManager.Chain.UpdateTimeout();
		}
	}

	private void DecideToAllowBankOrRefresh()
	{
		if (StreakManager.CurrentStreak() > 0)
		{
			this.SetBankEnabled(true);
			this.HideTheBackButton();
		}
		else
		{
			this.SetBankEnabled(false);
		}
	}

	public void OnPlayerMatchResults(string content, string error, int status, object userData)
	{
		if (ScreenManager.Instance.CurrentScreen != ScreenID.PlayerList)
		{
			return;
		}
		PlayerListScreen x = ScreenManager.Instance.ActiveScreen as PlayerListScreen;
		if (x == null)
		{
			return;
		}
		this.requestingMatches = false;
		if (status == 200 && error == null)
		{
			if (!string.IsNullOrEmpty(content))
			{
				JsonDict jsonDict = new JsonDict();
				if (!jsonDict.Read(content))
				{
					this.DisplayNetworkErrorDialog();
					return;
				}
				JsonList jsonList = jsonDict.GetJsonList("player_list");
				RtwLeaderboardStatus newStatus = RtwLeaderboardStatus.FromDict(jsonDict);
				RtwLeaderboardStandings newStandings = RtwLeaderboardStandings.FromDict(jsonDict);
				SeasonServerDatabase.Instance.SetLeaderboardStandings(newStandings, newStatus);
				if (SeasonFlowManager.Instance.OnReceivedPlayerList())
				{
					int count = jsonList.Count;
					if (count <= 0)
					{
						this.DisplayNetworkErrorDialog();
						return;
					}
					PlayerListScreen.PlayerPPDeltaForMetrics.Clear();
					int num = 0;
					for (int i = 0; i < count; i++)
					{
						JsonDict jsonDict2 = jsonList.GetJsonDict(i);
						string @string = jsonDict2.GetString("replay_data");
						PlayerReplay playerReplay = PlayerReplay.CreateFromJson(@string, null);
						if (playerReplay != null && num < StreakManager.NumberOfOpponents())
						{
							RTWPlayerInfoComponent component = playerReplay.playerInfo.GetComponent<RTWPlayerInfoComponent>();
							RacePlayerInfoComponent component2 = playerReplay.playerInfo.GetComponent<RacePlayerInfoComponent>();
							component.MatchId = jsonDict2.GetString("match_id");
							CachedOpponentInfo cachedOpponentInfo = default(CachedOpponentInfo);
							cachedOpponentInfo.PlayerReplay = playerReplay;
							cachedOpponentInfo.EntryID = num;
							cachedOpponentInfo.Animated = false;
							cachedOpponentInfo.Defeated = false;
							PlayerListScreen._replays.Add(cachedOpponentInfo);
							StreakManager.AddOpponent(cachedOpponentInfo);
							PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
							int currentPPIndex = activeProfile.GetCurrentCar().CurrentPPIndex;
							PlayerListScreen.PlayerPPDeltaForMetrics.Add(component2.PPIndex - currentPPIndex);
							num++;
						}
					}
					if (num == 0)
					{
						this.DisplayNetworkErrorDialog();
						return;
					}
					this.PopulateItemLists();
					this.AssignAnimationsToItems();
					this.StartListItemAnimations();
					this.ShowAllButtons();
					if (!StreakManager.Chain.Active())
					{
						this.EntryFeeCash = StreakManager.CurrentEntryFeeCash();
						this.EntryFeeGold = StreakManager.CurrentEntryFeeGold();
						PlayerProfileManager.Instance.ActiveProfile.SpendCash(this.EntryFeeCash,"Streak","Streak");
						PlayerProfileManager.Instance.ActiveProfile.SpendGold(this.EntryFeeGold,"Streak","Streak");
						string value = "RTWEntry";
						if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldWorldTourEvent())
						{
							value = "WTEntry";
						}
						else if (RaceEventInfo.Instance.CurrentEvent.IsOnlineClubRacingEvent())
						{
							value = "ProEntry";
						}
						Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
						{
							{
								Parameters.Type,
								value
							},
							{
								Parameters.CostGold,
								this.EntryFeeGold.ToString()
							},
							{
								Parameters.CostCash,
								this.EntryFeeCash.ToString()
							},
							{
								Parameters.DGld,
								(-this.EntryFeeGold).ToString()
							},
							{
								Parameters.DCsh,
								(-this.EntryFeeCash).ToString()
							},
							{
								Parameters.StreakType,
								MultiplayerUtils.GetMultiplayerStreakType()
							}
						};
						Log.AnEvent(Events.MultiplayerPurchase, data);
						if (this.EntryFeeCash > 0 || this.EntryFeeGold > 0)
						{
							MenuAudio.Instance.playSound(AudioSfx.Purchase);
						}
					}
					Log.AnEvent(Events.WinStreak, new Dictionary<Parameters, string>
					{
						{
							Parameters.StreakType,
							MultiplayerUtils.GetMultiplayerStreakType()
						}
					});
					PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
					if (!MultiplayerUtils.HasPlayedAtLeastOneMultiplayerRace())
					{
						PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PLAYER_LIST_INTRO_TITLE", "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PLAYER_LIST_INTRO_BODY", new PopUpButtonAction(this.OnDismissMultiplayerPlayerListIntroPopup), false);
						Log.AnEvent(Events.GoRaceMP);
					}
				}
			}
			else
			{
				this.Bankit();
			}
		}
		else
		{
			this.DisplayNetworkErrorDialog();
			RTWStatusManager.Instance.ForcePollServer();
		}
	}

	private void OnDismissMultiplayerPlayerListIntroPopup()
	{
		Log.AnEvent(Events.PlayerlistOK);
	}

	protected override void Update()
	{
		base.Update();
		StreakManager.Chain.CheckBonusTimeOut();
		if (!this.requestingMatches && !this.IsInMultiplayerTutorial() && this.IsInAStreak())
		{
			FlowConditionBase nextValidCondition = this.screenConditional.GetNextValidCondition();
			if (nextValidCondition != null)
			{
				PopUp popup = nextValidCondition.GetPopup();
				bool flag = PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
			}
		}
		if (this.waitingForSnapshots)
		{
			this.snapshotTimeWaited += Time.deltaTime;
			bool flag2 = this.PlayerList.All((PlayerListItemRow row) => row.playerListItems.All((PlayerListItem item) => item.SnapshotIsLoaded));
			if (this.snapshotTimeWaited >= this.SnapshotWaitTime || flag2)
			{
				AudioManager.Instance.PlaySound("Reward_StarSwoosh", null);
				foreach (PlayerListItemRow current in this.PlayerList)
				{
					current.StartPlayingSlideAnimations();
				}
				this.LoadingNode.SetActive(false);
				this.waitingForSnapshots = false;
			}
		}
	}

	public void SetStatusText(string text, bool ShouldTranslate)
	{
		if (string.IsNullOrEmpty(text))
		{
			this.StatusText.gameObject.SetActive(false);
			return;
		}
		this.StatusText.gameObject.SetActive(true);
		this.StatusText.text = ((!ShouldTranslate) ? text : LocalizationManager.GetTranslation(text));
	}

	protected void OnItemClick(PlayerListItem zItem)
	{
        //if (this.CurrentAnimState != CSRScreen.AnimState.IN)
        //{
        //    return;
        //}
		this.StartRace(zItem);
	}

	private void StartRace(PlayerListItem item)
	{
		if (item != null)
		{
			PlayerListItem.ActionType lastUsedActionType = item.LastUsedActionType;
			if (lastUsedActionType == PlayerListItem.ActionType.Race)
			{
				if (!PlayerProfileManager.Instance.ActiveProfile.HasCompletedAnOnlineRace())
				{
					Log.AnEvent(Events.SelectOpponent1);
				}
                //CommonUI.Instance.NavBar.DisableBackButton();
                //CommonUI.Instance.NavBar.HideBackButton();
				VSDummy.BeginRace(item.CachedReplay, VSDummy.eVSMode.Multiplayer);
				MultiplayerUtils.SetUpReplayData(item.CachedReplay.PlayerReplay);
				PlayerListScreen.LastSelectedReplayData = item.CachedReplay;
			}
		}
	}

	public void UpdateStreakInfo()
	{
		switch (MultiplayerUtils.SelectedMultiplayerMode)
		{
		case MultiplayerMode.RACE_THE_WORLD:
			this.RaceTypeText.text = LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_RACETHEWORLD");
			this.RTWIcon.SetActive(true);
			break;
		case MultiplayerMode.PRO_CLUB:
            this.RaceTypeText.text = LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_CLUBRACING");
			this.EliteIcon.SetActive(true);
			break;
		case MultiplayerMode.EVENT:
            this.RaceTypeText.text = LocalizationManager.GetTranslation(MultiplayerEvent.Saved.Data.GetTitle()).ToUpper();
			this.EventIcon.SetActive(true);
			break;
		}
	}

	private void OnCancelBank()
	{
		if (!PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_SuccessfullyCompletedStreak)
		{
			Log.AnEvent(Events.UnBank);
		}
	}

	public void OnBankButtonClicked()
	{
		PopUpDatabase.Common.ShowStargazerPopupCancelConfirm("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_BANK_IT_CONFIRM_TITLE", "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_BANK_IT_CONFIRM_BODY", new PopUpButtonAction(this.OnCancelBank), new PopUpButtonAction(this.Bankit));
		if (!PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_SuccessfullyCompletedStreak)
		{
			Log.AnEvent(Events.ConfirmBank);
		}
	}

	private bool IsInMultiplayerTutorial()
	{
		return MultiplayerUtils.IsMultiplayerObjectiveActive() && MultiplayerUtils.HasNeverTakenAMultiplayerRace();
	}

	private bool IsInAStreak()
	{
		return StreakManager.CurrentStreak() != 0;
	}

	public override void RequestBackup()
	{
		if (this.IsInMultiplayerTutorial())
		{
			this.TutorialDontLeave();
		}
		else if (!this.IsInAStreak() && !StreakManager.Chain.Active())
		{
			this.Bankit();
		}
		else
		{
			this.DontLeaveMe();
		}
	}

	private void TutorialDontLeave()
	{
		PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PLAYER_LIST_NO_LEAVING_TITLE", "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PLAYER_LIST_NO_LEAVING_BODY", new PopUpButtonAction(this.OnTutorialDontLeave), false);
	}

	private void OnTutorialDontLeave()
	{
		if (this.requestingMatches)
		{
			Log.AnEvent(Events.BackOut);
		}
		else
		{
			Log.AnEvent(Events.HeyKid);
		}
	}

	private void DisplayNetworkErrorDialog()
	{
		if (ScreenManager.Instance.CurrentScreen != ScreenID.PlayerList)
		{
			return;
		}
		if (MultiplayerUtils.IsServerUnavailable())
		{
			PopUp popup = new PopUp
			{
				Title = "TEXT_POPUP_PLAYERLIST_NET_ERROR_TITLE",
				BodyText = "TEXT_MULTIPLAYER_UNAVAILABLE",
				IsBig = true,
				ConfirmAction = new PopUpButtonAction(this.OnNetworkErrorPopupDismissed),
				ConfirmText = "TEXT_POPUP_PLAYERLIST_NET_ERROR_CONFIRM"
			};
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
		}
		else if (StreakManager.Chain.Active())
		{
			PopUp popup2 = new PopUp
			{
				Title = "TEXT_POPUP_PLAYERLIST_NET_ERROR_TITLE",
				BodyText = "TEXT_POPUP_PLAYERLIST_NET_ERROR_STREAKCHAIN_BODY",
				IsBig = true,
				CancelAction = new PopUpButtonAction(this.OnNetworkErrorPopupRetry),
				CancelText = "TEXT_BUTTON_RETRY",
				ConfirmAction = new PopUpButtonAction(this.OnNetworkErrorPopupDismissed),
				ConfirmText = "TEXT_POPUP_PLAYERLIST_NET_ERROR_CONFIRM",
				GraphicPath = PopUpManager.Instance.graphics_stargazerPrefab,
				ImageCaption = "TEXT_NAME_FRANKIE",
				ShouldCoverNavBar = true
			};
			PopUpManager.Instance.TryShowPopUp(popup2, PopUpManager.ePriority.System, null);
		}
		else
		{
			PopUp popup3 = new PopUp
			{
				Title = "TEXT_POPUP_PLAYERLIST_NET_ERROR_TITLE",
				BodyText = "TEXT_POPUP_PLAYERLIST_NET_ERROR_BODY",
				IsBig = true,
				ConfirmAction = new PopUpButtonAction(this.OnNetworkErrorPopupDismissed),
				ConfirmText = "TEXT_POPUP_PLAYERLIST_NET_ERROR_CONFIRM"
			};
			PopUpManager.Instance.TryShowPopUp(popup3, PopUpManager.ePriority.System, null);
		}
	}

	private void OnNetworkErrorPopupDismissed()
	{
		this.Bankit();
	}

	private void OnNetworkErrorPopupRetry()
	{
		this.RebuildPlayerListFromWebRequest();
	}

	private void DontLeaveMe()
	{
		bool flag = StreakManager.CurrentStreak() > 0;
		string bodyText;
		if (StreakManager.Chain.Active())
		{
			bodyText = "TEXT_PLAYERLIST_BACKOUT_CHAIN_BODY";
		}
		else if (!flag && this.EntryFeeCash == 0 && this.EntryFeeGold == 0)
		{
			bodyText = "TEXT_PLAYERLIST_BACKOUT_NO_STREAKORFEE_BODY";
		}
		else
		{
			bodyText = (flag ? "TEXT_PLAYERLIST_BACKOUT_BODY" : "TEXT_PLAYERLIST_BACKOUT_NO_STREAK_BODY");
		}
		PopUp popup = new PopUp
		{
			Title = "TEXT_PLAYERLIST_BACKOUT",
			BodyText = bodyText,
			CancelAction = new PopUpButtonAction(this.CancelIt),
			ConfirmAction = new PopUpButtonAction(this.Bankit),
			CancelText = "TEXT_BUTTON_CANCEL",
			ConfirmText = "TEXT_BUTTON_LEAVE"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private void Bankit()
	{
		base.RequestBackup();
		WebRequestQueueRTW.Instance.RemoveItems("rtw_match_123");
		if (!PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_SuccessfullyCompletedStreak)
		{
			Log.AnEvent(Events.Bank);
		}
		if (StreakManager.HasBanked())
		{
			CommonUI.Instance.CashStats.CashLockedState(true);
		}
		StreakManager.UpdateMetricsOnBankit();
		StreakManager.BankIt();
		StreakManager.Chain.Reset();
		if (StreakManager.HasBanked())
		{
			RaceReCommon.StartBankFlow();
		}
	}

	private void CancelIt()
	{
	}

	private void OnHireTeam()
	{
        //ConsumableOverviewScreen.ConsumableMode = eCarConsumableMode.Timed;
		ScreenManager.Instance.PushScreen(ScreenID.ConsumableOverview);
	}

	private void HideAllButtons()
	{
		this.SetHireTeamEnabled(false);
		this.SetBankEnabled(false);
	}

	private void ShowAllButtons()
	{
		this.SetHireTeamEnabled(PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_VersusRaceTeamCompleted);
		this.DecideToAllowBankOrRefresh();
	}
}
