using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyBattleCalenderScreen : ZHUDScreen
{
	private enum State
	{
		ANIMATE_IN,
		INTERACTIVE
	}

	public float RewardOffset;

	public float ItemWidthSelected;

	public float ItemWidthUnselected;

	public Button ContinueButton;

    public GridLayoutGroup LayoutGroup;

	public Transform RewardListParent;

	public Transform StraplinePanel;

	public TextMeshProUGUI StraplineText;

	public float StraplineAnimSpeed;

	public float ItemFadeInterval;

	public float AnimInTime;

	public int DaysAheadToDisplay;

	public int DaysAheadToFocus;

	public AnimationCurve AnimInCurve;

	public string StraplineLocString;

	public string YesterdayLocString;

	public string TodayLocString;

	public string TomorrowLocString;

	public string DayLocString;

	public string ScreenAppearAudioEvent;

	public string HighlightPrizeAudioEvent;

    private Dictionary<int, DailyBattleRewardContainer> RewardContainersbyDay =
        new Dictionary<int, DailyBattleRewardContainer>();

	private static int CurrentDay = 4;

	private static int HighlightedDay = 3;

	private float StraplineAnimTargetX;

	private float RewardListFinalPosition;

	private float RewardListAnimPosition;

	private float StateTimer;

	private State DailyBattleCurrentState;

    public ScrollRect ScrollRect;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.DailyBattleCalender;
        }
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        ContinueButton.interactable = false;
        //this.StraplineText.text = LocalizationManager.GetTranslation(this.StraplineLocString);
		var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		CurrentDay = activeProfile.DailyBattlesConsecutiveDaysCount - 1;
		HighlightedDay = CurrentDay;
		var highestUnlockedClass = RaceEventQuery.Instance.getHighestUnlockedClass();
		var consecutiveDays = Math.Max(1, HighlightedDay);
        var rewardSequence =
            DailyBattleRewardManager.Instance.Sequence(consecutiveDays, highestUnlockedClass, true)
                .Take(this.DaysAheadToDisplay + 2)
                .ToList();
		var i = -1;
		var day = HighlightedDay - 1;
		var num3 = 0;
		while (i <= this.DaysAheadToDisplay)
		{
			if (day >= 0)
			{
				var dailyBattleReward = rewardSequence[num3];
				num3++;
			    var dailyBattleRewardContainer = DailyBattleRewardContainer.Create(day == HighlightedDay,true);
                //dailyBattleRewardContainer.Init(dailyBattleReward.RewardIcon);
                //dailyBattleRewardContainer.DayText.text = this.GetNameForDay(num2);
                //dailyBattleRewardContainer.DayFadeText.text = dailyBattleRewardContainer.DayText.text;
                dailyBattleRewardContainer.PrizeText.text = dailyBattleReward.GetRewardText();
				this.RewardContainersbyDay.Add(day, dailyBattleRewardContainer);
			    dailyBattleRewardContainer.transform.SetParent(LayoutGroup.transform, false);
			}
			i++;
			day++;
		}
        this.RewardListAnimPosition = GetNormalizedPosition(DaysAheadToFocus);
        this.RewardListFinalPosition = GetNormalizedPosition(consecutiveDays-1);
		AudioManager.Instance.PlaySound(this.ScreenAppearAudioEvent, null);
	}

    private float GetNormalizedPosition(int itemIndex)
    {
        var childCount = LayoutGroup.transform.childCount;
        return Mathf.InverseLerp(0, childCount - 1, itemIndex);
    }

	protected override void Update()
	{
        base.Update();
		State currentState = this.DailyBattleCurrentState;
		if (currentState != State.ANIMATE_IN)
		{
			if (currentState == State.INTERACTIVE)
			{
                //if (this.StraplinePanel.localPosition.x > this.StraplineAnimTargetX)
                //{
                //    this.StraplinePanel.Translate(Vector3.left * this.StraplineAnimSpeed * Time.deltaTime);
                //}
			}
		}
		else
		{
			this.StateTimer += Time.deltaTime / this.AnimInTime;
		    var x = Mathf.Lerp(this.RewardListAnimPosition, this.RewardListFinalPosition,
		        this.AnimInCurve.Evaluate(this.StateTimer));
		    ScrollRect.horizontalNormalizedPosition = x;
			if (this.StateTimer >= 1f)
			{
                //this.AnimateToDay(HighlightedDay + 1, true);
				this.DailyBattleCurrentState = State.INTERACTIVE;
			    ContinueButton.interactable = true;
			}
		}
	}

	private void AnimateToDay(int day, bool changeSelection)
	{
		if (changeSelection)
		{
			HighlightedDay = day;
		}
		int num = HighlightedDay - day;
		float num2 = (this.ItemWidthSelected - this.ItemWidthUnselected) * 0.5f;
		foreach (KeyValuePair<int, DailyBattleRewardContainer> current in this.RewardContainersbyDay)
		{
			int currentDay = current.Key - day;
            float num5 = (this.ItemWidthUnselected + this.RewardOffset) * (float)currentDay;
            if (currentDay > num)
			{
				num5 += num2;
			}
            if (currentDay < num)
			{
				num5 -= num2;
			}
			current.Value.AnimateToPosition(Vector3.right * num5);
			if (changeSelection)
			{
				current.Value.Selected = (current.Key == day);
			}
		}
		AudioManager.Instance.PlaySound(this.HighlightPrizeAudioEvent, null);
	}

	private string GetNameForDay(int day)
	{
		int num = day - CurrentDay;
		switch (num + 1)
		{
		case 0:
			return LocalizationManager.GetTranslation(this.YesterdayLocString);
		case 1:
            return LocalizationManager.GetTranslation(this.TodayLocString);
		case 2:
            return LocalizationManager.GetTranslation(this.TomorrowLocString);
		default:
            return string.Format(LocalizationManager.GetTranslation(this.DayLocString), day + 1);
		}
	}

	public void OnContinue()
	{
        Close();
	}
}
