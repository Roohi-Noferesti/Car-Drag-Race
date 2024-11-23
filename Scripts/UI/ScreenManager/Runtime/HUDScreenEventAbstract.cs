using KingKodeStudio;
using UnityEngine;

[RequireComponent(typeof(HUDScreen))]
public abstract class HUDScreenEventAbstract : MonoBehaviour
{
    protected HUDScreen hudScreen;
    void Awake()
    {
        hudScreen = GetComponent<HUDScreen>();
        //hudScreen.ScreenVisibilityChanged += OnScreenVisibilityChanged;
        //hudScreen.ScreenVisibilityChanging += OnScreenVisibilityChanging;

        Init();
    }

    protected abstract void OnFocused(HUDScreen obj);

    protected abstract void Init();

    //protected abstract void OnScreenVisibilityChanging(HUDScreen.HudScreenEventArgs obj);

    //protected abstract void OnScreenVisibilityChanged(HUDScreen.HudScreenEventArgs obj);
}
