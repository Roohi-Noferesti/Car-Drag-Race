using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;

public class ArrivalManager : MonoBehaviour
{
	public static ArrivalManager Instance;

	private ArrivalQueue ourQueue = new ArrivalQueue();

	private Arrival PriorityItem;

	private string carstring = string.Empty;

	public Arrival delegateArrival;

	public DateTime getTime()
	{
        return GTDateTime.Now;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
	}

	public static bool CanCheckForArrival()
	{
		if (SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Frontend 
            || PopUpManager.Instance.isShowingPopUp
            || ScreenManager.Instance.CurrentScreen == ScreenID.Intro
            || ScreenManager.Instance.CurrentScreen == ScreenID.PrizeOMatic
            || ScreenManager.Instance.CurrentScreen == ScreenID.Splash
            || ScreenManager.Instance.CurrentScreen == ScreenID.Home
            || ScreenManager.Instance.CurrentScreen == ScreenID.HighStakesChallenge
            || ScreenManager.Instance.CurrentScreen == ScreenID.Credits
            || ScreenManager.Instance.CurrentScreen == ScreenID.CrewProgression
            || ScreenManager.Instance.CurrentScreen == ScreenID.PreCrewProgression
            || ScreenManager.Instance.CurrentScreen == ScreenID.LevelUp
            || ScreenManager.Instance.CurrentScreen == ScreenID.LeagueChange
            || ScreenManager.Instance.CurrentScreen == ScreenID.BalanceRecovery)
		{
			return false;
		}
        //if (MultiplayerUtils.IsPlayingMultiplayer())
        //{
        //    return false;
        //}
        if (ScreenManager.Instance.IsTranslating)
        {
            return false;
        }
	    if (GarageCameraManager.Instance != null &&
	        GarageCameraManager.Instance.IsZoomedIn)
	    {
	        return false;
	    }
        if (LoadingScreenManager.Instance.IsShowingLoading)
		{
			return false;
		}
        if (ScreenManager.Instance.CurrentScreen == ScreenID.CareerModeMap)
        {
            CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
            if (careerModeMapScreen != null && careerModeMapScreen.GoToRaceAnimating)
            {
                return false;
            }
        }
		return true;
	}

