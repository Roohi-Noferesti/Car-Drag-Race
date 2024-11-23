using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using LitJson;
using UnityEngine;

public class JsonDict
{
    private Dictionary<string, object> dict = new Dictionary<string, object>();

	private string floatFormatString = "{0:0.000}";

	public string FloatFormatString
	{
		get
		{
			return this.floatFormatString;
		}
		set
		{
			this.floatFormatString = value;
		}
	}

	public Dictionary<string, object>.KeyCollection Keys
	{
		get
		{
			return this.dict.Keys;
		}
	}

	public bool ContainsKey(string key)
	{
		return this.dict.ContainsKey(key);
	}

	public bool Remove(string key)
	{
		return this.dict.Remove(key);
	}

	public bool Read(string json)
	{
		if (string.IsNullOrEmpty(json))
			return false;

		if (json[0] != '{' && json[0] != '[')
			return false;
		
		JsonReader jsonReader = JsonConverter.Reader(json);
		return jsonReader.Read() && this.Read(jsonReader);
	}

	public bool Read(JsonReader reader)
	{
		if (reader.Token != JsonToken.ObjectStart)
		{
			return false;
		}
		while (reader.Read())
		{
			if (reader.Token == JsonToken.ObjectEnd)
			{
				return true;
			}
			if (reader.Token != JsonToken.PropertyName)
			{
				return false;
			}
			string text = reader.Value as string;
			if (text == null)
			{
				return false;
			}
			if (!reader.Read())
			{
				return false;
			}
			if (reader.Token >= JsonToken.Int && reader.Token <= JsonToken.Null)
			{
				this.dict[text] = reader.Value;
			}
			else if (reader.Token == JsonToken.ObjectStart)
			{
				JsonDict jsonDict = new JsonDict();
				if (jsonDict.Read(reader))
				{
					this.dict[text] = jsonDict;
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
					this.dict[text] = jsonList;
				}
			}
		}
		return false;
	}

	public void Write(TextWriter writer)
	{
		writer.Write("{");
		int num = 0;
		int count = this.dict.Count;
		foreach (KeyValuePair<string, object> current in this.dict)
		{
			writer.Write("\"{0}\":", current.Key);
			if (current.Value == null)
			{
				writer.Write("null");
			}
			else if (current.Value is JsonDict)
			{
				JsonDict jsonDict = current.Value as JsonDict;
				jsonDict.Write(writer);
			}
			else if (current.Value is JsonList)
			{
				JsonList jsonList = current.Value as JsonList;
				jsonList.Write(writer);
			}
			else if (current.Value is string)
			{
				JsonStringWriter.Write(writer, current.Value.ToString());
			}
			else if (current.Value is bool)
			{
				writer.Write((!(bool)current.Value) ? "false" : "true");
			}
			else if (current.Value is float)
			{
				writer.Write(string.Format(CultureInfo.InvariantCulture, this.floatFormatString, new object[]
				{
					(float)current.Value
				}));
			}
			else
			{
				writer.Write(current.Value.ToString());
			}
			if (++num < count)
			{
				writer.Write(",");
			}
		}
		writer.Write("}");
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringWriter writer = new StringWriter(stringBuilder);
		this.Write(writer);
		return stringBuilder.ToString();
	}

	public bool Exists(string key)
	{
		object obj;
		return this.dict.TryGetValue(key, out obj);
	}

	public string GetString(string key)
	{
		string result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out string value)
	{
		object obj;
		if (this.dict.TryGetValue(key, out obj) && (obj is string || obj == null))
		{
			value = (string)obj;
			return true;
		}
		value = string.Empty;
		return false;
	}

	public void TryGetValue(string key, out string value, string defaultValue)
	{
		string text;
		if (this.TryGetValue(key, out text))
		{
			value = text;
		}
		else
		{
			value = defaultValue;
		}
	}

	public int GetInt(string key)
	{
		int result;
		this.TryGetValue(key, out result);
		return result;
	}

    public long GetLong(string key)
    {
        long result;
        this.TryGetValue(key, out result);
        return result;
    }

    public ulong GetUlong(string key)
    {
        ulong result;
        this.TryGetValue(key, out result);
        return result;
    }

