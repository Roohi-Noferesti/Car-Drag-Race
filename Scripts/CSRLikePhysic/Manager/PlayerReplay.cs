public class PlayerReplay
{
	public PlayerInfo playerInfo;

	public NetworkReplayData replayData;

	public string Identifier
	{
		get
		{
			return string.Format("{0}{1}", this.playerInfo.CsrUserID, this.replayData.finishTime);
		}
	}

	public PlayerReplay(PlayerInfo zPlayerInfo)
	{
		this.playerInfo = zPlayerInfo;
		this.replayData = new NetworkReplayData();
	}

	public PlayerReplay(PlayerInfo zPlayerInfo, NetworkReplayData zReplayData)
	{
		this.playerInfo = zPlayerInfo;
		this.replayData = zReplayData;
	}

	public static PlayerReplay CreateFromJson(string zJson, PlayerInfo playerInfo = null)
	{
        return NetworkReplayMapperFromJson.MapJsonToNetworkReplayData(zJson, playerInfo);
	}

    public string ToJson()
    {
        return NetworkReplayMapperToJson.MapNetworkReplayDataToJson(this);
    }
}
