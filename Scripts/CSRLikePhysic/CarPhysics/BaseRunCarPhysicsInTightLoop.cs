public class BaseRunCarPhysicsInTightLoop
{
    private const int MAX_NUM_FRAMES = 3000;

    protected CarPhysics carPhysics;

    protected bool isFirstFrame;

    protected bool isZerothFrame;

    protected DriverInputs driverInputs;

    public float startRPM { get; protected set; }

    public bool autoFireNitrous { get; set; }

    public SpeedMileStoneTimer speedMileStoneTimer { get; set; }

    public float startRevRPM { get; set; }

    public float nitrousUseMPH { get; set; }

    public float distanceOfTest { get; set; }

    public BaseRunCarPhysicsInTightLoop(CarPhysics zCarPhysicsIn)
    {
        this.carPhysics = zCarPhysicsIn;
        this.distanceOfTest = 1609.3f;
        this.autoFireNitrous = true;
    }

    public void StartTightLoopRun()
    {
        this.isFirstFrame = false;
        this.isZerothFrame = true;
        this.carPhysics.ResetPhysics();
        this.speedMileStoneTimer = this.carPhysics.SpeedMileStoneTimer;
    }

    public bool Execute(int numFrames = 3000)
    {
        bool result = false;
        for (int i = 0; i < numFrames; i++)
        {
            DriverInputs currentDriverInputs;
            currentDriverInputs.GearChangeUp = false;
            currentDriverInputs.GearChangeDown = false;
            currentDriverInputs.Nitrous = false;
            currentDriverInputs.Throttle = 1f;
            if (this.isFirstFrame)
            {
                currentDriverInputs.GearChangeUp = true;
                this.isFirstFrame = false;
            }
            this.OverrideInputsForAutomaticDriving(out currentDriverInputs, currentDriverInputs);
            this.carPhysics.DriverInputs = currentDriverInputs;
            this.carPhysics.HandleInputs();
            this.carPhysics.RunCarPhysics();
            if (this.isZerothFrame)
            {
                this.carPhysics.RaceStartedEventHandler();
                this.isZerothFrame = false;
                this.isFirstFrame = true;
            }
            if (this.carPhysics.DistanceTravelled > this.distanceOfTest)
            {
                result = true;
                break;
            }
            //Debug.Log(carPhysics.DistanceTravelled);
        }
        return result;
    }

    public virtual void OverrideInputsForAutomaticDriving(out DriverInputs driverInputs,
        DriverInputs currentDriverInputs)
    {
        driverInputs.GearChangeUp = currentDriverInputs.GearChangeUp;
        driverInputs.GearChangeDown = currentDriverInputs.GearChangeDown;
        driverInputs.Nitrous = currentDriverInputs.Nitrous;
        driverInputs.Throttle = currentDriverInputs.Throttle;
        if (this.isZerothFrame)
        {
            this.carPhysics.Engine.AutoLauchRPM = this.startRevRPM;
        }
        int num = this.carPhysics.GearBox.CurrentGear - 1;
        if (num >= 0 && this.carPhysics.SpeedMPH > this.carPhysics.GearBox.CalculatedOptimalGearChangeMPHArray()[num])
        {
            driverInputs.GearChangeUp = true;
        }
        if (this.autoFireNitrous && this.carPhysics.SpeedMPH > this.nitrousUseMPH)
        {
            driverInputs.Nitrous = true;
        }
    }
}
