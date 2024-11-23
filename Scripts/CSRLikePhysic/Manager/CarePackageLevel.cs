using Metrics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using I2.Loc;
using UnityEngine;

[Serializable]
public class CarePackageLevel
{
	public string ID;

	public string InactiveTime;

	public TimeSpan InactiveTimeSpan;

	public List<CarePackageReward> Rewards = new List<CarePackageReward>();

	public CarePackageNotification NotificationDetails = new CarePackageNotification();

	private int prevNotifTime;

	private static readonly IDictionary<CarePackageInGameNotification.eConfirmAction, CarePackageActionBase> confirmActionMapping = new Dictionary<CarePackageInGameNotification.eConfirmAction, CarePackageActionBase>
	{
		{
			CarePackageInGameNotification.eConfirmAction.Reward,
			new RewardCarePackageAction()
		},
		{
			CarePackageInGameNotification.eConfirmAction.TierShowroom,
			new TierShowroomCarePackageAction()
		},
		{
			CarePackageInGameNotification.eConfirmAction.TuningScreen,
			new TuningScreenCarePackageAction()
		},
		{
			CarePackageInGameNotification.eConfirmAction.ConsumablesOverview,
			new ConsumablesOverviewCarePackageAction()
		},
		{
			CarePackageInGameNotification.eConfirmAction.MultiplayerPane,
			new MultiplayerPaneCarePackageAction()
		},
		{
			CarePackageInGameNotification.eConfirmAction.MapScreen,
			new MapScreenCarePackageAction()
		}
	};

	public bool IsInProgress
	{
		get;
		private set;
	}

	public CarePackageRewardDetails CalculatedReward
	{
		get;
		private set;
	}

	public CarePackageLevel()
	{
		this.IsInProgress = false;
		this.CalculatedReward = new CarePackageRewardDetails();
	}

	public void Initialise()
	{
		//Debug.Log("Timespan level : " + ID);
		if (!TimeSpan.TryParse(this.InactiveTime, out this.InactiveTimeSpan))
		{
		}
        //this.InactiveTime = null;
	}

	void WriteFile(string bodyKey, string text, string icon, string time, string timeMakeSchedule)
	{   if (Debug.isDebugBuild)
		{
			try
			{
				if (!File.Exists(GetPath()))
				{
					File.Create(GetPath());
				}
				if (File.Exists(GetPath()))
				{
					using(StreamWriter writer = new StreamWriter(GetPath(), true, Encoding.UTF8))
					{
						writer.WriteLine("BodyKey: " + bodyKey + "\n\t\t"+ "Body: " + text + "\n\t\t" + "Image: " + icon + "\n\t\t" + "Set Schedule Time: " + timeMakeSchedule + 
						                 "pass duration to show: " +time + "minutes" + "\n\t\t" + "-------------------Notification-------------------" + "\n\t\t");
					}
				}
			}
			catch (Exception e)
			{
				Debug.Log(e);
			}
		}
	}

	void AppendToTheFirstLine(string text, string icon, string time)
	{
		if (!File.Exists(GetPath()))
		{
			File.Create(GetPath());
		}
		if (File.Exists(GetPath()))
		{
			File.AppendAllText(GetPath(), "Body: " + text + "\n\t\t" + "Image: " + icon + "\n\t\t" + "pass duration to show: " +time + "minutes" + "\n\t\t" + "-------------------Notification-------------------" + "\n\t\t" + Environment.NewLine);

		}
	}
	public void Activate()
	{
		this.IsInProgress = true;
		this.CalculatedReward = this.calculateTotalReward();
		PlayerProfileManager.Instance.ActiveProfile.IncrementCarePackageReceivedCount(this.ID);
		if (this.NotificationDetails.InGameNotification.IsEmpty())
		{
			this.Deactivate();
		}
	}

	public void Deactivate()
	{
		this.sendCarePackageCompleteMetricEvent();
		this.IsInProgress = false;
		PlayerProfileManager.Instance.ActiveProfile.SetCarePackageInfo(CarePackagesDatabase.Time(), string.Empty, false);
		GameDatabase.Instance.CarePackages.ScheduleSuitableCarePackage();
	}

	public void ScheduleNotification()
	{
		if (!this.NotificationDetails.LocalNotification.IsEmpty())
		{
			string bodyText = LocalizationManager.GetTranslation(this.NotificationDetails.LocalNotification.Message);
			string buttonText = LocalizationManager.GetTranslation(this.NotificationDetails.LocalNotification.ActionText);
			
			string icon = BasePlatform.ActivePlatform.InsideCountry ? NotificationDetails.LocalNotification.LargeIconInSideCountry: NotificationDetails.LocalNotification.LargeIconOutSideCountry;
			NotificationManager.Active.AddWithTag(Convert.ToInt32(this.InactiveTimeSpan.TotalSeconds), "CarePackage", bodyText, buttonText, 7, icon);
			prevNotifTime = Convert.ToInt32(this.InactiveTimeSpan.TotalSeconds);
			WriteFile(NotificationDetails.LocalNotification.Message, bodyText, icon,Time.time.ToString() ,(Convert.ToInt32(this.InactiveTimeSpan.TotalMinutes)).ToString());
		}
		
	}

