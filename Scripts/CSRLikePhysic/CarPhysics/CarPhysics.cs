using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GT/CarPhysics/CarPhysics")]
public class CarPhysics : MonoBehaviour
{
    public delegate bool PreChangeGearUpEventDelegate(GearChangeRating gearChangeRating);

    private float mWheelAngleDelta;

    private float mTheoreticalMaxSpeedForThisGear;

    private DriverInputs mDriverInputs;

    private Engine mEngine;

    private GearBox mGearBox;

    private Wheels mWheels;

    public GearBoxData GearBoxData;

    public EngineData EngineData;

    public TireData TireData;

    public ChassisData ChassisData;

    public NitrousData NitrousData;

    public bool ShowDebugTelem;

    private float mDragForceMagnitude;

    private float mDriveForceMagnitude;

    private float mWantedDriveForceMagnitude;

    private float mRollingFrictionForceMagnitude;

    private float mEngineFrictionMagnitude;

    private float mDistanceTravelled;

    private Vector3 mPosition = default(Vector3);

    private bool ableToStart;

    //private CarSnakeBehaviour xMovementSnake;

    private AnimationCurve xMovementSnakeCurve;

    //private float originalXPosition;

    private bool startTimer = true;

    private float cachedDragCoefficient;

    private float cachedRollingFrictionMagnitude;

    private float cachedInv150MPHASMS;

    public List<MetricsRaceGearShiftEvent> GearShifts;

    public List<CarPhysics.PreChangeGearUpEventDelegate> PreChangeGearUpEventDelegateList = new List<CarPhysics.PreChangeGearUpEventDelegate>();

    public float WheelAngleDelta
    {
        get
        {
            return this.mWheelAngleDelta;
        }
    }

    public float TheoreticalMaxSpeedForThisGear
    {
        get
        {
            return this.mTheoreticalMaxSpeedForThisGear;
        }
    }

    public float MaxSpeedMPH
    {
        get;
        set;
    }

    public float FirstGearTheoreticalSpeedMPH
    {
        get;
        private set;
    }

    public DriverInputs DriverInputs
    {
        get
        {
            return this.mDriverInputs;
        }
        set
        {
            this.mDriverInputs = value;
        }
    }

    public bool IsNetworkPlayer
    {
        get;
        set;
    }

    public Engine Engine
    {
        get
        {
            return this.mEngine;
        }
    }

    public GearBox GearBox
    {
        get
        {
            return this.mGearBox;
        }
    }

    public Wheels Wheels
    {
        get
        {
            return this.mWheels;
        }
    }

    public GearChangeLogic.OutputState GearChangeState
    {
        get
        {
            return this.GearChangeLogic.State;
        }
    }

    public GearChangeLogic GearChangeLogic
    {
        get;
        private set;
    }

    public bool HasHadWheelSpinStart
    {
        get;
        private set;
    }

    public eCarTier CarTier
    {
        get;
        set;
    }

    public float FettledEngineFactor
    {
        get;
        set;
    }

    public int FettledTyreFactor
    {
        get;
        set;
    }

    public int ConsumableTyreFactor
    {
        get;
        set;
    }

    public float ConsumableNitrousBodyFactor
    {
        get;
        set;
    }

    public bool IsUsingNitrous
    {
        get;
        set;
    }

    public bool NitrousAvailable
    {
        get;
        private set;
    }

    public bool HasUsedNitrous
    {
        get;
        set;
    }

    public float DriveForceMagnitude
    {
        get
        {
            return this.mDriveForceMagnitude;
        }
    }

    public float WantedDriveForceMagnitude
    {
        get
        {
            return this.mWantedDriveForceMagnitude;
        }
    }

    public float DistanceTravelled
    {
        get
        {
            return this.mDistanceTravelled;
        }
        set
        {
            this.mDistanceTravelled = value;
        }
    }

    public float PreviousFrameDistanceTravelled
    {
        get;
        private set;
    }

    public float Velocity
    {
        get;
        set;
    }

