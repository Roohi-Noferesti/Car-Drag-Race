using System;
using System.Collections.Generic;

public class SeasonPrizeSystemManager
{
	private static SeasonPrizeSystemManager _instance;

	private List<SeasonPrizeIdentifier> _prizesWon = new List<SeasonPrizeIdentifier>();

	private List<SeasonPrizeIdentifier> _prizesRequiringAward = new List<SeasonPrizeIdentifier>();

	public static SeasonPrizeSystemManager Instance
	{
		get
		{
			if (SeasonPrizeSystemManager._instance == null)
			{
				SeasonPrizeSystemManager._instance = new SeasonPrizeSystemManager();
				SeasonServerDatabase.Instance.ServerLeaderboardStandingsUpdated += new SeasonServerDatabase.SeasonServerDataUpdated(SeasonPrizeSystemManager._instance.OnNewServerLeaderboardStandings);
			}
			return SeasonPrizeSystemManager._instance;
		}
	}

	public void OnNewServerLeaderboardStandings(SeasonServerDatabase updatedDatabase)
	{
		this._prizesWon.Clear();
		this._prizesRequiringAward.Clear();
		List<RtwLeaderboardStanding> allLeaderboardStandings = updatedDatabase.GetAllLeaderboardStandings();
		List<SeasonPrizeIdentifier> list = new List<SeasonPrizeIdentifier>();
		foreach (RtwLeaderboardStanding current in allLeaderboardStandings)
		{
			int leaderboard_id = current.leaderboard_id;
			foreach (int current2 in current.prizes)
			{
				SeasonPrizeIdentifier item = new SeasonPrizeIdentifier(current2, leaderboard_id);
				list.Add(item);
			}
		}
		if (list.Count < 0)
		{
			return;
		}
		this.AddPrizes(list);
	}

	private void AddPrizes(List<SeasonPrizeIdentifier> prizesWon)
	{
		this._prizesRequiringAward.Clear();
		this._prizesWon.Clear();
		for (int i = 0; i < prizesWon.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < this._prizesWon.Count; j++)
			{
				if (this._prizesWon[j].Equals(prizesWon[i]))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this._prizesWon.Add(prizesWon[i]);
			}
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return;
		}
		for (int k = 0; k < this._prizesWon.Count; k++)
		{
			if (this.CanWeAwardPrize(activeProfile, this._prizesWon[k]))
			{
				this._prizesRequiringAward.Add(this._prizesWon[k]);
			}
		}
		this._prizesRequiringAward.Sort(new SeasonPrizeIdentifierComparer());
	}

	private bool CanWeAwardPrize(PlayerProfile activeProfile, SeasonPrizeIdentifier prizeID)
	{
		return activeProfile != null && !activeProfile.HasSeasonPrizeBeenAwarded(prizeID) && GameDatabase.Instance.SeasonPrizes.ContainsPrize(prizeID.PrizeID);
	}

	public void AtomicAwardPrize(SeasonPrizeIdentifier prize)
	{
		this._prizesRequiringAward.Remove(prize);
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (!this.CanWeAwardPrize(activeProfile, prize))
		{
			return;
		}
		activeProfile.AwardSeasonPrize(prize, GameDatabase.Instance.SeasonPrizes.GetPrizeMetadata(prize));
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	public SeasonPrizeIdentifier GetNextPrize()
	{
		if (this._prizesRequiringAward.Count <= 0)
		{
			return null;
		}
		return this._prizesRequiringAward[0];
	}

	public int GetNumPrizesToAward()
	{
		return this._prizesRequiringAward.Count;
	}

	public void OnUserChanged()
	{
		this._prizesWon.Clear();
		this._prizesRequiringAward.Clear();
	}

	public void DebugResetAwardedPrizes()
	{
	}
}
