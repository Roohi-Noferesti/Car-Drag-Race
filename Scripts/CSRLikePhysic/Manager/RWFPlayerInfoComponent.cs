public class RWFPlayerInfoComponent : PlayerInfoComponent
{
	public float _bestTimeInCurrentCar;

	public bool HasSetTimeInCurrentCar
	{
		get
		{
			return this._bestTimeInCurrentCar > 0f;
		}
	}

	public float GetBestTimeInCurrentCar
	{
		get
		{
			return this._bestTimeInCurrentCar;
		}
	}

    public override void SerialiseToJson(JsonDict jsonDict)
    {
        jsonDict.Set("mt", this._bestTimeInCurrentCar);
    }

    public override void SerialiseFromJson(JsonDict jsonDict)
    {
        jsonDict.TryGetValue("mt", out this._bestTimeInCurrentCar);
    }
}
