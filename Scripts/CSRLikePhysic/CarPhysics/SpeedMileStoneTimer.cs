using System;
using UnityEngine;

public class SpeedMileStoneTimer
{
    public float[] SpeedMilestones;

    private float[] mTimeToReachMilestone;

    private float mTimer;

    private bool mStarted;

    private int mSpeedMileStoneIndex;

    public float mQuarterMileTime;

    public float mHalfMileTime;

    public float mMileTime;

    private CarPhysics mCarPhysics;

    public float SpeedAtQuarter;

    public float SpeedAtHalf;

    public float SpeedAtMile;

    public float[] TimeToReachMilestone
    {
        get
        {
            return this.mTimeToReachMilestone;
        }
    }

    public CarPhysics CarPhysics
    {
        set
        {
            this.mCarPhysics = value;
        }
    }

    public SpeedMileStoneTimer()
    {
        this.SpeedMilestones = new float[]
		{
			60f,
            62.1371192f,
			100f,
			130f,
			150f,
		};
        this.mTimeToReachMilestone = new float[this.SpeedMilestones.Length];
        this.Reset();
    }

    public float CurrentTime()
    {
        return this.mTimer;
    }

    public void Reset()
    {
        this.mStarted = false;
        this.mTimer = 0f;
        this.mSpeedMileStoneIndex = 0;
        this.mQuarterMileTime = -1f;
        this.mHalfMileTime = -1f;
        this.mMileTime = -1f;
        for (int i = 0; i < this.SpeedMilestones.Length; i++)
        {
            this.mTimeToReachMilestone[i] = -1f;
        }
    }

    public void Begin()
    {
        this.mStarted = true;
        this.mTimer = 0f;
    }

    public void OrderedUpdate()
    {
        if (!this.mStarted)
        {
            return;
        }
        this.mTimer += Time.fixedDeltaTime;
        if (this.mSpeedMileStoneIndex < this.SpeedMilestones.Length && this.mCarPhysics.SpeedMPH >= this.SpeedMilestones[this.mSpeedMileStoneIndex])
        {
            float num = this.CalculateTimeSinceSpeed(this.SpeedMilestones[this.mSpeedMileStoneIndex]);
            this.mTimeToReachMilestone[this.mSpeedMileStoneIndex] = this.mTimer - num;
            this.mSpeedMileStoneIndex++;
        }
        if (this.mCarPhysics.DistanceTravelled > PhysicsConstants.QUARTER_MILE_DISTANCE)
        {
            float num2 = this.CalculateTimeSinceFinish(PhysicsConstants.QUARTER_MILE_DISTANCE);
            if (this.mQuarterMileTime == -1f)
            {
                this.mQuarterMileTime = this.mTimer - num2;
                this.SpeedAtQuarter = this.mCarPhysics.SpeedMPH;
            }
        }
        if (this.mCarPhysics.DistanceTravelled > PhysicsConstants.HALF_MILE_DISTANCE)
        {
            float num3 = this.CalculateTimeSinceFinish(PhysicsConstants.HALF_MILE_DISTANCE);
            if (this.mHalfMileTime == -1f)
            {
                this.mHalfMileTime = this.mTimer - num3;
                this.SpeedAtHalf = this.mCarPhysics.SpeedMPH;
            }
        }
        if (this.mCarPhysics.DistanceTravelled > 1609.3f)
        {
            float num4 = this.CalculateTimeSinceFinish(1609.3f);
            if (this.mMileTime == -1f)
            {
                this.mMileTime = this.mTimer - num4;
                this.SpeedAtMile = this.mCarPhysics.SpeedMPH;
            }
        }
    }

    private float CalculateTimeSinceFinish(float zDistanceMarker)
    {
        float num = this.mCarPhysics.DistanceTravelled - zDistanceMarker;
        return num / this.mCarPhysics.SpeedMS;
    }

    private float CalculateTimeSinceSpeed(float zSpeedMarker)
    {
        float num = this.mCarPhysics.SpeedMS - zSpeedMarker * 0.44722718f;
        float num2 = (this.mCarPhysics.SpeedMS - this.mCarPhysics.PreviousFrameSpeedMS) / Time.fixedDeltaTime;
        return num / num2;
    }
}
