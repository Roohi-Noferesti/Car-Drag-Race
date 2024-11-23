using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CleanDownManager : MonoBehaviour
{
    private const float numSecondsBetweenHandledMemoryWarnings = 10f;

    public bool cleaning;

    private Stopwatch stopwatch = new Stopwatch();

    private float LastHandledMemoryWarning;

    public static CleanDownManager Instance { get; private set; }

    private void Awake()
    {
        CleanDownManager.Instance = this;
        NativeEvents.ApplicationDidRecieveMemoryWarningEvent +=
            new NativeEvents_Delegate(this.applicationDidReceiveMemoryWarning);
    }

    public void OnTaskSwitch()
    {
        this.Run(false);
        GC.Collect();
    }

    public void OnScreenPopped(ScreenID id)
    {
        if (BaseDevice.ActiveDevice.IsLowMemoryDevice())
        {
            this.Run(false);
        }
        if (!BaseDevice.ActiveDevice.IsLowMemoryDevice() &&
            (id == ScreenID.LiveryCustomise || id == ScreenID.CarSelect || id == ScreenID.Showroom ||
             id == ScreenID.Manufacturer))
        {
            this.Run(true);
        }
    }

    public void OnCarLoadingFinished()
    {
    }

    public void OnBackgroundManagerLoadFinished()
    {
    }

    public void OnShowroomSlotClear()
    {
        this.Run(false);
    }

    public void OnShowroomSwitchCar()
    {
        this.Run(false);
    }

    public void OnEnterShowroom()
    {
        if (BaseDevice.ActiveDevice.IsLowMemoryDevice())
        {
            AsyncSwitching.Instance.ClearSlot(AsyncBundleSlotDescription.HumanCar);
        }
    }

    public void OnMapScreenCacheReleased()
    {
    }

    public void OnCrewProgressionDeactivate()
    {
        this.Run(false);
    }

    public void OnCrewProgressionUnloadCrew()
    {
        this.Run(false);
    }

    public void OnLiveryReady()
    {
    }

    public void OnEnterMap()
    {
    }

    public void OnShowAdvert()
    {
        this.Run(true);
    }

    public void OnEnterMultiplayerVersus()
    {
        if (BaseDevice.ActiveDevice.IsLowMemoryDevice())
        {
            this.Run(false);
        }
    }

    public void OnRaceInternationalVersus()
    {
        if (BaseDevice.ActiveDevice.IsLowMemoryDevice())
        {
            this.Run(false);
        }
    }

    public void OnTierXThemeChanged()
    {
        this.Run(false);
    }

    public void Run(bool forceCacheClearing = false)
    {
        if (forceCacheClearing || BaseDevice.ActiveDevice.IsLowMemoryDevice())
        {
            CachedBundlePool.ClearCache();
        }
        if (forceCacheClearing)
        {
            TexturePack.ClearCache();
        }
        base.StartCoroutine(this.BusinessTimeCoroutine());
    }

    private void applicationDidReceiveMemoryWarning()
    {
        float time = Time.time;
        if (this.LastHandledMemoryWarning + 10f < time)
        {
            this.Run(true);
            this.LastHandledMemoryWarning = time;
        }
    }

    private IEnumerator BusinessTimeCoroutine()
    {
        if (!cleaning)
        {
            cleaning = true;
            stopwatch.Reset();
            stopwatch.Start();
            yield return Resources.UnloadUnusedAssets();
        }
        stopwatch.Stop();
        cleaning = false;
    }
}
