using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;

public class StarLeagueStats : MonoBehaviour,IPersistentUI 
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
        base.gameObject.SetActive(zShow);
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
            this.CurrentDisplayStar = PlayerProfileManager.Instance.ActiveProfile.PlayerLeagueStar;
            this.LastKnownStar = this.CurrentDisplayStar;
            this.onFirstUpdate = false;
            this.SetStarText(this.CurrentDisplayStar);
            this.SetToCurrentKnown();
        }
        this.CurrentStar = PlayerProfileManager.Instance.ActiveProfile.PlayerLeagueStar;
        if (PlayerProfileManager.Instance.ActiveProfile.PlayerLeagueStar <= 0)
            CurrentDisplayStar = 0;
        this.HandleStarAnim();
        this.DisplayValues();
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

    private void DisplayValues()
    {
        if (!this.canUpdateStar)
        {
            return;
        }
        if (this.CurrentDisplayStar != this.LastDisplayedStar)
        {
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
        this.OldStarValue = (this.LastKnownStar = (this.CurrentStar = (this.CurrentDisplayStar = (this.LastDisplayedStar = PlayerProfileManager.Instance.ActiveProfile.GetPlayerLeagueStar()))));
        this.CurrentDisplayStar = PlayerProfileManager.Instance.ActiveProfile.PlayerLeagueStar;
        this.SetStarText(this.CurrentDisplayStar);
    }
}
