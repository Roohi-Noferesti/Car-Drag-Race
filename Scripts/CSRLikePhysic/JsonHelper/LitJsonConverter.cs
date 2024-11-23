using System;
using System.Text;
using LitJson;

public class LitJsonConverter : JsonConverterBase
{
	protected override Type LibraryJsonException
	{
		get
		{
			return typeof(LitJson.JsonException);
		}
	}

	public LitJsonConverter()
	{
		JsonMapper.RegisterImporter<string, TimeSpan>((string x) => TimeSpan.Parse(x));
		JsonMapper.RegisterImporter<float, double>((float x) => System.Convert.ToSingle(x));
	}

	protected override T DeserializeObjectConverter<T>(string json)
	{
		return JsonMapper.ToObject<T>(json);
	}

	protected override JsonData DeserializeObjectConverter(string json)
	{
		LitJson.JsonData newData = JsonMapper.ToObject(json);
		return new LitJsonData(newData);
	}

	protected override string SerializeObjectConverter(object obj, bool writeDefaults = true)
	{
		return JsonMapper.ToJson(obj, writeDefaults);
	}

	protected override string PrettifyConverter(string json, bool writeDefaults = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		JsonWriter jsonWriter = new JsonWriter(stringBuilder);
		jsonWriter.PrettyPrint = true;
		LitJson.JsonData obj = JsonMapper.ToObject(json);
		JsonMapper.ToJson(obj, jsonWriter, writeDefaults);
		return stringBuilder.ToString();
	}

	public override JsonReader Reader(string json)
	{
		return new LitJsonReader(json);
	}
}
