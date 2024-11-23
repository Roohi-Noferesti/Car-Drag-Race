using System;
using UnityEngine;

public class FuelManager : MonoBehaviour
{
	public Action OnFuelPurchase;

	public Action OnFuelAutoReplenish;

	public Action OnFuelSpend;

	public Action OnFuelTimerUpdated;

	public Action OnFuelTankUpgraded;

	public Action OnFuelTankUpgradeRevoked;

	public Action OnFuelUnlimitedRevoked;

	private int lastDeltaSeconds = -1;

	private int lastUnlimitedFuelSeconds = -1;

	private bool canPlayAnimations = true;

	private int pipsWaitingToAdd;

	public bool StopFuelRefills;

    private DateTime LastNotificationTriggerTime = GTDateTime.Now;

	public static FuelManager Instance
	{
		get;
		private set;
	}

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
        LastNotificationTriggerTime = GTDateTime.Now;
    }

    public void OnProfileChanged()
	{
		this.UpdateAutoReplenish(true);
	}

	public void FuelLockedState(bool inLocked)
	{
		this.canPlayAnimations = !inLocked;
		if (!inLocked)
		{
			this.CheckLatentFuelAnimations();
		}
	}

	public int FillTank(FuelAnimationLockAction latentAnimationState)
	{
        AudioManager.Instance.PlaySound(AudioEvent.Frontend_Fuel_Refill, Camera.main.gameObject);
	    var zFuelToAdd = this.CurrentMaxFuel() - PlayerProfileManager.Instance.ActiveProfile.FuelPips;
		return this.AddFuel(zFuelToAdd, FuelReplenishTimeUpdateAction.UPDATE, latentAnimationState);

	}

	public void EmptyTank()
	{
		PlayerProfileManager.Instance.ActiveProfile.SetFuelAutoReplenishTimestampToNow();
        this.SpendFuel(PlayerProfileManager.Instance.ActiveProfile.FuelPips);
	}

	public int CurrentMaxFuel()
	{
		if (PlayerProfileManager.Instance.ActiveProfile.HasUpgradedFuelTank)
		{
			return GameDatabase.Instance.IAPs.UpgradedGasTankSize();
		}

	    return GameDatabase.Instance.CurrenciesConfiguration != null
	        ? GameDatabase.Instance.CurrenciesConfiguration.Fuel.FuelTankSize
	        : 10;
	}

	public int AddFuel(int zFuelToAdd, FuelReplenishTimeUpdateAction replenishTimeUpdateAction, FuelAnimationLockAction animationState)
	{
		if (zFuelToAdd < 0)
		{
			return 0;
		}
		if (replenishTimeUpdateAction == FuelReplenishTimeUpdateAction.UPDATE)
		{
			PlayerProfileManager.Instance.ActiveProfile.SetFuelAutoReplenishTimestampToNow();
		}
		var result = PlayerProfileManager.Instance.ActiveProfile.AddFuel(zFuelToAdd);
		if (animationState == FuelAnimationLockAction.OBEY)
		{
			this.pipsWaitingToAdd += zFuelToAdd;
		}
		if (this.OnFuelAutoReplenish != null && (animationState == FuelAnimationLockAction.DONTCARE || this.canPlayAnimations))
		{
			this.OnFuelAutoReplenish();
		}
		if (!this.IsWaitingToReplenish() && this.OnFuelTimerUpdated != null)
		{
			this.OnFuelTimerUpdated();
		}

		return result;
	}

	public int GetFuel()
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return 0;
		}
        return PlayerProfileManager.Instance.ActiveProfile.FuelPips;
	}

	public bool SpendFuel(int pips = 1)
	{
		var flag = PlayerProfileManager.Instance.ActiveProfile.SpendFuel(pips);
		if (flag)
		{
            //PrizeProgression.AddProgress(PrizeProgressionType.FuelSpent, (float)pips);
			if (this.OnFuelSpend != null)
			{
				this.OnFuelSpend();
			}
            var fuel = PlayerProfileManager.Instance.ActiveProfile.FuelPips;
			if (fuel == this.CurrentMaxFuel() - pips)
			{
				PlayerProfileManager.Instance.ActiveProfile.SetFuelAutoReplenishTimestampToNow();
			}
		}
		return flag;
	}

	public DateTime TimeWhenWillAutoRefill()
	{
		var value = PlayerProfileManager.Instance.ActiveProfile.LastFuelAutoReplenishedTime();
        var totalSecondsFromLastFill = (int)GTDateTime.Now.Subtract(value).TotalSeconds;
		var fuelDiff = this.CurrentMaxFuel() - this.GetFuel();
		var secondsToFullFill = GameDatabase.Instance.CareerConfiguration.FuelReplenishTime * fuelDiff;
        return GTDateTime.Now.AddSeconds((double)(secondsToFullFill - totalSecondsFromLastFill));
	}

	public int TimeSinceAutoReplenish()
	{
		var value = PlayerProfileManager.Instance.ActiveProfile.LastFuelAutoReplenishedTime();
        return (int)GTDateTime.Now.Subtract(value).TotalSeconds;
	}

	public int TimeUntilNextReplenish()
	{
		if (!this.IsWaitingToReplenish())
		{
			return GameDatabase.Instance.CareerConfiguration.FuelReplenishTime;
		}
		var num = this.TimeSinceAutoReplenish();
		return Mathf.Clamp(GameDatabase.Instance.CareerConfiguration.FuelReplenishTime - num, 0, GameDatabase.Instance.CareerConfiguration.FuelReplenishMaxDisplay);
	}

	public bool IsWaitingToReplenish()
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return false;
		}
        var fuel = PlayerProfileManager.Instance.ActiveProfile.FuelPips;
		return fuel < this.CurrentMaxFuel();
	}

	private void Update()
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
			return;
		if (!GameDatabase.Instance.IsReady())
			return;
		if (SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Race && RaceController.RaceIsRunning())
			return;
		
		this.UpdateAutoReplenish(false);
        this.CheckUpdateNotification();
	}

	private void CheckLatentFuelAnimations()
	{
		if (this.pipsWaitingToAdd == 0)
		{
			return;
		}
		if (!this.canPlayAnimations)
		{
			return;
		}
		this.pipsWaitingToAdd = 0;
		this.OnFuelAutoReplenish();
	}

	public void UpdateAutoReplenish(bool zForceUpdate)
	{
		if (this.StopFuelRefills)
		{
			return;
		}
  //       TimeSpan timeRemaining = UnlimitedFuelManager.TimeRemaining;
	 //    bool isActive =UnlimitedFuelManager.IsActive;
		// if (isActive)
		// {
  //           if (this.OnFuelTimerUpdated != null && (timeRemaining.Seconds != this.lastUnlimitedFuelSeconds || zForceUpdate))
  //           {
  //               this.OnFuelTimerUpdated();
  //               this.lastUnlimitedFuelSeconds = timeRemaining.Seconds;
  //           }
  //           int num = this.CurrentMaxFuel() - PlayerProfileManager.Instance.ActiveProfile.FuelPips;
		// 	if (num > 0)
		// 	{
  //               if (UnlimitedFuelManager.IsPlayerClockCheating())
  //               {
  //                   UnlimitedFuelManager.Revoke();
  //               }
  //               else
  //               {
  //                   this.AddFuel(num, FuelReplenishTimeUpdateAction.UPDATE, FuelAnimationLockAction.OBEY);
  //               }
		// 	}
		// 	return;
		// }
		// if (this.lastUnlimitedFuelSeconds != -1)
		// {
  //           UnlimitedFuelManager.Revoke();
		// 	this.OnFuelUnlimitedRevoked();
		// 	this.lastUnlimitedFuelSeconds = -1;
		// }
		if (!zForceUpdate && !this.IsWaitingToReplenish())
		{
			return;
		}
        var fuel = PlayerProfileManager.Instance.ActiveProfile.FuelPips;
		var num2 = this.TimeSinceAutoReplenish();
		var num3 = num2 / GameDatabase.Instance.CareerConfiguration.FuelReplenishTime;
		if (num2 != this.lastDeltaSeconds || zForceUpdate)
		{
			if (this.OnFuelTimerUpdated != null)
			{
				this.OnFuelTimerUpdated();
			}
			this.lastDeltaSeconds = num2;
		}
		if (num3 > 0)
		{
			if (fuel >= this.CurrentMaxFuel())
			{
				return;
			}
			num3 = Mathf.Clamp(num3, 0, this.CurrentMaxFuel() - fuel);
			this.AddFuel(num3, FuelReplenishTimeUpdateAction.UPDATE, FuelAnimationLockAction.OBEY);
		}
	}

	public void CheckUpdateNotification()
	{
		var dateTime = this.TimeWhenWillAutoRefill();
        if (dateTime < this.LastNotificationTriggerTime  || Mathf.Abs((int)(dateTime - this.LastNotificationTriggerTime).TotalSeconds) < 10)
		{
			return;
		}
		this.LastNotificationTriggerTime = dateTime;
        NotificationManager.Active.UpdateFuelNotification(dateTime);
        //Debug.Log("notify penalty : " + dateTime);
	}
}
