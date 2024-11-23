public interface JsonReader
{
	JsonToken Token
	{
		get;
	}

	object Value
	{
		get;
	}

	bool Read();
}
