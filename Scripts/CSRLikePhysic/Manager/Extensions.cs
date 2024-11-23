using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using I2.Loc;
using UnityEngine;

public static class Extensions_2
{
	private const string INDENT_STRING = "    ";

	public static char[] specialHTMLCharactersExcludeList = new char[]
	{
		'[',
		']',
		'<',
		'>'
	};

	public static GameObject FindFromRoot(this GameObject gameobject, string name)
	{
		Transform transform = gameobject.transform.root.FindChildRecursively(name);
		if (transform != null)
		{
			return transform.gameObject;
		}
		return null;
	}

	public static Transform FindChildRecursively(this Transform transform, string name)
	{
		foreach (Transform transform2 in transform)
		{
			if (transform2.gameObject.name == name)
			{
				Transform result = transform2;
				return result;
			}
			Transform transform3 = transform2.FindChildRecursively(name);
			if (transform3 != null)
			{
				Transform result = transform3;
				return result;
			}
		}
		return null;
	}

	public static T GetComponentInChildren<T>(this GameObject gameObject, bool includeInactive) where T : Component
	{
		if (includeInactive || gameObject.activeInHierarchy)
		{
			T t = gameObject.GetComponent<T>();
			if (t != null)
			{
				return t;
			}
			foreach (Transform transform in gameObject.transform)
			{
				t = transform.gameObject.GetComponentInChildren<T>(includeInactive);
				if (t != null)
				{
					return t;
				}
			}
		}
		return (T)((object)null);
	}

	public static T GetComponentInChildren<T>(this Transform transform, bool includeInactive) where T : Component
	{
        return transform.gameObject.GetComponentInChildren<T>(includeInactive);
	}

	public static T GetComponentInChildren<T>(this Component component, bool includeInactive) where T : Component
	{
        return component.gameObject.GetComponentInChildren<T>(includeInactive);
	}

	public static T GetComponentInParent<T>(this GameObject gameObject, bool includeInactive) where T : Component
	{
		if (includeInactive || gameObject.activeInHierarchy)
		{
			T component = gameObject.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			if (gameObject.transform.parent != null)
			{
                return gameObject.transform.parent.gameObject.GetComponentInParent<T>(includeInactive);
			}
		}
		return (T)((object)null);
	}

	public static T GetComponentInParent<T>(this Transform transform, bool includeInactive) where T : Component
	{
        return transform.gameObject.GetComponentInParent<T>(includeInactive);
	}

	public static T GetComponentInParent<T>(this Component component, bool includeInactive) where T : Component
	{
        return component.gameObject.GetComponentInParent<T>(includeInactive);
	}

	public static string Localise(this string textId)
	{
		return LocalizationManager.GetTranslation(textId);
	}

	public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
	{
		foreach (T current in ie)
		{
			action(current);
		}
	}

