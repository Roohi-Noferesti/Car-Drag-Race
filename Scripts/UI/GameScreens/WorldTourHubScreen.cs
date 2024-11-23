using System.Collections;
using System.Collections.Generic;
using DataSerialization;
using KingKodeStudio;
using UnityEngine;

public class WorldTourHubScreen : ZHUDScreen
{
    public override ScreenID ID
    {
        get { return ScreenID.WorldTourChoice; }
    }


    public override void OnActivate(bool zAlreadyOnStack)
    {
        base.OnActivate(zAlreadyOnStack);
        TierXPanelSetup(null);
    }

    public void TierXPanelSetup(TierXManager.OnTierXReady onTXready)
    {
        if (!TierXManager.Instance.TryLoadThemeTransition(delegate
        {
            this.LoadTierXJson(onTXready);
        }))
        {
            if (!TierXManager.Instance.IsJsonLoaded)
            {
                this.LoadTierXJson(onTXready);
            }
            else
            {
                this.TierXPanelSetupCompete();
            }
        }
        if (ScreenManager.Instance.CurrentScreen != this.ID)
        {
            return;
        }
        //this.targetTierPosition = CareerModeMapScreen.GetPanelPositionOffset(panel);
        //this.targetTierPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(this.targetTierPosition);
        //if (CareerModeMapScreen._lastPaneSelected == -2)
        //{
        //    //this.tierPosition = this.targetTierPosition;
        //}
        //if (!isWorldTourUnlocked)
        //{
        //    this.TierText.gameObject.SetActive(false);
        //    this.TopOffset.gameObject.SetActive(false);
        //    this.ObjectiveText.gameObject.SetActive(false);
        //}
        //this.SetupPeekingPanel(panel);
        base.StopCoroutine("RemoveOldPins");
        base.StartCoroutine("RemoveOldPins");
        //if (!isWorldTourUnlocked)
        //{
        //    this.eventPane.OnTierXLocked();
        //}
        CleanDownManager.Instance.OnTierXThemeChanged();
    }


    private void LoadTierXJson(TierXManager.OnTierXReady onTXready)
    {
        TierXManager.Instance.LoadTierXJson(delegate
        {
            this.TierXPanelSetupCompete();
            if (onTXready != null)
            {
                onTXready();
            }
        });
    }


    private void TierXPanelSetupCompete()
    {
        ThemeLayout themeDescriptor = TierXManager.Instance.ThemeDescriptor;
        if (themeDescriptor.ShowTitle)
        {
            //this.SetupTierText(themeDescriptor.Name, true);
        }
        else
        {
            //this.SetupTierText(string.Empty, true);
        }
        if (themeDescriptor.ShowEventPane)
        {
            //this.eventPane.ActivateAll();
        }
        else
        {
            //this.eventPane.DeactivateAll();
        }
        //this.TierText.gameObject.SetActive(themeDescriptor.ShowTierText);
        //this.TopOffset.gameObject.SetActive(themeDescriptor.ShowSelectedThemeDescription);
        //this.ObjectiveText.gameObject.SetActive(themeDescriptor.ShowObjective);
        if (themeDescriptor.ShowObjective)
        {
            //this.SetupObjectiveText();
        }
        if (TierXManager.Instance.IsOverviewThemeActive())
        {
            //UnityEngine.Vector3 b = new UnityEngine.Vector3(0f, 0f, 0f);
            //CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
            //EventPane eventPane = careerModeMapScreen.eventPane;
            //b.x = eventPane.PaneWidthTight / 2f;
            //this.TopOffset.transform.parent = this.EventSelect.transform;
            //this.TopOffset.transform.localPosition = new UnityEngine.Vector3(0f, 2.2f, 0f) - CareerModeMapScreen.GetPanelPositionOffset(CareerModeMapScreen.mapPaneSelected) - b;
        }
        if (themeDescriptor.CanSwipe)
        {
            //this.Pagination.AnimateIn();
        }
        else
        {
            //this.Pagination.SetInvisible();
        }
        if (themeDescriptor.GetThemeOptionLayoutDetails() != null)
        {
            if (MapScreenCache.InternationalEventHubGO == null)
            {
                MapScreenCache.InternationalEventHubGO = (UnityEngine.Object.Instantiate(Resources.Load("Career/MapInternationalEventHub")) as GameObject);
            }
            MapScreenCache.InternationalEventHubGO.GetComponent<EventHubBackgroundManager>().Show(themeDescriptor.GetThemeOptionLayoutDetails());
        }
        else
        {
            MapScreenCache.DestroyInternationalBackground();
        }
        //if (this.ShowWorldTourHighStakesIfRequired())
        //{
        //    return;
        //}
        //if (this.ShowDeferredNarrativeSceneIfRequired())
        //{
        //    return;
        //}
        TierXManager.Instance.RefreshThemeMap();
        if (RelayManager.GoToNextRelayRaceIfRequired())
        {
            return;
        }
        TierXManager.Instance.SetupBackground();
    }
}
