using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpScreen : ZHUDScreen
{
    public TextMeshProUGUI OldLevelNumberText;

    public TextMeshProUGUI NewLevelNumberText;

    //public TextMeshProUGUI BaseRewardText;

    //public TextMeshProUGUI Title;

    //public TextMeshProUGUI SubTitle;

    public TextMeshProUGUI GoldNumber;

    public Button CollectButton;

    public RuntimeTextButton TwitterButton;
    public RuntimeButton ShareButton;

    public static int BeforeLevel;

    public static int AfterLevel;

    public AnimationCurve CurveControlShake;

    private LevelUpState LevelUpCurrentState;

    private float PauseTime;

    private float WaitHide = 0.2f;

    private bool overrideScreenLogic;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.LevelUp;
        }
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        CollectButton.SetState(BaseRuntimeControl.State.Active);
        CommonUI.Instance.DisableAllButtonOperability();
        //if (SocialController.TwitterIsDisabled && SocialController.FacebookIsDisabled)
        //{
        //    this.TwitterButton.Runtime.CurrentState = BaseRuntimeControl.State.Disabled;
        //}
        this.SetShareButtonString(false);
        MenuAudio.Instance.fadeMusic(0.4f, 0.5f);

        //this.Title.text = LocalizationManager.GetTranslation("TEXT_LEVEL_UP_CONGRATULATIONS_TITLE");
        //this.SubTitle.text = LocalizationManager.GetTranslation("TEXT_LEVEL_UP_CONGRATULATIONS_SUB_TITLE_1");
        //this.ColoredButtonText.text = LocalizationManager.GetTranslation("TEXT_LEVEL_UP_COLLECT_BUTTON");
        //this.GrayButtonText.text = LocalizationManager.GetTranslation("TEXT_LEVEL_UP_COLLECT_BUTTON");
        //this.LevelText.text = LocalizationManager.GetTranslation("TEXT_UI_XP_LEVEL");
        //this.TwitterButton.GetTextSprite().transform.localPosition += new Vector3(0.11f, 0f, 0f);
        //SocialController expr_F8 = SocialController.Instance;
        //expr_F8.OnGivenSocialReward = (Action)Delegate.Combine(expr_F8.OnGivenSocialReward, new Action(this.SetShareButtonStringAction));
        if (PopUpManager.Instance.isShowingPopUp)
        {
            PopUpManager.Instance.KillPopUp();
        }
        
        ShareButton.gameObject.SetActive(BuildType.CanShowShareButton());
    }

    public override void OnAfterActivate()
    {
        base.OnAfterActivate();
        ScreenManager.Instance.Interactable = true;
        //Animator.Play(OpenAnimationName);
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        CommonUI.Instance.EnableAllButtonOperability();
        MenuAudio.Instance.fadeMusic(1f, 0.5f);
        //SocialController expr_2E = SocialController.Instance;
        //expr_2E.OnGivenSocialReward = (Action)Delegate.Remove(expr_2E.OnGivenSocialReward, new Action(this.SetShareButtonStringAction));
    }

    public void SetShareButtonStringAction()
    {
        this.SetShareButtonString(false);
    }

    public void SetShareButtonString(bool forceGenericString = false)
    {
        //string text;
        //if (SocialController.Instance.SocialRewardAllowed() && !forceGenericString)
        //{
        //    string colouredCashString = CurrencyUtils.GetColouredCashString(GameDatabase.Instance.Social.GetCashRewardForTwitter());
        //    text = string.Format(LocalizationManager.GetTranslation("TEXT_SHARE_TO_GET"), colouredCashString);
        //}
        //else
        //{
        //    text = LocalizationManager.GetTranslation("TEXT_MENU_ICON_SHARE");
        //}
        //this.TwitterButton.SetText(text, true, true);
    }

    protected override void Awake()
    {
        this.OnInitialState();
        this.OnProgressBarState();
    }

    protected override void Update()
    {
        base.Update();
        switch (this.LevelUpCurrentState)
        {
            case LevelUpState.ProgressBar:
                {
                    this.UpdateProgressBarColor();
                    if (this.IsProgressBarFilled())
                    {
                        this.MoveToState(LevelUpState.HideNumber);
                    }
                    break;
                }
            case LevelUpState.HideNumber:
                if (this.IsHideFinished())
                {
                    this.MoveToState(LevelUpState.Pause);
                }
                break;
            case LevelUpState.Pause:
                if (this.IsPauseFinished())
                {
                    this.MoveToState(LevelUpState.Explosion);
                }
                break;
        }
    }

    private void MoveToState(LevelUpState nextState)
    {
        if (!this.overrideScreenLogic)
        {
            this.LevelUpCurrentState = nextState;
            this.StateAction();
        }
    }

    private void StateAction()
    {
        switch (this.LevelUpCurrentState)
        {
            case LevelUpState.Intitial:
                this.OnInitialState();
                break;
            case LevelUpState.ProgressBar:
                this.OnProgressBarState();
                break;
            case LevelUpState.HideNumber:
                this.OnHideNumberState();
                break;
            case LevelUpState.Pause:
                this.OnPauseState();
                break;
            case LevelUpState.Explosion:
                this.DoCameraShake();
                this.OnExplosionState();
                break;
        }
    }

    private void ButtonPressed()
    {
        if (this.LevelUpCurrentState == LevelUpState.Finished)
        {
            this.LevelUpCurrentState = LevelUpState.Intitial;
        }
    }

    public void CollectRewardAction()
    {
        int reward = GameDatabase.Instance.XPEvents.CalculateGoldReward();
        string originalReward = CurrencyUtils.GetGoldStringWithIcon(reward);

        if (GTAdManager.Instance.CanShowAdForExtraReward()) {
            VideoForRewardsManager.Instance.SetExtraRewardResult(new VideoForRewardsManager.ExtraRewardResult()
            {
                ActionOnVideoFail = ()=>GiveRewardToPlayer(false),
                ActionOnVideoOfferReject = ()=>GiveRewardToPlayer(false),
                ActionOnVideoSuccess = ()=>GiveRewardToPlayer(true),
                VideoFailRewardText = originalReward,
                VideoSuccessRewardText = CurrencyUtils.GetCashString(GameDatabase.Instance.XPEvents.GetLevelUpExtraCashReward())
            });
            VideoForRewardsManager.Instance.StartFlow(VideoForRewardConfiguration.eRewardID.VideoForExtraCashPrize);
        } else {
            GiveRewardToPlayer(false);
        }
    }

    private void GiveRewardToPlayer(bool extraReward = false)
    {
        //CommonUI.Instance.XPStats.StopGlowPulse();
        int num;
        GameDatabase.Instance.XPEvents.LevelUpPlayerToXP(out num);
        if(extraReward)
            GameDatabase.Instance.XPEvents.GiveLevelUpExtraCashReward();
        XPStats.DetectedLevelUp = false;
        LogLevelupToMetrics(num);
        CollectButton.SetState(BaseRuntimeControl.State.Disabled);
        Invoke(nameof(Close), 1f);
        //Close();
    }

    private void LogLevelupToMetrics(int num)
    {
        int lvl = PlayerProfileManager.Instance.ActiveProfile.GetPlayerLevel();
        Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
        {
            {
                Parameters.Level,
                lvl.ToString()
            },
            {
                Parameters.DGld,
                num.ToString()
            }
        };
        Log.AnEvent(Events.LevelUp, data);
        switch (lvl)
        {
            case 2:
                Log.AnEvent(Events.LevelReached_02);
                break;
            case 3:
                Log.AnEvent(Events.LevelReached_03);
                break;
            case 4:
                Log.AnEvent(Events.LevelReached_04);
                break;
            case 5:
                Log.AnEvent(Events.LevelReached_05);
                break;
            case 6:
                Log.AnEvent(Events.LevelReached_06);
                break;
            case 7:
                Log.AnEvent(Events.LevelReached_07);
                break;
            case 8:
                Log.AnEvent(Events.LevelReached_08);
                break;
            case 9:
                Log.AnEvent(Events.LevelReached_09);
                break;
            case 10:
                Log.AnEvent(Events.LevelReached_10);
                break;
        }
    }

    private void OnInitialState()
    {
        //this.LevelUpAnimation.Rewind("LevelUp");
        this.UpdateLevelNumbersText();
        this.UpdateBasRewardText();
        this.OldLevelNumberText.gameObject.SetActive(true);
        int num = GameDatabase.Instance.XPEvents.CalculateGoldReward();
        this.GoldNumber.text = CurrencyUtils.GetGoldStringWithIcon(num);
        AudioManager.Instance.PlaySound("LevelUp", null);
        this.MoveToState(LevelUpState.ProgressBar);
    }

    private void UpdateBasRewardText()
    {
        //var playerLevel = PlayerProfileManager.Instance.ActiveProfile.GetPlayerLevel();
        //var hasrewardMultiplier = GameDatabase.Instance.CurrenciesConfiguration.RewardsMultipliers
        //    .LeagueBonus
        //    .HasLevelBonusRewardMultiplier(playerLevel);

        //if (hasrewardMultiplier)
        //{
        //    //var rewardMultiplier = GameDatabase.Instance.CurrenciesConfiguration.RewardsMultipliers
        //    //    .LeagueBonus
        //    //    .GetLevelBonusRewardMultiplier(playerLevel);
        //    //BaseRewardText.gameObject.SetActive(true);
        //    //var str = LocalizationManager.GetTranslation("");
        //    //BaseRewardText.text = String.Format(str, rewardMultiplier.CashPrizeMultiplier*100);
        //}
        //else
        //{
            //BaseRewardText.gameObject.SetActive(false);
        //}
    }

    private void OnProgressBarState()
    {
        //this.LevelUpAnimation.Play("LevelUp", PlayMode.StopSameLayer);
    }

    private void OnHideNumberState()
    {
        this.PauseTime = Time.time;
    }

    private void OnPauseState()
    {
        this.PauseTime = Time.time;
    }

    private void OnExplosionState()
    {
        //this.ChangingNumber.gameObject.SetActive(false);
        //this.SubTitle.text = LocalizationManager.GetTranslation("TEXT_LEVEL_UP_CONGRATULATIONS_SUB_TITLE_2");
        //this.Coin.gameObject.SetActive(false);
        //this.Coin.gameObject.SetActive(true);
        //this.ColoredText();
        //CollectButton.interactable = true;
        this.MoveToState(LevelUpState.Finished);
    }

    private void DoCameraShake()
    {
        //this.cameraShake = GUICamera.Instance.gameObject.AddComponent<GUICameraShake>();
        //this.cameraShake.SetCurve(this.CurveControlShake);
        //this.cameraShake.ShakeTime = Time.time;
    }

    private void UpdateProgressBarColor()
    {
        //Color color = this.ProgressBar.color;
        //color.a = this.BarPosition;
        //this.ProgressBar.SetColor(color);
    }

    private void UpdateLevelNumbersText()
    {
        this.OldLevelNumberText.text = BeforeLevel.ToNativeNumber();
        this.NewLevelNumberText.text = AfterLevel.ToNativeNumber();
    }

    private bool IsProgressBarFilled()
    {
        return true;
    }

    private bool IsHideFinished()
    {
        float time = Time.time;
        float num = time - this.PauseTime;
        return num >= this.WaitHide;
    }

    private bool IsPauseFinished()
    {
        float time = Time.time;
        //return time - this.PauseTime >= this.WaitBeforeExplosion;
        return true;
    }

    public void OnShareButton()
    {
        this.FinishAnimation();
        SocialController.Instance.OnShareButton(SocialController.MessageType.LEVEL_UP, LevelUpScreen.AfterLevel.ToString(), false, false);
    }

    public void PrepareForScreenshot()
    {
        this.overrideScreenLogic = true;
        this.OnInitialState();
        this.OnHideNumberState();
        //this.Title.text = LocalizationManager.GetTranslation("TEXT_LEVEL_UP_CONGRATULATIONS_TITLE");
        //this.SubTitle.text = LocalizationManager.GetTranslation("TEXT_LEVEL_UP_CONGRATULATIONS_SUB_TITLE_1");
        //this.ColoredButtonText.text = LocalizationManager.GetTranslation("TEXT_LEVEL_UP_COLLECT_BUTTON");
        //this.GrayButtonText.text = LocalizationManager.GetTranslation("TEXT_LEVEL_UP_COLLECT_BUTTON");
        //this.LevelText.text = LocalizationManager.GetTranslation("TEXT_UI_XP_LEVEL");
    }

    public void FinishAnimation()
    {
        this.overrideScreenLogic = false;
        this.OnInitialState();
        this.OnProgressBarState();
        this.OnHideNumberState();
        this.OnPauseState();
        this.OnExplosionState();
    }

    public override void RequestBackup()
    {
    }
}