    public float LastVelocity
    {
        get;
        private set;
    }

    public float SpeedMS
    {
        get;
        private set;
    }

    public float PreviousFrameSpeedMS
    {
        get;
        private set;
    }

    public float SpeedMPH
    {
        get
        {
            return this.SpeedMS * 2.236f;
        }
    }

    public float SpeedKPH
    {
        get
        {
            return this.SpeedMS * 3.57760024f;
        }
    }

    public Vector3 Position
    {
        get
        {
            return this.mPosition;
        }
        set
        {
            this.mPosition = value;
        }
    }

    public float HorsePowerDisplayRatio
    {
        get;
        set;
    }

    public float TransmissionPowerLossMultiplier
    {
        get;
        set;
    }

    public float RedLineRPM
    {
        get
        {
            return this.mEngine.RedLineRPM;
        }
        set
        {
            this.mEngine.RedLineRPM = value;
        }
    }

    public float TrueRedLineRPM
    {
        get
        {
            return this.mEngine.TrueRedLineRPM;
        }
        set
        {
            this.mEngine.TrueRedLineRPM = value;
        }
    }

    public float IdleRPM
    {
        get
        {
            return this.mEngine.IdleRPM;
        }
    }

    public float OptimalLaunchRPM
    {
        get;
        set;
    }

    public float wheelSpinDistance
    {
        get;
        private set;
    }

    public SpeedMileStoneTimer SpeedMileStoneTimer
    {
        get;
        private set;
    }

    public bool FrontendMode
    {
        get;
        set;
    }

    public bool HasLaunched
    {
        get
        {
            return this.mEngine.HasLaunched;
        }
    }

    public float EffectiveMass
    {
        get
        {
            if (this.IsUsingNitrous)
            {
                return this.ChassisData.Mass - this.ConsumableNitrousBodyFactor;
            }
            return this.ChassisData.Mass;
        }
    }

    public float NormalisedRPMForAudio
    {
        get;
        private set;
    }

    public float LongitudinalRollAmount
    {
        get;
        private set;
    }

    private List<bool> DoPreChangeGearUpEvent(GearChangeRating gearChangeRating)
    {
        List<bool> list = new List<bool>();
        foreach (CarPhysics.PreChangeGearUpEventDelegate current in this.PreChangeGearUpEventDelegateList)
        {
            list.Add(current(gearChangeRating));
        }
        return list;
    }

    public void Initialise()
    {
        if (this.mEngine == null)
        {
            this.mEngine = base.gameObject.AddComponent<Engine>();
        }
        if (this.mGearBox == null)
        {
            this.mGearBox = base.gameObject.AddComponent<GearBox>();
        }
        if (this.mWheels == null)
        {
            this.mWheels = base.gameObject.AddComponent<Wheels>();
        }
        this.mEngine.CarPhysics = this;
        this.mGearBox.CarPhysics = this;
        this.mWheels.CarPhysics = this;
        this.mEngine.EngineData = this.EngineData;
        this.mGearBox.GearBoxData = this.GearBoxData;
        this.mWheels.TireData = this.TireData;
        this.mEngine.Initialise(this.FettledEngineFactor);
        this.mWheels.Initialise(this.FettledTyreFactor);
        this.mGearBox.Initialise();
        this.NitrousAvailable = (this.NitrousData.Duration > 0f);
        this.SpeedMileStoneTimer = new SpeedMileStoneTimer();
        this.SpeedMileStoneTimer.CarPhysics = this;
        this.IsUsingNitrous = false;
        //if (!this.FrontendMode)
        //{
        //    this.xMovementSnake = (Resources.Load("Prefabs/SnakeBehaviour") as GameObject).GetComponent<CarSnakeBehaviour>();
        //}
        this.cachedInv150MPHASMS = 0.0149066672f;
        this.GearChangeLogic = new GearChangeLogic(this);
        this.GearShifts = new List<MetricsRaceGearShiftEvent>();
        this.GearChangeLogic.FireGearChangeUpEvent += new GearChangeLogic.ChangeGearUpEventDelegate(this.AddGearShiftUpEvent);
        this.GearChangeLogic.FireGearChangeDownEvent += new GearChangeLogic.ChangeGearDownEventDelegate(this.AddGearShiftDownEvent);
    }

