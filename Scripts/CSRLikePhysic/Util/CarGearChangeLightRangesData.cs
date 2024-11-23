using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class CarGearChangeLightRangesData:ScriptableObject
{
	public float LaunchWindowLeadupRPM = 2700f;

	public float GoodLaunchRPMWindow = 900f;

	public float LaunchPerfectBandFraction = 0.5f;

	public float LeadupToGearChangeWindow = 900f;

	public float GoodGearChangeBand = 300f;

	public float PerfectGearChangeBandFraction = 0.5f;

	public float[] OkayLaunchBandByTier;

	public float MinSlowStartWindow;

	public CarGearChangeLightRangesData()
	{
		float[] expr_49 = new float[6];
		expr_49[0] = 2800f;
		expr_49[1] = 1800f;
		expr_49[2] = 800f;
		this.OkayLaunchBandByTier = expr_49;
		this.MinSlowStartWindow = 500f;
	}

    public static void GetTotalGearLightRPMRange(out float launchWindowLeadupBand, out float goodLaunchBand,
        out float launchPerfectWindowFraction, out float leadUpToGearChangeBand, out float goodGearChangeBand,
        out float perfectGearChangeBandFraction)
    {
        CarGearChangeLightRangesData carGearLightData;
#if UNITY_EDITOR
        carGearLightData = AssetDatabase.LoadAssetAtPath<CarsConfiguration>("Assets/configuration/CarsConfiguration.asset").CarGearLightData;
#else
        carGearLightData = GameDatabase.Instance.CarsConfiguration.CarGearLightData;
#endif
	    if (carGearLightData == null)
	    {
		    launchWindowLeadupBand = 0;
		    goodLaunchBand = 0;
		    launchPerfectWindowFraction = 0;
		    leadUpToGearChangeBand = 0;
		    goodGearChangeBand = 0;
		    perfectGearChangeBandFraction = 0;
		    GTDebug.Log(GTLogChannel.Other,"error on getting okayLaunchBand because carGearLightData is null");
	    }
	    else
	    {
		    launchWindowLeadupBand = carGearLightData.LaunchWindowLeadupRPM;
		    goodLaunchBand = carGearLightData.GoodLaunchRPMWindow;
		    launchPerfectWindowFraction = carGearLightData.LaunchPerfectBandFraction;
		    leadUpToGearChangeBand = carGearLightData.LeadupToGearChangeWindow;
		    goodGearChangeBand = carGearLightData.GoodGearChangeBand;
		    perfectGearChangeBandFraction = carGearLightData.PerfectGearChangeBandFraction;
	    }
    }

    public static void GetTotalGearLightRPMRange(out float launchWindowLeadupBand, out float goodLaunchBand, out float launchPerfectWindowFraction, out float leadUpToGearChangeBand, out float goodGearChangeBand, out float perfectGearChangeBandFraction, out float okayLaunchBand, out float minSlowStart, eCarTier carTier)
	{
	    CarGearChangeLightRangesData carGearLightData;
#if UNITY_EDITOR
        carGearLightData = AssetDatabase.LoadAssetAtPath<CarsConfiguration>("Assets/configuration/CarsConfiguration.asset").CarGearLightData;
#else
        carGearLightData = GameDatabase.Instance.CarsConfiguration.CarGearLightData;
#endif
		GetTotalGearLightRPMRange(out launchWindowLeadupBand, out goodLaunchBand, out launchPerfectWindowFraction, out leadUpToGearChangeBand, out goodGearChangeBand, out perfectGearChangeBandFraction);
		if (carGearLightData == null)
		{
			okayLaunchBand = 1000;
			minSlowStart = 500;
			GTDebug.Log(GTLogChannel.Other,"error on getting okayLaunchBand because carGearLightData is null");
		}
		else
		{
			int tierForOKStart = (int)carGearLightData.GetTierForOKStart(carTier);
			okayLaunchBand = carGearLightData.OkayLaunchBandByTier[tierForOKStart];
			minSlowStart = carGearLightData.MinSlowStartWindow;
		}
	}

	public bool ShouldShowOKStart(eCarTier carTier)
	{
		int tierForOKStart = (int)this.GetTierForOKStart(carTier);
		return this.OkayLaunchBandByTier[tierForOKStart] > 0f;
	}

	public bool ShouldShowStartRevGuide(eCarTier carTier)
	{
        bool flag = RaceEventInfo.Instance.IsDailyBattleEvent || PlayerProfileManager.Instance.ActiveProfile.DailyBattlesLastEventAt > DateTime.MinValue;
        return !flag;
	}

	private eCarTier GetTierForOKStart(eCarTier carTier)
	{
        if(!Application.isPlaying)
            return carTier;
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (RaceEventInfo.Instance.IsDailyBattleEvent || RaceEventInfo.Instance.CurrentEvent == GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial || RaceEventInfo.Instance.CurrentEvent == GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial2)
        {
            IEnumerator enumerator = Enum.GetValues(typeof(eCarTier)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    eCarTier eCarTier = (eCarTier)((int)enumerator.Current);
                    if (eCarTier == eCarTier.TIER_5)
                    {
                        eCarTier result = eCarTier;
                        return result;
                    }
                    if (activeProfile.GetBossChallengeState(eCarTier) != BossChallengeStateEnum.FINISHED)
                    {
                        eCarTier result = eCarTier;
                        return result;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return carTier;
        }
        return carTier;
	}
}
