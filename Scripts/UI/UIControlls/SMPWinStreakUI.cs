using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SMPWinStreakUI : MonoBehaviour
{
	[Serializable]
	public class RewardTypeSMPItemMap
	{
		public ERewardType RewardType;

		public SMPWinStreakItemUI SMPWinStreakItemType;
	}

    //public PrefabPlaceholder WinsContainerPlaceholder;

	public GridLayoutGroup RewardsGrid;

    public Transform TimerWidget;

	public TextMeshProUGUI TimerLabel;

	public RewardTypeSMPItemMap[] RewardSMPItemCollection;

	public SMPWinStreakItemUI DefaultItemUI;

	public static int PreviousWinCount = -1;

	private Transform RewardWidget;

    //private TweenPosition WinCountPositionTween;

    //private TweenAlpha WinCountAlphaTween;

    //private TweenAlpha TimerAlphaTween;

	private bool UpdateWinCountBoxPosition;

	private int previousHours;

	private int previousMins;

	private int previousSecs;

	private int fadeWinStreakIndex;

    private List<SMPWinStreakItemUI> m_itemList = new List<SMPWinStreakItemUI>(); 

    public WinCountBoxUI WinStreakUI;

	private void Awake()
	{
		if (WinStreakUI != null)
		{
            //this.WinCountPositionTween = this.WinCountBox.GetComponent<TweenPosition>();
            //if (this.WinCountPositionTween != null)
            //{
            //    this.WinCountPositionTween.AddOnFinished(new EventDelegate.Callback(this.OnWinCountPositionTweenFinished));
            //}
            //CoroutineManager.Instance.WaitForFrameThenExecute(4, OnWinCountPositionTweenFinished);
		    //this.WinCountAlphaTween = this.WinCountBox.GetComponent<TweenAlpha>();
		    //if (this.WinCountAlphaTween != null)
		    //{
		    //    this.WinCountAlphaTween.AddOnFinished(new EventDelegate.Callback(this.OnWinCountBoxAlphaTweenFinished));
		    //}
            //CoroutineManager.Instance.WaitForFrameThenExecute(4, OnWinCountBoxAlphaTweenFinished);
		}
		if (TimerWidget != null)
		{
            //this.TimerAlphaTween = this.TimerWidget.GetComponent<TweenAlpha>();
            //if (this.TimerAlphaTween != null)
            //{
            //    this.TimerAlphaTween.AddOnFinished(new EventDelegate.Callback(this.OnTimerWidgetAlphaTweenFinished));
            //}
            CoroutineManager.Instance.WaitForFrameThenExecute(4, OnTimerWidgetAlphaTweenFinished);
		}
        //if (this.WinsContainerPlaceholder != null)
        //{
        //    this.WinStreakUI = this.WinsContainerPlaceholder.GetBehaviourOnPrefab<WinCountBoxUI>();
        //}
        //if (CSRRoomsClient.Instance != null && CSRRoomsClient.Instance.WinStreakMonitor != null)
        //{
        //    CSRRoomsClient.Instance.OnWinChallengeAvailable += OnWinChallengeAvailable;
        //}
		previousHours = -1;
		previousMins = -1;
		fadeWinStreakIndex = 0;
	}

    private void Start()
    {
        int currentWinStreak = 0;
        if (PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null)
        {
            currentWinStreak = PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins;
        }
        if (PreviousWinCount == -1)
        {
            PreviousWinCount = currentWinStreak;
        }
        PreviousWinCount = ((PreviousWinCount >= currentWinStreak - 1) ? PreviousWinCount : (currentWinStreak - 1));
        PreviousWinCount = currentWinStreak - 1;
        if (WinStreakUI != null)
        {
            WinStreakUI.SetMaxWinStreakLabel();
            WinStreakUI.SetCurrentWinStreakLabel(currentWinStreak+1);//PreviousWinCount);
        }
        //if (RewardsGrid != null)
        //{
        //    RewardWidget = RewardsGrid.GetComponent<UIWidget>();
        //}
        int smpwinStreakValue = 9;
        if (true) //GameDataDatabase.instance != null && GameDataDatabase.instance.GameData != null)
        {
            List<SMPWinStreakReward> sMPWinStreakData = SMPConfigManager.WinStreak.SMPWinStreakData;
            if (SMPConfigManager.WinStreak.SMPWinStreakMaxValue > 0)
            {
                smpwinStreakValue = SMPConfigManager.WinStreak.SMPWinStreakMaxValue;
            }
            if (sMPWinStreakData == null || RewardsGrid == null)
            {
                return;
            }
            //UIWidget component = RewardsGrid.GetComponent<UIWidget>();
            //if (component != null)
            //{
            //    RewardsGrid.cellWidth = (RewardsGrid.cellHeight = (float)(component.width / smpwinStreakValue));
            //}
            List<SMPWinStreakReward> list =
                (from o in sMPWinStreakData orderby o.StreakCount select o).ToList<SMPWinStreakReward>();

            int i;
            if (currentWinStreak >= smpwinStreakValue)
            {
                currentWinStreak = 0;
            }
            for (i = 1; i <= smpwinStreakValue; i++)
            {
                SMPWinStreakReward sMPWinStreakReward = list.Find((SMPWinStreakReward o) => o.StreakCount == i);
                SMPWinStreakItemUI sMPWinStreakItemUI = DefaultItemUI;
                if (sMPWinStreakReward != null)
                {
                    sMPWinStreakItemUI = GetItemUIForReward(sMPWinStreakReward);
                }
                if (sMPWinStreakItemUI != null)
                {
                    GameObject winStreakInstance = Instantiate(sMPWinStreakItemUI.gameObject);
                    winStreakInstance.name = "SMPWinStreakItemUI_" + i;
                    winStreakInstance.transform.SetParent(RewardsGrid.transform, false);

                    SMPWinStreakItemUI winStreakComp = winStreakInstance.GetComponent<SMPWinStreakItemUI>();
                    m_itemList.Add(winStreakComp);
                    if (winStreakComp != null)
                    {
   
                        winStreakComp.Initialise(sMPWinStreakReward, i, currentWinStreak);
                        //if (winStreakComp.AnimRedirect != null)
                        //{
                        //    winStreakComp.AnimRedirect.AddRedirect(
                        //        new EventDelegate.Callback(this.OnWinStreakFadeInFinished));
                        //    winStreakComp.AnimRedirect.AddRedirect(
                        //        new EventDelegate.Callback(this.OnWinStreakFadeOutFinished));
                        //}
                    }
                }
            }
            //RewardsGrid.Reposition(false);
            UpdateWinCountBoxPosition = true;
            if (!PlayerProfileManager.Instance.ActiveProfile.IsSMPWinChallengeAvailable)
            {
                DeactivateWinChallengeSystem(0f, true);
            }
            else if (PreviousWinCount == SMPConfigManager.WinStreak.SMPWinStreakMaxValue &&
                     PlayerProfileManager.Instance.ActiveProfile.IsSMPWinChallengeAvailable)
            {
                if (PlayerProfileManager.Instance.ActiveProfile.IsNextSMPChallengeAvailable())
                {
                    ResetWinChallengeSystem();
                }
                else
                {
                    PlayerProfileManager.Instance.ActiveProfile.IsSMPWinChallengeAvailable = false;
                    DeactivateWinChallengeSystem(0f, true);
                }
                PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
            }
            if (TimerWidget != null)
            {
                TimerWidget.gameObject.SetActive(!PlayerProfileManager.Instance.ActiveProfile.IsSMPWinChallengeAvailable);
                //FadeTimerWidget(false, delaySec);
            }
            int sMPChallengeWins = PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins;
            if (sMPChallengeWins >= SMPConfigManager.WinStreak.SMPWinStreakMaxValue)
            {
                if (PlayerProfileManager.Instance.ActiveProfile.IsNextSMPChallengeAvailable())
                {
                    ResetWinChallengeSystem();
                    //DeactivateWinChallengeSystem(0f, true);
                }
                else
                {
                    PlayerProfileManager.Instance.ActiveProfile.IsSMPWinChallengeAvailable = false;
                    DeactivateWinChallengeSystem(0f, true);
                    PreviousWinCount = -1;
                }
                PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
            }
        }
    }

    public void OnDestroy()
	{
        //if (CSRRoomsClient.Instance != null && CSRRoomsClient.Instance.WinStreakMonitor != null)
        //{
        //    CSRRoomsClient.Instance.WinStreakMonitor.OnWinChallengeAvailable -= OnWinChallengeAvailable;
        //}
	}

	public void Update()
	{
		if (!PlayerProfileManager.Instance.ActiveProfile.IsSMPWinChallengeAvailable)
		{
			TimeSpan nextSMPWinChallengeRemainingTime = PlayerProfileManager.Instance.ActiveProfile.GetNextSMPWinChallengeRemainingTime();
			if (TimerLabel != null)
			{
				if (nextSMPWinChallengeRemainingTime.Hours > 0)
				{
					if (nextSMPWinChallengeRemainingTime.Hours != previousHours || nextSMPWinChallengeRemainingTime.Minutes != previousMins)
					{
						TimerLabel.text = nextSMPWinChallengeRemainingTime.GetTimeFormattedLocalisedString(2);
						previousHours = nextSMPWinChallengeRemainingTime.Hours;
						previousMins = nextSMPWinChallengeRemainingTime.Minutes;
					}
				}
				else if (nextSMPWinChallengeRemainingTime.Minutes != previousMins || nextSMPWinChallengeRemainingTime.Seconds != previousSecs)
				{
					TimerLabel.text = nextSMPWinChallengeRemainingTime.GetTimeFormattedLocalisedString(2);
					previousHours = nextSMPWinChallengeRemainingTime.Hours;
					previousMins = nextSMPWinChallengeRemainingTime.Minutes;
					previousSecs = nextSMPWinChallengeRemainingTime.Seconds;
				}
			}
		}
	}

	public void LateUpdate()
	{
        //if (RewardsGrid != null && RewardWidget != null && RewardWidget.enabled)
        //{
        //    UIWidget component = GetComponent<UIWidget>();
        //    if (component != null)
        //    {
        //        RewardsGrid.gameObject.transform.localPosition = new Vector3(0f, 0f);
        //    }
        //    RewardWidget.enabled = false;
        //}
		if (UpdateWinCountBoxPosition)
		{
			UpdateWinCountBoxPosition = false;
			TriggerWinStreakUpdate();
		}
	}

	private SMPWinStreakItemUI GetItemUIForReward(SMPWinStreakReward reward)
	{
		SMPWinStreakItemUI sMPWinStreakItemUI = null;
        for (int i = 0; i < RewardSMPItemCollection.Length; i++)
		{
            if (RewardSMPItemCollection[i].RewardType == reward.WinReward.rewardType)
			{
                sMPWinStreakItemUI = RewardSMPItemCollection[i].SMPWinStreakItemType;
				break;
			}
		}
		if (sMPWinStreakItemUI == null)
		{
			sMPWinStreakItemUI = DefaultItemUI;
		}
		return sMPWinStreakItemUI;
	}

	public void TriggerWinStreakUpdate()
	{
		int sMPChallengeWins = PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins;
		if (true)//sMPChallengeWins == PreviousWinCount)
		{
			PositionWinStreakBox(sMPChallengeWins);
			return;
		}
        //PositionWinStreakBox(PreviousWinCount);
        //if (RewardsGrid != null && sMPChallengeWins <= RewardsGrid.transform.childCount && PreviousWinCount >= 0 && PreviousWinCount < RewardsGrid.transform.childCount)
        //{
        //    int index = (PreviousWinCount - 1 >= 0) ? (PreviousWinCount - 1) : 0;
        //    int index2 = (sMPChallengeWins - 1 >= 0) ? (sMPChallengeWins - 1) : 0;
        //    Transform child = RewardsGrid.transform.GetChild(index);
        //    Transform child2 = RewardsGrid.transform.GetChild(index2);
        //    if (child2 != null && child != null)// && this.WinCountPositionTween != null)
        //    {
        //        //this.WinCountPositionTween.from = new Vector3(child.localPosition.x, WinCountBox.transform.localPosition.y);
        //        //this.WinCountPositionTween.to = new Vector3(child2.localPosition.x, WinCountBox.transform.localPosition.y);
        //        //this.WinCountPositionTween.delay = 1.5f;
        //        //this.WinCountPositionTween.duration = 0.35f;
        //        //this.WinCountPositionTween.enabled = true;
        //        //this.WinCountPositionTween.PlayForward();
        //        AudioManager.Instance.PlaySound(AudioEvent.SMPNumOfWins, null);
        //    }
        //    else if (child2 != null)
        //    {
        //        WinStreakUI.transform.position = new Vector3(child2.position.x, WinStreakUI.transform.position.y, WinStreakUI.transform.position.z);
        //        if (WinStreakUI != null)
        //        {
        //            WinStreakUI.SetCurrentWinStreakLabel(sMPChallengeWins);
        //        }
        //    }
        //}
        //PreviousWinCount = sMPChallengeWins;
	}

	private void OnWinCountPositionTweenFinished()
	{
		int sMPChallengeWins = PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins;
		if (WinStreakUI != null)
		{
			WinStreakUI.TriggerWinStreakLabelUpdate(true);
			if (RewardsGrid != null)
			{
				int num = (sMPChallengeWins - 1 >= 0) ? (sMPChallengeWins - 1) : 0;
				int num2 = num - 1;
				if (num2 >= 0)
				{
					Transform child = RewardsGrid.transform.GetChild(num2);
					if (child != null)
					{
						SMPWinStreakItemUI component = child.GetComponent<SMPWinStreakItemUI>();
						if (component != null)
						{
							component.PlayAnimationState("PrizeCompleted");
						}
					}
				}
				Transform child2 = RewardsGrid.transform.GetChild(num);
				if (child2 != null)
				{
					SMPWinStreakItemUI component2 = child2.GetComponent<SMPWinStreakItemUI>();
					if (component2 != null)
					{
						component2.PlayAnimationState("PrizeWin");
					}
				}
			}
		}
	}

	public void OnCurrentWinAnimationFinished()
	{
		int sMPChallengeWins = PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins;
		if (sMPChallengeWins >= SMPConfigManager.WinStreak.SMPWinStreakMaxValue)
		{
			if (PlayerProfileManager.Instance.ActiveProfile.IsNextSMPChallengeAvailable())
			{
				DeactivateWinChallengeSystem(0f, true);
			}
			else
			{
				PlayerProfileManager.Instance.ActiveProfile.IsSMPWinChallengeAvailable = false;
				DeactivateWinChallengeSystem(0f, true);
				PreviousWinCount = -1;
			}
			PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
		}
	}

	public void OnWinStreakFadeInFinished()
	{
		fadeWinStreakIndex++;
		fadeWinStreakIndex = ((fadeWinStreakIndex >= RewardsGrid.transform.childCount) ? (RewardsGrid.transform.childCount - 1) : fadeWinStreakIndex);
        //Transform child = RewardsGrid.transform.GetChild(fadeWinStreakIndex);
        //if (child != null)
        //{
        //    SMPWinStreakItemUI component = child.GetComponent<SMPWinStreakItemUI>();
        //    if (component != null)
        //    {
        //        component.PlayAnimationState("PrizeFadeIn");
        //    }
        //}
	}

	public void OnWinStreakFadeOutFinished()
	{
		if (fadeWinStreakIndex == 0)
		{
			if (PlayerProfileManager.Instance.ActiveProfile.IsSMPWinChallengeAvailable)
			{
				ResetWinChallengeSystem();
			}
			else
			{
				ShowTimerWidget(0f);
			}
		}
		else
		{
			fadeWinStreakIndex--;
			fadeWinStreakIndex = ((fadeWinStreakIndex < 0) ? 0 : fadeWinStreakIndex);
            //Transform child = RewardsGrid.transform.GetChild(fadeWinStreakIndex);
            //if (child != null)
            //{
            //    SMPWinStreakItemUI component = child.GetComponent<SMPWinStreakItemUI>();
            //    if (component != null)
            //    {
            //        component.PlayAnimationState("PrizeInactive");
            //    }
            //}
		}
	}

	private bool PositionWinStreakBox(int winCount)
	{
	    int num = (winCount  >= 0) ? (winCount ) : 0;
		if (RewardsGrid != null && num < RewardsGrid.transform.childCount)
		{
			Transform child = RewardsGrid.transform.GetChild(num);
			if (child != null)
			{
                WinStreakUI.transform.position = new Vector3(child.position.x, WinStreakUI.transform.position.y, WinStreakUI.transform.position.z);
			}
		}
		return false;
	}

	private void FadeTimerWidget(bool bFadeIn, float delay = 0f)
	{
        //if (this.TimerAlphaTween != null)
        //{
        //    this.TimerAlphaTween.enabled = true;
        //    this.TimerAlphaTween.delay = delay;
        //    this.TimerAlphaTween.from = 0f;
        //    this.TimerAlphaTween.to = 1f;
        //    if (bFadeIn)
        //    {
        //        this.TimerAlphaTween.PlayForward();
        //    }
        //    else
        //    {
        //        this.TimerAlphaTween.PlayReverse();
        //    }
        //}
	}

	private void FadeWinCountBox(bool bFadeIn, float delay = 0f)
	{
        //if (this.WinCountAlphaTween != null)
        //{
        //    this.WinCountAlphaTween.enabled = true;
        //    this.WinCountAlphaTween.delay = delay;
        //    this.WinCountAlphaTween.from = 0f;
        //    this.WinCountAlphaTween.to = 1f;
        //    if (bFadeIn)
        //    {
        //        this.WinCountAlphaTween.PlayForward();
        //    }
        //    else
        //    {
        //        this.WinCountAlphaTween.PlayReverse();
        //    }
        //}
	}

	private void DeactivateWinStreaks()
	{
        if (RewardsGrid != null)
        {
            RewardsGrid.gameObject.SetActive(false);
            //fadeWinStreakIndex = RewardsGrid.transform.childCount - 1;
            //Transform child = RewardsGrid.transform.GetChild(fadeWinStreakIndex);
            //if (child != null)
            //{
            //    SMPWinStreakItemUI component = child.GetComponent<SMPWinStreakItemUI>();
            //    if (component != null)
            //    {
            //        component.PlayAnimationState("PrizeInactive");
            //    }
            //}
        }
	}

	private void ActivateWinStreaks()
	{
		if (RewardsGrid != null)
		{
            RewardsGrid.gameObject.SetActive(true);
            //fadeWinStreakIndex = 0;
            //Transform child = RewardsGrid.transform.GetChild(fadeWinStreakIndex);
            //if (child != null)
            //{
            //    SMPWinStreakItemUI component = child.GetComponent<SMPWinStreakItemUI>();
            //    if (component != null)
            //    {
            //        component.PlayAnimationState("PrizeFadeIn");
            //    }
            //}
		}
	}

	private void OnTimerWidgetAlphaTweenFinished()
	{
		if (PlayerProfileManager.Instance.ActiveProfile.IsSMPWinChallengeAvailable)
		{
			ActivateWinStreaks();
		}
		else
		{
			DeactivateWinStreaks();
		}
        //if (this.TimerAlphaTween != null)
        //{
        //    this.TimerAlphaTween.enabled = false;
        //}
	}

	private void OnWinCountBoxAlphaTweenFinished()
	{
        //if (this.WinCountAlphaTween != null)
        //{
        //    this.WinCountAlphaTween.enabled = false;
        //}
	}

	private void DeactivateWinChallengeSystem(float delaySec, bool bDoFadeAnimation = true)
	{
		PlayerProfileManager.Instance.ActiveProfile.ResetSMPChallengeWins();
        //FadeWinCountBox(false, delaySec);
        WinStreakUI.gameObject.SetActive(false);
        //if (bDoFadeAnimation)
        //{
        //    DeactivateWinStreaks();
        //}
        //else
        //{
			ShowTimerWidget(delaySec);
        //}
	}

	private void ShowTimerWidget(float delaySec)
	{
		if (TimerWidget != null)
		{
			TimerWidget.gameObject.SetActive(true);
            //TimerWidget.alpha = 0f;
			FadeTimerWidget(true, delaySec);
			if (TimerLabel != null)
			{
				TimeSpan nextSMPWinChallengeRemainingTime = PlayerProfileManager.Instance.ActiveProfile.GetNextSMPWinChallengeRemainingTime();
				TimerLabel.text = string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_HOURS_AND_MINUTES"), nextSMPWinChallengeRemainingTime.Hours, nextSMPWinChallengeRemainingTime.Minutes);
			}
		}
	}

	private void ActivateWinChallengeSystem(float delaySec)
	{
		FadeWinCountBox(true, delaySec);
		if (TimerWidget != null)
		{
            TimerWidget.gameObject.SetActive(false);
            //FadeTimerWidget(false, delaySec);
		}
	}

	private void ResetWinChallengeSystem()
	{
		FadeWinCountBox(true, 0f);
		PlayerProfileManager.Instance.ActiveProfile.ResetSMPChallengeWins();
		int sMPChallengeWins = PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins;
		PositionWinStreakBox(sMPChallengeWins);
		if (WinStreakUI != null)
		{
			WinStreakUI.TriggerWinStreakLabelUpdate(true);
			ActivateWinStreaks();
		}
		PreviousWinCount = -1;

        //for (int i = 0; i < m_itemList.Count; i++)
        //{
        //    var smpWinStreakItemUi = m_itemList[i];
        //    smpWinStreakItemUi.PlayAnimationState(i == 0 ? "PrizeActive" : "PrizeInactive");
        //}
	}

	private void OnWinChallengeAvailable()
	{
		ActivateWinChallengeSystem(0f);
	}
}