	public bool TryGetValue(string key, out int value)
	{
		object o;
		if (this.dict.TryGetValue(key, out o))
		{
			try
			{
				value = this.ConvertGenericObject<int>(o);
				return true;
			}
			catch
			{
			}
		}
		value = 0;
		return false;
	}

	public void TryGetValue(string key, out int value, int defaultValue)
	{
		int num;
		if (this.TryGetValue(key, out num))
		{
			value = num;
		}
		else
		{
			value = defaultValue;
		}
	}

	public bool TryGetValue(string key, out long value)
	{
		object o;
		if (this.dict.TryGetValue(key, out o))
		{
			try
			{
				value = this.ConvertGenericObject<long>(o);
				return true;
			}
			catch
			{
			}
		}
		value = 0L;
		return false;
	}

    public bool TryGetValue(string key, out ulong value)
    {
        object obj;
        if (this.dict.TryGetValue(key, out obj))
        {
            if (obj is ulong || obj is long || obj is int)
            {
                value = Convert.ToUInt64(obj);
                return true;
            }
            if (obj != null)
            {
            }
        }
        value = 0uL;
        return false;
    }

	public float GetFloat(string key)
	{
		float result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out float value)
	{
		object obj;
		if (this.dict.TryGetValue(key, out obj))
		{
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
			if (obj is long)
			{
				value = (float)((long)obj);
				return true;
			}
			if (obj is int)
			{
				value = (float)((int)obj);
				return true;
			}
		}
		value = 0f;
		return false;
	}

	public bool GetBool(string key)
	{
		bool result;
		this.TryGetValue(key, out result);
		return result;
	}

    public bool GetBool(string key, bool defaultValue = false)
    {
        bool result;
        this.TryGetValue(key, out result, defaultValue);
        return result;
    }

	public bool TryGetValue(string key, out bool value)
	{
		object obj;
		if (this.dict.TryGetValue(key, out obj) && obj is bool)
		{
			value = (bool)obj;
			return true;
		}
		value = false;
		return false;
	}

	public void TryGetValue(string key, out bool value, bool defaultValue)
	{
		bool flag;
		if (this.TryGetValue(key, out flag))
		{
			value = flag;
		}
		else
		{
			value = defaultValue;
		}
	}

	public DateTime GetDateTime(string key)
	{
		DateTime result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out DateTime value)
	{
		object obj;
		if (this.dict.TryGetValue(key, out obj) && (obj is string || obj == null))
		{
			value = GetDateTimeFromString((string)obj);
			return true;
		}
		value = default(DateTime);
		return false;
	}

	public void TryGetValue(string key, out DateTime value, DateTime defaultValue)
	{
		DateTime dateTime;
		if (this.TryGetValue(key, out dateTime))
		{
			value = dateTime;
		}
		else
		{
			value = defaultValue;
		}
	}

