using System;
using UnityEngine;

public class FriendReplayDatabase : IBundleOwner
{
	public class Replay
	{
		public string replay_data
		{
			get;
			set;
		}
	}

	public delegate void ReplayDelegate(PlayerReplay replay);

	private const int ERROR_CODE_REPLAY_OR_USER_NOT_FOUND = 452;

	private static FriendReplayDatabase _instance = new FriendReplayDatabase();

	private string carDBKey;

	private CachedFriendRaceData replayLump;

	private FriendReplayDatabase.ReplayDelegate onRequestCompleteDelegate;

	public static FriendReplayDatabase Instance
	{
		get
		{
			return FriendReplayDatabase._instance;
		}
	}

	public void RequestReplay(string carDBKey, CachedFriendRaceData replayLump, FriendReplayDatabase.ReplayDelegate onComplete)
	{
		this.carDBKey = carDBKey;
		this.replayLump = replayLump;
		this.onRequestCompleteDelegate = onComplete;
		if (replayLump.UserID == UserManager.Instance.currentAccount.UserID)
		{
			FriendWebRequests.GetRaceData(UserManager.Instance.currentAccount.UserID, carDBKey, new WebClientDelegate2(this.OnReplayFetchResponse));
		}
		else if (LumpManager.Instance.IsStarLump(replayLump))
		{
			this.LoadStarReplay(carDBKey, StarsManager.GetStarForCar(replayLump, carDBKey));
		}
		else
		{
			FriendWebRequests.GetRaceData(replayLump.UserID, carDBKey, new WebClientDelegate2(this.OnReplayFetchResponse));
		}
	}

	private void NotifyCompleted(PlayerReplay replay)
	{
		this.onRequestCompleteDelegate(replay);
	}

	private void DownloadFailedNetworkError()
	{
		this.onRequestCompleteDelegate(null);
	}

	private void DownloadFailedReplayIsInvalid()
	{
		StarType starType = StarsManager.GetMyStarForCar(this.carDBKey);
		if (starType + 1 != StarType.MAX)
		{
			starType++;
		}
		this.replayLump = LumpManager.Instance.StarLumps[starType];
		this.LoadStarReplay(this.carDBKey, starType);
	}

	public string GetStarReplayFileName(string carDBKey, StarType star)
	{
		return carDBKey + "_" + star.ToString();
	}

	private void OnReplayFetchResponse(string zHTTPContent, string zError, int zStatus, object zUserData)
	{
		if (zStatus == 200 && zError == null && !string.IsNullOrEmpty(zHTTPContent))
		{
			FriendReplayDatabase.Replay replay = JsonConverter.DeserializeObject<FriendReplayDatabase.Replay>(zHTTPContent);
			PlayerReplay playerReplay = PlayerReplay.CreateFromJson(replay.replay_data, this.GetOpponentInfo());
			if (playerReplay == null)
			{
				this.DownloadFailedReplayIsInvalid();
				return;
			}
			string carVersionNumber = CarDatabase.Instance.GetCar(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey).CarVersionNumber;
			if (playerReplay.replayData.replayVersion != carVersionNumber)
			{
				this.DownloadFailedReplayIsInvalid();
				return;
			}
			this.NotifyCompleted(playerReplay);
		}
		else if (zStatus == 452)
		{
			this.DownloadFailedReplayIsInvalid();
		}
		else
		{
			this.DownloadFailedNetworkError();
		}
	}

	private void LoadStarReplay(string carDBKey, StarType star)
	{
		AssetProviderClient.Instance.RequestAsset(this.GetStarReplayFileName(carDBKey, star), new BundleLoadedDelegate(this.LoadedStarReplay), this);
	}

	private void LoadedStarReplay(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
	{
		PlayerReplay replay = PlayerReplay.CreateFromJson((zAssetBundle.mainAsset as TextAsset).text, this.GetOpponentInfo());
		AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
		this.NotifyCompleted(replay);
	}

	private PlayerInfo GetOpponentInfo()
	{
		return LumpManager.Instance.PlayerInfoFromCachedFriendRaceData(this.replayLump);
	}
}
