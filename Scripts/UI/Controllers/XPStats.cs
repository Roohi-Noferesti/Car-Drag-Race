using System;
using I2.Loc;
using ir.metrix.unity;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPStats : MonoBehaviour, IPersistentUI
{
	public TextMeshProUGUI uiTxtPlayerLevel;

	public Slider ProgressBar;

	private bool canUpdateXP;

	private bool canLevelUp;

	private bool onFirstUpdate = true;

	public static bool DetectedLevelUp;

	private int OldXPValue = -1;

	private int LastKnownXP = -1;

	private int CurrentXP = -1;

	private int CurrentDisplayXP;

	private int LastDisplayedXP = -1;

	private bool runXP;

	private float CurrentTime;

	private float TargetTime;

	private int CurrentDisplayLevel = -1;

    public static event Action<int, int> LevelUp; 

	public void Awake()
	{
	}

	public void Start()
	{
	}

	public void XPLockedState(bool zLocked)
	{
		this.canUpdateXP = !zLocked;
	}

	public void LevelUpLockedState(bool zLocked)
	{
		this.canLevelUp = !zLocked;
	}

	public bool IsLevelUpLocked()
	{
		return !this.canLevelUp;
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
		if (GameDatabase.Instance.XPEventsConfiguration == null)
		{
			return;
		}
		if (this.onFirstUpdate)
		{
			this.CurrentDisplayLevel = GameDatabase.Instance.XPEvents.GetPlayerLevel();
			this.LastKnownXP = (this.CurrentDisplayXP = GameDatabase.Instance.XPEvents.XPTotalAtEndOfLevel(this.CurrentDisplayLevel - 1));
			this.onFirstUpdate = false;
			this.SetLevelText(this.CurrentDisplayLevel);
			this.SetToCurrentKnown();
		}
		this.CurrentXP = GameDatabase.Instance.XPEvents.GetPlayerXP();
		this.HandleXPAnim();
		this.DisplayValues();
		if (this.CurrentDisplayLevel != GameDatabase.Instance.XPEvents.GetPlayerLevel())
		{
			this.DoLevelUpCheck();
		}
	}

	private void HandleXPAnim()
	{
		if (!this.canUpdateXP)
		{
			return;
		}
		if (this.runXP)
		{
			this.CurrentTime += Time.deltaTime;
			if (this.CurrentTime >= this.TargetTime)
			{
				this.CurrentDisplayXP = this.CurrentXP;
				this.runXP = false;
			}
			else
			{
				float num = this.CurrentTime / this.TargetTime;
				this.CurrentDisplayXP = (int)((float)this.LastKnownXP * num + (float)this.OldXPValue * (1f - num));
			}
		}
		if (this.CurrentXP != this.LastKnownXP)
		{
			this.runXP = true;
			this.CurrentTime = 0f;
			this.TargetTime = 0.5f;
			if (this.CurrentXP > this.LastKnownXP && this.LastKnownXP > 0)
			{
				AudioManager.Instance.PlaySound("Reward/XPGain", null);
			}
			this.OldXPValue = this.CurrentDisplayXP;
			this.LastKnownXP = this.CurrentXP;
		}
	}

	private void DisplayValues()
	{
		if (!this.canUpdateXP)
		{
			return;
		}
		if (this.CurrentDisplayXP != this.LastDisplayedXP)
		{
			if (this.CurrentDisplayXP >= GameDatabase.Instance.XPEvents.XPTotalAtEndOfLevel(this.CurrentDisplayLevel))
			{
				this.CurrentDisplayLevel++;
				this.SetLevelText(this.CurrentDisplayLevel);
			}
            //float num = GameDatabase.Instance.XPEvents.ProgressThroughLevelForXP(this.CurrentDisplayXP, this.CurrentDisplayLevel);
		    UpdateProgressbar(CurrentDisplayLevel, CurrentDisplayXP);
			this.LastDisplayedXP = this.CurrentDisplayXP;
		}
	}

	private void SetLevelText(int level)
    {
        var text = LocalizationManager.GetTranslation("TEXT_COMMON_UI_LEVEL");

		this.uiTxtPlayerLevel.text =
            string.Format(text, level.ToString());//LocalizationManager.FixRTL_IfNeeded());
	}

	public bool DoLevelUpCheck()
	{
		if (!this.canLevelUp)
		{
			return false;
		}
		if (DetectedLevelUp)
		{
			return false;
		}
        if (ScreenManager.Instance.CurrentScreen == ScreenID.LevelUp)
        {
            return false;
        }
        if (ScreenManager.Instance.CurrentScreen == ScreenID.LeagueChange)
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
		var playerLevel = GameDatabase.Instance.XPEvents.GetPlayerLevel();
		var levelForPlayerXP = GameDatabase.Instance.XPEvents.GetLevelForPlayerXP();
		if (levelForPlayerXP > playerLevel)
		{
			LevelUpScreen.BeforeLevel = playerLevel;
			LevelUpScreen.AfterLevel = levelForPlayerXP;
		    ScreenManager.Instance.PushScreen(ScreenID.LevelUp);
			DetectedLevelUp = true;
            OnLevelUp(LevelUpScreen.BeforeLevel, LevelUpScreen.AfterLevel);
			return true;
		}
		return false;
	}

	public void SetToCurrentKnown()
	{
		this.OldXPValue = (this.LastKnownXP = (this.CurrentXP = (this.CurrentDisplayXP = (this.LastDisplayedXP = PlayerProfileManager.Instance.ActiveProfile.GetPlayerXP()))));
		this.CurrentDisplayLevel = GameDatabase.Instance.XPEvents.GetPlayerLevel();
		this.SetLevelText(this.CurrentDisplayLevel);
        this.UpdateProgressbar(CurrentDisplayLevel, CurrentXP);
	}

    private void UpdateProgressbar(int level,int xp)
    {
        ProgressBar.minValue = GameDatabase.Instance.XPEvents.XPTotalAtEndOfLevel(level - 1);
        ProgressBar.maxValue = GameDatabase.Instance.XPEvents.XPTotalAtEndOfLevel(level);
        ProgressBar.value = xp;
    }

    public static bool DoForceUpdateAndCheck()
	{
		if (ScreenManager.Instance.CurrentScreen == ScreenID.Invalid)
		{
			return false;
		}
		if (ScreenManager.Instance.CurrentScreen == ScreenID.RaceRewards)
		{
			return false;
		}
		
		int playerLevel = GameDatabase.Instance.XPEvents.GetPlayerLevel();
		int levelForPlayerXP = GameDatabase.Instance.XPEvents.GetLevelForPlayerXP();
		if (levelForPlayerXP > playerLevel)
		{
			LevelUpScreen.BeforeLevel = playerLevel;
			LevelUpScreen.AfterLevel = levelForPlayerXP;
            ScreenManager.Instance.PushScreen(ScreenID.LevelUp);
			DetectedLevelUp = true;
		    OnLevelUp(LevelUpScreen.BeforeLevel, LevelUpScreen.AfterLevel);
			return true;
		}
		return false;
	}

    
    private static void OnLevelUp(int oldLevel, int newLevel)
    {
	    if (newLevel == 10)
	    {
		    Metrix.NewEvent("qnany");
	    }
        PlayerProfileManager.Instance.ActiveProfile.HasSeenUnlockCarScreen = false;
        var handler = LevelUp;
        if (handler != null) handler(oldLevel, newLevel);
    }
}
