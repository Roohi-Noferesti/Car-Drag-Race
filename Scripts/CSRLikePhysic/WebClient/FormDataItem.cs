using System;
using UnityEngine;

public class FormDataItem
{
	public string Name
	{
		get;
		private set;
	}

	public string Value
	{
		get;
		private set;
	}

	public FormDataItem(string name, string value)
	{
		this.Name = WWW.EscapeURL(name);
		this.Value = WWW.EscapeURL(value);
	}

	public void SetValue(string data)
	{
		Value = data;
	}
}
