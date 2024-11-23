public class NetworkReplayMapperToJson
{
	public static string MapNetworkReplayDataToJson(PlayerReplay playerReplay)
	{
		JsonDict jsonDict = new JsonDict();
        playerReplay.playerInfo.SerialiseToJson(jsonDict);
		jsonDict.Set("D", NetworkReplayCompress.CreateStringFromReplayData(playerReplay));
		jsonDict.Set("V", playerReplay.replayData.replayVersion);
		string floatFormatString = jsonDict.FloatFormatString;
		jsonDict.FloatFormatString = "{0:0.00000}";
		string result = jsonDict.ToString();
		jsonDict.FloatFormatString = floatFormatString;
		return result;
	}
}
