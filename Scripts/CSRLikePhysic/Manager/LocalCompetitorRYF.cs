using System;
using System.Collections.Generic;

public class LocalCompetitorRYF : LocalCompetitorOnline
{
	protected override void OnEnteredPostRace()
	{
		base.OnEnteredPostRace();
        //CommonUI.Instance.StarCountStats.canUpdateSC = false;
		NetworkCompetitor networkCompetitor = CompetitorManager.Instance.OtherCompetitor as NetworkCompetitor;
		NetworkReplay playbackReplayData = networkCompetitor.PlaybackReplayData;
		PlayerReplay zOpponentPlayerReplay = new PlayerReplay(networkCompetitor.PlayerInfo, playbackReplayData.ReplayData);
		PlayerReplay playerReplay = new PlayerReplay(base.PlayerInfo, this.RecordableReplayData.ReplayData);
		RacePlayerInfoComponent component = playerReplay.playerInfo.GetComponent<RacePlayerInfoComponent>();
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		float bestTimeForCar = activeProfile.GetBestTimeForCar(component.CarDBKey);
		RaceResultsTracker.You.PrevBestForFriendsCar = bestTimeForCar;
        //int totalStars = StarsManager.GetMyStarStats().TotalStars;
		if (bestTimeForCar != 0f && playerReplay.replayData.finishTime >= bestTimeForCar)
		{
			return;
		}
		activeProfile.SetBestTimeForCar(component.CarDBKey, playerReplay.replayData.finishTime);
		playerReplay.playerInfo = new LocalPlayerInfo();
		CarInfo car = CarDatabase.Instance.GetCar(component.CarDBKey);
		playerReplay.replayData.replayVersion = car.CarVersionNumber;
		if (this.ShouldUploadReplay())
		{
			NetworkReplayManager.Instance.AddReplayToUploadQueue(playerReplay, zOpponentPlayerReplay, RaceEventInfo.Instance.CurrentEvent, ReplayType.RaceYourFriends);
		}
		activeProfile.Save();
        //FriendsRewardManager.Instance.CalculateRewardsForStars(StarsManager.GetMyStarStats().TotalStars, totalStars);
        //FriendsRewardManager.Instance.CalculateRewardsForTime(component.CarDBKey, playerReplay.replayData.finishTime, bestTimeForCar, RaceResultsTracker.You.IsWinner);
		this.QueuePushMessages(car, playerReplay.replayData.finishTime);
	}

	private void QueuePushMessages(CarInfo carInfo, float finishTime)
	{
        //List<int> allFriendsInTimeBracketForCar = LumpManager.Instance.GetAllFriendsInTimeBracketForCar(RaceResultsTracker.You.PrevBestForFriendsCar, finishTime, carInfo.Key);
        //if (allFriendsInTimeBracketForCar.Count == 0)
        //{
        //    return;
        //}
        //List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>();
        //list.Add(new KeyValuePair<string, bool>(PlayerProfileManager.Instance.ActiveProfile.DisplayNameWithUserNameFallback(), false));
        //list.Add(new KeyValuePair<string, bool>(carInfo.MediumName, true));
        //FriendWebRequests.QueuePushNotification(UserManager.Instance.currentAccount.UserID, allFriendsInTimeBracketForCar, "TEXT_FRIENDS_PUSH_TIMEBEATEN", list);
	}

	private bool ShouldUploadReplay()
	{
		return RaceResultsTracker.You.IsWinner || PlayerProfileManager.Instance.ActiveProfile.FriendRacesWon != 0;
	}

	protected override ReplayType GetReplayType()
	{
		return ReplayType.RaceYourFriends;
	}
}
