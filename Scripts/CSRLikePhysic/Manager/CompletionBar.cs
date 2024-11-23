using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletionBar : MonoBehaviour
{
	private const float FlashTime = 1.5f;

    public TextMeshProUGUI Text;

    public TextMeshProUGUI RewardText;

	public Slider ProgressBar;

	private string Text1 = string.Empty;

	private string Text2 = string.Empty;

	private bool isShowingText1 = true;

	private bool flashText;

	private float TimerVal;

	private void Reset()
	{
		this.isShowingText1 = true;
		this.flashText = false;
		this.TimerVal = 0f;
	}

	public void Disable()
	{
        RewardText.gameObject.SetActive(false);
		this.SetupBar(this.Text.text, ProgressBarStyle.None, default(Fraction));
	}

    public void SetupForTitle(RaceEventData zData)
    {
        RewardText.gameObject.SetActive(false);
        this.Text.gameObject.SetActive(true);
        this.Text.text = zData.Parent.GetProgressBarText(zData);
        this.SetupBar(this.Text.text, ProgressBarStyle.None, default(Fraction));
    }

	public void Setup(RaceEventData zData, ProgressBarStyle style = ProgressBarStyle.Bar, Fraction progress = default(Fraction))
	{
        this.Text.gameObject.SetActive(true);
        this.RewardText.gameObject.SetActive(true);
        this.Text.text = zData.Parent.GetProgressBarText(zData);
	    this.RewardText.text = this.GetRewardString(zData);
        this.SetupBar(this.Text.text, style, progress);
		this.Reset();
	}


    private string GetRewardString(RaceEventData raceData)
    {
        int cash;
        int gold = 0;
        if (raceData.IsDailyBattle())
        {
            int num = PlayerProfileManager.Instance.ActiveProfile.DailyBattlesConsecutiveDaysCount;
            if (PlayerProfileManager.Instance.ActiveProfile.GetDaysSinceLastDailyBattle() == 1 || PlayerProfileManager.Instance.ActiveProfile.GetIsNextDailyBattleAfterMidnight())
            {
                num++;
            }
            DailyBattleReward reward = DailyBattleRewardManager.Instance.GetReward(num, RaceEventQuery.Instance.getHighestUnlockedClass(), true);
            gold = ((reward.RewardType != DailyBattleRewardType.Gold) ? 0 : reward.RewardValue);
            cash = ((reward.RewardType != DailyBattleRewardType.Cash) ? 0 : reward.RewardValue);
        }
        else if (raceData.IsRegulationRace())
        {
            var tierEvent = raceData.GetTierEvent();
                if (tierEvent.GetCarTier() < eCarTier.TIER_2)
                    cash = raceData.Group.RaceEvents[0].RaceReward.GetCashReward();
                else
                {

                    var cash1 = raceData.Group.RaceEvents[0].RaceReward.GetCashReward();
                    var cash2 = raceData.Group.RaceEvents[1].RaceReward.GetCashReward();
                    var cash3 = raceData.Group.RaceEvents[2].RaceReward.GetCashReward();
                    cash = (cash1 + cash2 + cash3) / 3;
                }

                //var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            //var carTier = CarDatabase.Instance.GetCar(activeProfile.GetCurrentCar().CarDBKey).BaseCarTier;
            //var baseCarTier = raceData.GetTierEvent().GetCarTier();
            //var basePPindex = CarDatabase.Instance.GetCar(activeProfile.GetCurrentCar().CarDBKey).PPIndex;
            //cash = GameDatabase.Instance.Currencies.GetcashRewardByPPIndexAndDifficulty(basePPindex, (AutoDifficulty.DifficultyRating) EventPane.RaceGroupSelected, carTier, baseCarTier);//   EventGroup.RaceEvents[0].RaceReward.CashPrize;
        }
        else
        {
            cash = raceData.RaceReward.GetCashReward();
            gold = raceData.RaceReward.GoldPrize;
        }


        if (cash > 0)
        {
            return CurrencyUtils.GetCashString(cash);
        }
        else if (gold > 0)
        {
            return CurrencyUtils.GetGoldStringWithIcon(gold);
        }
        return String.Empty;
    }

    public void Setup(string zName, string zNameAlt, bool shouldUse, Fraction progress = default(Fraction))
	{
		this.SetupProgressBar(ProgressBarStyle.None);
        this.Text.gameObject.SetActive(true);
		this.Text1 = zName;
        this.Text.text = this.Text1;
		this.Text2 = zNameAlt;
		this.flashText = shouldUse;
		this.SetupBar((this.Text2.Length <= this.Text1.Length || !shouldUse) ? this.Text1 : this.Text2, ProgressBarStyle.None, default(Fraction));
		this.Reset();
	}

	public void SetupTierX(string zName,RaceEventData eventData, ProgressBarStyle style = ProgressBarStyle.Bar, Fraction progress = default(Fraction))
	{
		this.Text1 = zName;
        this.Text.text = this.Text1;
        this.SetupBar(this.Text.text, style, progress);
	    if (eventData != null)
	    {
	        this.RewardText.text = this.GetRewardString(eventData);
            this.RewardText.gameObject.SetActive(true);
        }
	    else if(this.RewardText!=null)
	    {
	        this.RewardText.gameObject.SetActive(false);
	    }

	    this.Reset();
	}

	private void SetupProgressBar(ProgressBarStyle style)
	{
	    if (this.ProgressBar != null)
	        this.ProgressBar.gameObject.SetActive(style == ProgressBarStyle.Bar);
	    //this.PipProgressBar.gameObject.SetActive(style == ProgressBarStyle.Pip);
	}

	private void SetupBar(string text, ProgressBarStyle style, Fraction progress)
	{
        //List<string> list = text.Split(new char[]
        //{
        //    '\n'
        //}).ToList<string>();
        //int num = list.Count + ((style != ProgressBarStyle.Pip) ? 0 : 1);
        //float a = list.MaxValue((string t) => this.Text.GetWidth(t));
        //this.BarWidth = Mathf.Max(a, this.DefaultBarWidth);
        //if (style == ProgressBarStyle.Pip)
        //{
        //    this.BarWidth = Mathf.Max(this.BarWidth, this.PipProgressBar.GetDefaultWidth(progress.Denominator));
        //}
        //float height = this.DefaultBarHeight * (float)Mathf.Max(1, num);
        //float yOffset = (float)(-(float)(num - 1)) * this.DefaultBarHeight / 2f;
        //bool flag = list.Count == 2 || style == ProgressBarStyle.Pip;
        //this.SingleBar.SetActive(!flag);
        //this.DoubleBar.SetActive(flag);
        //float y = (!flag) ? -0.059f : -0.178f;
        //if (flag)
        //{
        //    this.SetPanelSizes(this.DoubleLeft, this.DoubleMiddle, this.DoubleRight, this.BarWidth, yOffset, height);
        //}
        //else
        //{
        //    this.SetPanelSizes(this.Left, this.Middle, this.Right, this.BarWidth, yOffset, height);
        //}
        //this.ProgressBar.transform.localPosition = new Vector3(0f, y, 0.01f);
        //this.PipProgressBar.Width = this.BarWidth + this.Left.width + this.Right.width;
        //base.gameObject.transform.localScale = Vector3.one;

		this.SetupProgressBar(style);
		this.UpdateProgressionBar(progress);
	}

	public void SetAlpha(float alpha)
	{
		if (this.Text != null)
		{
			Color color = this.Text.color;
            this.Text.color = new Color(color.r,color.g,color.b,alpha);
		}
	}

	private void UpdateProgressionBar(Fraction progress)
	{
		this.UpdatePercentageBar(progress.ToPercent());
		this.UpdatePipBar(progress.Numerator, progress.Denominator);
	}

	private void UpdatePipBar(int progress, int count)
	{
        //this.PipProgressBar.UpdateProgress(progress, count);
	}

	private void UpdatePercentageBar(float zPercentageComplete)
	{
		float value = (zPercentageComplete + 0.07f) / 1.14f;
		if (zPercentageComplete <= 0f)
		{
			value = 0f;
		}
		if (zPercentageComplete >= 1f)
		{
			value = 1f;
		}
	    if (ProgressBar != null)
	        this.ProgressBar.value = value;
	    //this.ProgressBar.renderer.material.SetFloat("_Progress", value);
	}

	public void SetFlashText(bool setTo)
	{
		this.flashText = setTo;
		if (!setTo)
		{
			this.Text.text = this.Text1;
		}
	}

	private void Update()
	{
		if (this.flashText)
		{
			this.TimerVal += Time.deltaTime;
			if (this.TimerVal >= 1.5f)
			{
				if (this.isShowingText1)
				{
                    this.Text.text = this.Text2;
				}
				else
				{
                    this.Text.text = this.Text1;
				}
				this.isShowingText1 = !this.isShowingText1;
				this.TimerVal = 0f;
			}
		}
	}

    public void Hide()
    {
        this.Text.gameObject.SetActive(false);
        this.RewardText.gameObject.SetActive(false);
        if (this.ProgressBar != null)
            this.ProgressBar.gameObject.SetActive(false);
    }

}
