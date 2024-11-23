using System;
using I2.Loc;
using Objectives;
using UnityEngine;
using UnityEngine.UI;
//using Tutorial.Objectives.Commands;

public class LeftSidePanelAchievementsScreen : PlayerCrewUISubScreen
{
	public GameObject ScrollPanelPlaceholder_DailyTasks;

    //public GameObject ScrollPanelPlaceholder_Achievements;

    //public GameObject prefab_DailyTasks;

	public GameObject prefab_DailyTasksV2;

    //public GameObject prefab_Achievements;

    //public Toggle Toggle_DailyTasks;

    //public UIToggle Toggle_DailyTasksV2;

    //public UIToggle Toggle_Achievements;

    //public LeftSidePanelStatusBar StatusBar_DailyTasks;

    //public LeftSidePanelStatusBar StatusBar_Achievements;

    //public GameObject Window_GCActive;

    //public GameObject Window_GCInative;

    //public UILabel Label_LoginPrompt;

    //public GameObject AchievementLoadingSpinner;

    //public GameObject AchievementRetryButton;

    private VerticalLayoutGroup _scrollTable_DailyTasks;

    //private UITable _scrollTable_Achievements;

	private bool _metricsNextTabChangeIsClick;

	public static LeftSidePanelAchievementsScreen Instance;

	private static int queueAchievementRequest;

	private static bool gotAchievement;

	private static bool gotMeta;

    public VerticalLayoutGroup scrollTable_DailyTasks
	{
		get
		{
			if (this._scrollTable_DailyTasks == null)
			{
                this._scrollTable_DailyTasks = this.ScrollPanelPlaceholder_DailyTasks.GetComponentInChildren<VerticalLayoutGroup>(true);
			}
			return this._scrollTable_DailyTasks;
		}
	}

