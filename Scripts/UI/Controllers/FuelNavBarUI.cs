using System;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FuelNavBarUI : MonoBehaviour, IPersistentUI
{
    private enum Mode
    {
        BluePip,
        RedPip,
        BlueSolid
    }

    public const float SpendAnimationTime = 1.8f;

    public Color PipColourHigh;

    public Color PipColourLow;

    //public Animation FuelAnimGlow;

    public Animation DeltaTextAnimation;

    public TextMeshProUGUI DeltaText;

    public TextMeshProUGUI FuelText;

    public Image FuelIcon;

    //public GameObject FuelSpendAnim;

    //public GameObject FuelSpendAnimLow;

    //public GameObject AnimOffset;

    //public Transform Offset;

    //public Animation UpgradeTankAnimation;

    //public GameObject UpgradedTankEffects;

    //public TextMeshProUGUI TotalGasText;

    //private NavBarInfoPane NoFuelInfo;

    private int LastKnownFuel;

    private bool Animating;

    private float LastShown;

    private float AnimateStart;

    private int AnimateTo;

    private float TimerPos;

    private bool TriggerFireNoFuelInfo;

    private float TriggerFireNoFuelInfoTimer;

    private bool WaitingForSpendGlowContinue;

    //private List<FuelPipAnim> WaitingPipAnimations = new List<FuelPipAnim>();

    private bool WeAreShowing;

    public bool ButtonEnabled = true;

    private bool hasBeenPlaying;

    private bool HasTapShrunk;

    private float TapAnimTimer;
    private bool canUpdateFuel = true;

    public Animator FlashAnimator;

    private void Start()
    {
    }

    private void Awake()
    {
        FuelManager.Instance.OnFuelPurchase += UpdateFuelAnimate;
        FuelManager.Instance.OnFuelAutoReplenish += UpdateReplenished;
        FuelManager.Instance.OnFuelTimerUpdated += UpdateFuelTimerUpdated;
        FuelManager.Instance.OnFuelSpend += UpdateFuelInstant;
        FuelManager.Instance.OnFuelTankUpgraded += UpgradeFuelTank;
        FuelManager.Instance.OnFuelTankUpgradeRevoked += RemoveFuelTankUpgrade;
        FuelManager.Instance.OnFuelUnlimitedRevoked += RemoveUnlimitedFuel;
        //NavBarAnimationManager.Instance.Subscribe(this.Offset.gameObject);
        this.LastKnownFuel = FuelManager.Instance.GetFuel();
        //if (this.LastKnownFuel > 10)
        //{
            //this.TotalGasText.gameObject.SetActive(true);
        var fuelMax = FuelManager.Instance.CurrentMaxFuel();
        this.FuelText.text = FuelText.text = (fuelMax - LastKnownFuel).ToNativeNumber();

        //}
    }

    private void OnDestroy()
    {
        FuelManager.Instance.OnFuelPurchase -= UpdateFuelAnimate;
        FuelManager.Instance.OnFuelAutoReplenish -= UpdateReplenished;
        FuelManager.Instance.OnFuelTimerUpdated -= UpdateFuelTimerUpdated;
        FuelManager.Instance.OnFuelSpend -= UpdateFuelInstant;
        FuelManager.Instance.OnFuelTankUpgraded -= UpgradeFuelTank;
        FuelManager.Instance.OnFuelTankUpgradeRevoked -= RemoveFuelTankUpgrade;
        FuelManager.Instance.OnFuelUnlimitedRevoked -= RemoveUnlimitedFuel;
    }

    private int GetDeltaPips()
    {
        int fuel = FuelManager.Instance.GetFuel();
        return fuel - this.LastKnownFuel;
    }

    private void SetTimerText()
    {
        TimeSpan timeRemaining = UnlimitedFuelManager.TimeRemaining;
        string text;
        if ((int)timeRemaining.TotalHours > 0)
        {
            text = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_HOURS_AND_MINUTES"), (int)timeRemaining.TotalHours, timeRemaining.Minutes);
        }
        else
        {
            text = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_MINUTES_AND_SECONDS"), timeRemaining.Minutes, timeRemaining.Seconds);
        }
        //this.TotalGasText.gameObject.SetActive(true);
        this.FuelText.text = text;
    }

    private void TriggerDeltaText()
    {
        int fuel = FuelManager.Instance.GetFuel();
        var fuelMax = FuelManager.Instance.CurrentMaxFuel();
        if (UnlimitedFuelManager.IsActive)
        {
            this.SetTimerText();
        }
        else if (fuel > fuelMax)
        {
            //this.DeltaTextAnimation.gameObject.SetActive(false);
            //this.TotalGasText.gameObject.SetActive(true);
            this.FuelText.text = fuel.ToString();
        }
        else
        {
            //this.TotalGasText.gameObject.SetActive(false);
            int num = fuel - this.LastKnownFuel;
            this.LastKnownFuel = fuel;
            if (num == 0 || !this.WeAreShowing)
            {
                return;
            }
            //string text = ((num <= 0) ? "-" : "+") + Mathf.Abs(num);
            //this.DeltaTextAnimation.gameObject.SetActive(true);
            //this.DeltaText.text = text;
            //this.DeltaTextAnimation.Rewind();
            //this.DeltaTextAnimation.Play();
        }
        this.LastKnownFuel = fuel;
    }

    private void DoFuelGainGlow()
    {
        this.DoFuelGlow(FuelManager.Instance.GetFuel(), "GlowPulse");
    }

    public void DoFuelSpendGlow()
    {
        this.DoFuelGlow(this.LastKnownFuel, "GlowFlashRise");
    }

    private void DoFuelGlow(int amount, string animName)
    {
        //Color color = (amount > 3) ? this.PipColourHigh : this.PipColourLow;
        //this.FuelAnimGlowSprite.SetColor(color);
        //this.FuelAnimGlow.gameObject.SetActive(true);
        //this.FuelAnimGlow.Rewind(animName);
        //this.FuelAnimGlow.Play(animName);
    }

    private void UpdateFuelTimerUpdated()
    {
        if (UnlimitedFuelManager.IsActive)
        {
            this.SetCurrentPipGraphics(false);
            this.SetTimerText();
        }
        //else if (FuelManager.Instance.GetFuel() <= 10 && this.TotalGasText.gameObject.activeSelf)
        //{
        //    this.TotalGasText.gameObject.SetActive(false);
        //}
    }

    private void RemoveUnlimitedFuel()
    {
        var fuelMax = FuelManager.Instance.CurrentMaxFuel();
        if (FuelManager.Instance.GetFuel() <= fuelMax)
        {
            this.SetCurrentPipGraphics(false);
            //this.TotalGasText.gameObject.SetActive(false);
        }
        else
        {
            this.FuelText.text = this.LastKnownFuel.ToString();
            //this.TotalGasText.gameObject.SetActive(true);
        }
    }

    private void UpdateReplenished()
    {
        //if (this.NoFuelInfo != null)
        //{
        //    this.NoFuelInfo.FinishNice();
        //}
        this.UpdateFuelAnimate();
        if (CommonUI.Instance.IsShowingFullNavBar)
        {
            if (FuelManager.Instance.GetFuel() >= FuelManager.Instance.CurrentMaxFuel())
            {
                if (!UnlimitedFuelManager.IsActive)
                {
                    //float xPos = this.Offset.localPosition.x - -0.3f;
                    //float newX = -0.3f;
                    //NavBarInfoPane navBarInfoPane = CommonUI.Instance.NavBarInfoManager.NewForTime("TEXT_NAVBAR_POPDOWN_FUEL_REFILL", false, xPos, 3f, null, true);
                    //if (navBarInfoPane != null)
                    //{
                    //    navBarInfoPane.MoveNipple(newX);
                    //}
                }
            }
            this.TriggerDeltaText();
        }
        this.DoFuelGainGlow();
    }

    private void UpdateFuelInstant()
    {
        this.Animating = false;
        List<ScreenID> list = new List<ScreenID>
		{
			ScreenID.Dummy,
			ScreenID.Pause
		};
        if (list.Contains(ScreenManager.Instance.CurrentScreen))
        {
            this.LastKnownFuel = FuelManager.Instance.GetFuel();
            this.UpdateFuelInstantContinue();
        }
        else
        {
            //AudioManager.Instance.PlaySound("FuelSpend", null);
            this.WaitingForSpendGlowContinue = true;
            this.DoFuelSpendGlow();
        }
    }

    private void UpdateFuelInstantContinue()
    {
        this.SetCurrentPipGraphics(false);
        int deltaPips = this.GetDeltaPips();
        int num = Mathf.Abs(deltaPips);
        this.TriggerDeltaText();
        if (num == 0 || !base.gameObject.activeInHierarchy || !CommonUI.Instance.IsShowingFullNavBar)
        {
            this.LastKnownFuel = FuelManager.Instance.GetFuel();
            return;
        }
        this.ClearOutOldPips();
        for (int i = 1; i <= num; i++)
        {
            //int a = this.LastKnownFuel - i;
            //int num2 = Mathf.Min(a, 9);
            //GameObject original = (num2 >= 3) ? this.FuelSpendAnim : this.FuelSpendAnimLow;
            //GameObject gameObject = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
            //gameObject.transform.parent = this.AnimOffset.transform;
            //FuelPipAnim component = gameObject.GetComponent<FuelPipAnim>();
            //component.Prepare(new Vector3((float)num2 * 0.04f, 0f, 0f));
            //this.WaitingPipAnimations.Add(component);
        }
        //this.WaitingPipAnimations[0].Play();
        this.Animating = false;
    }

    private void UpdateFuelAnimate()
    {
        //if (this.NoFuelInfo != null)
        //{
        //    this.NoFuelInfo.FinishNice();
        //}
        this.Animating = true;
        this.AnimateTo = FuelManager.Instance.GetFuel();
        this.AnimateStart = this.LastShown;
        this.DoFuelGainGlow();
    }

    public void OnScreenChanged(ScreenID newScreen)
    {
        //this.NoFuelInfo = null;
        if (newScreen == ScreenID.CareerModeMap && FuelManager.Instance.GetFuel() == 0)
        {
            this.TriggerFireNoFuelInfo = true;
            this.TriggerFireNoFuelInfoTimer = 0f;
        }
    }

    private void ClearOutOldPips()
    {
        //foreach (FuelPipAnim current in this.WaitingPipAnimations)
        //{
        //    UnityEngine.Object.Destroy(current);
        //}
        //this.WaitingPipAnimations.Clear();
    }

    private void DeactivateAllPips()
    {
        //foreach (FuelPipAnim current in this.WaitingPipAnimations)
        //{
        //    current.Deactivate();
        //}
    }

    public void Show(bool show)
    {
        this.WeAreShowing = show;
        base.gameObject.SetActive(show);
        if (show)
        {
            this.LastKnownFuel = FuelManager.Instance.GetFuel();
            this.SetCurrentPipGraphics(true);
            this.SetupUnlockForInitialState();
        }
        else
        {
            //this.DeltaText.text = string.Empty;
            //this.DeltaTextAnimation.Stop();
        }
    }

    public void ShowCorrectPips(bool canDisplay)
    {
        //List<PackedSprite> list = new List<PackedSprite>
        //{
        //    this.SolidBlue,
        //    this.BluePips,
        //    this.RedPips
        //};
        //foreach (PackedSprite current in list)
        //{
        //    bool active = current == activeSprite && canDisplay;
        //    current.gameObject.SetActive(active);
        //}
    }

    public void SetCurrentPipGraphics(bool forceDisplay = false)
    {
        if(!canUpdateFuel)
            return;
        int num = FuelManager.Instance.GetFuel();
        var maxFuel = FuelManager.Instance.CurrentMaxFuel();
        this.LastShown = (float)num;
        bool canDisplay = (ScreenManager.Instance.CurrentScreen != ScreenID.Dummy && ScreenManager.Instance.CurrentScreen != ScreenID.Pause) || forceDisplay;
        //PackedSprite packedSprite = this.BluePips;
        if (num > maxFuel || (num >= maxFuel && UnlimitedFuelManager.IsActive))
        {
            FuelIcon.color = Color.green;
            this.FuelText.color = Color.green;
            //packedSprite = this.SolidBlue;
        }
        else if (num > 3)
        {
            FuelIcon.color = Color.white;
            this.FuelText.color = Color.white;
        }
        else
        {
            FuelIcon.color = Color.red;
            this.FuelText.color = Color.red;
            //packedSprite = this.RedPips;
        }
        this.ShowCorrectPips(canDisplay);
        if (num <= maxFuel)
        {
            num = Mathf.Max(num, 0);
            //packedSprite.SetColor(new Color(1f, 1f, 1f, this.PipPositions[num]));
        }
        FuelText.text = (maxFuel - num).ToNativeNumber();
    }

    public void SetCurrentPipGraphics(float at)
    {
        this.LastShown = at;
        if (at > 10f)
        {
            this.ShowCorrectPips(true);
            return;
        }
        //int num = Mathf.Max(0, Mathf.FloorToInt(at));
        //int num2 = Mathf.Max(0, Mathf.CeilToInt(at));
        //float num3 = Mathf.Clamp01(at - (float)num);
        //float num4 = this.PipPositions[num];
        //float num5 = this.PipPositions[num2];
        //float a = num3 * num5 + (1f - num3) * num4;
        //PackedSprite packedSprite = (at > 3f) ? this.BluePips : this.RedPips;
        this.ShowCorrectPips(true);
        //packedSprite.SetColor(new Color(1f, 1f, 1f, a));
    }

    private void TryTriggerNextPip()
    {
        //bool flag = false;
        //foreach (FuelPipAnim current in this.WaitingPipAnimations)
        //{
        //    if (current.CanTriggerNext())
        //    {
        //        flag = true;
        //    }
        //    else if (flag)
        //    {
        //        current.Play();
        //        break;
        //    }
        //}
    }

    private void CleanWaitingPipAnimsDestroyed()
    {
        //this.WaitingPipAnimations.RemoveAll((FuelPipAnim q) => q == null);
    }

    private void Update()
    {
        if (!canUpdateFuel)
            return;
        this.CleanWaitingPipAnimsDestroyed();
        this.TryTriggerNextPip();
        if (this.WaitingForSpendGlowContinue)// && !this.FuelAnimGlow.isPlaying)
        {
            if (!UnlimitedFuelManager.IsActive)
                this.AnimateSpend();
            this.UpdateFuelInstantContinue();
            this.WaitingForSpendGlowContinue = false;
        }
        if (this.TriggerFireNoFuelInfo)
        {
            this.TriggerFireNoFuelInfoTimer += Time.deltaTime;
            if (this.TriggerFireNoFuelInfoTimer >= 0.2f)
            {
                this.TriggerFireNoFuelInfo = false;
                this.TriggerFireNoFuelInfoTimer = 0f;
                if (ScreenManager.Instance.CurrentScreen == ScreenID.CareerModeMap)
                {
                    //float xPos = this.Offset.localPosition.x - -0.3f;
                    //float newX = -0.3f;
                    //this.NoFuelInfo = CommonUI.Instance.NavBarInfoManager.NewForScreen("TEXT_NAVBAR_POPDOWN_FUEL_EMPTY", false, xPos, ScreenID.CareerModeMap, new Action(this.OnFuelTap), true);
                    //this.NoFuelInfo.MoveNipple(newX);
                }
            }
        }
        if (this.HasTapShrunk)
        {
            this.HandleTapAnim();
        }
        this.HandleExplosions();
        if (!this.Animating)
        {
            return;
        }
        this.TimerPos += Time.deltaTime;
        if (this.TimerPos >= 0.4f)
        {
            this.Animating = false;
            this.TimerPos = 0f;
            this.SetCurrentPipGraphics(false);
            return;
        }
        float currentPipGraphics = this.AnimateStart + this.TimerPos / 0.4f * ((float)this.AnimateTo - this.AnimateStart);
        this.SetCurrentPipGraphics(currentPipGraphics);
    }

    private void AnimateSpend()
    {
        FlashAnimator.Play("spend");
        AudioManager.Instance.PlaySound(AudioEvent.Frontend_Flash, Camera.main.gameObject);
    }

    private void HandleExplosions()
    {
        //if ((this.DeltaTextAnimation.isPlaying && this.DeltaTextAnimation.gameObject.activeSelf))
        //{
        //    this.hasBeenPlaying = true;
        //    //this.FuelAnimGlowSprite.SetColor(this.FuelAnimGlowSprite.color);
        //    //this.DeltaText.color = (this.DeltaText.color);
        //}
        //else if (this.hasBeenPlaying)
        //{
        //    this.hasBeenPlaying = false;
        //    //this.FuelAnimGlow.gameObject.SetActive(false);
        //    this.DeltaTextAnimation.Stop();
        //    this.DeltaTextAnimation.gameObject.SetActive(false);
        //}
    }

    public void OnFuelTap()
    {
        if (!this.ButtonEnabled)
        {
            return;
        }
        if (!TouchManager.AttemptToUseButton("FuelNavBarButton"))
        {
            return;
        }
        //this.Offset.localScale = new Vector3(0.85f, 0.85f, 1f);
        this.HasTapShrunk = true;
        this.TapAnimTimer = 0f;
        ScreenManager.Instance.PushScreen(ScreenID.GetMoreFuel);
    }

    private void HandleTapAnim()
    {
        this.TapAnimTimer += Time.deltaTime;
        if (this.TapAnimTimer >= 0.2f)
        {
            this.HasTapShrunk = false;
            //this.Offset.localScale = Vector3.one;
        }
    }

    private void UpgradeFuelTank()
    {
        //this.UpgradedTankEffects.gameObject.SetActive(true);
        //AnimationUtils.PlayFirstFrame(this.UpgradeTankAnimation);
        //AnimationUtils.PlayAnim(this.UpgradeTankAnimation);
    }

    private void RemoveFuelTankUpgrade()
    {
        //this.UpgradedTankEffects.gameObject.SetActive(false);
        //AnimationUtils.PlayFirstFrame(this.UpgradeTankAnimation);
    }

    private void SetupUnlockForInitialState()
    {
        if (PlayerProfileManager.Instance.ActiveProfile.HasUpgradedFuelTank)
        {
            //AnimationUtils.PlayLastFrame(this.UpgradeTankAnimation);
            //this.UpgradedTankEffects.gameObject.SetActive(true);
        }
        else
        {
            //AnimationUtils.PlayFirstFrame(this.UpgradeTankAnimation);
            //this.UpgradeTankAnimation.Sample();
            //this.UpgradedTankEffects.gameObject.SetActive(false);
        }
    }

    public void FuelLockedState(bool zLocked)
    {
        this.canUpdateFuel = !zLocked;
    }
}
