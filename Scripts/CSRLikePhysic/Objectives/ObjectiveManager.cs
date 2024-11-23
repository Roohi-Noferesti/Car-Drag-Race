using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Objectives.Impl;
using UnityEngine;
using Random = UnityEngine.Random;
//using Tutorial.Objectives.Commands;

namespace Objectives
{
	public class ObjectiveManager : MonoBehaviour
	{
		public List<CategoryCount> ObjectiveCategoryLimits = new List<CategoryCount>();

		public int NumObjectivesBeforeIncreasingDifficulty = 6;

		public int MaxConcurrentGameAreas = 2;

		public List<LevelBoundary> Levels = new List<LevelBoundary>();

		private readonly List<AbstractObjective> _activeObjectives = new List<AbstractObjective>();

		private readonly List<AbstractObjective> _inactiveAndUncollectedObjectives = new List<AbstractObjective>();

		private readonly List<AbstractObjective> _inactiveAndNonsequentialObjectives = new List<AbstractObjective>();

		public bool m_profileLoaded;

		public bool m_debugLog;

	    public bool m_enableObjectivesV2 = false;// = FeatureKillswitches.ObjectivesVersionTwoEnabled;

		private List<ObjectiveRewardDefinition> METADATA_rewardDefinitionList = new List<ObjectiveRewardDefinition>();

		private bool m_initalised;

		private List<DifficultyLevel> m_difficultyList;

		private Dictionary<int, List<DifficultyLevel>> m_dDifficultyList;

		private bool _haveInjectedObjectives;

		public TimeSpan m_dailyObjectiveResetTime = new TimeSpan(1, 0, 0, 0);

		public int m_numberOfObjectivesOverride = -1;

		private List<string> _updatedObjectiveIDs;

	    public event OnActiveObjectivesStateUpdated ActiveObjectivesStateUpdated;

	    public event OnAnyObjectivesStateUpdated AnyObjectivesStateUpdated;

		public static ObjectiveManager Instance
		{
			get;
			private set;
		}

		public List<AbstractObjective> AllObjectives
		{
			get;
			private set;
		}

        //public PopUpEvent activeRewardPopUpEventRequest
        //{
        //    get;
        //    private set;
        //}

		public bool ForceConvenientSaveActiveProfile
		{
			get;
			set;
		}

		public List<AbstractObjective> ActiveObjectives
		{
			get
			{
				this._activeObjectives.Clear();
				foreach (AbstractObjective current in this.AllObjectives)
				{
					if (current.IsActive)
					{
						this._activeObjectives.Add(current);
					}
				}
				return this._activeObjectives;
			}
		}

		private List<AbstractObjective> InactiveAndUncollectedObjectives
		{
			get
			{
				this._inactiveAndUncollectedObjectives.Clear();
				foreach (AbstractObjective current in this.AllObjectives)
				{
					if (!current.IsActive && !PlayerProfileManager.Instance.ActiveProfile.ObjectivesCollected.Contains(current.ID))
					{
						this._inactiveAndUncollectedObjectives.Add(current);
					}
				}
				return this._inactiveAndUncollectedObjectives;
			}
		}

		private List<AbstractObjective> InactiveAndNonsequentialObjectives
		{
			get
			{
				this._inactiveAndNonsequentialObjectives.Clear();
				foreach (AbstractObjective current in this.AllObjectives)
				{
					if (!current.IsActive && !current.IsComplete && !current.IsSequential)
					{
						this._inactiveAndNonsequentialObjectives.Add(current);
					}
				}
				return this._inactiveAndNonsequentialObjectives;
			}
		}

		public int MaximumActiveObjectives
		{
			get
			{
				if (this.m_numberOfObjectivesOverride != -1)
				{
					return this.m_numberOfObjectivesOverride;
				}
				int num = 0;
				foreach (CategoryCount current in this.ObjectiveCategoryLimits)
				{
					num += current.Count;
				}
				return num;
			}
		}

		private void ClearNonsequentialObjectives()
		{
			foreach (AbstractObjective current in this.AllObjectives)
			{
				if (!current.IsActive && current.IsComplete && !current.IsSequential)
				{
					current.Clear();
					if (PlayerProfileManager.Instance.ActiveProfile.ObjectivesCollected.Contains(current.ID))
					{
						PlayerProfileManager.Instance.ActiveProfile.ObjectivesCollected.Remove(current.ID);
					}
                    if (PlayerProfileManager.Instance.ActiveProfile.ObjectivesCompleted.Contains(current.ID))
                    {
                        PlayerProfileManager.Instance.ActiveProfile.ObjectivesCompleted.Remove(current.ID);
                    }
					if (PlayerProfileManager.Instance.ActiveProfile.ActiveObjectives.ContainsKey(current.ID))
					{
						PlayerProfileManager.Instance.ActiveProfile.ActiveObjectives.Remove(current.ID);
					}
				}
			}
		}

