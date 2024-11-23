public class MechanicPlayerInfoComponent : PlayerInfoComponent
{
	private bool _mechanicEnabled;

	public bool MechanicEnabled
	{
		get
		{
			return this._mechanicEnabled;
		}
		set
		{
			this._mechanicEnabled = value;
		}
	}

    public override void SerialiseToJson(JsonDict jsonDict)
    {
        jsonDict.Set("me", this._mechanicEnabled);
    }

    public override void SerialiseFromJson(JsonDict jsonDict)
    {
        jsonDict.TryGetValue("me", out this._mechanicEnabled);
    }
}