	public static bool Contains<T>(this IEnumerable<T> ie, T item)
	{
		foreach (T current in ie)
		{
			if (current.Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	public static string Remove_HTMLTagsBrackets(this string input)
	{
		string text = input;
		char[] array = Extensions_2.specialHTMLCharactersExcludeList;
		for (int i = 0; i < array.Length; i++)
		{
			char value = array[i];
			int startIndex;
			while ((startIndex = text.IndexOf(value)) > -1)
			{
				text = text.Remove(startIndex, 1);
			}
		}
		return text;
	}

	public static Vector3 ToVector3(this Vector2 v)
	{
		return new Vector3(v.x, v.y, 0f);
	}

    //public static Dictionary<T, U> GetDictionary<T, U>(this JsonDict dict, string key, GetObjectDelegate<CSR2KeyValuePair<T, U>> parseDictEntryCallback)
    //{
    //    Dictionary<T, U> dictionary = new Dictionary<T, U>();
    //    List<CSR2KeyValuePair<T, U>> objectList = dict.GetObjectList<CSR2KeyValuePair<T, U>>(key, parseDictEntryCallback);
    //    foreach (CSR2KeyValuePair<T, U> current in objectList)
    //    {
    //        dictionary.Add(current.Key, current.Value);
    //    }
    //    return dictionary;
    //}

	public static string FormatJson(this string str)
	{
		int num = 0;
		bool flag = false;
		StringBuilder sb = new StringBuilder();
		int i = 0;
		while (i < str.Length)
		{
			char c = str[i];
			char c2 = c;
			switch (c2)
			{
			case '[':
				goto IL_75;
			case '\\':
				//IL_3F:
				switch (c2)
				{
				case '{':
					goto IL_75;
				case '|':
					//IL_55:
					if (c2 == '"')
					{
						sb.Append(c);
						bool flag2 = false;
						int num2 = i;
						while (num2 > 0 && str[--num2] == '\\')
						{
							flag2 = !flag2;
						}
						if (!flag2)
						{
							flag = !flag;
						}
						goto IL_1C5;
					}
					if (c2 == ',')
					{
						sb.Append(c);
						if (!flag)
						{
							sb.AppendLine();
							Enumerable.Range(0, num).ForEach(delegate(int item)
							{
								sb.Append("    ");
							});
						}
						goto IL_1C5;
					}
					if (c2 != ':')
					{
						sb.Append(c);
						goto IL_1C5;
					}
					sb.Append(c);
					if (!flag)
					{
						sb.Append(" ");
					}
					goto IL_1C5;
				case '}':
					goto IL_B8;
				}
			        break;
			    //goto IL_55;
			case ']':
				goto IL_B8;
			}
			//goto IL_3F;
			IL_1C5:
			i++;
			continue;
			IL_75:
			sb.Append(c);
			if (!flag)
			{
				sb.AppendLine();
				Enumerable.Range(0, ++num).ForEach(delegate(int item)
				{
					sb.Append("    ");
				});
			}
			goto IL_1C5;
			IL_B8:
			if (!flag)
			{
				sb.AppendLine();
				Enumerable.Range(0, --num).ForEach(delegate(int item)
				{
					sb.Append("    ");
				});
			}
			sb.Append(c);
			goto IL_1C5;
		}
		return sb.ToString();
	}

	public static string GetTimeFormattedLocalisedString(this TimeSpan time, int max_slots = 2)
	{
		string result = string.Empty;
		if (max_slots > 3)
		{
			if (time.Days > 0)
			{
				result = string.Format("TEXT_UNITS_TIME_DAYS_HOURS_MINUTES_AND_SECONDS".Localise(), new object[]
				{
					time.Days,
					time.Hours,
					time.Minutes,
					time.Seconds
				});
			}
			else if (time.Hours > 0)
			{
				result = string.Format("TEXT_UNITS_TIME_HOURS_MINUTES_AND_SECONDS".Localise(), time.Hours, time.Minutes, time.Seconds);
			}
			else if (time.Minutes > 0)
			{
				result = string.Format("TEXT_UNITS_TIME_MINUTES_AND_SECONDS".Localise(), time.Minutes, time.Seconds);
			}
			else
			{
				result = string.Format("TEXT_UNITS_TIME_SECONDS".Localise(), time.Seconds);
			}
		}
		else if (max_slots == 3)
		{
			if (time.Days > 0)
			{
				result = string.Format("TEXT_UNITS_TIME_DAYS_HOURS_AND_MINUTES".Localise(), time.Days, time.Hours, time.Minutes);
			}
			else if (time.Hours > 0)
			{
				result = string.Format("TEXT_UNITS_TIME_HOURS_MINUTES_AND_SECONDS".Localise(), time.Hours, time.Minutes, time.Seconds);
			}
			else if (time.Minutes > 0)
			{
				result = string.Format("TEXT_UNITS_TIME_MINUTES_AND_SECONDS".Localise(), time.Minutes, time.Seconds);
			}
			else
			{
				result = string.Format("TEXT_UNITS_TIME_SECONDS".Localise(), time.Seconds);
			}
		}
		else if (max_slots == 1)
		{
			if (time.Days > 0)
			{
				result = string.Format("TEXT_UNITS_TIME_DAYS".Localise(), time.Days, time.Hours, time.Minutes);
			}
			else if (time.Hours > 0)
			{
				result = string.Format("TEXT_UNITS_TIME_HOURS_SHORT".Localise(), time.Hours, time.Minutes, time.Seconds);
			}
			else if (time.Minutes > 0)
			{
				result = string.Format("TEXT_UNITS_TIME_MINUTES_SHORT".Localise(), time.Minutes, time.Seconds);
			}
			else
			{
				result = string.Format("TEXT_UNITS_TIME_SECONDS".Localise(), time.Seconds);
			}
		}
		else if (time.Days > 0)
		{
			result = string.Format("TEXT_UNITS_TIME_DAYS_AND_HOURS".Localise(), time.Days, time.Hours);
		}
		else if (time.Hours > 0)
		{
			result = string.Format("TEXT_UNITS_TIME_HOURS_AND_MINUTES".Localise(), time.Hours, time.Minutes);
		}
		else if (time.Minutes > 0)
		{
			result = string.Format("TEXT_UNITS_TIME_MINUTES_AND_SECONDS".Localise(), time.Minutes, time.Seconds);
		}
		else
		{
			result = string.Format("TEXT_UNITS_TIME_SECONDS".Localise(), time.Seconds);
		}
		return result;
	}

    public static T AddMissingComponent<T>(this GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }

}
