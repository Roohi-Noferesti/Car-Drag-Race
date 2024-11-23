using System;
using System.Collections.Generic;
using I2.Loc;
using Objectives;
using UnityEngine;

public class GTSystemOrder
{
	public static bool systemsReady;

	public static bool BeenTriggered;

	public static AssetSystemManager.Reason reasonForShutdown;

	public static List<Action> AsynchActions;

	public static bool SystemsReady
	{
		get
		{
			return systemsReady;
		}
	}

	public static void ShutDownGameSystems(AssetSystemManager.Reason reason)
	{
		if (systemsReady)
		{
			systemsReady = false;
			BubbleManager.Instance.KillAllMessages();
            WebRequestQueue.Instance.enabled = false;
            WebRequestQueueRTW.Instance.enabled = false;
            ProductManager.Instance.enabled = false;
            AssetDatabaseVersionPoll.Instance.enabled = false;
            UserManager.Instance.enabled = false;
			FuelManager.Instance.enabled = false;
            CommonUI.Instance.PlayAnimation(false);
            //CommonUI.Instance.gameObject.SetActive(false);
			CarInfoUI.Instance.gameObject.SetActive(false);
			ArrivalManager.Instance.enabled = false;
            GameCenterController.Instance.enabled = false;
			PlayerProfileManager.Instance.enabled = false;
            AppStore.Instance.enabled = false;
            ServerAccountConnectActions.Instance.enabled = false;
            SocialFriendsManager.Instance.enabled = false;
            GTAdManager.Instance.Shutdown();
            BundleOfferController.Instance.CleanUp();
            MetricsIntegration.Instance.enabled = false;
            ObjectiveManager.Instance.enabled = false;
		    LegacyLeaderboardManager.Instance.ShutDown();
            LegacyLeaderboardManager.Instance.enabled = false;
            if (!MultiplayerUtils.DisableMultiplayer)
            {
                RTWStatusManager.Instance.enabled = false;
                NetworkReplayManager.Instance.enabled = false;
            }
            GameDatabase.Instance.ShutDown();
            CarDatabase.Instance.ShutDown();
			ManufacturerDatabase.Instance.Shutdown();
            //TierXManager.Instance.OnAssetSystemUnload();
            //LocalizationManager.OnAssetSystemUnload();
            SeasonServerDatabase.Instance.Shutdown();
            //LumpManager.Instance.Shutdown();
            SeasonCountdownManager.DisconnectFromSeasonServerDatabase();
            AssetBackgroundDownloader.Instance.Unload();
            RPBonusManager.Unload();
            RYFStatusManager.Instance.Shutdown();
            //TierXManager.Instance.ShutDown();
            RelayManager.ResetRelayData();
            AssetLoaderPool.Instance.Clear();
			CachedBundlePool.DeleteAllBundlesFromMemory();
		    DailyPrizeManager.Instance.Shutdown();
            if (reason == AssetSystemManager.Reason.ChangedServerStack || reason == AssetSystemManager.Reason.ChangedBranch || reason == AssetSystemManager.Reason.OnLoadProgression)
            {
                AssetDatabaseClient.Instance.CleanCache();
                AssetDatabaseClient.Instance.Directory.CleanCache();
            }
		}
	}

	public static void StartUpGameSystems()
	{
		BeenTriggered = true;
		AsynchActions = new List<Action>
		{
            new Action(AssetBackgroundDownloader.Instance.OnStartup),
            new Action(AssetDatabaseClient.Instance.LoadDatabaseFromLocalStorageAction),
            new Action(ManufacturerDatabase.Instance.Load),
			new Action(CarDatabase.Instance.OnAssetDatabaseLoaded),
            new Action(GameDatabase.Instance.Initialise),
            new Action(MetricsIntegration.Instance.StartMetricsSessions),
            //new Action(LocalizationManager.OnAssetSystemLoad),
            //new Action(DynamicFontManager.OnAssetSystemLoad),
            new Action(TierXManager.Instance.OnAssetSystemLoad),
            //new Action(LumpManager.Instance.Initialise)
		};
        GTSystemOrderAsync.Instance.TriggerAsynchActions();
	}

	public static bool CanGoIntoGame()
	{
		return !BeenTriggered || systemsReady;
	}

