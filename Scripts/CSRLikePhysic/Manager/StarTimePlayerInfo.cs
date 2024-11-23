public class StarTimePlayerInfo : PlayerInfo
{
	private const string ONE_STAR_NAME = "Star1";

	private const string TWO_STAR_NAME = "Star2";

	private const string THREE_STAR_NAME = "Star3";

	public StarTimePlayerInfo(StarType type) : base(new StarPersona(type))
	{
		base.AddComponent<RWFPlayerInfoComponent>();
		base.AddComponent<RacePlayerInfoComponent>();
		switch (type)
		{
		case StarType.BRONZE:
			this._csrUserName = "Star1";
			break;
		case StarType.SILVER:
			this._csrUserName = "Star2";
			break;
		case StarType.GOLD:
			this._csrUserName = "Star3";
			break;
		}
	}
}