	public Color GetColor(string key)
	{
		Color result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out Color value)
	{
		object obj;
		if (this.dict.TryGetValue(key, out obj) && obj is JsonList)
		{
			JsonList jsonList = (JsonList)obj;
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

	public List<int> GetIntList(string key)
	{
		List<int> result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out List<int> value)
	{
		return this.TryGetList<int>(key, out value);
	}

	public void TryGetValue(string key, out List<int> value, List<int> defaultValue)
	{
		List<int> list;
		if (this.TryGetList<int>(key, out list))
		{
			value = list;
		}
		else
		{
			value = new List<int>(defaultValue);
		}
	}

	public List<string> GetStringList(string key)
	{
		List<string> result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out List<string> value)
	{
		return this.TryGetList<string>(key, out value);
	}

	public void TryGetValue(string key, out List<string> value, List<string> defaultValue)
	{
		List<string> list;
		if (this.TryGetList<string>(key, out list))
		{
			value = list;
		}
		else
		{
			value = new List<string>(defaultValue);
		}
	}

	public List<float> GetFloatList(string key)
	{
		List<float> result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out List<float> value)
	{
		return this.TryGetList<float>(key, out value);
	}

	public List<bool> GetBoolList(string key)
	{
		List<bool> result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out List<bool> value)
	{
		return this.TryGetList<bool>(key, out value);
	}

	public int[] GetIntArray(string key)
	{
		int[] result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out int[] value)
	{
		return this.TryGetArray<int>(key, out value);
	}

    public bool[] GetBoolArray(string key)
    {
        bool[] result;
        this.TryGetValue(key, out result);
        return result;
    }

	public string[] GetStringArray(string key)
	{
		string[] result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out string[] value)
	{
		return this.TryGetArray<string>(key, out value);
	}

	public float[] GetFloatArray(string key)
	{
		float[] result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out float[] value)
	{
		return this.TryGetArray<float>(key, out value);
	}

	public bool TryGetValue(string key, out bool[] value)
	{
		return this.TryGetArray<bool>(key, out value);
	}

	public JsonDict GetJsonDict(string key)
	{
		JsonDict result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out JsonDict value)
	{
		object obj;
		if (this.dict.TryGetValue(key, out obj))
		{
			value = (JsonDict)obj;
			return true;
		}
		value = null;
		return false;
	}

	public JsonList GetJsonList(string key)
	{
		JsonList result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out JsonList value)
	{
		object obj;
		if (this.dict.TryGetValue(key, out obj))
		{
		    if (obj is JsonList)
		    {
		        value = (JsonList) obj;
		        return true;
		    }
		    else
		    {
                //Debug.Log(key+" is not json list");
		    }
		}
		value = null;
		return false;
	}

	public List<DateTime> GetDateTimeList(string key)
	{
		List<DateTime> result;
		this.TryGetValue(key, out result);
		return result;
	}

	public bool TryGetValue(string key, out List<DateTime> value)
	{
		JsonList jsonList;
		if (this.TryGetValue(key, out jsonList))
		{
			value = new List<DateTime>();
			for (int i = 0; i < jsonList.Count; i++)
			{
				DateTime item;
				jsonList.TryGetValue(i, out item);
				value.Add(item);
			}
			return true;
		}
		value = null;
		return false;
	}

	public T GetEnum<T>(string key)
	{
		T result;
		this.TryGetEnum<T>(key, out result);
		return result;
	}

	public bool TryGetEnum<T>(string key, out T value)
	{
		object obj = null;
		if (this.dict.TryGetValue(key, out obj) && obj is string)
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

	public List<T> GetEnumList<T>(string key)
	{
		List<T> result;
		this.TryGetEnum<List<T>>(key, out result);
		return result;
	}

	public bool TryGetEnumList<T>(string key, out List<T> value)
	{
		JsonList jsonList;
		if (this.TryGetValue(key, out jsonList))
		{
			value = new List<T>();
			for (int i = 0; i < jsonList.Count; i++)
			{
				T item;
				jsonList.TryGetEnum<T>(i, out item);
				value.Add(item);
			}
			return true;
		}
		value = null;
		return false;
	}

	public T GetObject<T>(string key, GetObjectDelegate<T> callback) where T : new()
	{
		T result;
		this.TryGetObject<T>(key, out result, callback);
		return result;
	}

	public bool TryGetObject<T>(string key, out T value, GetObjectDelegate<T> callback) where T : new()
	{
		object obj = null;
		if (this.dict.TryGetValue(key, out obj) && obj != null)
		{
			value = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
			callback((JsonDict)obj, ref value);
			return true;
		}
		value = default(T);
		return false;
	}

	public List<T> GetObjectList<T>(string key, GetObjectDelegate<T> callback) where T : new()
	{
		List<T> result;
		this.TryGetObjectList<T>(key, out result, callback);
		return result;
	}

	public bool TryGetObjectList<T>(string key, out List<T> value, GetObjectDelegate<T> callback) where T : new()
	{
		JsonList jsonList;
		if (this.TryGetValue(key, out jsonList))
		{
			value = new List<T>();
			for (int i = 0; i < jsonList.Count; i++)
			{
				T item = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
				JsonDict jsonDict;
				jsonList.TryGetValue(i, out jsonDict);
				if (jsonDict != null)
				{
					callback(jsonDict, ref item);
					value.Add(item);
				}
			}
			return true;
		}
		value = null;
		return false;
	}

	public T[] GetObjectArray<T>(string key, GetObjectDelegate<T> callback) where T : new()
	{
		T[] result;
		this.TryGetObjectArray<T>(key, out result, callback);
		return result;
	}

	public bool TryGetObjectArray<T>(string key, out T[] value, GetObjectDelegate<T> callback) where T : new()
	{
		JsonList jsonList;
		if (this.TryGetValue(key, out jsonList))
		{
			value = new T[jsonList.Count];
			for (int i = 0; i < jsonList.Count; i++)
			{
				T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
				JsonDict jsonDict;
				jsonList.TryGetValue(i, out jsonDict);
				callback(jsonDict, ref t);
				value[i] = t;
			}
			return true;
		}
		value = null;
		return false;
	}

	public Dictionary<U, T> GetDictFromObject<U, T>(string key)
	{
		Dictionary<U, T> result;
		this.TryGetDictFromObject<U, T>(key, out result);
		return result;
	}

	public bool TryGetDictFromObject<U, T>(string key, out Dictionary<U, T> val)
	{
		bool result = false;
		val = new Dictionary<U, T>();
		JsonDict jsonDict;
		if (this.TryGetValue(key, out jsonDict))
		{
			result = true;
			foreach (KeyValuePair<string, object> current in jsonDict.dict)
			{
				val[(U)((object)Convert.ChangeType(current.Key, typeof(U)))] = (T)((object)Convert.ChangeType(current.Value, typeof(T)));
			}
		}
		return result;
	}

	public Dictionary<U, T> GetObjectArrayFromDictValues<U, T>(string key, GetObjectDelegate<T> callback, List<U> keysInOrder) where T : new()
	{
		Dictionary<U, T> result;
		this.TryGetObjectArrayFromDictValues<U, T>(key, out result, callback, keysInOrder);
		return result;
	}

	public bool TryGetObjectArrayFromDictValues<U, T>(string key, out Dictionary<U, T> val, GetObjectDelegate<T> callback, List<U> keysInOrder) where T : new()
	{
		JsonList jsonList;
		if (this.TryGetValue(key, out jsonList) && jsonList.Count == keysInOrder.Count)
		{
			val = new Dictionary<U, T>();
			for (int i = 0; i < jsonList.Count; i++)
			{
				U key2 = keysInOrder[i];
				T value = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
				JsonDict jsonDict;
				jsonList.TryGetValue(i, out jsonDict);
				callback(jsonDict, ref value);
				val[key2] = value;
			}
			return true;
		}
		val = null;
		return false;
	}

	private bool TryGetList<T>(string key, out List<T> value)
	{
		JsonList jsonList;
		if (this.TryGetValue(key, out jsonList))
		{
			value = new List<T>();
			for (int i = 0; i < jsonList.Count; i++)
			{
				object o;
				jsonList.TryGetValue(i, out o);
				value.Add(this.ConvertGenericObject<T>(o));
			}
			return true;
		}
		value = null;
		return false;
	}

	private bool TryGetArray<T>(string key, out T[] value)
	{
		JsonList jsonList;
		if (this.TryGetValue(key, out jsonList))
		{
			value = new T[jsonList.Count];
			for (int i = 0; i < jsonList.Count; i++)
			{
				object o;
				jsonList.TryGetValue(i, out o);
				value[i] = this.ConvertGenericObject<T>(o);
			}
			return true;
		}
		value = null;
		return false;
	}

	private T ConvertGenericObject<T>(object o)
	{
		if (typeof(T) != o.GetType())
		{
			return (T)((object)Convert.ChangeType(o, typeof(T)));
		}
		return (T)((object)o);
	}

	public void Set(string key, JsonDict value)
	{
		this.dict[key] = value;
	}

	public void Set(string key, JsonList value)
	{
		this.dict[key] = value;
	}

	public void Set(string key, int value)
	{
		this.dict[key] = value;
	}

	public void Set(string key, long value)
	{
		this.dict[key] = value;
	}

	public void Set(string key, float value)
	{
		this.dict[key] = value;
	}

	public void Set(string key, bool value)
	{
		this.dict[key] = value;
	}

	public void Set(string key, string value)
	{
		this.dict[key] = value;
	}

	public void Set(string key, DateTime value)
	{
		this.dict[key] = GetStringFromDateTime(value);
	}

	public void Set(string key, Color value)
	{
		JsonList jsonList = new JsonList();
		jsonList.Add(value.r);
		jsonList.Add(value.g);
		jsonList.Add(value.b);
		jsonList.Add(value.a);
		this.dict[key] = jsonList;
	}

	public void Set(string key, List<DateTime> value)
	{
		JsonList jsonList = new JsonList();
		foreach (DateTime current in value)
		{
			jsonList.Add(GetStringFromDateTime(current));
		}
		this.dict[key] = jsonList;
	}

	public void SetEnum<T>(string key, T value)
	{
		if (value is Enum)
		{
			this.dict[key] = value.ToString();
		}
	}

	public void SetEnumList<T>(string key, List<T> value)
	{
		JsonList jsonList = new JsonList();
		foreach (T current in value)
		{
			jsonList.AddEnum<T>(current);
		}
		this.dict[key] = jsonList;
	}

	public void Set(string key, List<int> value)
	{
		this.SetList<int>(key, value);
	}

	public void Set(string key, List<float> value)
	{
		this.SetList<float>(key, value);
	}

	public void Set(string key, List<bool> value)
	{
		this.SetList<bool>(key, value);
	}

	public void Set(string key, List<string> value)
	{
		this.SetList<string>(key, value);
	}

	public void Set(string key, int[] value)
	{
		this.SetArray<int>(key, value);
	}

	public void Set(string key, float[] value)
	{
		this.SetArray<float>(key, value);
	}

	public void Set(string key, bool[] value)
	{
		this.SetArray<bool>(key, value);
	}

	public void Set(string key, string[] value)
	{
		this.SetArray<string>(key, value);
	}

	public void SetObject<T>(string key, T value, SetObjectDelegate<T> callback)
	{
		JsonDict value2 = new JsonDict();
		callback(value, ref value2);
		this.dict[key] = value2;
	}

	public void SetObjectList<T>(string key, List<T> value, SetObjectDelegate<T> callback)
	{
		JsonList jsonList = new JsonList();
		foreach (T current in value)
		{
			JsonDict value2 = new JsonDict();
			callback(current, ref value2);
			jsonList.Add(value2);
		}
		this.dict[key] = jsonList;
	}

	public void SetObjectArray<T>(string key, T[] value, SetObjectDelegate<T> callback)
	{
		JsonList jsonList = new JsonList();
		for (int i = 0; i < value.Length; i++)
		{
			T theObject = value[i];
			JsonDict value2 = new JsonDict();
			callback(theObject, ref value2);
			jsonList.Add(value2);
		}
		this.dict[key] = jsonList;
	}

	public void SetObjectArrayFromDictValues<U, T>(string key, Dictionary<U, T> sourceDict, SetObjectDelegate<T> callback, List<U> keysInOrder)
	{
		JsonList jsonList = new JsonList();
		foreach (U current in keysInOrder)
		{
			T theObject = sourceDict[current];
			JsonDict value = new JsonDict();
			callback(theObject, ref value);
			jsonList.Add(value);
		}
		this.dict[key] = jsonList;
	}

	private void SetList<T>(string key, List<T> value)
	{
		JsonList jsonList = new JsonList();
		foreach (T current in value)
		{
			jsonList.Add(current);
		}
		this.dict[key] = jsonList;
	}

	private void SetArray<T>(string key, T[] value)
	{
		JsonList jsonList = new JsonList();
		for (int i = 0; i < value.Length; i++)
		{
			T t = value[i];
			jsonList.Add(t);
		}
		this.dict[key] = jsonList;
	}

	public static string GetStringFromDateTime(DateTime dateTime)
	{
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		return dateTime.ToString("MM/dd/yyyy HH:mm:ss", invariantCulture);
	}

	public static DateTime GetDateTimeFromString(string dateTime)
	{
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		DateTime result;
		try
		{
			result = DateTime.ParseExact(dateTime, "MM/dd/yyyy HH:mm:ss", invariantCulture);
		}
		catch (FormatException)
		{
			result = DateTime.Now;
		}
		return result;
	}

    public bool TryGetValue(string name, out object value)
    {
        return dict.TryGetValue(name, out value);
    }
}
