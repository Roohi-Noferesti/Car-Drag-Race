using LitJson;
using System;
using System.Collections.Generic;
using System.Reflection;
using I2.Loc;
using UnityEngine;

namespace Objectives
{
	public abstract class AbstractObjective : MonoBehaviour
	{
		public string ID;

		public string Title;

		public string Description;

		public DifficultyLevel Difficulty;

		public ObjectiveCategory Category;

		public GameArea GameArea;

		public int TotalProgressSteps = 1;

		public float TotalProgressStepsOverride = -1f;

		public bool CanUpdateWhenInactive;

		public bool IsSequential;

		public List<ObjectiveRewardData> Rewards = new List<ObjectiveRewardData>();

		[SerializeInProfile, HideInInspector]
		public bool IsActive;

		[HideInInspector]
		public bool IsComplete;

		[SerializeInProfile, HideInInspector]
		public bool HasCollected;

		private readonly List<FieldInfo> _serializableInProfileFields = new List<FieldInfo>();

		private int _currentProgress;

		public float ProgressOverride = -1f;

		public bool HasChanged
		{
			get;
			private set;
		}

		public int CurrentProgress
		{
			get
			{
				return this._currentProgress;
			}
			protected set
			{
				if (value != this._currentProgress)
				{
					this.HasChanged = true;
					this._currentProgress = value;
				}
			}
		}

		public bool AwaitingCollection
		{
			get
			{
				return this.IsActive && this.IsComplete;
			}
		}

		public bool CanBeCompleted
		{
			get
			{
				return this.CurrentProgress >= this.TotalProgressSteps;
			}
		}

		public bool IsUpdatable
		{
			get
			{
				return this.IsActive || (!this.IsComplete && this.CanUpdateWhenInactive);
			}
		}

		public float Progress
		{
			get
			{
				if (this.ProgressOverride >= 0f)
				{
					return this.ProgressOverride;
				}
				return (float)this.CurrentProgress / (float)this.TotalProgressSteps;
			}
		}

		public bool HasMadeProgress
		{
			get
			{
				return this.CurrentProgress > 0;
			}
		}

		public void CollectV2()
		{
			this.HasCollected = true;
			this.HasChanged = true;
		}

		public void ClearChanges()
		{
			this.HasChanged = false;
		}

		public virtual string GetDescription()
		{
			return LocalizationManager.GetTranslation(this.Description);
		}

		public virtual TimeSpan GetTimeLimit()
		{
            if (ObjectiveManager.Instance != null && ObjectiveManager.Instance.m_enableObjectivesV2 && ServerSynchronisedTime.Instance.ServerTimeValid && PlayerProfileManager.Instance != null)
            {
                PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
                if (activeProfile != null)
                {
                    DateTime serverNowClock_InCurrentTimeZone = ServerSynchronisedTime.Instance.GetDateTime();
                    return activeProfile.ObjectiveEndTime - serverNowClock_InCurrentTimeZone;
                }
            }
			return DateTime.MinValue.TimeOfDay;
		}

		public virtual void UpdateState()
		{
		}

		public virtual bool IsPossibleToComplete()
		{
			return true;
		}
		
		public bool CanHaveExtraReward()
		{
			foreach (var reward in Rewards)
			{
				switch (reward.Reward.rewardType)
				{
					case ERewardType.Cash:
					case ERewardType.Gold:
						return true;
				}
			}
			return false;
		}

		public string GetRewardText(bool extraReward)
		{
			string res = "";
			var reward = Rewards[0];
			int amount = extraReward ? reward.Amount * 2 : reward.Amount;
			switch (reward.Reward.rewardType)
			{
				case ERewardType.Cash:
					res = CurrencyUtils.GetCashString(amount);
					break;
				case ERewardType.Gold:
					res = CurrencyUtils.GetGoldStringWithIcon(amount);
					break;
				case ERewardType.FreeUpgrade:
					res = CurrencyUtils.GetFreeUpgradeString(amount);
					break;
				case ERewardType.FuelPip:
					res = CurrencyUtils.GetFuelPipeString(amount);
					break;
			}
			return res;
		}

		protected void ForceComplete()
		{
			this.CurrentProgress = this.TotalProgressSteps;
            //ZTrackMetricsHelper.LogObjectiveMetric("complete", this);
		}

		public virtual JsonDict ToDict()
		{
			JsonDict jsonDict = new JsonDict();
			foreach (FieldInfo current in this._serializableInProfileFields)
			{
				AbstractObjective.SetDictValue(current.GetValue(this), current.Name, jsonDict);
			}
			return jsonDict;
		}

		public virtual void FromDict(JsonDict source)
		{
			foreach (FieldInfo current in this._serializableInProfileFields)
			{
				current.SetValue(this, AbstractObjective.GetDictValue(current.Name, source, current.FieldType));
			}
		}

		protected virtual void Awake()
		{
			Type type = base.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				if (fieldInfo.GetCustomAttributes(typeof(SerializeInProfileAttribute), true).Length > 0)
				{
					this._serializableInProfileFields.Add(fieldInfo);
				}
			}
		}

		private static void SetDictValue(object val, string name, JsonDict dict)
		{
			if (val is int)
			{
				dict.Set(name, (int)val);
			}
			else if (val is ulong)
			{
				dict.Set(name, (ulong)val);
			}
			else if (val is long)
			{
				dict.Set(name, (long)val);
			}
			else if (val is float)
			{
				dict.Set(name, (float)val);
			}
			else if (val is bool)
			{
				dict.Set(name, (bool)val);
			}
			else if (val is string)
			{
				dict.Set(name, (string)val);
			}
			else if (val is DateTime)
			{
				dict.Set(name, (DateTime)val);
			}
			else if (val is List<int>)
			{
				dict.Set(name, (List<int>)val);
			}
            else if (val is List<string>)
            {
                dict.Set(name, (List<string>)val);
            }
		}

		private static object GetDictValue(string name, JsonDict dict, Type type)
		{
			if (type == typeof(int))
			{
				return dict.GetInt(name);
			}
            if (type == typeof(ulong))
            {
                return dict.GetUlong(name);
            }
            if (type == typeof(long))
            {
                return dict.GetLong(name);
            }
			if (type == typeof(float))
			{
				return dict.GetFloat(name);
			}
            if (type == typeof(bool))
            {
                return dict.GetBool(name, false);
            }
			if (type == typeof(string))
			{
				return dict.GetString(name);
			}
			if (type == typeof(DateTime))
			{
				return dict.GetDateTime(name);
			}
			if (type == typeof(List<int>))
			{
				return dict.GetIntList(name);
			}
            if (type == typeof(List<string>))
            {
                return dict.GetStringList(name);
            }
			return null;
		}

		internal virtual void Clear()
		{
			this.IsActive = false;
			this.IsComplete = false;
			this.HasCollected = false;
			this.CurrentProgress = 0;
		}
	}
}
