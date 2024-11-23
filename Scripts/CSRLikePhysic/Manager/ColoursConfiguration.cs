using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColoursConfiguration:ScriptableObject
{
	public Dictionary<string, Color> TierColours = new Dictionary<string, Color>();
}