    public void OnDestroy()
    {
        UnityEngine.Object.Destroy(this.mWheels);
        UnityEngine.Object.Destroy(this.mGearBox);
        UnityEngine.Object.Destroy(this.mEngine);
        //this.xMovementSnake = null;
        this.mWheels = null;
        this.mGearBox = null;
        this.mEngine = null;
        this.GearShifts = null;
        this.GearChangeLogic.FireGearChangeUpEvent -= new GearChangeLogic.ChangeGearUpEventDelegate(this.AddGearShiftUpEvent);
        this.GearChangeLogic.FireGearChangeDownEvent -= new GearChangeLogic.ChangeGearDownEventDelegate(this.AddGearShiftDownEvent);
    }

    private void Start()
    {
        RaceController raceController = UnityEngine.Object.FindObjectOfType(typeof(RaceController)) as RaceController;
        if (raceController != null)
        {
            raceController.Events.HandleEvent("RaceStart", new Action(this.RaceStartedEventHandler));
        }
        this.ResetPhysics();
    }

    public void ResetPhysics()
    {
        this.mDragForceMagnitude = 0f;
        this.mDriveForceMagnitude = 0f;
        this.mEngineFrictionMagnitude = 0f;
        this.mDistanceTravelled = 0f;
        this.Velocity = 0f;
        this.HasHadWheelSpinStart = false;
        this.HasUsedNitrous = false;
        this.IsUsingNitrous = false;
        this.SpeedMS = 0f;
        this.PreviousFrameSpeedMS = 0f;
        if (!this.FrontendMode)
        {
            base.gameObject.transform.position = this.mPosition;
            //this.originalXPosition = this.mPosition.x;
            //if (this.xMovementSnake != null)
            //{
            //    this.xMovementSnakeCurve = this.xMovementSnake.XMovementCurve;
            //}
        }
        this.mGearBox.ResetGearBox();
        this.mEngine.ResetEngine();
        this.mWheels.ResetWheels();
        this.ableToStart = false;
        this.MaxSpeedMPH = 0f;
        this.wheelSpinDistance = 0f;
        this.mDriverInputs.Reset();
        if (this.SpeedMileStoneTimer != null)
        {
            this.SpeedMileStoneTimer.Reset();
        }
        this.startTimer = true;
        this.NormalisedRPMForAudio = 0f;
        this.cachedRollingFrictionMagnitude = this.EffectiveMass * 9.81f * this.TireData.RollingFrictionCoefficient;
        float num = 0.85f * this.ChassisData.Width * this.ChassisData.Height;
        this.cachedDragCoefficient = 0.5f * this.ChassisData.DragCoefficient * 1.2f * num;
        this.GearChangeLogic.Setup();
        this.GearChangeLogic.Reset();
        this.GearShifts.Clear();
    }

    public void RaceStartedEventHandler()
    {
        this.ableToStart = true;
    }

    public void RunPreCountdownCarPhysics()
    {
        float num = this.EngineData.EngineRevRate / (this.TrueRedLineRPM - this.IdleRPM);
        if (this.mDriverInputs.Throttle > 0f)
        {
            this.NormalisedRPMForAudio += num * 1.5f * Time.fixedDeltaTime;
        }
        else
        {
            this.NormalisedRPMForAudio -= num * 1.5f * Time.fixedDeltaTime;
        }
        this.NormalisedRPMForAudio = Mathf.Clamp(this.NormalisedRPMForAudio, 0f, 1f);
        this.UpdateVisualWheels();
    }

