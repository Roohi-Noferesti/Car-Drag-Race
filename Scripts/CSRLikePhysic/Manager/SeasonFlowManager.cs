using System;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;

public class SeasonFlowManager
{
	private static SeasonFlowManager _instance;

	private DateTime _lastShownDataOutOfDateTime = DateTime.MinValue;

	public static SeasonFlowManager Instance
	{
		get
		{
			if (SeasonFlowManager._instance == null)
			{
				SeasonFlowManager._instance = new SeasonFlowManager();
			}
			return SeasonFlowManager._instance;
		}
	}

	public bool EndOfRaceSeasonChangeTriggered
	{
		get;
		private set;
	}

	public int LastPlayedMultiplayerLeaderboardID
	{
		get
		{
			return PlayerProfileManager.Instance.ActiveProfile.LastPlayedMultiplayerLeaderboardID;
		}
		private set
		{
			PlayerProfileManager.Instance.ActiveProfile.LastPlayedMultiplayerLeaderboardID = value;
		}
	}

	public void OnEnteredWorkshop()
	{
		this.runSeasonEndedFlowLogic();
	}

	public void OnEnteredModeSelect()
	{
		this.runSeasonEndedFlowLogic();
	}

	public void OnMultiplayerHubDetectedSeasonEnd()
	{
		this.runSeasonEndedFlowLogic();
	}

	public bool OnReceivedPlayerList()
	{
		bool flag = this.runSeasonEndedFlowLogic();
		this.EndOfRaceSeasonChangeTriggered = false;
		if (flag)
		{
			this.LastPlayedMultiplayerLeaderboardID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID();
			this.setSeasonLastPlayedFlags();
		}
		else
		{
			this.LastPlayedMultiplayerLeaderboardID = -1;
		}
		return flag;
	}

	public void OnReceivedRaceResults()
	{
		this.EndOfRaceSeasonChangeTriggered = !this.CanContinuePlayingMultiplayer();
	}

	public void OnRespectRankingResults(int leaderboardIDReceived)
	{
		if (leaderboardIDReceived == -1 || leaderboardIDReceived != this.LastPlayedMultiplayerLeaderboardID)
		{
			this.EndOfRaceSeasonChangeTriggered = true;
		}
	}

	private void OnDismissSeasonEndedDialog()
	{
		if (!this.canPrizeGivingFlowComplete())
		{
			return;
		}
		if (ScreenManager.Instance.ActiveScreen.ID == ScreenID.MultiplayerModeSelect)
		{
			this.introduceNewSeasonFlow();
		}
		else if (ScreenManager.Instance.ActiveScreen.ID == ScreenID.PlayerList)
		{
			this.KickOutOfMultiplayer();
		}
	}

	private bool canDataOutOfDateFlowComplete()
	{
		if (SeasonUtilities.IsDataOutOfDate())
		{
			this.showDataOutOfDatePopUp();
			return false;
		}
		return true;
	}

	private bool canPrizeGivingFlowComplete()
	{
		if (this.isTheirPrizesToBeAwarded())
		{
			this.showPrizeGivingFlow();
			return false;
		}
		return true;
	}

	private void introduceNewSeasonFlow()
	{
		this.showNewSeasonIntroductionPopUp();
	}

	private bool runSeasonEndedFlowLogic()
	{
		if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
		{
			return false;
		}
		if (!this.canDataOutOfDateFlowComplete())
		{
			this.ResetMiniLeaderBoard();
			return false;
		}
		if (this.hasSeasonChanged())
		{
			if (SeasonServerDatabase.Instance.IsAnySeasonActive())
			{
				if (!this.shouldShowNewSeasonIntroduction())
				{
					return true;
				}
				if (this.isTheirPrizesToBeAwarded())
				{
					this.showSeasonEndedResultsAreInPopUp();
				}
				else if (ScreenManager.Instance.ActiveScreen.ID == ScreenID.PlayerList)
				{
					SeasonUtilities.ShowSeasonEndedPopUp(new PopUpButtonAction(this.onSeasonEndedPopupDismissed));
				}
				else
				{
					this.OnDismissSeasonEndedDialog();
				}
			}
			else if (ScreenManager.Instance.ActiveScreen.ID != ScreenID.Workshop && ScreenManager.Instance.ActiveScreen.ID != ScreenID.MultiplayerModeSelect)
			{
				SeasonUtilities.ShowSeasonEndedPopUp(new PopUpButtonAction(this.onSeasonEndedPopupDismissed));
			}
			this.ResetMiniLeaderBoard();
			return false;
		}
		if (!SeasonServerDatabase.Instance.IsAnySeasonActive() && ScreenManager.Instance.ActiveScreen.ID == ScreenID.PlayerList)
		{
			if (SeasonServerDatabase.Instance.AreStandingsUpToDate())
			{
				SeasonUtilities.ShowSeasonEndedPopUp(new PopUpButtonAction(this.onSeasonEndedPopupDismissed));
			}
			else
			{
				this.showInvalidStatePopUp();
			}
			this.ResetMiniLeaderBoard();
			return false;
		}
		return true;
	}

