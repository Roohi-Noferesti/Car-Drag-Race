using System;

public class RaceResultsData : ICloneable
{
	public bool IsWinner;

	public float RaceTime;

	public float PrevBestForFriendsCar;

	public float TopRaceSpeedMPH;

	public float[] SpeedMileStones;

	public float SpeedWhenCrossingFinishLine;

	public float Nought60Time;

    public float Nought100TimeKPH;

	public float Nought100Time;

	public float WheelSpinDistance;

	public int NumberOfChanges;

	public int NumberOfEarlyChanges;

	public int NumberOfGoodChanges;

	public int NumberOfOptimalChanges;

	public int NumberOfLateChanges;

	public bool GreatLaunch;

	public bool UsedNitrous;

	public bool HadBoostNitrousAvailable;

	public eCarTier CarTierEnum;

	public int PerformancePotential;

	public bool TimeWasExtrapolated;

	public int EventID;

	public object Clone()
	{
		RaceResultsData raceResultsData = base.MemberwiseClone() as RaceResultsData;
		if (raceResultsData.SpeedMileStones != null)
		{
			raceResultsData.SpeedMileStones = (raceResultsData.SpeedMileStones.Clone() as float[]);
		}
		return raceResultsData;
	}
}