	public static void CompleteGameSystemStartup()
	{
        MenuAudio.Instance.OnProfileChanged();
        FuelManager.Instance.OnProfileChanged();
        ObjectiveManager.Instance.OnProfileChanged();
        GameCenterController.Instance.OnProfileChanged();
        //NumberPlateManager.Instance.RenderPlayerNumberPlate(null);
        CarSnapshotManager.Instance.OnProfileChanged();
        DifficultyManager.OnProfileChanged();
        NetworkReplayManager.Instance.OnProfileChanged();
        SeasonPrizeSystemManager.Instance.OnUserChanged();
        SeasonCountdownManager.ConnectToSeasonServerDatabase();
        RYFStatusManager.Instance.StartUp();
        DailyBattleRewardManager.Instance.StartUp();
        ServerAccountConnectActions.Instance.enabled = true;
        ArrivalManager.Instance.enabled = true;
        CommonUI.Instance.gameObject.SetActive(true);
        FuelManager.Instance.enabled = true;
        UserManager.Instance.enabled = true;
        AssetDatabaseVersionPoll.Instance.enabled = true;
        WebRequestQueue.Instance.enabled = true;
        WebRequestQueueRTW.Instance.enabled = true;
        GameCenterController.Instance.enabled = true;
        PlayerProfileManager.Instance.enabled = true;
        AppStore.Instance.enabled = true;
        SocialFriendsManager.Instance.enabled = true;
        CarInfoUI.Instance.gameObject.SetActive(true);
        CarInfoUI.Instance.ShowCarStats(false);
		PlayerProfileManager.Instance.ActiveProfile.UpdateOnlineRacesWonToday();
		PlayerProfileManager.Instance.ActiveProfile.UpdateCurrentPhysicsSetup();
		PlayerProfileManager.Instance.ActiveProfile.ResetDailyBattleDaysIfRequired();
        PlayerProfileManager.Instance.ActiveProfile.CheckProfileHasUpdated();
        GTAdManager.Instance.Initialise();
        MetricsIntegration.Instance.enabled = true;
        InvitationManager.Instance.enabled = true;
	    ObjectiveManager.Instance.enabled = true;
        LegacyLeaderboardManager.Instance.enabled = true;
        RPBonusManager.Initialise();
        if (!MultiplayerUtils.DisableMultiplayer)
        {
            RTWStatusManager.Instance.enabled = true;
            NetworkReplayManager.Instance.enabled = true;
        }
        DailyPrizeManager.Instance.OnProfileChanged();
        GameDatabase.Instance.MiniStore.Configuration.Initialize();
        AssetSystemManager.Instance.InvokeJustKickedCallback(GTSystemOrder.reasonForShutdown);
		systemsReady = true;
		BeenTriggered = false;

#if UNITY_EDITOR
        if(GameDatabase.Instance.CarsConfiguration.GiveAllCars)
            PlayerProfileManager.Instance.GivePlayerAllCars();
#endif
	}

	public static bool AreAsyncGameSystemsReady()
	{
		var notReadySystemName = WhichGameSystemIsNotReady();
        return notReadySystemName == null;
	}

	public static int CountAsynchSystemsReady()
	{
		int num = 0;
		if (GameDatabase.Instance.IsReady())
		{
			num++;
		}
        if (ManufacturerDatabase.Instance.isReady)
        {
            num++;
        }
		if (CarDatabase.Instance.isReady)
		{
			num++;
		}
        //if (LocalizationManager.IsReady)
        //{
        //    num++;
        //}
        //if (DynamicFontManager.Instance.IsReady())
        //{
        //    num++;
        //}
        if (TierXManager.Instance.IsReady())
        {
            num++;
        }
        //if (LumpManager.Instance.IsReady)
        //{
        //    num++;
        //}
		return num;
	}

	public static string WhichGameSystemIsNotReady()
	{
		if (!GameDatabase.Instance.IsReady())
		{
			return GameDatabase.Instance.WhichConfigurationDatabaseIsNotReady();
		}
        if (!ManufacturerDatabase.Instance.isReady)
        {
            return "ManufacturerDatabase";
        }
		if (!CarDatabase.Instance.isReady)
		{
			return "CarDatabase";
		}
        //if (!LocalizationManager.IsReady)
        //{
        //    return "LocalisationManager";
        //}
        //if (!DynamicFontManager.Instance.IsReady())
        //{
        //    return "DynamicFontManager";
        //}
        if (!TierXManager.Instance.IsReady())
        {
            return "TierXManager";
        }
        //if (!LumpManager.Instance.IsReady)
        //{
        //    return "LumpManager";
        //}
		return null;
	}
}
