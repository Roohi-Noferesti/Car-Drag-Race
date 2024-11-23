public class ConsumablePlayerInfoComponent : PlayerInfoComponent
{
	private int _consumablePRAgent;

	private int _consumableTyre;

	private int _consumableN2O;

	private int _consumableEngine;

	public int ConsumablePRAgent
	{
		get
		{
			return this._consumablePRAgent;
		}
		set
		{
			this._consumablePRAgent = value;
		}
	}

	public int ConsumableEngine
	{
		get
		{
			return this._consumableEngine;
		}
		set
		{
			this._consumableEngine = value;
		}
	}

	public int ConsumableTyre
	{
		get
		{
			return this._consumableTyre;
		}
		set
		{
			this._consumableTyre = value;
		}
	}

	public int ConsumableN2O
	{
		get
		{
			return this._consumableN2O;
		}
		set
		{
			this._consumableN2O = value;
		}
	}

	public override void SerialiseToJson(JsonDict jsonDict)
	{
		jsonDict.Set("ca", this._consumablePRAgent);
		jsonDict.Set("ce", this._consumableEngine);
		jsonDict.Set("ct", this._consumableTyre);
		jsonDict.Set("cn", this._consumableN2O);
	}

	public override void SerialiseFromJson(JsonDict jsonDict)
	{
		jsonDict.TryGetValue("ca", out this._consumablePRAgent);
		jsonDict.TryGetValue("ce", out this._consumableEngine);
		jsonDict.TryGetValue("ct", out this._consumableTyre);
		jsonDict.TryGetValue("cn", out this._consumableN2O);
	}
}
