public class LitJsonReader : JsonReader
{
	private LitJson.JsonReader reader;

	public JsonToken Token
	{
		get
		{
			return (JsonToken)this.reader.Token;
		}
	}

	public object Value
	{
		get
		{
			return this.reader.Value;
		}
	}

	public LitJsonReader(string json)
	{
		this.reader = new LitJson.JsonReader(json);
	}

	public bool Read()
	{
		return this.reader.Read();
	}
}
