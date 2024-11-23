using System;
using System.Text.RegularExpressions;

public abstract class JsonConverterBase
{
	protected abstract Type LibraryJsonException
	{
		get;
	}

	protected abstract T DeserializeObjectConverter<T>(string json);

	protected abstract JsonData DeserializeObjectConverter(string json);

	protected abstract string SerializeObjectConverter(object obj, bool writeDefaults = true);

	protected abstract string PrettifyConverter(string json, bool writeDefaults = true);

	public abstract JsonReader Reader(string json);

	public T DeserializeObject<T>(string json)
	{
		try
		{
			return this.DeserializeObjectConverter<T>(json);
		}
		catch (ApplicationException e)
		{
			this.CheckForJsonException(e);
		}
		return default(T);
	}

	public JsonData DeserializeObject(string json)
	{
		try
		{
			return this.DeserializeObjectConverter(json);
		}
		catch (ApplicationException e)
		{
			this.CheckForJsonException(e);
		}
		return null;
	}

	public string SerializeObject(object obj, bool writeDefaults = true)
	{
		try
		{
			string result;
			if (obj is JsonData)
			{
				JsonData jsonData = obj as JsonData;
				result = jsonData.Serialize();
				return result;
			}
			result = this.SerializeObjectConverter(obj, writeDefaults);
			return result;
		}
		catch (ApplicationException e)
		{
			this.CheckForJsonException(e);
		}
		return null;
	}

	public string Prettify(string json, bool writeDefaults = true)
	{
		try
		{
			string json2 = this.PrettifyConverter(json, writeDefaults);
			return this.UnescapeUnicodeChars(json2);
		}
		catch (ApplicationException e)
		{
			this.CheckForJsonException(e);
		}
		return null;
	}

	public string Strip(string json)
	{
		JsonData obj = this.DeserializeObject(json);
		string json2 = this.SerializeObject(obj, true);
		return this.UnescapeUnicodeChars(json2);
	}

	private void CheckForJsonException(ApplicationException e)
	{
		if (e.GetType() == this.LibraryJsonException)
		{
			throw new JsonException(e);
		}
		throw e;
	}

	protected string UnescapeUnicodeChars(string json)
	{
		return Regex.Replace(json, "\\\\u([0-9A-Fa-f]{4})", (Match m) => ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
	}
}
