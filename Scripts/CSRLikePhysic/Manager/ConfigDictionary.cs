using LitJson;
using System;
using System.Collections.Generic;
using System.Reflection;

public class ConfigDictionary
{
	public delegate void LoadConfigDataDelegate(ConfigDictionary config);

	public delegate void LoadObjectDelegate<T>(T loadedObj);

	private JsonDict Dictionary;

	public string Path
	{
		get;
		private set;
	}

	public Dictionary<string, object>.KeyCollection Keys
	{
		get
		{
			return this.Dictionary.Keys;
		}
	}

	public ConfigDictionary(JsonDict config, string path)
	{
		this.Dictionary = config;
		this.Path = path;
	}

	public static void Load(string configName, ConfigDictionary.LoadConfigDataDelegate callback)
	{
		//ConfigDictionary.LoadConfigDataDelegate wrappedCallback = delegate(ConfigDictionary config)
		//{
		//	try
		//	{
		//		callback(config);
		//	}
		//	catch (Exception var_0_11)
		//	{
		//	}
		//};
        //if (CSRNmgServicesManager.Instance != null && CSRNmgServicesManager.Instance.ConfigHandler != null)
        //{
        //    try
        //    {
        //        CSRNmgServicesManager.Instance.ConfigHandler.LoadConfigData(configName, delegate(JsonDict config)
        //        {
        //            if (config == null)
        //            {
        //                wrappedCallback(null);
        //                return;
        //            }
        //            ConfigDictionary configDictionary = new ConfigDictionary(config, configName + ".metadata");
        //            ConfigDictionary subDictionary = configDictionary.GetSubDictionary(configName);
        //            if (subDictionary != null)
        //            {
        //                wrappedCallback(subDictionary);
        //                return;
        //            }
        //        });
        //    }
        //    catch (Exception var_0_6B)
        //    {
        //    }
        //}
	}

	public static void LoadAndUpdate(string configName, ConfigDictionary.LoadConfigDataDelegate callback)
	{
        //Action<bool> action = delegate(bool restart)
        //{
        //    ConfigDictionary.Load(configName, callback);
        //};
        //action(false);
        //CSRNmgConfigHandler.OnMetadataUpdated += action;
	}

	public bool TryGetValue<T>(string key, out T value) where T : class
	{
	    //return this.Dictionary.TryGetValue<T>(key, out value);
	    value = null;
	    return false;
	}

    public static void LoadIntoObject<T>(string configName, ConfigDictionary.LoadObjectDelegate<T> callback)
	{
        //CSRNmgConfigHandler cSRNmgConfigHandler = (!(CSRNmgServicesManager.Instance == null)) ? CSRNmgServicesManager.Instance.ConfigHandler : null;
        //if (cSRNmgConfigHandler == null)
        //{
        //    return;
        //}
        //string text = cSRNmgConfigHandler.LoadRawConfigData(configName);
        //if (string.IsNullOrEmpty(text))
        //{
        //    return;
        //}
        //BackgroundJSONParser.Load<T>(text, configName, callback);
	}

	public bool ContainsKey(string key)
	{
		return this.Dictionary.ContainsKey(key);
	}

	public string PathToKey(string key)
	{
		return this.Path + "/" + key;
	}

	public string GetStringForKey(string key, string valueIfNotFound = null)
	{
		string text = valueIfNotFound;
		if (this.ContainsKey(key) && this.Dictionary.TryGetValue(key, out text) && text == null)
		{
			text = valueIfNotFound;
		}
		return text;
	}

	public bool GetBoolForKey(string key, bool valueIfNotFound = false)
	{
		bool result = valueIfNotFound;
		if (this.ContainsKey(key))
		{
			this.Dictionary.TryGetValue(key, out result);
		}
		return result;
	}

	public ConfigDictionary GetSubDictionary(string key)
	{
		if (this.Dictionary != null && this.Dictionary.ContainsKey(key))
		{
			JsonDict jsonDict = this.Dictionary.GetJsonDict(key);
			if (jsonDict != null)
			{
				return new ConfigDictionary(jsonDict, this.PathToKey(key));
			}
		}
		return null;
	}

	public bool ApplyOverrides<T>(T obj, HashSet<string> skipKeys = null)
	{
		if (this.Dictionary != null && obj != null)
		{
			Type type = obj.GetType();
			Dictionary<string, object>.KeyCollection keys = this.Dictionary.Keys;
			foreach (string current in keys)
			{
				if (skipKeys == null || !skipKeys.Contains(current))
				{
					object obj2 = null;
                    //this.Dictionary.TryGetValueAsObject(current, out obj2);
					try
					{
						PropertyInfo property = type.GetProperty(current);
						FieldInfo field = type.GetField(current);
						if (obj2 != null && obj2 is JsonDict)
						{
							ConfigDictionary configDictionary = new ConfigDictionary(obj2 as JsonDict, this.PathToKey(current));
							MethodInfo methodInfo = (property == null) ? null : property.GetGetMethod();
							if (methodInfo != null)
							{
								object obj3 = methodInfo.Invoke(obj, new object[0]);
								configDictionary.ApplyOverrides<object>(obj3, null);
							}
							else if (field != null)
							{
								configDictionary.ApplyOverrides<object>(field.GetValue(obj), null);
							}
						}
						else
						{
							if (property != null)
							{
								MethodInfo setMethod = property.GetSetMethod();
								if (setMethod != null)
								{
									setMethod.Invoke(obj, new object[]
									{
										obj2
									});
									continue;
								}
							}
							if (field != null)
							{
								field.SetValue(obj, obj2);
							}
						}
					}
					catch (Exception)// var_11_160)
					{
						if (obj2 != null)
						{
							//string text = obj2.GetType().ToString() + "(" + obj2.ToString() + ")";
						}
					}
				}
			}
			return true;
		}
		return false;
	}

	public static implicit operator JsonDict(ConfigDictionary dictionary)
	{
		if (dictionary != null)
		{
			return dictionary.Dictionary;
		}
		return null;
	}
}
