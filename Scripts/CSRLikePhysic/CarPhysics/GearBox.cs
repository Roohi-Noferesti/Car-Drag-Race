using System;
using UnityEngine;

[AddComponentMenu("GT/CarPhysics/GearBox")]
public class GearBox : MonoBehaviour
{
    public static float[] gearRatioRangesLower = new float[]
	{
		2f,
		1.5f,
		1.1f,
		0.85f,
		0.65f,
		0.5f,
		0.4f
	};

    public static float finalDriveRatioLower = 2f;

    public static float finalDriveRatioHigher = 5f;

    public static float[] gearRatioRangesHigher = new float[]
	{
		4.5f,
		3f,
		2.1f,
		1.7f,
		1.5f,
		1.3f,
		1.1f
	};

    private CarPhysics mCarPhysics;

    private int mCurrentGear;

    private int mPreviousGear;

    private float mClutchDelay;

    private float mClutch;

    private float[] calculatedOptimalGearChangeMPHArray;

    public GearBoxData GearBoxData
    {
        get;
        set;
    }

    public CarPhysics CarPhysics
    {
        set
        {
            this.mCarPhysics = value;
        }
    }

    public int CurrentGear
    {
        get
        {
            return this.mCurrentGear;
        }
    }

    public int OldGear
    {
        get
        {
            return this.mPreviousGear;
        }
    }

    public int NumGears
    {
        get;
        private set;
    }

    public bool IsInTopGear
    {
        get
        {
            return this.mCurrentGear == this.calculatedOptimalGearChangeMPHArray.Length;
        }
    }

    public float GearRatioCurrent
    {
        get
        {
            return this.GearRatio(this.mCurrentGear);
        }
    }

    public float GearRatioOld
    {
        get
        {
            return this.GearRatio(this.mPreviousGear);
        }
    }

    public float GearRatioFinal
    {
        get
        {
            return this.GearBoxData.FinalGearRatio;
        }
    }

    public bool IsGearShifting
    {
        get
        {
            return this.mClutch != 1f;
        }
    }

    public float CombinedGearRatio
    {
        get
        {
            if (this.CurrentGear == 0)
            {
                return 1f;
            }
            return this.GearRatioCurrent * this.GearRatioFinal;
        }
    }

    public bool IsInNeutral
    {
        get
        {
            return this.mCurrentGear == 0 && this.mClutch == 0f;
        }
    }

    public float Clutch
    {
        get
        {
            return this.mClutch;
        }
    }

    public float GearRatioFinalDrive
    {
        get
        {
            return this.GearBoxData.FinalGearRatio;
        }
    }

    public float[] CalculatedOptimalGearChangeMPHArray()
    {
        return this.calculatedOptimalGearChangeMPHArray;
    }

    public void SetOptimalGearChangeMPHArray(float[] zOptimalGearChangeArray)
    {
        this.calculatedOptimalGearChangeMPHArray = new float[zOptimalGearChangeArray.Length];
        zOptimalGearChangeArray.CopyTo(this.calculatedOptimalGearChangeMPHArray, 0);
    }

    public int GetNextGear()
    {
        if (this.mCurrentGear == this.GearBoxData.GearRatios.Length)
        {
            return this.mCurrentGear;
        }
        return this.mCurrentGear + 1;
    }

    public void Initialise()
    {
        this.NumGears = this.GearBoxData.GearRatios.Length;
    }

    private void Start()
    {
        this.ResetGearBox();
    }

    public void ResetGearBox()
    {
        this.mCurrentGear = (this.mPreviousGear = 0);
        this.mClutch = 0f;
    }

    public void OrderedUpdate()
    {
        this.UpdateClutch();
    }

    private void UpdateClutch()
    {
        if (this.mCurrentGear == 0)
        {
            return;
        }
        this.mClutch += Time.fixedDeltaTime / this.mClutchDelay;
        this.mClutch = Mathf.Clamp01(this.mClutch);
    }

    public bool GearShiftUp()
    {
        if (this.mCurrentGear == this.GearBoxData.GearRatios.Length)
        {
            return false;
        }
        if (this.mCurrentGear > 0 && this.mClutch != 1f)
        {
            return false;
        }
        this.mClutch = 0f;
        if (this.CurrentGear == 0)
        {
            this.mClutchDelay = this.GearBoxData.ClutchDelayFirstGear;
        }
        else
        {
            this.mClutchDelay = this.GearBoxData.ClutchDelay;
        }
        this.mPreviousGear = this.mCurrentGear;
        this.mCurrentGear++;
        this.mCarPhysics.Engine.CalculateEngineForce();
        this.mCarPhysics.Wheels.CalculateWheelSpin();
        return true;
    }

    public bool GearShiftDown()
    {
        if (this.mCurrentGear == 1)
        {
            return false;
        }
        if (this.mClutch != 1f)
        {
            return false;
        }
        this.mClutch = 0f;
        this.mClutchDelay = this.GearBoxData.ClutchDelay;
        this.mPreviousGear = this.mCurrentGear;
        this.mCurrentGear--;
        return true;
    }

    public float GearRatio(int zGearNumber)
    {
        if (zGearNumber == 0)
        {
            return 0f;
        }
        return this.GearBoxData.GearRatios[zGearNumber - 1];
    }

    public void SetGearRatio(int zGearNumber, float zRatio)
    {
        this.GearBoxData.GearRatios[zGearNumber - 1] = zRatio;
    }

    public void SetFinalRatio(float zRatio)
    {
        this.GearBoxData.FinalGearRatio = zRatio;
    }
}
