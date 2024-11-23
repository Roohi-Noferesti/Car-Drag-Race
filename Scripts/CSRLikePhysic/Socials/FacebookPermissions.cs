using System;
using System.Collections.Generic;

public class FacebookPermissions
{
	private Dictionary<string, bool> m_permissions = new Dictionary<string, bool>();

	public FacebookPermissions()
	{
	}

	public FacebookPermissions(JsonDict jsonResponce)
	{
		JsonList jsonList = jsonResponce.GetJsonList("data");
		for (int i = 0; i < jsonList.Count; i++)
		{
			JsonDict jsonDict = jsonList.GetJsonDict(i);
			string @string = jsonDict.GetString("permission");
			bool value = jsonDict.GetString("status") == "granted";
			if (!string.IsNullOrEmpty(@string))
			{
				this.m_permissions.Add(@string, value);
			}
		}
	}

	public Dictionary<string, bool> GetPermissions()
	{
		return new Dictionary<string, bool>(this.m_permissions);
	}

	public bool Granted(string permision)
	{
		bool result = false;
		this.m_permissions.TryGetValue(permision, out result);
		return result;
	}

	public bool Granted(List<string> permisions)
	{
		return permisions.TrueForAll((string p) => this.Granted(p));
	}

	public override string ToString()
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, bool> current in this.m_permissions)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Permission: '",
				current.Key,
				"' Granted: '",
				current.Value,
				"'",
				Environment.NewLine
			});
		}
		return text;
	}
}
