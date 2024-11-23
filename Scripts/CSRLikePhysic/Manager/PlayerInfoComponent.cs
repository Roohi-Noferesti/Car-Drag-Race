public abstract class PlayerInfoComponent
{
    public abstract void SerialiseToJson(JsonDict jsonDict);

    public abstract void SerialiseFromJson(JsonDict jsonDict);
}
