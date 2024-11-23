using UnityEngine;

public static class CarPhysicsCalculations
{
    public static float CalculateTheoreticalTopSpeedForThisGear(float zEngineRedline, float zCombinedGearRatio,
        float zTireRadius)
    {
        float num = zEngineRedline/(60f*zCombinedGearRatio);
        float num2 = 6.28318f*num;
        return zTireRadius*num2;
    }

    public static float EvaluateTorqueAtWheelAtThisRPM(InGameTorqueCurve torqueCurve, float zRPMNormalised,
        float extraTorqueFromNitrousInFootPounds, float combinedGearRatio)
    {
        return (torqueCurve.EvaluateTorqueAtNormalisedRPM(zRPMNormalised) + extraTorqueFromNitrousInFootPounds)*
               combinedGearRatio*1.355818f;
    }

    public static float ExtrapolateApproximateFinishTime(CarPhysics carPhysics, bool isHalfMile,
        out float extrapolatedFinishSpeed)
    {
        float num = (!isHalfMile) ? 402.325f : 804.65f;
        float speedMS = carPhysics.SpeedMS;
        float num2 = num - carPhysics.DistanceTravelled;
        float num3 = (carPhysics.SpeedMS - carPhysics.PreviousFrameSpeedMS)/Time.fixedDeltaTime;
        num3 = Mathf.Clamp(num3, 0.01f, 100f);
        if (carPhysics.SpeedMileStoneTimer.SpeedMilestones[2] == -1f)
        {
            float num4 = 100f - speedMS;
            float num5 = num4/num3;
            carPhysics.SpeedMileStoneTimer.SpeedMilestones[2] = carPhysics.SpeedMileStoneTimer.CurrentTime() + num5;
        }
        float f = speedMS*speedMS + 2f*num3*num2;
        float num6 = (Mathf.Sqrt(f) - speedMS)/num3;
        extrapolatedFinishSpeed = 2.236f*(speedMS + num3*num6);
        return num6 + carPhysics.SpeedMileStoneTimer.CurrentTime();
    }

    public static float ExtrapolateApproximateFinishTime(CarPhysics carPhysics, bool isHalfMile)
    {
        float num = 0f;
        return ExtrapolateApproximateFinishTime(carPhysics, isHalfMile, out num);
    }
}
