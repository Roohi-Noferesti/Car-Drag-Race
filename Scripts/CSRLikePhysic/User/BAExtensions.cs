using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class BAExtensions
{
	public static void EnumerateEach<T>(this IEnumerable<T> ie, Action<T, int> action)
	{
		int num = 0;
		foreach (T current in ie)
		{
			action(current, num++);
		}
	}

	public static int ToIntOrDefault(this string s, int defval)
	{
		int num;
		bool flag = int.TryParse(s, out num);
		return (!flag) ? defval : num;
	}

	public static int GetCrossPlatformHashCode(this string s)
	{
		int num = 0;
		for (int i = 0; i < s.Length; i++)
		{
			char c = s[i];
			num = (num << 5) - num + (int)c;
		}
		return num;
	}

	public static T CopyComponent<T>(this Component component, T other) where T : Component
	{
		Type type = component.GetType();
		if (type != other.GetType())
		{
			throw new InvalidOperationException(string.Format("Type mismatch when copying components; {0} <- {1}", type, other.GetType()));
		}
		BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		PropertyInfo[] properties = type.GetProperties(bindingAttr);
		PropertyInfo[] array = properties;
		for (int i = 0; i < array.Length; i++)
		{
			PropertyInfo propertyInfo = array[i];
			if (propertyInfo.CanWrite)
			{
				propertyInfo.SetValue(component, propertyInfo.GetValue(other, null), null);
			}
		}
		FieldInfo[] fields = type.GetFields(bindingAttr);
		FieldInfo[] array2 = fields;
		for (int j = 0; j < array2.Length; j++)
		{
			FieldInfo fieldInfo = array2[j];
			fieldInfo.SetValue(component, fieldInfo.GetValue(other));
		}
		return component as T;
	}

	public static T AddComponent<T>(this GameObject go, T copyFrom) where T : Component
	{
		return (T)((object)go.AddComponent(copyFrom.GetType()).CopyComponent(copyFrom));
	}
}