    public void RunCarPhysics()
    {
        this.HandleInputs();
        this.HasHadWheelSpinStart = ((this.Wheels.WheelSpin > 0f || this.HasHadWheelSpinStart) && this.GearBox.CurrentGear == 1 && !this.Engine.BustingTheEngine);
        float zCombinedGearRatio = this.GearBox.GearRatio(1) * this.GearBoxData.FinalGearRatio;
        this.FirstGearTheoreticalSpeedMPH = CarPhysicsCalculations.CalculateTheoreticalTopSpeedForThisGear(this.mEngine.RedLineRPM, zCombinedGearRatio, this.TireData.WheelRadius) * 2.236f;
        this.mEngine.OrderedUpdate();
        this.NormalisedRPMForAudio = this.Engine.GetNormlisedRPM(this.Engine.GaugeVisibleRPM);
        this.NitrousAvailable = (this.mEngine.NitrousTimeLeft > 0f);
        this.mGearBox.OrderedUpdate();
        this.mWheels.OrderedUpdate();
        this.GearChangeLogic.OrderedUpdate();
        this.mTheoreticalMaxSpeedForThisGear = CarPhysicsCalculations.CalculateTheoreticalTopSpeedForThisGear(this.mEngine.RedLineRPM, this.GearBoxData.FinalGearRatio * this.mGearBox.GearRatioCurrent, this.TireData.WheelRadius);
        float num = this.mEngine.CalculateAcceleration();
        num *= this.EffectiveMass;
        num *= this.mGearBox.Clutch;
        num *= this.TransmissionPowerLossMultiplier;
        float num2 = this.SpeedMS * this.cachedInv150MPHASMS;
        num2 = Mathf.Clamp01(num2);
        float num3 = 200f * num2;
        this.mDragForceMagnitude = this.cachedDragCoefficient * this.Velocity * this.Velocity;
        this.mDriveForceMagnitude = num;
        this.mWantedDriveForceMagnitude = this.mDriveForceMagnitude;
        if (this.Velocity < 0.1f)
        {
            this.mRollingFrictionForceMagnitude = 0f;
        }
        else
        {
            this.mRollingFrictionForceMagnitude = this.cachedRollingFrictionMagnitude;
        }
        this.mEngineFrictionMagnitude = num3;
        float num4 = this.mWheels.GetTotalPoweredTyreForce() * this.Wheels.EvaluateTyreGripFractionAtWheelSpin();
        this.mDriveForceMagnitude = Mathf.Clamp(this.mDriveForceMagnitude, -num4, num4);
        this.LastVelocity = this.Velocity;
        this.IntegrateEuler();
        this.CalculateWeightTransfer();
        this.UpdateVisualWheels();
        if (this.SpeedMPH > this.MaxSpeedMPH)
        {
            this.MaxSpeedMPH = this.SpeedMPH;
        }
        if (this.SpeedMileStoneTimer != null)
        {
            this.SpeedMileStoneTimer.OrderedUpdate();
        }
    }

    private void CalculateWeightTransfer()
    {
        float num = this.Velocity - this.LastVelocity;
        num /= Time.fixedDeltaTime;
        if (this.IsUsingNitrous && this.Engine.SuperNitrousAvailable)
        {
            num *= 2f;
        }
        this.LongitudinalRollAmount = this.ChassisData.LongitudinalBodyRollDamping * this.LongitudinalRollAmount + (1f - this.ChassisData.LongitudinalBodyRollDamping) * num * -this.ChassisData.LongitudinalBodyRollScaleFactor;
        float num2 = this.SpeedMPH / this.ChassisData.LongitudinalBodyRollScaleDownWithSpeed;
        num2 = Mathf.Clamp01(num2 * num2);
        this.LongitudinalRollAmount *= 1f - num2;
    }

    private void UpdateVisualWheels()
    {
        this.mWheelAngleDelta = this.SpeedMS * Time.fixedDeltaTime / (6.28318f * this.TireData.WheelRadius);
    }

