using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;

public class NativeToCSCallback
{
	private class MethodData
	{
		public MethodInfo MethodInfo;

		public List<Type> ParameterTypes = new List<Type>();
	}

	public delegate void CSCallbackDelegate(string classAndMethod, IntPtr ptrParams, int paramLength);

	protected const string DLL_ID = "__Internal";

	private static Dictionary<string, MethodData> m_MethodLookup = new Dictionary<string, MethodData>();

	public static void Initialise()
	{
	}

	[MonoPInvokeCallback(typeof(CSCallbackDelegate))]
	public static void EntrypointCallback(string classAndMethod, IntPtr ptrParams, int paramLength)
	{
		List<string> list = new List<string>();
		IntPtr[] array = new IntPtr[paramLength];
		if (ptrParams != IntPtr.Zero)
		{
			Marshal.Copy(ptrParams, array, 0, paramLength);
			IntPtr[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				IntPtr ptr = array2[i];
				string item = Marshal.PtrToStringAuto(ptr);
				list.Add(item);
			}
		}
		if (LookupMethod(classAndMethod))
		{
			Invoke(classAndMethod, list);
		}
	}

	private static bool LookupMethod(string classAndMethod)
	{
		if (m_MethodLookup.ContainsKey(classAndMethod))
		{
			return true;
		}
		string[] array = classAndMethod.Split(new char[]
		{
			':'
		});
		if (array.Length != 2)
		{
			return false;
		}
		string cname = array[0];
		string mname = array[1];
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		Assembly[] array2 = assemblies;
		for (int i = 0; i < array2.Length; i++)
		{
			Assembly assembly = array2[i];
			if (LookupAssembly(assembly, cname, mname, classAndMethod))
			{
				return true;
			}
		}
		return false;
	}

	private static bool LookupAssembly(Assembly assembly, string cname, string mname, string classAndMethod)
	{
		Type type = assembly.GetType(cname);
		if (type == null)
		{
			return false;
		}
		MethodData methodData = new MethodData();
		methodData.MethodInfo = type.GetMethod(mname, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (methodData.MethodInfo == null)
		{
			return false;
		}
		ParameterInfo[] parameters = methodData.MethodInfo.GetParameters();
		for (int i = 0; i < parameters.Length; i++)
		{
			ParameterInfo parameterInfo = parameters[i];
			methodData.ParameterTypes.Add(parameterInfo.ParameterType);
		}
		m_MethodLookup.Add(classAndMethod, methodData);
		return true;
	}

	private static void Invoke(string classAndMethod, List<string> paramStrings)
	{
		MethodData methodData = m_MethodLookup[classAndMethod];
		List<object> list = new List<object>();
		if (paramStrings.Count != methodData.ParameterTypes.Count)
		{
			return;
		}
		for (int i = 0; i < paramStrings.Count; i++)
		{
			string text = paramStrings[i];
			Type type = methodData.ParameterTypes[i];
			if (type == typeof(bool))
			{
				bool flag = text == "1";
				list.Add(flag);
			}
			else
			{
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				if (!converter.CanConvertTo(type))
				{
					return;
				}
				object item = converter.ConvertFromString(text);
				list.Add(item);
			}
		}
		methodData.MethodInfo.Invoke(null, list.ToArray());
	}
}
