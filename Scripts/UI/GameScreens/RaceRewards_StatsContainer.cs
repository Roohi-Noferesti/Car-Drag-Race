using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceRewards_StatsContainer : MonoBehaviour
{
	public TextMeshProUGUI Stats_ColumnOne_FinishSpeed;

	public TextMeshProUGUI[] Stats_ColumnOne_0to60;

	public TextMeshProUGUI Stats_ColumnOne_0to100;

	public CarStatsElem Stats_ColumnTwo_CarStats;

	public TextMeshProUGUI Stats_ColumnTwo_Name;

	public TextMeshProUGUI Stats_ColumnTwo_FinishSpeed;

	public TextMeshProUGUI Stats_ColumnTwo_0to60;

	public TextMeshProUGUI Stats_ColumnTwo_0to100;

	public Image Stats_ColumnTwo_NewFinishSpeed;

	public Image Stats_ColumnTwo_New0to60;

	public Image Stats_ColumnTwo_New0to100;

	public CarStatsElem Stats_ColumnThree_CarStats;

	public TextMeshProUGUI Stats_ColumnThree_Name;

	public TextMeshProUGUI Stats_ColumnThree_FinishSpeed;

	public TextMeshProUGUI Stats_ColumnThree_0to60;

	public TextMeshProUGUI Stats_ColumnThree_0to100;

	public virtual void Setup(RaceResultsTrackerState shownResults, bool useFakeRaceData)
	{
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        //eCarTier zClass = eCarTier.MAX_CAR_TIERS;
        //int num = 0;
        float opponentFinishSpeed = 0f;
        float opponentNought60 = 0f;
		//float num4 = 0f;
		//eCarTier zClass2;
		//int zPerformanceIndex;
        float playerFinishSpeed;
        float playerNought60;
		//float num7;
		if (useFakeRaceData)
		{
			//zClass2 = eCarTier.TIER_2;
			//zPerformanceIndex = 221;
            playerFinishSpeed = 189.12f;
            playerNought60 = 7.321f;
			//num7 = 0f;
			//zClass = eCarTier.TIER_5;
			//num = 691;
            opponentFinishSpeed = 173.1f;
            opponentNought60 = 9.321f;
			//num4 = 12.334f;
		}
        else
        {
            if (RaceEventInfo.Instance.IsDailyBattleEvent || RaceEventInfo.Instance.IsWorldTourLoanEvent)
            {
                //zClass2 = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.LocalPlayerCarDBKey).BaseCarTier;
            }
            else
            {
                //zClass2 = shownResults.You.CarTierEnum;
            }


            //zPerformanceIndex = shownResults.You.PerformancePotential;
            playerFinishSpeed = shownResults.You.SpeedWhenCrossingFinishLine;
            if (!activeProfile.UseMileAsUnit)
            {
                playerFinishSpeed *= 1.609344F;
            }

            playerNought60 = shownResults.You.Nought100TimeKPH;
            //num7 = shownResults.You.Nought100Time;
            if (shownResults.Them != null)
            {
                //zClass = shownResults.Them.CarTierEnum;
                //num = shownResults.Them.PerformancePotential;
                opponentFinishSpeed = shownResults.Them.SpeedWhenCrossingFinishLine;
                if (!activeProfile.UseMileAsUnit)
                {
                    opponentFinishSpeed *= 1.609344F;
                }

                opponentNought60 = shownResults.Them.Nought100TimeKPH;
                //num4 = shownResults.Them.Nought100Time;
            }
        }

        //this.Stats_ColumnOne_FinishSpeed.text = LocalizationManager.GetTranslation("TEXT_RESULTS_FINISH_SPEED");
        foreach (var textMeshProUgui in Stats_ColumnOne_0to60)
        {
            if (activeProfile.UseMileAsUnit)
                textMeshProUgui.text = LocalizationManager.GetTranslation("TEXT_RACE_REWARD_NOUGHT_60");
            else
            {
                textMeshProUgui.text = LocalizationManager.GetTranslation("TEXT_RACE_REWARD_NOUGHT_100");
            }
        }

        //this.Stats_ColumnOne_0to100.text = LocalizationManager.GetTranslation("TEXT_RESULTS_TIME_100");
        ////this.Stats_ColumnTwo_CarStats.Set(zClass2, zPerformanceIndex);
        //this.Stats_ColumnTwo_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_YOU");
        var finishSpeedText = "TEXT_UNITS_SPEED_WITH_MPH";
        if (!activeProfile.UseMileAsUnit)
        {
            finishSpeedText += "_IN_METER";
        }
        this.Stats_ColumnTwo_FinishSpeed.text = string.Format(LocalizationManager.GetTranslation(finishSpeedText), playerFinishSpeed.ToString("0.##"));
        this.Stats_ColumnTwo_0to60.text = ((playerNought60 <= 0f) ? "-" : string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), playerNought60));
        //this.Stats_ColumnTwo_0to100.text = ((num7 <= 0f) ? "-" : string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), num7));
		if (shownResults.Them != null)
		{
            //if (RaceEventInfo.Instance.CurrentEvent.AutoDifficulty)
            //{
            //    num = Mathf.Min(num, RaceEventInfo.Instance.CurrentEvent.MaxPerformancePotentialIndex);
            //}
            ////this.Stats_ColumnThree_CarStats.Set(zClass, num);
            //this.Stats_ColumnThree_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_RIVAL");
            this.Stats_ColumnThree_FinishSpeed.text = string.Format(LocalizationManager.GetTranslation(finishSpeedText), opponentFinishSpeed.ToString("0.##"));
            this.Stats_ColumnThree_0to60.text = ((opponentNought60 <= 0f) ? "-" : string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), opponentNought60));
            //this.Stats_ColumnThree_0to100.text = ((num4 <= 0f) ? "-" : string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), num4));
		}
        //if (shownResults.Best != null)
        //{
        //    this.Stats_ColumnTwo_NewFinishSpeed.gameObject.SetActive((num5 >= shownResults.Best.SpeedWhenCrossingFinishLine));
        //    this.Stats_ColumnTwo_New0to60.gameObject.SetActive( (num6 <= shownResults.Best.Nought60Time && shownResults.Best.Nought60Time > 0f && num6 > 0f));
        //    this.Stats_ColumnTwo_New0to100.gameObject.SetActive((num7 <= shownResults.Best.Nought100Time && shownResults.Best.Nought100Time > 0f && num7 > 0f));
        //}
        //else
        //{
        //    this.Stats_ColumnTwo_NewFinishSpeed.gameObject.SetActive(false);
        //    this.Stats_ColumnTwo_New0to60.gameObject.SetActive( false);
        //    this.Stats_ColumnTwo_New0to100.gameObject.SetActive( false);
        //}
        //if (this.Stats_ColumnTwo_NewFinishSpeed.gameObject.activeSelf)
        //{
        //    this.PositionStatsNewBestStar(this.Stats_ColumnTwo_FinishSpeed, this.Stats_ColumnTwo_NewFinishSpeed.transform);
        //}
        //if (this.Stats_ColumnTwo_New0to60.gameObject.activeSelf)
        //{
        //    this.PositionStatsNewBestStar(this.Stats_ColumnTwo_0to60, this.Stats_ColumnTwo_New0to60.transform);
        //}
        //if (this.Stats_ColumnTwo_New0to100.gameObject.activeSelf)
        //{
        //    this.PositionStatsNewBestStar(this.Stats_ColumnTwo_0to100, this.Stats_ColumnTwo_New0to100.transform);
        //}
	}

	private void PositionStatsNewBestStar(TextMeshProUGUI text, Transform star)
	{
        //Vector3 localPosition = star.localPosition;
        //localPosition.x = text.transform.localPosition.x + text.TotalWidth / 2f + 0.06f;
        //star.localPosition = localPosition;
	}
}
