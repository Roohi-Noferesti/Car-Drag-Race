using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using LitJson;
using UnityEngine;

public class JsonList
{
	private List<object> list = new List<object>();

	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	public void Clear()
	{
		this.list.Clear();
	}

	public bool Read(string json)
	{
		JsonReader jsonReader = JsonConverter.Reader(json);
		return jsonReader.Read() && this.Read(jsonReader);
	}

	public bool Read(JsonReader reader)
	{
		if (reader.Token != JsonToken.ArrayStart)
		{
			return false;
		}
		while (reader.Read())
		{
			if (reader.Token == JsonToken.ArrayEnd)
			{
				return true;
			}
			if (reader.Token >= JsonToken.Int && reader.Token <= JsonToken.Null)
			{
				this.list.Add(reader.Value);
			}
			else if (reader.Token == JsonToken.ObjectStart)
			{
				JsonDict jsonDict = new JsonDict();
				if (jsonDict.Read(reader))
				{
					this.list.Add(jsonDict);
				}
			}
			else
			{
				if (reader.Token != JsonToken.ArrayStart)
				{
					return false;
				}
				JsonList jsonList = new JsonList();
				if (jsonList.Read(reader))
				{
					this.list.Add(jsonList);
				}
			}
		}
		return false;
	}

	public void Write(TextWriter writer)
	{
		writer.Write("[");
		int num = 0;
		int count = this.list.Count;
		foreach (object current in this.list)
		{
			if (current == null)
			{
				writer.Write("null");
			}
			else if (current is JsonDict)
			{
				JsonDict jsonDict = current as JsonDict;
				jsonDict.Write(writer);
			}
			else if (current is JsonList)
			{
				JsonList jsonList = current as JsonList;
				jsonList.Write(writer);
			}
			else if (current is string)
			{
				JsonStringWriter.Write(writer, current.ToString());
			}
			else if (current is bool)
			{
				writer.Write((!(bool)current) ? "false" : "true");
			}
			else if (current is float)
			{
				writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:0.000}", new object[]
				{
					(float)current
				}));
			}
			else
			{
				writer.Write(current.ToString());
			}
			if (++num < count)
			{
				writer.Write(",");
			}
		}
		writer.Write("]");
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringWriter writer = new StringWriter(stringBuilder);
		this.Write(writer);
		return stringBuilder.ToString();
	}

	public void Add(int value)
	{
		this.list.Add(value);
	}

	public void Add(string value)
	{
		this.list.Add(value);
	}

	public void Add(float value)
	{
		this.list.Add(value);
	}

	public void Add(bool value)
	{
		this.list.Add(value);
	}

	public void Add(DateTime value)
	{
		this.list.Add(JsonDict.GetStringFromDateTime(value));
	}

	public void Add(Color value)
	{
		JsonList jsonList = new JsonList();
		jsonList.Add(value.r);
		jsonList.Add(value.g);
		jsonList.Add(value.b);
		jsonList.Add(value.a);
		this.list.Add(jsonList);
	}

	public void AddEnum<T>(T value)
	{
		if (value is Enum)
		{
			this.list.Add(value.ToString());
		}
	}

	public void Add(JsonDict value)
	{
		this.list.Add(value);
	}

	public void Add(JsonList value)
	{
		this.list.Add(value);
	}

	public void Add(object value)
	{
		this.list.Add(value);
	}

	public int GetInt(int row)
	{
		int result;
		this.TryGetValue(row, out result);
		return result;
	}

	public bool TryGetValue(int row, out int value)
	{
		object obj = this.list[row];
		if (obj is int)
		{
			value = (int)obj;
			return true;
		}
		if (obj is long && (long)obj <= 2147483647L && (long)obj >= -2147483648L)
		{
			value = Convert.ToInt32(obj);
			return true;
		}
		value = 0;
		return false;
	}

	public float GetFloat(int row)
	{
		float result;
		this.TryGetValue(row, out result);
		return result;
	}

	public bool TryGetValue(int row, out float value)
	{
		object obj = this.list[row];
		if (obj is float)
		{
			value = (float)obj;
			return true;
		}
		if (obj is double)
		{
			value = (float)Convert.ToDouble(obj);
			return true;
		}
		value = 0f;
		return false;
	}

	public string GetString(int row)
	{
		string result;
		this.TryGetValue(row, out result);
		return result;
	}

	public bool TryGetValue(int row, out string value)
	{
		object obj = this.list[row];
		if (obj is string)
		{
			value = (string)obj;
			return true;
		}
		value = string.Empty;
		return false;
	}

	public bool GetBool(int row)
	{
		bool result;
		this.TryGetValue(row, out result);
		return result;
	}

	public bool TryGetValue(int row, out bool value)
	{
		object obj = this.list[row];
		if (obj is bool)
		{
			value = (bool)obj;
			return true;
		}
		value = false;
		return false;
	}

	public DateTime GetDateTime(int row)
	{
		DateTime result;
		this.TryGetValue(row, out result);
		return result;
	}

	public bool TryGetValue(int row, out DateTime value)
	{
		value = JsonDict.GetDateTimeFromString((string)this.list[row]);
		return true;
	}

	public Color GetColor(int row)
	{
		Color result;
		this.TryGetValue(row, out result);
		return result;
	}

	public bool TryGetValue(int row, out Color value)
	{
		if (this.list[row] is JsonList)
		{
			JsonList jsonList = (JsonList)this.list[row];
			if (jsonList.Count == 4)
			{
				value = default(Color);
				jsonList.TryGetValue(0, out value.r);
				jsonList.TryGetValue(1, out value.g);
				jsonList.TryGetValue(2, out value.b);
				jsonList.TryGetValue(3, out value.a);
				return true;
			}
		}
		value = default(Color);
		return false;
	}

	public JsonDict GetJsonDict(int row)
	{
		JsonDict result;
		this.TryGetValue(row, out result);
		return result;
	}

	public bool TryGetValue(int row, out JsonDict value)
	{
		object obj = this.list[row];
		if (obj is JsonDict)
		{
			value = (JsonDict)this.list[row];
			return true;
		}
		value = null;
		return false;
	}

	public JsonList GetJsonList(int row)
	{
		JsonList result;
		this.TryGetValue(row, out result);
		return result;
	}

	public bool TryGetValue(int row, out JsonList value)
	{
		object obj = this.list[row];
		if (obj is JsonList)
		{
			value = (JsonList)this.list[row];
			return true;
		}
		value = null;
		return false;
	}

	public object GetObject(int row)
	{
		object result;
		this.TryGetValue(row, out result);
		return result;
	}

	public bool TryGetValue(int row, out object value)
	{
		value = this.list[row];
		return true;
	}

	public void Set(int row, JsonDict value)
	{
		this.list[row] = value;
	}

	public T GetEnum<T>(int row)
	{
		T result;
		this.TryGetEnum<T>(row, out result);
		return result;
	}

	public bool TryGetEnum<T>(int row, out T value)
	{
		object obj = this.list[row];
		if (obj is string)
		{
			if (Enum.IsDefined(typeof(T), (string)obj))
			{
				value = (T)((object)Enum.Parse(typeof(T), (string)obj));
				return true;
			}
		}
		value = default(T);
		return false;
	}
}
