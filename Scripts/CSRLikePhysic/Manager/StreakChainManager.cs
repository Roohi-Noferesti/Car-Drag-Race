using System;

public class StreakChainManager
{
	private int streakChainCount;

	private bool offerStreakChain;

    //private RPBonusWindow systemTimeWindow = new RPBonusWindow();

    //private RPBonusWindow serverTimeWindow = new RPBonusWindow();

	//private DateTime latestServerTime = default(DateTime);

    //private RPBonusChain bonus = new RPBonusChain();

	private string streakChainCarID;

	public int ChainCount
	{
		get
		{
			return this.streakChainCount;
		}
	}

	public int VisibleChainCount
	{
		get
		{
			return this.ChainCount + 1;
		}
	}

	public float Multiplier
	{
		get
		{
            //if (this.Active())
            //{
            //    return GameDatabase.Instance.RPBonusConfiguration.GetStreakChainRPMultiplier(this.streakChainCount);
            //}
			return 0f;
		}
	}

	public bool Active()
	{
		return this.streakChainCount > 0;
	}

	private void restartSystemTimeNow()
	{
        //DateTime inStart = this.systemTimeNow();
        //DateTime inFinish = inStart.Add(GameDatabase.Instance.RPBonusConfiguration.GetStreakChainRPDuration());
        //this.systemTimeWindow = new RPBonusWindow(inStart, inFinish);
	}

	private void restartServerTimeNow()
	{
        //if (ServerSynchronisedTime.Instance.ServerTimeValid)
        //{
        //    this.latestServerTime = ServerSynchronisedTime.Instance.GetDateTime();
        //    DateTime inFinish = this.latestServerTime.Add(GameDatabase.Instance.RPBonusConfiguration.GetStreakChainRPDuration());
        //    this.serverTimeWindow = new RPBonusWindow(this.latestServerTime, inFinish);
        //}
	}

	public void UpdateTimeout()
	{
		if (this.Active())
		{
            //if (ServerSynchronisedTime.Instance.ServerTimeValid)
            //{
            //    this.latestServerTime = ServerSynchronisedTime.Instance.GetDateTime();
            //}
			if (this.systemTimeSuitable())
			{
				this.restartSystemTimeNow();
			}
			if (this.serverTimeSuitable())
			{
				this.restartServerTimeNow();
			}
		}
	}

	public void StreakWon()
	{
		this.offerStreakChain = true;
	}

	public void Reset()
	{
        //this.systemTimeWindow = new RPBonusWindow();
        //this.serverTimeWindow = new RPBonusWindow();
		this.streakChainCount = 0;
		this.offerStreakChain = false;
        //if (RPBonusManager.ContainsUniqueBonusObject(this.bonus))
        //{
        //    RPBonusManager.RemoveUniqueBonusObject(this.bonus);
        //}
	}

	public void CheckBonusTimeOut()
	{
		if (!PopUpManager.Instance.isShowingPopUp && this.Active() && !this.timeSuitable())
		{
			PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_STREAK_CHAIN_TIMEOUT_TITLE", "TEXT_POPUPS_MULTIPLAYER_STREAK_CHAIN_TIMEOUT_BODY", null, false);
			this.Reset();
		}
	}

	private void PushStreakChainScreen(Action acceptChain, Action rejectChain)
	{
	    //bool flag = false;//GameDatabase.Instance.RPBonusConfiguration.GetStreakChainRPMultiplier(this.streakChainCount) == GameDatabase.Instance.RPBonusConfiguration.BonusLimit;
        //string body = (!flag) ? string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_MULTIPLAYER_STREAK_CHAIN_OFFER_BODY"), Mathf.RoundToInt(GameDatabase.Instance.RPBonusConfiguration.GetStreakChainRPMultiplier(this.streakChainCount + 1) * 100f)) : LocalizationManager.GetTranslation("TEXT_POPUPS_MULTIPLAYER_STREAK_CHAIN_OFFER_BODY_MAXED");
        //PopUpScreen.InitScreen("TEXT_POPUPS_MULTIPLAYER_STREAK_CHAIN_OFFER_TITLE", body, PopUpManager.Instance.graphics_stargazerPrefab, "TEXT_NAME_FRANKIE", delegate
        //{
        //    this.onPopupConfirmStreakChain(acceptChain);
        //}, "TEXT_BUTTON_OK", delegate
        //{
        //    this.onPopupCancelStreakChain(rejectChain);
        //}, "TEXT_BUTTON_CANCEL", false, true, false);
		this.startNewStreakInChain();
        //ScreenManager.Instance.PushScreen(ScreenID.PopUp);
	}

