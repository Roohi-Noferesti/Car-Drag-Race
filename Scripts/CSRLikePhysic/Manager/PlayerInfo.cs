using System;
using System.Collections.Generic;

public abstract class PlayerInfo
{
	private const int _defaultMaxWidthDisplayName = 80;

	private const string _defaultTruncatedEnding = "...";

	protected string _csrUserName;

	protected Dictionary<Type, PlayerInfoComponent> Components = new Dictionary<Type, PlayerInfoComponent>();

	protected PersonaComponent persona;

	public string CsrUserName
	{
		get
		{
			return this._csrUserName;
		}
	}

	public int CsrUserID
	{
		get
		{
			if (this.CsrUserName != null && this.CsrUserName.Length > 4)
			{
				string s = this.CsrUserName.Substring(4, this.CsrUserName.Length - 4);
				int result = 0;
				if (int.TryParse(s, out result))
				{
					return result;
				}
			}
			return 0;
		}
	}

	public string DisplayName
	{
		get
		{
			return NameValidater.GetDisplayableName(this.Persona.GetDisplayName(), this._csrUserName);
		}
	}

	public bool IsStar
	{
		get
		{
			return this is StarTimePlayerInfo;
		}
	}

	public PersonaComponent Persona
	{
		get
		{
			return this.persona;
		}
	}

	public PlayerInfo(PersonaComponent persona)
	{
		this.persona = persona;
	}

	public string GetLengthConstrainedDisplayName(int max = 80, string endString = "...")
	{
		string text = this.DisplayName;
		if (text.Length > max)
		{
			text = text.Substring(0, max - 1) + endString;
		}
		return text;
	}

	public T AddComponent<T>()
	{
		if (typeof(T).IsSubclassOf(typeof(PersonaComponent)))
		{
			return default(T);
		}
		if (this.Components.ContainsKey(typeof(T)))
		{
			return this.GetComponent<T>();
		}
		PlayerInfoComponent value = Activator.CreateInstance(typeof(T)) as PlayerInfoComponent;
		this.Components.Add(typeof(T), value);
		return (T)((object)Convert.ChangeType(value, typeof(T)));
	}

	public T GetComponent<T>()
	{
		PlayerInfoComponent value = null;
		if (this.Components.TryGetValue(typeof(T), out value))
		{
			return (T)((object)Convert.ChangeType(value, typeof(T)));
		}
		return default(T);
	}

	public bool DoesComponentExist<T>()
	{
		return this.Components.ContainsKey(typeof(T));
	}

	public static int CompareCarsByPPIndexDescending(CarGarageInstance a, CarGarageInstance b)
	{
		return b.CurrentPPIndex - a.CurrentPPIndex;
	}

    public void SerialiseToJson(JsonDict jsonDict)
    {
        jsonDict.Set("un", this._csrUserName);
        this.Persona.SerialiseToJson(jsonDict);
        foreach (PlayerInfoComponent current in this.Components.Values)
        {
            current.SerialiseToJson(jsonDict);
        }
    }

    public void SerialiseFromJson(JsonDict jsonDict)
    {
        jsonDict.TryGetValue("un", out this._csrUserName);
        this.Persona.SerialiseFromJson(jsonDict);
        foreach (PlayerInfoComponent current in this.Components.Values)
        {
            current.SerialiseFromJson(jsonDict);
        }
    }
}
