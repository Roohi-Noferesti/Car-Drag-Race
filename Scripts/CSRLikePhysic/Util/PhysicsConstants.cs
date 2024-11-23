public class PhysicsConstants
{
	public const float METRES_SEC_TO_MPH = 2.236f;

	public const float METRES_SEC_TO_KPH = 3.57760024f;

	public const float PI = 3.14159f;

	public const float GRAVITY = 9.81f;

	public const float AIR_DENSITY = 1.2f;

	public const float MPH_TO_METRES_SEC = 0.44722718f;

	public const float MILE_DISTANCE = 1609.3f;

	public const float QUARTER_MILE_DISTANCE = 402.325f;

	public const float HALF_MILE_DISTANCE = 804.65f;

	public const float EIGHTH_MILE_DISTANCE = 201.1625f;

	public const float KILOMETRE_DISTANCE = 1000f;

	public const float NEWTON_METRES_TO_FOOT_POUNDS = 0.7375621f;

	public const float FOOT_POUNDS_TO_NEWTON_METRES = 1.355818f;

	public const float TORQUE_HORSEPOWER_CONVERSION = 5252f;

	public const float RECIPROCAL_TORQUE_HORSEPOWER_CONVERSION = 0.000190403662f;

	public const float INCHES_TO_METRES = 0.0254f;

	public const float METRES_TO_INCHES = 39.37008f;

	public const float KGS_TO_POUNDS = 2.20462251f;

	public const float POUNDS_TO_KGS = 0.4535924f;

	public static float WheelLinearSpeedFromRPM(float zWheelRadius, float zRPM)
	{
		return zWheelRadius * zRPM * 2f * 3.14159f / 60f;
	}

	public static float WheelRPMFromLinearSpeed(float zWheelRadius, float zLinearSpeed)
	{
		return zLinearSpeed * 60f / (zWheelRadius * 2f * 3.14159f);
	}
}