	public bool OfferStreakChainIfAvailable(Action acceptChain, Action rejectChain)
	{
		if (this.offerStreakChain)
		{
			this.UpdateTimeout();
			if (this.Active() && !this.timeSuitable())
			{
				this.Reset();
				return PopUpManager.Instance.TryShowPopUp(PopUpDatabase.Common.GetStargazerPopup("TEXT_POPUPS_MULTIPLAYER_STREAK_CHAIN_OFFER_TIMEOUT_TITLE", "TEXT_POPUPS_MULTIPLAYER_STREAK_CHAIN_OFFER_TIMEOUT_BODY", delegate
				{
					this.onPopupConfirmStreakChainTimeout(rejectChain);
				}, false), PopUpManager.ePriority.Default, null);
			}
            //if (PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_VersusRaceTeamCompleted && GameDatabase.Instance.RPBonusConfiguration.HasStreakChainData())
            //{
            //    this.PushStreakChainScreen(acceptChain, rejectChain);
            //    return true;
            //}
		}
		this.offerStreakChain = false;
		return false;
	}

	private void onPopupConfirmStreakChainTimeout(Action rejectChain)
	{
		if (rejectChain != null)
		{
			rejectChain();
		}
	}

	private void onPopupConfirmStreakChain(Action acceptChain)
	{
		this.triggerMetricEvent(true);
		this.UpdateTimeout();
		if (acceptChain != null)
		{
			acceptChain();
		}
	}

	private void onPopupCancelStreakChain(Action rejectChain)
	{
		this.triggerMetricEvent(false);
		this.Reset();
		if (rejectChain != null)
		{
			rejectChain();
		}
	}

	private void triggerMetricEvent(bool doContinue)
	{
        //int mostRecentActiveSeasonLeaderboardID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID();
        //EventLeaderboard leaderboard = NetworkReplayManager.Instance.Leaderboard.GetLeaderboard(mostRecentActiveSeasonLeaderboardID);
        //Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
        //{
        //    {
        //        Parameters.Continue,
        //        (!doContinue) ? "0" : "1"
        //    },
        //    {
        //        Parameters.LPos,
        //        (leaderboard == null) ? "NA" : leaderboard.currentSeasonRank.ToString()
        //    },
        //    {
        //        Parameters.StreakType,
        //        MultiplayerUtils.GetMultiplayerStreakType()
        //    }
        //};
        //Log.AnEvent(Events.StreakChain, data);
	}

	private void startNewStreakInChain()
	{
        //this.streakChainCount++;
        //if (!RPBonusManager.ContainsUniqueBonusObject(this.bonus))
        //{
        //    RPBonusManager.AddUniqueBonusObject(this.bonus);
        //}
        //RPBonusManager.RefreshUI();
	}

	private bool timeSuitable()
	{
		return this.systemTimeSuitable() && this.serverTimeSuitable();
	}

	private bool systemTimeSuitable()
	{
	    //return this.systemTimeWindow.IsInvalid() || this.systemTimeWindow.IsInsideWindow(this.systemTimeNow());
	    return false;
	}

    private bool serverTimeSuitable()
    {
        //return this.serverTimeWindow.IsInvalid() || this.serverTimeWindow.IsInsideWindow(this.latestServerTime);
        return false;
    }

    private DateTime systemTimeNow()
	{
        return ServerSynchronisedTime.Instance.GetDateTime();
	}
}
