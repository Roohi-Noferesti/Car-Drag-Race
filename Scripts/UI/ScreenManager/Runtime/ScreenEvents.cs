using System;
using KingKodeStudio;

public class ScreenEvents : MonoSingleton<ScreenEvents>
{
    public static event Action<string> ScreenLoadingStarted;
    public static event Action<string, HUDScreen> ScreenLoadingFinished;
    public static event Action<HUDScreen> ScreenDestroyed;

    public virtual void OnScreenLoadingStarted(string obj)
    {
        var handler = ScreenLoadingStarted;
        if (handler != null) handler(obj);
    }

    public virtual void OnScreenLoadingFinished(string arg1, HUDScreen arg2)
    {
        var handler = ScreenLoadingFinished;
        if (handler != null) handler(arg1, arg2);
    }

    public virtual void OnScreenVisibilityChanging(HUDScreen.ScreenEventArgs obj)
    {
        //var handler = ScreenVisibilityChanging;
        //if (handler != null) handler(obj);
    }

    public virtual void OnScreenDestroyed(HUDScreen obj)
    {
        var handler = ScreenDestroyed;
        if (handler != null) handler(obj);
    }
}