	public PopUp GetPopUp()
	{
		CarePackageInGameNotification inGameNotification = this.NotificationDetails.InGameNotification;
		if (!inGameNotification.IsEmpty())
		{
		    var body = inGameNotification.Body;
		    var bodyAlreadyTranslated = false;
            this.CalculatedReward = this.calculateTotalReward();
		    var cashReward = CalculatedReward.NonNegativeCash;
            var goldReward = CalculatedReward.NonNegativeGold;

            if (cashReward > 0)
		    {
		        body = string.Format(LocalizationManager.GetTranslation(inGameNotification.Body), CurrencyUtils.GetCashString(cashReward));
		        bodyAlreadyTranslated = true;
		    }
            else if (goldReward > 0)
            {
                body = string.Format(LocalizationManager.GetTranslation(inGameNotification.Body), CurrencyUtils.GetGoldStringWithIcon(goldReward));
                bodyAlreadyTranslated = true;
            }
		    else
		    {
		        body = inGameNotification.Body;
		    }
			PopUp popUp = new PopUp
			{
				Title = inGameNotification.Title,
                BodyText = body,
                BodyAlreadyTranslated = bodyAlreadyTranslated,
				CancelAction = new PopUpButtonAction(this.Deactivate),
				CancelText = inGameNotification.CancelText,
				ConfirmAction = new PopUpButtonAction(this.getConfirmAction),
				ConfirmText = inGameNotification.ConfirmText,
                //ItemGraphicPath = inGameNotification.Image,
				GraphicPath = inGameNotification.Image,
				ImageCaption = inGameNotification.ImageCaption,
				ShouldCoverNavBar = true
			};
			if (inGameNotification.BossTier > 0)
			{
				popUp.BossTier = inGameNotification.BossTier - 1;
				popUp.IsCrewLeader = true;
			}
			return popUp;
		}
		return null;
	}

	private void getConfirmAction()
	{
		if (!this.NotificationDetails.InGameNotification.IsEmpty() && CarePackageLevel.confirmActionMapping.ContainsKey(this.NotificationDetails.InGameNotification.ConfirmActionEnum))
		{
			CarePackageActionBase carePackageActionBase = CarePackageLevel.confirmActionMapping[this.NotificationDetails.InGameNotification.ConfirmActionEnum];
			if (carePackageActionBase != null)
			{
				carePackageActionBase.Action(this);
			}
		}
	}

	private void sendCarePackageCompleteMetricEvent()
	{
		DateTime carePackageUpdateTime = PlayerProfileManager.Instance.ActiveProfile.CarePackageUpdateTime;
		DateTime d = CarePackagesDatabase.Time();
		string value = string.Format("{0:#}", (d - carePackageUpdateTime).TotalSeconds);
		string value2 = PlayerProfileManager.Instance.ActiveProfile.CarePackageTotalReceivedLevelCount(this.ID).ToString();
		string value3 = (!NotificationManager.Active.LaunchedViaNotification("CarePackage")) ? "0" : "1";
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.PackageLevel,
				this.ID
			},
			{
				Parameters.DCsh,
				this.CalculatedReward.NonNegativeCash.ToString()
			},
			{
				Parameters.DGld,
				this.CalculatedReward.NonNegativeGold.ToString()
			},
			{
				Parameters.FreeUpgs,
				this.CalculatedReward.NonNegativeUpgrades.ToString()
			},
			{
				Parameters.LapseTime,
				value
			},
			{
				Parameters.CmlLevelPacks,
				value2
			},
			{
				Parameters.Notif,
				value3
			}
		};
		Log.AnEvent(Events.CarePackage, data);
	}

	private CarePackageRewardDetails calculateTotalReward()
	{
		CarePackageRewardDetails carePackageRewardDetails = new CarePackageRewardDetails();
		foreach (CarePackageReward current in this.Rewards)
		{
			carePackageRewardDetails += current.GetCalculatedDetails();
		}
		return carePackageRewardDetails;
	}
	
	public string GetPath()
	{
#if UNITY_EDITOR
		return Application.dataPath + "ScheduleNotification.txt";
#elif UNITY_ANDROID
        return Application.persistentDataPath + "ScheduleNotification.txt"; 
#endif

	}
}