	public void Update()
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return;
		}
		if (!CarDatabase.Instance.isReady)
		{
			return;
		}
		if (!CanCheckForArrival())
		{
			return;
		}
		List<Arrival> allDeliverableArrivals = this.ourQueue.GetAllDeliverableArrivals();
		if (allDeliverableArrivals.Count > 0)
		{
			this.DeliveryArrived(allDeliverableArrivals);
		}
	}

	public int HowManyCarsAreOnOrder()
	{
		List<Arrival> list = this.ourQueue.getAllNotArrived().FindAll((Arrival x) => x.arrivalType == ArrivalType.Car);
		return list.Count;
	}

    public void AddArrival(Arrival a)
    {
        this.ourQueue.Add(a);
        a.dueTime = this.getTime().AddSeconds((double) a.deliveryTimeSecs);
        if (a.deliveryTimeSecs > 0f)
        {
            CarInfo car = CarDatabase.Instance.GetCar(a.carId);
            string arg = LocalizationManager.GetTranslation(car.ShortName);
            if (a.arrivalType == ArrivalType.Car)
            {
                string bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_CARARRIVED"), arg);
                string buttonText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_CARARRIVED_ACTION");
                a.AssociatedPushNotification = NotificationManager.Active.Add((int) a.deliveryTimeSecs, bodyText,
                    buttonText,
                    1);
            }
            else if (a.arrivalType == ArrivalType.Upgrade)
            {
                string bodyText2 = string.Format(LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_UPGRADE_ARRIVED"), arg);
                string buttonText2 = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_UPGRADE_ARRIVED_ACTION");
                a.AssociatedPushNotification = NotificationManager.Active.Add((int) a.deliveryTimeSecs, bodyText2,
                    buttonText2, 1);
                Arrival arrival = this.CoalesceNotificationsOfType(ArrivalType.Upgrade, false);
                if (arrival != null)
                {
                    NotificationManager.Active.Cancel(arrival.AssociatedPushNotification);
                    TimeSpan absoluteRemaingTimeUntil = arrival.dueTime - GTDateTime.Now;
                    if (absoluteRemaingTimeUntil.TotalSeconds > 0.0)
                    {
                        bodyText2 = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_ALL_UPGRADES_ARRIVED");
                        arrival.AssociatedPushNotification =
                            NotificationManager.Active.Add((int) absoluteRemaingTimeUntil.TotalSeconds, bodyText2,
                                buttonText2,
                                1);
                    }
                }
            }
        }
    }


    private Arrival CoalesceNotificationsOfType(ArrivalType arrivalType, bool pickSoonest = true)
    {
        List<Arrival> list = this.ourQueue.theQueue.FindAll((Arrival a) => a.arrivalType == arrivalType && a.AssociatedPushNotification != -1 && !a.hasArrived);
        if (list.Count > 1)
        {
            if (pickSoonest)
            {
                list.Sort((Arrival a, Arrival b) => a.dueTime.CompareTo(b.dueTime));
            }
            else
            {
                list.Sort((Arrival a, Arrival b) => b.dueTime.CompareTo(a.dueTime));
            }
            Arrival arrival = list[0];
            foreach (Arrival current in list)
            {
                if (current != arrival)
                {
                    NotificationManager.Active.Cancel(current.AssociatedPushNotification);
                    current.AssociatedPushNotification = -1;
                }
            }
            return arrival;
        }
        return null;
    }

    public void RemoveArrival(Arrival a)
	{
        Arrival arrival = this.ourQueue.Remove(a);
        if (arrival == null)
        {
            return;
        }
        if (arrival.AssociatedPushNotification >= 0)
        {
            NotificationManager.Active.Cancel(arrival.AssociatedPushNotification);
            List<Arrival> list = this.ourQueue.theQueue.FindAll((Arrival b) => b.arrivalType == a.arrivalType && !b.hasArrived && (b.dueTime-GTDateTime.Now).TotalSeconds > 0.0);
            if (list.Count > 0)
            {
                string text = null;
                string buttonText = null;
                if (arrival.arrivalType == ArrivalType.Car)
                {
                    list.Sort((Arrival x, Arrival y) => x.dueTime.CompareTo(y.dueTime));
                    if (!string.IsNullOrEmpty(list[0].carId))
                    {
                        CarInfo car = CarDatabase.Instance.GetCar(list[0].carId);
                        string arg = LocalizationManager.GetTranslation(car.MediumName);
                        text = string.Format(LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_CARARRIVED"), arg);
                        buttonText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_CARARRIVED_ACTION");
                    }
                }
                else if (arrival.arrivalType == ArrivalType.Upgrade)
                {
                    buttonText = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_UPGRADE_ARRIVED_ACTION");
                    if (list.Count == 1)
                    {
                        if (!string.IsNullOrEmpty(list[0].carId))
                        {
                            CarInfo car2 = CarDatabase.Instance.GetCar(list[0].carId);
                            string arg2 = LocalizationManager.GetTranslation(car2.MediumName);
                            text = string.Format(LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_UPGRADE_ARRIVED"), arg2);
                        }
                    }
                    else
                    {
                        list.Sort((Arrival x, Arrival y) => y.dueTime.CompareTo(x.dueTime));
                        text = LocalizationManager.GetTranslation("TEXT_NOTIFICATIONS_ALL_UPGRADES_ARRIVED");
                    }
                }
                if (text != null)
                {
                    TimeSpan absoluteRemaingTimeUntil = GTDateTime.Now - list[0].dueTime;
                    if (absoluteRemaingTimeUntil.TotalSeconds > 0.0)
                    {
                        list[0].AssociatedPushNotification = NotificationManager.Active.Add((int)absoluteRemaingTimeUntil.TotalSeconds, text, buttonText, 1);
                    }
                }
            }
        }
	}

	public void RemoveCar(string carKey)
	{
		this.RemoveArrival(new Arrival
		{
			carId = carKey,
			arrivalType = ArrivalType.Car
		});
	}

	public void RemoveUpgrade(string carKey, eUpgradeType upgrade)
	{
		this.RemoveArrival(new Arrival
		{
			carId = carKey,
			arrivalType = ArrivalType.Upgrade,
			upgradeType = upgrade
		});
	}

	public void ResetAllArrivals()
	{
		List<Arrival> list = new List<Arrival>();
		foreach (Arrival current in this.ourQueue.theQueue)
		{
			list.Add(current);
		}
		foreach (Arrival current2 in list)
		{
			this.RemoveArrival(current2);
			this.AddArrival(current2);
		}
	}

	public bool isCarOnOrder(string carKey)
	{
		Arrival arrival = new Arrival();
		arrival.carId = carKey;
		arrival.arrivalType = ArrivalType.Car;
		return this.ourQueue.FindMatchIndex(arrival) >= 0;
	}

	public bool isUpgradeOnOrder(string carKey, eUpgradeType upgrade)
	{
		Arrival arrival = new Arrival();
		arrival.carId = carKey;
		arrival.arrivalType = ArrivalType.Upgrade;
		arrival.upgradeType = upgrade;
		return this.ourQueue.FindMatchIndex(arrival) >= 0;
	}

	public string getTimeStringTilCarDelivery(string carKey)
	{
		Arrival arrival = new Arrival();
		arrival.carId = carKey;
		arrival.arrivalType = ArrivalType.Car;
		int num = this.ourQueue.FindMatchIndex(arrival);
		if (num < 0)
		{
			return string.Empty;
		}
		Arrival arrival2 = this.ourQueue.At(num);
		int num2;
		int num3;
		arrival2.GetTimeUntilDelivery(out num2, out num3);
		return string.Concat(new object[]
		{
			string.Empty,
			num2,
			":",
			(num3 >= 10) ? string.Empty : "0",
			num3
		});
	}

	private void GetTimeUntilDelivery(string carKey, ArrivalType type, eUpgradeType upgrade, out int minutes, out int seconds)
	{
		Arrival arrival = new Arrival();
		arrival.carId = carKey;
		arrival.arrivalType = type;
		arrival.upgradeType = upgrade;
		int num = this.ourQueue.FindMatchIndex(arrival);
		if (num < 0)
		{
			minutes = 0;
			seconds = 0;
			return;
		}
		Arrival arrival2 = this.ourQueue.At(num);
		arrival2.GetTimeUntilDelivery(out minutes, out seconds);
	}

	public void GetTimeUntilDelivery(string carKey, out int minutes, out int seconds)
	{
		this.GetTimeUntilDelivery(carKey, ArrivalType.Car, eUpgradeType.ENGINE, out minutes, out seconds);
	}

	public void GetTimeUntilDelivery(string carKey, eUpgradeType upgrade, out int minutes, out int seconds)
	{
		this.GetTimeUntilDelivery(carKey, ArrivalType.Upgrade, upgrade, out minutes, out seconds);
	}

	public Arrival GetArrival(Arrival matches)
	{
		int num = this.ourQueue.FindMatchIndex(matches);
		if (num < 0)
		{
			return null;
		}
		return this.ourQueue.At(num);
	}

	public Arrival GetArrivalForCar(string carKey)
	{
		return this.GetArrival(new Arrival
		{
			carId = carKey,
			arrivalType = ArrivalType.Car
		});
	}

	public Arrival GetArrivalForUpgrade(string carKey, eUpgradeType upgrade)
	{
		return this.GetArrival(new Arrival
		{
			carId = carKey,
			arrivalType = ArrivalType.Upgrade,
			upgradeType = upgrade
		});
	}

	public void DeliveryArrived(List<Arrival> arrivals)
	{
	    foreach (Arrival current in arrivals)
	    {
	        ArrivalType arrivalType = current.arrivalType;
	        if (arrivalType == ArrivalType.Upgrade)
	        {

	            PlayerProfileManager.Instance.ActiveProfile.GiveUpgrade(current.carId, current.upgradeType);
	        }
	        else
	        {
	            PlayerProfileManager.Instance.ActiveProfile.GiveCar(current.carId, current.ColourIndex, false);
	        }
	    }

	    PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		if (!StreakManager.Chain.Active())
		{
			this.NotifyUserOfDelivery(arrivals);
		}
	}

	public void NotifyUserOfDelivery(List<Arrival> arrivals)
	{
		int carCount = 0;
		int upgradeCount = 0;
		bool isSelectedcar = true;
		List<string> upgradeListCarNames = new List<string>();
		List<string> carListCarNames = new List<string>();
		this.delegateArrival = null;
		string currentlySelectedCarDBKey = PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey;
		foreach (Arrival current in arrivals)
		{
			CarInfo car = CarDatabase.Instance.GetCar(current.carId);
			if (car != null)
			{
				if (current.carId != currentlySelectedCarDBKey)
				{
					isSelectedcar = false;
				}
				ArrivalType arrivalType = current.arrivalType;
				if (arrivalType != ArrivalType.Car)
				{
					if (arrivalType == ArrivalType.Upgrade)
					{
						upgradeListCarNames.Add(LocalizationManager.GetTranslation(car.ShortName));
						this.RemoveArrival(current);
						upgradeCount++;
						if (this.delegateArrival == null)
						{
							this.delegateArrival = current;
						}
					}
				}
				else
				{
                    this.RemoveArrival(current);
					carCount++;
					if (this.delegateArrival == null)
					{
						carListCarNames.Add(LocalizationManager.GetTranslation(car.ShortName));
						this.delegateArrival = current;
					}
					else
					{
						CarInfo car2 = CarDatabase.Instance.GetCar(this.delegateArrival.carId);
						if (car2.BuyPrice < car.BuyPrice)
						{
                            carListCarNames.Insert(0, LocalizationManager.GetTranslation(car.ShortName));
							this.delegateArrival = current;
						}
						else
						{
							carListCarNames.Add(LocalizationManager.GetTranslation(car.MediumName));
						}
					}
				}
			}
		}

		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		string bodyText = string.Empty;
		if (carCount == 0 && upgradeCount == 0)
		{
			return;
		}
		if (upgradeCount == 1 && carCount == 0)
		{
			if (isSelectedcar)
			{
				bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_ARRIVAL_UPGRADE_SINGLE"), upgradeListCarNames[0]);
				PopUp popup = new PopUp
				{
					Title = "TEXT_POPUPS_UPGRADEDELIVERY_TITLE",
					BodyText = bodyText,
					BodyAlreadyTranslated = true,
					CancelAction = new PopUpButtonAction(this.delegateFunctionFail),
					ConfirmAction = new PopUpButtonAction(this.delegateFunctionConfirm),
					CancelText = "TEXT_BUTTON_LATER",
					ConfirmText = "TEXT_BUTTON_SHOW_ME"
				};
				PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
			}
			else
			{
				bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_ARRIVAL_UPGRADE_SINGLE_NOSEE"), upgradeListCarNames[0]);
				PopUp popup2 = new PopUp
				{
					Title = "TEXT_POPUPS_UPGRADEDELIVERY_TITLE",
					BodyText = bodyText,
					BodyAlreadyTranslated = true,
					ConfirmAction = new PopUpButtonAction(this.delegateFunctionFail),
					ConfirmText = "TEXT_BUTTON_OK"
				};
				PopUpManager.Instance.TryShowPopUp(popup2, PopUpManager.ePriority.Default, null);
			}
			return;
		}
		if (upgradeCount > 0)
		{
			string text = string.Empty;
			int count = 5;
			foreach (string current2 in carListCarNames)
			{
				if (count <= 0)
				{
					break;
				}
				text = text + current2 + "\n";
				count--;
			}
			upgradeListCarNames = upgradeListCarNames.Distinct<string>().ToList<string>();
			foreach (string current3 in upgradeListCarNames)
			{
				if (count <= 0)
				{
					break;
				}
				text = text + string.Format(LocalizationManager.GetTranslation("TEXT_ARRIVAL_UPGRADE_ARRIVED"), current3) + "\n";
				count--;
			}
			bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_ARRIVAL_GENERAL"), text);
			PopUp popup3 = new PopUp
			{
				Title = "TEXT_POPUPS_DELIVERY_TITLE",
				BodyText = bodyText,
				BodyAlreadyTranslated = true,
				IsBig = true,
				ConfirmAction = new PopUpButtonAction(this.delegateFunctionFail),
				ConfirmText = "TEXT_BUTTON_OK",
                ImageCaption = "TEXT_NAME_AGENT",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab
			};
			PopUpManager.Instance.TryShowPopUp(popup3, PopUpManager.ePriority.Default, null);
			return;
		}
		this.carstring = string.Empty;
		int num4 = 0;
		foreach (string current4 in carListCarNames)
		{
			if (num4 >= 3)
			{
				this.carstring += current4;
				if (carListCarNames.Count > 4)
				{
					this.carstring += " ...";
				}
				this.carstring += "\n";
				break;
			}
			this.carstring = this.carstring + current4 + "\n";
			num4++;
		}
		if (carCount == 1)
		{
			bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_DELIVERY_SINGLE"), this.carstring);
		}
		else
		{
			bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_DELIVERY_MANY"), this.carstring);
		}
		PopUp popup4 = new PopUp
		{
			Title = "TEXT_POPUPS_DELIVERY_TITLE",
			BodyText = bodyText,
			BodyAlreadyTranslated = true,
			IsBig = true,
			CancelAction = new PopUpButtonAction(this.delegateFunctionFail),
			ConfirmAction = new PopUpButtonAction(this.delegateFunctionConfirm),
			SocialAction = new PopUpButtonAction(this.delegateFunctionSocial),
			CancelText = "TEXT_BUTTON_NO",
			ConfirmText = "TEXT_BUTTON_SHOW_ME",
            ImageCaption = "TEXT_NAME_AGENT",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab
        };
		PopUpManager.Instance.TryShowPopUp(popup4, PopUpManager.ePriority.Default, null);
	}

	public void delegateFunctionSocial()
	{
        //SocialController.Instance.OnShareButton(SocialController.MessageType.NEW_CAR, this.carstring, true, false);
	}

	public void delegateFunctionFail()
	{
		this.delegateArrival = null;
	}

	public void delegateFunctionConfirm()
	{
		if (this.delegateArrival == null)
		{
			return;
		}
		if (this.delegateArrival.arrivalType == ArrivalType.Car)
		{
            if (ScreenManager.Instance.CurrentScreen != ScreenID.CarSelect)
            {
                MyCarScreen.OnLoadCar = this.delegateArrival.carId;
                if (ScreenManager.Instance.IsScreenOnStack(ScreenID.CarSelect))
                {
                    ScreenManager.Instance.PopToScreen(ScreenID.CarSelect);
                }
                else
                {
                    ScreenManager.Instance.PushScreen(ScreenID.CarSelect);
                }
            }
            else
            {
                MyCarScreen carSelectScreen = ScreenManager.Instance.ActiveScreen as MyCarScreen;
                carSelectScreen.GoViewCar(this.delegateArrival.carId, true);
            }
		}
		if (this.delegateArrival.arrivalType == ArrivalType.Upgrade)
		{
            TuningScreen.StartScreenOnCurrentOwned = true;
            if (ScreenManager.Instance.CurrentScreen != ScreenID.Tuning)
            {
                TuningScreen.ExternalStartScreenOn = this.delegateArrival.upgradeType;
                TuningScreen.StartScreenOnCurrentOwned = true;
                if (ScreenManager.Instance.IsScreenOnStack(ScreenID.Tuning))
                {
                    ScreenManager.Instance.PopToScreen(ScreenID.Tuning);
                }
                else
                {
                    ScreenManager.Instance.PushScreen(ScreenID.Tuning);
                }
            }
            else
            {
                TuningScreen tuningScreen = ScreenManager.Instance.ActiveScreen as TuningScreen;
                tuningScreen.GoViewUpgrade(this.delegateArrival.upgradeType);
            }
		}
		this.delegateArrival = null;
	}

	public void ReduceDeliveryTime(int amount)
	{
		this.PriorityItem = this.GetArrival(this.PriorityItem);
		if (this.PriorityItem != null)
		{
			this.PriorityItem.dueTime = this.PriorityItem.dueTime.AddSeconds((double)(-(double)amount));
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
			if (this.PriorityItem.dueTime.CompareTo(this.getTime()) <= 0)
			{
				DeliverNow.doDeliver(this.PriorityItem);
                if (ScreenManager.Instance.CurrentScreen == ScreenID.Showroom)
                {
                    ShowroomScreen showroomScreen = ScreenManager.Instance.ActiveScreen as ShowroomScreen;
                    if (showroomScreen != null)
                    {
                        showroomScreen.onDeliverCar();
                    }
                }
			}
			this.PriorityItem = null;
		}
	}

	public void RequestPriorityDelivery(Arrival priorityItem)
	{
		this.PriorityItem = priorityItem;
		if (priorityItem.arrivalType == ArrivalType.Car) {
			VideoForRewardsManager.Instance.StartFlow(VideoForRewardConfiguration.eRewardID.CarDelivery);
		} else {
			VideoForRewardsManager.Instance.StartFlow(VideoForRewardConfiguration.eRewardID.PartDelivery);
		}
	}

	public Arrival GetPriorityDelivery()
	{
		return this.PriorityItem;
	}

	public void debugClearArrivals()
	{
		if (this.ourQueue != null)
		{
			this.ourQueue.Clear();
		}
	}
}
