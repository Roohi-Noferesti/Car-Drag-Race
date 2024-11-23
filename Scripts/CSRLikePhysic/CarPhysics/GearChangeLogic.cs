#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GearChangeLogic
{
    public struct OutputState
    {
        public float normalisedLightsNumber;

        public float earlyGearStartNumber;

        public float goodGearStartNumber;

        public float lateGearStartNumber;

        public float perfectGearStartNumber;

        public float perfectGearEndNumber;

        public int currentGear;

        public bool inNeutralGear;
    }

    public delegate void ChangeGearUpEventDelegate(GearChangeRating gearChangeRating);

    public delegate void EnterRPMRegionDelegate(RPMRegion rpmRegion, bool isRacing);

    public delegate void ChangeGearDownEventDelegate();

    private GearChangeRating gearChangeRating = GearChangeRating.NotSet;

    private RPMRegion RPMregion = RPMRegion.NotSet;

    private RPMRegion oldRPMRegion = RPMRegion.NotSet;

    private OutputState state;

    private float launchGreenBand;

    private float perfectLaunchBandFraction;

    private float launchWindowLeadupBand;

    private float okayLaunchBand;

    private float slowStartBandMin;

    private float RPMforPerfectShift;

    private float leadUpToGearChangeBand;

    private float goodGearChangeBand;

    private float perfectGearChangeBandFraction;

    private bool redlineTouched;

    private bool shouldShowLateMessage;

    private float RPMRangeReciprocal;

    private float shiftTimer;

    private float perfectShiftTimeTolerance = 0.5f;

    private float perfectShiftZoneScalar = 0.225f;

    private CarGearChangeLightRangesData m_carGearChangeLightRangesData;

    public event ChangeGearUpEventDelegate FireGearChangeUpEvent;

    public event EnterRPMRegionDelegate FireEnterRPMRegionEvent;

    public event ChangeGearDownEventDelegate FireGearChangeDownEvent;

    public int NumGearChanges { get; private set; }

    public bool DidGreatLaunch { get; private set; }

    public int NumPerfectChanges { get; private set; }

    public int NumEarlyChanges { get; private set; }

    public int NumGoodChanges { get; private set; }

    public int NumLateChanges { get; private set; }

    public OutputState State
    {
        get { return this.state; }
    }

    public CarPhysics CarPhysics { get; private set; }

    public GearChangeLogic(CarPhysics physics)
    {
        this.CarPhysics = physics;
        CarGearChangeLightRangesData.GetTotalGearLightRPMRange(out this.launchWindowLeadupBand, out this.launchGreenBand,
            out this.perfectLaunchBandFraction, out this.leadUpToGearChangeBand, out this.goodGearChangeBand,
            out this.perfectGearChangeBandFraction, out this.okayLaunchBand, out this.slowStartBandMin, physics.CarTier);

#if UNITY_EDITOR
        m_carGearChangeLightRangesData = AssetDatabase.LoadAssetAtPath<CarsConfiguration>("Assets/configuration/CarsConfiguration.asset").CarGearLightData;
#else
        m_carGearChangeLightRangesData = GameDatabase.Instance.CarsConfiguration.CarGearLightData;
#endif
    }

    public void Setup()
    {
        float num = this.CarPhysics.RedLineRPM - this.CarPhysics.IdleRPM;
        this.RPMRangeReciprocal = 1f/num;
        this.Reset();
    }

    public void Reset()
    {
        this.redlineTouched = false;
        this.shouldShowLateMessage = false;
        this.shiftTimer = 0f;
        this.RPMregion = RPMRegion.NotSet;
        this.gearChangeRating = GearChangeRating.NotSet;
        this.oldRPMRegion = RPMRegion.NotSet;
        this.NumGearChanges = 0;
        this.DidGreatLaunch = false;
        this.NumPerfectChanges = 0;
        this.NumEarlyChanges = 0;
        this.NumGoodChanges = 0;
        this.NumLateChanges = 0;
    }

    public void OrderedUpdate()
    {
        this.state.normalisedLightsNumber = (this.CarPhysics.Engine.CurrentRPM - this.CarPhysics.IdleRPM) * this.RPMRangeReciprocal;
        this.state.currentGear = this.CarPhysics.GearBox.CurrentGear;
        this.state.inNeutralGear = (this.CarPhysics.GearBox.CurrentGear == 0);
        this.gearChangeRating = GearChangeRating.NotSet;
        if (this.CarPhysics.GearBox.Clutch < 1f && !this.CarPhysics.GearBox.IsInNeutral)
        {
            this.state.normalisedLightsNumber = -1f;
            return;
        }
        if (this.CarPhysics.GearBox.CurrentGear == 0)
        {
            this.CalculateForLaunch();
            this.CalculateChangeGearRating(true);
            this.CalculateLaunchGearRating();
            return;
        }
        if (this.CarPhysics.GearBox.IsInTopGear)
        {
            this.state.normalisedLightsNumber = -1f;
            this.gearChangeRating = GearChangeRating.NotSet;
            return;
        }

        //if (this.CarPhysics.HasHadWheelSpinStart)
        //{
        //    this.CalculateWheelSpinStart();
        //}
        //else
        //{
        //    this.CalculateForRace();
        //}
        this.CalculateForRace();
        if (this.CarPhysics.Engine.CurrentRPM >= this.CarPhysics.Engine.RedLineRPM)
        {
            this.redlineTouched = true;
        }
        if (this.redlineTouched)
        {
            this.state.normalisedLightsNumber = 1.5f;
            this.shiftTimer += Time.deltaTime;
            if (this.shiftTimer > this.perfectShiftTimeTolerance)
            {
                this.shouldShowLateMessage = true;
            }
        }
        this.CalculateChangeGearRating(false);
    }

    private void CalculateForRace()
    {
        //float num = this.RPMforPerfectShift + this.goodGearChangeBand*this.perfectGearChangeBandFraction;
        //float num2 = num - (this.CarPhysics.Engine.RedLineRPM - 10f);
        //if (num2 > 0f)
        //{
        //    this.RPMforPerfectShift -= num2;
        //}
        //float num3 = this.RPMforPerfectShift - this.goodGearChangeBand*0.5f - this.leadUpToGearChangeBand;
        //this.state.earlyGearStartNumber = (num3 - this.CarPhysics.IdleRPM)*this.RPMRangeReciprocal;
        //this.state.goodGearStartNumber = (this.RPMforPerfectShift - this.goodGearChangeBand*0.5f -
        //                                  this.CarPhysics.IdleRPM)*this.RPMRangeReciprocal;
        //this.state.perfectGearStartNumber = (this.RPMforPerfectShift -
        //                                     this.goodGearChangeBand*this.perfectGearChangeBandFraction*0.5f -
        //                                     this.CarPhysics.IdleRPM)*this.RPMRangeReciprocal;
        //this.state.perfectGearEndNumber = (this.RPMforPerfectShift +
        //                                   this.goodGearChangeBand*this.perfectGearChangeBandFraction*0.5f -
        //                                   this.CarPhysics.IdleRPM)*this.RPMRangeReciprocal;
        //this.state.lateGearStartNumber = (this.RPMforPerfectShift + this.goodGearChangeBand*0.5f -
        //                                  this.CarPhysics.IdleRPM)*this.RPMRangeReciprocal;

        //this.state.PerfectGearStartRpm = (this.RPMforPerfectShift - this.goodGearChangeBand * 0.5f) ;
        //this.state.PerfectGearEndRpm = (this.RPMforPerfectShift + this.goodGearChangeBand * 0.5f) ;



        float num = this.perfectGearChangeBandFraction;
        if (true)//RaceHUDManager.HUD_V2 && RaceHUDManager.EnablePerfectShiftScalar)
        {
            num = this.perfectGearChangeBandFraction * (1f - ((float)this.State.currentGear - 1f) * this.perfectShiftZoneScalar);
            if (num < 0.1f)
            {
                num = 0.1f;
            }
        }
        //float num2 = this.RPMforPerfectShift + this.goodGearChangeBand * num * 0.5f;
        //if (true) //RaceHUDManager.HUD_V2 && RaceHUDManager.EnablePerfectShiftAdjustment)
        //{
        //    float num3 = num2 - this.CarPhysics.Engine.RedLineRPM;
        //    if (num3 > 0f)
        //    {
        //        this.RPMforPerfectShift -= num3;
        //    }
        //}
        float num4 = this.RPMforPerfectShift - this.goodGearChangeBand * 0.5f - this.leadUpToGearChangeBand;
        this.state.earlyGearStartNumber = (num4 - this.CarPhysics.IdleRPM) * this.RPMRangeReciprocal;
        this.state.goodGearStartNumber = (this.RPMforPerfectShift - this.goodGearChangeBand * 0.5f - this.CarPhysics.IdleRPM) * this.RPMRangeReciprocal;
        this.state.perfectGearStartNumber = (this.RPMforPerfectShift - this.goodGearChangeBand * num * 0.5f - this.CarPhysics.IdleRPM) * this.RPMRangeReciprocal;
        this.state.perfectGearEndNumber = (this.RPMforPerfectShift + this.goodGearChangeBand * num * 0.5f - this.CarPhysics.IdleRPM) * this.RPMRangeReciprocal;
        this.state.lateGearStartNumber = (this.RPMforPerfectShift + this.goodGearChangeBand * 0.5f - this.CarPhysics.IdleRPM) * this.RPMRangeReciprocal;
    }

    private void CalculateWheelSpinStart()
    {
        this.state.normalisedLightsNumber = this.CarPhysics.SpeedMPH/
                                            (this.CarPhysics.TheoreticalMaxSpeedForThisGear*2.236f);
        this.state.normalisedLightsNumber = Mathf.Clamp01(this.state.normalisedLightsNumber);
        this.state.earlyGearStartNumber = 0.5f;
        this.state.goodGearStartNumber = 0.8f;
        this.state.perfectGearStartNumber = 0.825f;
        this.state.perfectGearEndNumber = 0.875f;
        this.state.lateGearStartNumber = 0.9f;

    }

    private void CalculateForLaunch()
    {
        this.state.goodGearStartNumber = (this.CarPhysics.OptimalLaunchRPM - this.launchGreenBand*0.5f -
                                          this.CarPhysics.IdleRPM)*this.RPMRangeReciprocal;
        this.state.lateGearStartNumber = (this.CarPhysics.OptimalLaunchRPM + this.launchGreenBand*0.5f -
                                          this.CarPhysics.IdleRPM)*this.RPMRangeReciprocal;
        this.state.perfectGearStartNumber = (this.CarPhysics.OptimalLaunchRPM -
                                             this.launchGreenBand*this.perfectLaunchBandFraction*0.5f -
                                             this.CarPhysics.IdleRPM)*this.RPMRangeReciprocal;
        this.state.perfectGearEndNumber = (this.CarPhysics.OptimalLaunchRPM +
                                           this.launchGreenBand*this.perfectLaunchBandFraction*0.5f -
                                           this.CarPhysics.IdleRPM)*this.RPMRangeReciprocal;

        if (m_carGearChangeLightRangesData.ShouldShowOKStart(this.CarPhysics.CarTier))
        {
            float b = this.CarPhysics.OptimalLaunchRPM - this.launchGreenBand*0.5f - this.okayLaunchBand -
                      this.CarPhysics.IdleRPM;
            this.state.earlyGearStartNumber = Mathf.Max(this.slowStartBandMin, b)*this.RPMRangeReciprocal;
        }
        else
        {
            this.state.earlyGearStartNumber = (this.CarPhysics.OptimalLaunchRPM - this.launchGreenBand*0.5f -
                                               this.launchWindowLeadupBand - this.CarPhysics.IdleRPM)*
                                              this.RPMRangeReciprocal;
        }
    }

    private void CalculateLaunchGearRating()
    {
        if (this.state.normalisedLightsNumber > this.state.lateGearStartNumber)
        {
            this.gearChangeRating = GearChangeRating.WheelspinLaunch;
        }
        else if (this.state.normalisedLightsNumber > this.state.goodGearStartNumber)
        {
            this.gearChangeRating = GearChangeRating.GoodLaunch;
            if (this.state.normalisedLightsNumber >= this.state.perfectGearStartNumber &&
                this.state.normalisedLightsNumber <= this.state.perfectGearEndNumber)
            {
                this.gearChangeRating = GearChangeRating.PerfectLaunch;
            }
        }
        else if (this.state.normalisedLightsNumber > this.state.earlyGearStartNumber
            && m_carGearChangeLightRangesData.ShouldShowOKStart(this.CarPhysics.CarTier))
        {
            this.gearChangeRating = GearChangeRating.OkayLaunch;
        }
        else
        {
            this.gearChangeRating = GearChangeRating.SlowLaunch;
        }
    }

    public GearChangeRating CurrentChangeGearRating()
    {
        return this.gearChangeRating;
    }

    private void CalculateChangeGearRating(bool rpmRegionOnly)
    {
        this.oldRPMRegion = this.RPMregion;
        if (this.state.normalisedLightsNumber > this.state.lateGearStartNumber || this.shouldShowLateMessage)
        {
            this.gearChangeRating = GearChangeRating.Late;
            this.RPMregion = RPMRegion.Late;
        }
        else if (this.state.normalisedLightsNumber > this.state.goodGearStartNumber)
        {
            this.gearChangeRating = GearChangeRating.Good;
            this.RPMregion = RPMRegion.Ready;
            if (this.state.normalisedLightsNumber >= this.state.perfectGearStartNumber &&
                this.state.normalisedLightsNumber <= this.state.perfectGearEndNumber)
            {
                this.RPMregion = RPMRegion.Now;
                this.gearChangeRating = GearChangeRating.Perfect;
            }
        }
        else if (this.state.normalisedLightsNumber > this.state.earlyGearStartNumber)
        {
            this.RPMregion = RPMRegion.NearlyReady;
            float num = this.state.goodGearStartNumber - this.state.earlyGearStartNumber;
            if (this.state.normalisedLightsNumber - this.state.earlyGearStartNumber > num * 0.66f)
            {
                this.RPMregion = RPMRegion.Ready;
            }
            this.gearChangeRating = GearChangeRating.Early;
        }
        else
        {
            this.RPMregion = RPMRegion.NotReady;
            this.gearChangeRating = GearChangeRating.VeryEarly;
        }
        this.FireRPMRegionEvents();



        //GearChangeRating gearChangeRating = this.gearChangeRating;
        //this.oldRPMRegion = this.RPMregion;
        //if (this.state.normalisedLightsNumber > this.state.lateGearStartNumber || this.shouldShowLateMessage)
        //{
        //    bool flag = false;
        //    if (RaceHUDController.Instance != null && RaceHUDController.Instance.hudTacho != null)
        //    {
        //        flag = (RaceHUDController.Instance.hudTacho.maximumPerfectRevs >= RaceHUDController.Instance.HumanCarPhysics.Engine.RedLineRPM);
        //    }
        //    if (flag && this.redlineTouched)
        //    {
        //        this.timeBeenAtTopEndOfRevs += Time.deltaTime;
        //        if (this.timeBeenAtTopEndOfRevs > 0.5f)
        //        {
        //            if (!rpmRegionOnly)
        //            {
        //                this.gearChangeRating = GearChangeRating.Late;
        //            }
        //            this.RPMregion = RPMRegion.Late;
        //        }
        //        else if (this.timeBeenAtTopEndOfRevs > 0.3f && this.timeBeenAtTopEndOfRevs <= 0.5f)
        //        {
        //            if (!rpmRegionOnly)
        //            {
        //                this.gearChangeRating = GearChangeRating.Good;
        //            }
        //            this.RPMregion = RPMRegion.Now;
        //        }
        //        else
        //        {
        //            if (!rpmRegionOnly)
        //            {
        //                this.gearChangeRating = GearChangeRating.Perfect;
        //            }
        //            this.RPMregion = RPMRegion.Now;
        //        }
        //    }
        //    else
        //    {
        //        if (!rpmRegionOnly)
        //        {
        //            this.gearChangeRating = GearChangeRating.Late;
        //        }
        //        this.RPMregion = RPMRegion.Late;
        //    }
        //}
        //else if (this.state.normalisedLightsNumber > this.state.goodGearStartNumber)
        //{
        //    if (!rpmRegionOnly)
        //    {
        //        this.gearChangeRating = GearChangeRating.Good;
        //    }
        //    if (this.state.normalisedLightsNumber >= this.state.perfectGearStartNumber && this.state.normalisedLightsNumber <= this.state.perfectGearEndNumber)
        //    {
        //        this.RPMregion = RPMRegion.Now;
        //        if (!rpmRegionOnly)
        //        {
        //            this.gearChangeRating = GearChangeRating.Perfect;
        //        }
        //    }
        //}
        //else if (this.state.normalisedLightsNumber > this.state.earlyGearStartNumber)
        //{
        //    this.RPMregion = RPMRegion.NearlyReady;
        //    float num = this.state.goodGearStartNumber - this.state.earlyGearStartNumber;
        //    if (this.state.normalisedLightsNumber - this.state.earlyGearStartNumber > num * 0.66f)
        //    {
        //        this.RPMregion = RPMRegion.Ready;
        //    }
        //    if (!rpmRegionOnly)
        //    {
        //        this.gearChangeRating = GearChangeRating.Early;
        //    }
        //}
        //else
        //{
        //    this.RPMregion = RPMRegion.NotReady;
        //    if (!rpmRegionOnly)
        //    {
        //        this.gearChangeRating = GearChangeRating.VeryEarly;
        //    }
        //}
        //this.FireRPMRegionEvents();
        //if (!rpmRegionOnly && this.gearChangeRating != gearChangeRating && this.FireChangeGearRatingEvent != null)
        //{
        //    this.FireChangeGearRatingEvent(this.gearChangeRating, !this.CarPhysics.GearBox.IsInNeutral);
        //}
    }

    public void FireRPMRegionEvents()
    {
        if (this.FireEnterRPMRegionEvent == null)
        {
            return;
        }
        if (this.oldRPMRegion != this.RPMregion && this.RPMregion != RPMRegion.NotSet)
        {
            this.FireEnterRPMRegionEvent(this.RPMregion, !this.CarPhysics.GearBox.IsInNeutral);
        }
        this.oldRPMRegion = this.RPMregion;
    }

    public void GearUp(bool isChangeFromNeutralToFirst)
    {
        this.redlineTouched = false;
        this.shouldShowLateMessage = false;
        this.shiftTimer = 0f;
        this.RPMregion = RPMRegion.NotSet;
        this.oldRPMRegion = RPMRegion.NotSet;
        if (this.FireGearChangeUpEvent != null)
        {
            this.FireGearChangeUpEvent(this.gearChangeRating);
        }
        if (!isChangeFromNeutralToFirst)
        {
            this.NumGearChanges++;
        }
        switch (this.gearChangeRating)
        {
            case GearChangeRating.SlowLaunch:
            case GearChangeRating.OkayLaunch:
            case GearChangeRating.GoodLaunch:
            case GearChangeRating.WheelspinLaunch:
                this.DidGreatLaunch = false;
                break;
            case GearChangeRating.PerfectLaunch:
                this.DidGreatLaunch = true;
                break;
            case GearChangeRating.Early:
                this.NumEarlyChanges++;
                break;
            case GearChangeRating.Good:
                this.NumGoodChanges++;
                break;
            case GearChangeRating.Perfect:
                this.NumPerfectChanges++;
                break;
            case GearChangeRating.Late:
                this.NumLateChanges++;
                break;
        }
        this.FireGearSounds();
        this.CalculateOptimalShiftPoint();
    }

    public void GearDown()
    {
        this.redlineTouched = false;
        this.shouldShowLateMessage = false;
        this.shiftTimer = 0f;
        this.RPMregion = RPMRegion.NotSet;
        this.oldRPMRegion = RPMRegion.NotSet;
        if (this.FireGearChangeDownEvent != null)
        {
            this.FireGearChangeDownEvent();
        }
        this.CalculateOptimalShiftPoint();
    }

    private void FireGearSounds()
    {
        if (!this.CarPhysics.FrontendMode && (CompetitorManager.Instance != null) &&
            CompetitorManager.Instance.LocalCompetitor != null &&
            this.CarPhysics == CompetitorManager.Instance.LocalCompetitor.CarPhysics &&
            AudioManager.Instance.EnableGears)
        {
            switch (this.gearChangeRating)
            {
                case GearChangeRating.GoodLaunch:
                case GearChangeRating.Good:
                    AudioManager.Instance.PlaySound(AudioEvent.HUD_ShiftGood, Camera.main.gameObject);
                    break;

                case GearChangeRating.PerfectLaunch:
                case GearChangeRating.Perfect:
                    AudioManager.Instance.PlaySound(AudioEvent.HUD_ShiftPerfect, Camera.main.gameObject);
                    break;

                case GearChangeRating.VeryEarly:
                case GearChangeRating.Early:
                case GearChangeRating.Late:
                    AudioManager.Instance.PlaySound(AudioEvent.HUD_ShiftBad, Camera.main.gameObject);
                    break;
            }
        }
    }

    private void CalculateOptimalShiftPoint()
    {
        int currentGear = this.CarPhysics.GearBox.CurrentGear;
        if (currentGear < 1)
        {
            return;
        }
        float[] array = this.CarPhysics.GearBox.CalculatedOptimalGearChangeMPHArray();
        float zLinearSpeed = array[currentGear - 1]*0.44722718f;
        this.RPMforPerfectShift =
            PhysicsConstants.WheelRPMFromLinearSpeed(this.CarPhysics.TireData.WheelRadius, zLinearSpeed)*
            this.CarPhysics.GearBox.CombinedGearRatio;
    }
}