    //private UITable scrollTable_Achievements
    //{
    //    get
    //    {
    //        if (this._scrollTable_Achievements == null)
    //        {
    //            this._scrollTable_Achievements = this.ScrollPanelPlaceholder_Achievements.GetComponentInChildren(true);
    //        }
    //        return this._scrollTable_Achievements;
    //    }
    //}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
        //Camera componentInChildren = base.gameObject.transform.GetComponentInChildren<Camera>();
        //componentInChildren.enabled = false;
        //this.AchievementLoadingSpinner.SetActive(false);
        //this.AchievementRetryButton.SetActive(false);
        //TextMeshProUGUI[] componentsInChildren = this.Toggle_DailyTasks.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true);
        //for (int i = 0; i < componentsInChildren.Length; i++)
        //{
        //    TextMeshProUGUI uILabel = componentsInChildren[i];
        //    uILabel.text = LocalizationManager.GetTranslation("TEXT_SIDE_PANEL_OBJECTIVES");
        //}
        //UILabel[] componentsInChildren2 = this.Toggle_Achievements.gameObject.GetComponentsInChildren<UILabel>(true);
        //for (int j = 0; j < componentsInChildren2.Length; j++)
        //{
        //    UILabel uILabel2 = componentsInChildren2[j];
        //    uILabel2.text = LocalizationManager.GetTranslation("TEXT_MENU_ICON_ACHIEVEMENTS");
        //}
        //this.Label_LoginPrompt.text = LocalizationManager.GetTranslation("TEXT_GOOGLE_PLAY_LOGIN_PROMPT");
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
		if (ObjectiveManager.Instance != null)
		{
			ObjectiveManager.Instance.AnyObjectivesStateUpdated -= new OnAnyObjectivesStateUpdated(this.RefreshAllData);
		}
		if (LeftSidePanelContainer.Instance)
		{
			LeftSidePanelContainer.OnLeftSidePanelToggled -= new Action<bool>(this.OnLeftPanelToggled);
            //EventDelegate.Remove(LeftSidePanelContainer.Instance.Toggle_Tab1.onChange, new EventDelegate.Callback(this.FillTable_DailyTasks));
			LeftSidePanelContainer.Instance.RemoveSubScreen(this);
		}
	}

    //void OnDisable()
    //{
    //    Debug.Log("here");
    //}

	private void Start()
	{
        //TutorialCommand.ExecuteOnObjective<object>(new OnAchievementPanelStarted());
		ObjectiveManager.Instance.AnyObjectivesStateUpdated += new OnAnyObjectivesStateUpdated(this.RefreshAllData);
		if (LeftSidePanelContainer.Instance)
		{
			LeftSidePanelContainer.OnLeftSidePanelToggled += new Action<bool>(this.OnLeftPanelToggled);
            //EventDelegate.Add(LeftSidePanelContainer.Instance.Toggle_Tab1.onChange, new EventDelegate.Callback(this.FillTable_DailyTasks));
		}
		this.FillTable_DailyTasks();
		LeftSidePanelContainer.Instance.AddSubScreen(this);
	}

	protected void OnLeftPanelToggled(bool open)
	{
		if (open)
		{
			this.RefreshAllData();
			this.FillTable_DailyTasks();
		}
	}

	private void Update()
	{
        //if (Input.GetKey(KeyCode.A))
        //{
        //    this.scrollTable_DailyTasks.Reposition();
        //}
	}

	public void RefreshAllData()
	{
		this.FillTable_DailyTasks();
        //this.Window_GCActive.SetActive(false);
        //this.Window_GCInative.SetActive(true);
	}

	private void FillTable_DailyTasks()
	{
		while (0 < this.scrollTable_DailyTasks.transform.childCount)
		{
            DestroyImmediate(this.scrollTable_DailyTasks.transform.GetChild(0).gameObject);
            //NGUITools.Destroy(this.scrollTable_DailyTasks.transform.GetChild(0).gameObject);
		}
		int maximumActiveObjectives = ObjectiveManager.Instance.MaximumActiveObjectives;
		if (maximumActiveObjectives <= 3)
		{
            Transform scrollBarPanel = base.transform.FindChildRecursively("ScrollBarPanel");
            if (scrollBarPanel != null)
			{
                scrollBarPanel.gameObject.SetActive(false);
			}
            Transform scrollViewPanel = base.transform.FindChildRecursively("ScrollViewPanel");
            if (scrollViewPanel != null)
			{
                //UIScrollView component = scrollViewPanel.gameObject.GetComponent<UIScrollView>();
                //if (component != null)
                //{
                //}
			}
		}
		int num = 0;
		foreach (AbstractObjective current in ObjectiveManager.Instance.ActiveObjectives)
		{
			if (num >= maximumActiveObjectives)
			{
				break;
			}
			if (current.IsActive)
			{
			    GameObject dailyTaskPrefab = this.prefab_DailyTasksV2;//(!ObjectiveManager.Instance.m_enableObjectivesV2) ? this.prefab_DailyTasks : this.prefab_DailyTasksV2;
			    GameObject taskInstance = Instantiate(dailyTaskPrefab);//   NGUITools.AddChild(this.scrollTable_DailyTasks.gameObject, dailyTaskPrefab);
			    taskInstance.transform.SetParent(this.scrollTable_DailyTasks.transform, false);
				LeftSidePanelDailyTasksItem dailyTaskItem = taskInstance.GetComponent<LeftSidePanelDailyTasksItem>();
				dailyTaskItem.Title = LocalizationManager.GetTranslation(current.Title);
				dailyTaskItem.Description = current.GetDescription();

			    dailyTaskItem.StatusBarCurrencyValue = current.Rewards[0].GetRewardText();// string.Format("{0:##,###}", current.Rewards[0].Amount);//LocalisationManager.CultureFormat(current.Rewards[0].Amount, "N0"));
				dailyTaskItem.SetRewardType(current.Rewards[0].Reward.rewardType);
				if (current.TotalProgressStepsOverride != -1f)
				{
					dailyTaskItem.CurrentProgressOverride = current.TotalProgressStepsOverride;
				}
				else
				{
					dailyTaskItem.CurrentProgress = current.CurrentProgress;
				}
				TimeSpan timeLimit = current.GetTimeLimit();
				if (timeLimit == DateTime.MinValue.TimeOfDay)
				{
					dailyTaskItem.TimeLimit = string.Empty;
				}
				else
				{
					TimeSpan timeSpan = timeLimit;
					if (0 < timeSpan.Days)
					{
                        dailyTaskItem.TimeLimit = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_DAYS_HOURS_AND_MINUTES_SHORT"), timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
					}
					else if (0 < timeSpan.Hours)
					{
                        dailyTaskItem.TimeLimit = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_HOURS_AND_MINUTES_SHORT"), timeSpan.Hours, timeSpan.Minutes);
					}
					else if (0 < timeSpan.Minutes)
					{
						dailyTaskItem.TimeLimit = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_MINUTES"), timeSpan.Minutes);
					}
					else
					{
						dailyTaskItem.TimeLimit = string.Empty;
					}
				}
				if (!current.IsComplete)
				{
					dailyTaskItem.TaskCompletion = current.Progress;
                    dailyTaskItem.EnableCollectButton(false);
				    dailyTaskItem.Toggle_Complete.isOn = false;
				}
				else if (ObjectiveManager.Instance.m_enableObjectivesV2)
				{
					if (dailyTaskItem.Toggle_Complete != null)
					{
						if (current.HasCollected)
						{
							dailyTaskItem.Toggle_Complete.isOn = true;
							dailyTaskItem.EnableCollectButton(false);
						}
						else
						{
                            dailyTaskItem.Toggle_Complete.isOn = true;
							dailyTaskItem.SetObjectiveID(current.ID);
						    dailyTaskItem.OnCollectPressedCallback += OnCollectPressedCallback;
						}
					}
				}
				else
				{
                    dailyTaskItem.TaskCompletion = 1;
                    dailyTaskItem.Toggle_Complete.isOn = true;
					dailyTaskItem.SetObjectiveID(current.ID);
                    dailyTaskItem.OnCollectPressedCallback += OnCollectPressedCallback;
				}
				num++;
			}
		}
        //this.scrollTable_DailyTasks.Reposition();
	}

	private void OnCollectPressedCallback(LeftSidePanelDailyTasksItem optionItem)
	{
        if (optionItem != null && ObjectiveManager.Instance)
        {
	        string objectiveID = optionItem.GetObjectiveID();
	        AbstractObjective objctv = ObjectiveManager.Instance.GetObjectiveById(objectiveID);
	        bool canHaveExtraReward = GTAdManager.Instance.CanShowAdForExtraReward() && objctv.CanHaveExtraReward();
	        string oldReward = objctv.GetRewardText(false);
	        string newReward = objctv.GetRewardText(true);
	        if (canHaveExtraReward) {
		        VideoForRewardsManager.Instance.SetExtraRewardResult(new VideoForRewardsManager.ExtraRewardResult()
		        {
			        ActionOnVideoFail = ()=>CollectPrize(objectiveID, false),
			        ActionOnVideoOfferReject = ()=>CollectPrize(objectiveID, false),
			        ActionOnVideoSuccess = ()=>CollectPrize(objectiveID, true),
			        VideoFailRewardText = oldReward,
			        VideoSuccessRewardText = newReward
		        });
		        VideoForRewardsManager.Instance.StartFlow(VideoForRewardConfiguration.eRewardID.VideoForDoubledPrize);
	        } else {
		        CollectPrize(objectiveID, false);
	        }
		}
	}

	private void CollectPrize(string objectiveID, bool extraReward)
	{
		ObjectiveManager.Instance.CollectReward(objectiveID, extraReward);
		AudioManager.Instance.PlaySound(AudioEvent.Frontend_Fuel_Refill, Camera.main.gameObject);
		this.RefreshAllData();
		//TutorialCommand.ExecuteOnObjective<object>(new OnObjectiveRewardCollected());
	}

    //private void FillTable_Achievements()
    //{
    //    while (0 < this.scrollTable_Achievements.transform.childCount)
    //    {
    //        NGUITools.Destroy(this.scrollTable_Achievements.transform.GetChild(0).gameObject);
    //    }
    //    List<GameCenterAchievementMetadata> achievementMeta = CSR2GameCenterState.Instance.AchievementMeta;
    //    for (int i = 0; i < achievementMeta.Count; i++)
    //    {
    //        GameObject gameObject = NGUITools.AddChild(this.scrollTable_Achievements.gameObject, this.prefab_Achievements);
    //        LeftSidePanelAchievementsItem component = gameObject.GetComponent<LeftSidePanelAchievementsItem>();
    //        GameCenterAchievementMetadata gameCenterAchievementMetadata = achievementMeta[i];
    //        GameCenterAchievement achievementForId = CSR2GameCenterState.Instance.GetAchievementForId(gameCenterAchievementMetadata.identifier);
    //        bool flag = achievementForId != null && achievementForId.completed;
    //        UIProgressBar[] progressBar_CompletionBar = component.ProgressBar_CompletionBar;
    //        for (int j = 0; j < progressBar_CompletionBar.Length; j++)
    //        {
    //            UIProgressBar uIProgressBar = progressBar_CompletionBar[j];
    //            if (achievementForId != null)
    //            {
    //                uIProgressBar.value = ((!flag) ? (achievementForId.percentComplete / 100f) : 1f);
    //            }
    //            else
    //            {
    //                uIProgressBar.value = 0f;
    //            }
    //        }
    //        component.Toggle_Complete.value = flag;
    //        CSR2FX_UILabel[] label_Title = component.Label_Title;
    //        for (int k = 0; k < label_Title.Length; k++)
    //        {
    //            CSR2FX_UILabel cSR2FX_UILabel = label_Title[k];
    //            cSR2FX_UILabel.SetText(gameCenterAchievementMetadata.title);
    //        }
    //        CSR2FX_UILabel[] label_Description = component.Label_Description;
    //        for (int l = 0; l < label_Description.Length; l++)
    //        {
    //            CSR2FX_UILabel cSR2FX_UILabel2 = label_Description[l];
    //            cSR2FX_UILabel2.SetText((!flag) ? gameCenterAchievementMetadata.unachievedDescription : gameCenterAchievementMetadata.description);
    //        }
    //        CSR2FX_UILabel[] label_StatusBarCurrencyValue = component.Label_StatusBarCurrencyValue;
    //        for (int m = 0; m < label_StatusBarCurrencyValue.Length; m++)
    //        {
    //            CSR2FX_UILabel cSR2FX_UILabel3 = label_StatusBarCurrencyValue[m];
    //            cSR2FX_UILabel3.SetText(gameCenterAchievementMetadata.maximumPoints.ToString());
    //        }
    //        CSR2FX_UILabel[] label_StatusBarGameCenterValue = component.Label_StatusBarGameCenterValue;
    //        for (int n = 0; n < label_StatusBarGameCenterValue.Length; n++)
    //        {
    //            CSR2FX_UILabel cSR2FX_UILabel4 = label_StatusBarGameCenterValue[n];
    //            cSR2FX_UILabel4.SetText(gameCenterAchievementMetadata.maximumPoints.ToString());
    //        }
    //    }
    //    this.scrollTable_Achievements.Reposition();
    //}

	public void OnTabChange()
	{
		this.registerGameCenterEvents();
		this.FillTable_DailyTasks();
		if (this._metricsNextTabChangeIsClick)
		{
			this.LogMetricEvent("click");
		}
		else
		{
			this._metricsNextTabChangeIsClick = true;
		}
		Singleton<AudioManager>.Instance.PlaySound(AudioEvent.Frontend_Forward, null);
	}

	public override void LogMetricEvent(string eventType)
	{
        //string subTab;
        //if (this.Toggle_DailyTasks.value)
        //{
        //    subTab = "objectives";
        //}
        //else
        //{
        //    subTab = "achievements";
        //}
        //ZTrackMetricsHelper.LogSidePanelEvent("achievements", eventType, subTab, null);
	}

	public void OnTapRetry()
	{
        //this.AchievementRetryButton.SetActive(false);
		this.FetchAchievementData();
	}

	private void FetchAchievementData()
	{
        //while (0 < this.scrollTable_Achievements.transform.childCount)
        //{
        //    NGUITools.Destroy(this.scrollTable_Achievements.transform.GetChild(0).gameObject);
        //}
        //this.AchievementLoadingSpinner.SetActive(true);
        //LeftSidePanelAchievementsScreen.queueAchievementRequest = 2;
        //LeftSidePanelAchievementsScreen.gotAchievement = false;
        //LeftSidePanelAchievementsScreen.gotMeta = false;
        //CSR2GameCenterController.Instance.DoGetAchievements();
	}

    //private void achievementsLoaded(List<GameCenterAchievement> achievements)
    //{
    //    LeftSidePanelAchievementsScreen.gotAchievement = true;
    //    this.handleCallback(true);
    //}

    //private void achievementMetadataLoaded(List<GameCenterAchievementMetadata> meta)
    //{
    //    LeftSidePanelAchievementsScreen.gotMeta = true;
    //    this.handleCallback(true);
    //}

	private void loadFailed(string error)
	{
		this.handleCallback(false);
	}

	private void handleCallback(bool success)
	{
		//bool flag = gotAchievement && gotMeta;
		queueAchievementRequest--;
		if (queueAchievementRequest == 0)
		{
            //this.AchievementLoadingSpinner.SetActive(false);
            //if (flag)
            //{
            //    //this.FillTable_Achievements();
            //}
            //else
            //{
            //    this.AchievementRetryButton.SetActive(true);
            //}
		}
	}

	private void registerGameCenterEvents()
	{
        //GameCenterManager.achievementsLoaded -= new AchievementLoadedEventHandler(this.achievementsLoaded);
        //GameCenterManager.loadAchievementsFailed -= new GameCenterErrorEventHandler(this.loadFailed);
        //GameCenterManager.achievementMetadataLoaded -= new AchievementMetadataLoadedEventHandler(this.achievementMetadataLoaded);
        //GameCenterManager.retrieveAchievementMetadataFailed -= new GameCenterErrorEventHandler(this.loadFailed);
        //GameCenterManager.achievementsLoaded += new AchievementLoadedEventHandler(this.achievementsLoaded);
        //GameCenterManager.loadAchievementsFailed += new GameCenterErrorEventHandler(this.loadFailed);
        //GameCenterManager.achievementMetadataLoaded += new AchievementMetadataLoadedEventHandler(this.achievementMetadataLoaded);
        //GameCenterManager.retrieveAchievementMetadataFailed += new GameCenterErrorEventHandler(this.loadFailed);
	}
}
