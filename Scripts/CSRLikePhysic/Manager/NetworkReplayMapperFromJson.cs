public class NetworkReplayMapperFromJson
{
	public static PlayerReplay MapJsonToNetworkReplayData(string jsonData, PlayerInfo playerInfo = null)
	{
		JsonDict jsonDict = new JsonDict();
		if (!jsonDict.Read(jsonData))
		{
			return null;
		}
		bool flag = false;
		string empty = string.Empty;
		jsonDict.TryGetValue("D", out empty);
		string text = string.Empty;
		if (!jsonDict.TryGetValue("V", out text))
		{
			text = "1.4";
		}
		if (playerInfo == null && text != GameDatabase.Instance.OnlineConfiguration.NetworkReplayVersion)
		{
			return null;
		}
		PlayerReplay playerReplay = NetworkReplayCompress.CreateReplayFromString(empty, out flag, playerInfo);
        playerReplay.playerInfo.SerialiseFromJson(jsonDict);
		playerReplay.replayData.replayVersion = text;
		if (!flag)
		{
			return null;
		}
		return playerReplay;
	}
}
