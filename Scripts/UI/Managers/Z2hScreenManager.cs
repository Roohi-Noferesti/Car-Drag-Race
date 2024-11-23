using System.Collections;
using System.Collections.Generic;
using KingKodeStudio;
using UnityEngine;

public class Z2hScreenManager : MonoBehaviour
{
    ////private HUDScreen.HudScreenEventArgs m_currentEvent;
    //private bool m_useFade;


    //public class ScreenTransitionName
    //{
    //    public string From;
    //    public string To;

    //    public ScreenTransitionName(string from, string to)
    //    {
    //        From = from;
    //        To = to;
    //    }
    //}

    //private List<ScreenID> m_nonBlackScreenTransitions = new List<ScreenID>
    //{
    //    ScreenID.Customise,
    //    ScreenID.BodyPaint,
    //    ScreenID.Stickers,
    //    ScreenID.FrontLightColors,
    //    ScreenID.Spoilers,
    //    ScreenID.RingColors,
    //    ScreenID.Dummy,
    //    ScreenID.LevelUp,
    //    ScreenID.LeagueChange,
    //    ScreenID.ChooseName,
    //    ScreenID.SelectAgent,
    //    ScreenID.PrizeOMatic,
    //    //ScreenID.Splash,
    //};
    //void Awake()
    //{
    //    //ScreenEvents.ScreenVisibilityChanging += StackManager_ScreenVisibilityChanging;
    //    //ScreenEvents.ScreenVisibilityChanged += StackManager_ScreenVisibilityChanged;
    //    ScreenEvents.ScreenLoadingFinished += ScreenEvents_ScreenLoadingFinished;
    //    ScreenManager.ScreenChanging += ScreenManager_ScreenChanging;
    //    ScreenManager.ScreenChanged += ScreenManager_ScreenChanged;
    //}

    //void OnDestroy()
    //{
    //    //ScreenEvents.ScreenVisibilityChanging -= StackManager_ScreenVisibilityChanging;
    //    //ScreenEvents.ScreenVisibilityChanged -= StackManager_ScreenVisibilityChanged;
    //    ScreenEvents.ScreenLoadingFinished -= ScreenEvents_ScreenLoadingFinished;
    //    ScreenManager.ScreenChanging -= ScreenManager_ScreenChanging;
    //    ScreenManager.ScreenChanged -= ScreenManager_ScreenChanged;
    //}

    //public bool IsScreenTransitionValidForFade(ScreenID screenID)
    //{
    //    if (m_nonBlackScreenTransitions.Contains(screenID))
    //        return false;
    //    return true;
    //}

    //private void ScreenManager_ScreenChanging(ScreenID zNewScreenID)
    //{
    //    if (!IsScreenTransitionValidForFade(zNewScreenID))
    //        return;
    //    if (LoadingScreenManager.ScreenFadeQude != null &&
    //        LoadingScreenManager.ScreenFadeQude.FadeState != FadeQuadLoad.FadeState.fadeToBlack)
    //    {
    //        //obj.Wait = true;
    //        //Debug.Log("wait to true");
    //        LoadingScreenManager.ScreenFadeQude.FadeTo(new Color(0, 0, 0, 1), .3F, () =>
    //        {
    //            //Debug.Log("wait to false");
    //            CarInfoUI.Instance.ShowCarStats(false);
    //            //obj.Wait = false;
    //        });
    //    }
    //}

    //private void ScreenManager_ScreenChanged(ScreenID zNewScreenID)
    //{

    //}



    //void ScreenEvents_ScreenLoadingFinished(string arg1, HUDScreen arg2)
    //{
    //}

    //TODO
    //void StackManager_ScreenVisibilityChanging(HUDScreen.HudScreenEventArgs obj)
    //{
    //    m_currentEvent = obj;
    //    if (!obj.Visible)
    //    {
    //        if (!IsScreenTransitionValidForFade(obj))
    //            return;
    //        if (LoadingScreenManager.ScreenFadeQude != null &&
    //            LoadingScreenManager.ScreenFadeQude.FadeState != FadeQuadLoad.FadeState.fadeToBlack)
    //        {
    //            obj.Wait = true;
    //            //Debug.Log("wait to true");
    //            LoadingScreenManager.ScreenFadeQude.FadeTo(new Color(0, 0, 0, 1), .3F, () =>
    //            {
    //                //Debug.Log("wait to false");
    //                CarInfoUI.Instance.ShowCarStats(false);
    //                obj.Wait = false;
    //            });
    //        }
    //    }
    //    else
    //    {
    //        var activeScreen = (ZHUDScreen)obj.TargetScreen;
    //        //Debug.Log("screen visibility changing to true : " + activeScreen.GetScreenID());
    //        if (activeScreen != null)
    //            StartCoroutine(WaitThenShow(activeScreen.ScreenBackground));
    //        //BackgroundManager.ShowBackground(activeScreen.ScreenBackground);
    //        //if (IsScreenTransitionValidForFade(obj))
    //        //{
    //        //    if (LoadingScreenManager.ScreenFadeQude != null &&
    //        //        LoadingScreenManager.ScreenFadeQude.FadeState != FadeQuadLoad.FadeState.fadeOut)
    //        //        LoadingScreenManager.ScreenFadeQude.FadeTo(new Color(0, 0, 0, 0), .3F);
    //        //}
    //    }
    //}

    //private IEnumerator WaitThenShow(BackgroundManager.BackgroundType backgroundType)
    //{
    //    int frame = 0;
    //    while (frame<10)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        frame++;
    //    }
    //    BackgroundManager.ShowBackground(backgroundType);
    //}

    //TODO
    //void StackManager_ScreenVisibilityChanged(HUDScreen.HudScreenEventArgs obj)
    //{
    //    if (obj.Visible && IsScreenTransitionValidForFade(obj))
    //    {
    //        if (LoadingScreenManager.ScreenFadeQude != null &&
    //            LoadingScreenManager.ScreenFadeQude.FadeState != FadeQuadLoad.FadeState.fadeOut)
    //            LoadingScreenManager.ScreenFadeQude.FadeTo(new Color(0, 0, 0, 0), .3F);
    //    }
    //    if (!obj.Visible)
    //        return;
    //    bool zShow = false;
    //    bool showLogo = false;
    //    bool showReward = false;
    //    var zScreen = obj.ToScreen as ZHUDScreen;
    //    if (zScreen != null)
    //    {
    //        zShow = zScreen.ShowCarStats;
    //        showLogo = zScreen.ShowLogo;
    //        showReward = zScreen.ShowReward;
    //    }
    //    if (CarInfoUI.Instance != null)
    //    {
    //        CarInfoUI.Instance.ShowCarStats(zShow);
    //        //CarInfoUI.Instance.ShowLogo(showLogo);
    //        //CarInfoUI.Instance.ShowReward(showReward);
    //        //CarInfoUI.Instance.SetUIStat(showReward ? CarInfoUI.CarinfoUIStat.ShowRoom : CarInfoUI.CarinfoUIStat.Garage);
    //    }
    //    TutorialBubblesManager.Instance.OnScreenChanged();
    //    if (zScreen != null)
    //    {
    //        zScreen.TriggerScreenActivateEventDelayed();
    //    }
    //}
}
