using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class RaceRewards_BonusBlockDefault : RaceRewards_BonusBlock
{
	protected CarGarageInstance carGarageInstance;
	protected int SumBonus = 0;
	public override void Setup(RaceResultsTrackerState resultsData)
	{
		base.Setup(resultsData);
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		int cashReward = currentEvent.RaceReward.GetCashReward();
        int goldPrize = (int)currentEvent.RaceReward.GoldPrize;
		this.SetPlayerWon(resultsData.You.IsWinner);
		bool greatLaunch = resultsData.You.GreatLaunch;
		int numberOfOptimalChanges = resultsData.You.NumberOfOptimalChanges;
		int numberOfGoodChanges = resultsData.You.NumberOfGoodChanges;
		RaceEventTypeMultipliers raceEventTypeRewardMultipliers = GameDatabase.Instance.Currencies.getRaceEventTypeRewardMultipliers(currentEvent);
        var baseTier = RaceEventInfo.Instance.CurrentEventTier;

        if (greatLaunch)
        {
            Bonus_2_PrizeVal = (int)((float)cashReward * raceEventTypeRewardMultipliers.PerfectStartCashMultiplier);
        }
        //this.Bonus_2_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_PERFECT_START");
        Bonus_2_Prize.text = CurrencyUtils.GetCashString(Bonus_2_PrizeVal).ToNativeNumber();
        TotalCash += Bonus_2_PrizeVal;

		this.Bonus_3_PrizeVal += (int)((float)cashReward * raceEventTypeRewardMultipliers.OptimalShiftsCashMultiplier) * numberOfOptimalChanges;
        this.Bonus_3_PrizeVal += (int)((float)cashReward * raceEventTypeRewardMultipliers.GoodShiftsCashMultiplier) * numberOfGoodChanges;
        //this.Bonus_3_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_PERFECT_SHIFTS");
        this.Bonus_3_Prize.text = CurrencyUtils.GetCashString(this.Bonus_3_PrizeVal).ToNativeNumber();
		this.TotalCash += this.Bonus_3_PrizeVal;


        //this.Bonus_4_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_GOOD_SHIFTS");
        //this.Bonus_4_Prize.text = CurrencyUtils.GetShortCashString(this.Bonus_4_PrizeVal);
        //this.TotalCash += this.Bonus_4_PrizeVal;
        carGarageInstance = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        if (currentEvent.IsTestDrive())
        {
            carGarageInstance = RaceEventInfo.Instance.HumanCarGarageInstance;
        }
        //bool flag = carGarageInstance.AppliedLiveryName != null && carGarageInstance.AppliedLiveryName.Length > 0;
        //if (flag && resultsData.You.IsWinner && cashReward > 0)
        //{
        //    this.Bonus_5_PrizeVal = this.DetermineLiveryCashReward(currentEvent, carGarageInstance.AppliedLiveryName);
        //    this.TotalCash += this.Bonus_5_PrizeVal;
        //}

        //this.Bonus_5_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_LEVEL_BONUS");
        //this.Bonus_5_Prize.text = CurrencyUtils.GetShortCashString(this.Bonus_5_PrizeVal);

        //We don't want to minus stake reward from player if he lose , because we do this at the start of game
        var stakePrizeLosed = 0;
        if (currentEvent.IsSMPRaceEvent())
        {
            if (SMPNetworkManager.Instance.SMPRaceInvalidated)
            {
                cashReward = 0;
            }
            else
            {
                var eventOrder = RaceEventInfo.Instance.CurrentEvent.EventOrder;
                var stakeReward = GameDatabase.Instance.Online.GetStake(eventOrder, baseTier);
                if (playerWon)
                {
                    cashReward += stakeReward * 2;
                }
                else
                {
                    stakePrizeLosed = -stakeReward;
                }
            }

        }
		if (this.playerWon)
		{
            //this.Bonus_1_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_RACE_PRIZE");
            this.Bonus_1_Prize.text = CurrencyUtils.GetCashString(cashReward).ToNativeNumber();
			this.Bonus_1_PrizeVal = cashReward;
			this.TotalCash += this.Bonus_1_PrizeVal;
            this.TotalGold += goldPrize;
            if (RemoteConfigABTest.CheckRemoteConfigValue())
            {
	            FindSumOfCarBonuses(carGarageInstance, out SumBonus);
	            Bonus_5_PrizeVal = SumBonus;
	            Bonus_5_Prize.text = CurrencyUtils.GetCashString(Bonus_5_PrizeVal).ToNativeNumber();
	            TotalCash += Bonus_5_PrizeVal; 
            }
            
		}
		else
		{
            //this.Bonus_1_Name.text = LocalizationManager.GetTranslation("TEXT_REWARDS_RACE_BONUS");
		    var loseReward = 0 ;//(cashReward*raceEventTypeRewardMultipliers.RaceLoseCashMultiplier);
            this.Bonus_1_PrizeVal = (int)loseReward;
			this.TotalCash += this.Bonus_1_PrizeVal;
		    var rewardLose = stakePrizeLosed;
            this.Bonus_1_Prize.text = (rewardLose < 0 ? "<color=#FF2121FF>" : "") + CurrencyUtils.GetCashString((int)rewardLose).ToNativeNumber();
		}
		this.TotalXP = GameDatabase.Instance.XPEvents.GetXPPrizeForRaceComplete(this.playerWon);
	    var leagueName = GameDatabase.Instance.StarDatabase.GetPlayerLeague();
	    var starReward = currentEvent.RaceReward.RaceStarReward.GetLeagueRewardByLeagueName(leagueName);
	    this.TotalStar = resultsData.You.IsWinner
            ? starReward.WinStar
            : starReward.LoseStar;
        this.SetTotalCash(this.TotalCash+stakePrizeLosed);
		this.SetBonusGold(this.TotalGold);
	}

	public override void SetPlayerWon(bool currentEventWon)
	{
		this.playerWon = ((!RelayManager.IsCurrentEventRelay()) ? currentEventWon : RelayManager.HumanWon());
	}

	protected void FindSumOfCarBonuses(CarGarageInstance carGarageInstance,out int virtualItemType)
	{
		List<VirtualItemType> items = new List<VirtualItemType>();
		List<int> liveryBonusReward = new List<int>();
		items.Add(VirtualItemType.BodyShader);
		items.Add(VirtualItemType.RingShader);
		items.Add(VirtualItemType.HeadLighShader);
		items.Add(VirtualItemType.CarSpoiler);
		items.Add(VirtualItemType.CarSticker);
		foreach (var item in items)
		{
			var correctItemID = GetLiveryBonusForItemType(carGarageInstance, item);
			if (correctItemID != null)
			{
				liveryBonusReward.Add(DetermineLiveryCashReward(RaceEventInfo.Instance.CurrentEvent, correctItemID));

			}
		}
		
		virtualItemType = liveryBonusReward.Count != 0 ? (int)liveryBonusReward.Sum() : 0;
		

	}
	public static string GetLiveryBonusForItemType(CarGarageInstance carGarageInstance, VirtualItemType Item)
	{
		var item = carGarageInstance.EquipItemCollection.GetEquipedItem(Item,carGarageInstance.CarDBKey);
		var correctItemID = item != null ? carGarageInstance.GetCorrectItemID(item.VirtualItemID, false) : null;
		return correctItemID;
	}
	private int DetermineLiveryCashReward(RaceEventData myEvent, string liveryName = null)
	{
	    string zAssetBundleID = (liveryName == null) ? PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().AppliedLiveryName : liveryName;
        int goldCostFromAssetID = PaintScreen.GetGoldCostFromAssetID(zAssetBundleID);
        int baseLiveryBonus = 0;
        float liveryGoldMultiplier = 0f;
        float liveryTierMultiplier = 0f;
        float customizationTypeMultiplier = 0f;
        if (myEvent.Parent != null)
        {
	        if (goldCostFromAssetID > 0)
	        {
		        GameDatabase.Instance.Currencies.getLiveryBonusAmounts(PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().Tier, zAssetBundleID, out baseLiveryBonus, out liveryGoldMultiplier, out liveryTierMultiplier, out customizationTypeMultiplier);
	        }
        }
        if (PlayerProfileManager.Instance.ActiveProfile.ShouldUseOldLiveryCalc)
        {
            return Mathf.CeilToInt(((float)baseLiveryBonus + liveryGoldMultiplier * (float)goldCostFromAssetID) * liveryTierMultiplier);
        }
        int num4 = 0;
        if (goldCostFromAssetID > 0)
        {
            num4 = Mathf.CeilToInt(Mathf.Log((float)goldCostFromAssetID, 2.71828f));
        }
        return Mathf.CeilToInt(((float)baseLiveryBonus + liveryGoldMultiplier * (float)num4) * liveryTierMultiplier * customizationTypeMultiplier);
	}

    public override void Anim_Totals_AddBonus1()
	{
		base.PlayAudioStarTally(this.Bonus_1_PrizeVal);
        //Bonus_1.GetComponent<Animator>().Play("play");
        //AnimationUtils.PlayAnim(this.Bonus_1_Prize.transform.parent.animation, "Rewards_Bonus_ItemCashIn");
		this._currentTotalValue += this.Bonus_1_PrizeVal;
		this.SetTotalCash(this._currentTotalValue);
        //AnimationUtils.PlayAnim(this.TotalAddedText.animation, "Rewards_Bonus_TotalStart");
	}

	public override void Anim_Totals_AddBonus2()
	{
		base.PlayAudioStarTally(this.Bonus_2_PrizeVal);
		if (this.Bonus_2_PrizeVal == 0 && this.SkipAllZeroBonuses)
		{
			return;
		}
        //Bonus_2.GetComponent<Animator>().Play("play");
        //AnimationUtils.PlayAnim(this.Bonus_2_Prize.transform.parent.animation, "Rewards_Bonus_ItemCashIn");
		this._currentTotalValue += this.Bonus_2_PrizeVal;
		this.SetTotalCash(this._currentTotalValue);
        //AnimationUtils.PlayAnim(this.TotalAddedText.animation, "Rewards_Bonus_TotalAdd");
	}

	public override void Anim_Totals_AddBonus3()
	{
		base.PlayAudioStarTally(this.Bonus_3_PrizeVal);
		if (this.Bonus_3_PrizeVal == 0 && this.SkipAllZeroBonuses)
		{
			return;
		}
        //Bonus_3.GetComponent<Animator>().Play("play");
        //AnimationUtils.PlayAnim(this.Bonus_3_Prize.transform.parent.animation, "Rewards_Bonus_ItemCashIn");
		this._currentTotalValue += this.Bonus_3_PrizeVal;
		this.SetTotalCash(this._currentTotalValue);
        //AnimationUtils.PlayAnim(this.TotalAddedText.animation, "Rewards_Bonus_TotalAdd");
	}

	public override void Anim_Totals_AddBonus4()
	{
		base.PlayAudioStarTally(this.Bonus_4_PrizeVal);
		if (this.Bonus_4_PrizeVal == 0 && this.SkipAllZeroBonuses)
		{
			return;
		}
        //Bonus_4.GetComponent<Animator>().Play("play");
        //AnimationUtils.PlayAnim(this.Bonus_4_Prize.transform.parent.animation, "Rewards_Bonus_ItemCashIn");
		this._currentTotalValue += this.Bonus_4_PrizeVal;
		this.SetTotalCash(this._currentTotalValue);
        //AnimationUtils.PlayAnim(this.TotalAddedText.animation, "Rewards_Bonus_TotalAdd");
		this.SetEvo();
	}

	public override void Anim_Totals_AddBonus5()
	{
		base.PlayAudioStarTally(this.Bonus_5_PrizeVal);
		if (this.Bonus_5_PrizeVal == 0 && this.SkipAllZeroBonuses)
		{
			return;
		}
        //AnimationUtils.PlayAnim(this.Bonus_5_Prize.transform.parent.animation, "Rewards_Bonus_ItemCashIn");
		this._currentTotalValue += this.Bonus_5_PrizeVal;
		this.SetTotalCash(this._currentTotalValue);
        //AnimationUtils.PlayAnim(this.TotalAddedText.animation, "Rewards_Bonus_TotalAdd");
	}

	public override void Anim_Totals_Final()
	{
	    base.Anim_Totals_Final();
        //AudioManager.Instance.PlaySound("Reward_Totals", null);
        //AudioManager.Instance.PlaySound("Reward_ResultTable", null);
        //AnimationUtils.PlayAnim(this.TotalAddedText.animation, "Rewards_Bonus_TotalEnd");
		if (this.TotalGold > 0)
		{
			this.SetBonusGold(this.TotalGold);
            //AnimationUtils.PlayAnim(this.TotalBonusText.animation, "Rewards_Bonus_TotalGoldEnd");
		}
	}

    private int CalculateStakeRewardLose()
    {
        var currentEvent = RaceEventInfo.Instance.CurrentEvent;
        var baseTier = RaceEventInfo.Instance.CurrentEventTier;
        if (currentEvent.IsSMPRaceEvent())
        {
            if (!SMPNetworkManager.Instance.SMPRaceInvalidated)
            {
                var eventOrder = RaceEventInfo.Instance.CurrentEvent.EventOrder;
                var stakeReward = GameDatabase.Instance.Online.GetStake(eventOrder, baseTier);
                if (!playerWon)
                {
                    return -stakeReward;
                }
            }
        }
        return 0;
    }

	public override void Finish()
	{
	    var stakeLose = CalculateStakeRewardLose();
        this.SetTotalCash(this.TotalCash + stakeLose);
		this.SetBonusGold(this.TotalGold);
		this.SetEvo();
	}

	protected void SetBonusGold(int total)
	{
        if (total <= 0)
        {
            //this.TotalBonusText.gameObject.SetActive(false);
            return;
        }
        //this.TotalBonusText.gameObject.SetActive(true);
        this.Bonus_Gold.SetActive(true);
        this.TotalBonusText.text = CurrencyUtils.GetGoldStringWithIcon(total);
	}

	protected void SetEvo()
	{
        //string currentlySelectedCarDBKey = PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey;
        //if (this.playerWon && CarDatabase.Instance.GetCar(currentlySelectedCarDBKey).UsesEvoUpgrades())
        //{
        //    PinDetail worldTourPinPinDetail = RaceEventInfo.Instance.CurrentEvent.GetWorldTourPinPinDetail();
        //    if (worldTourPinPinDetail != null && worldTourPinPinDetail.WorldTourScheduledPinInfo != null && CarDatabase.Instance.GetCar(currentlySelectedCarDBKey).EvolutionSequences.Contains(worldTourPinPinDetail.WorldTourScheduledPinInfo.GetRootParentSequence().ID))
        //    {
        //        this.TotalEvoToken.gameObject.SetActive(true);
        //    }
        //}
	}

	protected void SetTotalCash(int total)
	{
        this.TotalAddedText.text = (total < 0 ? "<color=#FF2121FF>" : "") + CurrencyUtils.GetCashString(total).ToNativeNumber();
        //int length = this.TotalAddedText.text.Length;
        //UnityEngine.Vector3 localPosition = this.TotalAddedGlowPos.localPosition;
        //localPosition.x = -0.08f * (float)length;
        //this.TotalAddedGlowPos.localPosition = localPosition;
        //localPosition = this.TotalAddedFlarePos.localPosition;
        //localPosition.x = -0.08f * (float)length;
        //this.TotalAddedFlarePos.localPosition = localPosition;
	}
}
