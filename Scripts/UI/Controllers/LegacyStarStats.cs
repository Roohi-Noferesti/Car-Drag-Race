using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;

public class LegacyStarStats : MonoBehaviour, IPersistentUI
{
    public TextMeshProUGUI uiTxtPlayerStar;

    private bool canUpdateStar;

    private bool onFirstUpdate = true;

    public static bool DetectedLeagueChange;

    private int OldStarValue = -1;

    private int LastKnownStar = -1;

    private int CurrentStar = -1;

    private int CurrentDisplayStar;

    private int LastDisplayedStar = -1;

    private LeagueData.LeagueName CurrentDisplayLeague = LeagueData.LeagueName.None;

    private bool runStar;

    private float CurrentTime;

    private float TargetTime;

    private bool canLeagueChange;

    public void Awake()
    {
    }

    public void Start()
    {
    }

    public void StarLockedState(bool zLocked)
    {
        this.canUpdateStar = !zLocked;
    }

    public void NewLeagueLockedState(bool zLocked)
    {
        this.canLeagueChange = !zLocked;
    }

    public bool IsLeagueUpLocked()
    {
        return !this.canLeagueChange;
    }

    public void OnScreenChanged(ScreenID screen)
    {
    }

    public void Show(bool zShow)
    {
        base.gameObject.SetActive(zShow && PolledNetworkState.IsNetworkConnected);
    }

    public bool IsVisible()
    {
        return base.gameObject.activeInHierarchy;
    }

    private void Update()
    {
        if (GameDatabase.Instance.StarConfiguration == null)
        {
            return;
        }
        if (this.onFirstUpdate)
        {
            this.CurrentDisplayLeague = GameDatabase.Instance.StarDatabase.GetPlayerLeague();
            this.CurrentDisplayStar = PlayerProfileManager.Instance.ActiveProfile.PlayerStar;
            this.LastKnownStar = this.CurrentDisplayStar;
            this.onFirstUpdate = false;
            this.SetStarText(this.CurrentDisplayStar);
            this.SetToCurrentKnown();
        }
        this.CurrentStar = PlayerProfileManager.Instance.ActiveProfile.PlayerStar;
        if (PlayerProfileManager.Instance.ActiveProfile.PlayerStar <= 0)
            CurrentDisplayStar = 0;
        this.HandleStarAnim();
        this.DisplayValues();
        if (this.CurrentDisplayLeague != GameDatabase.Instance.StarDatabase.GetPlayerLeague())
        {
            this.DoLeagueChangeCheck();
        }
    }
    
    private void HandleStarAnim()
    {
        if (!this.canUpdateStar)
        {
            return;
        }
        if (this.runStar)
        {
            this.CurrentTime += Time.deltaTime;
            if (this.CurrentTime >= this.TargetTime)
            {
                this.CurrentDisplayStar = this.CurrentStar;
                this.runStar = false;
            }
            else
            {
                float num = this.CurrentTime / this.TargetTime;
                this.CurrentDisplayStar = (int)((float)this.LastKnownStar * num + (float)this.OldStarValue * (1f - num));
            }
        }
        if (this.CurrentStar != this.LastKnownStar)
        {
            this.runStar = true;
            this.CurrentTime = 0f;
            this.TargetTime = 0.5f;
            if (this.CurrentStar > this.LastKnownStar && this.LastKnownStar > 0)
            {
                AudioManager.Instance.PlaySound("StarGain", null);
            }
            this.OldStarValue = this.CurrentDisplayStar;
            this.LastKnownStar = this.CurrentStar;
        }
    }

    private void Save()
    {
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
    }
    public bool DoLeagueChangeCheck()
    {
        if (!this.canLeagueChange)
        {
            return false;
        }
        if (DetectedLeagueChange)
        {
            return false;
        }
        if (ScreenManager.Instance.ActiveScreen is LeagueChangeScreen)
        {
            return false;
        }
        if (ScreenManager.Instance.ActiveScreen is LevelUpScreen)
        {
            return false;
        }
        //if (ScreenManager.Active.CurrentScreen == ScreenID.FriendReward)
        //{
        //    return false;
        //}
        if (PopUpManager.Instance.isShowingPopUp || PopUpManager.Instance.WaitingToShowPopup)
        {
            return false;
        }
        if (LoadingScreenManager.Instance.IsShowingLoadingPanel)
        {
            return false;
        }
        var playerLeagueIndex = (int)GameDatabase.Instance.StarDatabase.GetPlayerLeague();
        int leagueIndexForPlayerStar = (int) GameDatabase.Instance.StarDatabase.GetLeagueForPlayerStar();
        if (leagueIndexForPlayerStar != playerLeagueIndex)
        {
            //If player league down , Notify him when his star is 100 point below next league roof
            if (leagueIndexForPlayerStar < playerLeagueIndex)
            {
                var leagueDownStar = GameDatabase.Instance.StarDatabase.StarTotalAtEndOfLeague(leagueIndexForPlayerStar);
                if (leagueDownStar - CurrentStar < 100)
                {
                    return false;
                }
            }
            LeagueChangeScreen.BeforeLeague = playerLeagueIndex;
            LeagueChangeScreen.AfterLeague = leagueIndexForPlayerStar;
            //ScreenManager.Instance.PushScreen(ScreenID.LeagueChange);
            DetectedLeagueChange = true;
            return true;
        }
        return false;
    }

    private void DisplayValues()
    {
        if (!this.canUpdateStar)
        {
            return;
        }
        if (this.CurrentDisplayStar != this.LastDisplayedStar)
        {
            var currentLeageIndex = (int) CurrentDisplayLeague;
            var totalStarAtEndOfLevel = GameDatabase.Instance.StarDatabase.StarTotalAtEndOfLeague(currentLeageIndex);

            if (CurrentDisplayStar >= totalStarAtEndOfLevel)
            {
                this.CurrentDisplayLeague++;
                //this.SetLeagueText(this.CurrentDisplayLeague);
            }
            else if (currentLeageIndex > 0)
            {
                var totalStarAtEndOfDownLevel =
                    GameDatabase.Instance.StarDatabase.StarTotalAtEndOfLeague(currentLeageIndex - 1);
                if (CurrentDisplayStar < totalStarAtEndOfDownLevel - 100)
                    this.CurrentDisplayLeague--;
            }
            this.SetStarText(CurrentDisplayStar);
            this.LastDisplayedStar = this.CurrentDisplayStar;
        }
    }

    private void SetStarText(int level)
    {
        this.uiTxtPlayerStar.text = level.ToString();//LocalizationManager.FixRTL_IfNeeded();
    }

    public void SetToCurrentKnown()
    {
        this.OldStarValue = (this.LastKnownStar = (this.CurrentStar = (this.CurrentDisplayStar = (this.LastDisplayedStar = PlayerProfileManager.Instance.ActiveProfile.GetPlayerStar()))));
        this.CurrentDisplayStar = PlayerProfileManager.Instance.ActiveProfile.PlayerStar;
        this.SetStarText(this.CurrentDisplayStar);
    }
}
