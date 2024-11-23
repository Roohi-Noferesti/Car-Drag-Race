using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KingKodeStudio;
using UnityEngine;
using Debug = UnityEngine.Debug;

[AddComponentMenu("GT/Logic/Player/PlayerProfileManager")]
public class PlayerProfileManager : MonoBehaviour
{
	private bool _requestImmediateActiveSave;

	private bool _requestConvenientActiveSave;

	private bool _playerProfileRequiresKickback;

    public PlayerProfilePendingTransactions PendingTransactions;

    public static PlayerProfileManager Instance
	{
		get;
		private set;
	}

	public PlayerProfile ActiveProfile
	{
		get; set;
	}

    public PlayerProfile RecoveredProfile
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
        UserManager.LoggedInEvent += new UserLoggedInDelegate(PlayerProfileManager.Instance.OnUserLoggedIn);
        UserManager.UserChangedEvent += Instance.OnUserChanged;
        ApplicationManager.WillResignActiveEvent += Instance.OnApplicationWillResignActive;
	    CarDatabase.CarDatabaseUpdatedEvent += GiveInitialCar;
		AssetSystemManager.JustKicked += this.GiveInitialCarWithReason;
    }

    private void OnUserLoggedIn()
    {
        if (this.ActiveProfile != null && this.ActiveProfile.Name == UserManager.Instance.currentAccount.Username)
        {
            return;
        }
    }

    public void GivePlayerAllCars()
    {
        if (string.IsNullOrEmpty(ActiveProfile.CurrentlySelectedCarDBKey))
            ActiveProfile.CurrentlySelectedCarDBKey = CarDatabase.Instance.GetDefaultCar().Key;
        ActiveProfile.CarsOwned = new List<CarGarageInstance>();
        var cars = CarDatabase.Instance.GetAllCars();
        for (int i = 0; i < cars.Count; i++)
        {
            var car = new CarGarageInstance()
            {
                CarDBKey = cars[i].Key,
                CurrentPPIndex = cars[i].BasePerformanceIndex
            };

            car.SetupNewGarageInstance(cars[i]);
            ActiveProfile.CarsOwned.Add(car);
            ActiveProfile.HasBoughtFirstCar = true;
        }
    }

    private void SetupTestOwnedCar()
    {
        if (!string.IsNullOrEmpty(ActiveProfile.CurrentlySelectedCarDBKey))
        {
            var carInfo = CarDatabase.Instance.GetCar(ActiveProfile.CurrentlySelectedCarDBKey);
            var car = new CarGarageInstance()
            {
                CarDBKey = carInfo.Key,
                CurrentPPIndex = carInfo.BasePerformanceIndex
            };

            car.SetupNewGarageInstance(carInfo);
            ActiveProfile.CarsOwned.Add(car);
        }
    }

	private void Update()
	{
		if (this._playerProfileRequiresKickback && GTSystemOrder.SystemsReady)
		{
            if (!PopUpManager.Instance.isShowingPopUp)
            {
                this.DoProfileReplacedDuringLoadPopup();
            }
			return;
		}
		if (this._requestImmediateActiveSave)
		{
			this._requestImmediateActiveSave = false;
			this._requestConvenientActiveSave = false;
			this.ActiveProfile.Save();
            PlayerProfileWeb.QueueSync();
		}
		if (this.ActiveProfile != null)
		{
            PlayerProfileWeb.Update();
		}
	}

	public void OnApplicationWillResignActive()
	{
		if (this.ActiveProfile != null && (ScreenManager.Instance==null || ScreenManager.Instance.CurrentScreen!=ScreenID.Options))
		{
            PlayerProfileWeb.Sync();
            WebRequestQueue.Instance.ProcessQueue();
		}
	}

	public void OnUserChanged()
	{
        if (this.ActiveProfile != null)
        {
            NotificationManager.Active.ClearAllNotifications();
        }
        this.LoadActiveProfile();
        this.ActiveProfile.UserStartedLastSession = GTDateTime.Now;
	}

    public void LoadActiveProfile()
    {
        this.ActiveProfile = new PlayerProfile(UserManager.Instance.currentAccount.Username);
        this.PendingTransactions = new PlayerProfilePendingTransactions();
        if (this.ActiveProfile.Load(EProfileFileType.account))
        {
            this.PendingTransactions.Load();
            MemoryValidator.Instance.Mangle<MangledCashSpent>(MangleInvoker.ActiveProfileLoad);
            MemoryValidator.Instance.Mangle<MangledCashEarned>(MangleInvoker.ActiveProfileLoad);
            MemoryValidator.Instance.Mangle<MangledGoldSpent>(MangleInvoker.ActiveProfileLoad);
            MemoryValidator.Instance.Mangle<MangledGoldEarned>(MangleInvoker.ActiveProfileLoad);
        }
        else
        {
            this.PendingTransactions.ClearTransactions();
            this.ActiveProfile.CreateDefault();
            this.RequestImmediateSaveActiveProfile();
        }

        //This cuase error because we do not use identity anymore because of access exception
        //if (!BaseIdentity.ActivePlatform.ConfirmIdentity(this.ActiveProfile))
        //{
        //    base.StartCoroutine(this.SendProfileMigrationCheatDetected());
        //}
        //LogUtility.Log("UserID: " + ActiveProfile.ID + "   ,   DeviceID: " + ActiveProfile.DeviceUniqueIdentifier);
    }

    public void OnScreenChanged(ScreenID zNewScreenId)
    {
        //Debug.Log("screenchanged    " + _requestConvenientActiveSave);
		this.PerformConvenientSave();
	}

	public void OnSceneChanged()
	{
        this.PerformConvenientSave();
	}

	public static void CurrentCarNewPP(string carName, eCarTier tier, int ppIndex, float newQMTime)
	{
		PlayerProfile activeProfile = Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return;
		}
		CarGarageInstance currentCar = activeProfile.GetCurrentCar();
		currentCar.CurrentPPIndex = ppIndex;
		currentCar.TightLoopQuarterMileTime = newQMTime;
	}

	[DebuggerHidden]
	private IEnumerator SendProfileMigrationCheatDetected()
	{
	    yield return 0;
	    //PlayerProfileManager.<SendProfileMigrationCheatDetected>c__Iterator13 <SendProfileMigrationCheatDetected>c__Iterator = new PlayerProfileManager.<SendProfileMigrationCheatDetected>c__Iterator13();
	    //<SendProfileMigrationCheatDetected>c__Iterator.<>f__this = this;
	    //return <SendProfileMigrationCheatDetected>c__Iterator;
	}

	public void SendMetricsForPlayerMigrated(string profileName, string profileUDID)
	{
		base.StartCoroutine(this.SendMetricsForPlayerMigratedCoroutine(profileName, profileUDID));
	}

	[DebuggerHidden]
	private IEnumerator SendMetricsForPlayerMigratedCoroutine(string profileName, string profileUDID)
	{
	    yield return 0;
	    //PlayerProfileManager.<SendMetricsForPlayerMigratedCoroutine>c__Iterator14 <SendMetricsForPlayerMigratedCoroutine>c__Iterator = new PlayerProfileManager.<SendMetricsForPlayerMigratedCoroutine>c__Iterator14();
	    //<SendMetricsForPlayerMigratedCoroutine>c__Iterator.profileName = profileName;
	    //<SendMetricsForPlayerMigratedCoroutine>c__Iterator.profileUDID = profileUDID;
	    //<SendMetricsForPlayerMigratedCoroutine>c__Iterator.<$>profileName = profileName;
	    //<SendMetricsForPlayerMigratedCoroutine>c__Iterator.<$>profileUDID = profileUDID;
	    //return <SendMetricsForPlayerMigratedCoroutine>c__Iterator;
	}

	public void RequestImmediateSaveActiveProfile()
	{
		this._requestImmediateActiveSave = true;
	}

	public void RequestConvenientSaveActiveProfile()
	{
		this._requestConvenientActiveSave = true;
	}

	private void PerformConvenientSave()
	{
		if (this._requestConvenientActiveSave)
		{
			this._requestConvenientActiveSave = false;
			this._requestImmediateActiveSave = false;
			this.ActiveProfile.Save();
            this.PendingTransactions.Save();
			PlayerProfileWeb.QueueSync();
		}
	}

	public void GiveInitialCar()
	{
		if (this.ActiveProfile != null && this.ActiveProfile.GiveInitialCar())
		{
			this.RequestImmediateSaveActiveProfile();
		}
	}

	public void GiveInitialCarWithReason(AssetSystemManager.Reason reason)
	{
		this.GiveInitialCar();
	}

	private void DoProfileReplacedDuringLoadPopup()
	{
		if (SceneLoadManager.Instance == null || (SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Frontend))// && ScreenManager.Active.CurrentScreen == ScreenID.VSDummy))
		{
			return;
		}
		PopUp popup = new PopUp
		{
			Title = "TEXT_ERROR_POPUP_TITLE",
			BodyText = "TEXT_POPUP_USER_ID_NOT_ON_SERVER",
			ConfirmAction = new PopUpButtonAction(this.OnProfileReplacedDuringLoadConfirmed),
			ConfirmText = "TEXT_BUTTON_CONTINUE"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private void OnProfileReplacedDuringLoadConfirmed()
	{
		this._playerProfileRequiresKickback = false;
        this.LoadActiveProfile();
		AssetSystemManager.Instance.KickBackToSafePlaceAndReload(AssetSystemManager.Reason.OnUserChanged);
	}

    public bool LoadRecoveredProfile()
    {
        PlayerProfileFile.SetToRootDirectory();
        this.RecoveredProfile = new PlayerProfile(UserManager.Instance.currentAccount.Username);
        if (!this.RecoveredProfile.Load(EProfileFileType.account))
        {
            this.ClearRecoveredProfile();
            PlayerProfileFile.SetToDefaultDirectory();
            return false;
        }
        PlayerProfileFile.SetToDefaultDirectory();
        return true;
    }

    public void ClearRecoveredProfile()
    {
        this.RecoveredProfile = null;
    }

    public bool HasPendingTransactions()
    {
        return this.PendingTransactions != null && this.PendingTransactions.GetTransactionCount() > 0;
    }

    public void ClearPendingTransactions()
    {
        this.PendingTransactions.ClearTransactions();
    }
}
