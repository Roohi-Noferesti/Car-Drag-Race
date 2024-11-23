using System;
using System.Collections.Generic;
using System.Globalization;

public class NetworkReplayWorkItemRTW : NetworkReplayWorkItem
{
	public PlayerReplay OpponentReplay;

	public bool IsBlogger
	{
		get;
		private set;
	}

	public bool IsEliteRace
	{
		get;
		private set;
	}

	public bool IsEvent
	{
		get;
		private set;
	}

	public NetworkReplayServerResponse Response
	{
		get;
		set;
	}

	public NetworkReplayWorkItemRTW(JsonDict json, NetworkReplayServerResponse response)
	{
		this.Response = response;
		this.FromJson(json);
	}

	public NetworkReplayWorkItemRTW(PlayerReplay PlayerReplay, PlayerReplay inOpponentReplay, RaceEventData eventData, NetworkReplayServerResponse response) : base(PlayerReplay)
	{
		this.OpponentReplay = inOpponentReplay;
        this.IsEliteRace = (MultiplayerUtils.SelectedMultiplayerMode == MultiplayerMode.PRO_CLUB);
        this.IsEvent = (MultiplayerUtils.SelectedMultiplayerMode == MultiplayerMode.EVENT);
		ConsumablePlayerInfoComponent component = PlayerReplay.playerInfo.GetComponent<ConsumablePlayerInfoComponent>();
		this.IsBlogger = (component.ConsumablePRAgent > 0);
		this.Response = response;
	}

	public override int GetHashCode()
	{
		throw new NotSupportedException();
	}

	public override ReplayType Type()
	{
		return ReplayType.RaceTheWorld;
	}

	public override bool Equals(object obj)
	{
		if (!base.Equals(obj))
		{
			return false;
		}
		NetworkReplayWorkItemRTW networkReplayWorkItemRTW = obj as NetworkReplayWorkItemRTW;
		return networkReplayWorkItemRTW != null && !(this.OpponentReplay.ToJson() != networkReplayWorkItemRTW.OpponentReplay.ToJson()) && this.IsBlogger == networkReplayWorkItemRTW.IsBlogger && this.IsEliteRace == networkReplayWorkItemRTW.IsEliteRace && this.IsEvent == networkReplayWorkItemRTW.IsEvent;
	}

	public override void ToJson(JsonDict jsonDict)
	{
		base.ToJson(jsonDict);
	    jsonDict.Set("OpponentReplay", OpponentReplay == null ? string.Empty : this.OpponentReplay.ToJson());
		jsonDict.Set("IsBlogger", this.IsBlogger);
		jsonDict.Set("IsEliteRace", this.IsEliteRace);
		jsonDict.Set("IsEvent", this.IsEvent);
	}

	public override void FromJson(JsonDict jsonDict)
	{
		base.FromJson(jsonDict);
		this.OpponentReplay = PlayerReplay.CreateFromJson(jsonDict.GetString("OpponentReplay"), new RTWPlayerInfo());
		this.IsBlogger = jsonDict.GetBool("IsBlogger");
		this.IsEliteRace = jsonDict.GetBool("IsEliteRace");
		this.IsEvent = jsonDict.GetBool("IsEvent");
	}

	public override string ToString()
	{
		return base.ToString() + string.Format("[NetworkReplayWorkItemRTW: OpponentReplay={0}, IsBlogger={1}, IsEliteRace={2}, IsEvent={3}]", new object[]
		{
			this.OpponentReplay.ToJson(),
			this.IsBlogger,
			this.IsEliteRace,
			this.IsEvent
		});
	}

