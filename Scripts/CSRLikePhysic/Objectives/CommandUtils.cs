using System;
using System.Collections.Generic;
using System.Reflection;

public static class CommandUtils
{
	private static Dictionary<Type, Dictionary<string, MethodInfo>> cache = new Dictionary<Type, Dictionary<string, MethodInfo>>();

	public static MethodInfo GetMethod(object targetObject, string methodName)
	{
		if (targetObject == null)
		{
			return null;
		}
		Type type = targetObject.GetType();
		if (!CommandUtils.cache.ContainsKey(type))
		{
			CommandUtils.cache.Add(type, new Dictionary<string, MethodInfo>());
		}
		Dictionary<string, MethodInfo> dictionary = CommandUtils.cache[type];
		MethodInfo methodInfo;
		if (!dictionary.ContainsKey(methodName))
		{
			methodInfo = type.GetMethod(methodName);
			if (methodInfo != null && !methodInfo.IsDefined(typeof(CommandAttribute), true))
			{
				throw new Exception(string.Format("Method '{0}' has been found on object '{1}' but does not have a [Command] attribute", methodName, targetObject));
			}
			dictionary.Add(methodName, methodInfo);
		}
		else
		{
			methodInfo = dictionary[methodName];
		}
		return methodInfo;
	}
}