	private void ResetMiniLeaderBoard()
	{
		GameObject gameObject = GameObject.Find("MapMultiplayerLeaderboard(Clone)");
		if (gameObject != null)
		{
			gameObject.GetComponent<MapMultiplayerLeaderboard>().ClearMiniLeaderboard();
		}
	}

	private bool hasSeasonChanged()
	{
		if (!SeasonServerDatabase.Instance.DoWeHaveStatusAndStandings())
		{
			return false;
		}
		if (!SeasonServerDatabase.Instance.AreStandingsUpToDate())
		{
			return !SeasonServerDatabase.Instance.IsAnySeasonActive();
		}
		int mostRecentSeasonStandingsLeaderboardID = SeasonServerDatabase.Instance.GetMostRecentSeasonStandingsLeaderboardID();
		int seasonLastPlayedLeaderboardID = PlayerProfileManager.Instance.ActiveProfile.SeasonLastPlayedLeaderboardID;
		return (mostRecentSeasonStandingsLeaderboardID != -1 || seasonLastPlayedLeaderboardID != -1) && ((mostRecentSeasonStandingsLeaderboardID != -1 && seasonLastPlayedLeaderboardID == -1) || mostRecentSeasonStandingsLeaderboardID >= seasonLastPlayedLeaderboardID);
	}

	private void onSeasonEndedPopupDismissed()
	{
		this.KickOutOfMultiplayer();
	}

	private void onDataOutofDataPopupDismissed()
	{
		AssetDatabaseVersionPoll.Instance.PollNow();
		this.KickOutOfMultiplayer();
	}