	public override void Upload(WebClientDelegate2 uploadComplete)
	{
        RTWPlayerInfoComponent rtwPlayerInfoComponent = null;
	    RacePlayerInfoComponent racePlayerInfoComponent = null;
		PlayerReplay playerReplay = base.PlayerReplay;
		PlayerReplay opponentReplay = this.OpponentReplay;
		JsonDict jsonDict = new JsonDict();
        jsonDict.Set("uid", UserManager.Instance.currentAccount.UserID.ToString());
		RacePlayerInfoComponent component = playerReplay.playerInfo.GetComponent<RacePlayerInfoComponent>();
	    if (opponentReplay != null)
	    {
            rtwPlayerInfoComponent = opponentReplay.playerInfo.GetComponent<RTWPlayerInfoComponent>();
            racePlayerInfoComponent = opponentReplay.playerInfo.GetComponent<RacePlayerInfoComponent>();
	    }

		if (playerReplay != null)
		{
            jsonDict.Set("branch", UserManager.Instance.currentAccount.AssetDatabaseBranch);
			jsonDict.Set("replay_pp", component.PPIndex.ToString());
            jsonDict.Set("dt", UserManager.Instance.currentAccount.DeviceToken);
			string value = playerReplay.ToJson();
			jsonDict.Set("replay_data", value);
			jsonDict.Set("replay_version", playerReplay.replayData.replayVersion);
			jsonDict.Set("replay_time", playerReplay.replayData.finishTime.ToString(CultureInfo.InvariantCulture));
			jsonDict.Set("replay_finish_speed", playerReplay.replayData.finishSpeed.ToString(CultureInfo.InvariantCulture));
			jsonDict.Set("replay_tier", ((int)component.CarTier).ToString());
		    jsonDict.Set("replay_pcr", component.CarDBKey);
		}
        MultiplayerEventData data = MultiplayerEvent.Saved.Data;
        jsonDict.Set("evtmp", ((data == null) ? 1f : data.RPMultiplier).ToString(CultureInfo.InvariantCulture));
	    if (opponentReplay != null)
	    {
	        jsonDict.Set("challenger_uid", opponentReplay.playerInfo.CsrUserID.ToString());
	        jsonDict.Set("challenger_replay_time", opponentReplay.replayData.finishTime.ToString(CultureInfo.InvariantCulture));
	    }
	    if (racePlayerInfoComponent != null)
	    {
	        jsonDict.Set("challenger_pp", racePlayerInfoComponent.PPIndex.ToString());
	        jsonDict.Set("challenger_tier", ((int)racePlayerInfoComponent.CarTier).ToString());
	    }
	    jsonDict.Set("display_name", PlayerProfileManager.Instance.ActiveProfile.DisplayNameWithUserNameFallback());
		jsonDict.Set("elite", (!this.IsEliteRace) ? "0" : "1");
		jsonDict.Set("evt", (!this.IsEvent) ? "0" : "1");
		jsonDict.Set("rematch", "0");
		jsonDict.Set("blogger", (!this.IsBlogger) ? "0" : "1");
        //jsonDict.Set("leaderboard_id", SeasonFlowManager.Instance.LastPlayedMultiplayerLeaderboardID.ToString());
	    if (rtwPlayerInfoComponent != null)
	    {
	        if (rtwPlayerInfoComponent.MatchId == null)
	        {
	            jsonDict.Set("match_id", string.Empty);
	        }
	        else
	        {
	            jsonDict.Set("match_id", rtwPlayerInfoComponent.MatchId);
	        }
	    }
	    jsonDict.Set("platform", BasePlatform.TargetPlatform.ToString());
		jsonDict.Set("app_ver", ApplicationVersion.Current);
        jsonDict.Set("mpt", UserManager.Instance.currentAccount.MPToken);
        //jsonDict.Set("multiplier", RPBonusManager.GetOverallMultiplier().ToString(CultureInfo.InvariantCulture));
		jsonDict.Set("car_id", component.CarDBKey);
        WebRequestQueueRTW.Instance.StartCall("rtw_result_123", "Storing replay", jsonDict, uploadComplete, UserManager.Instance.currentAccount.UserID, string.Empty, 5);
        //ClientConnectionManager.StoreReplay(PlayerProfileManager.Instance.ActiveProfile.ID, jsonDict.ToString(), playerReplay.replayData.finishTime, component.PPIndex, component.CarDBKey, playerReplay.replayData.replayVersion);
	}

	public override bool ProcessContent(string content)
	{
		if (!string.IsNullOrEmpty(content))
		{
		}
		JsonDict jsonDict = new JsonDict();
		if (jsonDict.Read(content))
		{
			NetworkReplayServerResponse response = this.Response;
			if (response != null)
			{
				this.Response.previousSeasonRankPercentile = response.currentSeasonRankPercentile;
				this.Response.previousSeasonRank = response.currentSeasonRank;
			}
			this.Response.deltaRP = jsonDict.GetInt("rp_season_delta");
			this.Response.raceBonus = jsonDict.GetInt("rp_race_bonus");
			this.Response.eliteBonus = jsonDict.GetInt("rp_elite_bonus");
			this.Response.leadBonus = jsonDict.GetInt("rp_lead_bonus");
			this.Response.rematchBonus = jsonDict.GetInt("rp_rematch_bonus");
            //RtwLeaderboardStatus leaderboardStatus = RtwLeaderboardStatus.FromDict(jsonDict);
            //SeasonServerDatabase.Instance.SetLeaderboardStatus(leaderboardStatus);
			//List<NetworkReplayManager.Ranking> list = NetworkReplayManager.Ranking.FromJsonList(jsonDict.GetJsonList("rankings"));
            //int activeSeasonLeaderboardID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID();
            //NetworkReplayManager.Ranking ranking = list.Find((NetworkReplayManager.Ranking p) => p.leaderboard_id == activeSeasonLeaderboardID);
            //if (ranking != null)
            //{
            //    this.Response.currentSeasonRP = ranking.rp;
            //    this.Response.currentSeasonRankPercentile = ranking.percentile;
            //    PlayerProfileManager.Instance.ActiveProfile.SetPlayerRP(this.Response.currentSeasonRP);
            //    PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
            //}
            //else
            //{
            //    this.Response.currentSeasonRP = -1;
            //    this.Response.currentSeasonRankPercentile = -1;
            //}
			return true;
		}
		return false;
	}
}
