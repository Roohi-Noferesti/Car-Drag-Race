public static class JsonConverter
{
	private static JsonConverterBase converter;

	static JsonConverter()
	{
		converter = new LitJsonConverter();
	}

	public static T DeserializeObject<T>(string json)
	{
		return converter.DeserializeObject<T>(json);
	}

	public static JsonData DeserializeObject(string json)
	{
		return converter.DeserializeObject(json);
	}

	public static string SerializeObject(object obj, bool writeDefaults = true)
	{
		return converter.SerializeObject(obj, writeDefaults);
	}

	public static string Prettify(string json, bool writeDefaults = true)
	{
		return converter.Prettify(json, writeDefaults);
	}

	public static string Strip(string json)
	{
		return converter.Strip(json);
	}

	public static JsonReader Reader(string json)
	{
		return converter.Reader(json);
	}
}
