using System;
using System.Collections.Generic;
using Metrics;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class PauseScreen : ZHUDScreen 
{
    
    public static PauseScreen Instance;
    public static bool ScreenFinished;

    
    public int MPGoldToRestart = 3;
    private static GameObject PauseButton;
    public RuntimeTextButton ResumeButton;
    public RuntimeTextButton RestartButton;
    public RuntimeTextButton QuitButton;
    public Sprite PetrolPump;
    public Sprite BackgroundObject;
    
    public override ScreenID ID
    {
        get { return ScreenID.Pause; }
    }

    protected override void Awake()
    {
        if (Instance != null)
        {
        }

        Instance = this;

        ScreenFinished = false;
        base.Awake();
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {


    }

    public override void Close()
    {
        base.Close();
        PauseGame.isGamePaused = false;
    }
    
    
  private void HideButton(RuntimeTextButton button)
  {
    button.CurrentState = BaseRuntimeControl.State.Hidden;
    button.gameObject.SetActive(false);
  }

  public override void OnActivate(bool zAlreadyOnStack)
  {
    base.OnActivate(zAlreadyOnStack);
    if (PauseButton == null)
      PauseButton = GameObject.Find("Pause");
    PauseButton.SetActive(false);
    // this.ResumeButton.ForceAwake();
    // this.ResumeButton.Runtime.EnableFeatureCreepFridayHack();
    // this.RestartButton.ForceAwake();
    // this.RestartButton.Runtime.EnableFeatureCreepFridayHack();
    // this.QuitButton.ForceAwake();
    // this.QuitButton.Runtime.EnableFeatureCreepFridayHack();
    LoadingScreenManager.Instance.ForceEndLoadingEffects();
    RestartButton.gameObject.SetActive(true);
    QuitButton.gameObject.SetActive(true);
    if (IngameTutorial.IsInTutorial || RaceEventInfo.Instance.IsNonRestartable() || (PlayerProfileManager.Instance.ActiveProfile.RacesEntered == 1 || RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent()))
    {
      HideButton(RestartButton);
      HideButton(QuitButton);
    }
    else if (RaceEventInfo.Instance.IsRelayEvent())
      RestartButton.CurrentState = BaseRuntimeControl.State.Disabled;
    else if (RaceEventInfo.Instance.CurrentEvent.AutoHeadstart && RaceEventInfo.Instance.CurrentEvent.AutoHeadstartTimeDifference() < 0.0 && !RaceHUDController.Instance.hudRaceCentreMessage.HeadstartRaceCountdown.Shown)
      RestartButton.CurrentState = BaseRuntimeControl.State.Disabled;
    else if (RaceEventInfo.Instance.IsHighStakesEvent || RaceEventInfo.Instance.IsDailyBattleEvent)
    {
      RestartButton.CurrentState = BaseRuntimeControl.State.Disabled;
      QuitButton.CurrentState = BaseRuntimeControl.State.Disabled;
    }
    else if (!RaceController.Instance.Machine.StateIs(RaceStateEnum.race))
    {
      RestartButton.CurrentState = BaseRuntimeControl.State.Disabled;
    }
    else
    {
      RestartButton.CurrentState = BaseRuntimeControl.State.Active;
      QuitButton.CurrentState = BaseRuntimeControl.State.Active;
    }
    CentrePumpWithRestartText();
  }

  private void CentrePumpWithRestartText()
  {
    //SpriteText textSprite = this.RestartButton.GetTextSprite();
    //if ( textSprite ==  null)
    //  return;
    //Vector3 localPosition = textSprite.transform.localPosition;
    //float num = (float) ((double) this.PetrolPump.width + (double) textSprite.TotalWidth + 0.0700000002980232);
    //this.PetrolPump.transform.localPosition = new Vector3((float) (-(double) num / 2.0 - 0.00999999977648258), 0.0f, -0.1f);
    //localPosition.x = (float) ((double) num / 2.0 - (double) textSprite.TotalWidth / 2.0);
    //textSprite.transform.localPosition = localPosition;
  }

  public override void OnDeactivate()
  {
    if ( PauseButton !=  null)
      PauseButton.SetActive(true);
    base.OnDeactivate();
  }

  public void OnResumeButton()
  {
    PauseButton.SetActive(true);
    PauseGame.UnPause();
  }

  protected override void Update()
  {
    if (!firstUpdate && (bool) KeyMappings.Instance && KeyMappings.Instance.GetKeyDown(KeyMappings.Action.PAUSE))
      PauseGame.UnPause();
    base.Update();
  }

  public override void OnHardwareBackButton()
  {
    if (firstUpdate)
      return;
    MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
    PauseGame.UnPause();
  }

  protected override void OnEnterPressed()
  {
    if (firstUpdate)
      return;
    MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
    PauseGame.UnPause();
  }

  public void OnRestartButton()
  {
    if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent())
      return;
    if (!FuelManager.Instance.SpendFuel(GameDatabase.Instance.Currencies.GetFuelCostForEvent(RaceEventInfo.Instance.CurrentEvent)))
    {
      if (PlayerProfileManager.Instance.ActiveProfile.GetCurrentGold() >= GameDatabase.Instance.Currencies.GetGoldToFillFuelTank())
        PopUpDatabase.Common.ShowBuyFuelAndRestartPopUp(BuyFuelAndRestart);
      else
        PopUpDatabase.Common.ShowGoGetFuelPopUp(GoGetFuel);
    }
    else
    {
      PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
      PauseButton.SetActive(true);
      RaceController.Instance.ResetRace();
    }
  }

  public void GoGetFuel()
  {
    VideoCapture.StopRecording();
    PauseGame.UnPause();
    SceneManagerFrontend.SetScreenToPushWhenInFrontend(ScreenID.GetMoreFuel);
    RaceController.Instance.BackToFrontend();
  }

  public void BuyFuelAndRestart()
  {
    int goldToFillFuelTank = GameDatabase.Instance.Currencies.GetGoldToFillFuelTank();
    PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
    activeProfile.SpendGold(goldToFillFuelTank,"buy","fuel");
    ++activeProfile.FuelRefillsBoughtWithGold;
    int num = FuelManager.Instance.FillTank(FuelAnimationLockAction.OBEY);
    Log.AnEvent(Events.PurchaseItem, new Dictionary<Parameters, string>
    {
      {
        Parameters.DGld,
        (-goldToFillFuelTank).ToString()
      },
      {
        Parameters.Dfuel,
        num.ToString()
      },
      {
        Parameters.ItmClss,
        "fuel"
      },
      {
        Parameters.Itm,
        "restart"
      }
    });
    PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
    if (!FuelManager.Instance.SpendFuel(GameDatabase.Instance.Currencies.GetFuelCostForEvent(RaceEventInfo.Instance.CurrentEvent)))
      return;
    PauseButton.SetActive(true);
    RaceController.Instance.ResetRace();
  }

  private void QuitRace()
  {
    VideoCapture.StopRecording();
    PauseGame.UnPause();
    if (RaceResultsTracker.You != null)
      RaceResultsTracker.You.IsWinner = false;
    RaceController.Instance.BackToFrontend();
  }

  public void OnQuitButton()
  {
    if (RaceEventInfo.Instance.IsRelayEvent())
      RelayManager.ShowConfirmQuitPopup(() =>
      {
        RelayManager.ResetRelayData();
        QuitRace();
      }, true);
    else
      QuitRace();
  }



  public override bool HasBackButton()
  {
    return false;
  }
}
