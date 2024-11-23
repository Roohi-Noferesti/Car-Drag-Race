using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProgressionMapPinsConfiguration:ScriptableObject
{
	public List<ProgressionMapPinsData> MapPins = new List<ProgressionMapPinsData>();
}