    private void IntegrateEuler()
    {
        float num = this.mDriveForceMagnitude;
        num -= this.mDragForceMagnitude;
        num -= this.mRollingFrictionForceMagnitude;
        num -= this.mEngineFrictionMagnitude;
        this.PreviousFrameSpeedMS = this.SpeedMS;
        this.Velocity += num * Time.fixedDeltaTime / this.EffectiveMass;
        this.SpeedMS = this.Velocity;
        float num2 = this.SpeedMS * Time.fixedDeltaTime;
        this.PreviousFrameDistanceTravelled = this.mDistanceTravelled;
        this.mDistanceTravelled += num2;
        this.mPosition.z = this.mPosition.z + num2;
        this.PostIntegrate(num2);
    }

    public void InterpolatePosition(float interpolatedSpeed)
    {
        float num = interpolatedSpeed * Time.fixedDeltaTime;
        this.PreviousFrameDistanceTravelled = this.mDistanceTravelled;
        this.mDistanceTravelled += num;
        this.PreviousFrameSpeedMS = this.SpeedMS;
        this.Velocity = interpolatedSpeed;
        this.SpeedMS = this.Velocity;
        this.mPosition.z = this.mPosition.z + num;
        this.PostIntegrate(num);
        this.UpdateVisualWheels();
        if (this.SpeedMPH > this.MaxSpeedMPH)
        {
            this.MaxSpeedMPH = this.SpeedMPH;
        }
        if (this.SpeedMileStoneTimer != null)
        {
            this.SpeedMileStoneTimer.OrderedUpdate();
        }
    }

    private void PostIntegrate(float deltaDistance)
    {
        if (this.mWheels.WheelSpin > 0f && !this.mEngine.BustingTheEngine)
        {
            this.wheelSpinDistance += deltaDistance;
        }
        if (!this.FrontendMode)
        {
            base.gameObject.transform.position = this.mPosition;
        }
        //if (this.xMovementSnakeCurve != null)
        //{
        //    this.mPosition.x = this.originalXPosition + this.xMovementSnakeCurve.Evaluate(this.mDistanceTravelled / PhysicsConstants.QUARTER_MILE_DISTANCE) * this.xMovementSnake.Scalar;
        //}
    }

    public void HandleInputs()
    {
        this.mEngine.Throttle = this.mDriverInputs.Throttle;
        this.mEngine.NitrousInput = this.mDriverInputs.Nitrous;
        if (this.mDriverInputs.Nitrous)
        {
            this.HasUsedNitrous = true;
        }
        if (this.mDriverInputs.GearChangeDown && this.ableToStart)
        {
            bool flag = this.mGearBox.GearShiftDown();
            if (flag)
            {
                this.mEngine.BustingTheEngine = false;
                this.GearChangeLogic.GearDown();
            }
        }
        if (this.mDriverInputs.GearChangeUp && this.ableToStart)
        {
            if (this.SpeedMileStoneTimer != null && this.startTimer)
            {
                this.startTimer = false;
                this.SpeedMileStoneTimer.Begin();
            }
            if (!this.DoPreChangeGearUpEvent(this.GearChangeLogic.CurrentChangeGearRating()).Contains(false))
            {
                bool flag2 = this.mGearBox.GearShiftUp();
                if (flag2)
                {
                    this.mEngine.BustingTheEngine = false;
                    this.GearChangeLogic.GearUp(this.mGearBox.CurrentGear == 1);
                }
            }
        }
    }

    public string GetGearShiftTypeForMetricsEvent(int index)
    {
        if (index >= this.GearShifts.Count)
        {
            return "NA";
        }
        return this.GearShifts[index].GearShiftRating;
    }

    private void AddGearShiftUpEvent(GearChangeRating rating)
    {
        MetricsRaceGearShiftEvent item;
        item.GearDown = false;
        item.GearUp = true;
        item.GearShiftRating = rating.ToString();
        this.GearShifts.Add(item);
    }

    private void AddGearShiftDownEvent()
    {
        MetricsRaceGearShiftEvent item;
        item.GearDown = true;
        item.GearUp = false;
        item.GearShiftRating = "Down";
        this.GearShifts.Add(item);
    }
}