		public void Awake()
		{
			if (Instance != null)
			{
				return;
			}
			Instance = this;
			this._updatedObjectiveIDs = new List<string>();
			this.AllObjectives = new List<AbstractObjective>();
			if (!this.m_enableObjectivesV2)
			{
				this.GatherObjectives();
			}
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		public void Update()
		{
			if ((this.m_enableObjectivesV2 && this.m_initalised) || !this.m_enableObjectivesV2)
			{
				this.UpdateObjectives(false);
				if (this.m_debugLog)
				{
				}
				int maximumActiveObjectives = this.MaximumActiveObjectives;
				if (!this.m_enableObjectivesV2)
				{
					if (this.m_profileLoaded && this.ActiveObjectives.Count < maximumActiveObjectives)
					{
						this.ActivateNextObjective();
					}
				}
                else if (ServerSynchronisedTime.Instance.ServerTimeValid && PlayerProfileManager.Instance != null)
                {
                    PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
                    if (activeProfile != null)
                    {
                        DateTime serverNowClock_InCurrentTimeZone = ServerSynchronisedTime.Instance.GetDateTime();
                        if (serverNowClock_InCurrentTimeZone > activeProfile.ObjectiveEndTime)
                        {
                            this.ResetDailyObjectives();
                        }
                    }
                }
			}
		}

		public void SetObjectiveEndTime()
		{
            if (Instance.m_enableObjectivesV2 && ServerSynchronisedTime.Instance.ServerTimeValid && PlayerProfileManager.Instance != null)
            {
                PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
                if (activeProfile != null)
                {
                    DateTime objectiveEndTime = ServerSynchronisedTime.Instance.GetDateTime();
                    if (this.m_dailyObjectiveResetTime < new TimeSpan(1, 0, 0, 0))
                    {
                        objectiveEndTime = objectiveEndTime.Add(this.m_dailyObjectiveResetTime);
                    }
                    else
                    {
                        objectiveEndTime = objectiveEndTime.AddDays(1.0).Date;
                    }
                    activeProfile.ObjectiveEndTime = objectiveEndTime;
                }
            }
		}

		public int ObjectivesAwaitingCollectionCount()
		{
			if (this.m_enableObjectivesV2)
			{
				return this.ObjectivesAwaitingCollectionCountV2();
			}
			return this.ObjectivesAwaitingCollectionCountV1();
		}

		private int ObjectivesAwaitingCollectionCountV1()
		{
			int num = 0;
			if (this.ActiveObjectives != null)
			{
				foreach (AbstractObjective current in this.ActiveObjectives)
				{
					if (current.IsComplete && current.IsActive)
					{
						num++;
					}
				}
			}
			return num;
		}

		private int ObjectivesAwaitingCollectionCountV2()
		{
			int num = 0;
			if (this.ActiveObjectives != null)
			{
				foreach (AbstractObjective current in this.ActiveObjectives)
				{
					if (current.IsComplete && current.IsActive && !current.HasCollected)
					{
						num++;
					}
				}
			}
			return num;
		}

		public void SetConfigData(ConfigDictionary config)
		{
			if (this._haveInjectedObjectives)
			{
				this.GatherObjectives();
				this._haveInjectedObjectives = false;
			}
			if (config != null)
			{
				ConfigDictionary subDictionary = config.GetSubDictionary("DataOverrides");
				if (subDictionary != null)
				{
					HashSet<string> skipKeys = new HashSet<string>
					{
						"Rewards"
					};
					foreach (string current in subDictionary.Keys)
					{
						AbstractObjective objectiveById = this.GetObjectiveById(current);
						if (objectiveById != null)
						{
							ConfigDictionary subDictionary2 = subDictionary.GetSubDictionary(current);
							if (subDictionary2 != null)
							{
								JsonDict jsonDict = subDictionary2;
								subDictionary2.ApplyOverrides<AbstractObjective>(objectiveById, skipKeys);
								if (jsonDict.ContainsKey("Rewards"))
								{
									JsonList jsonList = jsonDict.GetJsonList("Rewards");
									if (jsonList != null)
									{
										List<ObjectiveRewardData> list = new List<ObjectiveRewardData>();
										for (int i = 0; i < jsonList.Count; i++)
										{
											JsonDict jsonDict2 = jsonList.GetJsonDict(i);
											if (jsonDict2 != null)
											{
												ObjectiveRewardData objectiveRewardData = new ObjectiveRewardData();
												if (!jsonDict2.TryGetEnum<CSR2Reward>("RewardType", out objectiveRewardData.Reward))
												{
												}
												objectiveRewardData.Amount = jsonDict2.GetInt("Amount");
												list.Add(objectiveRewardData);
											}
										}
										objectiveById.Rewards = list;
									}
								}
								this._haveInjectedObjectives = true;
							}
						}
					}
				}
			}
		}

		public void SetConfigDataV2(ObjectiveConfiguration configuration)
		{
		    if (configuration != null)
		    {
		        this.METADATA_rewardDefinitionList.Clear();

		        foreach (var current in configuration.rewardDefinitions)
		        {
		            if (current.valid)
		            {
		                this.METADATA_rewardDefinitionList.Add(current);
		            }
		        }


                foreach (var current2 in configuration.DailyObjectives)
		        {
                    this.AddObjectivesFromMetadata(current2);
		        }
		    }
		    this.m_initalised = true;
			this.GatherObjectives();
		}

		public void UpdateObjectives(bool forceUpdate = false)
		{
			if (this.AllObjectives != null && ((this.m_enableObjectivesV2 && this.m_initalised) || !this.m_enableObjectivesV2))
			{
				this._updatedObjectiveIDs.Clear();
				foreach (AbstractObjective current in this.AllObjectives)
				{
					if (current.IsUpdatable)
					{
						current.UpdateState();
						if (forceUpdate || current.HasChanged)
						{
							this._updatedObjectiveIDs.Add(current.ID);
							current.ClearChanges();
							if (current.CanBeCompleted)
							{
								current.IsComplete = true;
							}
						}
					}
				}
				if (this._updatedObjectiveIDs.Count == 0)
				{
					return;
				}
				this.SaveObjectivesToProfile();
				if (this.ActiveObjectivesStateUpdated != null)
				{
					this.ActiveObjectivesStateUpdated(this._updatedObjectiveIDs);
				}
				if (this.AnyObjectivesStateUpdated != null)
				{
					this.AnyObjectivesStateUpdated();
				}
			}
		}

		public void ActivateObjective(string id)
		{
			AbstractObjective objectiveById = this.GetObjectiveById(id);
			this.ActivateObjective(objectiveById);
		}

		public void CollectReward(string id, bool extraReward)
		{
			AbstractObjective objectiveById = this.GetObjectiveById(id);
			bool flag = this.DeactivateObjective(objectiveById);
			if (flag)
			{
				this.ApplyObjectiveReward(objectiveById, extraReward);
				if (!PlayerProfileManager.Instance.ActiveProfile.ObjectivesCollected.Contains(objectiveById.ID))
				{
					PlayerProfileManager.Instance.ActiveProfile.ObjectivesCollected.Add(objectiveById.ID);
				}
				if (this.m_enableObjectivesV2)
				{
					objectiveById.CollectV2();
				}
                //ZTrackMetricsHelper.LogObjectiveMetric("collect", objectiveById);
			}
			this.UpdateObjectivesInProfile();
			if (!this.m_enableObjectivesV2)
			{
				this.ActivateNextObjective();
			}
            //int rPPrizeForObjective = RPDatabase.Instance.GetRPPrizeForObjective();
            //PlayerProfileManager.Instance.ActiveProfile.AddRP(rPPrizeForObjective, ERPEarnedReason.Objective);
			bool forceConvenientSaveActiveProfile = this.ForceConvenientSaveActiveProfile;
			this.ForceConvenientSaveActiveProfile = true;
			this.SaveObjectivesToProfile();
			this.ForceConvenientSaveActiveProfile = forceConvenientSaveActiveProfile;
		}

		public void OnProfileChanged()
		{
            this.m_profileLoaded = false;
            List<string> objectivesCompleted = PlayerProfileManager.Instance.ActiveProfile.ObjectivesCompleted;
            Dictionary<string, JsonDict> activeObjectives = PlayerProfileManager.Instance.ActiveProfile.ActiveObjectives;
            if (activeObjectives.Count + objectivesCompleted.Count == 0)
            {
                this.ActivateFirstTimeObjectives();
            }
            else
            {
                this.SetCompletedObjectivesFromProfile();
                this.SetActiveObjectivesFromProfile();
            }
            this.m_profileLoaded = true;
		}

		public void Clear()
		{
			foreach (AbstractObjective current in this.AllObjectives)
			{
				current.Clear();
			}
		}

		private void ActivateFirstTimeObjectives()
		{
			base.StartCoroutine(this.ActivateFirstTimeObjectivesCR());
		}

		private IEnumerator ActivateFirstTimeObjectivesCR()
		{
		    while (m_enableObjectivesV2 && !m_initalised)
		    {
		        yield return new WaitForEndOfFrame();
		    }
		    var i = 0;
		    while (i<MaximumActiveObjectives)
		    {
		        var nexObjective = FindNextObjective();
		        if (nexObjective != null)
		        {
                    ActivateObjective(nexObjective);
		        }
		        i++;
		    }

            SetObjectiveEndTime();
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}

	    public AbstractObjective GetObjectiveById(string id)
		{
			AbstractObjective abstractObjective = null;
			foreach (AbstractObjective current in this.AllObjectives)
			{
				if (current.ID == id)
				{
					abstractObjective = current;
					break;
				}
			}
			if (abstractObjective == null)
			{
			}
			return abstractObjective;
		}

	    private void ActivateObjective(AbstractObjective objective)
		{
			if (objective == null)
			{
				return;
			}
			if (this.ActiveObjectives.Count == this.MaximumActiveObjectives)
			{
				return;
			}
			if (objective.IsActive)
			{
				return;
			}
			objective.IsActive = true;
			objective.enabled = true;
			if (!PlayerProfileManager.Instance.ActiveProfile.ActiveObjectives.ContainsKey(objective.ID))
			{
			    var dic = objective.ToDict();
				PlayerProfileManager.Instance.ActiveProfile.ActiveObjectives.Add(objective.ID,dic );

			}
			else
			{
                //ZTrackMetricsHelper.LogObjectiveMetric("alreadyTracking", objective);
			}
			this._updatedObjectiveIDs.Add(objective.ID);
            //ZTrackMetricsHelper.LogObjectiveMetric("activate", objective);
		}

		private void ApplyObjectiveReward(AbstractObjective objective, bool extraReward)
		{
			foreach (ObjectiveRewardData current in objective.Rewards)
			{
				if (current != null)
				{
					CSR2ApplyableReward cSR2ApplyableReward = new CSR2ApplyableReward(current.Reward, current.Amount);
					cSR2ApplyableReward.FillAnyRandomPrizes(current.Reward);
					cSR2ApplyableReward.ApplyAwardToPlayerProfile(false, false, extraReward);
                    //ZTrackMetricsHelper.LogObjectiveReward(cSR2ApplyableReward, current.Amount, objective);
					if (current.Reward.rewardType == ERewardType.FusionUpgrade && Singleton<PopUpManager>.Instance != null)
					{
						if (CommonUI.Instance != null)
						{
							CommonUI.Instance.CashStats.CashLockedState(true);
							CommonUI.Instance.CashStats.GoldLockedState(true);
						}
						if (FuelManager.Instance != null)
						{
							FuelManager.Instance.FuelLockedState(true);
						}
                        //if (LeftSidePanelContainer.Instance.IsLeftSidePanelOpen())
                        //{
                        //    LeftSidePanelContainer.Instance.HideLeftSidePanel();
                        //}
                        //RewardPopup rewardPopup = new RewardPopup("TEXT_PRIZE_CONGRATULATIONS", "TEXT_BUTTON_COLLECT", cSR2ApplyableReward, null, null, null, false, false, true, false, PopUpCharacterRole.MECHANIC);
                        //rewardPopup.DisableBackButton = true;
                        //this.activeRewardPopUpEventRequest = Singleton<PopUpManager>.Instance.RequestPopUp(rewardPopup, PopUpPriorityGroup.OBJECTIVE_NOTIFCATION, new PopUpEvent.OnRequestResult(this.OnRewardPopUpEventRequestResult), new PopUpEvent.OnComplete(this.OnRewardPopUpEventComplete), true, false);
                        //ZTrackMetricsHelper.ReceiveRewardPart(new MetricsTrackingID(), "objective", cSR2ApplyableReward);
					}
					else
					{
                        //ZTrackMetricsHelper.ReceiveRewardPart(new MetricsTrackingID(), "objective", cSR2ApplyableReward);
                        //if (!LeftSidePanelContainer.Instance.IsLeftSidePanelOpen() && TutorialCommand.ExecuteOnObjective<bool>(new CanShowLeftSidePanel()))
                        //{
                        //    LeftSidePanelContainer.Instance.ShowLeftSidePanel();
                        //}
					}
				}
			}
		}

        //private void OnRewardPopUpEventRequestResult(PopUpEvent popUpEvent, bool success)
        //{
        //    if (!success)
        //    {
        //        CommonUI.Instance.CashStats.CashLockedState(false);
        //        CommonUI.Instance.CashStats.GoldLockedState(false);
        //    }
        //}

        //private void OnRewardPopUpEventComplete(PopUpEvent popUpEvent)
        //{
        //    this.activeRewardPopUpEventRequest = null;
        //    if (!LeftSidePanelContainer.Instance.IsLeftSidePanelOpen() && TutorialCommand.ExecuteOnObjective<bool>(new CanShowLeftSidePanel()))
        //    {
        //        LeftSidePanelContainer.Instance.ShowLeftSidePanel();
        //    }
        //}

		private bool DeactivateObjective(AbstractObjective objectiveToDeactivate)
		{
			if (!objectiveToDeactivate.IsActive)
			{
				return false;
			}
			if (!objectiveToDeactivate.IsComplete)
			{
				return false;
			}
			if (this.m_enableObjectivesV2)
			{
				objectiveToDeactivate.IsActive = true;
                //ZTrackMetricsHelper.LogObjectiveMetric("deactivatev2", objectiveToDeactivate);
			}
			else
			{
				objectiveToDeactivate.IsActive = false;
                //ZTrackMetricsHelper.LogObjectiveMetric("deactivate", objectiveToDeactivate);
				this.RemoveObjectiveFromProfile(objectiveToDeactivate.ID);
				this._updatedObjectiveIDs.Add(objectiveToDeactivate.ID);
			}
			return true;
		}

		public void SilentPassObjective(AbstractObjective objectiveToDeactivate)
		{
			objectiveToDeactivate.IsActive = false;
			objectiveToDeactivate.IsComplete = true;
			if (this.m_enableObjectivesV2)
			{
				objectiveToDeactivate.HasCollected = false;
			}
			this.RemoveObjectiveFromProfile(objectiveToDeactivate.ID);
			this._updatedObjectiveIDs.Add(objectiveToDeactivate.ID);
            //ZTrackMetricsHelper.LogObjectiveMetric("skipped", objectiveToDeactivate);
			this.ActivateNextObjective();
			this.SaveObjectivesToProfile();
		}

		private void ActivateNextObjective()
		{
			if (this.m_enableObjectivesV2)
			{
				this.ActivateNextObjectiveV2();
			}
			else
			{
				this.ActivateNextObjectiveV1();
			}
		}

		private void ActivateNextObjectiveV1()
		{
			AbstractObjective abstractObjective = this.FindNextObjectiveV1();
			if (abstractObjective != null)
			{
				this.ActivateObjective(abstractObjective);
			}
		}

		private void ActivateNextObjectiveV2()
		{
			AbstractObjective abstractObjective = this.FindNextObjectiveV2();
			if (abstractObjective != null)
			{
				this.ActivateObjective(abstractObjective);
			}
		}

		private void RemoveObjectiveFromProfile(string id)
		{
			Dictionary<string, JsonDict> activeObjectives = PlayerProfileManager.Instance.ActiveProfile.ActiveObjectives;
			foreach (KeyValuePair<string, JsonDict> current in activeObjectives)
			{
				if (current.Key == id)
				{
					activeObjectives.Remove(current.Key);
					break;
				}
			}
		}

		private AbstractObjective FindNextObjective()
		{
			if (this.m_enableObjectivesV2)
			{
				return this.FindNextObjectiveV2();
			}
			return this.FindNextObjectiveV1();
		}

		private AbstractObjective FindNextObjectiveV1()
		{
			ObjectiveCategory objectiveCategory = this.FindNextMissingObjectiveCategoryV1();
			if (objectiveCategory == ObjectiveCategory.None)
			{
				return null;
			}
			AbstractObjective abstractObjective;
			this.FindNextSequentialObjective(out abstractObjective, objectiveCategory);
			if (abstractObjective != null)
			{
				return abstractObjective;
			}
			this.FindNextObjectiveFromPool(out abstractObjective, objectiveCategory);
			return abstractObjective;
		}

		private AbstractObjective FindNextObjectiveV2()
		{
			ObjectiveCategory objectiveCategory = this.FindNextMissingObjectiveCategoryV2();
			if (objectiveCategory == ObjectiveCategory.None)
			{
				return null;
			}
			AbstractObjective abstractObjective;
			this.FindNextSequentialObjective(out abstractObjective, objectiveCategory);
			if (abstractObjective != null)
			{
				return abstractObjective;
			}
			this.FindNextObjectiveFromPool(out abstractObjective, objectiveCategory);
			return abstractObjective;
		}

		private void FindNextObjectiveFromPool(out AbstractObjective output, ObjectiveCategory category)
		{
			if (this.m_enableObjectivesV2)
			{
				this.FindNextObjectiveFromPoolV2(out output, category);
			}
			else
			{
				this.FindNextObjectiveFromPoolV1(out output, category);
			}
		}

		private void FindNextObjectiveFromPoolV1(out AbstractObjective output, ObjectiveCategory category)
		{
			output = null;
			List<GameArea> excludedGameAreas = this.GetExcludedGameAreas();
			DifficultyLevel difficultyLevel = this.GetDifficulty();
			if (difficultyLevel == DifficultyLevel.None)
			{
				return;
			}
			List<AbstractObjective> list = new List<AbstractObjective>();
			while (difficultyLevel <= DifficultyLevel.VeryHard)
			{
				list.Clear();
				foreach (AbstractObjective current in this.InactiveAndNonsequentialObjectives)
				{
					if (current.Category == category && current.Difficulty == difficultyLevel && !excludedGameAreas.Contains(current.GameArea))
					{
						list.Add(current);
					}
				}
				if (list.Count > 0)
				{
					if (list.Count == 1 && difficultyLevel == DifficultyLevel.VeryHard)
					{
						this.ClearNonsequentialObjectives();
					}
					output = list[Random.Range(0, list.Count)];
					break;
				}
				difficultyLevel++;
			}
		}

		private void FindNextObjectiveFromPoolV2(out AbstractObjective output, ObjectiveCategory category)
		{
			output = null;
			List<GameArea> excludedGameAreas = this.GetExcludedGameAreas();
			DifficultyLevel difficultyLevel;
			if (category == ObjectiveCategory.DailyLogin || category == ObjectiveCategory.Completionist)
			{
				difficultyLevel = DifficultyLevel.Easy;
			}
			else
			{
				difficultyLevel = this.GetDifficultyV2(true);
			}
			if (difficultyLevel == DifficultyLevel.None)
			{
				return;
			}
			List<AbstractObjective> list = new List<AbstractObjective>();
			foreach (AbstractObjective current in this.InactiveAndNonsequentialObjectives)
			{
				if (current.Category == category && current.Difficulty == difficultyLevel && !excludedGameAreas.Contains(current.GameArea) && current.IsPossibleToComplete())
				{
					list.Add(current);
				}
			}
			if (list.Count <= 0)
			{
				foreach (AbstractObjective current2 in this.InactiveAndNonsequentialObjectives)
				{
					if (current2.Category == category && !excludedGameAreas.Contains(current2.GameArea) && current2.IsPossibleToComplete())
					{
						list.Add(current2);
					}
				}
			}
			if (list.Count > 0)
			{
				output = list[Random.Range(0, list.Count)];
			}
		}

		private void FindNextSequentialObjective(out AbstractObjective output, ObjectiveCategory category)
		{
			output = null;
			foreach (AbstractObjective current in this.InactiveAndUncollectedObjectives)
			{
				if (current.IsSequential && current.Category == category)
				{
					output = current;
					break;
				}
			}
		}

		private DifficultyLevel GetDifficulty()
		{
			int playerLevel = PlayerProfileManager.Instance.ActiveProfile.GetPlayerLevel();
			DifficultyLevel difficultyLevel = DifficultyLevel.None;
			foreach (LevelBoundary current in this.Levels)
			{
				if ((current.Level.x < (float)playerLevel && current.Level.y == -1f) || current.Level.y > (float)playerLevel)
				{
					difficultyLevel = current.Difficulty;
					break;
				}
			}
			if (difficultyLevel == DifficultyLevel.None)
			{
				difficultyLevel = (DifficultyLevel)Random.Range(1, 4);
			}
			int objectivesCompletedCount = PlayerProfileManager.Instance.ActiveProfile.ObjectivesCompleted.Count;
			bool flag = objectivesCompletedCount > 0 && difficultyLevel < DifficultyLevel.VeryHard && objectivesCompletedCount % this.NumObjectivesBeforeIncreasingDifficulty == 0;
			if (flag)
			{
				difficultyLevel++;
			}
			return difficultyLevel;
		}

		private void BuildDifficultyList()
		{
			this.m_dDifficultyList = new Dictionary<int, List<DifficultyLevel>>();
			List<DifficultyLevel> list = new List<DifficultyLevel>();
			list.Add(DifficultyLevel.Easy);
			list.Add(DifficultyLevel.Easy);
			list.Add(DifficultyLevel.Easy);
			list.Add(DifficultyLevel.Easy);
			list.Add(DifficultyLevel.Medium);
			list.Add(DifficultyLevel.Medium);
			this.m_dDifficultyList.Add(0, list);
			List<DifficultyLevel> list2 = new List<DifficultyLevel>();
			list2.Add(DifficultyLevel.Easy);
			list2.Add(DifficultyLevel.Easy);
			list2.Add(DifficultyLevel.Easy);
			list2.Add(DifficultyLevel.Medium);
			list2.Add(DifficultyLevel.Medium);
			list2.Add(DifficultyLevel.Hard);
			this.m_dDifficultyList.Add(1, list2);
			List<DifficultyLevel> list3 = new List<DifficultyLevel>();
			list3.Add(DifficultyLevel.Easy);
			list3.Add(DifficultyLevel.Easy);
			list3.Add(DifficultyLevel.Medium);
			list3.Add(DifficultyLevel.Medium);
			list3.Add(DifficultyLevel.Hard);
			list3.Add(DifficultyLevel.Hard);
			this.m_dDifficultyList.Add(2, list3);
			int playerLevel = PlayerProfileManager.Instance.ActiveProfile.GetPlayerLevel();
			int num = 0;
			foreach (LevelBoundary current in this.Levels)
			{
				if ((current.Level.x < (float)playerLevel && current.Level.y == -1f) || current.Level.y > (float)playerLevel)
				{
					break;
				}
				num++;
			}
			if (num >= 0 && num < this.m_dDifficultyList.Count)
			{
				this.m_difficultyList = this.m_dDifficultyList[num];
			}
			else
			{
				this.m_difficultyList = this.m_dDifficultyList[0];
			}
		}

		private DifficultyLevel GetDifficultyV2(bool popDifficulty = true)
		{
			DifficultyLevel result = DifficultyLevel.Easy;
			if (this.m_difficultyList == null || this.m_difficultyList.Count <= 0)
			{
				this.BuildDifficultyList();
			}
			if (this.m_difficultyList != null && this.m_difficultyList.Count > 0)
			{
				int index = Random.Range(0, this.m_difficultyList.Count);
				result = this.m_difficultyList[index];
				if (popDifficulty)
				{
					this.m_difficultyList.Remove(this.m_difficultyList[index]);
				}
			}
			return result;
		}

		private List<GameArea> GetExcludedGameAreas()
		{
			Dictionary<GameArea, int> gameAreaDic = new Dictionary<GameArea, int>();
			List<GameArea> gameAreaList = new List<GameArea>();
			foreach (AbstractObjective current in this.ActiveObjectives)
			{
				if (gameAreaDic.ContainsKey(current.GameArea))
				{
                    gameAreaDic[current.GameArea]++;
				}
				else
				{
					gameAreaDic.Add(current.GameArea, 1);
				}
				if (gameAreaDic[current.GameArea] >= this.MaxConcurrentGameAreas)
				{
					gameAreaList.Add(current.GameArea);
				}
			}
			return gameAreaList;
		}

		private ObjectiveCategory FindNextMissingObjectiveCategoryV1()
		{
			Dictionary<ObjectiveCategory, int> categoryCountDic = new Dictionary<ObjectiveCategory, int>();
			foreach (AbstractObjective activeObjective in this.ActiveObjectives)
			{
				if (!categoryCountDic.ContainsKey(activeObjective.Category))
				{
					categoryCountDic.Add(activeObjective.Category, 0);
				}
                categoryCountDic[activeObjective.Category]++;
			}
			foreach (CategoryCount categoryLimit in this.ObjectiveCategoryLimits)
			{
				if (!categoryCountDic.ContainsKey(categoryLimit.Category))
				{
					return categoryLimit.Category;
				}
				if (categoryCountDic[categoryLimit.Category] < categoryLimit.Count)
				{
					return categoryLimit.Category;
				}
			}
			return ObjectiveCategory.Farm;
		}

		private ObjectiveCategory FindNextMissingObjectiveCategoryV2()
		{
            if (this.ActiveObjectives.Count == 0)
            {
                return ObjectiveCategory.DailyLogin;
            }
			if (this.ActiveObjectives.Count == this.MaximumActiveObjectives - 1)
			{
				return ObjectiveCategory.Completionist;
			}
			var dictionary = new Dictionary<ObjectiveCategory, int>();
			foreach (AbstractObjective current in this.ActiveObjectives)
			{
				if (!dictionary.ContainsKey(current.Category))
				{
					dictionary.Add(current.Category, 0);
				}
                dictionary[current.Category]++;
			}
			foreach (CategoryCount current2 in this.ObjectiveCategoryLimits)
			{
				if (!dictionary.ContainsKey(current2.Category))
				{
                    return current2.Category;
				}
				if (dictionary[current2.Category] < current2.Count)
				{
					return current2.Category;
				}
			}
			return ObjectiveCategory.Farm;
		}

		public void ForceUpdateObjectiveInProfile(string objectiveId)
		{
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			AbstractObjective objectiveById = this.GetObjectiveById(objectiveId);
            if (objectiveById.IsComplete && !activeProfile.ObjectivesCompleted.Contains(objectiveById.ID))
            {
                activeProfile.ObjectivesCompleted.Add(objectiveById.ID);
            }
			JsonDict value = objectiveById.ToDict();
			if (activeProfile.ActiveObjectives.ContainsKey(objectiveId))
			{
				activeProfile.ActiveObjectives[objectiveId] = value;
			}
			else
			{
				activeProfile.ActiveObjectives.Add(objectiveId, value);
			}
		}

		private void UpdateObjectivesInProfile()
		{
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			foreach (string current in this._updatedObjectiveIDs)
			{
				AbstractObjective objectiveById = this.GetObjectiveById(current);
                if (objectiveById.IsComplete && !activeProfile.ObjectivesCompleted.Contains(objectiveById.ID))
                {
                    activeProfile.ObjectivesCompleted.Add(objectiveById.ID);
                }
				JsonDict value = objectiveById.ToDict();
				if (activeProfile.ActiveObjectives.ContainsKey(current))
				{
					activeProfile.ActiveObjectives[current] = value;
				}
				else
				{
					activeProfile.ActiveObjectives.Add(current, value);
				}
			}
		}

		private void SaveObjectivesToProfile()
		{
			this.UpdateObjectivesInProfile();
			if (RaceController.RaceIsRunning() || this.ForceConvenientSaveActiveProfile)
			{
				PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
			}
			else
			{
				PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
			}
		}

		private void GatherObjectives()
		{
			this.AllObjectives = new List<AbstractObjective>();
			AbstractObjective[] components = base.gameObject.GetComponents<AbstractObjective>();
            for (int i = 0; i < components.Length; i++)
			{
                AbstractObjective abstractObjective = components[i];
				abstractObjective.enabled = false;
				this.AllObjectives.Add(abstractObjective);
			}
		}

	    private AbstractObjective AddObjectiveComponentToObjectiveManager(ObjectiveDefinition objectiveDefinition)
	    {
	        AbstractObjective result = null;
	        switch (objectiveDefinition.Type)
	        {
	            case ObjectiveImplementationType.DailyLogIn:
	            {
	                DailyLoginObjective dailyLoginObjective = base.gameObject.AddComponent<DailyLoginObjective>();
	                result = dailyLoginObjective;
	                break;
	            }
	            case ObjectiveImplementationType.GetXPPValueForCurrentCar:
	            {
	                GetXPPValueForCurrentCar getXPPValueForCurrentCar = base.gameObject.AddComponent<GetXPPValueForCurrentCar>();
	                getXPPValueForCurrentCar.PPValueTarget = objectiveDefinition.TargetValue;
                    result = getXPPValueForCurrentCar;
	                break;
	            }
	            case ObjectiveImplementationType.GetXPerfectShift:
	            {
	                GetXPerfectShift getXPerfectShift = base.gameObject.AddComponent<GetXPerfectShift>();
                    getXPerfectShift.PerfectShiftTarget = objectiveDefinition.TargetValue;
	                result = getXPerfectShift;
	                break;
	            }
	            case ObjectiveImplementationType.GetXPerfectStarts:
	            {
	                GetXPerfectStarts getXPerfectStarts = base.gameObject.AddComponent<GetXPerfectStarts>();
                    getXPerfectStarts.PerfectStartTarget = objectiveDefinition.TargetValue;
	                result = getXPerfectStarts;
	                break;
	            }
	            case ObjectiveImplementationType.GetXWinsWithoutNitro:
	            {
	                GetXWinsWithoutNitro getXWinsWithoutNitro = base.gameObject.AddComponent<GetXWinsWithoutNitro>();
                    getXWinsWithoutNitro.NumberOfRaces = objectiveDefinition.TargetValue;
	                result = getXWinsWithoutNitro;
	                break;
	            }
	            case ObjectiveImplementationType.UpgradeItem:
	            {
	                UpgradeItem upgradeItem = base.gameObject.AddComponent<UpgradeItem>();
                    upgradeItem.UpgradeLevel = objectiveDefinition.TargetValue;
	                result = upgradeItem;
	                break;
	            }
	            case ObjectiveImplementationType.WinXTierRaces:
	            {
	                WinXTierRaces winXTierRaces = base.gameObject.AddComponent<WinXTierRaces>();
                    winXTierRaces.Tier = objectiveDefinition.TargetTier;
                    winXTierRaces.NumberOfRaces = objectiveDefinition.TargetValue;
	                result = winXTierRaces;
	                break;
	            }
	            case ObjectiveImplementationType.TuneCarXTimes:
	            {
	                TuneCarXTimes tuneCarXTimes = base.gameObject.AddComponent<TuneCarXTimes>();
                    tuneCarXTimes.UpgradeLevel = objectiveDefinition.TargetValue;
	                result = tuneCarXTimes;
	                break;
	            }
	            case ObjectiveImplementationType.FuseXParts:
	            {
	                FuseXParts fuseXParts = base.gameObject.AddComponent<FuseXParts>();
                    fuseXParts.TargetValue = objectiveDefinition.TargetValue;
	                result = fuseXParts;
	                break;
	            }
	            case ObjectiveImplementationType.PlayXArcadeGameSession:
	            {
	                PlayXArcadeGameSession playXArcadeGameSession = base.gameObject.AddComponent<PlayXArcadeGameSession>();
                    playXArcadeGameSession.TargetValue = objectiveDefinition.TargetValue;
	                result = playXArcadeGameSession;
	                break;
	            }
	            case ObjectiveImplementationType.SpendXSC:
	            {
	                SpendXSC spendXSC = base.gameObject.AddComponent<SpendXSC>();
                    spendXSC.SCToSpendTarget = objectiveDefinition.TargetValue;
	                result = spendXSC;
	                break;
	            }
	            case ObjectiveImplementationType.OpenXCrates:
	            {
	                OpenXCrates openXCrates = base.gameObject.AddComponent<OpenXCrates>();
                    openXCrates.m_openedCratesTarget = objectiveDefinition.TargetValue;
	                result = openXCrates;
	                break;
	            }
	            case ObjectiveImplementationType.PlayForXMinutes:
	            {
	                PlayForXMinutes playForXMinutes = base.gameObject.AddComponent<PlayForXMinutes>();
                    playForXMinutes.m_playTimeCountTarget = objectiveDefinition.TargetValue;
	                result = playForXMinutes;
	                break;
	            }
	            case ObjectiveImplementationType.WinXLadderRaces:
	            {
	                WinXLadderRaces winXLadderRaces = base.gameObject.AddComponent<WinXLadderRaces>();
                    winXLadderRaces.NumberOfRaces = objectiveDefinition.TargetValue;
	                result = winXLadderRaces;
	                break;
	            }
	            case ObjectiveImplementationType.RaceXMilesInADay:
	            {
	                RaceXMilesInADay raceXMilesInADay = base.gameObject.AddComponent<RaceXMilesInADay>();
                    raceXMilesInADay.m_milesRacedTarget = objectiveDefinition.TargetValue;
	                result = raceXMilesInADay;
	                break;
	            }
	            case ObjectiveImplementationType.WinXInvitationalEventRaces:
	            {
	                WinXInvitationalEventRaces winXInvitationalEventRaces =  base.gameObject.AddComponent<WinXInvitationalEventRaces>();
                    winXInvitationalEventRaces.NumberOfRaces = objectiveDefinition.TargetValue;
	                result = winXInvitationalEventRaces;
	                break;
	            }
	            case ObjectiveImplementationType.PerformXTestDrives:
	            {
	                PerformXTestDrives performXTestDrives = base.gameObject.AddComponent<PerformXTestDrives>();
                    performXTestDrives.TargetValue = objectiveDefinition.TargetValue;
	                result = performXTestDrives;
	                break;
	            }
	            case ObjectiveImplementationType.WinXLoanBattleInADay:
	            {
	                WinXLoanBattleInADay winXLoanBattleInADay = base.gameObject.AddComponent<WinXLoanBattleInADay>();
                    winXLoanBattleInADay.NumberOfRaces = objectiveDefinition.TargetValue;
	                result = winXLoanBattleInADay;
	                break;
	            }
	            case ObjectiveImplementationType.PlayXSMPRaces:
	            {
	                PlayXSMPRaces playXSMPRaces = base.gameObject.AddComponent<PlayXSMPRaces>();
                    playXSMPRaces.NumberOfRaces = objectiveDefinition.TargetValue;
	                result = playXSMPRaces;
	                break;
	            }
	            case ObjectiveImplementationType.WinXSMPRacesInTXCar:
	            {
	                WinXSMPRacesInTXCar winXSMPRacesInTXCar = base.gameObject.AddComponent<WinXSMPRacesInTXCar>();
                    winXSMPRacesInTXCar.NumberOfRaces = objectiveDefinition.TargetValue;
                    winXSMPRacesInTXCar.Tier = objectiveDefinition.TargetTier;
	                result = winXSMPRacesInTXCar;
	                break;
	            }
	            case ObjectiveImplementationType.GetFullFreshness:
	            {
	                GetFullFreshness getFullFreshness = base.gameObject.AddComponent<GetFullFreshness>();
	                result = getFullFreshness;
	                break;
	            }
	            case ObjectiveImplementationType.EarnXFullyFreshRP:
	            {
	                EarnXFullyFreshRP earnXFullyFreshRP = base.gameObject.AddComponent<EarnXFullyFreshRP>();
                    earnXFullyFreshRP.TargetValue = objectiveDefinition.TargetValue;
	                result = earnXFullyFreshRP;
	                break;
	            }
	            case ObjectiveImplementationType.DonateXCrewToken:
	            {
	                DonateXCrewToken donateXCrewToken = base.gameObject.AddComponent<DonateXCrewToken>();
                    donateXCrewToken.TargetValue = objectiveDefinition.TargetValue;
	                result = donateXCrewToken;
	                break;
	            }
	            case ObjectiveImplementationType.WinXRPForCrew:
	            {
	                WinXRPForCrew winXRPForCrew = base.gameObject.AddComponent<WinXRPForCrew>();
                    winXRPForCrew.TargetValue = objectiveDefinition.TargetValue;
	                result = winXRPForCrew;
	                break;
	            }
	            case ObjectiveImplementationType.WinXCrewCupRace:
	            {
	                WinXCrewCupRace winXCrewCupRace = base.gameObject.AddComponent<WinXCrewCupRace>();
                    winXCrewCupRace.NumberOfRaces = objectiveDefinition.TargetValue;
	                result = winXCrewCupRace;
	                break;
	            }
	            case ObjectiveImplementationType.CompleteAllDailyObjectives:
	            {
	                CompleteAllDailyObjectives completeAllDailyObjectives =
	                    base.gameObject.AddComponent<CompleteAllDailyObjectives>();
	                result = completeAllDailyObjectives;
	                break;
	            }
	        }
	        return result;
	    }

	    private ObjectiveCategory GetObjectiveCategoryFromString(string objectiveCategoryString)
		{
            //using (IEnumerator enumerator = Enum.GetValues(typeof(ObjectiveCategory)).GetEnumerator())
            //{
            //    while (enumerator.MoveNext())
            //    {
            //        ObjectiveCategory objectiveCategory = (ObjectiveCategory)((int)enumerator.Current);
            //        if (objectiveCategoryString.ToUpper() == objectiveCategory.ToString().ToUpper())
            //        {
            //            return objectiveCategory;
            //        }
            //    }
            //}
			return ObjectiveCategory.None;
		}

		private void AddObjectivesFromMetadata(ObjectiveDefinition objectiveDefinition)
		{
			AbstractObjective abstractObjective = this.AddObjectiveComponentToObjectiveManager(objectiveDefinition);
			if (abstractObjective != null)
			{
                abstractObjective.enabled = false;
                abstractObjective.ID = objectiveDefinition.ObjectiveID;
                abstractObjective.Title = objectiveDefinition.Title;
                abstractObjective.Description = objectiveDefinition.Description;
                abstractObjective.Difficulty = objectiveDefinition.Difficulty;
			    abstractObjective.Category = objectiveDefinition.Category;
                abstractObjective.GameArea = GameArea.Garage;
                abstractObjective.TotalProgressSteps = objectiveDefinition.TotalProgressSteps;
                abstractObjective.TotalProgressStepsOverride = objectiveDefinition.TotalProgressStepsOverride;
                abstractObjective.CanUpdateWhenInactive = objectiveDefinition.CanUpdateWhenInactive;
                abstractObjective.IsSequential = objectiveDefinition.IsSequential;
			    ObjectiveRewardDefinition objectiveRewardDefinition =
			        this.METADATA_rewardDefinitionList.Find(x => x.rewardID == objectiveDefinition.rewardID);
                if (objectiveRewardDefinition == null)
                {
                    objectiveRewardDefinition = this.METADATA_rewardDefinitionList.Find((ObjectiveRewardDefinition x) => x.rewardID == "defaultRewardIfErrorInMetadata");
                }
                abstractObjective.Rewards = new List<ObjectiveRewardData>();
                ObjectiveRewardData objectiveRewardData = new ObjectiveRewardData();
                objectiveRewardData.Reward = new CSR2Reward();
                objectiveRewardData.Reward.rewardType = objectiveRewardDefinition.rewardType;
                if (objectiveRewardDefinition.rewardAmount == -1)
                {
                    int num = 0;
                    objectiveRewardData.Amount = objectiveRewardDefinition.rewardAmountPerTier[num];
                }
                else
                {
                    objectiveRewardData.Amount = objectiveRewardDefinition.rewardAmount;
                }
                abstractObjective.Rewards.Add(objectiveRewardData);
                this.AllObjectives.Add(abstractObjective);
			}
		}

		private void SetCompletedObjectivesFromProfile()
		{
            List<string> objectivesCompleted = PlayerProfileManager.Instance.ActiveProfile.ObjectivesCompleted;
            foreach (string current in objectivesCompleted)
            {
                AbstractObjective objectiveById = this.GetObjectiveById(current);
                if (objectiveById != null)
                {
                    objectiveById.IsComplete = true;
                }
            }
            List<string> objectivesCollected = PlayerProfileManager.Instance.ActiveProfile.ObjectivesCollected;
            foreach (string current2 in objectivesCollected)
            {
                AbstractObjective objectiveById2 = this.GetObjectiveById(current2);
                if (objectiveById2 != null)
                {
                    objectiveById2.IsComplete = true;
                    if (this.m_enableObjectivesV2)
                    {
                        objectiveById2.IsActive = true;
                        objectiveById2.HasCollected = true;
                    }
                    else
                    {
                        objectiveById2.IsActive = false;
                    }
                    this.RemoveObjectiveFromProfile(objectiveById2.ID);
                    this._updatedObjectiveIDs.Add(objectiveById2.ID);
                }
            }
		}

		private void ResetDailyObjectives()
		{
			foreach (AbstractObjective current in this.ActiveObjectives)
			{
				current.IsActive = false;
				current.IsComplete = true;
				if (this.m_enableObjectivesV2)
				{
					current.HasCollected = true;
				}
				this.RemoveObjectiveFromProfile(current.ID);
				this._updatedObjectiveIDs.Add(current.ID);
				if (!PlayerProfileManager.Instance.ActiveProfile.ObjectivesCollected.Contains(current.ID))
				{
					PlayerProfileManager.Instance.ActiveProfile.ObjectivesCollected.Add(current.ID);
				}
				this.ClearNonsequentialObjectives();
			}
			int maximumActiveObjectives = this.MaximumActiveObjectives;
			if (this.m_enableObjectivesV2)
			{
				for (int i = 0; i < maximumActiveObjectives; i++)
				{
					if (this.m_profileLoaded && this.ActiveObjectives.Count < maximumActiveObjectives)
					{
						this.ActivateNextObjective();
					}
				}
			}
			this.SetObjectiveEndTime();
		}

		private void SetActiveObjectivesFromProfile()
		{
			Dictionary<string, JsonDict> activeObjectives = PlayerProfileManager.Instance.ActiveProfile.ActiveObjectives;
			foreach (KeyValuePair<string, JsonDict> current in activeObjectives)
			{
				AbstractObjective objectiveById = this.GetObjectiveById(current.Key);
				if (objectiveById != null)
				{
					objectiveById.FromDict(current.Value);
				}
			}
		}
	}
}
