public class RTWPlayerInfoComponent : PlayerInfoComponent
{
	public int _rankPoints;

	public int _rank;

	public int _worldRank;

	public string _match_id = string.Empty;

	public string MatchId
	{
		get
		{
			return this._match_id;
		}
		set
		{
			this._match_id = value;
		}
	}

	public int Rank
	{
		get
		{
			return this._rank;
		}
	}

	public int RankPoints
	{
		get
		{
			return this._rankPoints;
		}
	}

	public int WorldRank
	{
		get
		{
			return this._worldRank;
		}
	}

	public override void SerialiseToJson(JsonDict jsonDict)
	{
		jsonDict.Set("rp", this._rankPoints);
		jsonDict.Set("ra", this._rank);
		jsonDict.Set("wr", this._worldRank);
		jsonDict.Set("mi", this._match_id);
	}

	public override void SerialiseFromJson(JsonDict jsonDict)
	{
		jsonDict.TryGetValue("rp", out this._rankPoints);
		jsonDict.TryGetValue("ra", out this._rank);
		jsonDict.TryGetValue("wr", out this._worldRank);
		jsonDict.TryGetValue("mi", out this._match_id);
	}
}
