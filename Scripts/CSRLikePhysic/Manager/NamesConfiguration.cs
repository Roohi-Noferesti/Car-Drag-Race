using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NamesConfiguration:ScriptableObject
{
	public List<string> DisplayNames;
    public List<string> EnglishNames;
    public List<string> PersianArabicNames;
    public List<string> ChineseNames;
}
