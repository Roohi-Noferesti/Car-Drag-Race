using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceRewards_WinLose : RaceRewards_WinLoseBase
{
	public bool onePlayerResultsOnly;

	public bool FakeRaceData;

    public TextMeshProUGUI WinLose_WinnerName;
    public TextMeshProUGUI WinLose_WinnerRate;
    public TextMeshProUGUI WinLose_WinnerCarName;
    public TextMeshProUGUI WinLose_WinnerTime;
    public TextMeshProUGUI WinLose_WinnerStar;

	public TextMeshProUGUI WinLose_WinnerArrow_Text;

    public TextMeshProUGUI WinLose_LoserName;
    public TextMeshProUGUI WinLose_LoserRate;
    public TextMeshProUGUI WinLose_LoserCarName;
    public TextMeshProUGUI WinLose_LoserTime;
    public TextMeshProUGUI WinLose_LoserStar;

	public TextMeshProUGUI WinLose_LoserArrow_Text;

    public RawImage WinLose_WinnerCarSprite;

    public RawImage WinLose_LoserCarSprite;

    public RawImage WinLose_WinnerPlayerSprite;

    public RawImage WinLose_LoserPlayerSprite;

    public Image WinLose_WinnerBestTimeStar;

	public TextMeshProUGUI WinLose_WinnerBestTimeText;

    public Image WinLose_LoserBestTimeStar;

	public TextMeshProUGUI WinLose_LoserBestTimeText;

	protected GameObject eliteGameObjectWinner;

	protected GameObject eliteGameObjectLoser;

	public override void Setup(RaceResultsTrackerState resultsData)
	{
	    onePlayerResultsOnly = resultsData.Them == null;
		bool isPlayerWinner;
		float num;
		float num2;
		if (FakeRaceData)
		{
			isPlayerWinner = true;
			num = 11.772f;
			num2 = 11.875f;
		}
		else
		{
			isPlayerWinner = resultsData.You.IsWinner;
			num = resultsData.You.RaceTime;
			num2 = ((!onePlayerResultsOnly) ? resultsData.Them.RaceTime : 0f);
			if (RaceEventInfo.Instance.CurrentEvent.AutoHeadstart)
			{
				num2 += RaceEventInfo.Instance.CurrentEvent.AutoHeadstartTimeDifference();
			}
		}

        string opponentName = RaceEventInfo.Instance.GetRivalName();//RaceEventInfo.Instance.IsCrewRaceEvent || RaceEventInfo.Instance.IsWorldTourRaceHasOverrideDriver()
            //? RaceEventInfo.Instance.GetRivalName() : LocalizationManager.FixRTL_IfNeeded(RaceEventInfo.Instance.GetRivalName());
        //string text2 = PlayerProfileManager.Instance.ActiveProfile.DisplayNameWithYOUFallback().ToUpper();
	    string playerName = PlayerProfileManager.Instance.ActiveProfile.DisplayName;
		if (RaceEventInfo.Instance.IsDailyBattleEvent || RaceEventInfo.Instance.IsWorldTourLoanEvent)
		{
			//CarInfo car = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.LocalPlayerCarDBKey);
            //CommonUI.Instance.CarNameStats.SetShortStatBarText(car.MediumName, car.BaseCarTier, resultsData.You.PerformancePotential);
		}
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
        //if (currentEvent != null && currentEvent.Parent != null && currentEvent.IsBossRace())
        //{
        //    eCarTier carTier = currentEvent.Parent.GetTierEvents().GetCarTier();
        //    //text = LocalizationManager.GetTranslation(Chatter.GetCrewLeaderName(carTier)).ToUpper();
        //}
		CarGarageInstance humanCarGarageInstance = RaceEventInfo.Instance.HumanCarGarageInstance;
        CarGarageInstance aICarGarageInstance = RaceEventInfo.Instance.AICarGarageInstance;
		Texture2D playerSnapShot = null;
		if (humanCarGarageInstance != null)
		{
		    if (PreRaceCarGarageSetup.Instance.HumanSnapShot == null)
		    {
		        ResourceManager.GetCarThumbnail(humanCarGarageInstance.CarDBKey, true, tex =>
		        {
		            playerSnapShot = tex;
		        });
		    }
		    else
		    {
                playerSnapShot = PreRaceCarGarageSetup.Instance.HumanSnapShot;
		    }
            if (isPlayerWinner)
            {
                WinLose_WinnerCarSprite.texture = playerSnapShot;
            }
            else
            {
                WinLose_LoserCarSprite.texture = playerSnapShot;
            }
		}


	    CarInfo playerCarInfo = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.LocalPlayerCarDBKey);
	    CarInfo opponentCarInfo = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.OpponentCarDBKey);
	    var playerCarName = LocalizationManager.GetTranslation(playerCarInfo.ShortName);
	    var opponentCarName = opponentCarInfo != null ? LocalizationManager.GetTranslation(opponentCarInfo.ShortName) : "";
		Texture2D aiSnapShot = null;
	    if (!onePlayerResultsOnly)
	    {
	        if (aICarGarageInstance != null)
	        {
	            if (PreRaceCarGarageSetup.Instance.AiSnapShot == null)
	            {
	                ResourceManager.GetCarThumbnail(aICarGarageInstance.CarDBKey, true, tex =>
	                {
	                    aiSnapShot = tex;
	                });
	            }
	            else
	            {
	                aiSnapShot = PreRaceCarGarageSetup.Instance.AiSnapShot;//!isPlayerWinner
	                    //? PreRaceCarGarageSetup.Instance.AiSnapShot
	                    //: PreRaceCarGarageSetup.Instance.AiSnapShotReverse;
	            }
	            if (!isPlayerWinner)
	            {
	                WinLose_WinnerCarSprite.texture = aiSnapShot;
	            }
	            else
	            {
	                WinLose_LoserCarSprite.texture = aiSnapShot;
	            }
	        }
	    }

        //ImageUtility.LoadImage(PlayerProfileManager.Instance.ActiveProfile.ImageUrl,UserImageSize.Size_100,
        //    (res, tex) =>
        //    {
        //        if (res)
        //        {
        //            if (isPlayerWinner)
        //            {
        //                WinLose_WinnerPlayerSprite.texture = tex;
        //            }
        //            else
        //            {
        //                WinLose_LoserPlayerSprite.texture = tex;
        //            }
        //        }
        //    });

        //ImageUtility.LoadImage(RaceEventInfo.Instance.OpponentImageUrl, UserImageSize.Size_100,
        //    (res, tex) =>
        //    {
        //        if (res)
        //        {
        //            if (isPlayerWinner)
        //            {
        //                WinLose_LoserPlayerSprite.texture = tex;
        //            }
        //            else
        //            {
        //                WinLose_WinnerPlayerSprite.texture = tex;
        //            }
        //        }
        //    });



	 //   Texture2D texture2D3;
		//Texture2D texture2D4;
		//bool eliteCar;
		//bool eliteCar2;
		if (isPlayerWinner)
		{
			//texture2D3 = playerSnapShot;
			//texture2D4 = aiSnapShot;
            //eliteCar = RaceEventInfo.Instance.HumanCarGarageInstance.EliteCar;
            //eliteCar2 = RaceEventInfo.Instance.AICarGarageInstance.EliteCar;
		}
		else
		{
			//texture2D3 = aiSnapShot;
			//texture2D4 = playerSnapShot;
            //eliteCar2 = RaceEventInfo.Instance.HumanCarGarageInstance.EliteCar;
            //eliteCar = RaceEventInfo.Instance.AICarGarageInstance.EliteCar;
		}
		//if (texture2D3 != null && !currentEvent.IsFriendRaceEvent())
		//{
  //          //this.WinLose_WinnerCarSprite.SetTexture(texture2D3);
  //          //this.WinLose_WinnerCarSprite.gameObject.GetComponent<MeshRenderer>().enabled = true;
  //          //this.WinLose_WinnerCarSprite.gameObject.SetActive(true);
		//}
        //if (texture2D4 != null && !currentEvent.IsFriendRaceEvent())
        //{
        //    //this.WinLose_LoserCarSprite.SetTexture(texture2D4);
        //    this.WinLose_LoserCarSprite.gameObject.GetComponent<MeshRenderer>().enabled = true;
        //    this.WinLose_LoserCarSprite.gameObject.SetActive(true);
        //}
        //this.WinLose_WinnerArrow_Text.text = LocalizationManager.GetTranslation("TEXT_RESULTS_WINNER");
        //this.WinLose_LoserArrow_Text.text = LocalizationManager.GetTranslation("TEXT_RESULTS_LOSER");
	    var reward = RaceEventInfo.Instance.CurrentEvent.RaceReward;
	    //var playerProfile = PlayerProfileManager.Instance.ActiveProfile;
	    var leagueName = GameDatabase.Instance.StarDatabase.GetPlayerLeague();
	    var starReward = reward.RaceStarReward.GetLeagueRewardByLeagueName(leagueName);
        WinLose_WinnerStar.text = String.Format("{0:+#;-#}", starReward.WinStar).ToNativeNumber();
		if (isPlayerWinner)
		{
		    
            WinLose_WinnerTime.text = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), num).ToNativeNumber();
            WinLose_LoserTime.text = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), num2).ToNativeNumber();
            WinLose_WinnerName.text = playerName;//string.IsNullOrEmpty(playerName)?playerName:LocalizationManager.FixRTL_IfNeeded(playerName);
            WinLose_WinnerRate.text = RaceResultsTracker.You.PerformancePotential.ToNativeNumber();
		    WinLose_WinnerCarName.text = playerCarName;
		    WinLose_LoserStar.text = String.Format("{0:+#;-#}", starReward.LoseStar).ToNativeNumber();
		    WinLose_LoserName.text = opponentName;
            WinLose_LoserRate.text = currentEvent.IsSMPRaceEvent() ? "?" : RaceResultsTracker.Them.PerformancePotential.ToNativeNumber();
            WinLose_LoserCarName.text = opponentCarName;
            WinLose_WinnerCarSprite.texture = playerSnapShot;
            WinLose_LoserCarSprite.texture = aiSnapShot;

			if (!FakeRaceData)
			{
				//bool flag2 = currentEvent != null && currentEvent.IsHighStakesEvent();
			    bool hideWinnerStar = true;//num3 > num4 || flag2;
				if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
				{
					HideBestTimeStars(true, true);
				}
				else if (RaceEventInfo.Instance.CurrentEvent.AutoHeadstart)
				{
					HideBestTimeStars(true, false);
				}
				else
				{
					HideBestTimeStars(hideWinnerStar, true);
				}
			}
			else
			{
				HideBestTimeStars(false, false);
			}
		}
		else
		{
			if(PlayerProfileManager.Instance.ActiveProfile.PlayerStar <= 0 && PlayerProfileManager.Instance.ActiveProfile.PlayerLeagueStar <= 0)
				WinLose_LoserStar.text = 0.ToString();
			else
				WinLose_LoserStar.text = String.Format("{0:+#;-#}", starReward.LoseStar).ToNativeNumber();
            WinLose_WinnerTime.text = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), num2).ToNativeNumber();
            WinLose_LoserTime.text = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), num).ToNativeNumber();
		    WinLose_WinnerName.text = opponentName;
            WinLose_LoserName.text = playerName;//string.IsNullOrEmpty(playerName)?playerName:LocalizationManager.FixRTL_IfNeeded(playerName);
            WinLose_WinnerRate.text = currentEvent.IsSMPRaceEvent()
		        ? "?"
		        : RaceResultsTracker.Them.PerformancePotential.ToNativeNumber();
            WinLose_WinnerCarName.text = opponentCarName;
		    WinLose_LoserRate.text = RaceResultsTracker.You.PerformancePotential.ToNativeNumber();
            WinLose_LoserCarName.text = playerCarName;
            WinLose_WinnerCarSprite.texture = aiSnapShot;
            WinLose_LoserCarSprite.texture = playerSnapShot;
			if (!FakeRaceData)
			{
				if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
				{
					HideBestTimeStars(true, true);
				}
				else if (RaceEventInfo.Instance.CurrentEvent.AutoHeadstart)
				{
					HideBestTimeStars(false, true);
				}
				else
				{
                    //this.HideBestTimeStars(true, num3 > num4);
				}
			}
			else
			{
				HideBestTimeStars(true, false);
			}
		}
        //this.SetBestTimeText(this.WinLose_WinnerBestTimeText);
        //this.SetBestTimeText(this.WinLose_LoserBestTimeText);
	}

	private void SetBestTimeText(TextMeshProUGUI TextMeshProUGUI)
	{
		if (TextMeshProUGUI.gameObject.activeSelf)
		{
			if (RaceEventInfo.Instance.CurrentEvent.AutoHeadstart)
			{
				TextMeshProUGUI.text = "+" + Mathf.Abs(RaceEventInfo.Instance.CurrentEvent.AutoHeadstartTimeDifference()).ToString("0.00") + "s";
                //TextMeshProUGUI.gameObject.transform.parent.renderer.enabled = false;
			}
			else
			{
                TextMeshProUGUI.text = LocalizationManager.GetTranslation("TEXT_RESULTS_NEW_BEST");
			}
		}
	}

	private void HideBestTimeStars(bool hideWinnerStar, bool hideLoserStar)
	{
        //this.WinLose_WinnerBestTimeStar.gameObject.SetActive(!hideWinnerStar);
        //this.WinLose_WinnerBestTimeText.gameObject.SetActive(!hideWinnerStar);
        //this.WinLose_LoserBestTimeStar.gameObject.SetActive(!hideLoserStar);
        //this.WinLose_LoserBestTimeText.gameObject.SetActive(!hideLoserStar);
	}

	public void OnDestroy()
	{
		WinLose_WinnerName = null;
		WinLose_WinnerTime = null;
		WinLose_LoserName = null;
		WinLose_LoserTime = null;
	}

	public override void Hide(bool hide)
	{
        WinLose_WinnerName.gameObject.SetActive(!hide);
        WinLose_WinnerTime.gameObject.SetActive(!hide);
        WinLose_LoserName.gameObject.SetActive(!hide);
        WinLose_LoserTime.gameObject.SetActive(!hide);
	}
}