	private void KickOutOfMultiplayer()
	{
		if (ScreenManager.Instance.IsScreenOnStack(ScreenID.MultiplayerModeSelect))
		{
			ScreenManager.Instance.PopToScreen(ScreenID.MultiplayerModeSelect);
		}
		else if (ScreenManager.Instance.IsScreenOnStack(ScreenID.Workshop))
		{
			if (ScreenManager.Instance.ActiveScreen.ID != ScreenID.Showroom || PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
			{
				ScreenManager.Instance.PopToScreen(ScreenID.Workshop);
			}
		}
	}

	private void showSeasonEndedResultsAreInPopUp()
	{
		int mostRecentFinishedSeasonStatusLeaderboardID = SeasonServerDatabase.Instance.getMostRecentFinishedSeasonStatusLeaderboardID();
		SeasonEventMetadata eventForLeaderboard = SeasonServerDatabase.Instance.GetEventForLeaderboard(mostRecentFinishedSeasonStatusLeaderboardID);
		int mostRecentActiveSeasonLeaderboardID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID();
		SeasonEventMetadata eventForLeaderboard2 = SeasonServerDatabase.Instance.GetEventForLeaderboard(mostRecentActiveSeasonLeaderboardID);
		PopUp popup = new PopUp
		{
			Title = "TEXT_SEASON_ENDED",
			BodyText = string.Format(LocalizationManager.GetTranslation("TEXT_SEASON_END_RESULTS_IN"), LocalizationManager.GetTranslation(eventForLeaderboard.EventTitle), LocalizationManager.GetTranslation(eventForLeaderboard2.EventTitle)),
			BodyAlreadyTranslated = true,
			ConfirmText = "TEXT_BUTTON_OK",
			IsBig = true,
			GraphicPath = PopUpManager.Instance.graphics_raceOfficialPrefab,
			ImageCaption = "TEXT_NAME_RACE_OFFICIAL",
			ConfirmAction = new PopUpButtonAction(this.OnDismissSeasonEndedDialog)
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private void showDataOutOfDatePopUp()
	{
		bool flag = true;
		if (ScreenManager.Instance.ActiveScreen.ID == ScreenID.Workshop)
		{
			DateTime t = this._lastShownDataOutOfDateTime;
			t = t.AddDays(1.0);
			if (GTDateTime.Now < t)
			{
				flag = false;
			}
		}
		if (flag)
		{
			SeasonUtilities.ShowDataOutOfDatePopUp(new PopUpButtonAction(this.onDataOutofDataPopupDismissed));
            this._lastShownDataOutOfDateTime = GTDateTime.Now;
		}
		else
		{
			this.onDataOutofDataPopupDismissed();
		}
	}

	private void showInvalidStatePopUp()
	{
		PopUp popup = new PopUp
		{
			Title = "Invalid State",
			BodyText = "Error - seasons is in an invalid state!",
			ConfirmText = "OK",
			ConfirmAction = new PopUpButtonAction(this.onDataOutofDataPopupDismissed)
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
	}

	public void setSeasonLastPlayedFlags()
	{
		int mostRecentActiveSeasonEventID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonEventID();
		int mostRecentActiveSeasonLeaderboardID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID();
		bool flag = false;
		if (mostRecentActiveSeasonLeaderboardID == -1)
		{
			flag = true;
		}
		if (mostRecentActiveSeasonEventID == -1)
		{
			flag = true;
		}
		if (flag)
		{
			return;
		}
		if (PlayerProfileManager.Instance.ActiveProfile.SeasonLastPlayedEventID == mostRecentActiveSeasonEventID && PlayerProfileManager.Instance.ActiveProfile.SeasonLastPlayedLeaderboardID == mostRecentActiveSeasonLeaderboardID)
		{
			return;
		}
		PlayerProfileManager.Instance.ActiveProfile.SeasonLastPlayedEventID = mostRecentActiveSeasonEventID;
		PlayerProfileManager.Instance.ActiveProfile.SeasonLastPlayedLeaderboardID = mostRecentActiveSeasonLeaderboardID;
		PlayerProfileManager.Instance.ActiveProfile.Save();
	}

	private void showNewSeasonIntroductionPopUp()
	{
		ScreenManager.Instance.PushScreen(ScreenID.SeasonIntro);
		ScreenManager.Instance.UpdateImmediately();
	}

	private void showPrizeGivingFlow()
	{
		PopUp popup = new PopUp
		{
			Title = "TEXT_EVENT_POPUP_PRIZES_TITLE",
			BodyText = "TEXT_EVENT_POPUP_PRIZES_BODY",
			ConfirmText = "TEXT_BUTTON_GETPRIZE",
			ConfirmAction = new PopUpButtonAction(this.ShowSeasonResultScreen),
			GraphicPath = PopUpManager.Instance.graphics_stargazerPrefab,
			ImageCaption = "TEXT_NAME_FRANKIE",
			ShouldCoverNavBar = true
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
	}

	private void ShowSeasonResultScreen()
	{
		if (!ScreenManager.Instance.IsScreenOnStack(ScreenID.Workshop))
		{
			ScreenManager.Instance.PopToScreen(ScreenID.Home);
			ScreenManager.Instance.PushScreen(ScreenID.Workshop);
		}
		ScreenManager.Instance.PopToScreen(ScreenID.Workshop);
		ScreenManager.Instance.PushScreen(ScreenID.SeasonResult);
		ScreenManager.Instance.UpdateImmediately();
	}

	private bool isTheirPrizesToBeAwarded()
	{
		return SeasonPrizeSystemManager.Instance.GetNumPrizesToAward() > 0;
	}

	private bool shouldShowNewSeasonIntroduction()
	{
		int mostRecentCompetedSeasonStandingsLeaderboardID = SeasonServerDatabase.Instance.GetMostRecentCompetedSeasonStandingsLeaderboardID();
		return mostRecentCompetedSeasonStandingsLeaderboardID != -1;
	}

	private bool CanContinuePlayingMultiplayer()
	{
		List<RtwLeaderboardStatusItem> allLeaderboardStatuses = SeasonServerDatabase.Instance.GetAllLeaderboardStatuses();
		if (allLeaderboardStatuses == null || allLeaderboardStatuses.Count <= 0)
		{
			return false;
		}
		if (this.LastPlayedMultiplayerLeaderboardID == -1)
		{
			return false;
		}
		int mostRecentActiveSeasonLeaderboardID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID();
		return mostRecentActiveSeasonLeaderboardID == this.LastPlayedMultiplayerLeaderboardID;
	}
}
