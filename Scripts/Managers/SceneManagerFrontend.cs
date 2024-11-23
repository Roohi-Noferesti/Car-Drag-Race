using System.Collections.Generic;
using System.Globalization;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;

public class SceneManagerFrontend : MonoBehaviour
{
    //[SerializeField] private bool m_log = true;
    private static ScreenID OverrideScreen;
    private static ScreenID[] OverrideScreenHistory = new ScreenID[0];
    public static void SetScreenToPushWhenInFrontend(ScreenID zScreenName, params ScreenID[] zScreenHistory)
	{
        OverrideScreen = zScreenName;
		OverrideScreenHistory = zScreenHistory;
	}

    private void Awake()
    {
        GTDebug.Log(GTLogChannel.SceneLoadManager, "Frontend here ...");
        //LocalizationManager.CurrentLanguage = "English";
        //UnityEngine.Profiling.Profiler.maxNumberOfSamplesPerFrame = -1;
        bool flag = Z2HInitialisation.Instance.Init();
        if (flag)
        {
            Z2HInitialisation.Instance.okToGoBig = true;
        }

        UICacheManager.Instance.CreateInstances();
        CarReflectionMapManager.LoadCubemap(CarReflectionMapManager.ReflectionTexType.WorkshopDay);
        CarReflectionMapManager.LoadCubemap(CarReflectionMapManager.ReflectionTexType.WorkshopNight);
        CarReflectionMapManager.LoadCubemap(CarReflectionMapManager.ReflectionTexType.Showroom);
        BackgroundManager.Load();
        SceneLoadManager.Instance.CompleteLoadingScene();
        if (SceneLoadManager.Instance.LastScene == SceneLoadManager.Scene.Race)
        {
            GTAdManager.Instance.ResetAdProviders();
            PlayerProfileManager.Instance.ActiveProfile.UpdateCurrentPhysicsSetup();
        }
        MapScreenCache.Load();
        if (PlayerProfileManager.Instance != null)
        {
            PlayerProfileManager.Instance.OnSceneChanged();
        }

        if (GarageScreen.Instance != null)
        {
            GarageScreen.Instance.OnSceneChanged();
        }

        if (ScreenManager.Instance.ActiveScreen != null)
        {
            //Debug.Log("pop here");
            if (ScreenManager.Instance.CurrentScreen != ScreenID.Dummy)
            {
            }
            ScreenManager.Instance.PopScreen();
        }
        if (SceneLoadManager.Instance.LastScene == SceneLoadManager.Scene.Race)
        {
            if (ScreenManager.Instance.NominalNextScreen() != ScreenID.Workshop)
            {
                //Debug.Log("nominal not workshop");
                if (ScreenManager.Instance.IsScreenOnStack(ScreenID.Workshop))
                {
                    //Debug.Log("workshop is on stack");
                    ScreenManager.Instance.PopToScreen(ScreenID.Workshop);
                }
                else
                {
                    //Debug.Log("workshop is not on stack");
                    ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.Workshop, new ScreenID[]
					{
						ScreenID.Home
					});
                }
            }
            if (!RaceEventInfo.Instance.IsRelayEvent())
            {
                bool flag2 = CrewProgressionSetup.PostRaceSetupCrewProgressionScreen(OverrideScreen);
                flag2 |= CrewProgressionSetup.PostRaceSetupForNarrativeScene(OverrideScreen);
                if (flag2)
                {
                    OverrideScreen = ScreenID.Invalid;
                    return;
                }
            }
            if (OverrideScreen != ScreenID.Invalid)
            {
                GTDebug.Log(GTLogChannel.SceneLoadManager,OverrideScreen.ToString());
                ScreenManager.Instance.PushScreenWithFakedHistory(OverrideScreen, OverrideScreenHistory);
                OverrideScreen = ScreenID.Invalid;
            }
            if (RaceEventInfo.Instance.IsDailyBattleEvent)
            {
                //ScreenManager.Active.pushScreenImmediately(ScreenID.DailyBattleCalender.ToString());
            }
            if (RaceEventInfo.Instance.CurrentEvent.IsSMPRaceEvent())
            {
                ScreenManager.Instance.PushScreen(ScreenID.CareerModeMap);
                ScreenManager.Instance.PushScreen(ScreenID.SMPLobby);
            }

            //Since there is no automatic opening screen in our screenManager system we should open pushed screen here
            //ScreenManager.Active.openTopScreen();
        }
        else
        {
            ScreenManager.Instance.PushScreen(ScreenID.Splash);

            //Debug.Log("show language : " + showLanguage);
            if (string.IsNullOrEmpty(UserManager.Instance.currentAccount.Language))
            {
                try {
                    FindObjectOfType<SplashLoadingOverlay>().Close();
                }
                catch {}
                
                if(!BuildType.IsAppTuttiBuild)
                    ScreenManager.Instance.PushScreen(ScreenID.Language);
            }

        }
    }


    private static void GoToRace()
    {
        if (Camera.main != null)
            Camera.main.enabled = false;
        if (ScreenManager.Instance.ActiveScreen.ID != ScreenID.VSDummy
            && ScreenManager.Instance.ActiveScreen.ID!=ScreenID.VS
            && ScreenManager.Instance.ActiveScreen.ID != ScreenID.OnlineVs)
        {
            ScreenManager.Instance.PushScreen(ScreenID.Dummy);
            ScreenManager.Instance.UpdateImmediately();
        }
        CarStatsCalculator.Instance.DestroyFrontendPhysics();
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        MapScreenCache.Unload();
        TexturePack.ClearCache();
        LeaderboardScreen.ClearCache();
        GarageScreen.ResetNativeBannerCheck();
        CarReflectionMapManager.UnloadCubemap(CarReflectionMapManager.ReflectionTexType.WorkshopDay);
        CarReflectionMapManager.UnloadCubemap(CarReflectionMapManager.ReflectionTexType.WorkshopNight);
        UICacheManager.Instance.DestroyInstances();
        GarageScreen.WorldTourPopupEvaulated = false;
        //WebRequestQueue.Instance.ResetQueue();
        //WebRequestQueueRTW.Instance.ResetQueue();
        //FriendsRewardManager.Instance.ClearLastAwardedCar();
        GarageScreen.HasShownCrewChatter = false;
        PlayerProfileManager.Instance.ActiveProfile.ClearLastAcquiredCar();
        PlayerProfileManager.Instance.ActiveProfile.RacesStartedThisSession++;
        var useloadScreen = RaceEventInfo.Instance.CurrentEvent.IsTutorial();
        SceneLoadManager.Instance.LoadScene(SceneLoadManager.Scene.Race, useloadScreen);
    }

    public static void ButtonStart()
    {
        //MetricsCalculate.SetTimeToNow();
        BackgroundManager.UnloadAll();
        GoToRace();
    }
}
